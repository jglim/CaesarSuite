using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    // FLASH_DESCRIPTION_HEADER
    // 0x10: 4 :  4, 4, 4, 4,  4, 4, 4, 4,  4, 4, 4, 4
    public class FlashDescriptionHeader
    {
        public string Qualifier;
        public int Description;
        public int FlashAreaName;
        public int FlashTableStructureCount;
        public int FlashTableStructureOffset;
        public int NumberOfUploads;
        public int UploadTableRefTable;
        public int NumberOfIdentServices;
        public int IdentServicesOffset;
        public int UniqueObjectID;
        public int unkb;
        public int unkc;
        public long BaseAddress;

        public FlashDescriptionHeader(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong flashBitFlags = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref flashBitFlags, reader, baseAddress);
            Description = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            FlashAreaName = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            FlashTableStructureCount = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            FlashTableStructureOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            NumberOfUploads = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            UploadTableRefTable = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            NumberOfIdentServices = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            IdentServicesOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            UniqueObjectID = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            unkb = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            unkc = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(Description)} : {Description}");
            Console.WriteLine($"{nameof(FlashAreaName)} : {FlashAreaName}");
            Console.WriteLine($"{nameof(FlashTableStructureCount)} : {FlashTableStructureCount}");

            Console.WriteLine($"{nameof(FlashTableStructureOffset)} : 0x{FlashTableStructureOffset:X}");
            Console.WriteLine($"{nameof(NumberOfUploads)} : {NumberOfUploads}");
            Console.WriteLine($"{nameof(UploadTableRefTable)} : {UploadTableRefTable}");
            Console.WriteLine($"{nameof(NumberOfIdentServices)} : {NumberOfIdentServices}");

            Console.WriteLine($"{nameof(IdentServicesOffset)} : {IdentServicesOffset}");
            Console.WriteLine($"{nameof(UniqueObjectID)} : {UniqueObjectID}");
            Console.WriteLine($"{nameof(unkb)} : {unkb}");
            Console.WriteLine($"{nameof(unkc)} : {unkc}");
        }
    }
}
