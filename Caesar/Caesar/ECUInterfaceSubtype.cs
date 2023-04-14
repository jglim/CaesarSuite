using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Caesar
{
    public class ECUInterfaceSubtype
    {
        public enum PhysicalProtocolType : int
        {
            // one or more of these are probably D2B and Most
            Unknown = 0,
            KLINE = 1,
            LSCAN = 2,
            HOMING_PIGEONS = 3,
            HSCAN = 4,
        }

        public string Qualifier;
        public int Name_CTF;
        public int Description_CTF;

        public int ParentInterfaceIndex;
        public int Unk4AlmostAlways1; // almost always 1

        public int PhysicalProtocolRaw;
        public int Unk6;
        public int Unk7;

        // these 2 below params appear specific o kline/kw2000pe
        public int Unk8;
        public int Unk9;

        public int Unk10; // might be signed, almost always -3?

        private long BaseAddress;

        public List<ComParameter> CommunicationParameters = new List<ComParameter>();
        public PhysicalProtocolType PhysicalProtocol { get { return (PhysicalProtocolType)PhysicalProtocolRaw; } }

        private CTFLanguage Language;

        public void Restore(CTFLanguage language) 
        {
            Language = language;
            foreach (ComParameter cp in CommunicationParameters) 
            {
                cp.Restore(language);
            }
        }

        public ECUInterfaceSubtype() { }

        public ECUInterfaceSubtype(BinaryReader reader, long baseAddress, int index, CTFLanguage language)
        {
            BaseAddress = baseAddress;
            Language = language;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            // we can now properly operate on the interface block
            ulong ctBitflags = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref ctBitflags, reader, BaseAddress);
            Name_CTF = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);

            ParentInterfaceIndex = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);
            Unk4AlmostAlways1 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);

            PhysicalProtocolRaw = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            Unk6 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            Unk7 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);

            Unk8 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            Unk9 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            Unk10 = CaesarReader.ReadBitflagInt8(ref ctBitflags, reader); // might be signed
            // PrintDebug();
        }

        public void PrintDebug()
        {
            Console.WriteLine($"iface subtype: @ 0x{BaseAddress:X}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
            Console.WriteLine($"{nameof(ParentInterfaceIndex)} : {ParentInterfaceIndex}");
            Console.WriteLine($"{nameof(Unk4AlmostAlways1)} : {Unk4AlmostAlways1}");
            Console.WriteLine($"{nameof(PhysicalProtocolRaw)} : {PhysicalProtocolRaw}");
            Console.WriteLine($"{nameof(Unk6)} : {Unk6}");
            Console.WriteLine($"{nameof(Unk7)} : {Unk7}");
            Console.WriteLine($"{nameof(Unk8)} : {Unk8}");
            Console.WriteLine($"{nameof(Unk9)} : {Unk9}");
            Console.WriteLine($"{nameof(Unk10)} : {Unk10}");
            Console.WriteLine($"CT: {Qualifier}");
        }
    }
}
