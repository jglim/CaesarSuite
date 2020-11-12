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
        public string Qualifier;
        public int EcuName_CTF;
        public int EcuDescription_CTF;
        public string EcuXmlVersion;
        public int InterfaceBlockCount;
        public int InterfaceTableOffset;
        public int SubinterfacesCount;
        public int SubinterfacesOffset;
        public string EcuClassName;
        public string UnkStr7;
        public string UnkStr8;

        public int IgnitionRequired;
        public int Unk2;
        public int UnkBlockCount;
        public int UnkBlockOffset;
        public int EcuSgmlSource;
        public int Unk6RelativeOffset;

        public int EcuVariant_BlockOffset; // 1
        public int EcuVariant_EntryCount;
        public int EcuVariant_EntrySize;
        public int EcuVariant_BlockSize;

        public int DiagJob_BlockOffset; // 2
        public int DiagJob_EntryCount;
        public int DiagJob_EntrySize;
        public int DiagJob_BlockSize;

        public int Dtc_BlockOffset; // 3
        public int Dtc_EntryCount;
        public int Dtc_EntrySize;
        public int Dtc_BlockSize;

        public int Env_BlockOffset; // 4
        public int Env_EntryCount;
        public int Env_EntrySize;
        public int Env_BlockSize;

        public int VcDomain_BlockOffset; // 5 , 0x15716
        public int VcDomain_EntryCount; // [1], 43 0x2B
        public int VcDomain_EntrySize; // [2], 12 0xC (multiply with [1] for size), 43*12=516 = 0x204
        public int VcDomain_BlockSize; // [3] unused

        public int Presenations_BlockOffset;
        public int Presentations_EntryCount;
        public int Presentations_EntrySize;
        public int Presentations_BlockSize;

        public int Info_BlockOffset; // 31
        public int Info_EntryCount; // 32
        public int Info_EntrySize; // 33
        public int Info_BlockSize; // 34

        public int Unk_BlockOffset;
        public int Unk_EntryCount;
        public int Unk_EntrySize;
        public int Unk_BlockSize;

        public int Unk39;

        public List<ECUInterface> ECUInterfaces = new List<ECUInterface>();
        public List<ECUInterfaceSubtype> ECUInterfaceSubtypes = new List<ECUInterfaceSubtype>();
        public List<ECUVariant> ECUVariants = new List<ECUVariant>();

        public List<DiagService> GlobalDiagServices = new List<DiagService>();

        public long BaseAddress;

        byte[] cachedVarcodingPool = new byte[] { };
        byte[] cachedVariantPool = new byte[] { };
        byte[] cachedDiagjobPool = new byte[] { };
        byte[] cachedEcuInfoPool = new byte[] { };
        byte[] cachedPresentationsPool = new byte[] { };
        byte[] cachedEnvPool = new byte[] { };

        public CaesarContainer ParentContainer;

        public string ECUDescriptionTranslated = "";

        public byte[] ReadDiagjobPool(BinaryReader reader)
        {
            if (cachedDiagjobPool.Length == 0)
            {
                cachedDiagjobPool = ReadEcuPool(reader, DiagJob_BlockOffset, DiagJob_EntryCount, DiagJob_EntrySize);
            }
            return cachedDiagjobPool;
        }

        public byte[] ReadVariantPool(BinaryReader reader)
        {
            if (cachedVariantPool.Length == 0)
            {
                cachedVariantPool = ReadEcuPool(reader, EcuVariant_BlockOffset, EcuVariant_EntryCount, EcuVariant_EntrySize);
            }
            return cachedVariantPool;
        }

        public byte[] ReadVarcodingPool(BinaryReader reader)
        {
            if (cachedVarcodingPool.Length == 0)
            {
                cachedVarcodingPool = ReadEcuPool(reader, VcDomain_BlockOffset, VcDomain_EntryCount, VcDomain_EntrySize);
            }
            return cachedVarcodingPool;
        }
        // don't actually know what the proper name is, using "ECUInfo" for now
        public byte[] ReadECUInfoPool(BinaryReader reader)
        {
            if (cachedEcuInfoPool.Length == 0)
            {
                cachedEcuInfoPool = ReadEcuPool(reader, Info_BlockOffset, Info_EntryCount, Info_EntrySize);
            }
            return cachedEcuInfoPool;
        }
        public byte[] ReadECUPresentationsPool(BinaryReader reader)
        {
            if (cachedPresentationsPool.Length == 0)
            {
                cachedPresentationsPool = ReadEcuPool(reader, Presenations_BlockOffset, Presentations_EntryCount, Presentations_EntrySize);
            }
            return cachedPresentationsPool;
        }
        public byte[] ReadECUEnvPool(BinaryReader reader)
        {
            if (cachedEnvPool.Length == 0)
            {
                cachedEnvPool = ReadEcuPool(reader, Env_BlockOffset, Env_EntryCount, Env_EntrySize);
            }
            return cachedEnvPool;
        }

        public byte[] ReadEcuPool(BinaryReader reader, long addressToReadFrom, int multiplier1, int multiplier2) 
        {
            reader.BaseStream.Seek(addressToReadFrom, SeekOrigin.Begin);
            return reader.ReadBytes(multiplier1 * multiplier2);
        }

        public ECU(BinaryReader reader, CTFLanguage language, CFFHeader header, long baseAddress, CaesarContainer parentContainer)  
        {
            ParentContainer = parentContainer;
            BaseAddress = baseAddress;
            // Read 32+16 bits
            ulong ecuBitFlags = reader.ReadUInt32();
            // after exhausting the 32 bits, load these additional 16 bits
            ulong ecuBitFlagsExtended = reader.ReadUInt16();

            // Console.WriteLine($"ECU bitflags: {ecuBitFlags:X}");

            // advancing forward to ecuBase + 10
            int ecuHdrIdk1 = reader.ReadInt32(); // no idea
            // Console.WriteLine($"Skipping: {ecuHdrIdk1:X8}");

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            EcuName_CTF = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader, -1);
            EcuDescription_CTF = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader, -1);
            EcuXmlVersion = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            InterfaceBlockCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            InterfaceTableOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            SubinterfacesCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            SubinterfacesOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            EcuClassName = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            UnkStr7 = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            UnkStr8 = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);

            int dataBufferOffsetRelativeToFile = header.StringPoolSize + StubHeader.StubHeaderSize + header.CffHeaderSize + 4;
            // Console.WriteLine($"{nameof(dataBufferOffsetRelativeToFile)} : 0x{dataBufferOffsetRelativeToFile:X}");

            IgnitionRequired = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            Unk2 = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            UnkBlockCount = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            UnkBlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            EcuSgmlSource = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            Unk6RelativeOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            EcuVariant_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            EcuVariant_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            EcuVariant_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            EcuVariant_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            DiagJob_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            DiagJob_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            DiagJob_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            DiagJob_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Dtc_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            Dtc_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Dtc_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Dtc_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Env_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            Env_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Env_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            // bitflags will be exhausted at this point, load the extended bitflags
            ecuBitFlags = ecuBitFlagsExtended;

            Env_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            VcDomain_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            VcDomain_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            VcDomain_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            VcDomain_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Presenations_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            Presentations_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Presentations_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Presentations_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Info_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            Info_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Info_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Info_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Unk_BlockOffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            Unk_EntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Unk_EntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            Unk_BlockSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            Unk39 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            // read ecu's supported interfaces and subtypes

            // try to read interface block from the interface buffer table
            // this address is relative to the definitions block
            long interfaceTableAddress = BaseAddress + InterfaceTableOffset;
            // Console.WriteLine($"Interface table address: {interfaceTableAddress:X}, given offset: {interfaceTableOffset:X}");

            ECUInterfaces = new List<ECUInterface>();
            for (int interfaceBufferIndex = 0; interfaceBufferIndex < InterfaceBlockCount; interfaceBufferIndex++)
            {
                // Console.WriteLine($"Parsing interface {interfaceBufferIndex + 1}/{interfaceBlockCount}");

                // find our interface block offset
                reader.BaseStream.Seek(interfaceTableAddress + (interfaceBufferIndex * 4), SeekOrigin.Begin);
                // seek to the actual block (ambiguity: is this relative to the interface table or the current array?)
                int interfaceBlockOffset = reader.ReadInt32();

                long ecuInterfaceBaseAddress = interfaceTableAddress + interfaceBlockOffset;

                ECUInterface ecuInterface = new ECUInterface(reader, ecuInterfaceBaseAddress);
                ECUInterfaces.Add(ecuInterface);
            }

            // try to read interface subtype block from the interface buffer table
            // this address is relative to the definitions block
            ECUInterfaceSubtypes = new List<ECUInterfaceSubtype>();
            long ctTableAddress = BaseAddress + SubinterfacesOffset;
            // Console.WriteLine($"Interface subtype table address: {ctTableAddress:X}, given offset: {ecuChildTypesOffset:X}");
            for (int ctBufferIndex = 0; ctBufferIndex < SubinterfacesCount; ctBufferIndex++)
            {
                // Console.WriteLine($"Parsing interface subtype {ctBufferIndex + 1}/{ecuNumberOfEcuChildTypes}");
                // find our ct block offset
                reader.BaseStream.Seek(ctTableAddress + (ctBufferIndex * 4), SeekOrigin.Begin);
                // seek to the actual block (ambiguity: is this relative to the ct table or the current array?)
                int actualBlockOffset = reader.ReadInt32();
                long ctBaseAddress = ctTableAddress + actualBlockOffset;

                ECUInterfaceSubtype ecuInterfaceSubtype = new ECUInterfaceSubtype(reader, ctBaseAddress, ctBufferIndex);
                ECUInterfaceSubtypes.Add(ecuInterfaceSubtype);
            }

            // unknown "qwer" block
            /*
            if (ecuIdk_3_qwer_count > 0)
            {
                Console.WriteLine("QWER block found, intentionally throwing exception");
                throw new NotImplementedException("yay qwer");
            }
            else
            {
                //Console.WriteLine("QWER block is absent");
            }
            */

            CreateDiagServices(reader, language);
            CreateEcuVariants(reader, language);
            ECUDescriptionTranslated = language.GetString(EcuDescription_CTF);
        }

        public void CreateDiagServices(BinaryReader reader, CTFLanguage language) 
        {
            byte[] diagjobPool = ReadDiagjobPool(reader);
            // arrays since list has become too expensive
            DiagService[] globalDiagServices = new DiagService[DiagJob_EntryCount];


            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(diagjobPool)))
            {
                for (int diagjobIndex = 0; diagjobIndex < DiagJob_EntryCount; diagjobIndex++)
                {
                    int offset = poolReader.ReadInt32();
                    int size = poolReader.ReadInt32();
                    uint crc = poolReader.ReadUInt32();
                    uint config = poolReader.ReadUInt16();
                    long diagjobBaseAddress = offset + DiagJob_BlockOffset;
                    // Console.WriteLine($"DJ @ {offset:X} with size {size:X}");

                    DiagService dj = new DiagService(reader, language, diagjobBaseAddress, diagjobIndex, this);
                    // GlobalDiagServices.Add(dj);
                    globalDiagServices[diagjobIndex] = dj;
                }
            }

            GlobalDiagServices = new List<DiagService>(globalDiagServices);
        }
        public void CreateEcuVariants(BinaryReader reader, CTFLanguage language) 
        {
            ECUVariants.Clear();
            byte[] ecuVariantPool = ReadVariantPool(reader);

            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(ecuVariantPool)))
            {
                for (int ecuVariantIndex = 0; ecuVariantIndex < EcuVariant_EntryCount; ecuVariantIndex++)
                {
                    poolReader.BaseStream.Seek(ecuVariantIndex * EcuVariant_EntrySize, SeekOrigin.Begin);
                    
                    int entryOffset = poolReader.ReadInt32();
                    int entrySize = poolReader.ReadInt32();
                    ushort poolEntryAttributes = poolReader.ReadUInt16();
                    long variantBlockAddress = entryOffset + EcuVariant_BlockOffset;

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
            Console.WriteLine($"ECU Name: {Qualifier}");
            Console.WriteLine($"{nameof(EcuName_CTF)} : {EcuName_CTF}");
            Console.WriteLine($"{nameof(EcuDescription_CTF)} : {EcuDescription_CTF}");
            Console.WriteLine($"ECU ecuXmlVersion: {EcuXmlVersion}");
            Console.WriteLine($"{nameof(InterfaceBlockCount)} : {InterfaceBlockCount}");
            Console.WriteLine($"{nameof(InterfaceTableOffset)} : 0x{InterfaceTableOffset:X}");
            Console.WriteLine($"{nameof(SubinterfacesCount)} : {SubinterfacesCount}");
            Console.WriteLine($"{nameof(SubinterfacesOffset)} : {SubinterfacesOffset}");
            Console.WriteLine($"ECU ecuClassName: {EcuClassName}");
            Console.WriteLine($"ECU ecuIdk7: {UnkStr7}");
            Console.WriteLine($"ECU ecuIdk8: {UnkStr8}");


            Console.WriteLine($"{nameof(IgnitionRequired)} : {IgnitionRequired}");
            
            Console.WriteLine($"{nameof(Unk2)} : {Unk2}"); 

            Console.WriteLine($"{nameof(UnkBlockCount)} : {UnkBlockCount}");
            Console.WriteLine($"{nameof(UnkBlockOffset)} : {UnkBlockOffset}");
            
            Console.WriteLine($"{nameof(EcuSgmlSource)} : {EcuSgmlSource}");
            Console.WriteLine($"{nameof(Unk6RelativeOffset)} : 0x{Unk6RelativeOffset:X}");
            
            Console.WriteLine($"{nameof(EcuVariant_BlockOffset)} : 0x{EcuVariant_BlockOffset:X}");
            Console.WriteLine($"{nameof(EcuVariant_EntryCount)} : {EcuVariant_EntryCount}");
            Console.WriteLine($"{nameof(EcuVariant_EntrySize)} : {EcuVariant_EntrySize}");
            Console.WriteLine($"{nameof(EcuVariant_BlockSize)} : 0x{EcuVariant_BlockSize:X}");
            Console.WriteLine($"{nameof(DiagJob_BlockOffset)} : 0x{DiagJob_BlockOffset:X}");
            Console.WriteLine($"{nameof(DiagJob_EntryCount)} : {DiagJob_EntryCount}");
            Console.WriteLine($"{nameof(DiagJob_EntrySize)} : {DiagJob_EntrySize}");
            Console.WriteLine($"{nameof(DiagJob_BlockSize)} : 0x{DiagJob_BlockSize:X}");
            Console.WriteLine($"{nameof(Dtc_BlockOffset)} : 0x{Dtc_BlockOffset:X}");
            Console.WriteLine($"{nameof(Dtc_EntryCount)} : {Dtc_EntryCount}");
            Console.WriteLine($"{nameof(Dtc_EntrySize)} : {Dtc_EntrySize}");
            Console.WriteLine($"{nameof(Dtc_BlockSize)} : 0x{Dtc_BlockSize:X}");
            Console.WriteLine($"{nameof(Env_BlockOffset)} : 0x{Env_BlockOffset:X}");
            Console.WriteLine($"{nameof(Env_EntryCount)} : {Env_EntryCount}");
            Console.WriteLine($"{nameof(Env_EntrySize)} : {Env_EntrySize}");

            // Console.WriteLine("--- bitflag load 2 ---");

            Console.WriteLine($"{nameof(Env_BlockSize)} : 0x{Env_BlockSize:X}");
            Console.WriteLine($"{nameof(VcDomain_BlockOffset)} : 0x{VcDomain_BlockOffset:X}");
            Console.WriteLine($"{nameof(VcDomain_EntryCount)} : {VcDomain_EntryCount}");
            Console.WriteLine($"{nameof(VcDomain_EntrySize)} : {VcDomain_EntrySize}");
            Console.WriteLine($"{nameof(VcDomain_BlockSize)} : 0x{VcDomain_BlockSize:X}");
            Console.WriteLine($"{nameof(Presenations_BlockOffset)} : 0x{Presenations_BlockOffset:X}");
            Console.WriteLine($"{nameof(Presentations_EntryCount)} : {Presentations_EntryCount}");
            Console.WriteLine($"{nameof(Presentations_EntrySize)} : {Presentations_EntrySize}");
            Console.WriteLine($"{nameof(Presentations_BlockSize)} : 0x{Presentations_BlockSize:X}");
            Console.WriteLine($"{nameof(Info_BlockOffset)} : 0x{Info_BlockOffset:X}");
            Console.WriteLine($"{nameof(Info_EntryCount)} : {Info_EntryCount}");
            Console.WriteLine($"{nameof(Info_EntrySize)} : {Info_EntrySize}");
            Console.WriteLine($"{nameof(Info_BlockSize)} : 0x{Info_BlockSize:X}");
            Console.WriteLine($"{nameof(Unk_BlockOffset)} : 0x{Unk_BlockOffset:X}");
            Console.WriteLine($"{nameof(Unk_EntryCount)} : {Unk_EntryCount}");
            Console.WriteLine($"{nameof(Unk_EntrySize)} : {Unk_EntrySize}");
            Console.WriteLine($"{nameof(Unk_BlockSize)} : {Unk_BlockSize}");
            Console.WriteLine($"{nameof(Unk39)} : {Unk39}");
        }
    }
}

/*
 example env records

ENV_48_Data_Record_1_StdEnvData
ENV_56_StdEnv_OccurenceFlag
ENV_64_StdEnv_OriginalOdometerValue
ENV_80_StdEnv_MostRecentOdometerValue
ENV_96_StdEnv_FrequencyCounter
ENV_104_StdEnv_OperationCycleCounter
ENV_112_Data_Record_2_CommonEnvData
ENV_120_CommonEnv_StorageSequence
ENV_128_Data_Record_3_First_Occurrence
ENV_136_First_SIGNALS_xSet1
ENV_136_First_SIGNALS_xSet2
*/
