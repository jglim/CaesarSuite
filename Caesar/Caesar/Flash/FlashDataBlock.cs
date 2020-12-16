using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashDataBlock
    {

        // 0x16 [6,  4,4,4,4,  4,4,4,4,  4,4,4,2,  4,4,4,4,  4,4,4,4,  4,4,4,4,4],
        public string Qualifier;
        public int LongName;
        public int Description;
        public int FlashData;
        public int BlockLength;
        public int DataFormat;
        public int FileName;
        public int NumberOfFilters;
        public int FiltersOffset;
        public int NumberOfSegments;
        public int SegmentOffset;
        public int EncryptionMode;
        public int KeyLength;
        public int KeyBuffer;
        public int NumberOfOwnIdents;
        public int IdentsOffset;
        public int NumberOfSecurities;
        public int SecuritiesOffset;
        public string DataBlockType;
        public int UniqueObjectId;
        public string FlashDataInfo_Idk;
        public int FlashDataInfoLang1;
        public int FlashDataInfoLang2;
        public int FlashDataInfo_Idk2;

        public long BaseAddress;
        public List<FlashSegment> FlashSegments = new List<FlashSegment>();

        public FlashDataBlock(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);
            
            ulong bitflags = reader.ReadUInt32();
            reader.ReadUInt16();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress);
            LongName = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Description = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            FlashData = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            BlockLength = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            DataFormat = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            FileName = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            NumberOfFilters = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            FiltersOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            NumberOfSegments = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            SegmentOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            EncryptionMode = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            KeyLength = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            KeyBuffer = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            NumberOfOwnIdents = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            IdentsOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            NumberOfSecurities = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            SecuritiesOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            DataBlockType = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress);
            UniqueObjectId = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            FlashDataInfo_Idk = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress);
            FlashDataInfoLang1 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            FlashDataInfoLang2 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            FlashDataInfo_Idk2 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);


            // CtfUnk1 = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            FlashSegments = new List<FlashSegment>();
            for (int segmentIndex = 0; segmentIndex < NumberOfSegments; segmentIndex++) 
            {
                long segmentEntryAddress = SegmentOffset + BaseAddress + (segmentIndex * 4);
                reader.BaseStream.Seek(segmentEntryAddress, SeekOrigin.Begin);

                long segmentBaseAddress = SegmentOffset + BaseAddress + reader.ReadInt32();

                FlashSegment segment = new FlashSegment(reader, segmentBaseAddress);
                FlashSegments.Add(segment);
            }

        }


        public long GetBlockLengthOffset(BinaryReader reader)
        {
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt32();
            reader.ReadUInt16();

            CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress); // Qualifier 
            CaesarReader.ReadBitflagInt32(ref bitflags, reader); // LongName 
            CaesarReader.ReadBitflagInt32(ref bitflags, reader); // Description 
            CaesarReader.ReadBitflagInt32(ref bitflags, reader); // FlashData 

            if (CaesarReader.CheckAndAdvanceBitflag(ref bitflags))
            {
                return reader.BaseStream.Position;
            }
            else
            {
                return -1;
            }
        }
        public long GetFlashDataOffset(BinaryReader reader)
        {
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt32();
            reader.ReadUInt16();

            CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress); // Qualifier 
            CaesarReader.ReadBitflagInt32(ref bitflags, reader); // LongName 
            CaesarReader.ReadBitflagInt32(ref bitflags, reader); // Description 

            if (CaesarReader.CheckAndAdvanceBitflag(ref bitflags))
            {
                return reader.BaseStream.Position;
            }
            else
            {
                return -1;
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(LongName)} : {LongName}");
            Console.WriteLine($"{nameof(Description)} : {Description}");
            Console.WriteLine($"{nameof(FlashData)} : {FlashData}");
            Console.WriteLine($"{nameof(BlockLength)} : 0x{BlockLength:X}");
            Console.WriteLine($"{nameof(DataFormat)} : {DataFormat}");
            Console.WriteLine($"{nameof(FileName)} : {FileName}");
            Console.WriteLine($"{nameof(NumberOfFilters)} : {NumberOfFilters}");
            Console.WriteLine($"{nameof(FiltersOffset)} : {FiltersOffset}");
            Console.WriteLine($"{nameof(NumberOfSegments)} : {NumberOfSegments}");
            Console.WriteLine($"{nameof(SegmentOffset)} : {SegmentOffset}");
            Console.WriteLine($"{nameof(EncryptionMode)} : {EncryptionMode}");
            Console.WriteLine($"{nameof(KeyLength)} : {KeyLength}");
            Console.WriteLine($"{nameof(KeyBuffer)} : {KeyBuffer}");
            Console.WriteLine($"{nameof(NumberOfOwnIdents)} : {NumberOfOwnIdents}");
            Console.WriteLine($"{nameof(IdentsOffset)} : {IdentsOffset}");
            Console.WriteLine($"{nameof(NumberOfSecurities)} : {NumberOfSecurities}");
            Console.WriteLine($"{nameof(SecuritiesOffset)} : {SecuritiesOffset}");
            Console.WriteLine($"{nameof(DataBlockType)} : {DataBlockType}");
            Console.WriteLine($"{nameof(UniqueObjectId)} : {UniqueObjectId}");
            Console.WriteLine($"{nameof(FlashDataInfo_Idk)} : {FlashDataInfo_Idk}");
            Console.WriteLine($"{nameof(FlashDataInfoLang1)} : {FlashDataInfoLang1}");
            Console.WriteLine($"{nameof(FlashDataInfoLang2)} : {FlashDataInfoLang2}");
            Console.WriteLine($"{nameof(FlashDataInfo_Idk2)} : {FlashDataInfo_Idk2}");

        }
    }
}
