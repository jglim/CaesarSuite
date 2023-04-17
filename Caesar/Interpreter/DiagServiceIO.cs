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
        public DiagServiceIO(Interpreter ih, string name, ushort unk1) : base(ih) 
        {
            Name = name;
            Unknown1 = unk1;
            // create a runner in the host
        }

        public override string ToString()
        {
            return $"T#{ObjectIndex}: DSIO {Name}, Unk1: {Unknown1:X4}";
        }

        public bool IsNegativeResponse() 
        {
            return false;
        }

        public void DoDiagService() 
        {
        
        }

        public int GetPrepPresType(int index) 
        {
            if (Name == "DNU_LOGIN_BC")
            {
                return 17; // "expect 17 got XX"
            }
            throw new Exception($"GetPrepPresType unimplemented for {Name}");
        }

        public int GetPrepParamDumpLength(int index) 
        {
            if (Name == "DNU_LOGIN_BC")
            {
                return 4;
            }
            /*
Name	Byte Position	Data Size	Inferred Type	Direction	Raw Data
SID	0	1	IntegerType	Input	27
sendKey	1	1	IntegerType	Input	0C
key	2	4	NativeInfoPoolType	Input	
PRES_Login_BC	0	2	NativePresentationType	Output (0)	
             */
            throw new Exception($"GetPrepParamDumpLength unimplemented for {Name}");
        }

        public int GetPresParamDumpLength(int index) 
        {
            // hack : ki211 debug requires this
            if (Name == "DNU_SEED_BC")
            {
                /*
                Name	Byte Position	Data Size	Inferred Type	Direction	Raw Data
                SID	0	1	IntegerType	Input	27
                request_Seed_Stufe_11	1	1	IntegerType	Input	0B
                PRES_4ByteDump	2	4	NativePresentationType	Output (0)	<----------
                */
                return 4;
            }
            throw new Exception($"GetPresParamDumpLength unimplemented for {Name}");
        }

        public byte[] GetDumpPresParam(int index) 
        {
            if (Name == "DNU_SEED_BC")
            {
                return new byte[] { 0x11, 0x22, 0x33, 0x44, };
            }
            throw new Exception($"GetDumpPresParam unimplemented for {Name}");
        }

        public byte[] GetPresentationDumpValue(int index) 
        {
            // hack : ki211 debug requires this
            if (Name == "SEC_SecurityAccess_GetSeedLevel1")
            {
                return new byte[] { 0x27, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            }
            
            // hack : hu45 debug requires this
            if (Name == "DNU_Security_Access_Get_Level_7_Request_Seed")
            {
                return new byte[] { 0x27, 0x07, 0x01, 0x02, };
            }
            throw new Exception($"GetPresentationDumpValue unimplemented for {Name}");
        }

        public string GetStringPresentationParam(int index) 
        {
            // seems to read the scaled value (name) from the output presentation
            return "hello\0";

            /*
Step PC: 00000663, OP: 03A5, F: 00000663, SP: 00000196, SyncSP 00000196 CC: 1508u VST: 00001434 (C)
GetPrepParamDumpLength: DNU_LOGIN_BC with paramindex 0u, returning 4
...

Step PC: 0000075A, OP: 0393, F: 0000075A, SP: 00000196, SyncSP 00000196 CC: 1554u VST: 00001434 (C)
GetStringPresentationParam: DNU_LOGIN_BC, parameter#0

Step PC: 0000070B, OP: 0389, F: 0000070B, SP: 00000196, SyncSP 00000196 CC: 1543u VST: 00001434 (C)
GetPresParamType: DNU_LOGIN_BC with paramindex 0u, returning type 17

Step PC: 0000076B, OP: 02C4, F: 0000076B, SP: 00000198, SyncSP 00000198 CC: 1563u VST: 00001434 (C)
Strcpy: 0x40030000 to 0x10000064, src: `hello`

Step PC: 0000017A, OP: 02C4, F: 0000017A, SP: 000000C0, SyncSP 000000C0 CC: 1590u VST: 00001440 (0)
Strcpy: 0x10000064 to 0x300001BB, src: `hello`

Step PC: 000000C6, OP: 02C3, F: 000000C6, SP: 000000C0, SyncSP 000000C0 CC: 1607u VST: 00001440 (0)
String compare (equal: False) 1@30000208: 'Timer noch nicht abgelaufen', 2@10000064: 'hello'`

             */
        }
        public void SetDumpPreparationParam(int index, byte[] value) 
        {
        
        }

        public void SetUWordPreparationParam(int index, ushort value)
        {

        }
        public void SetSLongPreparationParam(int index, int value)
        {

        }



    }
}
