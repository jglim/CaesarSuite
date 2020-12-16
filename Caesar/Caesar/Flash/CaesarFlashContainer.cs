using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class CaesarFlashContainer
    {
        public FlashHeader CaesarFlashHeader;
        public CTFHeader CaesarCTFHeader;

        public byte[] FileBytes = new byte[] { };
        public CaesarFlashContainer(byte[] fileBytes)
        {
            FileBytes = fileBytes;
            // from DIOpenCFF
            using (BinaryReader reader = new BinaryReader(new MemoryStream(fileBytes)))
            {
                byte[] header = reader.ReadBytes(StubHeader.StubHeaderSize);

                int cffHeaderSize = reader.ReadInt32();
                byte[] cffHeaderData = reader.ReadBytes(cffHeaderSize);

                uint computedChecksum = CaesarReader.ComputeFileChecksumLazy(fileBytes);
                uint providedChecksum = ReadFileChecksum(fileBytes);

                if (computedChecksum != providedChecksum)
                {
                    Console.WriteLine($"WARNING: Checksum mismatch : computed/provided: {computedChecksum:X8}/{providedChecksum:X8}");
                }
                ReadFlashCFF(reader); // fix this
                ReadCTF(reader);
            }
        }
        void ReadCTF(BinaryReader fileReader)
        {
            if (CaesarFlashHeader.CTFHeaderTable == 0)
            {
                throw new NotImplementedException("No idea how to handle nonexistent ctf header");
            }
            long ctfOffset = CaesarFlashHeader.BaseAddress + CaesarFlashHeader.CTFHeaderTable;
            CaesarCTFHeader = new CTFHeader(fileReader, ctfOffset, CaesarFlashHeader.CffHeaderSize);
        }


        public uint ReadFileChecksum(byte[] fileBytes)
        {
            return BitConverter.ToUInt32(fileBytes, fileBytes.Length - 4);
        }

        void ReadFlashCFF(BinaryReader fileReader)
        {
            CaesarFlashHeader = new FlashHeader(fileReader);
        }

        public static void ExportCFFMemorySegments(string filePath) 
        {
            string directory = Path.GetDirectoryName(filePath);

            Console.WriteLine($"Starting CFF segment export. Assumes that segments are embedded and unprotected.");
            byte[] flashContainer = File.ReadAllBytes(filePath);
            CaesarFlashContainer container = new CaesarFlashContainer(flashContainer);


            using (BinaryReader reader = new BinaryReader(new MemoryStream(flashContainer)))
            {
                foreach (FlashDataBlock db in container.CaesarFlashHeader.DataBlocks)
                {
                    Console.WriteLine($"FlashDataBlock: {db.Qualifier}");
                    long fileCursor = 0;
                    foreach (FlashSegment seg in db.FlashSegments)
                    {
                        long offset = 
                            db.FlashData + 
                            container.CaesarFlashHeader.CffHeaderSize +
                            container.CaesarFlashHeader.LanguageBlockLength + 
                            fileCursor + 
                            0x414;

                        fileCursor += seg.SegmentLength;
                        Console.WriteLine($"Segment: {seg.SegmentName} mapped to 0x{seg.FromAddress:X} with size 0x{seg.SegmentLength:X}");
                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        byte[] fileBytes = reader.ReadBytes(seg.SegmentLength);

                        File.WriteAllBytes($"{directory}\\{db.Qualifier}_{seg.FromAddress:X}.bin", fileBytes);
                    }
                }
            }
            Console.WriteLine($"Exported segments can be found at {directory}");
        }

        public void SpliceCFFFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            Console.WriteLine($"Starting CFF splicer..");
            byte[] flashContainer = File.ReadAllBytes(filePath);
            CaesarFlashContainer container = new CaesarFlashContainer(flashContainer);

            using (BinaryReader reader = new BinaryReader(new MemoryStream(flashContainer)))
            {
                foreach (FlashDataBlock db in container.CaesarFlashHeader.DataBlocks)
                {
                    Console.WriteLine($"FlashDataBlock: {db.Qualifier}");
                    long fileCursor = 0;
                    foreach (FlashSegment seg in db.FlashSegments)
                    {
                        // check: which fields are mutable when splicing

                        long offset =
                            db.FlashData + // somewhat mutable : probably if there's more than 1 datablock, this value will be nonzero
                            container.CaesarFlashHeader.CffHeaderSize + // constant
                            container.CaesarFlashHeader.LanguageBlockLength + // constant
                            fileCursor + // mutable, see below
                            0x414; // constant

                        fileCursor += seg.SegmentLength; // mutable because of segment length

                        Console.WriteLine($"Segment: {seg.SegmentName} mapped to 0x{seg.FromAddress:X} with size 0x{seg.SegmentLength:X}");
                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        byte[] fileBytes = reader.ReadBytes(seg.SegmentLength);

                        File.WriteAllBytes($"{directory}\\{db.Qualifier}_{seg.FromAddress:X}.bin", fileBytes);
                    }
                }
            }
            Console.WriteLine($"Exported segments can be found at {directory}");
        }

    }
}
