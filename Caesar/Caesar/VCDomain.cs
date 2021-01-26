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

        public string Qualifier;
        public int Name_CTF;
        public int Description_CTF;
        public string ReadServiceName;
        public string WriteServiceName;
        public int FragmentCount;
        public int FragmentTableOffset;
        public int DumpSize;
        public int DefaultStringCount;
        public int StringTableOffset;
        public int Unk1;

        public List<VCFragment> VCFragments = new List<VCFragment>();
        public ECU ParentECU;

        public List<Tuple<string, byte[]>> DefaultData = new List<Tuple<string, byte[]>>();

        public VCDomain(BinaryReader reader, ECU parentEcu, CTFLanguage language, int variantCodingDomainEntry) 
        {
            ParentECU = parentEcu;

            byte[] variantCodingPool = parentEcu.ReadVarcodingPool(reader);
            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(variantCodingPool)))
            {
                poolReader.BaseStream.Seek(variantCodingDomainEntry * parentEcu.VcDomain_EntrySize, SeekOrigin.Begin);
                int entryOffset = poolReader.ReadInt32();
                int entrySize = poolReader.ReadInt32();
                uint entryCrc = poolReader.ReadUInt32();
                long vcdBlockAddress = entryOffset + parentEcu.VcDomain_BlockOffset;

                // Console.WriteLine($"VCD Entry @ 0x{entryOffset:X} with size 0x{entrySize:X} and CRC {entryCrc:X8}, abs addr {vcdBlockAddress:X8}");

                long baseAddress = vcdBlockAddress;
                reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
                ulong bitflags = reader.ReadUInt16();

                Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                Name_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
                ReadServiceName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                WriteServiceName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
                FragmentCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                FragmentTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader) + (int)baseAddress; // demoting long (warning)
                DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                DefaultStringCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                StringTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
                Unk1 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

                // PrintDebug();

                VCFragments = new List<VCFragment>();
                for (int fragmentIndex = 0; fragmentIndex < FragmentCount; fragmentIndex++)
                {
                    VCFragment fragment = new VCFragment(reader, this, FragmentTableOffset, fragmentIndex, language, parentEcu);
                    VCFragments.Add(fragment);
                }
                // ValidateFragmentCoverage();

                if (DefaultStringCount > 0) 
                {
                    DefaultData = new List<Tuple<string, byte[]>>();
                    long stringTableBaseAddress = StringTableOffset + baseAddress;
                    // this could almost be a class of its own but there isn't a distinct name to it
                    for (int stringTableIndex = 0; stringTableIndex < DefaultStringCount; stringTableIndex++) 
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
            int expectedLengthInBits = DumpSize * 8;
            List<VCFragment> fragments = new List<VCFragment>(VCFragments);
            List<int> bitGapPositions = new List<int>();

            while (fragments.Count > 0)
            {
                VCFragment result = fragments.Find(x => x.ByteBitPos == bitCursor);
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
                    bitCursor += result.BitLength;
                    fragments.Remove(result);
                }
            }
        }

        public void PrintDebug() 
        {

            Console.WriteLine($"VCD Name: {Qualifier}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
            Console.WriteLine($"{nameof(ReadServiceName)} : {ReadServiceName}");
            Console.WriteLine($"{nameof(WriteServiceName)} : {WriteServiceName}");

            Console.WriteLine($"{nameof(FragmentCount)} : {FragmentCount}");
            Console.WriteLine($"{nameof(FragmentTableOffset)} : 0x{FragmentTableOffset:X}");
            Console.WriteLine($"{nameof(DumpSize)} : {DumpSize}");
            Console.WriteLine($"{nameof(DefaultStringCount)} : {DefaultStringCount}");
            Console.WriteLine($"{nameof(StringTableOffset)} : {StringTableOffset}");
            Console.WriteLine($"{nameof(Unk1)} : {Unk1}");

        }
    }
}
