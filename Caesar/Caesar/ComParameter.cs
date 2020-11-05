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
        public int comIndex;
        public int idk3;
        public int subinterfaceIndex;
        public int idk5;
        public int phrase;
        public int DumpSize;
        public byte[] Dump;
       
        // public int idk8;
        // public int idk9;
        public int comValue;
        public string ParamName = "";

        public long BaseAddress;
        public ComParameter(BinaryReader reader, long baseAddress, ECUInterface parentEcuInterface) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ushort minBitflags = reader.ReadUInt16();
            if ((minBitflags & 0xFF10) > 0)
            {
                // no idea where the actual comparams are read out, this whole class's parsing is guessed from typical caesar behavior
                throw new Exception("ComParameters has bitflags for fields that the library does not understand.");
            }
            ulong bitflags = minBitflags;

            // one of these unknown fields is probably the data type

            comIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader); // reader.ReadInt16();
            idk3 = CaesarReader.ReadBitflagInt16(ref bitflags, reader); // reader.ReadInt16();
            subinterfaceIndex = CaesarReader.ReadBitflagInt16(ref bitflags, reader, 0); // reader.ReadInt16();
            idk5 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);  // reader.ReadInt16();

            // this is an unknown, implemented to shift the bits once. using int16 as a guess
            int unknownRead = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            phrase = CaesarReader.ReadBitflagInt16(ref bitflags, reader); // reader.ReadInt32(); 
            DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);  // reader.ReadInt16();
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, DumpSize, baseAddress);
            // idk8 = CaesarReader.ReadBitflagInt16(ref bitflags, reader); // reader.ReadInt16();
            // idk9 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);  // reader.ReadInt16();
            // comValue = CaesarReader.ReadBitflagInt32(ref bitflags, reader); // reader.ReadInt32();
            comValue = 0;
            if (DumpSize == 4) 
            {
                comValue = BitConverter.ToInt32(Dump, 0);
            }

            if (comIndex > parentEcuInterface.comParameters.Count)
            {
                throw new Exception("Invalid communication parameter : parent interface has no matching key");
            }
            ParamName = parentEcuInterface.comParameters[comIndex];
            // PrintDebug();
        }

        public void PrintDebug() 
        {
            Console.WriteLine($"ComParam: id {comIndex} ({ParamName}), v {comValue} 0x{comValue:X8} SI_Index:{subinterfaceIndex} | 3:{idk3} 5:{idk5} DumpSize:{DumpSize} D: {BitUtility.BytesToHex(Dump)}");
            Console.WriteLine($"Pos 0x{BaseAddress:X}");
        }
    }
}
