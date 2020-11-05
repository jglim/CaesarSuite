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

        public int idkBufferSize;

        public byte[] idkBuffer;
        public int idk3;
        public int idk4;
        public int idk5;
        public string VendorName;

        public int idk7;
        public int idk8;
        public int idk9;
        public int idk10;

        public int idk11;
        public int idk12;
        public int idk13;
        public int idk14;
        public int idk15;

        public byte[] idk16;

        public int idk17;
        public int idk18;
        public int idk19;
        public int idk20;

        public string idk21;

        public int idk22;
        public int idk23;
        public int VariantID;
        public int PatternType;

        public long BaseAddress;
        public ECUVariantPattern(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();

            idkBufferSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            idkBuffer = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, idkBufferSize, baseAddress);
            idk3 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            idk4 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            idk5 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            VendorName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            idk7 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            idk8 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            idk9 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            idk10 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            idk11 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk12 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk13 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk14 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk15 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            idk16 = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, 5, baseAddress); // read with a constant size

            idk17 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk18 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk19 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            idk20 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            idk21 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            idk22 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            idk23 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            VariantID = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PatternType = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            // type 3 contains a vendor name

        }
    }
}
