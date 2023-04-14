using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashUpload
    {
        // very similar layout to FlashTable
        // 2,   4, 4, 4, 4,    4, 4, 4, 4,    4, 4, 4, 4,    4
        public int MeaningCTF;
        public string UploadKey;
        public int DescriptionCTF;
        public int SessionIndex;
        public int Priority;
        public string UploadService;
        public string Qualifier;
        public int UniqueObjectID;
        public int AccessCode;


        public int[] FlashClassIndexes = Array.Empty<int>();
        public string[] AllowedECUs = Array.Empty<string>();

        public long BaseAddress;

        public FlashUpload(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            MeaningCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @1
            UploadKey = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @2
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @3
            SessionIndex = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @4
            Priority = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @5

            int numberOfFlashClasses = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @6
            int offsetToFlashClasses = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @7

            UploadService = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @8

            int numberOfAllowedEcus = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @9
            int offsetToAllowedEcus = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @10

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @11
            UniqueObjectID = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @12
            AccessCode = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @13


            AllowedECUs = new string[numberOfAllowedEcus];
            for (int ecuIndex = 0; ecuIndex < numberOfAllowedEcus; ecuIndex++)
            {
                // this assumes that uploads behave like flashtables
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


