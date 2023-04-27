using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caesar;
using CaesarInterpreter;

namespace Diogenes
{
    public class DiagServiceRunner : CaesarInterpreter.Host.IDiagServiceRunner
    {
        public DiagService Service = null;
        public BitArray DiagBits = null;
        public BindingList<DiagPreparation> VisiblePreparations = new BindingList<DiagPreparation>();
        public byte[] Response = new byte[] { };
        private FlashItem FlashData;

        public Action FlashProgressChanged = null;
        public FlashHost FlashHost = null;

        public DiagServiceRunner(DiagService ds)
        {
            Service = ds;
            ResetPreparations();
        }

        public void ResetPreparations()
        {
            // pause updates
            VisiblePreparations.RaiseListChangedEvents = false;
            VisiblePreparations.Clear();

            if (Service != null)
            {
                foreach (var prep in Service.InputPreparationPresentations)
                {
                    VisiblePreparations.Add(prep.DeepCopy());
                }
            }

            // restore updates
            VisiblePreparations.RaiseListChangedEvents = true;
            VisiblePreparations.ResetBindings();
        }

        // reassembles preparations into a single bitarray
        public void GenerateRequestBitArray()
        {
            // clear bitarray, check diagservice actually exists
            DiagBits = new(new byte[] { });
            if (Service is null)
            {
                return;
            }
            // initialize the array with sane defaults
            //DiagBits = new BitArray(Service.RequestBytes.Length * 8);
            DiagBits = new BitArray(Service.RequestBytes);


            // fill from preparations
            foreach (var prep in VisiblePreparations)
            {
                // fixme: ic204:DT_Memory_nur_Entwicklung_Length_number_of_bytes_of_the_Memory_Address_parameter, absurdly large value

                //  if the parameter won't fit, complain and skip
                if ((prep.BitPosition + prep.Content.Length) > DiagBits.Length)
                {
                    Console.WriteLine($"{prep.Qualifier} was not written (out of bounds)");
                    continue;
                }
                for (int i = 0; i < prep.Content.Length; i++)
                {
                    DiagBits[i + prep.BitPosition] = prep.Content[i];
                }
            }
        }

        public void DoDiagService()
        {
            // executes a prepared request
            if (DiogenesSharedContext.Singleton.Channel is null)
            {
                Console.WriteLine($"An active ECU connection is required for this request");
                return;
            }

            // prepare the bitarray to be sent
            GenerateRequestBitArray();

            // run scripts, then preparations
            // consideration: varcoding script can probably write the card id to the preparation
            DoDiagServiceScripts();
            DoDiagServiceByPreparation();
        }

        public string GetOutputPreparationResultString()
        {
            if (Response.Length == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            // transform the response to a human-readable string
            foreach (var prepSets in Service.OutputPreparations)
            {
                foreach (var prep in prepSets)
                {
                    // seems like even diag scripts have a return value: see {PRES_TEXTTABLE_SecurityAccess [8] @ 0} in crd3: ..9a
                    var prepPres = prep.ParentECU.GlobalPresentations[prep.PresPoolIndex];
                    sb.AppendLine($"{prep.Qualifier} [{prep.SizeInBits}] @ {prep.BitPosition}");
                    BitArray responseAsBits = new BitArray(Response);
                    BitArray sliced = BitArrayExtension.Slice(responseAsBits, prep.BitPosition, prep.SizeInBits);
                    sb.AppendLine($"{prepPres.DescriptionString}: {prepPres.Interpret(sliced)}");
                }
            }
            return sb.ToString();
        }

        public void DoDiagServiceByPreparation()
        {
            // no scripts, executing a regular request
            byte[] requestData = BitArrayExtension.ToBytes(DiagBits);

            // don't send empty dumps
            if (requestData.Length == 0)
            {
                return;
            }

            Console.WriteLine($"REQ: {Caesar.BitUtility.BytesToHex(requestData, true)}");
            Response = DiogenesSharedContext.Singleton.Channel.Send(requestData, true);
            Console.WriteLine($"ECU: {Caesar.BitUtility.BytesToHex(Response, true)}");
        }

        public void DoDiagServiceScripts()
        {
            // run diag script
            foreach (var script in Service.DiagServiceCode)
            {
                RunSingleDiagScript(script.Qualifier, script.ScriptBytes);
            }
        }

        public void RunSingleDiagScript(string fnName, byte[] dscBytes)
        {
            PhysicalChannel ch = new PhysicalChannel();
            DiagServiceHost dsh = new DiagServiceHost();
            FlashHost = null;
            if (FlashData != null) 
            {
                FlashHost = new FlashHost(FlashData);
                FlashHost.FlashProgressChanged = () => 
                {
                    // pass the event to whoever's accessing the diagservicerunner
                    FlashProgressChanged?.Invoke();
                };
            }

            CaesarInterpreter.Interpreter ih = CaesarInterpreter.Interpreter.Create(dscBytes, fnName, new byte[] { }, ch, dsh, FlashHost);
            CaesarInterpreter.Interpreter.Run(ih);
            // supposed to pick up a response somewhere here and fill up response so that it can be interpreted..?
        }


        public bool IsNegativeResponse()
        {
            bool posResponse = (Response.Length > 1) && (Response[0] != 0x7F);
            return !posResponse;
        }
        public int GetPresParamType(int index)
        {
            // can't confirm, entriegeln bc expects 17u
            // return Service.OutputPreparations[0][index].SystemParam - 0x10; // this is -1?

            // fixme:hack
            Console.WriteLine($"GetPresParamType: {Service.Config}");
            return 17;
        }
        public int GetPrepParamDumpLength(int index)
        {
            return Service.InputPreparations[index].Dump.Length;
        }

        public int GetPresParamDumpLength(int index)
        {
            return Service.OutputPreparations[0][index].SizeInBits / 8;
        }

        public byte[] GetDumpPresParam(int index)
        {
            var pres = Service.OutputPreparations[0][index];
            BitArray dump = new BitArray(Response);
            BitArray slice = BitArrayExtension.Slice(dump, pres.BitPosition, pres.SizeInBits);
            byte[] sliceBytes = BitArrayExtension.ToBytes(slice);
            return sliceBytes;
        }

        public void SetDumpPreparationParam(int index, byte[] value)
        {
            BitArray valueAsBits = new BitArray(value);
            BitArrayExtension.CopyBitsClamped(VisiblePreparations[index].Content, valueAsBits, rightToLeft: false);
        }
        public void SetUWordPreparationParam(int index, ushort value)
        {
            BitArray valueAsBits = BitArrayExtension.ToBitArray(value, bigEndian: true, bitArraySize: 16);
            BitArrayExtension.CopyBitsClamped(VisiblePreparations[index].Content, valueAsBits, rightToLeft: false);
        }

        public void SetSLongPreparationParam(int index, int value)
        {
            BitArray valueAsBits = BitArrayExtension.ToBitArray(value, bigEndian: true, bitArraySize: 32);
            BitArrayExtension.CopyBitsClamped(VisiblePreparations[index].Content, valueAsBits, rightToLeft: false);
        }

        public byte[] GetPresentationDumpValue(int index)
        {

            // hack : ki211 debug requires this
            if (Service.Qualifier == "SEC_SecurityAccess_GetSeedLevel1")
            {
                return new byte[] { 0x27, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            }

            // hack : hu45 debug requires this
            if (Service.Qualifier == "DNU_Security_Access_Get_Level_7_Request_Seed")
            {
                return new byte[] { 0x27, 0x07, 0x01, 0x02, };
            }
            throw new Exception($"GetPresentationDumpValue unimplemented for {Service.Qualifier}");
        }
        public string GetStringPresentationParam(int index) 
        {
            var prep = Service.OutputPreparations[0][index];

            // seems like even diag scripts have a return value: see {PRES_TEXTTABLE_SecurityAccess [8] @ 0} in crd3: ..9a
            var prepPres = Service.ParentECU.GlobalPresentations[prep.PresPoolIndex];

            BitArray responseAsBits = new BitArray(Response);
            BitArray sliced = BitArrayExtension.Slice(responseAsBits, prep.BitPosition, prep.SizeInBits);
            string result = $"{prepPres.Interpret(sliced)}\0"; // insert null terminator
            
            // fixme: this is definitely borked, entriegeln_bc compares the output with a string to check if the response is valid
            // it just happens that the path it takes is what we want
            // check the subsequent strcmp to get an idea of what it is looking for
            Console.WriteLine($"GetStringPresentationParam: {result}");
            return result;

            // seems to read the scaled value (name) from the output presentation
            // return "hello\0";

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

        public void AttachFlashHost(FlashItem flash) 
        {
            FlashData = flash;
        }
    }
}
