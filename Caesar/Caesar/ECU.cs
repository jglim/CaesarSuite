using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class ECU
    {
        public string ecuName;
        public int ECUName_T;
        public int ECUDescription_T;
        public string ecuXmlVersion;
        public int interfaceBlockCount;
        public int interfaceTableOffset;
        public int ecuNumberOfEcuChildTypes;
        public int ecuChildTypesOffset;
        public string ecuClassName;
        public string ecuIdk7;
        public string ecuIdk8;

        public int ecuIdk_1;
        public int ecuIdk_2;
        public int ecuIdk_3_qwer_count;
        public int ecuIdk_4_qwer_offset;
        public int ecuSgmlSource;
        public int ecuIdk_6_reloffset;

        public int ecuvariant_fileoffset_1; // 1
        public int ecuvariant_tableEntryCount;
        public int ecuvariant_tableEntrySize;
        public int ecuvariant_tableSize;

        public int ecuIdk_11_fileoffset_2; // 2
        public int ecuIdk_12_tableEntryCount;
        public int ecuIdk_13_tableEntrySize;
        public int ecuIdk_14_tableSize;

        public int ecuIdk_15_fileoffset_3; // 3
        public int ecuIdk_16_tableEntryCount;
        public int ecuIdk_17_tableEntrySize;
        public int ecuIdk_18_tableSize;

        public int openecu_fileoffset_4; // 4
        public int ecuIdk_20_tableEntryCount;
        public int ecuIdk_21_tableEntrySize;
        public int ecuIdk_22_tableSize;

        public int varcoding_fileoffset_5; // 5 , 0x15716
        public int varcoding_tableEntryCount; // [1], 43 0x2B
        public int varcoding_tableEntrySize; // [2], 12 0xC (multiply with [1] for size), 43*12=516 = 0x204
        public int ecuIdk_26_tableSize; // [3] unused
        // there is an invisible DWORD here that stores the cached read data's ptr
        
        public int ecuIdk_27_fileoffset_6;
        public int ecuIdk_28_tableEntryCount;
        public int ecuIdk_29_tableEntrySize;
        public int ecuIdk_30_tableSize;

        public int ecuInfoPool_fileoffset_7; // 31
        public int ecuInfoPool_tableEntryCount; // 32
        public int ecuInfoPool_tableEntrySize; // 33
        public int ecuInfoPool_tableSize; // 34

        public int ecuIdk_35_fileoffset_8;
        public int ecuIdk_36_tableEntryCount;
        public int ecuIdk_37_tableEntrySize;
        public int ecuIdk_38_tableSize;
        public int ecuIdk_39;

        public List<ECUInterface> ECUInterfaces = new List<ECUInterface>();
        public List<ECUInterfaceSubtype> ECUInterfaceSubtypes = new List<ECUInterfaceSubtype>();
        public List<ECUVariant> ECUVariants = new List<ECUVariant>();

        public long BaseAddress;

        byte[] cachedVarcodingPool = new byte[] { };
        byte[] cachedVariantPool = new byte[] { };
        byte[] cachedEcuInfoPool = new byte[] { };

        public string ECUDescriptionTranslated = "";

        public byte[] ReadVariantPool(BinaryReader reader)
        {
            if (cachedVariantPool.Length == 0)
            {
                cachedVariantPool = ReadEcuPool(reader, ecuvariant_fileoffset_1, ecuvariant_tableEntryCount, ecuvariant_tableEntrySize);
            }
            return cachedVariantPool;
        }

        public byte[] ReadVarcodingPool(BinaryReader reader)
        {
            if (cachedVarcodingPool.Length == 0)
            {
                cachedVarcodingPool = ReadEcuPool(reader, varcoding_fileoffset_5, varcoding_tableEntryCount, varcoding_tableEntrySize);
            }
            return cachedVarcodingPool;
        }
        // don't actually know what the proper name is, using "ECUInfo" for now
        public byte[] ReadECUInfoPool(BinaryReader reader)
        {
            if (cachedEcuInfoPool.Length == 0)
            {
                cachedEcuInfoPool = ReadEcuPool(reader, ecuInfoPool_fileoffset_7, ecuInfoPool_tableEntryCount, ecuInfoPool_tableEntrySize);
            }
            return cachedEcuInfoPool;
        }

        public byte[] ReadEcuPool(BinaryReader reader, long addressToReadFrom, int multiplier1, int multiplier2) 
        {
            reader.BaseStream.Seek(addressToReadFrom, SeekOrigin.Begin);
            return reader.ReadBytes(multiplier1 * multiplier2);
        }

        public ECU(BinaryReader reader, CTFLanguage language, CFFHeader header, long baseAddress)  
        {
            BaseAddress = baseAddress;
            // Read 32+16 bits
            ulong ecuBitFlags = reader.ReadUInt32();
            // after exhausting the 32 bits, load these additional 16 bits
            ulong ecuBitFlagsExtended = reader.ReadUInt16();

            // Console.WriteLine($"ECU bitflags: {ecuBitFlags:X}");

            // advancing forward to ecuBase + 10
            int ecuHdrIdk1 = reader.ReadInt32(); // no idea
            // Console.WriteLine($"Skipping: {ecuHdrIdk1:X8}");

            ecuName = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            ECUName_T = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader, -1);
            ECUDescription_T = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader, -1);
            ecuXmlVersion = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            interfaceBlockCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            interfaceTableOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuNumberOfEcuChildTypes = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuChildTypesOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuClassName = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            ecuIdk7 = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            ecuIdk8 = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);

            int dataBufferOffsetRelativeToFile = header.sizeOfStringPool + StubHeader.StubHeaderSize + header.cffHeaderSize + 4;
            // Console.WriteLine($"{nameof(dataBufferOffsetRelativeToFile)} : 0x{dataBufferOffsetRelativeToFile:X}");

            ecuIdk_1 = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_2 = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_3_qwer_count = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_4_qwer_offset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuSgmlSource = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_6_reloffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuvariant_fileoffset_1 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuvariant_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuvariant_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuvariant_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_11_fileoffset_2 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_12_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_13_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_14_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_15_fileoffset_3 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_16_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_17_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_18_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            openecu_fileoffset_4 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_20_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_21_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            // bitflags will be exhausted at this point, load the extended bitflags
            ecuBitFlags = ecuBitFlagsExtended;

            ecuIdk_22_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            varcoding_fileoffset_5 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            varcoding_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            varcoding_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_26_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_27_fileoffset_6 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_28_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_29_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_30_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuInfoPool_fileoffset_7 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuInfoPool_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuInfoPool_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuInfoPool_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_35_fileoffset_8 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_36_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_37_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_38_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_39 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            // read ecu's supported interfaces and subtypes

            // try to read interface block from the interface buffer table
            // this address is relative to the definitions block
            long interfaceTableAddress = BaseAddress + interfaceTableOffset;
            // Console.WriteLine($"Interface table address: {interfaceTableAddress:X}, given offset: {interfaceTableOffset:X}");

            ECUInterfaces = new List<ECUInterface>();
            for (int interfaceBufferIndex = 0; interfaceBufferIndex < interfaceBlockCount; interfaceBufferIndex++)
            {
                // Console.WriteLine($"Parsing interface {interfaceBufferIndex + 1}/{interfaceBlockCount}");

                // find our interface block offset
                reader.BaseStream.Seek(interfaceTableAddress + (interfaceBufferIndex * 4), SeekOrigin.Begin);
                // seek to the actual block (ambiguity: is this relative to the interface table or the current array?)
                int interfaceBlockOffset = reader.ReadInt32();
                reader.BaseStream.Seek(interfaceTableAddress + interfaceBlockOffset, SeekOrigin.Begin);

                ECUInterface ecuInterface = new ECUInterface(reader, reader.BaseStream.Position);
                ECUInterfaces.Add(ecuInterface);
            }

            // try to read interface subtype block from the interface buffer table
            // this address is relative to the definitions block
            ECUInterfaceSubtypes = new List<ECUInterfaceSubtype>();
            long ctTableAddress = BaseAddress + ecuChildTypesOffset;
            // Console.WriteLine($"Interface subtype table address: {ctTableAddress:X}, given offset: {ecuChildTypesOffset:X}");
            for (int ctBufferIndex = 0; ctBufferIndex < ecuNumberOfEcuChildTypes; ctBufferIndex++)
            {
                // Console.WriteLine($"Parsing interface subtype {ctBufferIndex + 1}/{ecuNumberOfEcuChildTypes}");
                // find our ct block offset
                reader.BaseStream.Seek(ctTableAddress + (ctBufferIndex * 4), SeekOrigin.Begin);
                // seek to the actual block (ambiguity: is this relative to the ct table or the current array?)
                int actualBlockOffset = reader.ReadInt32();
                reader.BaseStream.Seek(ctTableAddress + actualBlockOffset, SeekOrigin.Begin);

                ECUInterfaceSubtype ecuInterfaceSubtype = new ECUInterfaceSubtype(reader, reader.BaseStream.Position);
                ECUInterfaceSubtypes.Add(ecuInterfaceSubtype);

            }
            /*
            // unknown "qwer" block
            if (ecuIdk_3_qwer_count > 0)
            {
                Console.WriteLine("QWER block found, intentionally throwing exception");
                throw new NotImplementedException("yay qwer");
            }
            else
            {
                Console.WriteLine("QWER block is absent");
            }
            */
            //PrintDebug();
            CreateVariants(reader, language);
            ECUDescriptionTranslated = language.GetString(ECUDescription_T);
        }

        public void CreateVariants(BinaryReader reader, CTFLanguage language) 
        {
            ECUVariants.Clear();
            byte[] ecuVariantPool = ReadVariantPool(reader);

            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(ecuVariantPool)))
            {
                for (int ecuVariantIndex = 0; ecuVariantIndex < ecuvariant_tableEntryCount; ecuVariantIndex++)
                {
                    poolReader.BaseStream.Seek(ecuVariantIndex * ecuvariant_tableEntrySize, SeekOrigin.Begin);
                    
                    int entryOffset = poolReader.ReadInt32();
                    int entrySize = poolReader.ReadInt32();
                    ushort poolEntryAttributes = poolReader.ReadUInt16();
                    long variantBlockAddress = entryOffset + ecuvariant_fileoffset_1;

                    ECUVariant variant = new ECUVariant(reader, this, language, variantBlockAddress, entrySize);
                    ECUVariants.Add(variant);
                    // Console.WriteLine($"Variant Entry @ 0x{entryOffset:X} with size 0x{entrySize:X} and CRC {poolEntryAttributes:X8}, abs addr {variantBlockAddress:X8}");

#if DEBUG
                    int resultLimit = 1999;
                    if (ecuVariantIndex >= resultLimit)
                    {
                        Console.WriteLine($"Breaking prematurely to create only {resultLimit} variant(s) (debug)");
                        break;
                    }
#endif
                }
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine($"ECU Name: {ecuName}");
            Console.WriteLine($"{nameof(ECUName_T)} : {ECUName_T}");
            Console.WriteLine($"{nameof(ECUDescription_T)} : {ECUDescription_T}");
            Console.WriteLine($"ECU ecuXmlVersion: {ecuXmlVersion}");
            Console.WriteLine($"{nameof(interfaceBlockCount)} : {interfaceBlockCount}");
            Console.WriteLine($"{nameof(interfaceTableOffset)} : 0x{interfaceTableOffset:X}");
            Console.WriteLine($"{nameof(ecuNumberOfEcuChildTypes)} : {ecuNumberOfEcuChildTypes}");
            Console.WriteLine($"{nameof(ecuChildTypesOffset)} : {ecuChildTypesOffset}");
            Console.WriteLine($"ECU ecuClassName: {ecuClassName}");
            Console.WriteLine($"ECU ecuIdk7: {ecuIdk7}");
            Console.WriteLine($"ECU ecuIdk8: {ecuIdk8}");


            Console.WriteLine($"{nameof(ecuIdk_1)} : {ecuIdk_1}");
            Console.WriteLine($"{nameof(ecuIdk_2)} : {ecuIdk_2}");
            Console.WriteLine($"{nameof(ecuIdk_3_qwer_count)} : {ecuIdk_3_qwer_count}");
            Console.WriteLine($"{nameof(ecuIdk_4_qwer_offset)} : {ecuIdk_4_qwer_offset}");
            Console.WriteLine($"{nameof(ecuSgmlSource)} : {ecuSgmlSource}");
            Console.WriteLine($"{nameof(ecuIdk_6_reloffset)} : 0x{ecuIdk_6_reloffset:X}");
            Console.WriteLine($"{nameof(ecuvariant_fileoffset_1)} : 0x{ecuvariant_fileoffset_1:X}");
            Console.WriteLine($"{nameof(ecuvariant_tableEntryCount)} : {ecuvariant_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuvariant_tableEntrySize)} : {ecuvariant_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuvariant_tableSize)} : 0x{ecuvariant_tableSize:X}");
            Console.WriteLine($"{nameof(ecuIdk_11_fileoffset_2)} : 0x{ecuIdk_11_fileoffset_2:X}");
            Console.WriteLine($"{nameof(ecuIdk_12_tableEntryCount)} : {ecuIdk_12_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_13_tableEntrySize)} : {ecuIdk_13_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_14_tableSize)} : 0x{ecuIdk_14_tableSize:X}");
            Console.WriteLine($"{nameof(ecuIdk_15_fileoffset_3)} : 0x{ecuIdk_15_fileoffset_3:X}");
            Console.WriteLine($"{nameof(ecuIdk_16_tableEntryCount)} : {ecuIdk_16_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_17_tableEntrySize)} : {ecuIdk_17_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_18_tableSize)} : 0x{ecuIdk_18_tableSize:X}");
            Console.WriteLine($"{nameof(openecu_fileoffset_4)} : 0x{openecu_fileoffset_4:X}");
            Console.WriteLine($"{nameof(ecuIdk_20_tableEntryCount)} : {ecuIdk_20_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_21_tableEntrySize)} : {ecuIdk_21_tableEntrySize}");

            // Console.WriteLine("--- bitflag load 2 ---");

            Console.WriteLine($"{nameof(ecuIdk_22_tableSize)} : 0x{ecuIdk_22_tableSize:X}");
            Console.WriteLine($"{nameof(varcoding_fileoffset_5)} : 0x{varcoding_fileoffset_5:X}");
            Console.WriteLine($"{nameof(varcoding_tableEntryCount)} : {varcoding_tableEntryCount}");
            Console.WriteLine($"{nameof(varcoding_tableEntrySize)} : {varcoding_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_26_tableSize)} : 0x{ecuIdk_26_tableSize:X}");
            Console.WriteLine($"{nameof(ecuIdk_27_fileoffset_6)} : 0x{ecuIdk_27_fileoffset_6:X}");
            Console.WriteLine($"{nameof(ecuIdk_28_tableEntryCount)} : {ecuIdk_28_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_29_tableEntrySize)} : {ecuIdk_29_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_30_tableSize)} : 0x{ecuIdk_30_tableSize:X}");
            Console.WriteLine($"{nameof(ecuInfoPool_fileoffset_7)} : 0x{ecuInfoPool_fileoffset_7:X}");
            Console.WriteLine($"{nameof(ecuInfoPool_tableEntryCount)} : {ecuInfoPool_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuInfoPool_tableEntrySize)} : {ecuInfoPool_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuInfoPool_tableSize)} : 0x{ecuInfoPool_tableSize:X}");
            Console.WriteLine($"{nameof(ecuIdk_35_fileoffset_8)} : 0x{ecuIdk_35_fileoffset_8:X}");
            Console.WriteLine($"{nameof(ecuIdk_36_tableEntryCount)} : {ecuIdk_36_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_37_tableEntrySize)} : {ecuIdk_37_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_38_tableSize)} : {ecuIdk_38_tableSize}");
            Console.WriteLine($"{nameof(ecuIdk_39)} : {ecuIdk_39}");
        }
    }
}
