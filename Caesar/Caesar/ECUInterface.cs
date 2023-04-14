using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class ECUInterface
    {
        public string Qualifier;
        public int Name_CTF;
        public int Description_CTF;
        public string VersionString;
        public int Version;
        private int ComParamCount;
        private int ComParamListOffset;
        public int Unk6;
        

        public List<string> ComParameterNames = new List<string>();

        private CTFLanguage Language;
        private long BaseAddress;

        public void Restore(CTFLanguage language) 
        {
            Language = language;
        }

        public ECUInterface() { }

        public ECUInterface(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            // we can now properly operate on the interface block
            ulong interfaceBitflags = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref interfaceBitflags, reader, BaseAddress);
            Name_CTF = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader, -1);
            VersionString = CaesarReader.ReadBitflagStringWithReader(ref interfaceBitflags, reader, BaseAddress);
            Version = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            ComParamCount = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            ComParamListOffset = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            Unk6 = CaesarReader.ReadBitflagInt16(ref interfaceBitflags, reader);


            // absolute file offset to the comparam string table
            // this points to an array of integers
            // each of these ints is added to the fileoffset to get to the actual string
            long comparamFileOffset = ComParamListOffset + BaseAddress;
            // Console.WriteLine($"interface string table offset from definition block : {interfaceStringTableOffset_fromDefinitionBlock:X}");

            for (int interfaceStringIndex = 0; interfaceStringIndex < ComParamCount; interfaceStringIndex++)
            {
                // seek to string pointer
                reader.BaseStream.Seek(comparamFileOffset + (interfaceStringIndex * 4), SeekOrigin.Begin);
                // from pointer, seek to string
                long interfaceStringReadoutPtr = reader.ReadInt32() + comparamFileOffset;
                reader.BaseStream.Seek(interfaceStringReadoutPtr, SeekOrigin.Begin);
                string comParameter = CaesarReader.ReadStringFromBinaryReader(reader);
                ComParameterNames.Add(comParameter);
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
            Console.WriteLine($"{nameof(VersionString)} : {VersionString}");
            Console.WriteLine($"{nameof(Version)} : {Version}");
            Console.WriteLine($"{nameof(ComParamCount)} : {ComParamCount}");
            Console.WriteLine($"{nameof(ComParamListOffset)} : 0x{ComParamListOffset:X}");
            Console.WriteLine($"{nameof(Unk6)} : {Unk6}");

            foreach (string comParameter in ComParameterNames)
            {
                Console.WriteLine($"InterfaceComParameter: {comParameter}");
            }
        }
    }
}
