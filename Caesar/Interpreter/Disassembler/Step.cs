using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Disassembler
{
    public class Step
    {
        public int Address;
        public int Frequency = 1;
        public ushort Opcode = 0;
        public int JumpDisplacement = 0;
        public int JumpChannel = 0;

        // set this when indicating that the size parameter has been written prematurely (e.g. jump/call)
        public bool StepEndPositionMarked = false;

        public List<string> DescriptionLines = new List<string>();
        public byte[] InstructionBytes = new byte[] { };

        public string InstructionBytesAsString = "";
        public string InstructionDescription = "";


        public void AddDescription(string content) 
        {
            DescriptionLines.Add(content);
            //Console.WriteLine(content);
        }


        private static void InsertDataGapFiller(int lastDataStartPosition, int currentIndex, Interpreter ih) 
        {
            // currently on instruction, close gap if it is open
            if (lastDataStartPosition != -1)
            {
                // copy data bytes
                int payloadSize = currentIndex - lastDataStartPosition;
                byte[] payload = new byte[payloadSize];
                Array.ConstrainedCopy(ih.Context.PALBytes, lastDataStartPosition, payload, 0, payloadSize);

                string label = $"Data {payload.Length}u bytes from 0x{lastDataStartPosition:X} (incl.) to 0x{currentIndex:X} (excl.)";
                ih.Steps.Add(lastDataStartPosition, new Step()
                {
                    Address = lastDataStartPosition,
                    Opcode = 0xFFFF,
                    DescriptionLines = new List<string>() { label },
                    InstructionBytes = payload,
                    InstructionBytesAsString = BitUtility.BytesToHex(payload.Take(4).ToArray()) + "...",
                    InstructionDescription = label,
                });
            }
        }

        // add a gap entry when there is a coverage void between 2 instructions
        public static void DisassemblerFillDataGaps(Interpreter ih) 
        {
            byte[] script = ih.Context.PALBytes;

            int lastDataStartPosition = -1;
            int currentIndex = 0;

            while (currentIndex < script.Length)
            {
                if (ih.Steps.ContainsKey(currentIndex))
                {
                    InsertDataGapFiller(lastDataStartPosition, currentIndex, ih);

                    lastDataStartPosition = -1;

                    // skip past current step
                    Step step = ih.Steps[currentIndex];
                    currentIndex += step.InstructionBytes.Length;

                }
                else 
                {
                    if (lastDataStartPosition == -1) 
                    {
                        // if we weren't on a start of a gap, now we are
                        lastDataStartPosition = currentIndex;
                    }
                    // else we are currently within a gap
                    currentIndex++;
                }
            }

            InsertDataGapFiller(lastDataStartPosition, currentIndex, ih);
        }

        public static void DisassemblerLogStepStart(Interpreter ih, int position)
        {
            // if we are in the middle of a insn-extend, don't replace the current instruction
            if (ih.Opcode >= 0x100) 
            { 
                return;
            }
            ih.ActiveStep = new Step();
            ih.ActiveStep.Address = position;
        }

        public static void DisassemblerAssignJumpChannels(Interpreter ih)
        {
            // this is purely visual so that jumplines can be rendered

            // fixme: i think it's broken
            // trying to get ida/olly-like jumplines

            var listOfJumps = ih.Steps.Values.Where(x => x.JumpDisplacement != 0);

            if (listOfJumps.Count() == 0)
            {
                ih.JumpSteps = new List<Step>();
                ih.JumpChannelMaximumValue = 0;
                return;
            }

            // reset all jump channels
            foreach (var x in listOfJumps)
            {
                x.JumpChannel = int.MaxValue;
            }

            // debug: force values
            int forceVal = 0;
            foreach (var x in listOfJumps)
            {
                x.JumpChannel = forceVal;
                forceVal++;
                forceVal %= 6;
            }
            ih.JumpSteps = listOfJumps.OrderBy(x => x.Address).ToList();
            ih.JumpChannelMaximumValue = ih.JumpSteps.Max(x => x.JumpChannel);
            return;


            int jumpChannelIndex = 0;

            // load initial working set
            List<Step> workingSet = listOfJumps.Where(x => x.JumpChannel == int.MaxValue).ToList();

            while (workingSet.Count > 0) 
            {
                AssignJumpChannelForIndex(jumpChannelIndex, workingSet);
                jumpChannelIndex++;

                // update working set
                workingSet = listOfJumps.Where(x => x.JumpChannel == int.MaxValue).ToList();
            }

            // cache these values now
            ih.JumpSteps = listOfJumps.OrderBy(x => x.Address).ToList();
            ih.JumpChannelMaximumValue = ih.JumpSteps.Max(x => x.JumpChannel);
        }

        private static bool AssignJumpChannelForIndex(int channelIndex, IEnumerable<Step> inListOfJumps) 
        {
            // this is purely visual so that jumplines can be rendered
            // fixme: i think it's also broken

            List<Step> listOfJumps = new List<Step>(inListOfJumps);

            Step sourceStep = listOfJumps.FirstOrDefault(x => x.JumpChannel >= channelIndex);
            
            // return false if theres nothing to do
            if (sourceStep is null) 
            {
                return false;
            }

            // load a first unassigned step. list is ordered by address
            while (sourceStep != null)
            {
                var minMaxAddressSource = GetJumpMinMaxAddresses(sourceStep);
                bool stepIsOverlapped = false;

                foreach (var testStep in listOfJumps)
                {
                    // discard if operating on the same value that we are comparing
                    if (testStep.Address == sourceStep.Address)
                    {
                        continue;
                    }

                    var minMaxAddressCompare = GetJumpMinMaxAddresses(testStep);
                    
                    if (minMaxAddressSource.Item1 > minMaxAddressCompare.Item1) 
                    {
                        // overlapping a test
                        stepIsOverlapped = true;
                        // take it out of the evaluation set, since we can't assign it anymore
                        listOfJumps.Remove(sourceStep);
                        break;
                    }
                }

                if (!stepIsOverlapped) 
                {
                    // this testcase is successful and did not have any overlaps
                    sourceStep.JumpChannel = channelIndex;

                    // done, take it out of the set
                    listOfJumps.Remove(sourceStep);
                }

                // fetch next item
                sourceStep = listOfJumps.FirstOrDefault(x => x.JumpChannel >= channelIndex);

            }
            return true;
        }

        public static Tuple<int, int> GetJumpMinMaxAddresses(Step step)
        {
            int minAddress = step.Address;
            int maxAddress = step.Address + step.JumpDisplacement;
            // check if displacement is negative (backward jump), then swap min/max
            if (maxAddress < minAddress)
            {
                int temp = maxAddress;
                maxAddress = minAddress;
                minAddress = temp;
            }
            return new Tuple<int, int>(minAddress, maxAddress);
        }

        public static void SetCurrentOpcode(Interpreter ih)
        {
            ih.ActiveStep.Opcode = ih.Opcode;
        }
        public static void SetJumpDisplacement(Interpreter ih, int displacement)
        {
            ih.ActiveStep.JumpDisplacement = displacement;
        }

        public static void MarkStepPcEnd(Interpreter ih, int position)
        {
            Step step = ih.ActiveStep;
            
            // if we are in the middle of a insn-extend, don't save the partially-complete step yet
            if (!ih.FetchInstructionOnNextCycle)
            {
                return;
            }

            // pc end can be marked *before* instruction cycle ends
            // this is necessary for jumps, calls
            if (step.StepEndPositionMarked) 
            {
                return;
            }

            int pcDifference = position - step.Address;
            if (pcDifference > 40)
            {
                pcDifference = 40;
            }
            step.InstructionBytes = ih.ScriptBytes.Skip(step.Address).Take(pcDifference).ToArray();
            step.InstructionBytesAsString = BitUtility.BytesToHex(step.InstructionBytes);

            step.StepEndPositionMarked = true;
        }

        public static void DisassemblerLogStepStop(Interpreter ih)
        {
            Step step = ih.ActiveStep;

            // if it's a partial extend, don't do anything to it
            if (!ih.FetchInstructionOnNextCycle) 
            {
                return;
            }

            // if step was already logged, discard current operation
            if (ih.Steps.ContainsKey(step.Address)) 
            {
                ih.Steps[step.Address].Frequency++;
                return;
            }

            // mark our current PC position as the end of the instruction, then diff (end-start) to get address size
            MarkStepPcEnd(ih, ih.Script.Position);

            // assemble description into a single line
            step.InstructionDescription = string.Join("; ", step.DescriptionLines);

            ih.Steps.Add(step.Address, step);
        }

    }
}
