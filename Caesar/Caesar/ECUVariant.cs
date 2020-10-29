using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class ECUVariant
    {
        public string variantName;
        public int variantName_T;
        public int variantLongName_T;
        public string variantIdkStr1;
        public string variantIdkStr2;
        public int variantIdk1;
        public int variantSubsectionA;
        public int variantIdk3;
        public int variantSubsectionB;
        public int variantIdk5;
        public int variantSubsectionC;
        public int variantIdk7;
        public int variantSubsectionD;
        public int variantIdk9;
        public int variantSubsectionE;
        public int variantIdk11;
        public int variantSubsectionF;
        public int variantIdk13;
        public int variantSubsectionG;
        public int variantIdk15;
        public int variantSubsectionH;
        public int variantIdk17;
        public int variantVCodingEntryCount;
        public int variantVCodingEntriesOffset;
        public string negativeResponseName;
        public int variantIdkByte;
        public List<int> variantCodingDomainOffsets = new List<int>();

        public List<VCDomain> VCDomains = new List<VCDomain>();

        public ECUVariant(BinaryReader reader, ECU parentEcu, CTFLanguage language, long baseAddress, int blockSize)
        {
            /*
            // going to assume there's more in variant pool?
            byte[] variantPool = ecu.ReadVariantPool(reader);
            //Console.WriteLine($"Variant Pool: \n{BitUtility.BytesToHex(variantPool)}");

            long variantBlockOffset;
            int variantBlockSize;
            using (BinaryReader variantPoolReader = new BinaryReader(new MemoryStream(variantPool)))
            {
                variantBlockOffset = variantPoolReader.ReadUInt32();
                variantBlockSize = variantPoolReader.ReadInt32();
            }
            reader.BaseStream.Seek(ecu.ecuvariant_fileoffset_1 + variantBlockOffset, SeekOrigin.Begin);
            byte[] variantBytes = reader.ReadBytes(variantBlockSize);
            */

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            byte[] variantBytes = reader.ReadBytes(blockSize);

            using (BinaryReader variantReader = new BinaryReader(new MemoryStream(variantBytes)))
            {
                ulong bitFlags = variantReader.ReadUInt32();
                int skip = variantReader.ReadInt32();

                variantName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantName_T = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                variantLongName_T = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader, -1);
                variantIdkStr1 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantIdkStr2 = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantIdk1 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 1 
                variantSubsectionA = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 2 
                variantIdk3 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 3 
                variantSubsectionB = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 4 
                variantIdk5 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 5 
                variantSubsectionC = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 6 
                variantIdk7 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 7 
                variantSubsectionD = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 8 
                variantIdk9 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 9 
                variantSubsectionE = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 10 
                variantIdk11 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 11 
                variantSubsectionF = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 12 
                variantIdk13 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 13 
                variantSubsectionG = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 14 
                variantIdk15 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 15 
                variantSubsectionH = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 16 
                variantIdk17 = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 17 
                variantVCodingEntryCount = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 18 
                variantVCodingEntriesOffset = CaesarReader.ReadBitflagInt32(ref bitFlags, variantReader);  // 19 
                negativeResponseName = CaesarReader.ReadBitflagStringWithReader(ref bitFlags, variantReader);
                variantIdkByte = CaesarReader.ReadBitflagInt8(ref bitFlags, variantReader);  // 20 byte

                variantCodingDomainOffsets = new List<int>();
                variantReader.BaseStream.Seek(variantVCodingEntriesOffset, SeekOrigin.Begin);
                for (int variantCodingIndex = 0; variantCodingIndex < variantVCodingEntryCount; variantCodingIndex++)
                {
                    variantCodingDomainOffsets.Add(variantReader.ReadInt32());
                }
            }

            // PrintDebug();
            CreateVCDomains(reader, parentEcu, language);
        }

        public VCDomain GetVCDomainByName(string name)
        {
            foreach (VCDomain domain in VCDomains)
            {
                if (domain.vcdName == name)
                {
                    return domain;
                }
            }
            return null;
        }
        public string[] GetVCDomainNames()
        {
            List<string> result = new List<string>();
            foreach (VCDomain domain in VCDomains)
            {
                result.Add(domain.vcdName);
            }
            return result.ToArray();
        }


        private void CreateVCDomains(BinaryReader reader, ECU parentEcu, CTFLanguage language) 
        {
            VCDomains = new List<VCDomain>();
            foreach (int variantCodingDomainEntry in variantCodingDomainOffsets)
            {
                VCDomain vcDomain = new VCDomain(reader, parentEcu, language, variantCodingDomainEntry);
                VCDomains.Add(vcDomain);
                /*
                byte[] variantCodingPool = parentEcu.ReadVarcodingPool(reader);
                using (BinaryReader poolReader = new BinaryReader(new MemoryStream(variantCodingPool)))
                {
                    poolReader.BaseStream.Seek(variantCodingDomainEntry * parentEcu.varcoding_tableEntrySize, SeekOrigin.Begin);
                    int entryOffset = poolReader.ReadInt32();
                    int entrySize = poolReader.ReadInt32();
                    uint entryCrc = poolReader.ReadUInt32();
                    long vcdBlockAddress = entryOffset + parentEcu.varcoding_fileoffset_5;

                    Console.WriteLine($"VCD Entry @ 0x{entryOffset:X} with size 0x{entrySize:X} and CRC {entryCrc:X8}, abs addr {vcdBlockAddress:X8}");

                    long baseAddress = vcdBlockAddress;
                    reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
                    ulong bitflags = reader.ReadUInt16();
                    string vcdName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                    int vcdName_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                    int vcdNameLong_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                    string vcdReadService = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                    string vcdWriteService = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                    int vcdFragmentCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                    int vcdFragmentTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader) + (int)baseAddress; // demoting long (warning)
                    int vcdDump = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                    int vcdDefaultStringCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                    int vcdIdkOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                    int vcdIdk1 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

                    Console.WriteLine($"VCD Name: {vcdName}");
                    Console.WriteLine($"{nameof(vcdName_T)} : {vcdName_T}");
                    Console.WriteLine($"{nameof(vcdNameLong_T)} : {vcdNameLong_T}");
                    Console.WriteLine($"{nameof(vcdReadService)} : {vcdReadService}");
                    Console.WriteLine($"{nameof(vcdWriteService)} : {vcdWriteService}");

                    Console.WriteLine($"{nameof(vcdFragmentCount)} : {vcdFragmentCount}");
                    Console.WriteLine($"{nameof(vcdFragmentTableOffset)} : 0x{vcdFragmentTableOffset:X}");
                    Console.WriteLine($"{nameof(vcdDump)} : {vcdDump}");
                    Console.WriteLine($"{nameof(vcdDefaultStringCount)} : {vcdDefaultStringCount}");
                    Console.WriteLine($"{nameof(vcdIdkOffset)} : {vcdIdkOffset}");
                    Console.WriteLine($"{nameof(vcdIdk1)} : {vcdIdk1}");

                    for (int fragmentIndex = 0; fragmentIndex < vcdFragmentCount; fragmentIndex++)
                    {
                        VCFragment fragment = new VCFragment(reader, vcdFragmentTableOffset, fragmentIndex, language);
                    }
                }
                */
            }
        }

        public void PrintDebug() 
        {

            Console.WriteLine($"{nameof(variantName)} : {variantName}");
            Console.WriteLine($"{nameof(variantName_T)} : {variantName_T}");
            Console.WriteLine($"{nameof(variantLongName_T)} : {variantLongName_T}");
            Console.WriteLine($"{nameof(variantIdkStr1)} : {variantIdkStr1}");
            Console.WriteLine($"{nameof(variantIdkStr2)} : {variantIdkStr2}");
            Console.WriteLine($"{nameof(variantVCodingEntryCount)} : {variantVCodingEntryCount}");
            Console.WriteLine($"{nameof(variantVCodingEntriesOffset)} : {variantVCodingEntriesOffset}");
            Console.WriteLine($"{nameof(negativeResponseName)} : {negativeResponseName}");

            foreach (int offset in variantCodingDomainOffsets)
            {
                Console.WriteLine($"VCD Offset: {offset:X8}");
            }
        }
    }
}
