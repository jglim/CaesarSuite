using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Caesar;

namespace Diogenes.Simulation
{
    public class Simulated_CRD3 : SimulatedDevice
    {
        // quacks like a CRD3 : 2131 from a DTC perspective
        public enum UDS : byte
        {
            UDS_AccessTimingParameters = 0x83,
            UDS_Authentication = 0x29,
            UDS_ClearDiagnosticInformation = 0x14,
            UDS_CommunicationControl = 0x28,
            UDS_ControlDTCSettings = 0x85,
            UDS_DiagnosticSessionControl = 0x10,
            UDS_DynamicallyDefineDataIdentifier = 0x2C,
            UDS_ECUReset = 0x11,
            UDS_InputOutputControlByIdentifier = 0x2F,
            UDS_LinkControl = 0x87,
            UDS_NegativeResponse = 0x7F,
            UDS_ReadDataByIdentifier = 0x22,
            UDS_ReadDataByIdentifierPeriodic = 0x2A,
            UDS_ReadDTCInformation = 0x19,
            UDS_ReadMemoryByAddress = 0x23,
            UDS_ReadScalingDataByIdentifier = 0x24,
            UDS_RequestDownload = 0x34,
            UDS_RequestFileTransfer = 0x38,
            UDS_RequestTransferExit = 0x37,
            UDS_RequestUpload = 0x35,
            UDS_ResponseOnEvent = 0x86,
            UDS_RoutineControl = 0x31,
            UDS_SecuredDataTransmission = 0x84,
            UDS_SecurityAccess = 0x27,
            UDS_TesterPresent = 0x3E,
            UDS_TransferData = 0x36,
            UDS_WriteDataByIdentifier = 0x2E,
            UDS_WriteMemoryByAddress = 0x3D,
        }

        public enum NR : byte 
        {
            NR_PositiveResponse = 0x00,
            NR_GeneralReject = 0x10,
            NR_ServiceNotSupported = 0x11,
            NR_SubfunctionNotSupported = 0x12,
            NR_IncorrectMessageLengthOrInvalidFormat = 0x13,
            NR_ResponseTooLong = 0x14,
            NR_BusyRepeatRequest = 0x21,
            NR_ConditionsNotCorrect = 0x22,
            NR_RequestSequenceError = 0x24,
            NR_NoResponseFromSubnetComponent = 0x25,
            NR_FailurePreventsExecutionOfRequestedAction = 0x26,
            NR_RequestOutOfRange = 0x31,
            NR_SecurityAccessDenied = 0x33,
            NR_InvalidKey = 0x35,
            NR_ExceedNumberOfAttempts = 0x36,
            NR_RequiredTimeDelayNotExpired = 0x37,
            NR_UploadDownloadNotAccepted = 0x70,
            NR_TransferDataSuspended = 0x71,
            NR_GeneralProgrammingFailure = 0x72,
            NR_WrongBlockSequenceCounter = 0x73,
            NR_RequestCorrectlyReceivedResponsePending = 0x78,
            NR_SubfunctionNotSupportedInActiveSession = 0x7E,
            NR_ServiceNotSupportedInActiveSession = 0x7F,
            NR_RpmTooHigh = 0x81,
            NR_RpmTooLow = 0x82,
            NR_EngineRunning = 0x83,
            NR_EngineNotRunning = 0x84,
            NR_EngineRunTimeTooLow = 0x85,
            NR_TemperatureTooHigh = 0x86,
            NR_TemperatureTooLow = 0x87,
            NR_VehicleSpeedTooHigh = 0x88,
            NR_VehicleSpeedTooLow = 0x89,
            NR_ThrottlePedalTooHigh = 0x8A,
            NR_ThrottlePedalTooLow = 0X8B,
            NR_TransmissionRangeNotInNeutral = 0X8C,
            NR_TransmissionRangeNotInGear = 0x8D,
            NR_BrakeSwitchesNotClosed = 0x8F,
            NR_ShifterLeverNotInPark = 0x90,
            NR_TorqueConverterClutchLocked = 0x91,
            NR_VoltageTooHigh = 0x92,
            NR_VoltageTooLow = 0x93,
        }

        public enum Identifier : ushort 
        {
            ID_SessionVariant = 0xF100,
            ID_SupplierIdentifier = 0xF154,
            ID_EROTAN = 0xF196,
            ID_VCFull = 0x1001,
            ID_VCPartial = 0x1002,
        }

        public enum Vendor : byte 
        {
            VENDOR_Unspecified = 0,
            VENDOR_Becker = 1,
            VENDOR_Blaupunkt = 2,
            VENDOR_Bosch = 3,
            VENDOR_MB = 4,
            VENDOR_HuF = 5,
            VENDOR_Kammerer = 6,
            VENDOR_Kostal = 7,
            VENDOR_Siemens = 8,
            VENDOR_Stribel = 9,
            VENDOR_MicroHeat = 10,
            VENDOR_JATCO = 11,
            VENDOR_SWF = 16,
            VENDOR_VDO = 17,
            VENDOR_Webasto = 18,
            VENDOR_Dornier = 19,
            VENDOR_TEG = 20,
            VENDOR_Hella = 21,
            VENDOR_Lucas = 22,
            VENDOR_GKR = 23,
            VENDOR_MBB = 24,
            VENDOR_Motometer = 25,
            VENDOR_Borg = 32,
            VENDOR_Temic = 33,
            VENDOR_Teves = 34,
            VENDOR_Borg_Warner = 35,
            VENDOR_MED_SPA = 36,
            VENDOR_DENSO = 37,
            VENDOR_ZF = 38,
            VENDOR_TRW = 39,
            VENDOR_Dunlop = 40,
            VENDOR_LUK = 41,
            VENDOR_Magneti_Marelli = 48,
            VENDOR_DODUCO = 49,
            VENDOR_Alpine = 50,
            VENDOR_AMC_AEG_Mobile = 51,
            VENDOR_Bose = 52,
            VENDOR_Dasa = 53,
            VENDOR_Motorola = 54,
            VENDOR_Nokia = 55,
            VENDOR_Panasonic = 56,
            VENDOR_APAG = 57,
            VENDOR_Rialtosoft = 58,
            VENDOR_Applicom = 59,
            VENDOR_Conti_Temic = 60,
            VENDOR_Cherry = 61,
            VENDOR_TI_Automotive = 62,
            VENDOR_Kongsberg_Company = 63,
            VENDOR_Delphi = 64,
            VENDOR_Alfmeier = 65,
            VENDOR_Sidler = 66,
            VENDOR_Marquardt = 67,
            VENDOR_Wehrle = 68,
            VENDOR_megamos = 69,
            VENDOR_ADC = 70,
            VENDOR_BERU = 71,
            VENDOR_Valeo = 72,
            VENDOR_Magna = 73,
            VENDOR_Allison = 74,
            VENDOR_Isringhausen = 75,
            VENDOR_Grammer = 76,
            VENDOR_Funkwerk_Dabendorf = 77,
            VENDOR_Hella_Behr = 78,
            VENDOR_Pollak = 79,
            VENDOR_AKG = 80,
            VENDOR_Automotive_Lighting = 81,
            VENDOR_TAG = 82,
            VENDOR_UNITED_PARTS = 83,
            VENDOR_catem = 84,
            VENDOR_Alge = 85,
            VENDOR_Pierburg = 86,
            VENDOR_Brusa = 87,
            VENDOR_Ecostar = 88,
            VENDOR_Xcellsis = 89,
            VENDOR_Wabco_Automotive = 90,
            VENDOR_Voith = 91,
            VENDOR_Knorr = 92,
            VENDOR_TVI = 93,
            VENDOR_Stoneridge = 94,
            VENDOR_Telma = 95,
            VENDOR_STW = 96,
            VENDOR_Koyo = 97,
            VENDOR_Eberspacher = 98,
            VENDOR_ADVICS = 99,
            VENDOR_OMRON = 100,
            VENDOR_Mitsubishi_Heavy_Industry = 101,
            VENDOR_Methode = 102,
            VENDOR_UNISIAJECS = 103,
            VENDOR_UNISIA_JKC_Steering_Systems = 104,
            VENDOR_AISIN = 105,
            VENDOR_Zexel_Valeo = 106,
            VENDOR_Schrader = 107,
            VENDOR_Ballard = 108,
            VENDOR_SFT = 112,
            VENDOR_Kieckert_AG = 113,
            VENDOR_Behr = 114,
            VENDOR_MB_Lenkungen = 115,
            VENDOR_Sachs_Automotive = 116,
            VENDOR_Peiker = 117,
            VENDOR_Petri = 118,
            VENDOR_Autoliv = 119,
            VENDOR_Thien_Electronic = 120,
            VENDOR_Siemens_VDO = 121,
            VENDOR_Dornier_Consulting_GmbH = 122,
            VENDOR_Alps = 123,
            VENDOR_PREH = 124,
            VENDOR_Hitachi_Unisia = 125,
            VENDOR_Hitachi = 126,
            VENDOR_Reserved_127 = 127,
            VENDOR_Huntsville = 128,
            VENDOR_Yazaki = 129,
            VENDOR_Lear = 130,
            VENDOR_Johnson_Controls = 131,
            VENDOR_HarmanBecker = 132,
            VENDOR_Mitsubishi_Electric = 133,
            VENDOR_Tokico_USA_Inc = 134,
            VENDOR_Nippon_Seiki_International = 135,
            VENDOR_Inalfa = 136,
            VENDOR_Nippon_Seiki_UK = 137,
            VENDOR_GHSP = 138,
            VENDOR_Vector = 139,
            VENDOR_Gentex = 140,
            VENDOR_Visteon = 141,
            VENDOR_Tochigi_Fuji = 142,
            VENDOR_DCA = 143,
            VENDOR_May_and_Scofield = 144,
            VENDOR_DaimlerChrysler_Hamburg_Plant = 145,
            VENDOR_AISIN_AW = 146,
            VENDOR_TOYODA_MACHINE_WORKS = 147,
            VENDOR_Solectron_Invotronics = 148,
            VENDOR_Kicker = 149,
            VENDOR_American_Axle_Company = 150,
            VENDOR_GETRAG = 151,
            VENDOR_Promate = 152,
            VENDOR_ArvinMeritor = 153,
            VENDOR_Reserved_MMC = 160,
            VENDOR_Reserved_MMC_SMART = 161,
            VENDOR_Reserved_162 = 162,
            VENDOR_After_Market_Supplier = 254,
            VENDOR_Unidentified = 255,
        }

        public Simulated_CRD3() 
        {
        
        }


        //ushort ECU_VARIANT = 0x2131;
        ushort ECU_VARIANT = 0x1000; // pretend to be eis166 for a while
        byte ECU_SUPPLIER_IDENTIFIER = (byte)Vendor.VENDOR_Delphi;

        byte ECU_SESSION = 1; // default
        byte ECU_GATEWAY_MODE = 2;
        ushort  ECU_TIMING_RESOLUTION_1MS = 20;
        ushort ECU_TIMING_RESOLUTION_10MS = 200;

        byte[] ECU_VariantCoding = { 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x10, 0x21, 0x40, 0xAF, 0x8C, 0xE0, 0x24, 0x58, 0x91, 0x59, 0x98, 0x56, 0x20, 0x49, 0x47, 0x7D, 0x00, 0xE5, 0x00, 0x36, 0x36 };
        byte[] ECU_Fingerprint = { 0x00, 0x40, 0x33, 0x10 };
        byte[] ECU_SoftwareCalibrationNumber = { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38 };
        byte[] ECU_EROTAN = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

        byte[] EnvTest = {0x00, 0x90, 0x00, 0x2F, 0x01, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x28, 0x02, 0x00, 0x24, 0x00, 0x93, 0x00, 0x38, 0x02, 0x0F, 0x01, 0x04, 0x7F, 0x7C, 0x00, 0x88, 0x8E, 0xA5, 0x3C, 0x0A, 0x00, 0x00, 0x00, 0x5F, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x53, 0x00, 0x03, 0x00, 0x24, 0x00, 0x93, 0x00, 0x38, 0x02, 0x0F, 0x01, 0x04, 0x7F, 0x7C, 0x00, 0x88, 0x8E, 0xA5, 0x3C, 0x0A, 0x00, 0x00, 0x00, 0x5F, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x53, 0x00 };

        // this behaves a bit more like my uC implementation
        // performance is generally poorer, though the IO is fairly minimal
        // maybe wrap it in a class with a proper binarywriter
        List<byte> SharedResponse = new List<byte>();

        public void WriteByte(byte inByte)
        {
            SharedResponse.Add(inByte);
        }
        public void WriteUint16(ushort inValue)
        {
            SharedResponse.Add((byte)((inValue >> 8) & 0xFF));
            SharedResponse.Add((byte)(inValue & 0xFF));
        }
        public void WriteInt16(short inValue)
        {
            SharedResponse.Add((byte)((inValue >> 8) & 0xFF));
            SharedResponse.Add((byte)(inValue & 0xFF));
        }
        public void WriteUint32(ushort inValue)
        {
            SharedResponse.Add((byte)((inValue >> 24) & 0xFF));
            SharedResponse.Add((byte)((inValue >> 16) & 0xFF));
            SharedResponse.Add((byte)((inValue >> 8) & 0xFF));
            SharedResponse.Add((byte)(inValue & 0xFF));
        }
        public void WriteInt32(int inValue)
        {
            SharedResponse.Add((byte)((inValue >> 24) & 0xFF));
            SharedResponse.Add((byte)((inValue >> 16) & 0xFF));
            SharedResponse.Add((byte)((inValue >> 8) & 0xFF));
            SharedResponse.Add((byte)(inValue & 0xFF));
        }

        public void WriteByteArray(byte[] inValue) 
        {
            SharedResponse.AddRange(inValue);
        }


        public byte[] CreateNegativeResponse(UDS callingFunction, NR reason) 
        {
            // throw new Exception("debugger please");
            return new byte[] {0x7F, (byte)callingFunction, (byte)reason };
        }

        public byte[] MemoryStreamWriterToArray(BinaryWriter writer) 
        {
            return ((MemoryStream)writer.BaseStream).ToArray();
        }
        
        public override byte[] ReceiveRequest(IEnumerable<byte> request)
        {
            SharedResponse = new List<byte>();

            byte[] requestBytes = request.ToArray();
            if (requestBytes.Length == 0)
            {
                // this shouldn't happen
                return new byte[] { };
            }
            
            if (!Enum.IsDefined(typeof(UDS), requestBytes[0]))
            {
                return CreateNegativeResponse((UDS)requestBytes[0], NR.NR_ServiceNotSupported);
            }
            UDS requestCommand = (UDS)requestBytes[0];
            WriteByte((byte)(requestBytes[0] + 0x40));

            ushort identifier = 0;
            if (requestBytes.Length > 2) 
            {
                identifier = (ushort)(requestBytes[1] << 8 | requestBytes[2]);
            }

            if ((requestCommand == UDS.UDS_TesterPresent) && (requestBytes.Length >= 2))
            {
                WriteByte(requestBytes[1]);
            }
            else if ((requestCommand == UDS.UDS_DiagnosticSessionControl) && (requestBytes.Length >= 2))
            {
                ECU_SESSION = requestBytes[1];
                WriteByte(ECU_SESSION);
                WriteUint16(ECU_TIMING_RESOLUTION_1MS);
                WriteUint16(ECU_TIMING_RESOLUTION_10MS);
            }
            else if ((requestCommand == UDS.UDS_ReadDataByIdentifier) && (requestBytes.Length >= 3))
            {
                WriteUint16(identifier);
                if (identifier == (ushort)Identifier.ID_SupplierIdentifier)
                {
                    WriteByte(ECU_SUPPLIER_IDENTIFIER);
                }
                else if (identifier == (ushort)Identifier.ID_SessionVariant)
                {
                    WriteByte(ECU_GATEWAY_MODE);
                    WriteUint16(ECU_VARIANT);
                    WriteByte(ECU_SESSION);
                }
            }
            else if ((requestCommand == UDS.UDS_WriteDataByIdentifier) && (requestBytes.Length >= 3))
            {

            }
            else if ((requestCommand == UDS.UDS_ReadDTCInformation) && (requestBytes.Length >= 2))
            {
                byte infoType = requestBytes[1];
                WriteByte(infoType);

                byte mask = requestBytes[2];

                if (infoType == 0x02)
                {
                    /*
                    // mask should go here?
                    WriteByte(0xFF); // mask
                    WriteByte(0x00);
                    WriteByte(0x90);
                    WriteByte(0x00);
                    WriteByte(0x2F);
                    */
                    WriteByteArray(BitUtility.BytesFromHex("5B D0 3D 2A 0B D0 32 2A 0B C0 19 2A 0B D0 38 2A 08 C1 22 87 0B D1 82 00 0B D1 80 00 0B 06 10 00 0B D1 81 00 03 D1 83 00 03 D1 98 00 41")); // eis166
                }
                else if (infoType == 0x06)
                {
                    /*
                    WriteByte(0xFF); // mask
                    WriteByteArray(EnvTest);
                    */
                    WriteByteArray(BitUtility.BytesFromHex("D0 3D 2A 0B 01 00 2A C0 2A C0 09 00 ")); // eis166
                }
            }
            else
            {
                // unrecognized
                return CreateNegativeResponse((UDS)requestBytes[0], NR.NR_ServiceNotSupported);
            }

            return SharedResponse.ToArray();
        }

        public static ushort CRC16ARC(byte[] inData) 
        {
            // tests:
            /*
            Console.WriteLine($"{Simulation.Simulated_CRD3.CRC16ARC(new byte[] { 0x78, 0x65, 0x36, 0x62, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x0B, 0x0B, 0x00, 0x01, 0x00, 0x00, 0x03, 0x00, 0x03, 0x02, 0x00, 0x00, 0x00, 0x01, 0x81, 0x03, 0xFF, 0xC1, 0x74, 0x0E, 0xE8, 0x03, 0x72, 0x0B, 0xC8, 0x08, 0xC2, 0x01 }):X4}"); // expects E6 2B (0x2BE6)
            Console.WriteLine($"{Simulation.Simulated_CRD3.CRC16ARC(new byte[] { 0x78, 0x64, 0x70, 0x39, 0x0C, 0x00, 0x00, 0x04, 0x00, 0x03, 0x02, 0x02, 0x02, 0x02, 0x00, 0x02, 0x01, 0x00, 0x01, 0x00, 0x04, 0x02, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x1A, 0x00, 0xA6, 0x09, 0xE8, 0x03, 0xE8, 0x03, 0xC6, 0x07, 0x81, 0x01 }):X4}"); // expects C1 12 (0x12C1)
             */
            uint[] crcTable = {
                0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
                0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
                0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
                0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
                0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
                0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
                0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
                0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
                0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
                0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
                0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
                0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
                0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
                0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
                0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
                0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
                0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
                0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
                0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
                0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
                0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
                0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
                0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
                0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
                0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
                0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
                0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
                0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
                0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
                0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
                0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
                0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040,
            };

            uint crc = 0;
            foreach (byte b in inData)
            {
                crc = (crc >> 8) ^ crcTable[(crc ^ b) & 0xFF];
            }
            return (ushort)(crc & 0xFFFF);
        }
    }
}
