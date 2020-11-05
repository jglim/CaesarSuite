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
        public string ecuIdk7Str;
        public string ecuIdk8Str;

        public int ecuIgnitionRequired;
        public int ecuIdk_2;
        public int ecuIdk_3_qwer_count;
        public int ecuIdk_4_qwer_offset;
        public int ecuSgmlSource;
        public int ecuIdk_6_reloffset;

        public int ecuvariant_fileoffset_1; // 1
        public int ecuvariant_tableEntryCount;
        public int ecuvariant_tableEntrySize;
        public int ecuvariant_tableSize;

        public int ecu_diagjobs_fileoffset_2; // 2
        public int ecu_diagjobs_tableEntryCount;
        public int ecu_diagjobs_tableEntrySize;
        public int ecu_diagjobs_tableSize;

        // strings like "P262600", all short, looks like identifiers
        public int ecuIdk_15_fileoffset_3; // 3
        public int ecuIdk_16_tableEntryCount;
        public int ecuIdk_17_tableEntrySize;
        public int ecuIdk_18_tableSize;

        public int env_pres_fileoffset_4; // 4
        public int env_pres_tableEntryCount;
        public int env_pres_tableEntrySize;
        public int env_pres_tableSize;

        public int varcoding_fileoffset_5; // 5 , 0x15716
        public int varcoding_tableEntryCount; // [1], 43 0x2B
        public int varcoding_tableEntrySize; // [2], 12 0xC (multiply with [1] for size), 43*12=516 = 0x204
        public int varcoding_tableSize; // [3] unused
        // there is an invisible DWORD here that stores the cached read data's ptr
        
        public int presentations_fileoffset_6;
        public int presentations_tableEntryCount;
        public int presentations_tableEntrySize;
        public int presentations_tableSize;

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

        public List<DiagService> GlobalDiagServices = new List<DiagService>();

        public long BaseAddress;

        byte[] cachedVarcodingPool = new byte[] { };
        byte[] cachedVariantPool = new byte[] { };
        byte[] cachedDiagjobPool = new byte[] { };
        byte[] cachedEcuInfoPool = new byte[] { };

        public string ECUDescriptionTranslated = "";

        public byte[] ReadDiagjobPool(BinaryReader reader)
        {
            if (cachedDiagjobPool.Length == 0)
            {
                cachedDiagjobPool = ReadEcuPool(reader, ecu_diagjobs_fileoffset_2, ecu_diagjobs_tableEntryCount, ecu_diagjobs_tableEntrySize);
            }
            return cachedDiagjobPool;
        }

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
            ecuIdk7Str = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);
            ecuIdk8Str = CaesarReader.ReadBitflagStringWithReader(ref ecuBitFlags, reader, BaseAddress);

            int dataBufferOffsetRelativeToFile = header.sizeOfStringPool + StubHeader.StubHeaderSize + header.cffHeaderSize + 4;
            // Console.WriteLine($"{nameof(dataBufferOffsetRelativeToFile)} : 0x{dataBufferOffsetRelativeToFile:X}");

            ecuIgnitionRequired = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_2 = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_3_qwer_count = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_4_qwer_offset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuSgmlSource = CaesarReader.ReadBitflagInt16(ref ecuBitFlags, reader);
            ecuIdk_6_reloffset = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuvariant_fileoffset_1 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuvariant_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuvariant_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuvariant_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecu_diagjobs_fileoffset_2 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecu_diagjobs_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecu_diagjobs_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecu_diagjobs_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            ecuIdk_15_fileoffset_3 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            ecuIdk_16_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_17_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            ecuIdk_18_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            env_pres_fileoffset_4 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            env_pres_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            env_pres_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            // bitflags will be exhausted at this point, load the extended bitflags
            ecuBitFlags = ecuBitFlagsExtended;

            env_pres_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            varcoding_fileoffset_5 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            varcoding_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            varcoding_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            varcoding_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

            presentations_fileoffset_6 = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader) + dataBufferOffsetRelativeToFile;
            presentations_tableEntryCount = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            presentations_tableEntrySize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);
            presentations_tableSize = CaesarReader.ReadBitflagInt32(ref ecuBitFlags, reader);

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

                long ecuInterfaceBaseAddress = interfaceTableAddress + interfaceBlockOffset;

                ECUInterface ecuInterface = new ECUInterface(reader, ecuInterfaceBaseAddress);
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
            //PrintDebug();
            CreateDiagServices(reader, language);
            CreateVariants(reader, language);
            ECUDescriptionTranslated = language.GetString(ECUDescription_T);
        }

        public void CreateDiagServices(BinaryReader reader, CTFLanguage language) 
        {
            byte[] diagjobPool = ReadDiagjobPool(reader);

            GlobalDiagServices = new List<DiagService>();
            using (BinaryReader poolReader = new BinaryReader(new MemoryStream(diagjobPool)))
            {
                for (int diagjobIndex = 0; diagjobIndex < ecu_diagjobs_tableEntryCount; diagjobIndex++)
                {
                    int offset = poolReader.ReadInt32();
                    int size = poolReader.ReadInt32();
                    uint crc = poolReader.ReadUInt32();
                    uint config = poolReader.ReadUInt16();
                    long diagjobBaseAddress = offset + ecu_diagjobs_fileoffset_2;
                    // Console.WriteLine($"DJ @ {offset:X} with size {size:X}");

                    DiagService dj = new DiagService(reader, language, diagjobBaseAddress, diagjobIndex);
                    GlobalDiagServices.Add(dj);
                }
            }

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
            Console.WriteLine($"ECU ecuIdk7: {ecuIdk7Str}");
            Console.WriteLine($"ECU ecuIdk8: {ecuIdk8Str}");


            Console.WriteLine($"{nameof(ecuIgnitionRequired)} : {ecuIgnitionRequired}");
            
            Console.WriteLine($"{nameof(ecuIdk_2)} : {ecuIdk_2}"); 

            Console.WriteLine($"{nameof(ecuIdk_3_qwer_count)} : {ecuIdk_3_qwer_count}");
            Console.WriteLine($"{nameof(ecuIdk_4_qwer_offset)} : {ecuIdk_4_qwer_offset}");
            
            Console.WriteLine($"{nameof(ecuSgmlSource)} : {ecuSgmlSource}");
            Console.WriteLine($"{nameof(ecuIdk_6_reloffset)} : 0x{ecuIdk_6_reloffset:X}");
            
            Console.WriteLine($"{nameof(ecuvariant_fileoffset_1)} : 0x{ecuvariant_fileoffset_1:X}");
            Console.WriteLine($"{nameof(ecuvariant_tableEntryCount)} : {ecuvariant_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuvariant_tableEntrySize)} : {ecuvariant_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuvariant_tableSize)} : 0x{ecuvariant_tableSize:X}");
            Console.WriteLine($"{nameof(ecu_diagjobs_fileoffset_2)} : 0x{ecu_diagjobs_fileoffset_2:X}");
            Console.WriteLine($"{nameof(ecu_diagjobs_tableEntryCount)} : {ecu_diagjobs_tableEntryCount}");
            Console.WriteLine($"{nameof(ecu_diagjobs_tableEntrySize)} : {ecu_diagjobs_tableEntrySize}");
            Console.WriteLine($"{nameof(ecu_diagjobs_tableSize)} : 0x{ecu_diagjobs_tableSize:X}");
            Console.WriteLine($"{nameof(ecuIdk_15_fileoffset_3)} : 0x{ecuIdk_15_fileoffset_3:X}");
            Console.WriteLine($"{nameof(ecuIdk_16_tableEntryCount)} : {ecuIdk_16_tableEntryCount}");
            Console.WriteLine($"{nameof(ecuIdk_17_tableEntrySize)} : {ecuIdk_17_tableEntrySize}");
            Console.WriteLine($"{nameof(ecuIdk_18_tableSize)} : 0x{ecuIdk_18_tableSize:X}");
            Console.WriteLine($"{nameof(env_pres_fileoffset_4)} : 0x{env_pres_fileoffset_4:X}");
            Console.WriteLine($"{nameof(env_pres_tableEntryCount)} : {env_pres_tableEntryCount}");
            Console.WriteLine($"{nameof(env_pres_tableEntrySize)} : {env_pres_tableEntrySize}");

            // Console.WriteLine("--- bitflag load 2 ---");

            Console.WriteLine($"{nameof(env_pres_tableSize)} : 0x{env_pres_tableSize:X}");
            Console.WriteLine($"{nameof(varcoding_fileoffset_5)} : 0x{varcoding_fileoffset_5:X}");
            Console.WriteLine($"{nameof(varcoding_tableEntryCount)} : {varcoding_tableEntryCount}");
            Console.WriteLine($"{nameof(varcoding_tableEntrySize)} : {varcoding_tableEntrySize}");
            Console.WriteLine($"{nameof(varcoding_tableSize)} : 0x{varcoding_tableSize:X}");
            Console.WriteLine($"{nameof(presentations_fileoffset_6)} : 0x{presentations_fileoffset_6:X}");
            Console.WriteLine($"{nameof(presentations_tableEntryCount)} : {presentations_tableEntryCount}");
            Console.WriteLine($"{nameof(presentations_tableEntrySize)} : {presentations_tableEntrySize}");
            Console.WriteLine($"{nameof(presentations_tableSize)} : 0x{presentations_tableSize:X}");
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

/*
 example env/pres

ENV_48_Data_Record_1_StdEnvData
PRES_ExtendedDataRecordNumber
ENV_56_StdEnv_OccurenceFlag
PRES_StdEnv_OccurenceFlag
ENV_64_StdEnv_OriginalOdometerValue
PRES_StdEnv_OriginalOdometerValue
ENV_80_StdEnv_MostRecentOdometerValue
PRES_StdEnv_MostRecentOdometerValue
ENV_96_StdEnv_FrequencyCounter
PRES_StdEnv_FrequencyCounter
ENV_104_StdEnv_OperationCycleCounter
PRES_StdEnv_OperationCycleCounter
ENV_112_Data_Record_2_CommonEnvData
PRES_ExtendedDataRecordNumber
ENV_120_CommonEnv_StorageSequence
PRES_CommonEnv_StorageSequence
ENV_128_Data_Record_3_First_Occurrence
PRES_ExtendedDataRecordNumber
ENV_136_First_SIGNALS_xSet1
PRES_SIGNALS_xSet1
ENV_136_First_SIGNALS_xSet2
PRES_SIGNALS_xSet2
ENV_136_First_SIGNALS_xSet3
PRES_SIGNALS_xSet3
ENV_136_First_SIGNALS_xSet4
PRES_SIGNALS_xSet4
ENV_136_First_SIGNALS_xSet5
PRES_SIGNALS_xSet5
ENV_136_First_SIGNALS_xSet6
PRES_SIGNALS_xSet6
ENV_136_First_SIGNALS_xSet7
PRES_SIGNALS_xSet7
ENV_136_First_SIGNALS_xSet8
PRES_SIGNALS_xSet8
ENV_136_First_SIGNALS_xSet9
PRES_SIGNALS_xSet9
ENV_144_First_SIGNALS_enhdtcinfo
PRES_SIGNALS_enhdtcinfo
ENV_152_First_SIGNALS_kmodoenv_w
PRES_SIGNALS_kmodoenv_w
ENV_168_First_SIGNALS_nmot
PRES_SIGNALS_nmot
ENV_176_First_SIGNALS_tmot
PRES_SIGNALS_tmot
ENV_184_First_SIGNALS_tumg
PRES_SIGNALS_tumg
ENV_192_First_SIGNALS_PID0Dh
PRES_SIGNALS_PID0Dh
ENV_200_First_SIGNALS_PID2Fh
PRES_SIGNALS_PID2Fh
ENV_208_First_SIGNALS_PID1Fh_CoEng_tiNormalOBD
PRES_SIGNALS_PID1Fh_CoEng_tiNormalOBD
ENV_224_First_SIGNALS_kmmilon_w
PRES_SIGNALS_kmmilon_w
ENV_224_First_SIGNALS_kmmilon_w_1
PRES_SIGNALS_kmmilon_w_1
ENV_240_First_SIGNALS_ub
PRES_SIGNALS_ub
ENV_240_First_SIGNALS_wub
PRES_SIGNALS_wub
ENV_248_First_SIGNALS_DA_Sys_BMS_State
PRES_SIGNALS_DA_Sys_BMS_State
ENV_248_First_SIGNALS_fstt_w
PRES_SIGNALS_fstt_w
ENV_248_First_SIGNALS_high_byte_of_taagrv_w
PRES_SIGNALS_high_byte_of_taagrv_w
ENV_248_First_SIGNALS_rl
PRES_SIGNALS_rl
ENV_248_First_SIGNALS_tans
PRES_SIGNALS_tans
ENV_248_First_SIGNALS_ubattee_w
PRES_SIGNALS_ubattee_w
ENV_256_First_SIGNALS_DA_Ecu_BusOff_State
PRES_SIGNALS_DA_Ecu_BusOff_State
ENV_256_First_SIGNALS_bdemodi_um
PRES_SIGNALS_bdemodi_um
ENV_256_First_SIGNALS_high_byte_of_agrvp_w
PRES_SIGNALS_high_byte_of_agrvp_w
ENV_256_First_SIGNALS_high_byte_of_fzabgs_w
PRES_SIGNALS_high_byte_of_fzabgs_w
ENV_256_First_SIGNALS_ml
PRES_SIGNALS_ml
ENV_256_First_SIGNALS_ora
PRES_SIGNALS_ora
ENV_264_First_SIGNALS_DA_Sys_CGW_State
PRES_SIGNALS_DA_Sys_CGW_State
ENV_264_First_SIGNALS_ECTOut_Raw
PRES_SIGNALS_ECTOut_Raw
ENV_264_First_SIGNALS_bdemod
PRES_SIGNALS_bdemod
ENV_264_First_SIGNALS_high_byte_of_agrvps_w
PRES_SIGNALS_high_byte_of_agrvps_w
ENV_264_First_SIGNALS_high_byte_of_fra_w
PRES_SIGNALS_high_byte_of_fra_w
ENV_264_First_SIGNALS_pecubaro_w
PRES_SIGNALS_pecubaro_w
ENV_264_First_SIGNALS_rl
PRES_SIGNALS_rl
ENV_272_First_SIGNALS_high_byte_of_ofmsndk_w
PRES_SIGNALS_high_byte_of_ofmsndk_w
ENV_272_First_SIGNALS_ora
PRES_SIGNALS_ora
ENV_272_First_SIGNALS_oscdktf_u
PRES_SIGNALS_oscdktf_u
ENV_272_First_SIGNALS_tans
PRES_SIGNALS_tans
ENV_272_First_SIGNALS_wdkba
PRES_SIGNALS_wdkba
ENV_280_First_SIGNALS_PID11h
PRES_SIGNALS_PID11h
ENV_280_First_SIGNALS_gnagen1flt
PRES_SIGNALS_gnagen1flt
ENV_280_First_SIGNALS_high_byte_of_fra_w
PRES_SIGNALS_high_byte_of_fra_w
ENV_280_First_SIGNALS_high_byte_of_tasrg_w
PRES_SIGNALS_high_byte_of_tasrg_w
ENV_280_First_SIGNALS_lamsoni_u
PRES_SIGNALS_lamsoni_u
ENV_280_First_SIGNALS_toel
PRES_SIGNALS_toel
ENV_280_First_SIGNALS_wdkba
PRES_SIGNALS_wdkba
ENV_288_First_SIGNALS_Eng_Trq
PRES_SIGNALS_Eng_Trq
ENV_288_First_SIGNALS_bdemod
PRES_SIGNALS_bdemod
ENV_288_First_SIGNALS_gen1utz
PRES_SIGNALS_gen1utz
ENV_288_First_SIGNALS_ml
PRES_SIGNALS_ml
ENV_288_First_SIGNALS_pvd_w
PRES_SIGNALS_pvd_w
ENV_288_First_SIGNALS_rkmeeff_w
PRES_SIGNALS_rkmeeff_w
ENV_288_First_SIGNALS_udkp1_u
PRES_SIGNALS_udkp1_u
ENV_296_First_SIGNALS_DA_Sys_DCDC_State
PRES_SIGNALS_DA_Sys_DCDC_State
ENV_296_First_SIGNALS_combust1_u
PRES_SIGNALS_combust1_u
ENV_296_First_SIGNALS_gen1excu_w
PRES_SIGNALS_gen1excu_w
ENV_296_First_SIGNALS_udkp2_u
PRES_SIGNALS_udkp2_u
ENV_296_First_SIGNALS_wdkba
PRES_SIGNALS_wdkba
ENV_304_First_SIGNALS_DA_Sys_DSI_State
PRES_SIGNALS_DA_Sys_DSI_State
ENV_304_First_SIGNALS_anzesrk
PRES_SIGNALS_anzesrk
ENV_304_First_SIGNALS_dynlsu_u
PRES_SIGNALS_dynlsu_u
ENV_304_First_SIGNALS_pistnd_w
PRES_SIGNALS_pistnd_w
ENV_304_First_SIGNALS_thermenv_u
PRES_SIGNALS_thermenv_u
ENV_304_First_SIGNALS_wdkdlr_u
PRES_SIGNALS_wdkdlr_u
ENV_312_First_SIGNALS_DA_Sys_EAC_State
PRES_SIGNALS_DA_Sys_EAC_State
ENV_312_First_SIGNALS_dtrlfS2_w
PRES_SIGNALS_dtrlfS2_w
ENV_312_First_SIGNALS_evz_aus
PRES_SIGNALS_evz_aus
ENV_312_First_SIGNALS_gen1exvo_w
PRES_SIGNALS_gen1exvo_w
ENV_312_First_SIGNALS_high_byte_of_rlmds_w
PRES_SIGNALS_high_byte_of_rlmds_w
ENV_312_First_SIGNALS_taml
PRES_SIGNALS_taml
ENV_312_First_SIGNALS_wped
PRES_SIGNALS_wped
ENV_320_First_SIGNALS_DA_Ecu_SM_State
PRES_SIGNALS_DA_Ecu_SM_State
ENV_320_First_SIGNALS_high_byte_of_fahfmdss_w
PRES_SIGNALS_high_byte_of_fahfmdss_w
ENV_320_First_SIGNALS_rl
PRES_SIGNALS_rl
ENV_320_First_SIGNALS_tatherm
PRES_SIGNALS_tatherm
ENV_320_First_SIGNALS_tfuelfr
PRES_SIGNALS_tfuelfr
ENV_320_First_SIGNALS_upwg1_u
PRES_SIGNALS_upwg1_u
ENV_328_First_SIGNALS_DA_Sys_EIS_State
PRES_SIGNALS_DA_Sys_EIS_State
ENV_328_First_SIGNALS_gmerrenvb1
PRES_SIGNALS_gmerrenvb1
ENV_328_First_SIGNALS_high_byte_of_fkmsdk_w
PRES_SIGNALS_high_byte_of_fkmsdk_w
ENV_328_First_SIGNALS_ora
PRES_SIGNALS_ora
ENV_328_First_SIGNALS_thdev
PRES_SIGNALS_thdev
ENV_328_First_SIGNALS_trrlS2_w
PRES_SIGNALS_trrlS2_w
ENV_328_First_SIGNALS_upwg2_u
PRES_SIGNALS_upwg2_u
ENV_336_First_SIGNALS_DA_Sys_EM1_State
PRES_SIGNALS_DA_Sys_EM1_State
ENV_336_First_SIGNALS_gmerrenvb2
PRES_SIGNALS_gmerrenvb2
ENV_336_First_SIGNALS_high_byte_of_fra_w
PRES_SIGNALS_high_byte_of_fra_w
ENV_336_First_SIGNALS_rl
PRES_SIGNALS_rl
ENV_336_First_SIGNALS_rlsol
PRES_SIGNALS_rlsol
ENV_336_First_SIGNALS_tajal
PRES_SIGNALS_tajal
ENV_336_First_SIGNALS_tfuelm
PRES_SIGNALS_tfuelm
ENV_344_First_SIGNALS_gangi
PRES_SIGNALS_gangi
ENV_344_First_SIGNALS_gmerrenvb3
PRES_SIGNALS_gmerrenvb3
ENV_344_First_SIGNALS_ora
PRES_SIGNALS_ora
ENV_344_First_SIGNALS_rinlsu_u
PRES_SIGNALS_rinlsu_u
ENV_344_First_SIGNALS_rl
PRES_SIGNALS_rl
ENV_344_First_SIGNALS_tfuelhpm
PRES_SIGNALS_tfuelhpm
ENV_352_First_SIGNALS_DA_Sys_EM2_State
PRES_SIGNALS_DA_Sys_EM2_State
ENV_352_First_SIGNALS_gmerrenvb4
PRES_SIGNALS_gmerrenvb4
ENV_352_First_SIGNALS_high_byte_of_fra_w
PRES_SIGNALS_high_byte_of_fra_w
ENV_352_First_SIGNALS_milsol_w
PRES_SIGNALS_milsol_w
ENV_352_First_SIGNALS_phlsnf
PRES_SIGNALS_phlsnf
ENV_352_First_SIGNALS_rk_w
PRES_SIGNALS_rk_w
ENV_352_First_SIGNALS_rlr_w
PRES_SIGNALS_rlr_w
ENV_352_First_SIGNALS_tmklivo
PRES_SIGNALS_tmklivo
ENV_360_First_SIGNALS_gangi
PRES_SIGNALS_gangi
ENV_360_First_SIGNALS_gen1fltf
PRES_SIGNALS_gen1fltf
ENV_360_First_SIGNALS_lftklianf
PRES_SIGNALS_lftklianf
ENV_360_First_SIGNALS_prhrlsu_u
PRES_SIGNALS_prhrlsu_u
ENV_368_First_SIGNALS_DA_Sys_EPKB_State
PRES_SIGNALS_DA_Sys_EPKB_State
ENV_368_First_SIGNALS_dlatrmo_u
PRES_SIGNALS_dlatrmo_u
ENV_368_First_SIGNALS_gen1gkf1
PRES_SIGNALS_gen1gkf1
ENV_368_First_SIGNALS_mfpsdia_efMsv_u8
PRES_SIGNALS_mfpsdia_efMsv_u8
ENV_368_First_SIGNALS_pistnd_w
PRES_SIGNALS_pistnd_w
ENV_368_First_SIGNALS_rk_w
PRES_SIGNALS_rk_w
ENV_368_First_SIGNALS_tmstemp_w
PRES_SIGNALS_tmstemp_w
ENV_376_First_SIGNALS_DA_Sys_FSCM_State
PRES_SIGNALS_DA_Sys_FSCM_State
ENV_376_First_SIGNALS_Mfvd_redcuradp
PRES_SIGNALS_Mfvd_redcuradp
ENV_376_First_SIGNALS_exhauenv_u
PRES_SIGNALS_exhauenv_u
ENV_376_First_SIGNALS_gen1gkf2_w
PRES_SIGNALS_gen1gkf2_w
ENV_384_First_SIGNALS_DA_Sys_HVAC_State
PRES_SIGNALS_DA_Sys_HVAC_State
ENV_384_First_SIGNALS_ctum
PRES_SIGNALS_ctum
ENV_384_First_SIGNALS_high_byte_of_prist_w
PRES_SIGNALS_high_byte_of_prist_w
ENV_384_First_SIGNALS_lamsoni_u
PRES_SIGNALS_lamsoni_u
ENV_384_First_SIGNALS_prroh_w
PRES_SIGNALS_prroh_w
ENV_384_First_SIGNALS_prsoll_w
PRES_SIGNALS_prsoll_w
ENV_384_First_SIGNALS_tabst_w
PRES_SIGNALS_tabst_w
ENV_392_First_SIGNALS_DA_Sys_IC_State
PRES_SIGNALS_DA_Sys_IC_State
ENV_392_First_SIGNALS_ctumextmod
PRES_SIGNALS_ctumextmod
ENV_392_First_SIGNALS_gen1gff1
PRES_SIGNALS_gen1gff1
ENV_392_First_SIGNALS_miszul_w
PRES_SIGNALS_miszul_w
ENV_392_First_SIGNALS_thdev
PRES_SIGNALS_thdev
ENV_400_First_SIGNALS_PID15h_0
PRES_SIGNALS_PID15h_0
ENV_400_First_SIGNALS_gen1gff2_w
PRES_SIGNALS_gen1gff2_w
ENV_400_First_SIGNALS_lamsoni_u
PRES_SIGNALS_lamsoni_u
ENV_400_First_SIGNALS_prroh_w
PRES_SIGNALS_prroh_w
ENV_400_First_SIGNALS_tabst_w
PRES_SIGNALS_tabst_w
ENV_400_First_SIGNALS_tfuelfr
PRES_SIGNALS_tfuelfr
ENV_408_First_SIGNALS_DA_Ecu_IgnSw_State
PRES_SIGNALS_DA_Ecu_IgnSw_State
ENV_408_First_SIGNALS_high_byte_of_dfravui_w
PRES_SIGNALS_high_byte_of_dfravui_w
ENV_408_First_SIGNALS_lamsons_u
PRES_SIGNALS_lamsons_u
ENV_408_First_SIGNALS_miksolv_w
PRES_SIGNALS_miksolv_w
ENV_408_First_SIGNALS_thdev
PRES_SIGNALS_thdev
ENV_416_First_SIGNALS_DA_Ecu_NM_State
PRES_SIGNALS_DA_Ecu_NM_State
ENV_416_First_SIGNALS_frm_u
PRES_SIGNALS_frm_u
ENV_416_First_SIGNALS_gentrq_w
PRES_SIGNALS_gentrq_w
ENV_416_First_SIGNALS_high_byte_of_engethfctr
PRES_SIGNALS_high_byte_of_engethfctr
ENV_416_First_SIGNALS_high_byte_of_fratlp_w
PRES_SIGNALS_high_byte_of_fratlp_w
ENV_416_First_SIGNALS_high_byte_of_tvldste_w
PRES_SIGNALS_high_byte_of_tvldste_w
ENV_416_First_SIGNALS_prdr_w
PRES_SIGNALS_prdr_w
ENV_424_First_SIGNALS_DA_Sys_NOX_State
PRES_SIGNALS_DA_Sys_NOX_State
ENV_424_First_SIGNALS_PID10h
PRES_SIGNALS_PID10h
ENV_424_First_SIGNALS_high_byte_of_facethdiag
PRES_SIGNALS_high_byte_of_facethdiag
ENV_424_First_SIGNALS_lamsoni_u
PRES_SIGNALS_lamsoni_u
ENV_424_First_SIGNALS_mss_info
PRES_SIGNALS_mss_info
ENV_424_First_SIGNALS_rinf_w
PRES_SIGNALS_rinf_w
ENV_432_First_SIGNALS_DA_Sys_ORC_State
PRES_SIGNALS_DA_Sys_ORC_State
ENV_432_First_SIGNALS_atphys_w
PRES_SIGNALS_atphys_w
ENV_432_First_SIGNALS_frm_u
PRES_SIGNALS_frm_u
ENV_432_First_SIGNALS_gen1linec
PRES_SIGNALS_gen1linec
ENV_432_First_SIGNALS_nsol
PRES_SIGNALS_nsol
ENV_432_First_SIGNALS_tankenv1_u
PRES_SIGNALS_tankenv1_u
ENV_440_First_SIGNALS_DA_Sys_PNC_State
PRES_SIGNALS_DA_Sys_PNC_State
ENV_440_First_SIGNALS_fteadf
PRES_SIGNALS_fteadf
ENV_440_First_SIGNALS_gen1rt_w
PRES_SIGNALS_gen1rt_w
ENV_440_First_SIGNALS_miagk_w
PRES_SIGNALS_miagk_w
ENV_440_First_SIGNALS_miist_w
PRES_SIGNALS_miist_w
ENV_440_First_SIGNALS_monitenv_u
PRES_SIGNALS_monitenv_u
ENV_440_First_SIGNALS_ushk
PRES_SIGNALS_ushk
ENV_448_First_SIGNALS_aadps_vg
PRES_SIGNALS_aadps_vg
ENV_448_First_SIGNALS_imatphys_w
PRES_SIGNALS_imatphys_w
ENV_448_First_SIGNALS_trlrS2_w
PRES_SIGNALS_trlrS2_w
ENV_448_First_SIGNALS_ubsq_w
PRES_SIGNALS_ubsq_w
ENV_456_First_SIGNALS_DA_Sys_PSM_State
PRES_SIGNALS_DA_Sys_PSM_State
ENV_456_First_SIGNALS_ECTOut
PRES_SIGNALS_ECTOut
ENV_456_First_SIGNALS_miist_w
PRES_SIGNALS_miist_w
ENV_456_First_SIGNALS_mss_info
PRES_SIGNALS_mss_info
ENV_456_First_SIGNALS_upvg_u
PRES_SIGNALS_upvg_u
ENV_464_First_SIGNALS_DA_Sys_SBC_State
PRES_SIGNALS_DA_Sys_SBC_State
ENV_464_First_SIGNALS_dtlrfS2_w
PRES_SIGNALS_dtlrfS2_w
ENV_464_First_SIGNALS_frtphys_w
PRES_SIGNALS_frtphys_w
ENV_464_First_SIGNALS_pu_w
PRES_SIGNALS_pu_w
ENV_464_First_SIGNALS_tmst
PRES_SIGNALS_tmst
ENV_464_First_SIGNALS_zwoutakt
PRES_SIGNALS_zwoutakt
ENV_472_First_SIGNALS_psr_w
PRES_SIGNALS_psr_w
ENV_472_First_SIGNALS_tecueng_w
PRES_SIGNALS_tecueng_w
ENV_472_First_SIGNALS_tmst
PRES_SIGNALS_tmst
ENV_472_First_SIGNALS_wnwa_u
PRES_SIGNALS_wnwa_u
ENV_480_First_SIGNALS_dwmsvvst_w
PRES_SIGNALS_dwmsvvst_w
ENV_480_First_SIGNALS_octphys_w
PRES_SIGNALS_octphys_w
ENV_480_First_SIGNALS_wnwa_u
PRES_SIGNALS_wnwa_u
ENV_480_First_SIGNALS_wnwsa_u
PRES_SIGNALS_wnwsa_u
ENV_488_First_SIGNALS_mizsolv_w
PRES_SIGNALS_mizsolv_w
ENV_488_First_SIGNALS_toilsen_w
PRES_SIGNALS_toilsen_w
ENV_488_First_SIGNALS_wnwe_u
PRES_SIGNALS_wnwe_u
ENV_488_First_SIGNALS_wnwsa_u
PRES_SIGNALS_wnwsa_u
ENV_496_First_SIGNALS_DA_Sys_SCCM_State
PRES_SIGNALS_DA_Sys_SCCM_State
ENV_496_First_SIGNALS_SWSADA_SSM_StEngHoodEnvtlDa
PRES_SIGNALS_SWSADA_SSM_StEngHoodEnvtlDa
ENV_496_First_SIGNALS_dwmsvd_w
PRES_SIGNALS_dwmsvd_w
ENV_496_First_SIGNALS_wnwe_u
PRES_SIGNALS_wnwe_u
ENV_496_First_SIGNALS_wnwse_u
PRES_SIGNALS_wnwse_u
ENV_504_First_SIGNALS_DA_Sys_SCRCM_State
PRES_SIGNALS_DA_Sys_SCRCM_State
ENV_504_First_SIGNALS_SWSADA_SSM_StGenEnvtl1Da
PRES_SIGNALS_SWSADA_SSM_StGenEnvtl1Da
ENV_504_First_SIGNALS_gnbinfo1_w
PRES_SIGNALS_gnbinfo1_w
ENV_504_First_SIGNALS_miagk_w
PRES_SIGNALS_miagk_w
ENV_504_First_SIGNALS_psr_w
PRES_SIGNALS_psr_w
ENV_504_First_SIGNALS_wnwse_u
PRES_SIGNALS_wnwse_u
ENV_512_First_SIGNALS_DA_Sys_SPC_State
PRES_SIGNALS_DA_Sys_SPC_State
ENV_512_First_SIGNALS_SWSADA_SSM_StGenEnvtl2Da
PRES_SIGNALS_SWSADA_SSM_StGenEnvtl2Da
ENV_512_First_SIGNALS_prist_um
PRES_SIGNALS_prist_um
ENV_512_First_SIGNALS_toel
 
 */
