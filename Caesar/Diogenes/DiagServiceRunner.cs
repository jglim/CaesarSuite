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
    public class DiagServiceRunner
    {
        public DiagService Service = null;
        public BitArray DiagBits = null;
        public BindingList<DiagPreparation> VisiblePreparations = new BindingList<DiagPreparation>();

        public DiagServiceRunner(DiagService ds) 
        {
            Service = ds;
        }

        public void ResetPreparations() 
        {
            // pause updates
            VisiblePreparations.RaiseListChangedEvents = false;
            VisiblePreparations.Clear();

            if (Service != null)
            {
                foreach (var prep in Service.InputPreparations)
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
            DiagBits = new BitArray(Service.RequestBytes.Length * 8);

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

        public void ExecuteDiagService() 
        {

            // executes a prepared request
            if (DiogenesSharedContext.Singleton.Channel is null)
            {
                Console.WriteLine($"An active ECU connection is required for this request");
                return;
            }

            if (Service.DiagServiceCode.Count == 0)
            {
                // no scripts, executing a regular request
                byte[] requestData = BitArrayExtension.ToBytes(DiagBits);

                // don't send empty dumps
                if (requestData.Length == 0)
                {
                    Console.WriteLine($"This request does not appear to be supported");
                    return;
                }

                Console.WriteLine($"REQ: {Caesar.BitUtility.BytesToHex(requestData, true)}");
                byte[] response = DiogenesSharedContext.Singleton.Channel.Send(requestData, true);
                Console.WriteLine($"ECU: {Caesar.BitUtility.BytesToHex(response, true)}");


                bool positiveResponse = (response.Length > 1) && (response[0] != 0x7F);
                if (!positiveResponse)
                {
                    return;
                }

                // transform the response to a human-readable string
                foreach (var prepSets in Service.OutputPreparations)
                {
                    foreach (var prep in prepSets)
                    {
                        var prepPres = prep.ParentECU.GlobalPresentations[prep.PresPoolIndex];
                        Console.WriteLine($"{prep.Qualifier} [{prep.SizeInBits}] @ {prep.BitPosition}");
                        BitArray responseAsBits = new BitArray(response);
                        BitArray sliced = BitArrayExtension.Slice(responseAsBits, prep.BitPosition, prep.SizeInBits);
                        Console.WriteLine(prepPres.Interpret(sliced));
                    }
                }
            }
            else
            {
                // run diag script
                foreach (var script in Service.DiagServiceCode)
                {
                    RunSingleDiagScript(script.Qualifier, script.ScriptBytes);
                }

            }

        }

        public void RunSingleDiagScript(string fnName, byte[] dscBytes) 
        {
            PhysicalChannel ch = new PhysicalChannel();
            Interpreter ih = Interpreter.Create(dscBytes, fnName, new byte[] { }, ch);
            Interpreter.Run(ih);
        }
    }
}
