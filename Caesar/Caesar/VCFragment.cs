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
        public int fragmentByteBitPos;
        public ushort implementationType;

        public int fragmentName_T;
        public int fragmentLongName_T;
        public int fragmentReadAccessLevel;
        public int fragmentWriteAccessLevel;
        public int fragmentByteOrder;
        public int fragmentRawBitLength;
        public int fragmentIttOffset;
        public int fragmentMeaningA_Presentation;
        public int fragmentMeaningB;
        public int fragmentMeaningC;
        public int fragmentCCFHandle;
        public int fragmentVarcodeDumpSize;
        public byte[] varcodeDump;
        public int fragmentNoOfSubFragments;
        public long fragmentSubfragmentFileOffset;
        public string fragmentName;

        public int fragmentImplementationUpper;
        public int fragmentImplementationLower;

        public int fragmentBitLength;

        public List<VCSubfragment> Subfragments = new List<VCSubfragment>();

        public static readonly byte[] FragmentLengthTable = new byte[] { 0, 1, 4, 8, 0x10, 0x20, 0x40 };
        public VCDomain ParentDomain;

        public VCFragment(BinaryReader reader, VCDomain parentDomain, long fragmentTable, int fragmentIndex, CTFLanguage language) 
        {
            // see DIOpenVarCodeFrag
            ParentDomain = parentDomain;

            long fragmentTableEntry = fragmentTable + (10 * fragmentIndex);
            reader.BaseStream.Seek(fragmentTableEntry, SeekOrigin.Begin);
            // no bitflag required for 10-byte table entry since it is mandatory
            int fragmentNewBaseOffset = reader.ReadInt32();

            fragmentByteBitPos = reader.ReadInt32();
            implementationType = reader.ReadUInt16();

            // Console.WriteLine($"Fragment new base @ 0x{fragmentNewBaseOffset:X}, byteBitPos 0x{fragmentByteBitPos:X}, implementationType: 0x{implementationType:X}");
            long fragmentBaseAddress = fragmentTable + fragmentNewBaseOffset;
            reader.BaseStream.Seek(fragmentBaseAddress, SeekOrigin.Begin);
            ulong fragmentBitflags = reader.ReadUInt32();
            // Console.WriteLine($"Fragment new bitflag @ 0x{fragmentBitflags:X}");

            fragmentName_T = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            fragmentLongName_T = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            fragmentReadAccessLevel = CaesarReader.ReadBitflagUInt8(ref fragmentBitflags, reader);
            fragmentWriteAccessLevel = CaesarReader.ReadBitflagUInt8(ref fragmentBitflags, reader);
            fragmentByteOrder = CaesarReader.ReadBitflagUInt16(ref fragmentBitflags, reader);
            fragmentRawBitLength = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            fragmentIttOffset = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            fragmentMeaningA_Presentation = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            fragmentMeaningB = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            fragmentMeaningC = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader, -1);
            fragmentCCFHandle = CaesarReader.ReadBitflagInt16(ref fragmentBitflags, reader, -1);
            fragmentVarcodeDumpSize = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            varcodeDump = CaesarReader.ReadBitflagDumpWithReader(ref fragmentBitflags, reader, fragmentVarcodeDumpSize, fragmentBaseAddress);
            fragmentNoOfSubFragments = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            fragmentSubfragmentFileOffset = CaesarReader.ReadBitflagInt32(ref fragmentBitflags, reader);
            fragmentName = CaesarReader.ReadBitflagStringWithReader(ref fragmentBitflags, reader, fragmentBaseAddress);

            // Console.WriteLine($"{nameof(fragmentName)} : {fragmentName}, child {fragmentNoOfSubFragments} @ 0x{fragmentSubfragmentFileOffset:X} base {fragmentBaseAddress:X}");

            
            if ((fragmentByteOrder != 0) && (fragmentBitLength > 0)) 
            {
                //throw new Exception("Currently assumes everything is little-endian");
                Console.WriteLine($"WARNING: {fragmentName} (Size: {fragmentBitLength}) has an unsupported byte order. Please proceed with caution");
                //PrintDebug(true);
            }
            

            long subfragmentTableAddress = fragmentSubfragmentFileOffset + fragmentBaseAddress;
            Subfragments.Clear();
            for (int subfragmentIndex = 0; subfragmentIndex < fragmentNoOfSubFragments; subfragmentIndex++)
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
            byte[] affectedBits = variantBits.Skip(fragmentByteBitPos).Take(fragmentBitLength).ToArray();

            foreach (VCSubfragment subfragment in Subfragments)
            {
                byte[] sfToCompare = BitUtility.ByteArrayToBitArray(subfragment.subfragmentDump).Take(fragmentBitLength).ToArray();
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
                if (subfragment.subfragmentNameResolved == subfragmentName)
                {
                    return SetSubfragmentConfiguration(variantCodingValue, subfragment);
                }
            }
            throw new FormatException($"Requested subfragment {subfragmentName} could not be found in {fragmentName}");
        }

        public byte[] SetSubfragmentConfiguration(byte[] variantCodingValue, VCSubfragment subfragment)
        {
            byte[] variantBits = BitUtility.ByteArrayToBitArray(variantCodingValue);
            List<byte> result = new List<byte>(variantBits.Take(fragmentByteBitPos));
            variantBits = variantBits.Skip(fragmentBitLength + fragmentByteBitPos).ToArray();
            byte[] sfToSet = BitUtility.ByteArrayToBitArray(subfragment.subfragmentDump).Take(fragmentBitLength).ToArray();
            result.AddRange(sfToSet);
            result.AddRange(variantBits);
            return BitUtility.BitArrayToByteArray(result.ToArray());
        }

        private void FindFragmentSize(BinaryReader reader) 
        {
            fragmentImplementationUpper = implementationType & 0xFF0;
            fragmentImplementationLower = implementationType & 0xF;
            fragmentBitLength = 0;

            // fixup the bit length
            if (fragmentImplementationLower > 6)
            {
                throw new NotImplementedException("The disassembly throws an exception when fragmentImplementationLower > 6, copying verbatim");
            }

            if (fragmentImplementationUpper > 0x420)
            {
                ECU ecu = ParentDomain.ParentECU;
                byte[] infoPool = ecu.ReadECUInfoPool(reader);
                // int infoEntryWidth = ecu.ecuInfoPool_tableEntrySize;
                // Console.WriteLine($"Info entry width: {infoEntryWidth}"); // 8

                using (BinaryReader poolReader = new BinaryReader(new MemoryStream(infoPool))) 
                {
                    poolReader.BaseStream.Seek(ecu.ecuInfoPool_tableEntrySize * fragmentMeaningA_Presentation, SeekOrigin.Begin);
                    int presentationStructOffset = poolReader.ReadInt32();
                    int presentationStructSize = poolReader.ReadInt32();

                    //Console.WriteLine($"struct offset: 0x{presentationStructOffset:X} , size: {presentationStructSize} , meaningA 0x{fragmentMeaningA_Presentation:X} infoBase 0x{ecu.ecuInfoPool_fileoffset_7:X}\n");

                    reader.BaseStream.Seek(presentationStructOffset + ecu.ecuInfoPool_fileoffset_7, SeekOrigin.Begin);
                    byte[] presentationStruct = reader.ReadBytes(presentationStructSize);

                    int presentationMode = CaesarStructure.ReadCBFWithOffset(0x1C, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // PRESS_Type
                    int presentationLength = CaesarStructure.ReadCBFWithOffset(0x1A, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // PRESS_TypeLength
                    if (presentationLength > 0)
                    {
                        fragmentBitLength = presentationLength;
                    }
                    else 
                    {
                        fragmentBitLength = CaesarStructure.ReadCBFWithOffset(0x21, CaesarStructure.StructureName.PRESENTATION_STRUCTURE, presentationStruct); // ???

                    }
                    // if value was specified in bytes, convert to bits
                    if (presentationMode == 0)
                    {
                        fragmentBitLength *= 8;
                    }
                }
            }
            else
            {
                if (fragmentImplementationUpper == 0x420)
                {
                    fragmentBitLength = FragmentLengthTable[fragmentImplementationLower];
                }
                else if (fragmentImplementationUpper == 0x320)
                {
                    fragmentBitLength = FragmentLengthTable[fragmentImplementationLower];
                }
                else if (fragmentImplementationUpper == 0x330)
                {
                    fragmentBitLength = fragmentRawBitLength;
                }
                else if (fragmentImplementationUpper == 0x340)
                {
                    throw new NotImplementedException("Requires implementation of ITT handle");
                }
                else
                {
                    throw new NotImplementedException($"No known fragment length format. Fragment upper: 0x{fragmentImplementationUpper:X}");
                }
            }

            if (fragmentBitLength == 0)
            {
                // not sure if there are dummy entries that might trip below exception
                // throw new NotImplementedException("Fragment length cannot be zero");
            }
        }

        public void PrintDebug(bool verbose=false)
        {
            if (verbose)
            {

                Console.WriteLine($"{nameof(fragmentByteBitPos)} : {fragmentByteBitPos}");
                Console.WriteLine($"{nameof(fragmentBitLength)} : {fragmentBitLength}");
                Console.WriteLine($"{nameof(implementationType)} : {implementationType}");
                Console.WriteLine($"{nameof(fragmentImplementationUpper)} : 0x{fragmentImplementationUpper:X}");

                Console.WriteLine($"{nameof(fragmentName_T)} : {fragmentName_T}");
                Console.WriteLine($"{nameof(fragmentLongName_T)} : {fragmentLongName_T}");
                Console.WriteLine($"{nameof(fragmentReadAccessLevel)} : {fragmentReadAccessLevel}");
                Console.WriteLine($"{nameof(fragmentWriteAccessLevel)} : {fragmentWriteAccessLevel}");
                Console.WriteLine($"{nameof(fragmentByteOrder)} : {fragmentByteOrder}");
                Console.WriteLine($"{nameof(fragmentRawBitLength)} : {fragmentRawBitLength}");
                Console.WriteLine($"{nameof(fragmentIttOffset)} : {fragmentIttOffset}");
                Console.WriteLine($"{nameof(fragmentMeaningA_Presentation)} : {fragmentMeaningA_Presentation}");
                Console.WriteLine($"{nameof(fragmentMeaningB)} : {fragmentMeaningB}");
                Console.WriteLine($"{nameof(fragmentMeaningC)} : {fragmentMeaningC}");
                Console.WriteLine($"{nameof(fragmentCCFHandle)} : {fragmentCCFHandle}");
                Console.WriteLine($"{nameof(fragmentVarcodeDumpSize)} : {fragmentVarcodeDumpSize}");
                Console.WriteLine($"{nameof(varcodeDump)} : {BitUtility.BytesToHex(varcodeDump)}");
                Console.WriteLine($"{nameof(fragmentNoOfSubFragments)} : {fragmentNoOfSubFragments}");
                Console.WriteLine($"{nameof(fragmentSubfragmentFileOffset)} : 0x{fragmentSubfragmentFileOffset:X}");
                Console.WriteLine($"{nameof(fragmentName)} : {fragmentName}");
            }
            else 
            {
                Console.WriteLine($"{fragmentName}%{fragmentByteBitPos}%{fragmentBitLength}%[{fragmentImplementationUpper:X}/{fragmentImplementationLower:X}]");
            }

        }
    }
}
