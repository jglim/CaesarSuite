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

        public string Qualifier;
        public int Name_CTF;
        public int Description_CTF;

        public int Unk3;
        public int Unk4;

        public int Unk5;
        public int Unk6;
        public int Unk7;

        public int Unk8;
        public int Unk9;
        public int Unk10; // might be signed

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

            Qualifier = CaesarReader.ReadBitflagStringWithReader(ref ctBitflags, reader, BaseAddress);
            Name_CTF = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);
            Description_CTF = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader, -1);

            Unk3 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);
            Unk4 = CaesarReader.ReadBitflagInt16(ref ctBitflags, reader);

            Unk5 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            Unk6 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);
            Unk7 = CaesarReader.ReadBitflagInt32(ref ctBitflags, reader);

            Unk8 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            Unk9 = CaesarReader.ReadBitflagUInt8(ref ctBitflags, reader);
            Unk10 = CaesarReader.ReadBitflagInt8(ref ctBitflags, reader); // might be signed
        }

        public ComParameter GetComParameterByName(string paramName) 
        {
            return CommunicationParameters.Find(x => x.ParamName == paramName);
        }
        public int GetComParameterValue(ParamName name)
        {
            return GetComParameterByName(name.ToString()).ComParamValue;
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
                result = param.ComParamValue;
                return true;
            }
        }

        public void PrintDebug()
        {
            Console.WriteLine($"iface subtype: @ 0x{BaseAddress:X}");
            Console.WriteLine($"{nameof(Name_CTF)} : {Name_CTF}");
            Console.WriteLine($"{nameof(Description_CTF)} : {Description_CTF}");
            Console.WriteLine($"{nameof(Unk3)} : {Unk3}");
            Console.WriteLine($"{nameof(Unk4)} : {Unk4}");
            Console.WriteLine($"{nameof(Unk5)} : {Unk5}");
            Console.WriteLine($"{nameof(Unk6)} : {Unk6}");
            Console.WriteLine($"{nameof(Unk7)} : {Unk7}");
            Console.WriteLine($"{nameof(Unk8)} : {Unk8}");
            Console.WriteLine($"{nameof(Unk9)} : {Unk9}");
            Console.WriteLine($"{nameof(Unk10)} : {Unk10}");
            Console.WriteLine($"CT: {Qualifier}");
        }
    }
}
