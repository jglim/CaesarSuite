using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    // FIXME: no idea where this goes, but it definitely exists
    public class FlashIdentServiceInfo
    {
        public int Unk1;
        public int Unk2;
        public int Unk3;
        public int Unk4;

        public long BaseAddress;
        // 2,     4, 4, 4, 4
        // also see DIGetFlashAreaIdentServiceInfoByIndex
        public FlashIdentServiceInfo(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitFlags = reader.ReadUInt16();
            // some/any of below types could also be caesar-strings
            Unk1 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @1
            Unk2 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @2
            Unk3 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @3
            Unk4 = CaesarReader.ReadBitflagInt32(ref bitFlags, reader); // @4
        }
    }
}

