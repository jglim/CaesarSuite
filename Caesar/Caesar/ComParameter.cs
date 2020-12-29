using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class ComParameter
    {
        public int ComParamIndex;
        // this takes precedence over SubinterfaceIndex for KW2C3PE
        public int ParentInterfaceIndex;
        public int SubinterfaceIndex;
        public int unk5;
        public int Unk_CTF;
        public int Phrase;
        public int DumpSize;
        public byte[] Dump;
       
        public int ComParamValue;
        public string ParamName = "";

        public long BaseAddress;

        // looks exactly like the definition in DIOpenDiagService (#T)
        public ComParameter(BinaryReader reader, long baseAddress, List<ECUInterface> parentEcuInterfaceList) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt16();

            ComParamIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            ParentInterfaceIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            SubinterfaceIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader, 0);
            unk5 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            Unk_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // no -1? ctf strings should have -1
            Phrase = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, DumpSize, baseAddress);
            ComParamValue = 0;
            if (DumpSize == 4) 
            {
                ComParamValue = BitConverter.ToInt32(Dump, 0);
            }


            ECUInterface parentEcuInterface = parentEcuInterfaceList[ParentInterfaceIndex];

            if (ComParamIndex >= parentEcuInterface.comParameters.Count)
            {
                // throw new Exception("Invalid communication parameter : parent interface has no matching key");
                ParamName = "CP_UNKNOWN_MISSING_KEY";
                Console.WriteLine($"Warning: Tried to load a communication parameter without a parent (value: {ComParamValue}), parent: {parentEcuInterface.Qualifier}.");
            }
            else
            {
                ParamName = parentEcuInterface.comParameters[ComParamIndex];
            }
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"ComParam: id {ComParamIndex} ({ParamName}), v {ComParamValue} 0x{ComParamValue:X8} SI_Index:{SubinterfaceIndex} | parentIndex:{ParentInterfaceIndex} 5:{unk5} DumpSize:{DumpSize} D: {BitUtility.BytesToHex(Dump)}");
            Console.WriteLine($"Pos 0x{BaseAddress:X}");
        }
    }
}
