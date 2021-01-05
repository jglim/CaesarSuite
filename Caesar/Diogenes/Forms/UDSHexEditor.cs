using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using System.Windows.Forms.Integration;
using Be.Windows.Forms;
using Caesar;
using System.Globalization;

namespace Diogenes
{
    public partial class UDSHexEditor : Form
    {
        ECUConnection Connection;
        HexBox Hexbox;
        byte[] OriginalBuffer = new byte[] { };
        public UDSHexEditor(ECUConnection connection)
        {
            InitializeComponent();
            Connection = connection;

            // missing docs for HexBox : https://stackoverflow.com/questions/53410380/how-to-create-hex-editor-inside-of-winforms-app
            Hexbox = new HexBox() 
            {
                /*
                UseFixedBytesPerLine = true,
                BytesPerLine = 16,
                */
                ColumnInfoVisible = true,
                LineInfoVisible = true,
                StringViewVisible = true,
                VScrollBarVisible = true
            };
            Hexbox.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(Hexbox);

            Hexbox.LineInfoOffset = 0;
            Hexbox.ByteProvider = new DynamicByteProvider(new byte[] { });
        }

        private void UDSHexEditor_Load(object sender, EventArgs e)
        {

        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!FetchAndValidateInput(out uint sourceAddress, out uint destinationAddress, out uint bufferSize))
            {
                return;
            }

            int strideWidth = decimal.ToInt32(nudIOWidth.Value);
            int addressWidth = decimal.ToInt32(nudAddressWidth.Value);
            int ioDigitCount = GetMemoryDigitCount(strideWidth);
            byte readCmd = decimal.ToByte(nudReadCmd.Value);
            byte positiveResponse = (byte)(readCmd + 0x40);

            byte alfid = GetAddressAndLengthFormatIdentifier(
                        addressWidth,
                        ioDigitCount
                    );


            byte[] memoryBuffer = new byte[bufferSize];
            uint readCursor = sourceAddress;
            int bufferCursor = 0;
            bool hasBadReads = false;

            while (readCursor < destinationAddress)
            {
                uint remainder = destinationAddress - readCursor;
                int readSize = strideWidth;
                if (readSize > remainder) 
                {
                    readSize = (int)remainder;
                }
                List<byte> readCommand = CreateReadCommand(readCmd, alfid, readCursor, addressWidth, readSize);
                //byte[] response = 

                //MessageBox.Show($"cmd: {BitUtility.BytesToHex(readCommand.ToArray(), true)}, width: {readSize}, remainder: {remainder}");
                byte[] response = Connection.SendMessage(readCommand);
                if (response[0] == positiveResponse)
                {
                    Array.ConstrainedCopy(response, 1, memoryBuffer, bufferCursor, readSize);
                }
                else 
                {
                    hasBadReads = true;
                }

                readCursor += (uint)readSize;
                bufferCursor += readSize;
            }

            OriginalBuffer = memoryBuffer;
            /*
            HexboxBuffer = new byte[memoryBuffer.Length];
            Array.ConstrainedCopy(memoryBuffer, 0, HexboxBuffer, 0, memoryBuffer.Length);
            */
            Hexbox.LineInfoOffset = sourceAddress;
            Hexbox.ByteProvider = new DynamicByteProvider(OriginalBuffer);

            if (hasBadReads) 
            {
                MessageBox.Show("One or more read requests were rejected by the ECU, and the empty blocks were substituted with zeros. \r\n\r\n" +
                    "Please check if the memory address is valid to the ECU, and if the ECU has been unlocked if required.\r\n\r\n" +
                    "If the memory region has protected bytes, consider loading a smaller, more specific range with a I/O width of 1 to skip past the protected bytes.", 
                    "Warning: Invalid reads");
            }
        }

        private List<byte> CreateReadCommand(byte readCmd, byte alfid, long sourceAddress, int addressWidth, int ioWidth)
        {
            List<byte> readCommand = new List<byte>();
            readCommand.Add(readCmd); // command
            readCommand.Add(alfid);   // addressing and memory mode
            readCommand.AddRange(ValueToBEByteArrayConstrained(sourceAddress, addressWidth)); // address, constrained to earlier specified width
            readCommand.AddRange(ValueToBEByteArray(ioWidth));
            return readCommand;
        }
        private List<byte> CreateWriteCommand(byte writeCmd, long destAddress, int addressWidth, byte[] payload)
        {
            int ioDigitCount = GetMemoryDigitCount(payload.Length);
            byte alfid = GetAddressAndLengthFormatIdentifier(
                        addressWidth,
                        ioDigitCount

                    );
            List<byte> writeCommand = new List<byte>();
            writeCommand.Add(writeCmd); // command
            writeCommand.Add(alfid);   // addressing and memory mode
            writeCommand.AddRange(ValueToBEByteArrayConstrained(destAddress, addressWidth)); // address, constrained to earlier specified width
            writeCommand.AddRange(ValueToBEByteArray(payload.Length));
            writeCommand.AddRange(payload);
            return writeCommand;
        }


        private List<byte> ValueToBEByteArrayConstrained(long inValue, int size)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < size; i++) 
            {
                byte row = (byte)(inValue & 0xFF);
                result.Insert(0, row);
                inValue >>= 8;
            }
            return result;
        }

        // unconstrained size
        private List<byte> ValueToBEByteArray(long inValue) 
        {
            List<byte> result = new List<byte>();
            while (inValue > 0) 
            {
                byte row = (byte)(inValue & 0xFF);
                result.Insert(0, row);
                inValue >>= 8;
            }
            return result;
        }

        private static int GetMemoryDigitCount(int value) 
        {
            int count = 0;
            while (value > 0) 
            {
                count++;
                value >>= 8;
            }
            return count;
        }

        private static byte GetAddressAndLengthFormatIdentifier(int addressSizeInBytes, int memorySizeInBytes) 
        {
            /*
            address map in bits:
            8 bits  : 1
            16 bits : 2
            24 bits : 3
            32 bits : 4
            40 bits : 5

            memory size map in bits:
            8 bits  : 1
            16 bits : 2
            24 bits : 3
            32 bits : 4
             */
            if ((addressSizeInBytes < 1) || (addressSizeInBytes > 5))
            {
                throw new ArgumentOutOfRangeException("Invalid address size for ALFID");
            }
            if ((memorySizeInBytes < 1) || (memorySizeInBytes > 5))
            {
                throw new ArgumentOutOfRangeException("Invalid memory size for ALFID");
            }
            return (byte)((memorySizeInBytes << 4) | addressSizeInBytes);
        }

        private bool FetchAndValidateInput(out uint sourceAddress_, out uint destinationAddress_, out uint bufferSize) 
        {
            sourceAddress_ = 0;
            destinationAddress_ = 0;
            bufferSize = 0;
            if (!uint.TryParse(txtSource.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint sourceAddress))
            {
                MessageBox.Show($"Please check the source address : could not parse '{txtSource.Text}'");
                return false;
            }
            if (!uint.TryParse(txtDestination.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint destinationAddress))
            {
                MessageBox.Show($"Please check the destination address : could not parse '{txtDestination.Text}'");
                return false;
            }

            if (rbSize.Checked)
            {
                destinationAddress += sourceAddress;
            }
            if (destinationAddress < sourceAddress)
            {
                MessageBox.Show($"Destination position cannot be lower than the source address.");
                return false;
            }

            sourceAddress_ = sourceAddress;
            destinationAddress_ = destinationAddress;
            bufferSize = destinationAddress - sourceAddress;
            const uint sizeLimitMB = 50;
            if (bufferSize > (sizeLimitMB * 1024 * 1024))
            {
                MessageBox.Show($"Buffer size is above {sizeLimitMB}MB. Please read in smaller chunks.");
                return false;
            }
            return true;
        }

        private bool BytearrayEqual(byte[] a, byte[] b) 
        {
            if (a.Length != b.Length) 
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++) 
            {
                if (a[i] != b[i]) 
                {
                    return false;
                } 
            }
            return true;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!FetchAndValidateInput(out uint sourceAddress, out uint destinationAddress, out uint bufferSize))
            {
                return;
            }

            byte[] memoryBuffer = ((DynamicByteProvider)Hexbox.ByteProvider).Bytes.ToArray();

            if (memoryBuffer.Length != OriginalBuffer.Length) 
            {
                MessageBox.Show("Please read the memory region first, before attempting to write.");
            }

            int strideWidth = decimal.ToInt32(nudIOWidth.Value);
            int addressWidth = decimal.ToInt32(nudAddressWidth.Value);
            byte writeCmd = decimal.ToByte(nudWriteCmd.Value);
            byte positiveResponse = (byte)(writeCmd + 0x40);

            uint writeCursor = sourceAddress;
            int bufferCursor = 0;
            bool hasBadWrites = false;

            while (writeCursor < destinationAddress)
            {
                uint remainder = destinationAddress - writeCursor;
                int writeSizeChunk = strideWidth;
                if (writeSizeChunk > remainder)
                {
                    writeSizeChunk = (int)remainder;
                }

                byte[] dataToWrite = new byte[writeSizeChunk];
                byte[] originalData = new byte[writeSizeChunk];
                Array.ConstrainedCopy(memoryBuffer, bufferCursor, dataToWrite, 0, writeSizeChunk);
                // consider checking in-place
                Array.ConstrainedCopy(OriginalBuffer, bufferCursor, originalData, 0, writeSizeChunk);

                // skip writes to unmodified chunks
                if (!BytearrayEqual(dataToWrite, originalData))
                {
                    List<byte> writeCommand = CreateWriteCommand(writeCmd, writeCursor, addressWidth, dataToWrite);
                    // MessageBox.Show($"cmd: {BitUtility.BytesToHex(writeCommand.ToArray(), true)}, width: {writeSizeChunk}, remainder: {remainder}");

                    byte[] response = Connection.SendMessage(writeCommand);
                    if (response[0] != positiveResponse)
                    {
                        hasBadWrites = true;
                    }
                }


                writeCursor += (uint)writeSizeChunk;
                bufferCursor += writeSizeChunk;
            }

            if (hasBadWrites)
            {
                MessageBox.Show("One or more write requests were rejected by the ECU\r\n\r\n" +
                    "Please check if the memory address is valid to the ECU, and if the ECU has been unlocked if required.\r\n\r\n",
                    "Warning: Write command rejected");
            }
            else
            {
                MessageBox.Show("Write complete");
            }
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            byte[] memoryBuffer = ((DynamicByteProvider)Hexbox.ByteProvider).Bytes.ToArray();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your binary data";
            sfd.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, memoryBuffer);
            }
        }
    }
}
