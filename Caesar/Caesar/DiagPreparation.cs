using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class DiagPreparation
    {
        public string qualifier;
        public int Name_T;
        public int unk1;
        public int unk2;
        public int fieldA;
        public int IITOffset;
        public int AvailableBitWidth;
        public int PresPool;
        public int field1E;
        public int noIdea;
        public int DumpMode;
        public int DumpSize;
        public byte[] Dump;

        public int BitPosition;
        public ushort ModeConfig;

        CTFLanguage Language;

        long BaseAddress;
        // void __cdecl DiagServiceReadPresentation(int *inBase, DECODED_PRESENTATION *outPresentation)
        public DiagPreparation(BinaryReader reader, CTFLanguage language, long baseAddress, int bitPosition, ushort modeConfig)
        {
            BitPosition = bitPosition;
            ModeConfig = modeConfig;
            Language = language;
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();


            qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            Name_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            unk1 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            unk2 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            fieldA = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            IITOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            AvailableBitWidth = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PresPool = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            field1E = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            noIdea = CaesarReader.ReadBitflagInt16(ref bitflags, reader, -1);
            DumpMode = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            if (DumpMode == 5) 
            {
                // dump is actually a string, use
                // CaesarReader.ReadBitflagDumpWithReaderAsString
            }
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, DumpSize, baseAddress);
            // PrintDebug();
        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(qualifier)} : {qualifier}");
            Console.WriteLine($"{nameof(BitPosition)} : {BitPosition}");
            Console.WriteLine($"{nameof(ModeConfig)} : 0x{ModeConfig:X}");
            Console.WriteLine($"{nameof(Name_T)} : {Name_T}");
            Console.WriteLine($"{nameof(Name_T)} : {Language.GetString(Name_T)}");
            Console.WriteLine($"{nameof(unk1)} : {unk1}");
            Console.WriteLine($"{nameof(unk2)} : {unk2}");
            Console.WriteLine($"{nameof(fieldA)} : {fieldA}");
            Console.WriteLine($"{nameof(IITOffset)} : {IITOffset}");
            Console.WriteLine($"{nameof(AvailableBitWidth)} : {AvailableBitWidth}");
            Console.WriteLine($"{nameof(PresPool)} : {PresPool}");
            Console.WriteLine($"{nameof(field1E)} : {field1E}");
            Console.WriteLine($"{nameof(noIdea)} : {noIdea}");
            // Console.WriteLine($"{nameof(noIdea_T)} : {language.GetString(noIdea_T)}");
            Console.WriteLine($"{nameof(Dump)} : {BitUtility.BytesToHex(Dump)}");
            Console.WriteLine("---------------");
        }
    }
}
