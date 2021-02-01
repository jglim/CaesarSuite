using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class VCSubfragment
    {
        public int Name_CTF;
        public byte[] Dump;
        public int Description_CTF;
        public string QualifierUsuallyDisabled;
        public int Unk3;
        public int Unk4;
        public string SupplementKey;


        [Newtonsoft.Json.JsonIgnore]
        public string NameResolved { get { return Language.GetString(Description_CTF); } }

        [Newtonsoft.Json.JsonIgnore]
        CTFLanguage Language;

        public void Restore(CTFLanguage language) 
        {
            Language = language;
        }

        public VCSubfragment() { }

        public VCSubfragment(BinaryReader reader, VCFragment parentFragment, CTFLanguage language, long baseAddress)
        {
            // see DIOpenCBF_FragValHandle
            Language = language;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt16();

            Name_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            if (parentFragment.CCFHandle == 5) 
            {
                // fragment should be parsed as PBSGetDumpAsStringFn, though internally we perceive this as the same
            }
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, parentFragment.VarcodeDumpSize, baseAddress);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            QualifierUsuallyDisabled = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            Unk3 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Unk4 = CaesarReader.ReadBitflagInt16(ref bitflags, reader, -1);
            SupplementKey = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            //int subfragmentIdk2 = reader.ReadInt32();
            //int subfragmentName = reader.ReadInt32();
            //int subfragmentIdkIncremented = reader.ReadInt32();
            //Console.WriteLine($"Subfragment: {subfragmentIdk1:X} {subfragmentIdk2:X} {language.GetString(subfragmentName)} {subfragmentIdkIncremented:X}");
            //PrintDebug();
        }

        private void PrintDebug(bool verbose = false) 
        {
            if (verbose)
            {
                Console.WriteLine("------------- subfragment ------------- ");
                Console.WriteLine($"{nameof(Name_CTF)}, {Name_CTF}");
                Console.WriteLine($"{nameof(Dump)}, {BitUtility.BytesToHex(Dump)}");
                Console.WriteLine($"{nameof(Description_CTF)}, {Description_CTF}");
                Console.WriteLine($"{nameof(NameResolved)}, {NameResolved}");
                Console.WriteLine($"{nameof(QualifierUsuallyDisabled)}, {QualifierUsuallyDisabled}");
                Console.WriteLine($"{nameof(Unk3)}, {Unk3}");
                Console.WriteLine($"{nameof(Unk4)}, {Unk4}");
                Console.WriteLine($"{nameof(SupplementKey)}, {SupplementKey}");
            }
            else
            {
                Console.WriteLine($">> {BitUtility.BytesToHex(Dump)} : {NameResolved}");
            }
        }
    }
}
