using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Caesar
{

    // DIAGJOB *__cdecl DIOpenDiagService(DI_ECUINFO *ecuHandle, char *serviceName, int ecuErrors)
    public class DiagService
    {
        /*
    5	DT	DATA
    7	DL	DOWNLOAD
    10	FN|DNU	DIAGNOSTIC_U, FN
    19	DJ	DIAGNOSTIC_JOB
    21	SES	SESSION
    22	DT_STO	STORED DATA
    23	RT	ROUTINE
    24	IOC	IO CONTROL
    26	WVC	VARIANTCODING WRITE
    27	WVC	VARIANTCODING READ

         */
        public enum ServiceType
        {
            Data = 5,
            Download = 7,
            DiagnosticFunction = 10,
            DiagnosticJob = 19,
            Session = 21,
            StoredData = 22,
            Routine = 23,
            IoControl = 24,
            VariantCodingWrite = 26,
            VariantCodingRead = 27,
        }

        public string Qualifier;

        public int Name_CTF;
        public int Description_CTF;

        public ushort DataClass_ServiceType;
        public int DataClass_ServiceTypeShifted;

        public ushort IsExecutable;
        public ushort ClientAccessLevel;
        public ushort SecurityAccessLevel;

        public int T_ComParam_Count;
        public int T_ComParam_Offset;

        public int Q_Count;
        public int Q_Offset;

        public int R_Count;
        public int R_Offset;

        public string InputRefNameMaybe;

        public int U_prep_Count;
        public int U_prep_Offset;

        public int V_Count;
        public int V_Offset;

        public int RequestBytes_Count;
        public int RequestBytes_Offset;

        public int W_OutPres_Count;
        public int W_OutPres_Offset;

        public ushort field50;

        public string NegativeResponseName;
        public string UnkStr3;
        public string UnkStr4;

        public int P_Count;
        public int P_Offset;

        public int DiagServiceCodeCount;
        public int DiagServiceCodeOffset;

        public int S_Count;
        public int S_Offset;

        public int X_Count;
        public int X_Offset;

        public int Y_Count;
        public int Y_Offset;

        public int Z_Count;
        public int Z_Offset;

        public byte[] RequestBytes;

        public long BaseAddress;
        public int PoolIndex;

        public List<DiagPreparation> InputPreparations = new List<DiagPreparation>();
        public List<List<DiagPreparation>> OutputPreparations = new List<List<DiagPreparation>>();
        public List<ComParameter> DiagComParameters = new List<ComParameter>();

        public ECU ParentECU;
        public CTFLanguage Language;

        public DiagService(BinaryReader reader, CTFLanguage language, long baseAddress, int poolIndex, ECU parentEcu) 
        {
            ParentECU = parentEcu;
            Language = language;
            PoolIndex = poolIndex;
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt32();
            ulong bitflagExtended = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            Name_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            DataClass_ServiceType = CaesarReader.ReadBitflagUInt16(ref bitflags, reader);
            DataClass_ServiceTypeShifted = 1 << (DataClass_ServiceType - 1);

            IsExecutable = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;
            ClientAccessLevel = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;
            SecurityAccessLevel = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;

            T_ComParam_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            T_ComParam_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Q_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Q_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            R_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            R_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            InputRefNameMaybe = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            U_prep_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            U_prep_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            // array of DWORDs, probably reference to elsewhere
            V_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            V_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            RequestBytes_Count = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            RequestBytes_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            W_OutPres_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            W_OutPres_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            field50 = CaesarReader.ReadBitflagUInt16(ref bitflags, reader);

            NegativeResponseName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress); // negative response name
            UnkStr3 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            UnkStr4 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            P_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            P_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            DiagServiceCodeCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            DiagServiceCodeOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            S_Count = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            S_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            bitflags = bitflagExtended;
            
            X_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            X_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Y_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Y_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Z_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Z_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            if (RequestBytes_Count > 0)
            {
                reader.BaseStream.Seek(baseAddress + RequestBytes_Offset, SeekOrigin.Begin);
                RequestBytes = reader.ReadBytes(RequestBytes_Count);
            }
            else 
            {
                RequestBytes = new byte[] { };
            }

            // u_table to u_entries
            InputPreparations = new List<DiagPreparation>();
            for (int prepIndex = 0; prepIndex < U_prep_Count; prepIndex++)
            {
                long presentationTableOffset = baseAddress + U_prep_Offset;
                reader.BaseStream.Seek(presentationTableOffset + (prepIndex * 10), SeekOrigin.Begin);

                // DIOpenDiagService (reads 4, 4, 2 then calls DiagServiceReadPresentation) to build a presentation
                int prepEntryOffset = reader.ReadInt32(); // file: 0 (DW)
                int prepEntryBitPos = reader.ReadInt32(); // file: 4 (DW)
                ushort prepEntryMode = reader.ReadUInt16(); // file: 8 (W)

                DiagPreparation preparation = new DiagPreparation(reader, language, presentationTableOffset + prepEntryOffset, prepEntryBitPos, prepEntryMode, parentEcu, this);
                //preparation.PrintDebug();
                InputPreparations.Add(preparation);
            }


            OutputPreparations = new List<List<DiagPreparation>>();
            long outPresBaseAddress = BaseAddress + W_OutPres_Offset;

            // FIXME: run it through the entire dbr cbf directory, check if any file actually has more than 1 item in ResultPresentationSet
            for (int presIndex = 0; presIndex < W_OutPres_Count; presIndex++)
            {
                reader.BaseStream.Seek(outPresBaseAddress + (presIndex * 8), SeekOrigin.Begin);
                // FIXME
                int resultPresentationCount = reader.ReadInt32(); // index? if true, will fix the "wtf" list<list<diagprep>>
                int resultPresentationOffset = reader.ReadInt32();

                List<DiagPreparation> ResultPresentationSet = new List<DiagPreparation>();
                for (int presInnerIndex = 0; presInnerIndex < resultPresentationCount; presInnerIndex++)
                {
                    long presentationTableOffset = outPresBaseAddress + resultPresentationOffset;

                    reader.BaseStream.Seek(presentationTableOffset + (presIndex * 10), SeekOrigin.Begin);

                    int prepEntryOffset = reader.ReadInt32(); // file: 0 (DW)
                    int prepEntryBitPos = reader.ReadInt32(); // file: 4 (DW)
                    ushort prepEntryMode = reader.ReadUInt16(); // file: 8 (W)

                    DiagPreparation preparation = new DiagPreparation(reader, language, presentationTableOffset + prepEntryOffset, prepEntryBitPos, prepEntryMode, parentEcu, this);
                    ResultPresentationSet.Add(preparation);
                }
                OutputPreparations.Add(ResultPresentationSet);
            }

            DiagComParameters = new List<ComParameter>();
            long comParamTableBaseAddress = BaseAddress + T_ComParam_Offset;
            for (int cpIndex = 0; cpIndex < T_ComParam_Count; cpIndex++)
            {
                reader.BaseStream.Seek(comParamTableBaseAddress + (cpIndex * 4), SeekOrigin.Begin);
                int resultCpOffset = reader.ReadInt32();
                long cpEntryBaseAddress = comParamTableBaseAddress + resultCpOffset;
                ComParameter cp = new ComParameter(reader, cpEntryBaseAddress, parentEcu.ECUInterfaces);
                DiagComParameters.Add(cp);
            }

            // DJ_Zugriffsberechtigung_Abgleich
            // DJ_Zugriffsberechtigung
            // DT_Abgasklappe_kontinuierlich
            // FN_HardReset
            // WVC_Implizite_Variantenkodierung_Write

            // NR_Disable_Resp_required noexec
            // DT_Laufzeiten_Resetzaehler_nicht_implementiert exec
            /*
            if (false && qualifierName.Contains("RVC_SCN_Variantencodierung_VGS_73_Lesen"))
            {

                Console.WriteLine($"{nameof(field50)} : {field50}");
                Console.WriteLine($"{nameof(IsExecutable)} : {IsExecutable} {IsExecutable != 0}");
                Console.WriteLine($"{nameof(AccessLevel)} : {AccessLevel}");
                Console.WriteLine($"{nameof(SecurityAccessLevel)} : {SecurityAccessLevel}");
                Console.WriteLine($"{nameof(DataClass)} : {DataClass}");



                Console.WriteLine($"{qualifierName} - ReqBytes: {RequestBytes_Count}, P: {P_Count}, Q: {Q_Count}, R: {R_Count}, S: {S_Count}, T: {T_Count}, Preparation: {U_prep_Count}, V: {V_Count}, W: {W_Count}, X: {X_Count}, Y: {Y_Count}, Z: {Z_Count}, DSC {DiagServiceCodeCount}");
                Console.WriteLine($"at 0x{baseAddress:X}, W @ 0x{W_Offset:X}, DSC @ 0x{DiagServiceCodeOffset:X}");
                Console.WriteLine($"ReqBytes: {BitUtility.BytesToHex(RequestBytes)}");
            }
            */
            //Console.WriteLine($"{qualifierName} - O: {RequestBytes_Count}, P: {P_Count}, Q: {Q_Count}, R: {R_Count}, S: {S_Count}, T: {T_Count}, U: {U_Count}, V: {V_Count}, W: {W_Count}, X: {X_Count}, Y: {Y_Count}, Z: {Z_Count}, DSC {DiagServiceCodeCount}");

            
            byte[] dscPool = parentEcu.ParentContainer.CaesarCFFHeader.DSCPool;
            long dscTableBaseAddress = BaseAddress + DiagServiceCodeOffset;

            using (BinaryReader dscPoolReader = new BinaryReader(new MemoryStream(dscPool)))
            {
                for (int dscIndex = 0; dscIndex < DiagServiceCodeCount; dscIndex++) 
                {
                    reader.BaseStream.Seek(dscTableBaseAddress + (4 * dscIndex), SeekOrigin.Begin);
                    long dscEntryBaseAddress = reader.ReadInt32() + dscTableBaseAddress;
                    reader.BaseStream.Seek(dscEntryBaseAddress, SeekOrigin.Begin);

                    ulong dscEntryBitflags = reader.ReadUInt16();
                    uint idk1 = CaesarReader.ReadBitflagUInt8(ref dscEntryBitflags, reader);
                    uint idk2 = CaesarReader.ReadBitflagUInt8(ref dscEntryBitflags, reader);
                    int dscPoolOffset = CaesarReader.ReadBitflagInt32(ref dscEntryBitflags, reader);
                    string dscQualifier = CaesarReader.ReadBitflagStringWithReader(ref dscEntryBitflags, reader, dscEntryBaseAddress);

                    dscPoolReader.BaseStream.Seek(dscPoolOffset * 8, SeekOrigin.Begin);
                    long dscRecordOffset = dscPoolReader.ReadInt32() + parentEcu.ParentContainer.CaesarCFFHeader.DscBlockOffset;
                    int dscRecordSize = dscPoolReader.ReadInt32();

                    reader.BaseStream.Seek(dscRecordOffset, SeekOrigin.Begin);

                    // Console.WriteLine($"DSC {qualifierName} @ 0x{dscTableBaseAddress:X8} {idk1}/{idk2} pool @ 0x{dscPoolOffset:X}, name: {dscQualifier}");
                    byte[] dscBytes = reader.ReadBytes(dscRecordSize);
#if DEBUG
                    //string dscName = $"{parentEcu.Qualifier}_{Qualifier}_{dscIndex}.pal";
                    //Console.WriteLine($"Exporting DSC: {dscName}");
                    //File.WriteAllBytes(dscName, dscBytes);
#endif
                    // at this point, the DSC binary is available in dscBytes, intended for use in DSCContext (but is currently unimplemented)
                    // Console.WriteLine($"DSC actual at 0x{dscRecordOffset:X}, size=0x{dscRecordSize:X}\n");
                }

            }

        }

        public string GetDescription()
        {
            return Language.GetString(Description_CTF);
        }
        public string GetName()
        {
            return Language.GetString(Name_CTF);
        }

        public long GetCALInt16Offset(BinaryReader reader) 
        {
            reader.BaseStream.Seek(BaseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt32();
            ulong bitflagExtended = reader.ReadUInt32();

            CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, BaseAddress); // Qualifier
            CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1); // Name
            CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1); // Description
            CaesarReader.ReadBitflagUInt16(ref bitflags, reader); // Type
            CaesarReader.ReadBitflagUInt16(ref bitflags, reader); // IsExecutable 
            if (CaesarReader.CheckAndAdvanceBitflag(ref bitflags))
            {
                return reader.BaseStream.Position;
            }
            else 
            {
                return -1;
            }
        }


        public void PrintDebug() 
        {
            Console.WriteLine($"{Qualifier} - ReqBytes: {RequestBytes_Count}, P: {P_Count}, Q: {Q_Count}, R: {R_Count}, S: {S_Count}, ComParams: {T_ComParam_Count}, Preparation: {U_prep_Count}, V: {V_Count}, OutPres: {W_OutPres_Count}, X: {X_Count}, Y: {Y_Count}, Z: {Z_Count}, DSC {DiagServiceCodeCount}, field50: {field50}");
            Console.WriteLine($"BaseAddress @ 0x{BaseAddress:X}, NR: {NegativeResponseName}");
            Console.WriteLine($"V @ 0x{BaseAddress + V_Offset:X}, count: {V_Count}");
        }
    }
}




/*
// originally EnvironmentContext, removed because of overlap
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class EnvironmentContext
    {
        // see : const char *__cdecl DIGetComfortErrorCode(DI_ECUINFO *ecuh, unsigned int dtcIndex)
        public string Qualifier;

        public long BaseAddress;
        public int PoolIndex;
        public int Name_CTF;
        public int Description_CTF;
        public int ServiceTypeMaybe;
        public int AccessLevelType_Maybe;
        public int AccessLevelType_Maybe2;
        public int PresentationTableCount;
        public int PresentationTableOffset;
        public int PresentationTableRowSize_Maybe; // see diagservice for similar layout, seems unused (uint16)

        public ECU ParentECU;
        CTFLanguage Language;

        public EnvironmentContext(BinaryReader reader, CTFLanguage language, long baseAddress, int poolIndex, ECU parentEcu)
        {
            ParentECU = parentEcu;
            PoolIndex = poolIndex;
            BaseAddress = baseAddress;
            Language = language;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            // layout seems very similar to DiagService
            ulong bitflags = reader.ReadUInt32();
            ulong bitflagsExtended = reader.ReadUInt32();

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            Name_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            ServiceTypeMaybe = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            AccessLevelType_Maybe = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            AccessLevelType_Maybe2 = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            // doesn't seem to be set for any files in my library
            for (int i = 0; i < 14; i++) 
            {
                if (CaesarReader.CheckAndAdvanceBitflag(ref bitflags))
                {
                    throw new Exception("Sorry, The parser for EnvironmentContext has encountered an unknown bitflag; please open an issue and indicate your CBF file name.");
                }
            }

            // these describe the table to the presentation
            PresentationTableCount = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PresentationTableOffset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            PresentationTableRowSize_Maybe = CaesarReader.ReadBitflagInt16(ref bitflags, reader);

            // ... looks like DiagService?!
        }

        public void PrintDebug()
        {
            Console.WriteLine(Qualifier);
        }
    }
}
*/