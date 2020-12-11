using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class Scale
    {
        public long BaseAddress;

        // 0x0b [2,   4,4,4,4,    4,4,4,4,   4,4,4],

        public int Unk1;
        public int Unk2;
        public int PrepLowBound;
        public int PrepUpBound;

        public float MultiplyFactor;
        public float AddConstOffset;

        public int SICount;
        public int OffsetSI;

        public int USCount;
        public int OffsetUS;

        public int UnkB;
        public int UnkC;

        public Scale(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt16();

            Unk1 = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // typically qualifier
            Unk2 = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // typically description

            PrepLowBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // could be float
            PrepUpBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // could be float

            MultiplyFactor = CaesarReader.ReadBitflagFloat(ref bitflags, reader);
            AddConstOffset = CaesarReader.ReadBitflagFloat(ref bitflags, reader);

            SICount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            OffsetSI = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            USCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            OffsetUS = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            UnkB = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            UnkC = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(Unk1)} : {Unk1}");
            Console.WriteLine($"{nameof(Unk2)} : {Unk2}");
            Console.WriteLine($"{nameof(PrepLowBound)} : {PrepLowBound}");
            Console.WriteLine($"{nameof(PrepUpBound)} : {PrepUpBound}");

            Console.WriteLine($"{nameof(MultiplyFactor)} : {MultiplyFactor}");
            Console.WriteLine($"{nameof(AddConstOffset)} : {AddConstOffset}");
            Console.WriteLine($"{nameof(SICount)} : {SICount}");
            Console.WriteLine($"{nameof(OffsetSI)} : {OffsetSI}");

            Console.WriteLine($"{nameof(USCount)} : {USCount}");
            Console.WriteLine($"{nameof(OffsetUS)} : {OffsetUS}");
            Console.WriteLine($"{nameof(UnkB)} : {UnkB}");
            Console.WriteLine($"{nameof(UnkC)} : {UnkC}");
        }
    }
}
