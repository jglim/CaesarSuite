using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashIdent
    {
        public string Qualifier;
        public int LongNameCTF;
        public int DescriptionCTF;
        public int UniqueObjectID;

        public long BaseAddress;
        // 2,     4, 4, 4, 4,   4, 4 
        public FlashIdent(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @1
            LongNameCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @2
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @3

            // FIXME: does this point to FlashIdentServiceInfo?
            int numberOfValues = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @4
            int offsetToValues = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @5

            UniqueObjectID = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @6
        }
    }
}

