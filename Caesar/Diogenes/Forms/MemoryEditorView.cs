using Be.Windows.Forms;
using Caesar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes.Forms
{
    public partial class MemoryEditorView : UserControl
    {
        HexBox Hexbox;
        byte[] OriginalBuffer = new byte[] { };
        static readonly string BinFileFormat = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
        bool MemoryDiffCacheIsAvailable = false;
        uint CacheAddress = 0;

        bool TransactionActive = false;

        public MemoryEditorView()
        {
            InitializeComponent();

            // missing docs for HexBox : https://stackoverflow.com/questions/53410380/how-to-create-hex-editor-inside-of-winforms-app
            Hexbox = new HexBox()
            {
                
                UseFixedBytesPerLine = true,
                BytesPerLine = 16,
                
                ColumnInfoVisible = true,
                LineInfoVisible = true,
                StringViewVisible = true,
                VScrollBarVisible = true
            };
            Hexbox.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(Hexbox);

            Hexbox.LineInfoOffset = 0;
            Hexbox.ByteProvider = new DynamicByteProvider(new byte[] { });

            RefreshReadWriteTooltips();
            toolTip1.SetToolTip(chkUseALFID, "This option is normally enabled for UDS devices, and disabled for KW2C3PE devices (e.g. KI211)");
            SetTransactionState(active: false, requiresInvoke: false);
        }



        private List<byte> CreateReadCommand(byte readCmd, byte alfid, long sourceAddress, int addressWidth, int ioWidth, bool includeAlfid)
        {
            List<byte> readCommand = new List<byte>();
            readCommand.Add(readCmd); // command
            if (includeAlfid)
            {
                readCommand.Add(alfid);   // addressing and memory mode
            }
            readCommand.AddRange(ValueToBEByteArray(sourceAddress, addressWidth)); // address, constrained to earlier specified width
            readCommand.AddRange(ValueToBEByteArray(ioWidth)); // should actually be constrained too, based on alfid
            return readCommand;
        }
        private List<byte> CreateWriteCommand(byte writeCmd, long destAddress, int addressWidth, byte[] payload, bool includeAlfid)
        {
            int ioDigitCount = GetMemoryDigitCount(payload.Length);
            byte alfid = GetAddressAndLengthFormatIdentifier(
                        addressWidth,
                        ioDigitCount
                    );
            List<byte> writeCommand = new List<byte>();
            writeCommand.Add(writeCmd); // command
            if (includeAlfid)
            {
                writeCommand.Add(alfid);   // addressing and memory mode
            }
            writeCommand.AddRange(ValueToBEByteArray(destAddress, addressWidth)); // address, constrained to earlier specified width
            writeCommand.AddRange(ValueToBEByteArray(payload.Length)); // should actually be constrained too, based on alfid
            writeCommand.AddRange(payload);
            return writeCommand;
        }


        // converts a long into a big-endian byte array
        private List<byte> ValueToBEByteArray(long inValue, int constrainedSize = -1)
        {
            List<byte> result = new List<byte>();
            if (constrainedSize == -1)
            {
                while (inValue > 0)
                {
                    byte row = (byte)(inValue & 0xFF);
                    result.Insert(0, row);
                    inValue >>= 8;
                }
            }
            else 
            {
                for (int i = 0; i < constrainedSize; i++)
                {
                    byte row = (byte)(inValue & 0xFF);
                    result.Insert(0, row);
                    inValue >>= 8;
                }
            }
            return result;
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

        private bool FetchAndValidateInput(out uint sourceAddress_, out uint destinationAddress_, out uint bufferSize, bool dryRun = false)
        {
            sourceAddress_ = 0;
            destinationAddress_ = 0;
            bufferSize = 0;

            if (!uint.TryParse(txtSrcAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint sourceAddress))
            {
                if (!dryRun) 
                {
                    MessageBox.Show($"Please check the source address : could not parse '{txtSrcAddress.Text}'"); 
                }
                return false;
            }
            if (!uint.TryParse(txtDestAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint destinationAddress))
            {
                if (!dryRun)
                {
                    MessageBox.Show($"Please check the destination address : could not parse '{txtDestAddress.Text}'");
                }
                return false;
            }

            if (rbSize.Checked)
            {
                destinationAddress += sourceAddress;
            }

            if (!dryRun)
            {
                if (destinationAddress < sourceAddress)
                {
                    MessageBox.Show($"Destination position cannot be lower than the source address.");
                    return false;
                }
            }

            sourceAddress_ = sourceAddress;
            destinationAddress_ = destinationAddress;

            // sanity check in case someone accidentally keys an oversized value
            // also going to be upsetting if a long read fails
            if (!dryRun) 
            {
                bufferSize = destinationAddress - sourceAddress;
                const uint sizeLimitMB = 50;
                if (bufferSize > (sizeLimitMB * 1024 * 1024))
                {
                    MessageBox.Show($"Buffer size is above {sizeLimitMB}MB. Please consider reading in smaller chunks.");
                    return false;
                }
            }
            return true;
        }

        private void nudPreview_ValueChanged(object sender, EventArgs e)
        {
            RefreshReadWriteTooltips();
        }

        private void txtSrcAddress_TextChanged(object sender, EventArgs e)
        {
            RefreshReadWriteTooltips();
        }

        private void chkUseALFID_CheckedChanged(object sender, EventArgs e)
        {
            RefreshReadWriteTooltips();
        }

        private static void GatewayWait()
        {
            // this was a fix for ki211 : https://github.com/jglim/CaesarSuite/issues/52
            // in theory the new networking stack would have a gateway delay
            // trying to get the basics in order first, this will be removed later (hopefully without breaking ki211)
            //System.Threading.Thread.Sleep(150);
            Task.Delay(150);
            //Application.DoEvents();
        }

        private void RefreshReadWriteTooltips()
        {
            string readTooltip = "Read from ECU";
            string writeTooltip = "Write to ECU";
            try
            {
                if (!FetchAndValidateInput(out uint sourceAddress, out uint destinationAddress, out uint bufferSize, dryRun: true))
                {
                    Console.WriteLine($"dbg: x");
                    throw new Exception();
                }
                int strideWidth = decimal.ToInt32(nudDataWidth.Value);
                int addressWidth = decimal.ToInt32(nudAddressWidth.Value);
                int ioDigitCount = GetMemoryDigitCount(strideWidth);
                byte readCmd = decimal.ToByte(nudReadRequest.Value);
                byte writeCmd = decimal.ToByte(nudWriteRequest.Value);
                bool useAlfid = chkUseALFID.Checked;

                byte alfid = GetAddressAndLengthFormatIdentifier(
                            addressWidth,
                            ioDigitCount
                        );

                // if the number of bytes to read is lower than the stride width, do not use the stride width
                int actualReadSize = bufferSize < strideWidth ? (int)bufferSize : strideWidth;

                List<byte> readCommand = CreateReadCommand(readCmd, alfid, sourceAddress, addressWidth, actualReadSize, useAlfid);
                readTooltip = BitUtility.BytesToHex(readCommand.ToArray(), true);

                // generate a dummy write request for preview
                byte[] placeholderWriteValue = new byte[strideWidth];
                for (int i = 0; i < placeholderWriteValue.Length; i++) 
                {
                    placeholderWriteValue[i] = (byte)((i & 0xF) | (i & 0xF) << 4);
                }
                List<byte> writeCommand = CreateWriteCommand(writeCmd, sourceAddress, addressWidth, placeholderWriteValue, useAlfid);
                writeTooltip = BitUtility.BytesToHex(writeCommand.ToArray(), true);
            }
            catch 
            {
                // probably a bad hex value, discard and show default tooltip
            }
            toolTip1.SetToolTip(btnEcuRead, readTooltip);
            toolTip1.SetToolTip(btnEcuWrite, writeTooltip);
        }

        private void SetTransactionState(bool active, bool requiresInvoke = true) 
        {
            TransactionActive = active;
            bool uiEnabled = !TransactionActive;

            if (requiresInvoke)
            {
                Invoke((Action)(() =>
                {
                    SetUIEnableState(uiEnabled);
                }));
            }
            else 
            {
                SetUIEnableState(uiEnabled);
            }
        }

        private void SetUIEnableState(bool uiEnabled) 
        {
            btnEcuRead.Enabled = uiEnabled;
            btnEcuWrite.Enabled = uiEnabled;
            btnFileSave.Enabled = uiEnabled;
            btnFileLoad.Enabled = uiEnabled;
            chkUseALFID.Enabled = uiEnabled;
            rbAddress.Enabled = uiEnabled;
            rbSize.Enabled = uiEnabled;
        }

        private void btnEcuRead_Click(object sender, EventArgs e)
        {

            if (DiogenesSharedContext.Singleton.Channel is null) 
            {
                Console.WriteLine($"An active ECU connection is required for this request");
                return;
            }
            Task.Run(() => EcuRead());
        }

        private void EcuRead()
        {
            if (!FetchAndValidateInput(out uint sourceAddress, out uint destinationAddress, out uint bufferSize))
            {
                Console.WriteLine($"Could not understand an address value. Please check the target addresses");
                return;
            }
            int strideWidth = decimal.ToInt32(nudDataWidth.Value);
            int addressWidth = decimal.ToInt32(nudAddressWidth.Value);
            int ioDigitCount = GetMemoryDigitCount(strideWidth);
            byte readCmd = decimal.ToByte(nudReadRequest.Value);
            byte positiveResponse = (byte)(readCmd + 0x40);

            byte alfid = GetAddressAndLengthFormatIdentifier(
                        addressWidth,
                        ioDigitCount
                    );


            byte[] memoryBuffer = new byte[bufferSize];
            uint readCursor = sourceAddress;
            int bufferCursor = 0;
            bool hasBadReads = false;

            SetTransactionState(active: true);

            Invoke((Action)(() =>
            {
                progressBar1.Maximum = (int)(destinationAddress - sourceAddress);
                progressBar1.Value = 0;
            }));
            // read all blocks
            try
            {
                while (readCursor < destinationAddress)
                {
                    Invoke((Action)(() =>
                    {
                        lblProgress.Text = $"Reading : 0x{readCursor:X8}";
                        progressBar1.Value = (int)(readCursor - sourceAddress);
                    }));
                    //Application.DoEvents();

                    uint remainder = destinationAddress - readCursor;
                    int readSize = strideWidth;
                    if (readSize > remainder)
                    {
                        readSize = (int)remainder;
                    }
                    List<byte> readCommand = CreateReadCommand(readCmd, alfid, readCursor, addressWidth, readSize, chkUseALFID.Checked);

                    byte[] response = DiogenesSharedContext.Singleton.Channel.Send(readCommand.ToArray());
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

                    // waiting for a moment here; if there is a gateway (e.g. 500k<->83.3k), let it take a breather
                    GatewayWait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred during the read request: {ex.Message}");
                hasBadReads = true;
            }


            if (hasBadReads)
            {
                MessageBox.Show("One or more read requests were rejected by the ECU, and the empty blocks were substituted with zeros. \r\n\r\n" +
                    "Please check if the memory address is valid to the ECU, and if the ECU has been unlocked if required.\r\n\r\n" +
                    "If the memory region has protected bytes, consider loading a smaller, more specific range with a I/O width of 1 to skip past the protected bytes.",
                    "Warning: Invalid reads");

                // no caching tricks for safety
                MemoryDiffCacheIsAvailable = false;
            }

            Invoke((Action)(() =>
            {
                progressBar1.Value = progressBar1.Maximum;

                // this is a fresh read, our view of the memory is in sync with ecu
                // cache the live state so we can diff for writes later
                MemoryDiffCacheIsAvailable = true;
                CacheAddress = sourceAddress;
                OriginalBuffer = memoryBuffer; 
                Hexbox.LineInfoOffset = sourceAddress;
                Hexbox.ByteProvider = new DynamicByteProvider(OriginalBuffer); // clear and update hex view
                lblProgress.Text = hasBadReads ? $"Read completed with errors" : "Read completed";
            }));
            SetTransactionState(active: false);
        }

        private void btnFileSave_Click(object sender, EventArgs e)
        {
            uint.TryParse(txtSrcAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint sourceAddress);

            string fileName = $"0x{sourceAddress:X8}.bin";
            if (DiogenesSharedContext.Singleton.PrimaryEcu != null) 
            {
                fileName = $"{DiogenesSharedContext.Singleton.PrimaryEcu.Qualifier}_0x{sourceAddress:X8}.bin";
            }
            byte[] memoryBuffer = ((DynamicByteProvider)Hexbox.ByteProvider).Bytes.ToArray();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your binary data";
            sfd.Filter = BinFileFormat;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, memoryBuffer);
            }
        }

        private void btnFileLoad_Click(object sender, EventArgs e)
        {
            if (!uint.TryParse(txtSrcAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint sourceAddress))
            {
                MessageBox.Show($"Please check the source address : could not parse '{txtSrcAddress.Text}'");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Binary File";
            ofd.Filter = BinFileFormat;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // loading from file, we can no longer use caching
                    // we cannot even guarantee filesizes or addresses are matching
                    MemoryDiffCacheIsAvailable = false;


                    // regex: check if file contains a source address, offer to autofill
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(ofd.FileName);
                    string pattern = @"0[xX][0-9a-fA-F]+$";
                    foreach (Match match in Regex.Matches(fileNameNoExtension, pattern))
                    {
                        if (match.Success && match.Groups.Count > 0)
                        {
                            if (uint.TryParse(match.Groups[0].Value.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint regexMatch))
                            {
                                string prompt = $"The file name appears to contain an address (0x{regexMatch:X}), would you like to set the source address to 0x{regexMatch:X} too?";
                                if (MessageBox.Show(prompt, $"Possible address in {fileNameNoExtension}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    txtSrcAddress.Text = $"{regexMatch:X}";
                                    sourceAddress = regexMatch;
                                }
                                break;
                            }
                        }
                    }
                    OriginalBuffer = File.ReadAllBytes(ofd.FileName); 
                    Hexbox.LineInfoOffset = sourceAddress;
                    Hexbox.ByteProvider = new DynamicByteProvider(OriginalBuffer); // reload hex view with file content
                    rbSize.Checked = true;
                    txtDestAddress.Text = $"{OriginalBuffer.Length:X}";
                    Console.WriteLine($"Loaded {OriginalBuffer.Length} bytes from {ofd.FileName}");


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load binary dump: {ex.Message}");
                }
            }
        }

        private void btnEcuWrite_Click(object sender, EventArgs e)
        {
            if (DiogenesSharedContext.Singleton.Channel is null)
            {
                Console.WriteLine($"An active ECU connection is required for this request");
                return;
            }
            Task.Run(() => EcuWrite());
        }


        private void EcuWrite()
        {
            if (!FetchAndValidateInput(out uint sourceAddress, out uint destinationAddress, out uint bufferSize))
            {
                Console.WriteLine($"Could not understand an address value. Please check the target addresses");
                return;
            }

            byte[] hexboxBuffer = ((DynamicByteProvider)Hexbox.ByteProvider).Bytes.ToArray();

            // if our hexbox length is desync'd from original, don't use the cache
            bool cachingReallyAvailable = hexboxBuffer.Length == OriginalBuffer.Length;

            // if caching was disabled by a prior function, don't use it for this run
            cachingReallyAvailable &= MemoryDiffCacheIsAvailable;

            // if cache's address is different, discard the cache
            cachingReallyAvailable &= (CacheAddress == sourceAddress);


            int strideWidth = decimal.ToInt32(nudDataWidth.Value);
            int addressWidth = decimal.ToInt32(nudAddressWidth.Value);
            byte writeCmd = decimal.ToByte(nudWriteRequest.Value);
            byte positiveResponse = (byte)(writeCmd + 0x40);

            uint writeCursor = sourceAddress;
            int bufferCursor = 0;
            bool hasBadWrites = false;

            SetTransactionState(active: true);

            Invoke((Action)(() =>
            {
                progressBar1.Maximum = (int)(destinationAddress - sourceAddress);
                progressBar1.Value = 0;
            }));
            
            // write all blocks
            try
            {
                while (writeCursor < destinationAddress)
                {
                    Invoke((Action)(() =>
                    {
                        lblProgress.Text = $"Writing : 0x{writeCursor:X8}";
                        progressBar1.Value = (int)(writeCursor - sourceAddress);
                    }));

                    // check how many bytes left to write, size is required for write cmd
                    uint remainder = destinationAddress - writeCursor;
                    int writeSizeChunk = strideWidth;
                    if (writeSizeChunk > remainder)
                    {
                        writeSizeChunk = (int)remainder;
                    }

                    // copy current data from hexbox
                    byte[] dataToWrite = new byte[writeSizeChunk];
                    Array.ConstrainedCopy(hexboxBuffer, bufferCursor, dataToWrite, 0, writeSizeChunk);

                    bool willWriteblock = true;

                    // if caching available, check if we can skip this block if data is identical
                    if (cachingReallyAvailable)
                    {
                        byte[] originalData = new byte[writeSizeChunk];
                        Array.ConstrainedCopy(OriginalBuffer, bufferCursor, originalData, 0, writeSizeChunk);
                        willWriteblock = !BytearrayEqual(dataToWrite, originalData);
                    }
                    
                    // send write request (cache miss)
                    if (willWriteblock)
                    {
                        List<byte> writeCommand = CreateWriteCommand(writeCmd, writeCursor, addressWidth, dataToWrite, chkUseALFID.Checked);

                        byte[] response = DiogenesSharedContext.Singleton.Channel.Send(writeCommand.ToArray());
                        if (response[0] != positiveResponse)
                        {
                            hasBadWrites = true;
                        }
                    }
                    writeCursor += (uint)writeSizeChunk;
                    bufferCursor += writeSizeChunk;


                    // waiting for a moment here; if there is a gateway (e.g. 500k<->83.3k), let it take a breather
                    GatewayWait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred during the write request: {ex.Message}");
                hasBadWrites = true;
            }


            if (hasBadWrites)
            {
                MessageBox.Show("One or more write requests were rejected by the ECU\r\n\r\n" +
                    "Please check if the memory address is valid to the ECU, and if the ECU has been unlocked if required.\r\n\r\n",
                    "Warning: Write command rejected");
                // if there was caching, it is no longer usable
                MemoryDiffCacheIsAvailable = false; 
            }

            Invoke((Action)(() =>
            {
                // assuming the write is successful, our view of the memory is now in sync with the ecu
                MemoryDiffCacheIsAvailable = true;
                CacheAddress = sourceAddress;

                progressBar1.Value = progressBar1.Maximum;
                OriginalBuffer = hexboxBuffer;
                Hexbox.LineInfoOffset = sourceAddress;
                Hexbox.ByteProvider = new DynamicByteProvider(OriginalBuffer);
                lblProgress.Text = hasBadWrites ? $"Write completed with errors" : "Write completed";
            }));
            SetTransactionState(active: false);
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
    }
}
