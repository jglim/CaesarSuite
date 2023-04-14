using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashSegment
    {
        public int FromAddress;
        public int SegmentLength;
        public int DataLength; // almost always 0, useful stuff in SegmentLength instead
        public string SegmentName;

        public int LongNameCTF;
        public int DescriptionCTF;
        public string UniqueObjectID;

        //    SEGMENT_TABLE_STRUCTURE  2,   4, 4, 4, 4,  4, 4, 4
        public long BaseAddress;

        public FlashSegment(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            FromAddress = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            SegmentLength = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DataLength = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // DIGetDataBlockSegmentStrings fills up the last 4 entries / caesar reads
            SegmentName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress);
            LongNameCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader);

            // confusing as to why this is a string
            UniqueObjectID = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); 
        }

    }
}
