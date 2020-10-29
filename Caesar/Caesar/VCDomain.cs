using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class VCDomain
    {

        public string vcdName;
        public int vcdName_T;
        public int vcdNameLong_T;
        public string vcdReadService;
        public string vcdWriteService;
        public int vcdFragmentCount;
        public int vcdFragmentTableOffset;
        public int vcdDumpSize;
        public int vcdDefaultStringCount;
        public int vcdStringTableOffset;
        public int vcdIdk1;

        public List<VCFragment> VCFragments = new List<VCFragment>();
        public ECU ParentECU;

        public List<Tuple<string, byte[]>> DefaultData = new List<Tuple<string, byte[]>>();

        public VCDomain(BinaryReader reader, ECU parentEcu, CTFLanguage language, int variantCodingDomainEntry) 
        {
            ParentECU = parentEcu;

            byte[] variantCodingPool = parentEcu.ReadVarcodingPool(reader);
            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(variantCodingPool)))
            {
                poolReader.BaseStream.Seek(variantCodingDomainEntry * parentEcu.varcoding_tableEntrySize, SeekOrigin.Begin);
                int entryOffset = poolReader.ReadInt32();
                int entrySize = poolReader.ReadInt32();
                uint entryCrc = poolReader.ReadUInt32();
                long vcdBlockAddress = entryOffset + parentEcu.varcoding_fileoffset_5;

                // Console.WriteLine($"VCD Entry @ 0x{entryOffset:X} with size 0x{entrySize:X} and CRC {entryCrc:X8}, abs addr {vcdBlockAddress:X8}");

                long baseAddress = vcdBlockAddress;
                reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
                ulong bitflags = reader.ReadUInt16();

                vcdName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                vcdName_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                vcdNameLong_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                vcdReadService = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                vcdWriteService = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                vcdFragmentCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                vcdFragmentTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader) + (int)baseAddress; // demoting long (warning)
                vcdDumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                vcdDefaultStringCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                vcdStringTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                vcdIdk1 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

                // PrintDebug();

                VCFragments = new List<VCFragment>();
                for (int fragmentIndex = 0; fragmentIndex < vcdFragmentCount; fragmentIndex++)
                {
                    VCFragment fragment = new VCFragment(reader, this, vcdFragmentTableOffset, fragmentIndex, language);
                    VCFragments.Add(fragment);
                }
                // ValidateFragmentCoverage();

                if (vcdDefaultStringCount > 0) 
                {
                    DefaultData = new List<Tuple<string, byte[]>>();
                    long stringTableBaseAddress = vcdStringTableOffset + baseAddress;
                    // this could almost be a class of its own but there isn't a distinct name to it
                    for (int stringTableIndex = 0; stringTableIndex < vcdDefaultStringCount; stringTableIndex++) 
                    {
                        reader.BaseStream.Seek(stringTableBaseAddress + (4 * stringTableIndex), SeekOrigin.Begin);
                        int offset = reader.ReadInt32();
                        long stringBaseAddress = stringTableBaseAddress + offset;
                        reader.BaseStream.Seek(stringBaseAddress, SeekOrigin.Begin);
                        ulong strBitflags = reader.ReadUInt16();
                        int nameUsuallyAbsent_T = CaesarReader.ReadBitflagInt32(ref strBitflags, reader, -1);
                        int offsetToBlob = CaesarReader.ReadBitflagInt32(ref strBitflags, reader);
                        int blobSize = CaesarReader.ReadBitflagInt32(ref strBitflags, reader);
                        int valueType_T = CaesarReader.ReadBitflagInt32(ref strBitflags, reader, -1);
                        string noIdeaStr1 = CaesarReader.ReadBitflagStringWithReader(ref strBitflags, reader, stringBaseAddress);
                        int noIdea2_T = CaesarReader.ReadBitflagInt32(ref strBitflags, reader, -1);
                        int noIdea3 = CaesarReader.ReadBitflagInt16(ref strBitflags, reader);
                        string noIdeaStr2 = CaesarReader.ReadBitflagStringWithReader(ref strBitflags, reader, stringBaseAddress);
                        byte[] blob = new byte[] { };
                        if (blobSize > 0)
                        {
                            long blobFileAddress = stringBaseAddress + offsetToBlob;
                            reader.BaseStream.Seek(blobFileAddress, SeekOrigin.Begin);
                            blob = reader.ReadBytes(blobSize);
                            // memcpy
                        }

                        string valueType = language.GetString(valueType_T);
                        DefaultData.Add(new Tuple<string, byte[]>(valueType, blob));
                        //Console.WriteLine($"Blob: {BitUtility.BytesToHex(blob)} @ {valueType}");
                        //Console.WriteLine($"String base address: 0x{stringBaseAddress:X}");
                    }
                }
            }
        }

        private void ValidateFragmentCoverage() 
        {
            // apparently gaps are okay, there isn't a 100% way to find out if parsing errors have snuck through
            int bitCursor = 0;
            int expectedLengthInBits = vcdDumpSize * 8;
            List<VCFragment> fragments = new List<VCFragment>(VCFragments);
            List<int> bitGapPositions = new List<int>();

            while (fragments.Count > 0)
            {
                VCFragment result = fragments.Find(x => x.fragmentByteBitPos == bitCursor);
                if (result is null)
                {
                    bitGapPositions.Add(bitCursor);
                    bitCursor++;
                    if (bitCursor > expectedLengthInBits) 
                    {
                        throw new Exception("wtf");
                    }
                }
                else 
                {
                    bitCursor += result.fragmentBitLength;
                    fragments.Remove(result);
                }
            }
            /*
            foreach (VCFragment fragment in VCFragments)
            {
                Console.WriteLine($"OK: {fragment.fragmentName}");
            }
            Console.WriteLine($"Bit End: {bitCursor}, expected end: {expectedLengthInBits}, gap count {bitGapPositions.Count}");
            */
        }

        public void PrintDebug() 
        {

            Console.WriteLine($"VCD Name: {vcdName}");
            Console.WriteLine($"{nameof(vcdName_T)} : {vcdName_T}");
            Console.WriteLine($"{nameof(vcdNameLong_T)} : {vcdNameLong_T}");
            Console.WriteLine($"{nameof(vcdReadService)} : {vcdReadService}");
            Console.WriteLine($"{nameof(vcdWriteService)} : {vcdWriteService}");

            Console.WriteLine($"{nameof(vcdFragmentCount)} : {vcdFragmentCount}");
            Console.WriteLine($"{nameof(vcdFragmentTableOffset)} : 0x{vcdFragmentTableOffset:X}");
            Console.WriteLine($"{nameof(vcdDumpSize)} : {vcdDumpSize}");
            Console.WriteLine($"{nameof(vcdDefaultStringCount)} : {vcdDefaultStringCount}");
            Console.WriteLine($"{nameof(vcdStringTableOffset)} : {vcdStringTableOffset}");
            Console.WriteLine($"{nameof(vcdIdk1)} : {vcdIdk1}");

        }
    }
}
