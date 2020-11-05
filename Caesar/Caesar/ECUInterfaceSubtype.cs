using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Caesar
{
    public class ECUInterfaceSubtype
    {
        public enum ParamName 
        {
            CP_BAUDRATE,
            CP_GLOBAL_REQUEST_CANIDENTIFIER,
            CP_FUNCTIONAL_REQUEST_CANIDENTIFIER,
            CP_REQUEST_CANIDENTIFIER,
            CP_RESPONSE_CANIDENTIFIER,
            CP_PARTNUMBERID,
            CP_PARTBLOCK,
            CP_HWVERSIONID,
            CP_SWVERSIONID,
            CP_SWVERSIONBLOCK,
            CP_SUPPLIERID,
            CP_SWSUPPLIERBLOCK,
            CP_ADDRESSMODE,
            CP_ADDRESSEXTENSION,
            CP_ROE_RESPONSE_CANIDENTIFIER,
            CP_USE_TIMING_RECEIVED_FROM_ECU,
            CP_STMIN_SUG,
            CP_BLOCKSIZE_SUG,
            CP_P2_TIMEOUT,
            CP_S3_TP_PHYS_TIMER,
            CP_S3_TP_FUNC_TIMER,
            CP_BR_SUG,
            CP_CAN_TRANSMIT,
            CP_BS_MAX,
            CP_CS_MAX,
            CPI_ROUTINECOUNTER,
            CP_REQREPCOUNT,
            // looks like outliers?
            CP_P2_EXT_TIMEOUT_7F_78,
            CP_P2_EXT_TIMEOUT_7F_21,
        }

        public string ctName;
        public int InterfaceName_T;
        public int InterfaceLongName_T;

        public int ctUnk3;
        public int ctUnk4;

        public int ctUnk5;
        public int ctUnk6;
        public int ctUnk7;

        public int ctUnk8;
        public int ctUnk9;
        public int ctUnk10; // might be signed

        public long BaseAddress;
        public int Index;

        public List<ComParameter> CommunicationParameters = new List<ComParameter>();

        public ECUInterfaceSubtype(BinaryReader reader, long baseAddress, int index)
        {
            Index = index;
            BaseAddress = baseAddress;
            reader.BaseStream.Seek(baseAddress, SeekOrigin.Begin);
            // we can now properly operate on the interface block
            ulong ctBitflags = reader.ReadUInt32();

            ctName = CaesarReader.ReadBitflagStringWithReader(ref ctBitflags, reader, BaseAddress);
            InterfaceName_T = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);
            InterfaceLongName_T = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);

            ctUnk3 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);
            ctUnk4 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);

            ctUnk5 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            ctUnk6 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            ctUnk7 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);

            ctUnk8 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            ctUnk9 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            ctUnk10 = CaesarReader.ReadBitflagInt8(ref ctBitflags, reader); // might be signed

            // PrintDebug();
        }

        public ComParameter GetComParameterByName(string paramName) 
        {
            return CommunicationParameters.Find(x => x.ParamName == paramName);
        }
        public int GetComParameterValue(ParamName name)
        {
            return GetComParameterByName(name.ToString()).comValue;
        }
        public bool GetComParameterValue(ParamName name, out int result)
        {
            ComParameter param = GetComParameterByName(name.ToString());
            if (param is null)
            {
                result = 0;
                return false;
            }
            else 
            {
                result = param.comValue;
                return true;
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine($"iface subtype: @ 0x{BaseAddress:X}");
            Console.WriteLine($"{nameof(InterfaceName_T)} : {InterfaceName_T}");
            Console.WriteLine($"{nameof(InterfaceLongName_T)} : {InterfaceLongName_T}");
            Console.WriteLine($"{nameof(ctUnk3)} : {ctUnk3}");
            Console.WriteLine($"{nameof(ctUnk4)} : {ctUnk4}");
            Console.WriteLine($"{nameof(ctUnk5)} : {ctUnk5}");
            Console.WriteLine($"{nameof(ctUnk6)} : {ctUnk6}");
            Console.WriteLine($"{nameof(ctUnk7)} : {ctUnk7}");
            Console.WriteLine($"{nameof(ctUnk8)} : {ctUnk8}");
            Console.WriteLine($"{nameof(ctUnk9)} : {ctUnk9}");
            Console.WriteLine($"{nameof(ctUnk10)} : {ctUnk10}");
            Console.WriteLine($"CT: {ctName}");
        }
    }
}
