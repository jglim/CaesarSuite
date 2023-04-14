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
        // 0x04 [4,4,4,4,4,4,4,2,2,2,2,1,1,1,1,1,5,1,1,1,1,4,4,4,4,4],

        public int UnkBufferSize;

        public byte[] UnkBuffer;
        public int Unk3;
        public int Unk4;
        public int Unk5;
        public string VendorName;

        public int KwpVendorID;
        public int Unk8;
        public int Unk9;
        public int Unk10;

        public int Unk11;
        public int Unk12;
        public int Unk13;
        public int Unk14;
        public int Unk15;

        public byte[] EcuId;

        public int Unk17;
        public int Unk18;
        public int Unk19;
        public int Unk20;

        public string Unk21;

        public int Unk22;
        public int Unk23;
        public int UdsVendorID;
        public int PatternType;

        // this doesn't actually exist on the file, but is a handy helper to get the actual variant
        public int VariantID 
        { 
            get { return UdsVendorID == 0 ? KwpVendorID : UdsVendorID; }
        }

        private readonly long BaseAddress;

        public void Restore() 
        {
        
        }

        public ECUVariantPattern() { }

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

            KwpVendorID = CaesarReader.ReadBitflagUInt16(ref bitflags, reader);
            Unk8 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk9 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk10 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            Unk11 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk12 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk13 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk14 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk15 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            EcuId = CaesarReader.ReadBitflagRawBytes(ref bitflags, reader, 4); // @prj's fix : https://github.com/jglim/CaesarSuite/issues/57

            Unk17 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk18 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk19 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk20 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);

            Unk21 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            Unk22 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Unk23 = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            // upper 2 bytes (e.g. 00 02) indicate the expected session? maybe boot: 3, diag: 2
            // lower 2 bytes are for the variant id
            UdsVendorID = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PatternType = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            // type 3 contains a vendor name
            //PrintDebug();
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"UnkBufferSize : {UnkBufferSize}");
            Console.WriteLine($"UnkBuffer : {UnkBuffer}");
            Console.WriteLine($"Unk3 : {Unk3}");
            Console.WriteLine($"Unk4 : {Unk4}");
            Console.WriteLine($"Unk5 : {Unk5}");
            Console.WriteLine($"VendorName : {VendorName}");
            Console.WriteLine($"Unk7 : {KwpVendorID}");
            Console.WriteLine($"Unk8 : {Unk8}");
            Console.WriteLine($"Unk9 : {Unk9}");
            Console.WriteLine($"Unk10 : {Unk10}");
            Console.WriteLine($"Unk11 : {Unk11}");
            Console.WriteLine($"Unk12 : {Unk12}");
            Console.WriteLine($"Unk13 : {Unk13}");
            Console.WriteLine($"Unk14 : {Unk14}");
            Console.WriteLine($"Unk15 : {Unk15}");
            Console.WriteLine($"EcuId : {BitConverter.ToString(EcuId)}");
            Console.WriteLine($"Unk17 : {Unk17}");
            Console.WriteLine($"Unk18 : {Unk18}");
            Console.WriteLine($"Unk19 : {Unk19}");
            Console.WriteLine($"Unk20 : {Unk20}");
            Console.WriteLine($"Unk21 : {Unk21}");
            Console.WriteLine($"Unk22 : {Unk22}");
            Console.WriteLine($"Unk23 : {Unk23}");
            Console.WriteLine($"UdsVendorID : {UdsVendorID}");
            Console.WriteLine($"PatternType : {PatternType}");

            Console.WriteLine($"VariantID : {VariantID} {VariantID:X}");
        }
    }
}
