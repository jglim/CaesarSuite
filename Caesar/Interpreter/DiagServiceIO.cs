using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class DiagServiceIO : TrackedObject
    {
        public string Name;
        public ushort Unknown1;
        public Host.IDiagServiceRunner DiagServiceRunner;

        public DiagServiceIO(Interpreter ih, string name, ushort unk1) : base(ih) 
        {
            Name = name;
            Unknown1 = unk1;
            // create a runner in the host
            DiagServiceRunner = ih.HostDiagService.CreateFromName(name);
        }

        public override string ToString()
        {
            return $"T#{ObjectIndex}: DSIO {Name}, Unk1: {Unknown1:X4}";
        }

        public bool IsNegativeResponse() 
        {
            return DiagServiceRunner.IsNegativeResponse();
        }

        public void DoDiagService() 
        {
            DiagServiceRunner.DoDiagService();
        }

        public int GetPrepPresType(int index) 
        {
            return DiagServiceRunner.GetPresParamType(index);
            /*
            if (Name == "DNU_LOGIN_BC")
            {
                return 17; // "expect 17 got XX" 
            }
            throw new Exception($"GetPrepPresType unimplemented for {Name}");
            */
        }

        public int GetPrepParamDumpLength(int index) 
        {
            return DiagServiceRunner.GetPrepParamDumpLength(index);
            /*
            if (Name == "DNU_LOGIN_BC")
            {
                return 4;
            }
            */
            /*
Name	Byte Position	Data Size	Inferred Type	Direction	Raw Data
SID	0	1	IntegerType	Input	27
sendKey	1	1	IntegerType	Input	0C
key	2	4	NativeInfoPoolType	Input	
PRES_Login_BC	0	2	NativePresentationType	Output (0)	
             */
            //throw new Exception($"GetPrepParamDumpLength unimplemented for {Name}");
        }

        public int GetPresParamDumpLength(int index) 
        {
            return DiagServiceRunner.GetPresParamDumpLength(index);
            /*
            // hack : ki211 debug requires this
            if (Name == "DNU_SEED_BC")
            {
                
                //Name	Byte Position	Data Size	Inferred Type	Direction	Raw Data
                //SID	0	1	IntegerType	Input	27
                //request_Seed_Stufe_11	1	1	IntegerType	Input	0B
                //PRES_4ByteDump	2	4	NativePresentationType	Output (0)	<----------
                return 4;
            }
            throw new Exception($"GetPresParamDumpLength unimplemented for {Name}");
            */
        }

        public byte[] GetDumpPresParam(int index) 
        {
            return DiagServiceRunner.GetDumpPresParam(index);
            /*
            if (Name == "DNU_SEED_BC")
            {
                return new byte[] { 0x11, 0x22, 0x33, 0x44, };
            }
            throw new Exception($"GetDumpPresParam unimplemented for {Name}");
            */
        }

        public byte[] GetPresentationDumpValue(int index) 
        {
            return DiagServiceRunner.GetPresentationDumpValue(index);
        }

        public string GetStringPresentationParam(int index) 
        {
            return DiagServiceRunner.GetStringPresentationParam(index);
        }
        public void SetDumpPreparationParam(int index, byte[] value) 
        {
            DiagServiceRunner.SetDumpPreparationParam(index, value);
        }

        public void SetUWordPreparationParam(int index, ushort value)
        {
            DiagServiceRunner.SetUWordPreparationParam(index, value);
        }
        public void SetSLongPreparationParam(int index, int value)
        {
            DiagServiceRunner.SetSLongPreparationParam(index, value);
        }



    }
}
