using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Caesar
{
    public class DiagPreparation
    {
        // switch below values to properties to get them to render in a datagridview
        public string Qualifier { get; set; }
        public int Name_CTF;

        // uncomment this, and comment the block below to have these values show in the datagridview
        /*
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int SizeForBitTypes { get; set; }
        public int IITOffset { get; set; }
        public int PrepPresPoolIndex { get; set; }
        public int PresPoolIndex { get; set; }
        public int Field1E { get; set; }
        public int SystemParam { get; set; }
        public int DumpMode { get; set; }
        */

        public int Unk1;
        public int Unk2;
        public int SizeForBitTypes;
        public int IITOffset;
        public int PrepPresPoolIndex;
        public int PresPoolIndex;
        public int Field1E;
        public int SystemParam;
        public int DumpMode;

        private int DumpSize;
        public byte[] Dump;

        public BitArray Content = null;

        // datagridview preview 
        // public string DumpText { get { return BitUtility.BytesToHex(Dump, true); } }

        // these are indirect fields, assigned by a diagservice parent
        public int BitPosition { get; set; }
        public ushort ModeConfig;

        public int SizeInBits { get; set; } = 0;
        public InferredDataType FieldType { get; set; }


        long BaseAddress;

        [System.Text.Json.Serialization.JsonIgnore]
        public static readonly byte[] IntegerSizeMapping = new byte[] { 0x00, 0x01, 0x04, 0x08, 0x10, 0x20, 0x40 };

        [System.Text.Json.Serialization.JsonIgnore]
        public ECU ParentECU;

        [System.Text.Json.Serialization.JsonIgnore]
        private DiagService ParentDiagService;

        [System.Text.Json.Serialization.JsonIgnore]
        private CTFLanguage Language;


        public enum InferredDataType 
        {
            UnassignedType,
            IntegerType,
            PrepPresentationType,
            NativePresentationType,
            UnhandledITType,
            UnhandledSP17Type,
            UnhandledType,
            BitDumpType,
            ExtendedBitDumpType,
        }

        public DiagPreparation DeepCopy()
        {
            DiagPreparation result = new DiagPreparation { };
            result.Qualifier = $"{Qualifier}";
            result.Name_CTF = Name_CTF;
            result.Unk1 = Unk1;
            result.Unk2 = Unk2;
            result.SizeForBitTypes = SizeForBitTypes;
            result.IITOffset = IITOffset;
            result.PrepPresPoolIndex = PrepPresPoolIndex;
            result.PresPoolIndex = PresPoolIndex;
            result.Field1E = Field1E;
            result.SystemParam = SystemParam;
            result.DumpMode = DumpMode;
            result.Dump = Dump.ToArray();

            result.BitPosition = BitPosition;
            result.ModeConfig = ModeConfig;
            result.SizeInBits = SizeInBits;
            result.FieldType = FieldType;

            // indirect
            result.ParentECU = ParentECU;
            result.ParentDiagService = ParentDiagService;
            result.Content = new BitArray(Content);
            return result;
        }

        public void Restore(CTFLanguage language, ECU parentEcu, DiagService parentDiagService) 
        {
            Language = language;
            ParentECU = parentEcu;
            ParentDiagService = parentDiagService;
        }

        public DiagPreparation() { }

        // void __cdecl DiagServiceReadPresentation(int *inBase, DECODED_PRESENTATION *outPresentation)
        // Looks like its actually a presentation
        // See DIDiagservice* functions
        public DiagPreparation(BinaryReader reader, CTFLanguage language, long baseAddress, int bitPosition, ushort modeConfig, ECU parentEcu, DiagService parentDiagService)
        {
            BitPosition = bitPosition;
            ModeConfig = modeConfig;
            Language = language;
            BaseAddress = baseAddress;
            ParentECU = parentEcu;
            ParentDiagService = parentDiagService;

            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            ulong bitflags = reader.ReadUInt32();


            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            Name_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Unk1 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            Unk2 = CaesarReader.ReadBitflagUInt8(ref bitflags, reader);
            SizeForBitTypes = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            IITOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PrepPresPoolIndex = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PresPoolIndex = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Field1E = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            SystemParam = CaesarReader.ReadBitflagInt16(ref bitflags, reader, -1);
            DumpMode = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            DumpSize = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            if (DumpMode == 5) 
            {
                // dump is actually a string, use
                // CaesarReader.ReadBitflagDumpWithReaderAsString
            }

            /*
            // debug: find other dump types
            if ((DumpMode != 2) && (DumpSize != 0))
            {
                // ic204 "DL_Menu_Dimming_Characteristic_Set" Dimmwert_Step_1 dumpmode: 3
                // ic204 "RT_Check_Routine_Start_CRC_Check" No_Of_CRC_Bytes dumpmode: 4
                Console.WriteLine($"{Qualifier} dumpmode is not 2, val: {DumpMode}");
            }
            */
            Dump = CaesarReader.ReadBitflagDumpWithReader(ref bitflags, reader, DumpSize, baseAddress);

            SizeInBits = GetSizeInBits(reader);
            Content = new BitArray(SizeInBits);

            FillContentBitarrayWithDumpData();

            // PrintDebug();
        }

        private void FillContentBitarrayWithDumpData() 
        {
            // mutable buffer to be used when executing a diagservice
            // is dumpmode some sort of type data that we might have already figured out somewhere else?

            if (DumpMode == 2) // 2: int-like
            {
                if (Dump.Length == 0) 
                {
                    // nothing to do here
                    return;
                }
                if (Dump.Length > 4) 
                {
                    throw new Exception($"DiagPreparation: int-like dump should not exceed 32bits");
                }

                // dump is in LE, but parameter is delivered as BE, conversion is required
                int dumpAsInt = BitArrayExtension.PromoteToInt32(new BitArray(Dump), false); // source is LE
                BitArray rawDump = BitArrayExtension.ToBitArray(dumpAsInt, true, SizeInBits); // dest is BE

                // copy as much bits as possible into our correctly-sized content buffer
                // since data is now in BE, left-side bytes should be discarded; copy from right to left
                int maxLength = Math.Min(rawDump.Length, Content.Length);
                for (int i = 0; i < maxLength; i++) 
                {
                    Content[Content.Length - i - 1] = rawDump[rawDump.Length - i - 1];
                }

                // if the dump has a sign bit somewhere, this will get hairy
                // haven't seen that yet though
            }
        }

        // look at.. DIInternalRetrieveConstParamPreparation
        private int GetSizeInBits(BinaryReader reader, bool verbose = true) 
        {
            // if (modeConfig & 0xF00) == 0x300, the value is a const param: DIIsConstParameter

            // VCFragment does the same thing.. with the same ITT exception
            // BitPosition /= 8

            // look for the string "nImplType <= 6"
            uint modeE = (uint)ModeConfig & 0xF000;
            uint modeH = (uint)ModeConfig & 0xFF0;
            uint modeL = (uint)ModeConfig & 0xF;
            int resultBitSize = 0;


            if ((ModeConfig & 0xF00) == 0x300) // this check is made in DIDiagServiceRetrievePreparation
            {
                if (modeL > 6)
                {
                    throw new Exception("nImplType <= 6; trying to map a data type that cannot exist");
                }

                // const params : 0x320, 0x330, 0x340
                if (modeH == 0x320)
                {
                    // this behavior is confirmed
                    resultBitSize = IntegerSizeMapping[modeL];
                    FieldType = InferredDataType.IntegerType;
                }
                else if (modeH == 0x330)
                {
                    // this behavior is also okay
                    resultBitSize = SizeForBitTypes; // inPres + 20
                    FieldType = InferredDataType.BitDumpType;
                }
                else if (modeH == 0x340)
                {
                    // from dasm, but unimplemented
                    // DIInternalRetrieveConstParamPreparation
                    FieldType = InferredDataType.UnhandledITType;
                    throw new NotImplementedException("WARNING: valid but unhandled data size (ITT not parsed)");
                    // resultBitSize = 0; // inPres + 20
                }
            }
            else 
            {
                // if systemparam is -1.. load a default system type
                if (SystemParam == -1)
                {
                    // apparently both 0x2000 and 0x8000 source from different pools, but use the same PRESENTATION structure
                    if (modeE == 0x8000)
                    {
                        FieldType = InferredDataType.PrepPresentationType;

                        DiagPresentation pres = ParentECU.GlobalPrepPresentations[PrepPresPoolIndex];
                        resultBitSize = pres.TypeLength_1A > 0 ? pres.TypeLength_1A : pres.TypeLengthBytesMaybe_21;

                        // if value was specified in bytes, convert to bits
                        if (pres.Type_1C == 0)
                        {
                            resultBitSize *= 8;
                        }

                    }
                    else if (modeE == 0x2000)
                    {
                        FieldType = InferredDataType.NativePresentationType;
                        byte[] presPool = ParentECU.ReadECUPresentationsPool(reader);

                        DiagPresentation pres = ParentECU.GlobalPresentations[PresPoolIndex];
                        resultBitSize = pres.TypeLength_1A > 0 ? pres.TypeLength_1A : pres.TypeLengthBytesMaybe_21;

                        // if value was specified in bytes, convert to bits
                        if (pres.Type_1C == 0)
                        {
                            resultBitSize *= 8;
                        }
                    }
                    else 
                    {
                        // should throw an exception?
                        //Console.WriteLine($"WARNING: Unknown or unhandled type for for {qualifier}");
                        throw new Exception($"Attempted to load an unknown system type for {Qualifier}");
                    }
                }
                else 
                {
                    // not a const param, not a native param, this is a special param, parsed at DIInternalRetrieveSpecialPreparation
                    // DIInternalRetrieveSpecialPreparation officially supports 0x410, 0x420 only
                    if (modeH == 0x410)
                    {
                        int reducedSysParam = SystemParam - 0x10;
                        if (reducedSysParam == 0)
                        {
                            // specifically requests for LOBYTE (& 0xFF)
                            int resultByteSize = (ParentDiagService.RequestBytes.Length & 0xFF) - (BitPosition / 8);
                            resultBitSize = resultByteSize * 8;
                            FieldType = InferredDataType.ExtendedBitDumpType;
                            // Console.WriteLine($"0x{modeH:X} debug for {qualifier} (L: {modeL}) (BitWidth: {AlternativeBitWidth} SP: {SystemParam}), sz: {resultBitSize} b ({resultBitSize/8} B)");
                        }
                        else if (reducedSysParam == 17)
                        {
                            // open a diagservice based on inputRef name
                            // this is experimental, haven't seen a cbf that uses this yet
                            Console.WriteLine($"Parsing experimental 0x410 prep with sysparam 17 at {Qualifier}");
                            DiagService referencedDs = ParentECU.GlobalDiagServices.Find(x => x.Qualifier == ParentDiagService.InputRefNameMaybe);
                            if (referencedDs != null)
                            {
                                bool referencedDsHasRequestData = referencedDs.RequestBytes.Length > 0; // supposed to check if requestMessage is valid too
                                int internalType = referencedDs.DataClass_ServiceTypeShifted;
                                if (((referencedDs.DataClass_ServiceTypeShifted & 0xC) > 0) && referencedDsHasRequestData)
                                {
                                    if ((referencedDs.DataClass_ServiceTypeShifted & 4) > 0)
                                    {
                                        internalType = 0x10000000;
                                    }
                                    else
                                    {
                                        internalType = 0x20000000;
                                    }
                                }
                                if ((internalType & 0x10000) != 0)
                                {
                                    // referenced type is a global variable
                                    resultBitSize = ParentDiagService.P_Count * 8;
                                    FieldType = InferredDataType.UnhandledSP17Type;
                                }
                                else
                                {
                                    // use pres dump length
                                    FieldType = InferredDataType.UnhandledSP17Type;
                                    resultBitSize = ParentDiagService.RequestBytes.Length * 8;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"0x410 : sys param: 17 for qualifier {Qualifier} could not find referenced DiagService with index {ParentDiagService.InputRefNameMaybe}");
                                // throw new NotImplementedException
                            }
                        }
                        else
                        {
                            throw new Exception($"Invalid system parameter for {Qualifier}");
                        }
                    }
                    else if (modeH == 0x420)
                    {
                        if (modeL > 6)
                        {
                            throw new Exception("nImplType <= 6; trying to map a data type that cannot exist");
                        }
                        FieldType = InferredDataType.IntegerType;
                        resultBitSize = IntegerSizeMapping[modeL];
                    }
                    else if (modeH == 0x430)
                    {
                        // mode 0x430 is nonstandard and doesn't seem to exist in the function that I was disassembling
                        /*
                            AlternativeBitWidth : 128
                            SystemParam : 37
                            
                            See 0x320 vs 0x330, seems to be similar
                        */

                        resultBitSize = SizeForBitTypes; // inPres + 20
                        FieldType = InferredDataType.BitDumpType;
                    }
                    else
                    {
                        FieldType = InferredDataType.UnhandledType;
                        Console.WriteLine($"Unhandled type: {modeH} for {Qualifier}");
                        PrintDebug();
                        throw new Exception($"Attempted to load an unknown special param type for {Qualifier}");
                        //Console.WriteLine($"{qualifier} ({poolThing}/{ParentECU.ecuInfoPool_tableEntryCount})\n{BitUtility.BytesToHex(presentationStruct)}\n\n");
                    }
                }
            }


            /*
            if (modeH == 0x430)
            {
                // guessed
                if (verbose)
                {
                    Console.WriteLine($"Unsupported 0x{modeH:X} behavior for {qualifier} (L: {modeL}) (BitWidth: {AlternativeBitWidth} ByteWidth: {SystemParam})");
                }
                //PrintDebug();
                resultBitSize = AlternativeBitWidth; // alternate bit width is 128 which should be a nice 16 bytes
            }
            else if (modeH > 0x430)
            {
                // guessed from varcoding behavior
                if ((PresPool == 0) && (AvailableBitWidth_PoolThing == 0))
                {
                    return 0;
                }
                else
                {
                    //Console.WriteLine($"No idea how to handle Pres 0x750 from {qualifier} : {PresPool}");
                }
                Console.WriteLine($"No idea how to handle 0x{modeH:X} from {qualifier} ({PresPool}, {AvailableBitWidth_PoolThing})");
            }
            */
            return resultBitSize;
        }

        public void PrintDebug()
        {
            Console.WriteLine($"{nameof(Qualifier)} : {Qualifier}");
            Console.WriteLine($"{nameof(BitPosition)} : {BitPosition}");
            Console.WriteLine($"{nameof(ModeConfig)} : 0x{ModeConfig:X}");
            Console.WriteLine($"Mode H : 0x{ModeConfig & 0xFF0:X}, L : 0x{ModeConfig & 0xF:X}");
            Console.WriteLine($"{nameof(SizeInBits)} : 0x{SizeInBits:X}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Language.GetString(Name_CTF)}");
            Console.WriteLine($"{nameof(Unk1)} : {Unk1}");
            Console.WriteLine($"{nameof(Unk2)} : {Unk2}");
            Console.WriteLine($"{nameof(SizeForBitTypes)} : {SizeForBitTypes}");
            Console.WriteLine($"{nameof(IITOffset)} : {IITOffset}");
            Console.WriteLine($"{nameof(PrepPresPoolIndex)} : {PrepPresPoolIndex}");
            Console.WriteLine($"{nameof(PresPoolIndex)} : {PresPoolIndex}");
            Console.WriteLine($"{nameof(Field1E)} : {Field1E}");
            Console.WriteLine($"{nameof(SystemParam)} : {SystemParam}");
            // Console.WriteLine($"{nameof(noIdea_T)} : {language.GetString(noIdea_T)}");
            Console.WriteLine($"{nameof(Dump)} : {BitUtility.BytesToHex(Dump)}");
            Console.WriteLine("---------------");
        }
    }
}
