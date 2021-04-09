using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caesar;
using SAE.J2534;

namespace Diogenes
{
    public partial class BlockDownload : Form
    {
        List<FlashBlock> FlashBlocks = new List<FlashBlock>();
        ECUConnection connection;
        public BlockDownload(ECUConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
        }

        private void BlockDownload_Load(object sender, EventArgs e)
        {

        }

        private void dgvMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dgvMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                TryLoadBlock(file);
            }
            FlashBlocks = FlashBlocks.OrderBy(x => x.Address).ToList();
            PresentRows();
        }

        private void TryLoadBlock(string fileName) 
        {
            string[] fileNameChunks = Path.GetFileNameWithoutExtension(fileName).Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (fileNameChunks.Length > 0)
            {
                if (uint.TryParse(fileNameChunks[fileNameChunks.Length - 1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture,  out uint result))
                {
                    if (FlashBlocks.FindAll(x => x.Path == fileName).Count > 0)
                    {
                        MessageBox.Show($"Skipping repeated block : {fileName}");
                    }
                    else 
                    {
                        FlashBlocks.Add(new FlashBlock { Address = result, Payload = File.ReadAllBytes(fileName), Path = fileName });
                    }
                }
                else
                {
                    MessageBox.Show($"Could not load {fileName} : unable to parse destination address. Please append a valid hex-value to the filename to specify the address (e.g. file_1FFFFF.bin)");
                }
            }
            else 
            {
                MessageBox.Show($"Could not load {fileName} : unable to parse name. Please append a valid hex-value to the filename to specify the address (e.g. file_1FFFFF.bin)");
            }
        }


        private void PresentRows()
        {
            if (FlashBlocks is null)
            {
                return;
            }

            DataTable dt = new DataTable();

            foreach (string header in new string[] { "Block #", "Name", "Address" })
            {
                dt.Columns.Add(header, typeof(string));
            }
            dgvMain.DataSource = dt;

            for (int i = 0; i < FlashBlocks.Count; i++)
            {
                dt.Rows.Add(
                    i.ToString(),
                    Path.GetFileNameWithoutExtension(FlashBlocks[i].Path),
                    $"{FlashBlocks[i].Address:X}"
                    );
            }

            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvMain.Columns[0].ReadOnly = true;
            dgvMain.Columns[1].ReadOnly = true;
            dgvMain.Columns[2].ReadOnly = true;

            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Please note that the STMin and BS values are hardcoded. This process will also take some time. Continue?", "Download", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            {
                StartDownload();
            }
        }

        private void StartDownload() 
        {
            int sumBytesTransferred = 0;
            int sumBytes = 0;
            foreach (FlashBlock block in FlashBlocks)
            {
                sumBytes += block.Payload.Length;
            }

            GenericLoader loader = new GenericLoader();
            loader.Text = "Reading from ECU";
            loader.TopMost = true;
            loader.Show();
            Application.DoEvents();

            foreach (FlashBlock block in FlashBlocks)
            {
                byte[] startDownloadCmd = CreateDownloadRequest(block.Address, (uint)block.Payload.Length);

                byte[] response = connection.SendMessage(startDownloadCmd); // comment this, uncomment below if dryrun
                //byte[] response = BitUtility.BytesFromHex("74 20 00 03");
                if (response.Length < 2)
                {
                    MessageBox.Show($"Error: ECU sent an incorrectly sized response when initiating download for block {Path.GetFileNameWithoutExtension(block.Path)}");
                    break;
                }
                if (response[0] != startDownloadCmd[0] + 0x40)
                {
                    MessageBox.Show($"Error: ECU sent a non-positive response ({BitUtility.BytesToHex(response)}) for block {Path.GetFileNameWithoutExtension(block.Path)}");
                    break;
                }
                // read alfid to know memory size description's width
                byte rxAlfid = response[1];
                // we are only expecting a size value
                if ((rxAlfid & 0xF) > 0)
                {
                    MessageBox.Show($"Error: Could not understand ECU ALFID response ({BitUtility.BytesToHex(response)}) for block {Path.GetFileNameWithoutExtension(block.Path)}");
                    break;
                }
                // find out how wide the memory size description is
                int memSize = rxAlfid >> 4;
                List<byte> memBytes = response.Skip(2).Take(memSize).ToList();
                if (memBytes.Count != memSize)
                {
                    MessageBox.Show($"Error: Could not understand ECU ALFID response (size mismatch) ({BitUtility.BytesToHex(response)}) for block {Path.GetFileNameWithoutExtension(block.Path)}");
                    break;
                }
                // parse the memory size desc as a BE integer
                uint messageSize = 0;
                for (int i = 0; i < memBytes.Count; i++)
                {
                    messageSize <<= 8;
                    messageSize |= memBytes[i];
                }
                // debug: uncomment to cap msg size at 0x100, normally 0xF02, blocksize = 0xF00
                // messageSize = 0x102;
                int blockSize = (int)messageSize - 2;
                int totalBlockCount = (int)(block.Payload.Length / blockSize);
                int remainderBytes = block.Payload.Length % blockSize;
                if (remainderBytes != 0)
                {
                    totalBlockCount++;
                }

                // reconfigure connection for block transfer
                connection.TesterPresentTimer.Enabled = false;
                RaiseSTMin();

                byte[] dataBlock = new byte[messageSize];

                loader.SetProgressMax(totalBlockCount);

                for (int blockCounter = 0; blockCounter < totalBlockCount; blockCounter++)
                {
                    dataBlock[0] = 0x36;
                    dataBlock[1] = (byte)((blockCounter + 1) & 0xFF);

                    // if this is the last block, and there is a remainder block, write only the remainder
                    if (((blockCounter + 1) == totalBlockCount) && (remainderBytes != 0))
                    {
                        Array.ConstrainedCopy(block.Payload, blockCounter * blockSize, dataBlock, 2, remainderBytes);
                        dataBlock = dataBlock.Take(2 + remainderBytes).ToArray();
                    }
                    else
                    {
                        Array.ConstrainedCopy(block.Payload, blockCounter * blockSize, dataBlock, 2, blockSize);
                    }

                    byte[] transferResponse = connection.SendMessage(dataBlock); // comment this, uncomment below if dryrun
                    //byte[] transferResponse = new byte[] { 0x76, 0x00 };
                    if ((transferResponse.Length > 0) && (transferResponse[0] == 0x76))
                    {
                        // no need to check block index, if the ecu accepts it, continue
                    }
                    else
                    {
                        MessageBox.Show($"Error: Data transfer rejected by ECU ({BitUtility.BytesToHex(transferResponse)}) for block {Path.GetFileNameWithoutExtension(block.Path)}");
                        break;
                    }

                    // update progress
                    sumBytesTransferred += dataBlock.Length - 2;
                    // loader.Text = $"Writing to ECU : 0x{sumBytesTransferred:X8} of 0x{sumBytes:X8}";
                    loader.Text = $"Writing to ECU : block {blockCounter} of 0x{totalBlockCount}";
                    loader.SetProgressValue(blockCounter);
                    Application.DoEvents();
                    // fixme: show two progressbars, one for the block# and another for the individual block's %
                }

                byte[] exitTransferResponse = connection.SendMessage(new byte[] { 0x37 });// ------------------ patch if debugging
                if (!((exitTransferResponse.Length > 0) && (exitTransferResponse[0] == 0x77)))
                {
                    MessageBox.Show($"Error: Exit transfer rejected by ECU ({BitUtility.BytesToHex(exitTransferResponse)}) for block {Path.GetFileNameWithoutExtension(block.Path)}");
                    break;
                }

            }

            // restore prior connection status
            connection.TesterPresentTimer.Enabled = true;
            ResetSTMin();

            loader.Close();
            MessageBox.Show("Download completed");
        }


        // WARNING: this bit here is hardcoded; should pull the correct values from the connection
        private void RaiseSTMin()
        {
            List<SConfig> sconfigList = new List<SConfig>();
            sconfigList.Add(new SConfig(Parameter.STMIN_TX, 65535));
            // todo: check if only STMIN_TX can be raised without affecting the other values
            sconfigList.Add(new SConfig(Parameter.ISO15765_STMIN, 20));
            sconfigList.Add(new SConfig(Parameter.ISO15765_BS, 8));
            connection.ConnectionChannel.SetConfig(sconfigList.ToArray());
        }
        private void ResetSTMin()
        {
            List<SConfig> sconfigList = new List<SConfig>();
            sconfigList.Add(new SConfig(Parameter.STMIN_TX, 20));
            sconfigList.Add(new SConfig(Parameter.ISO15765_STMIN, 20));
            sconfigList.Add(new SConfig(Parameter.ISO15765_BS, 8));
            connection.ConnectionChannel.SetConfig(sconfigList.ToArray());
        }

        private byte[] CreateDownloadRequest(uint address, uint size)
        {
            byte[] prefix = BitUtility.BytesFromHex(txtIOPrefixHex.Text.ToUpper().Replace(" ", ""));
            int addressWidth = (int)nudAddressWidth.Value;
            int payloadWidth = (int)nudPayloadWidth.Value;

            byte alfid = GetAddressAndLengthFormatIdentifier(addressWidth, payloadWidth);
            List<byte> command = new List<byte>(prefix);
            command.Add(alfid);
            command.AddRange(ValueToBEByteArrayConstrained(address, addressWidth));
            command.AddRange(ValueToBEByteArrayConstrained(size, payloadWidth));
            return command.ToArray();
        }

        // alfid generation copied fro UDS hex editor, (temporarily) preferring to duplicate to avoid coupling
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

        // also from uds hex editor
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

        private void btnAddBlock_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a BIN File";
            ofd.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryLoadBlock(ofd.FileName);
            }
            FlashBlocks = FlashBlocks.OrderBy(x => x.Address).ToList();
            PresentRows();
        }

        private void btnRemoveBlocks_Click(object sender, EventArgs e)
        {
            FlashBlocks.Clear();
            PresentRows();
        }

        // debug stub, not intended for direct execution
        private void SetSessionStub(ECUConnection Connection) 
        {
            return;
            Console.WriteLine("Entering bootmode");
            Connection.SendMessage(BitUtility.BytesFromHex("1002"));

            Console.WriteLine("Requesting for seed");
            byte[] seed = Connection.SendMessage(BitUtility.BytesFromHex("2705"));
            SecurityAccess.SecurityAutoLogin.QueryUnlockEcu(seed.Skip(2).ToArray(), "X_4", 5, out byte[] key);

            Console.Write("Authenticating for bootmode.. ");
            List<byte> keyResponse = new List<byte>(new byte[] { 0x27, 0x06 });
            keyResponse.AddRange(key);
            byte[] authResponse = Connection.SendMessage(keyResponse);
            Console.WriteLine(BitUtility.BytesToHex(authResponse));

            // remind myself to write fingerprint
            Console.WriteLine($"2EF15A 00 0004 150213 00000000\r\n2EF15A 01 0004 150213 00000000\r\n3101FF00");
            /*
            int blockId = 1; // block0: CODE,  block1: DATA
            Console.WriteLine($"Writing fingerprint");
            //  2e f1 5a 00 00 04 15 02 16 00 00 00 00
            Connection.SendMessage(BitUtility.BytesFromHex($"2EF15A {blockId:X2} 0004 150213 00000000"));

            // 2EF15A 00 0004 150213 00000000
            // 2EF15A 01 0004 150213 00000000

            Console.WriteLine($"Erasing block {blockId}");
            Connection.SendMessage(BitUtility.BytesFromHex("3101FF00"));
            Console.WriteLine("Ready to accept flash data");
            */
        }

        private void btnExportAsMono_Click(object sender, EventArgs e)
        {
            // fixme: buffer allocation
            // on a good day, the base is 0, however if it's significantly offset, this won't work out of the box
            // button is currently set as invisible until this is properly fixed
            byte[] finalBuffer = new byte[0x200000];
            foreach (FlashBlock b in FlashBlocks) 
            {
                Array.ConstrainedCopy(b.Payload, 0, finalBuffer, (int)b.Address, b.Payload.Length);
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your BIN file";
            sfd.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, finalBuffer);
            }
        }
    }

    public class FlashBlock 
    {
        public string Path;
        public byte[] Payload;
        public uint Address;
    }
}
