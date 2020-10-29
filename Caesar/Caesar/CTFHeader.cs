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
        public int ctfUnk1;
        public string ctfName;
        public int ctfUnk3;
        public int ctfUnk4;
        public int ctfLanguageCount;
        public int ctfLanguageTableOffset;
        public string ctfUnkString;

        public List<CTFLanguage> CTFLanguages;

        public long BaseAddress;
        public CTFHeader(BinaryReader reader, long baseAddress, CFFHeader header) 
        {
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            // ctf offsets start from here:

            ulong ctfBitflags = reader.ReadUInt16();

            ctfUnk1 = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            ctfName = CaesarReader.ReadBitflagStringWithReader(ref ctfBitflags, reader, BaseAddress);
            ctfUnk3 = CaesarReader.ReadBitflagInt16(ref ctfBitflags, reader);
            ctfUnk4 = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            ctfLanguageCount = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            ctfLanguageTableOffset = CaesarReader.ReadBitflagInt32(ref ctfBitflags, reader);
            ctfUnkString = CaesarReader.ReadBitflagStringWithReader(ref ctfBitflags, reader, BaseAddress);

            // PrintDebug();

            // Console.WriteLine($"ctf language table  {ctfLanguageTableOffsetRelativeToDefintions:X}");
            long ctfLanguageTableOffsetRelativeToDefintions = ctfLanguageTableOffset + BaseAddress;

            // parse every language record
            CTFLanguages = new List<CTFLanguage>();
            for (int languageEntry = 0; languageEntry < ctfLanguageCount; languageEntry++)
            {
                long languageTableEntryOffset = ctfLanguageTableOffsetRelativeToDefintions + (languageEntry * 4);

                reader.BaseStream.Seek(languageTableEntryOffset, SeekOrigin.Begin);
                long realLanguageEntryAddress = reader.ReadInt32() + ctfLanguageTableOffsetRelativeToDefintions;
                CTFLanguage language = new CTFLanguage(reader, realLanguageEntryAddress, header);
                CTFLanguages.Add(language);
            }


        }
        public void PrintDebug() 
        {
            Console.WriteLine("----------- ctf header ----------- ");
            Console.WriteLine($"{nameof(ctfUnk1)} : {ctfUnk1}");
            Console.WriteLine($"{nameof(ctfName)} : {ctfName}");
            Console.WriteLine($"{nameof(ctfUnk3)} : {ctfUnk3}");
            Console.WriteLine($"{nameof(ctfUnk4)} : {ctfUnk4}");
            Console.WriteLine($"{nameof(ctfLanguageCount)} : {ctfLanguageCount}");
            Console.WriteLine($"{nameof(ctfLanguageTableOffset)} : 0x{ctfLanguageTableOffset:X}");
            Console.WriteLine($"{nameof(ctfUnkString)} : {ctfUnkString}");
        }
    }
}
