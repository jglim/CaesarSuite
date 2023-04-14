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
    public class FlashArea
    {
        public string Qualifier;
        public int Description;
        public int FlashAreaName;
        public int UniqueObjectID;

        public long BaseAddress;

        public FlashTable[] FlashTables = Array.Empty<FlashTable>();
        public FlashUpload[] FlashUploads = Array.Empty<FlashUpload>();
        public FlashIdent[] FlashIdents = Array.Empty<FlashIdent>();
        public FlashClass[] FlashClasses = Array.Empty<FlashClass>();

        public FlashArea(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong flashBitFlags = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref flashBitFlags, reader, baseAddress);
            Description = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            FlashAreaName = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            int flashTableCount = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            int flashTableOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            int flashUploadCount = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            int flashUploadOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            int identCount = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            int identsOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            UniqueObjectID = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            int flashClassCount = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);
            int flashClassOffset = CaesarReader.ReadBitflagInt32(ref flashBitFlags, reader);

            // ft
            FlashTables = new FlashTable[flashTableCount];
            for (int flashTableIndex = 0; flashTableIndex < flashTableCount; flashTableIndex++)
            {
                long flashTableEntryAddress = flashTableOffset + BaseAddress + (flashTableIndex * 4);
                reader.BaseStream.Seek(flashTableEntryAddress, SeekOrigin.Begin);

                long flashEntryBaseAddress = flashTableOffset + BaseAddress + reader.ReadInt32();
                FlashTables[flashTableIndex] = new FlashTable(reader, flashEntryBaseAddress);
            }

            // uploads
            FlashUploads = new FlashUpload[flashUploadCount];
            for (int flashUploadIndex = 0; flashUploadIndex < flashUploadCount; flashUploadIndex++)
            {
                long flashUploadEntryAddress = flashUploadOffset + BaseAddress + (flashUploadIndex * 4);
                reader.BaseStream.Seek(flashUploadEntryAddress, SeekOrigin.Begin);

                long uploadBaseAddress = flashUploadOffset + BaseAddress + reader.ReadInt32();
                FlashUploads[flashUploadIndex] = new FlashUpload(reader, uploadBaseAddress);
            }

            // idents
            FlashIdents = new FlashIdent[identCount];
            for (int identIndex = 0; identIndex < identCount; identIndex++)
            {
                long identEntryAddress = identsOffset + BaseAddress + (identIndex * 4);
                reader.BaseStream.Seek(identEntryAddress, SeekOrigin.Begin);

                long identBaseAddress = identsOffset + BaseAddress + reader.ReadInt32();
                FlashIdents[identCount] = new FlashIdent(reader, identBaseAddress);
            }

            // classes
            FlashClasses = new FlashClass[flashClassCount];
            for (int flashClassIndex = 0; flashClassIndex < flashClassCount; flashClassIndex++)
            {
                long flashClassEntryAddress = flashClassOffset + BaseAddress + (flashClassIndex * 4);
                reader.BaseStream.Seek(flashClassEntryAddress, SeekOrigin.Begin);

                long flashClassBaseAddress = flashClassOffset + BaseAddress + reader.ReadInt32();
                FlashClasses[flashClassIndex] = new FlashClass(reader, flashClassBaseAddress); ;
            }
        }
    }
}
