using Caesar;
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

namespace Diogenes
{
    public partial class FlashSplicer : Form
    {

        List<List<byte[]>> FlashData = new List<List<byte[]>>();
        CaesarFlashContainer FlashContainer = null;
        List<List<string>> SplicePath = new List<List<string>>();
        List<List<int>> MappedAddresses = new List<List<int>>();
        byte[] FlashBytes = new byte[] { };

        public FlashSplicer()
        {
            InitializeComponent();
        }

        private void openCFFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CFF File";
            ofd.Filter = "CFF files (*.cff)|*.cff|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadCFF(ofd.FileName);
                PresentRows();
            }
        }

        public void LoadCFF(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            FlashBytes = File.ReadAllBytes(filePath);
            FlashContainer = new CaesarFlashContainer(FlashBytes);

            FlashData = new List<List<byte[]>>();
            SplicePath = new List<List<string>>();
            MappedAddresses = new List<List<int>>();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(FlashBytes)))
            {
                foreach (FlashDataBlock db in FlashContainer.CaesarFlashHeader.DataBlocks)
                {
                    long fileCursor = 0;

                    List<byte[]> segmentChunks = new List<byte[]>();
                    List<string> spliceStub = new List<string>();
                    List<int> mapStub = new List<int>();

                    foreach (FlashSegment seg in db.FlashSegments)
                    {
                        long offset =
                            db.FlashData +
                            FlashContainer.CaesarFlashHeader.CffHeaderSize +
                            FlashContainer.CaesarFlashHeader.LanguageBlockLength +
                            fileCursor +
                            0x414;

                        fileCursor += seg.SegmentLength;

                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        byte[] fileBytes = reader.ReadBytes(seg.SegmentLength);

                        segmentChunks.Add(fileBytes);
                        spliceStub.Add("");
                        mapStub.Add(seg.FromAddress);
                    }
                    FlashData.Add(segmentChunks);
                    SplicePath.Add(spliceStub);
                    MappedAddresses.Add(mapStub);
                }
            }

            txtLog.Text = GetLog();
        }

        private string GetLog() 
        {
            FlashHeader header = FlashContainer.CaesarFlashHeader;
            return $"Name\t\t\t: {header.FlashName}\r\n" +
                $"Author\t\t\t: {header.FileAuthor}\r\n" +
                $"Creation Time\t\t: {header.FileCreationTime}\r\n" +
                $"Authoring Tool Version\t: {header.AuthoringToolVersion}\r\n" +
                $"Trafo Version\t\t: {header.FTRAFOVersionString}\r\n" +
                $"CFF Version\t\t: {header.CFFVersionString}\r\n" +
                $"Referenced ECU count\t: {header.NumberOfECURefs}\r\n" +
                $"Generation Parameter:\n {header.FlashGenerationParams}\r\n";
        }

        private void PresentRows()
        {
            if (FlashContainer is null) 
            {
                return;
            }

            DataTable dt = new DataTable();

            foreach (string header in new string[] { "Block #", "Block Name", "Block Description", "Segment #", "Segment Name", "ECU Target Address (Editable)",  "Original Length", "Original Offset", "Splice Mode"})
            {
                dt.Columns.Add(header, typeof(string));
            }
            dgvMain.DataSource = dt;

            for (int i = 0; i < FlashContainer.CaesarFlashHeader.DataBlocks.Count; i++) 
            {
                FlashDataBlock db = FlashContainer.CaesarFlashHeader.DataBlocks[i];
                long fileCursor = 0;

                for (int j = 0; j < db.FlashSegments.Count; j++) 
                {
                    FlashSegment seg = db.FlashSegments[j];

                    long offset =
                        db.FlashData +
                        FlashContainer.CaesarFlashHeader.CffHeaderSize +
                        FlashContainer.CaesarFlashHeader.LanguageBlockLength +
                        fileCursor +
                        0x414;

                    fileCursor += seg.SegmentLength;

                    // Console.WriteLine($"Segment: {seg.SegmentName} mapped to 0x{seg.FromAddress:X} with size 0x{seg.SegmentLength:X}");
                    string spliceModeForRow = SplicePath[i][j].Length == 0 ? "Inherit Original" : SplicePath[i][j];

                    dt.Rows.Add(
                        i.ToString(),  
                        db.Qualifier, 
                        FlashContainer.CaesarCTFHeader.CtfLanguages[0].GetString(db.Description), 
                        j.ToString(), 
                        seg.SegmentName, 
                        seg.FromAddress.ToString("X"), 
                        seg.SegmentLength.ToString("X"), 
                        offset.ToString("X"),
                        spliceModeForRow
                        );
                }
            }

            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvMain.Columns[0].ReadOnly = true;
            dgvMain.Columns[1].ReadOnly = true;
            dgvMain.Columns[2].ReadOnly = true;
            dgvMain.Columns[3].ReadOnly = true;
            dgvMain.Columns[4].ReadOnly = true;
            dgvMain.Columns[5].ReadOnly = false;
            dgvMain.Columns[6].ReadOnly = true;
            dgvMain.Columns[7].ReadOnly = true;
            dgvMain.Columns[8].ReadOnly = false;

            dgvMain.Columns[dgvMain.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void GenericPicker_Load(object sender, EventArgs e)
        {
            PresentRows();
        }

        private void dgvMain_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex > dgvMain.Rows.Count)) 
            {
                return;
            }

            if (e.ColumnIndex == (dgvMain.Columns.Count - 1))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a replacement binary file";
                ofd.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    int blockIndex = int.Parse(dgvMain.Rows[e.RowIndex].Cells[0].Value.ToString());
                    int segmentIndex = int.Parse(dgvMain.Rows[e.RowIndex].Cells[3].Value.ToString());

                    SplicePath[blockIndex][segmentIndex] = ofd.FileName;
                    PresentRows();
                }
                e.Cancel = true;
            }
        }

        private void exportSplicedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FlashContainer is null)
            {
                return;
            }
            if (!LoadMappedAddresses()) 
            {
                return;
            }

            /*
             we need to..
                - get the fileoffsets of the segment sizes
                - get the fileoffsets of the segment addresses
                - get the fileoffset of the flash start address
                
                - patch all the fileoffsets (size+address) with the new value
             */

            int flashDataFileOffset = FlashContainer.CaesarFlashHeader.CffHeaderSize + FlashContainer.CaesarFlashHeader.LanguageBlockLength + 0x414;
            byte[] nonFlashData = FlashBytes.Take(flashDataFileOffset).ToArray();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(FlashBytes, 0, FlashBytes.Length, false, true)))
            using (BinaryWriter nonFlashWriter = new BinaryWriter(new MemoryStream(nonFlashData)))
            using (BinaryWriter flashPayloadWriter = new BinaryWriter(new MemoryStream()))
            {
                int segmentCursor = 0;

                for (int i = 0; i < FlashContainer.CaesarFlashHeader.DataBlocks.Count; i++)
                {
                    FlashDataBlock db = FlashContainer.CaesarFlashHeader.DataBlocks[i];
                    // read block offset fileoffset
                    long blockOffsetFileOffset = db.GetFlashDataOffset(reader);

                    // patch segment length on filebytes
                    nonFlashWriter.BaseStream.Seek(blockOffsetFileOffset, SeekOrigin.Begin);
                    nonFlashWriter.Write(segmentCursor);

                    int localBlockLength = 0;
                    for (int j = 0; j < db.FlashSegments.Count; j++)
                    {
                        FlashSegment seg = db.FlashSegments[j];
                        // check: which fields are mutable when splicing

                        long offset =
                            db.FlashData + // somewhat mutable : probably if there's more than 1 datablock, this value will be nonzero
                            FlashContainer.CaesarFlashHeader.CffHeaderSize + // constant
                            FlashContainer.CaesarFlashHeader.LanguageBlockLength + // constant
                            segmentCursor + // mutable, see below
                            0x414; // constant

                        byte[] segmentPayload = FlashData[i][j];
                        if (SplicePath[i][j].Length > 0) 
                        {
                            segmentPayload = File.ReadAllBytes(SplicePath[i][j]);
                        }

                        int segmentMappedAddress = MappedAddresses[i][j];


                        // read segment length's offset
                        long segmentLengthFileOffset = seg.GetSegmentLengthFileOffset(reader);
                        // read segment's mapped address
                        long segmentMappedAddressFileOffset = seg.GetMappedAddressFileOffset(reader);

                        // patch segment length on filebytes
                        nonFlashWriter.BaseStream.Seek(segmentLengthFileOffset, SeekOrigin.Begin);
                        nonFlashWriter.Write(segmentPayload.Length);
                        // patch segment's mapped address
                        nonFlashWriter.BaseStream.Seek(segmentMappedAddressFileOffset, SeekOrigin.Begin);
                        nonFlashWriter.Write(segmentMappedAddress);

                        // increment segment offset for db.flashdata
                        segmentCursor += segmentPayload.Length;
                        localBlockLength += segmentPayload.Length;

                        // append flash payload to temp buffer
                        flashPayloadWriter.Write(segmentPayload);

                    }

                    // refresh the block size
                    // read block size fileoffset
                    long blockSizeFileOffset = db.GetBlockLengthOffset(reader);

                    // patch block length
                    nonFlashWriter.BaseStream.Seek(blockSizeFileOffset, SeekOrigin.Begin);
                    nonFlashWriter.Write(localBlockLength);
                }


                // take the non-flash part of the bytes that we patched...
                // .. then merge it with the flash section that we rebuilt
                byte[] flashPayload = ((MemoryStream)flashPayloadWriter.BaseStream).ToArray();
                byte[] nonFlashPrefix = ((MemoryStream)nonFlashWriter.BaseStream).ToArray();

                byte[] result = new byte[flashPayload.Length + nonFlashPrefix.Length + 4];
                Array.ConstrainedCopy(nonFlashPrefix, 0, result, 0, nonFlashPrefix.Length);
                Array.ConstrainedCopy(flashPayload, 0, result, nonFlashPrefix.Length, flashPayload.Length);

                // restore the checksum
                uint checksum = CaesarReader.ComputeFileChecksumLazy(result);

                result[result.Length - 4] = (byte)((checksum >> 0) & 0xFF);
                result[result.Length - 3] = (byte)((checksum >> 8) & 0xFF);
                result[result.Length - 2] = (byte)((checksum >> 16) & 0xFF);
                result[result.Length - 1] = (byte)((checksum >> 24) & 0xFF);

                // save the result
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a location to save your new CFF file";
                sfd.Filter = "CFF files (*.cff)|*.cff|All files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, result);
                }
            }


        }

        private bool LoadMappedAddresses() 
        {
            for (int i = 0; i < dgvMain.Rows.Count; i++)
            {
                int blockIndex = int.Parse(dgvMain.Rows[i].Cells[0].Value.ToString());
                int segmentIndex = int.Parse(dgvMain.Rows[i].Cells[3].Value.ToString());

                string inHexString = dgvMain.Rows[i].Cells[5].Value.ToString();
                if (int.TryParse(inHexString, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out int result))
                {
                    MappedAddresses[blockIndex][segmentIndex] = result;
                }
                else
                {
                    MessageBox.Show($"Failed to parse a hex value: {inHexString}");
                    return false;
                }
            }
            return true;
        }
    }
}
