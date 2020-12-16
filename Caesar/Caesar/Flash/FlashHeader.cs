using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashHeader
    {
        public int CffHeaderSize;
        public long BaseAddress;

        public string FlashName;
        public string FlashGenerationParams;
        public int Unk3;
        public int Unk4;
        public string FileAuthor;
        public string FileCreationTime;
        public string AuthoringToolVersion;
        public string FTRAFOVersionString;
        public int FTRAFOVersionNumber;
        public string CFFVersionString;
        public int NumberOfFlashAreas;
        public int FlashDescriptionTable;
        public int DataBlockTableCountProbably;
        public int DataBlockRefTable;
        public int CTFHeaderTable;
        public int LanguageBlockLength;
        public int NumberOfECURefs;
        public int ECURefTable;
        public int UnkTableCount;
        public int UnkTableProbably;
        public int Unk15;

        public List<FlashDataBlock> DataBlocks = new List<FlashDataBlock>();
        public List<FlashDescriptionHeader> DescriptionHeaders = new List<FlashDescriptionHeader>();
        // DIIAddCBFFile
        /*
            21 bits active
            f [6, 4,4,4,4, 4,4,4,4, 4,4,4,4, 4,4,4,4, 4,4,4,4, 1],
         */
        public FlashHeader(BinaryReader reader)
        {
            reader.BaseStream.Seek(StubHeader.StubHeaderSize, SeekOrigin.Begin);
            CffHeaderSize = reader.ReadInt32();

            BaseAddress = reader.BaseStream.Position;

            ulong bitFlags = reader.ReadUInt32();

            reader.ReadUInt16(); // unused

            FlashName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FlashGenerationParams = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            Unk3 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            Unk4 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FileAuthor = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FileCreationTime = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            AuthoringToolVersion = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FTRAFOVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FTRAFOVersionNumber = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            CFFVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            NumberOfFlashAreas = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FlashDescriptionTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DataBlockTableCountProbably = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DataBlockRefTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            CTFHeaderTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            LanguageBlockLength = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            NumberOfECURefs = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            ECURefTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            UnkTableCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            UnkTableProbably = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            Unk15 = CaesarReader.ReadBitflagUInt8(ref bitFlags, reader);

            DescriptionHeaders = new List<FlashDescriptionHeader>();
            for (int flashDescIndex = 0; flashDescIndex < NumberOfFlashAreas; flashDescIndex++)
            {
                long flashTableEntryAddress = FlashDescriptionTable + BaseAddress + (flashDescIndex * 4);
                reader.BaseStream.Seek(flashTableEntryAddress, SeekOrigin.Begin);

                long flashEntryBaseAddress = FlashDescriptionTable + BaseAddress + reader.ReadInt32();
                FlashDescriptionHeader fdh = new FlashDescriptionHeader(reader, flashEntryBaseAddress);
                DescriptionHeaders.Add(fdh);
            }

            DataBlocks = new List<FlashDataBlock>();
            for (int dataBlockIndex = 0; dataBlockIndex < DataBlockTableCountProbably; dataBlockIndex++)
            {
                long datablockEntryAddress = DataBlockRefTable + BaseAddress + (dataBlockIndex * 4);
                reader.BaseStream.Seek(datablockEntryAddress, SeekOrigin.Begin);

                long datablockBaseAddress = DataBlockRefTable + BaseAddress + reader.ReadInt32();
                FlashDataBlock fdb = new FlashDataBlock(reader, datablockBaseAddress);
                DataBlocks.Add(fdb);
            }
        }

        public void PrintDebug()
        {

            Console.WriteLine($"{nameof(FlashName)} : {FlashName}");
            Console.WriteLine($"{nameof(FlashGenerationParams)} : {FlashGenerationParams}");
            Console.WriteLine($"{nameof(Unk3)} : {Unk3}");
            Console.WriteLine($"{nameof(Unk4)} : {Unk4}");

            Console.WriteLine($"{nameof(FileAuthor)} : {FileAuthor}");
            Console.WriteLine($"{nameof(FileCreationTime)} : {FileCreationTime}");
            Console.WriteLine($"{nameof(AuthoringToolVersion)} : {AuthoringToolVersion}");
            Console.WriteLine($"{nameof(FTRAFOVersionString)} : {FTRAFOVersionString}");

            Console.WriteLine($"{nameof(FTRAFOVersionNumber)} : {FTRAFOVersionNumber}");
            Console.WriteLine($"{nameof(CFFVersionString)} : {CFFVersionString}");
            Console.WriteLine($"{nameof(NumberOfFlashAreas)} : {NumberOfFlashAreas}");
            Console.WriteLine($"{nameof(FlashDescriptionTable)} : 0x{FlashDescriptionTable:X}");

            Console.WriteLine($"{nameof(DataBlockTableCountProbably)} : {DataBlockTableCountProbably}");
            Console.WriteLine($"{nameof(DataBlockRefTable)} : {DataBlockRefTable}");
            Console.WriteLine($"{nameof(CTFHeaderTable)} : {CTFHeaderTable}");
            Console.WriteLine($"{nameof(LanguageBlockLength)} : {LanguageBlockLength}");

            Console.WriteLine($"{nameof(NumberOfECURefs)} : {NumberOfECURefs}");
            Console.WriteLine($"{nameof(ECURefTable)} : {ECURefTable}");
            Console.WriteLine($"{nameof(UnkTableCount)} : {UnkTableCount}");
            Console.WriteLine($"{nameof(UnkTableProbably)} : 0x{UnkTableProbably:X}");


            Console.WriteLine($"{nameof(Unk15)} : {Unk15}");

        }
    }
}
