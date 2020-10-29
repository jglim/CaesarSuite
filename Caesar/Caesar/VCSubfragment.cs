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
        public int subfragmentName_T;
        public byte[] subfragmentDump;
        public int subfragmentName2_T;
        public string fragmentNameUsuallyDisabled;
        public int subfragmentIdk3;
        public int subfragmentIdk4;
        public string fragmentStringIdk;
        public string subfragmentNameResolved;

        public VCSubfragment(BinaryReader reader, VCFragment parentFragment, CTFLanguage language, long baseAddress)
        {
            // see DIOpenCBF_FragValHandle
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt16();

            subfragmentName_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            if (parentFragment.fragmentCCFHandle == 5) 
            {
                // fragment should be parsed as PBSGetDumpAsStringFn, though internally we perceive this as the same
            }
            subfragmentDump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, parentFragment.fragmentVarcodeDumpSize, baseAddress);
            subfragmentName2_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            fragmentNameUsuallyDisabled = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            subfragmentIdk3 = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            subfragmentIdk4 = CaesarReader.ReadBitflagInt16(ref bitflags, reader, -1);
            fragmentStringIdk = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            subfragmentNameResolved = language.GetString(subfragmentName2_T);

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
                Console.WriteLine($"{nameof(subfragmentName_T)}, {subfragmentName_T}");
                Console.WriteLine($"{nameof(subfragmentDump)}, {BitUtility.BytesToHex(subfragmentDump)}");
                Console.WriteLine($"{nameof(subfragmentName2_T)}, {subfragmentName2_T}");
                Console.WriteLine($"{nameof(subfragmentNameResolved)}, {subfragmentNameResolved}");
                Console.WriteLine($"{nameof(fragmentNameUsuallyDisabled)}, {fragmentNameUsuallyDisabled}");
                Console.WriteLine($"{nameof(subfragmentIdk3)}, {subfragmentIdk3}");
                Console.WriteLine($"{nameof(subfragmentIdk4)}, {subfragmentIdk4}");
                Console.WriteLine($"{nameof(fragmentStringIdk)}, {fragmentStringIdk}");
            }
            else
            {
                Console.WriteLine($">> {BitUtility.BytesToHex(subfragmentDump)} : {subfragmentNameResolved}");
            }
        }
    }
}
