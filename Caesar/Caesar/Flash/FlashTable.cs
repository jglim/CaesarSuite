using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashTable
    {
        public int MeaningCTF;
        public string FlashKey;
        public int FlashDescriptionCTF;
        public int SessionIndex;
        public int Priority;
        public int[] FlashClassIndexes = Array.Empty<int>();
        public string FlashService;
        public string Qualifier;
        public int UniqueObjectId;
        public int AccessCode;
        public string[] AllowedECUs = Array.Empty<string>();

        public long BaseAddress;
        // confirmed as: 2,   4, 4, 4, 4,  4, 4, 4, 4,  4, 4, 4, 4,  4

        public FlashTable(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            MeaningCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 6
            FlashKey = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // 2E
            FlashDescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 7
            SessionIndex = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 0
            Priority = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 1E

            int numberOfFlashClasses = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 01
            int offsetToFlashClasses = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 3E

            FlashService = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // 42

            int numberOfAllowedEcus = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 01
            int offsetToAllowedEcus = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // 50

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // 64

            UniqueObjectId = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            AccessCode = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // ref to table addr? not seen yet

            AllowedECUs = new string[numberOfAllowedEcus];
            for (int ecuIndex = 0; ecuIndex < numberOfAllowedEcus; ecuIndex++)
            {
                // stride size confirmed as 12 in DIGetFlashTableAllowedECUByIndex
                long ecuRow = offsetToAllowedEcus + BaseAddress + (ecuIndex * 12); 
                reader.BaseStream.Seek(ecuRow, SeekOrigin.Begin);

                long ecuName = offsetToAllowedEcus + BaseAddress + reader.ReadInt32();
                reader.BaseStream.Seek(ecuName, SeekOrigin.Begin);
                AllowedECUs[ecuIndex] = CaesarReader.ReadStringFromBinaryReader(reader);
            }

            FlashClassIndexes = new int[numberOfFlashClasses];
            reader.BaseStream.Seek(offsetToFlashClasses + BaseAddress, SeekOrigin.Begin);
            for (int flashClassIndex = 0; flashClassIndex < numberOfFlashClasses; flashClassIndex++)
            {
                FlashClassIndexes[flashClassIndex] = reader.ReadInt32();
            }

        }
    }
}

