using CaesarInterpreter.Disassembler;
using CaesarInterpreter.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class Interpreter
    {
        public DSCContext Context;
        public ExtendedBinaryStream Stack;
        public ExtendedBinaryStream Script;
        public List<GlobalVariable> GlobalVariables = new List<GlobalVariable>();
        public Dictionary<int, TrackedObject> TrackedObjects = new Dictionary<int, TrackedObject>();
        public ushort Opcode;
        public bool FetchInstructionOnNextCycle = true;
        public byte[] ScriptBytes = new byte[] { };
        
        public int TextIOPointer;
        public int VirtualStackBase;
        public int VirtualStackTop;
        public int HardStackTop;

        public int CycleCount;
        public InterpreterHacks Hacks;
        public byte[] PreinitializedParameterBytes = new byte[] { };

        // script host interaction
        public IChannel HostChannel;
        public IDiagService HostDiagService;
        public IFlash HostFlash;

        // disassembler-specific, not required for emulation
        public Step ActiveStep;
        public SortedDictionary<int, Disassembler.Step> Steps;
        public List<Step> JumpSteps;
        public int JumpChannelMaximumValue;

        public bool BreakExecution;

        public StringBuilder ChannelLog;
        public StringBuilder CommunicationLog;

        public Function EntryFunction;
        const int StackPadding = 5000;

        public bool AllowSelfModifyingCode = true;
        public bool CodeWasSelfModified = false;

        public static Interpreter Create(byte[] palBytes, string functionName, byte[] paramBytes, IChannel hostChannel, IDiagService hostDiagService, IFlash flashHost)
        {
            Interpreter ih = new Interpreter();
            ih.ScriptBytes = palBytes;

            // configure 'unofficial' hacks
            // ih.Hacks = new InterpreterHacks(ih);

            // attach host interfaces
            ih.HostChannel = hostChannel;
            ih.HostDiagService = hostDiagService;
            ih.HostFlash = flashHost;

            // parse the pal file
            ih.Context = new DSCContext(palBytes);
            ih.GlobalVariables = new List<GlobalVariable>();

            // create a set of mutable global variables
            ih.Context.GlobalVariables.ForEach(x => ih.GlobalVariables.Add(x.DeepCopy()));

            // executable file reader. cursor acts as program counter (pc)
            ih.Script = new ExtendedBinaryStream(palBytes);

            // find and seek to function entrypoint
            ih.EntryFunction = ih.Context.Functions.First(x => x.Name == functionName);
            ih.Script.Seek(ih.EntryFunction.EntryPoint, System.IO.SeekOrigin.Begin);

            // first 2 bytes (as u16) at a function entrypoint specifies the required stack height
            int scriptRequiredStackHeight = ih.Script.ReadU16();

            // insert function param bytes if available
            scriptRequiredStackHeight += paramBytes.Length;

            // allocate stack
            byte[] stack = new byte[scriptRequiredStackHeight + StackPadding];
            ih.Stack = new ExtendedBinaryStream(stack);

            // set stackptr
            ih.Stack.Seek(scriptRequiredStackHeight, System.IO.SeekOrigin.Begin);
            Console.WriteLine($"Stack allocated with size {stack.Length} and offset at 0x{scriptRequiredStackHeight:X8}");

            // copy in function bytes
            Array.ConstrainedCopy(paramBytes, 0, stack, 0, paramBytes.Length);

            // set virtual pointers
            ih.TextIOPointer = ih.Stack.Position;
            ih.VirtualStackBase = paramBytes.Length;
            ih.VirtualStackTop = stack.Length;
            ih.HardStackTop = stack.Length;

            // store paramBytes, this is needed to sync logging with hooked c32s
            ih.PreinitializedParameterBytes = paramBytes;

            // reset any stop condition
            ih.BreakExecution = false;

            // reset self-modifying code flag
            ih.CodeWasSelfModified = false;

            // count number of cycles
            ih.CycleCount = 0;

            // dasm: log every step
            ih.Steps = new SortedDictionary<int, Disassembler.Step>();

            // dasm: prepare logging
            ih.ChannelLog = new StringBuilder();
            ih.CommunicationLog = new StringBuilder();

            return ih;
        }

        public static void Run(Interpreter ih) 
        {
            Console.WriteLine($"Starting script execution for `{ih.EntryFunction.Name}`");

            int breakpointAddress = -1; // -1 to disable
            while (true)
            {
                if (ih.CycleCount == breakpointAddress)
                {
                    break;
                }
                if (ih.BreakExecution) 
                {
                    ih.BreakExecution = false;
                    break;
                }
                if (!SingleStep(ih))
                {
                    break;
                }
                ih.CycleCount++;
            }

            Console.WriteLine($"Script execution of `{ih.EntryFunction.Name}` has completed successfully in {ih.CycleCount}u cycles");
            if (ih.CodeWasSelfModified)
            {
                Console.WriteLine($"Self-modifying code was present in this script");
            }
        }

        public static void FetchInstruction(Interpreter ih) 
        {
            if (ih.FetchInstructionOnNextCycle)
            {
                ih.Opcode = ih.Script.ReadU8();
            }
            else 
            {
                ih.FetchInstructionOnNextCycle = true;
            }
        }

        public static bool SingleStep(Interpreter ih) 
        {

            FetchInstruction(ih);
            Disassembler.Step.DisassemblerLogStepStart(ih, ih.Script.Position - 1);

            Disassembler.Step.SetCurrentOpcode(ih);

            //Console.WriteLine();
            //Console.WriteLine($"Step PC: {ih.Script.Position:X8}, OP: {ih.Opcode:X4}, F: {ih.Script.Position:X8}, SP: {ih.Stack.Position:X8}, SyncSP {(ih.Stack.Position-ih.PreinitializedParameterBytes.Length):X8} CC: {ih.CycleCount}u VST: {ih.VirtualStackTop:X8} ({(ih.HardStackTop - ih.VirtualStackTop):X})");

            switch (ih.Opcode)
            {
                case 0:
                    Instructions.Core.NoOperation(ih);
                    break;

                case 1:
                case 2:
                case 0x32D:
                case 3:
                case 0x32E:
                    Instructions.Push.PushStackAbsolute(ih);
                    break;

                case 0x7:
                case 0x8:
                case 0x333:
                case 0x9:
                case 0x334:
                case 0xA:
                case 0xB:
                case 0x336:
                case 0xC:
                case 0x337:
                    Instructions.Push.PushValuePointedByStackBase(ih);
                    break;

                case 0xD:
                case 0xE:
                case 0x339:
                case 0xF:
                case 0x33A:
                    Instructions.Store.LoadPointerValue(ih);
                    break;

                case 0x13: 
                case 0x14:
                case 0x15:
                    Instructions.Push.PushFunctionParameter(ih);
                    break;

                case 0x16:
                    Instructions.Store.LoadPointerValueNegative(ih);
                    break;

                case 0x19:
                case 0x1A:
                case 0x345:
                case 0x1B:
                case 0x346:
                case 0x1C:
                case 0x1D:
                case 0x348:
                case 0x1E:
                case 0x349:
                    Instructions.Push.PushFromGlobalVarScalar(ih);
                    break;

                case 0x1F:
                case 0x20:
                case 0x34B:
                case 0x21:
                case 0x34C:
                    Instructions.Push.PushFromGlobalVarVector(ih);
                    break;

                case 0x22:
                case 0x23:
                case 0x34E:
                case 0x24:
                case 0x34F:
                case 0x25:
                case 0x26:
                case 0x351:
                case 0x27:
                case 0x352:
                    Instructions.Push.PushFromGlobalVarDereference(ih);
                    break;

                case 0x41:
                case 0x42:
                case 0x43:
                    Instructions.Push.PushImmediate(ih);
                    break;

                case 0x60:
                case 0x1A2:
                case 0x1A8:
                case 0x1AA:
                case 0x61:
                case 0x1A3:
                case 0x1A9:
                case 0x1AB:
                case 0x62:
                case 0x1A5:
                case 0x1AD:
                case 0x1AF:
                case 0x63:
                case 0x1A6:
                case 0x1AE:
                case 0x1B0:
                    Instructions.ControlFlow.CompareEquality(ih);
                    break;

                case 0x64:
                case 0x1B7:
                case 0x1BD:
                case 0x1BF:
                case 0x65:
                case 0x1B8:
                case 0x1BE:
                case 0x1C0:
                case 0x66:
                case 0x1BA:
                case 0x1C2:
                case 0x1C4:
                case 0x67:
                case 0x1BB:
                case 0x1C3:
                case 0x1C5:
                    Instructions.ControlFlow.CompareNonEquality(ih);
                    break;

                case 0x68:
                case 0x69:
                case 0x6A:
                case 0x6B:
                case 0x1CC:
                case 0x1CF:
                case 0x1D2:
                case 0x1D3:
                case 0x1D4:
                    Instructions.ControlFlow.CompareLessThan(ih);
                    break;

                case 0x6C:
                    Instructions.ControlFlow.CompareLessThanOrEqual(ih);
                    break;

                case 0x213:
                    Instructions.ControlFlow.CompareGreaterThanOrEqual(ih);
                    break;

                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x1FD:
                case 0x1FE:
                    Instructions.ControlFlow.CompareGreaterThan(ih);
                    break;

                case 0x78:
                case 0x79:
                case 0x7A:
                case 0x7B:
                case 0x7C:
                case 0x7D:
                case 0x7E:
                case 0x7F:
                case 0x80:
                case 0x81:
                case 0x289:
                case 0x28A:
                case 0x28B:
                case 0x28C:
                case 0x28D:
                case 0x28E:
                case 0x28F:
                case 0x290:
                case 0x291:
                case 0x292:
                case 0x293:
                case 0x294:
                case 0x295:
                case 0x296:
                case 0x297:
                    Instructions.Store.TwoStageStore(ih);
                    break;
                    
                case 0x83:
                case 0xA3:
                    Instructions.Load.LoadA(ih);
                    break;


                case 0x8A:
                case 0x2A1:
                case 0x8B:
                case 0x2A2:
                    Instructions.ControlFlow.CompareZero(ih);
                    break;

                case 0x88:
                case 0x29E:
                    Instructions.ArithmeticLogic.BitwiseInvert(ih);
                    break;

                case 0x9B:
                case 0x26B:
                case 0x266:
                case 0x267:
                    Instructions.ArithmeticLogic.BitwiseOr(ih);
                    break;

                case 0x9F:
                case 0x27C:
                case 0x280:
                    Instructions.ArithmeticLogic.BitwiseXor(ih);
                    break;

                case 0xA4:
                case 0xA5:
                case 0xA6:
                case 0xA7:
                    Instructions.ControlFlow.UnconditionalJump(ih);
                    break;

                case 0xA8:
                case 0xA9:
                case 0xAA:
                case 0xAB:
                case 0xAC:
                case 0xAD:
                case 0xAE:
                case 0xAF:
                    Instructions.ControlFlow.ConditionalJump(ih);
                    break;

                case 0xB1:
                case 0xB2:
                    Instructions.ControlFlow.SwitchJump(ih);
                    break;

                case 0xB3:
                case 0xBD:
                case 0xB4:
                    Instructions.Core.StackPopDiscard(ih);
                    break;

                case 0xB5:
                case 0xB6:
                case 0xB8:
                    Instructions.ControlFlow.Call(ih);
                    break;

                case 0xB9:
                    {
                        if (!Instructions.ControlFlow.SetEndOfFunction(ih)) 
                        {
                            // if return -> false, signal script end
                            return false;
                        }
                        break;
                    }

                case 0xBA:
                    {
                        Instructions.ControlFlow.Return(ih);
                        break;
                    }
                case 0xBB:
                    {
                        if (!Instructions.ControlFlow.SetEndOfFunctionAndReturn(ih))
                        {
                            // if return -> false, signal script end
                            return false;
                        }
                        break;
                    }

                case 0xBE:
                    Instructions.Push.PushZero(ih);
                    break;

                case 0xFD:
                case 0xFE:
                case 0xFF:
                    Instructions.Core.ExtendInstruction(ih);
                    break;

                case 0x10E:
                    Instructions.Push.PushScriptOffset(ih);
                    break;

                case 0x47:
                case 0x10F:
                case 0x110:
                case 0x111:
                case 0x112:
                case 0x113:
                case 0x114:
                case 0x115:
                case 0x116:
                case 0x117:
                case 0x118:
                case 0x119:
                case 0x11A:
                case 0x11B:
                case 0x11C:
                case 0x11D:
                case 0x11E:
                case 0x11F:
                case 0x120:
                case 0x121:
                case 0x122:
                case 0x123:
                    Instructions.ArithmeticLogic.Add(ih);
                    break;

                case 0x124:
                case 0x125:
                case 0x126:
                case 0x127:
                case 0x128:
                case 0x129:
                case 0x12A:
                case 0x12B:
                case 0x12C:
                case 0x12D:
                case 0x12E:
                case 0x12F:
                case 0x130:
                case 0x131:
                case 0x132:
                case 0x133:
                case 0x134:
                case 0x135:
                case 0x136:
                case 0x137:
                case 0x138:
                    Instructions.ArithmeticLogic.Subtract(ih);
                    break;

                case 0x4F:
                case 0x139:
                case 0x13A:
                case 0x13B:
                case 0x13C:
                case 0x13D:
                case 0x13E:
                case 0x13F:
                case 0x140:
                case 0x141:
                case 0x142:
                case 0x143:
                case 0x144:
                case 0x145:
                case 0x146:
                case 0x147:
                case 0x148:
                case 0x149:
                case 0x14A:
                case 0x14B:
                case 0x14C:
                case 0x14D:
                    Instructions.ArithmeticLogic.Multiply(ih);
                    break;

                case 0x178:
                case 0x17E:
                case 0x180:
                case 0x179:
                case 0x17F:
                case 0x181:
                    Instructions.ArithmeticLogic.LeftShift(ih);
                    break;

                case 0x194:
                case 0x195:
                    Instructions.ArithmeticLogic.RightShift(ih);
                    break;

                case 0x1A4:
                case 0x1A7:
                case 0x1AC:
                case 0x1B1:
                case 0x1B2:
                case 0x1B3:
                case 0x1B4:
                case 0x1B5:
                case 0x1B6:
                case 0x1B9:
                case 0x1BC:
                case 0x1C1:
                case 0x1C6:
                case 0x1C7:
                case 0x1C8:
                case 0x1C9:
                case 0x1CA:
                case 0x1CB:
                    // 1CC moved
                case 0x1CD:
                case 0x1CE:
                    // 1CF moved
                case 0x1D0:
                case 0x1D1:
                    // 1d2 moved
                    // 1d3 moved
                case 0x1D5:
                case 0x1D6:
                case 0x1D7:
                case 0x1D8:
                case 0x1D9:
                case 0x1DA:
                case 0x1DB:
                case 0x1DC:
                case 0x1DD:
                case 0x1DE:
                case 0x1DF:
                case 0x1E0:
                case 0x1E1:
                case 0x1E2:
                case 0x1E3:
                case 0x1E4:
                case 0x1E5:
                case 0x1E6:
                case 0x1E7:
                case 0x1E8:
                case 0x1E9:
                case 0x1EA:
                case 0x1EB:
                case 0x1EC:
                case 0x1ED:
                case 0x1EE:
                case 0x1EF:
                case 0x1F0:
                case 0x1F1:
                case 0x1F2:
                case 0x1F3:
                case 0x1F4:
                case 0x1F5:
                case 0x1F6:
                case 0x1F7:
                case 0x1F8:
                case 0x1F9:
                case 0x1FA:
                case 0x1FB:
                case 0x1FC:
                    // 1FD moved
                case 0x1FF:
                case 0x200:
                case 0x201:
                case 0x202:
                case 0x203:
                case 0x204:
                case 0x205:
                case 0x206:
                case 0x207:
                case 0x208:
                case 0x209:
                case 0x20A:
                case 0x20B:
                case 0x20C:
                case 0x20D:
                case 0x20E:
                case 0x20F:
                case 0x210:
                case 0x211:
                case 0x212:
                case 0x214:
                case 0x215:
                case 0x216:
                case 0x217:
                case 0x218:
                case 0x219:
                case 0x21A:
                case 0x21B:
                case 0x21C:
                case 0x21D:
                case 0x21E:
                case 0x21F:
                case 0x222:
                case 0x225:
                case 0x22A:
                case 0x22F:
                    Instructions.ControlFlow.Compare(ih);
                    break;
                
                case 0x24D:
                case 0x250:
                case 0x251:
                case 0x252:
                case 0x24E:
                case 0x256:
                    Instructions.ArithmeticLogic.BitwiseAnd(ih);
                    break;

                case 0x2BE:
                    Instructions.Core.MemoryCopy(ih);
                    break;

                case 0x2C3:
                    Instructions.TextIo.Strcmp(ih);
                    break;

                case 0x2C4:
                    Instructions.TextIo.Strcpy(ih);
                    break;

                case 0x2C7:
                    Instructions.TextIo.Strlen(ih);
                    break;

                case 0x2D1:
                    Instructions.TextIo.BiSprintf(ih);
                    break;

                case 0x2D7:
                    Instructions.TextIo.Unk2(ih);
                    break;

                case 0x2DB:
                    Instructions.Core.AllocateBuffer(ih);
                    break;

                case 0x2DD:
                    Instructions.Core.FreeBuffer(ih);
                    break;

                case 0x2E2:
                    Instructions.ChannelIo.RaiseChannelError(ih);
                    break;

                case 0x2E3:
                    Instructions.ChannelIo.ResetChannelError(ih);
                    break;

                case 0x2E4:
                    Instructions.ChannelIo.ChannelErrorIsSet(ih);
                    break;

                case 0x2E6:
                    Instructions.ChannelIo.DebugLine(ih);
                    break;

                case 0x2EB:
                    Instructions.ChannelIo.CollectionMessageSendRequest(ih);
                    break;

                case 0x2EC:
                    Instructions.ChannelIo.CollectMessageStart(ih);
                    break;

                case 0x2ED:
                    Instructions.ChannelIo.CollectMessageRun(ih);
                    break;

                case 0x2EE:
                    Instructions.ChannelIo.CollectMessageEnd(ih);
                    break;

                case 0x2EF:
                    Instructions.ChannelIo.SetCommunicationParameter(ih);
                    break;

                case 0x2F0:
                    Instructions.ChannelIo.GetCommunicationParameter(ih);
                    break;

                case 0x30B:
                    Instructions.DSIO.CreateDSIO(ih);
                    break;

                case 0x30C:
                    Instructions.DSIO.DoDiagService(ih);
                    break;

                case 0x30D:
                    Instructions.DSIO.DeleteDSIO(ih);
                    break;

                case 0x318:
                    Instructions.ChannelIo.CollectionMessageCreate(ih);
                    break;

                case 0x319:
                case 0x31D:
                    Instructions.ChannelIo.FreeRequest(ih);
                    break;

                case 0x31B:
                    Instructions.ChannelIo.CollectMessageWriteToPointers(ih);
                    break;

                case 0x31C:
                    Instructions.ChannelIo.CollectionResponseCreate(ih);
                    break;

                case 0x353:
                case 0x354:
                case 0x355:
                case 0x356:
                case 0x357:
                case 0x358:
                case 0x359:
                case 0x35A:
                case 0x35B:
                case 0x35C:
                case 0x35D:
                case 0x35E:
                case 0x35F:
                    Instructions.TypeConversion.Convert(ih);
                    break;

                case 0x362:
                    Instructions.Core.Sleep(ih);
                    break;

                case 0x389:
                    Instructions.DSIO.GetPresParamType(ih);
                    break;

                case 0x38C:
                    Instructions.DSIO.GetPresParamDumpLength(ih);
                    break;

                case 0x38E:
                    Instructions.DSIO.GetPresentationDumpValue(ih);
                    break;

                case 0x393:
                    Instructions.DSIO.GetStringPresentationParam(ih);
                    break;

                case 0x39C:
                    Instructions.DSIO.GetDumpPresParam(ih);
                    break;

                case 0x3A5:
                    Instructions.DSIO.GetPrepParamDumpLength(ih);
                    break;

                case 0x3A7:
                    Instructions.DSIO.SetUWordPreparationParam(ih);
                    break;

                case 0x3A9:
                    Instructions.DSIO.SetSLongPreparationParam(ih);
                    break;

                case 0x3B0:
                    Instructions.DSIO.SetDumpPreparationParam(ih);
                    break;

                case 0x3B1:
                    Instructions.DSIO.IsNegativeResponse(ih);
                    break;
                
                case 0x3B6:
                case 0x3B7:
                case 0x3B8:
                case 0x3B9:
                case 0x3BA:
                case 0x3BB:
                case 0x3BC:
                case 0x3BD:
                case 0x3BE:
                case 0x3BF:
                case 0x3C0:
                case 0x3C1:
                    Instructions.CaesarDateTime.GetDateTimeComponent(ih);
                    break;
                
                case 0x3C4:
                    Instructions.Core.GetCardID(ih);
                    break;

                case 0x3C5:
                    Instructions.Core.GetTesterID(ih);
                    break;

                case 0x3C7:
                    Instructions.Core.GetPreparedMessage(ih);
                    break;

                case 0x3C8:
                    Instructions.Core.SetMessageToPresent(ih);
                    break;

                case 0x3D1:
                    Instructions.Flash.MLDoDownload(ih);
                    break;

                case 0x3D5:
                    Instructions.Flash.MLGetDatablockChecksum(ih);
                    break;

                case 0x3D7:
                    Instructions.Flash.GetDatablockSecuritySignature(ih);
                    break;

                case 0x3D8:
                    Instructions.TextIo.Unk1(ih);
                    break;

                case 0x3DC:
                    Instructions.ChannelIo.SetErrorWithText(ih);
                    break;

                case 0x3F3:
                    Instructions.Flash.MLOpenSessionDataBlockByIndex(ih);
                    break;

                case 0x3F6:
                    Instructions.Flash.DIGetDataBlockNumberOfSecurities(ih);
                    break;

                default: 
                    {
                        DumpState(ih);
                        throw new Exception($"Unhandled opcode: {ih.Opcode:X4} at cycle {ih.CycleCount}");
                    }
            }


            Disassembler.Step.DisassemblerLogStepStop(ih);

            return true;
        }

        public static void DumpState(Interpreter ih) 
        {
            Console.WriteLine($"============================================================");
            Console.WriteLine($"                 dumping interpreter state                  ");
            Console.WriteLine($"============================================================");

            int lastPosition = ih.Stack.Position;
            ih.Stack.Position = ih.TextIOPointer;
            byte[] dump = ih.Stack.ReadBytes(0x30);
            ih.Stack.Position = lastPosition;
            Console.WriteLine($"SP dump @ TextIOPointer: {BitUtility.BytesToHex(dump)}");

            Console.WriteLine("\n\nGlobal Variables:");
            ih.GlobalVariables.ForEach(x => Console.WriteLine($"gv: {x} : {BitUtility.BytesToHex(x.Buffer.Take(0x10).ToArray())}"));

            Console.WriteLine("\n\nTracked Objects:");
            foreach (var x in ih.TrackedObjects)
            {
                Console.WriteLine($"TrackedObject: 0x{x.Value.GetPointer():X8} {x.Value.GetType().Name} {x.Value}");
            }

        }

    }
}
