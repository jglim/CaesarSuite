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

        public string qualifierName;

        public int elementName_T;
        public int elementDescription_T;

        public ushort DataClass_ServiceType;
        public int fieldC;

        public ushort IsExecutable;
        public ushort AccessLevel;
        public ushort SecurityAccessLevel;

        public int T_Count;
        public int T_Offset;

        public int Q_Count;
        public int Q_Offset;

        public int R_Count;
        public int R_Offset;

        public string strIdk1;

        public int U_prep_Count;
        public int U_prep_Offset;

        public int V_Count;
        public int V_Offset;

        public int RequestBytes_Count;
        public int RequestBytes_Offset;

        public int W_Count;
        public int W_Offset;

        public ushort field50;

        public string strIdk2;
        public string strIdk3;
        public string strIdk4;

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
        public List<DiagPreparation> Preparations = new List<DiagPreparation>();
        public DiagService(BinaryReader reader, CTFLanguage language, long baseAddress, int poolIndex) 
        {
            PoolIndex = poolIndex;
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);

            ulong bitflags = reader.ReadUInt32();
            ulong bitflagExtended = reader.ReadUInt32();

            qualifierName = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            elementName_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);
            elementDescription_T = CaesarReader.ReadBitflagInt32(ref bitflags, reader, -1);

            DataClass_ServiceType = CaesarReader.ReadBitflagUInt16(ref bitflags, reader);
            fieldC = 1 << (DataClass_ServiceType - 1);

            IsExecutable = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;
            AccessLevel = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;
            SecurityAccessLevel = CaesarReader.ReadBitflagUInt16(ref bitflags, reader); ;

            T_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            T_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            Q_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            Q_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            R_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            R_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            strIdk1 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

            U_prep_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            U_prep_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            V_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            V_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            RequestBytes_Count = CaesarReader.ReadBitflagInt16(ref bitflags, reader);
            RequestBytes_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            W_Count = CaesarReader.ReadBitflagInt32(ref bitflags, reader);
            W_Offset = CaesarReader.ReadBitflagInt32(ref bitflags, reader);

            field50 = CaesarReader.ReadBitflagUInt16(ref bitflags, reader);

            strIdk2 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            strIdk3 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);
            strIdk4 = CaesarReader.ReadBitflagStringWithReader(ref bitflags, reader, baseAddress);

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
            Preparations = new List<DiagPreparation>();
            for (int prepIndex = 0; prepIndex < U_prep_Count; prepIndex++)
            {
                long presentationTableOffset = baseAddress + U_prep_Offset;
                reader.BaseStream.Seek(presentationTableOffset + (prepIndex * 10), SeekOrigin.Begin);

                // DIOpenDiagService (reads 4, 4, 2 then calls DiagServiceReadPresentation) to build a presentation
                int prepEntryOffset = reader.ReadInt32(); // file: 0 (DW)
                int prepEntryBitPos = reader.ReadInt32(); // file: 4 (DW)
                ushort prepEntryMode = reader.ReadUInt16(); // file: 8 (W)
                // Console.WriteLine($"uEntry: 0x{uEntryOffset} , 0x{uEntryNoIdea} , 0x{uEntryMode}");
                // void __cdecl DiagServiceReadPresentation(int *inBase, DECODED_PRESENTATION *outPresentation)

                DiagPreparation preparation = new DiagPreparation(reader, language, presentationTableOffset + prepEntryOffset, prepEntryBitPos, prepEntryMode);
                //preparation.PrintDebug();
                Preparations.Add(preparation);
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



        }

        public void PrintDebug() 
        {
            Console.WriteLine($"{qualifierName} - ReqBytes: {RequestBytes_Count}, P: {P_Count}, Q: {Q_Count}, R: {R_Count}, S: {S_Count}, T: {T_Count}, Preparation: {U_prep_Count}, V: {V_Count}, W: {W_Count}, X: {X_Count}, Y: {Y_Count}, Z: {Z_Count}, DSC {DiagServiceCodeCount}, field50: {field50}");
        }
    }
}
