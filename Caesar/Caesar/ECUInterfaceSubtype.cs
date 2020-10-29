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
        public string ctName;
        public int InterfaceName_T;
        public int InterfaceLongName_T;

        public int ctUnk3;
        public int ctUnk4;

        public int ctUnk5;
        public int ctUnk6;
        public int ctUnk7;

        public int ctUnk8;
        public int ctUnk9;
        public int ctUnk10; // might be signed

        public long BaseAddress;

        public ECUInterfaceSubtype(BinaryReader reader, long baseAddress) 
        {
            BaseAddress = baseAddress;

            // we can now properly operate on the interface block
            ulong ctBitflags = reader.ReadUInt32();

            ctName = CaesarReader.ReadBitflagStringWithReader(ref ctBitflags, reader, BaseAddress);
            InterfaceName_T = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);
            InterfaceLongName_T = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);

            ctUnk3 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);
            ctUnk4 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);

            ctUnk5 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            ctUnk6 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            ctUnk7 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);

            ctUnk8 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            ctUnk9 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            ctUnk10 = CaesarReader.ReadBitflagInt8(ref ctBitflags, reader); // might be signed

            // PrintDebug();
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{nameof(InterfaceName_T)} : {InterfaceName_T}");
            Console.WriteLine($"{nameof(InterfaceLongName_T)} : {InterfaceLongName_T}");
            Console.WriteLine($"{nameof(ctUnk3)} : {ctUnk3}");
            Console.WriteLine($"{nameof(ctUnk4)} : {ctUnk4}");
            Console.WriteLine($"{nameof(ctUnk5)} : {ctUnk5}");
            Console.WriteLine($"{nameof(ctUnk6)} : {ctUnk6}");
            Console.WriteLine($"{nameof(ctUnk7)} : {ctUnk7}");
            Console.WriteLine($"{nameof(ctUnk8)} : {ctUnk8}");
            Console.WriteLine($"{nameof(ctUnk9)} : {ctUnk9}");
            Console.WriteLine($"{nameof(ctUnk10)} : {ctUnk10}");
            Console.WriteLine($"CT: {ctName}");
        }
    }
}
