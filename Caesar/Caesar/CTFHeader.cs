using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class CTFHeader
    {
        public int CtfUnk1;
        public string Qualifier;
        public int CtfUnk3;
        public int CtfUnk4;
        private int CtfLanguageCount;
        private int CtfLanguageTableOffset;
        public string CtfUnkString;

        public List<CTFLanguage> CtfLanguages;

        public long BaseAddress;

        public CTFHeader() { }
        public CTFHeader(BinaryReader reader, long baseAddress, int headerSize) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);
            ulong ctfBitflags = reader.ReadUInt16();

            CtfUnk1 = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref ctfBitflags, reader, BaseAddress);
            CtfUnk3 = CaesarReader.ReadBitflagInt16(ref ctfBitflags, reader);
            CtfUnk4 = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            CtfLanguageCount = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            CtfLanguageTableOffset = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            CtfUnkString = CaesarReader.ReadBitflagStringWithReader(ref ctfBitflags, reader, BaseAddress);

            long ctfLanguageTableOffsetRelativeToDefintions = CtfLanguageTableOffset + BaseAddress;

            // parse every language record
            CtfLanguages = new List<CTFLanguage>();
            for (int languageEntry = 0; languageEntry < CtfLanguageCount; languageEntry++)
            {
                long languageTableEntryOffset = ctfLanguageTableOffsetRelativeToDefintions + (languageEntry * 4);

                reader.BaseStream.Seek(languageTableEntryOffset, SeekOrigin.Begin);
                long realLanguageEntryAddress = reader.ReadInt32() + ctfLanguageTableOffsetRelativeToDefintions;
                CTFLanguage language = new CTFLanguage(reader, realLanguageEntryAddress, headerSize);
                CtfLanguages.Add(language);
            }
        }
        public void PrintDebug() 
        {
            Console.WriteLine("----------- CTF header ----------- ");
            Console.WriteLine($"{nameof(CtfUnk1)} : {CtfUnk1}");
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(CtfUnk3)} : {CtfUnk3}");
            Console.WriteLine($"{nameof(CtfUnk4)} : {CtfUnk4}");
            Console.WriteLine($"{nameof(CtfLanguageCount)} : {CtfLanguageCount}");
            Console.WriteLine($"{nameof(CtfLanguageTableOffset)} : 0x{CtfLanguageTableOffset:X}");
            Console.WriteLine($"{nameof(CtfUnkString)} : {CtfUnkString}");
        }
    }
}
