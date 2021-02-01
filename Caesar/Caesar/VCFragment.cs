using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{
    public class VCFragment
    {
        public int ByteBitPos;
        public ushort ImplementationType;

        public int Name_CTF;
        public int Description_CTF;
        public int ReadAccessLevel;
        public int WriteAccessLevel;
        public int ByteOrder;
        public int RawBitLength;
        public int IttOffset;
        public int InfoPoolIndex;
        public int MeaningB;
        public int MeaningC;
        public int CCFHandle;
        public int VarcodeDumpSize;
        public byte[] VarcodeDump;
        private int SubfragmentCount; // exposed as Subfragments.Count
        private long SubfragmentFileOffset; // exposed as Subfragments
        public string Qualifier;

        public int ImplementationUpper;
        public int ImplementationLower;

        public int BitLength;

        public List<VCSubfragment> Subfragments = new List<VCSubfragment>();

        [Newtonsoft.Json.JsonIgnore]
        private static readonly byte[] FragmentLengthTable = new byte[] { 0, 1, 4, 8, 0x10, 0x20, 0x40 };
        [Newtonsoft.Json.JsonIgnore]
        public VCDomain ParentDomain;
        [Newtonsoft.Json.JsonIgnore]
        public ECU ParentECU;

        public void Restore(ECU parentEcu, VCDomain parentDomain, CTFLanguage language) 
        {
            ParentECU = parentEcu;
            ParentDomain = parentDomain;
            foreach (VCSubfragment subfragment in Subfragments) 
            {
                subfragment.Restore(language);
            }
        }

        public VCFragment() { }

        public VCFragment(BinaryReader reader, VCDomain parentDomain, long fragmentTable, int fragmentIndex, CTFLanguage language, ECU parentEcu) 
        {
            // see DIOpenVarCodeFrag
            ParentDomain = parentDomain;
            ParentECU = parentEcu;

            long fragmentTableEntry = fragmentTable + (10 * fragmentIndex);
            reader.BaseStream.Seek(fragmentTableEntry, SeekOrigin.Begin);
            // no bitflag required for 10-byte table entry since it is mandatory
            int fragmentNewBaseOffset = reader.ReadInt32();

            ByteBitPos = reader.ReadInt32();
            ImplementationType = reader.ReadUInt16();

            // Console.WriteLine($"Fragment new base @ 0x{fragmentNewBaseOffset:X}, byteBitPos 0x{fragmentByteBitPos:X}, implementationType: 0x{implementationType:X}");
            long fragmentBaseAddress = fragmentTable + fragmentNewBaseOffset;
            reader.BaseStream.Seek(fragmentBaseAddress, SeekOrigin.Begin);
            ulong fragmentBitflags = reader.ReadUInt32();
            // Console.WriteLine($"Fragment new bitflag @ 0x{fragmentBitflags:X}");

            Name_CTF = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            ReadAccessLevel = CaesarReader.ReadBitflagUInt8(ref fragmentBitflags, reader);
            WriteAccessLevel = CaesarReader.ReadBitflagUInt8(ref fragmentBitflags, reader);
            ByteOrder = CaesarReader.ReadBitflagUInt16(ref fragmentBitflags, reader);
            RawBitLength = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            IttOffset = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            InfoPoolIndex = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            MeaningB = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            MeaningC = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            CCFHandle = CaesarReader.ReadBitflagInt16(ref fragmentBitflags, reader, -1);
            VarcodeDumpSize = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            VarcodeDump = CaesarReader.ReadBitflagDumpWithReader(ref fragmentBitflags, reader, VarcodeDumpSize, fragmentBaseAddress);
            SubfragmentCount = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            SubfragmentFileOffset = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref fragmentBitflags, reader, fragmentBaseAddress);

            // Console.WriteLine($"{nameof(fragmentName)} : {fragmentName}, child {fragmentNoOfSubFragments} @ 0x{fragmentSubfragmentFileOffset:X} base {fragmentBaseAddress:X}");

            
            if ((ByteOrder != 0) && (BitLength > 0)) 
            {
                //throw new Exception("Currently assumes everything is little-endian");
                Console.WriteLine($"WARNING: {Qualifier} (Size: {BitLength}) has an unsupported byte order. Please proceed with caution");
                //PrintDebug(true);
            }
            

            long subfragmentTableAddress = SubfragmentFileOffset + fragmentBaseAddress;
            Subfragments.Clear();
            for (int subfragmentIndex = 0; subfragmentIndex < SubfragmentCount; subfragmentIndex++)
            {
                reader.BaseStream.Seek(subfragmentTableAddress + (subfragmentIndex * 4), SeekOrigin.Begin);
                long subfragmentAddress = reader.ReadInt32() + subfragmentTableAddress;
                VCSubfragment subfragment = new VCSubfragment(reader, this, language, subfragmentAddress);
                Subfragments.Add(subfragment);
            }
            // PrintDebug();
            // Console.WriteLine($"implementation-default : {implementationType:X4} upper: {(implementationType & 0xFF0):X4} lower: {(implementationType & 0xF):X4}");
            FindFragmentSize(reader);
        }

        public VCSubfragment GetSubfragmentConfiguration(byte[] variantCodingValue)
        {
            byte[] variantBits = BitUtility.ByteArrayToBitArray(variantCodingValue);
            byte[] affectedBits = variantBits.Skip(ByteBitPos).Take(BitLength).ToArray();

            foreach (VCSubfragment subfragment in Subfragments)
            {
                byte[] sfToCompare = BitUtility.ByteArrayToBitArray(subfragment.Dump).Take(BitLength).ToArray();
                if (sfToCompare.SequenceEqual(affectedBits))
                {
                    return subfragment;
                }
            }
            return null;
        }
        public byte[] SetSubfragmentConfiguration(byte[] variantCodingValue, string subfragmentName)
        {
            foreach (VCSubfragment subfragment in Subfragments)
            {
                if (subfragment.NameResolved == subfragmentName)
                {
                    return SetSubfragmentConfiguration(variantCodingValue, subfragment);
                }
            }
            throw new FormatException($"Requested subfragment {subfragmentName} could not be found in {Qualifier}");
        }

        public byte[] SetSubfragmentConfiguration(byte[] variantCodingValue, VCSubfragment subfragment)
        {
            byte[] variantBits = BitUtility.ByteArrayToBitArray(variantCodingValue);
            List<byte> result = new List<byte>(variantBits.Take(ByteBitPos));
            variantBits = variantBits.Skip(BitLength + ByteBitPos).ToArray();
            byte[] sfToSet = BitUtility.ByteArrayToBitArray(subfragment.Dump).Take(BitLength).ToArray();
            result.AddRange(sfToSet);
            result.AddRange(variantBits);
            return BitUtility.BitArrayToByteArray(result.ToArray());
        }

        private void FindFragmentSize(BinaryReader reader) 
        {
            ImplementationUpper = ImplementationType & 0xFF0;
            ImplementationLower = ImplementationType & 0xF;
            BitLength = 0;

            // fixup the bit length
            if (ImplementationLower > 6)
            {
                throw new NotImplementedException("The disassembly throws an exception when fragmentImplementationLower > 6, copying verbatim");
            }

            if (ImplementationUpper > 0x420)
            {
                // Console.WriteLine($"fragment value upper: {fragmentImplementationUpper:X}");
                ECU ecu = ParentDomain.ParentECU;
                byte[] infoPool = ecu.ReadECUInfoPool(reader);
                // int infoEntryWidth = ecu.ecuInfoPool_tableEntrySize;
                // Console.WriteLine($"Info entry width: {infoEntryWidth}"); // 8

                using (BinaryReader poolReader = new BinaryReader(new MemoryStream(infoPool))) 
                {
                    DiagPresentation pres = ParentECU.GlobalInternalPresentations[InfoPoolIndex];
                    /*
                    // depreciate use of ReadCBFWithOffset
                    poolReader.BaseStream.Seek(ecu.Info_EntrySize * InfoPoolIndex, SeekOrigin.Begin);
                    int presentationStructOffset = poolReader.ReadInt32();
                    int presentationStructSize = poolReader.ReadInt32();

                    //Console.WriteLine($"struct offset: 0x{presentationStructOffset:X} , size: {presentationStructSize} , meaningA 0x{fragmentMeaningA_Presentation:X} infoBase 0x{ecu.ecuInfoPool_fileoffset_7:X}\n");

                    reader.BaseStream.Seek(presentationStructOffset + ecu.Info_BlockOffset, SeekOrigin.Begin);
                    byte[] presentationStruct = reader.ReadBytes(presentationStructSize);

                    int presentationMode = CaesarStructure.ReadCBFWithOffset(0x1C, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // PRESS_Type
                    int presentationLength = CaesarStructure.ReadCBFWithOffset(0x1A, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // PRESS_TypeLength
                    if (presentationLength > 0)
                    {
                        BitLength = presentationLength;
                    }
                    else 
                    {
                        BitLength = CaesarStructure.ReadCBFWithOffset(0x21, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // ???
                    }
                    */
                    BitLength = pres.TypeLength_1A > 0 ? pres.TypeLength_1A : pres.TypeLengthBytesMaybe_21;
                    // if value was specified in bytes, convert to bits
                    if (pres.Type_1C == 0)
                    {
                        BitLength *= 8;
                    }
                }
            }
            else
            {
                if (ImplementationUpper == 0x420)
                {
                    BitLength = FragmentLengthTable[ImplementationLower];
                }
                else if (ImplementationUpper == 0x320)
                {
                    BitLength = FragmentLengthTable[ImplementationLower];
                }
                else if (ImplementationUpper == 0x330)
                {
                    BitLength = RawBitLength;
                }
                else if (ImplementationUpper == 0x340)
                {
                    throw new NotImplementedException("Requires implementation of ITT handle");
                }
                else
                {
                    throw new NotImplementedException($"No known fragment length format. Fragment upper: 0x{ImplementationUpper:X}");
                }
            }

            if (BitLength == 0)
            {
                // not sure if there are dummy entries that might trip below exception
                // throw new NotImplementedException("Fragment length cannot be zero");
            }
        }

        public void PrintDebug(bool verbose=false)
        {
            if (verbose)
            {

                Console.WriteLine($"{nameof(ByteBitPos)} : {ByteBitPos}");
                Console.WriteLine($"{nameof(BitLength)} : {BitLength}");
                Console.WriteLine($"{nameof(ImplementationType)} : {ImplementationType}");
                Console.WriteLine($"{nameof(ImplementationUpper)} : 0x{ImplementationUpper:X}");

                Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
                Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
                Console.WriteLine($"{nameof(ReadAccessLevel)} : {ReadAccessLevel}");
                Console.WriteLine($"{nameof(WriteAccessLevel)} : {WriteAccessLevel}");
                Console.WriteLine($"{nameof(ByteOrder)} : {ByteOrder}");
                Console.WriteLine($"{nameof(RawBitLength)} : {RawBitLength}");
                Console.WriteLine($"{nameof(IttOffset)} : {IttOffset}");
                Console.WriteLine($"{nameof(InfoPoolIndex)} : {InfoPoolIndex}");
                Console.WriteLine($"{nameof(MeaningB)} : {MeaningB}");
                Console.WriteLine($"{nameof(MeaningC)} : {MeaningC}");
                Console.WriteLine($"{nameof(CCFHandle)} : {CCFHandle}");
                Console.WriteLine($"{nameof(VarcodeDumpSize)} : {VarcodeDumpSize}");
                Console.WriteLine($"{nameof(VarcodeDump)} : {BitUtility.BytesToHex(VarcodeDump)}");
                Console.WriteLine($"{nameof(SubfragmentCount)} : {SubfragmentCount}");
                Console.WriteLine($"{nameof(SubfragmentFileOffset)} : 0x{SubfragmentFileOffset:X}");
                Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            }
            else 
            {
                Console.WriteLine($"{Qualifier}%{ByteBitPos}%{BitLength}%[{ImplementationUpper:X}/{ImplementationLower:X}]");
            }

        }
    }
}
