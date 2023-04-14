using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class FlashSession
    {
        public string Qualifier;
        public int LongNameCTF;
        public int DescriptionCTF;

        public long BaseAddress;
        // 2,    4, 4, 4, 4,   4, 4, 4, 4,   4, 2
        public FlashSession(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, reader, BaseAddress); // @1
            LongNameCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @2
            DescriptionCTF = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @3

            // as far as i can tell, these are all indexes (integers)
            int identCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @4
            int identOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @5

            int securitiesCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @6
            int securitiesOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @7

            int datablocksCount = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @8
            int datablocksOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @9

            int FlashMethod = CaesarReader.ReadBitflagInt16(ref bitFlags, reader); // @10
        }
    }
}

