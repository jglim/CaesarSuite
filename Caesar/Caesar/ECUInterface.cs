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
        public long BaseAddress;


        public string interfaceName;
        public int interfaceUnk1;
        public int interfaceUnk2;
        public string interfaceVersionString;
        public int interfaceVersion;
        public int interfaceNoOfStrings;
        public int interfaceStringTableOffset_fromInterfaceBlock;
        public int interfaceUnk6;

        public List<string> comParameters = new List<string>();

        public ECUInterface(BinaryReader reader, long baseAddress)
        {
            BaseAddress = baseAddress;

            // we can now properly operate on the interface block
            ulong interfaceBitflags = reader.ReadUInt32();

            interfaceName = CaesarReader.ReadBitflagStringWithReader(ref interfaceBitflags, reader, BaseAddress);
            interfaceUnk1 = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader, -1);
            interfaceUnk2 = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader, -1);
            interfaceVersionString = CaesarReader.ReadBitflagStringWithReader(ref interfaceBitflags, reader, BaseAddress);
            interfaceVersion = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            interfaceNoOfStrings = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            interfaceStringTableOffset_fromInterfaceBlock = CaesarReader.ReadBitflagInt32(ref interfaceBitflags, reader);
            interfaceUnk6 = CaesarReader.ReadBitflagInt16(ref interfaceBitflags, reader);


            long interfaceStringTableOffset_fromDefinitionBlock = interfaceStringTableOffset_fromInterfaceBlock + BaseAddress;
            // Console.WriteLine($"interface string table offset from definition block : {interfaceStringTableOffset_fromDefinitionBlock:X}");

            for (int interfaceStringIndex = 0; interfaceStringIndex < interfaceNoOfStrings; interfaceStringIndex++)
            {
                // seek to string pointer
                reader.BaseStream.Seek(interfaceStringTableOffset_fromDefinitionBlock + (interfaceStringIndex * 4), SeekOrigin.Begin);
                // from pointer, seek to string
                int interfaceStringReadoutPtr = reader.ReadInt32();
                reader.BaseStream.Seek(interfaceStringTableOffset_fromDefinitionBlock + interfaceStringReadoutPtr, SeekOrigin.Begin);
                string comParameter = CaesarReader.ReadStringFromBinaryReader(reader, Encoding.ASCII);
                comParameters.Add(comParameter);
            }

            // PrintDebug();
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{nameof(interfaceName)} : {interfaceName}");
            Console.WriteLine($"{nameof(interfaceUnk1)} : {interfaceUnk1}");
            Console.WriteLine($"{nameof(interfaceUnk2)} : {interfaceUnk2}");
            Console.WriteLine($"{nameof(interfaceVersionString)} : {interfaceVersionString}");
            Console.WriteLine($"{nameof(interfaceVersion)} : {interfaceVersion}");
            Console.WriteLine($"{nameof(interfaceNoOfStrings)} : {interfaceNoOfStrings}");
            Console.WriteLine($"{nameof(interfaceStringTableOffset_fromInterfaceBlock)} : 0x{interfaceStringTableOffset_fromInterfaceBlock:X}");
            Console.WriteLine($"{nameof(interfaceUnk6)} : {interfaceUnk6}");

            foreach (string comParameter in comParameters)
            {
                Console.WriteLine($"InterfaceComParameter: {comParameter}");
            }
        }
    }
}
