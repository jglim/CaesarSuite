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

        public int EnumLowBound;
        public int EnumUpBound;
        public int PrepLowBound;
        public int PrepUpBound;

        public float MultiplyFactor;
        public float AddConstOffset;

        public int SICount;
        public int OffsetSI;

        public int USCount;
        public int OffsetUS;

        public int EnumDescription;
        public int UnkC;

        [Newtonsoft.Json.JsonIgnore]
        private CTFLanguage Language;

        public void Restore(CTFLanguage language) 
        {
            Language = language;
        }

        public Scale() { }

        public Scale(BinaryReader reader, long baseAddress, CTFLanguage language) 
        {
            BaseAddress = baseAddress;
            Language = language;
            
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt16();

            EnumLowBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            EnumUpBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            PrepLowBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // could be float
            PrepUpBound = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // could be float

            MultiplyFactor = CaesarReader.ReadBitflagFloat(ref bitflags, reader);
            AddConstOffset = CaesarReader.ReadBitflagFloat(ref bitflags, reader);

            SICount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            OffsetSI = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            USCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            OffsetUS = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            EnumDescription = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            UnkC = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(EnumLowBound)} : {EnumLowBound}");
            Console.WriteLine($"{nameof(EnumUpBound)} : {EnumUpBound}");
            Console.WriteLine($"{nameof(PrepLowBound)} : {PrepLowBound}");
            Console.WriteLine($"{nameof(PrepUpBound)} : {PrepUpBound}");

            Console.WriteLine($"{nameof(MultiplyFactor)} : {MultiplyFactor}");
            Console.WriteLine($"{nameof(AddConstOffset)} : {AddConstOffset}");
            Console.WriteLine($"{nameof(SICount)} : {SICount}");
            Console.WriteLine($"{nameof(OffsetSI)} : {OffsetSI}");

            Console.WriteLine($"{nameof(USCount)} : {USCount}");
            Console.WriteLine($"{nameof(OffsetUS)} : {OffsetUS}");
            Console.WriteLine($"{nameof(EnumDescription)} : {EnumDescription}");
            Console.WriteLine($"{nameof(UnkC)} : {UnkC}");
        }
    }
}
