using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;   

namespace Caesar
{
    public class ECUVariantPattern
    {

        public int UnkBufferSize;

        public byte[] UnkBuffer;
        public int Unk3;
        public int Unk4;
        public int Unk5;
        public string VendorName;

        public int Unk7;
        public int Unk8;
        public int Unk9;
        public int Unk10;

        public int Unk11;
        public int Unk12;
        public int Unk13;
        public int Unk14;
        public int Unk15;

        public byte[] Unk16;

        public int Unk17;
        public int Unk18;
        public int Unk19;
        public int Unk20;

        public string Unk21;

        public int Unk22;
        public int Unk23;
        public int VariantID;
        public int PatternType;

        public long BaseAddress;
        public ECUVariantPattern(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();

            UnkBufferSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            UnkBuffer = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, UnkBufferSize, baseAddress);
            Unk3 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk4 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk5 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            VendorName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            Unk7 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk8 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk9 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk10 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            Unk11 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk12 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk13 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk14 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk15 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            Unk16 = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, 5, baseAddress); // read with a constant size

            Unk17 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk18 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk19 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk20 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            Unk21 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            Unk22 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk23 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            VariantID = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PatternType = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            // type 3 contains a vendor name

        }
    }
}
