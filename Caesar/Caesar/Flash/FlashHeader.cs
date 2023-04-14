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
        public string CFFTrafoArguments;
        public int NameCTF;
        public int DescriptionCTF;
        public string FileAuthor;
        public string FileCreationTime;
        public string AuthoringToolVersion;
        public string FTRAFOVersionString;
        public int FTRAFOVersionNumber;
        public string CFFVersionString;
        public int NumberOfFlashAreas;
        public int FlashDescriptionTable;
        public int DataBlockTableCount;
        public int DataBlockRefTable;
        public int LanguageHeaderTable;
        public int LanguageBlockLength;
        public int ECURefCount;
        public int ECURefTable;
        public int SessionsCount;
        public int SessionsTable;
        public int CFFIsFromDataBase;

        // we know these sizes aot so maybe avoid lists
        public List<FlashDataBlock> DataBlocks = new List<FlashDataBlock>();
        public List<FlashArea> DescriptionHeaders = new List<FlashArea>();
        public List<FlashSession> Sessions = new List<FlashSession>();
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
            CFFTrafoArguments = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            NameCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FileAuthor = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // FladenAuthor (flatbread?)
            FileCreationTime = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            AuthoringToolVersion = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FTRAFOVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            FTRAFOVersionNumber = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            CFFVersionString = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);

            // parsed
            NumberOfFlashAreas = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            FlashDescriptionTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // parsed
            DataBlockTableCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DataBlockRefTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // parsed .. later
            LanguageHeaderTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            LanguageBlockLength = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // unparsed
            ECURefCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            ECURefTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // parsed .. but not used
            SessionsCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            SessionsTable = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            CFFIsFromDataBase = CaesarReader.ReadBitflagUInt8(ref bitFlags, reader);

            DescriptionHeaders = new List<FlashArea>();
            for (int flashAreaIndex = 0; flashAreaIndex < NumberOfFlashAreas; flashAreaIndex++)
            {
                long flashTableEntryAddress = FlashDescriptionTable + BaseAddress + (flashAreaIndex * 4);
                reader.BaseStream.Seek(flashTableEntryAddress, SeekOrigin.Begin);

                long flashEntryBaseAddress = FlashDescriptionTable + BaseAddress + reader.ReadInt32();
                FlashArea fdh = new FlashArea(reader, flashEntryBaseAddress);
                DescriptionHeaders.Add(fdh);
            }

            DataBlocks = new List<FlashDataBlock>();
            for (int dataBlockIndex = 0; dataBlockIndex < DataBlockTableCount; dataBlockIndex++)
            {
                long datablockEntryAddress = DataBlockRefTable + BaseAddress + (dataBlockIndex * 4);
                reader.BaseStream.Seek(datablockEntryAddress, SeekOrigin.Begin);

                long datablockBaseAddress = DataBlockRefTable + BaseAddress + reader.ReadInt32();
                FlashDataBlock fdb = new FlashDataBlock(reader, datablockBaseAddress);
                DataBlocks.Add(fdb);
            }


            Sessions = new List<FlashSession>();
            for (int sessionIndex = 0; sessionIndex < SessionsCount; sessionIndex++)
            {
                long sessionEntryAddress = SessionsTable + BaseAddress + (sessionIndex * 4);
                reader.BaseStream.Seek(sessionEntryAddress, SeekOrigin.Begin);

                long sessionBaseAddress = SessionsTable + BaseAddress + reader.ReadInt32();
                FlashSession fs = new FlashSession(reader, sessionBaseAddress);
                Sessions.Add(fs);
            }
        }

    }
}
