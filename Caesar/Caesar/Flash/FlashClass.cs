using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashClass
    {
        public string Identifier;
        public string Qualifier;
        public int LongNameCTF;
        public int DescriptionCTF;
        public int UniqueObjectID;

        public long BaseAddress;
        // 2,   4, 4, 4, 4, 4
        public FlashClass(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            Identifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @1
            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @2
            LongNameCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @3
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @4
            UniqueObjectID = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @5
        }
    }
}

