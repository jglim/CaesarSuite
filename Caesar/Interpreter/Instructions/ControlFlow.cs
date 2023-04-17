using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class ControlFlow
    {
        public static void UnconditionalJump(Interpreter ih)
        {
            int pcDelta = 0;
            int sizeOfDisplacementValue = 0;
            switch (ih.Opcode)
            {
                case 0xA4:
                    {
                        pcDelta = ih.Script.PeekU8();
                        sizeOfDisplacementValue = 1;
                        break;
                    }
                case 0xA5:
                    {
                        pcDelta = ih.Script.PeekI16();
                        sizeOfDisplacementValue = 2;
                        break;
                    }
                case 0xA6:
                    {
                        pcDelta = -ih.Script.PeekU8();
                        sizeOfDisplacementValue = 1;
                        break;
                    }
                case 0xA7:
                    {
                        pcDelta = -ih.Script.PeekI16();
                        sizeOfDisplacementValue = 2;
                        break;
                    }
            }
            Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + sizeOfDisplacementValue);
            Disassembler.Step.SetJumpDisplacement(ih, pcDelta);
            ih.Script.Seek(pcDelta);
            ih.ActiveStep.AddDescription($"Unconditional jump to 0x{ih.Script.Position:X4}");
        }
        public static void ConditionalJump(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                // A8 to AB takes a 2 byte value to check
                case 0xA8:
                    {
                        // pc is byte-sized
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();

                        int displacement = 1;
                        if (op > 0)
                        {
                            displacement = ih.Script.PeekU8();
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 1);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU8());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X4} > 0, displacement: 0x{displacement:X2}");
                        break;
                    }
                case 0xA9:
                    {
                        // pc is word-sized
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        int displacement = 2;
                        if (op > 0)
                        {
                            displacement = ih.Script.PeekU16();
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 2);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU16());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X4} > 0, displacement: 0x{displacement:X4}");
                        break;
                    }
                case 0xAA:
                    {
                        // pc is byte-sized
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        int displacement = ih.Script.PeekU8();
                        if (op > 0)
                        {
                            displacement = 1;
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 1);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU8());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X4} == 0, displacement: 0x{displacement:X2}");
                        break;
                    }
                case 0xAB:
                    {
                        // pc is word-sized
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        int displacement = ih.Script.PeekU16();
                        if (op > 0)
                        {
                            displacement = 2;
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 2);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU16());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X4} == 0, displacement: 0x{displacement:X4}");
                        break;
                    }
                // AC to AF takes a 4 byte value to check
                case 0xAC:
                    {
                        // pc is byte-sized
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        int displacement = 1;
                        if (op > 0)
                        {
                            displacement = ih.Script.PeekU8();
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 1);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU8());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X8} > 0, displacement: 0x{displacement:X2}");
                        break;
                    }
                case 0xAD:
                    {
                        // pc is word-sized
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        int displacement = 2;
                        if (op > 0)
                        {
                            displacement = ih.Script.PeekU16();
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 2);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU16());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X8} > 0, displacement: 0x{displacement:X4}");
                        break;
                    }
                case 0xAE:
                    {
                        // pc is byte-sized
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        int displacement = ih.Script.PeekU8();
                        if (op > 0)
                        {
                            displacement = 1;
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 1);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU8());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X8} == 0, displacement: 0x{displacement:X2}");
                        break;
                    }
                case 0xAF:
                    {
                        // pc is word-sized
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        int displacement = ih.Script.PeekU16();
                        if (op > 0)
                        {
                            displacement = 2;
                        }
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 2);
                        Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU16());
                        ih.Script.Seek(displacement);
                        ih.ActiveStep.AddDescription($"Jump if 0x{op:X8} == 0, displacement: 0x{displacement:X4}");
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unhandled conditional jump: 0x{ih.Opcode:X4}");
                    }
            }
        }


        public static void CompareEquality(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x60:
                case 0x1A2:
                case 0x1A8:
                case 0x1AA:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = (leftOp == rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X4} == 0x{rightOp:X4} = {result}");
                        break;
                    }
                case 0x61:
                case 0x1A3:
                case 0x1A9:
                case 0x1AB:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = (leftOp == rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X8} == 0x{rightOp:X4} = {result}");
                        break;
                    }
                case 0x62:
                case 0x1A5:
                case 0x1AD:
                case 0x1AF:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = (leftOp == rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X4} == 0x{rightOp:X8} = {result}");
                        break;
                    }
                case 0x63:
                case 0x1A6:
                case 0x1AE:
                case 0x1B0:
                    {
                        ih.Stack.Seek(-4);
                        int rightOp = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int leftOp = ih.Stack.PeekI32();
                        int result = (leftOp == rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X8} == 0x{rightOp:X8} = {result}");
                        break;
                    }
            }
        }

        public static void CompareNonEquality(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x64:
                case 0x1B7:
                case 0x1BD:
                case 0x1BF:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = (leftOp != rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X4} != 0x{rightOp:X4} = {result}");
                        break;
                    }
                case 0x65:
                case 0x1B8:
                case 0x1BE:
                case 0x1C0:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = (leftOp != rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X8} != 0x{rightOp:X4} = {result}");
                        break;
                    }
                case 0x66:
                case 0x1BA:
                case 0x1C2:
                case 0x1C4:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = (leftOp != rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X4} != 0x{rightOp:X8} = {result}");
                        break;
                    }
                case 0x67:
                case 0x1BB:
                case 0x1C3:
                case 0x1C5:
                    {
                        ih.Stack.Seek(-4);
                        int rightOp = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int leftOp = ih.Stack.PeekI32();
                        int result = (leftOp != rightOp) ? 1 : 0;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"Compare 0x{leftOp:X8} != 0x{rightOp:X8} = {result}");
                        break;
                    }
            }
        }

        public static void Compare(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x1E9:
                    {
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16();
                        int result = leftOp <= rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} <= {rightOp:X4} = {result}");
                        break;
                    }
                default:
                    {
                        Interpreter.DumpState(ih);
                        throw new NotImplementedException($"Unhandled compare opcode: 0x{ih.Opcode:X4}");
                    }
            }
        }


        public static void CompareLessThan(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x68:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X4} = {result}");
                        break;
                    }
                case 0x69:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} < {rightOp:X4} = {result}");
                        break;
                    }
                case 0x6A:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X8} = {result}");
                        break;
                    }
                case 0x6B:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} < {rightOp:X8} = {result}");
                        break;
                    }
                case 0x1CC:
                    {
                        // partially signed
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16(); // read as u16, then casted movxz to i32
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16(); // read as i16
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X4} = {result}");
                        break;
                    }
                case 0x1CF:
                    {
                        // partially signed
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16(); // read as i16
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X8} = {result}");
                        break;
                    }
                case 0x1D2:
                    {
                        // signed operation
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X4} = {result}");
                        break;
                    }
                case 0x1D3:
                    {
                        // signed operation
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} < {rightOp:X4} = {result}");
                        break;
                    }
                case 0x1D4:
                    {
                        // signed operation
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16();
                        int result = leftOp < rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} < {rightOp:X4} = {result}");
                        break;
                    }
            }
        }

        public static void CompareLessThanOrEqual(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x6C:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp <= rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} <= {rightOp:X4} = {result}");
                        break;
                    }
            }
        }
        public static void CompareGreaterThanOrEqual(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x213:
                    {
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16();
                        int result = leftOp >= rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} >= {rightOp:X4} = {result}");
                        break;
                    }
            }
        }

        public static void CompareGreaterThan(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x70:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} > {rightOp:X4} = {result}");
                        break;
                    }
                case 0x71:
                    {
                        ih.Stack.Seek(-2);
                        ushort rightOp = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} > {rightOp:X4} = {result}");
                        break;
                    }
                case 0x72:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        ushort leftOp = ih.Stack.PeekU16();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} > {rightOp:X8} = {result}");
                        break;
                    }
                case 0x73:
                    {
                        ih.Stack.Seek(-4);
                        uint rightOp = ih.Stack.PeekU32();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} > {rightOp:X8} = {result}");
                        break;
                    }
                case 0x1FD:
                    {
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-4);
                        uint leftOp = ih.Stack.PeekU32();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X8} > {rightOp:X4} = {result}");
                        break;
                    }
                case 0x1FE:
                    {
                        ih.Stack.Seek(-2);
                        short rightOp = ih.Stack.PeekI16();
                        ih.Stack.Seek(-2);
                        short leftOp = ih.Stack.PeekI16();
                        int result = leftOp > rightOp ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {leftOp:X4} > {rightOp:X4} = {result}");
                        break;
                    }
            }
        }

        public static void CompareZero(Interpreter ih) 
        {
            switch (ih.Opcode)
            {

                case 0x8A:
                case 0x2A1:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        int result = (op == 0) ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {op:X4} == 0 = {result}");
                        break;
                    }
                case 0x8B:
                case 0x2A2:
                    {
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        int result = (op == 0) ? 1 : 0;
                        ih.Stack.WriteU16((byte)result);
                        ih.ActiveStep.AddDescription($"Compare {op:X4} == 0 = {result}");
                        break;
                    }
            }
        }

        public static void Call(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0xB5:
                    {
                        // pushes stack virtual base and current sp to stack top
                        // might be used to mark start of call, for writing stack params
                        int currentSp = ih.Stack.Position;
                        ih.VirtualStackTop -= 4;
                        ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                        ih.Stack.WriteI32(ih.VirtualStackBase);

                        ih.VirtualStackTop -= 4;
                        ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                        ih.Stack.WriteI32(currentSp);
                        
                        ih.Stack.Position = currentSp;
                        ih.ActiveStep.AddDescription($"Control Flow: preserving current sp 0x{ih.Stack.Position:X8} and virtual base 0x{ih.VirtualStackBase:X8}");
                        break;
                    }
                case 0xB6:
                case 0xB8:
                    {
                        // this seems more call-like, does not preserve sp
                        int currentSp = ih.Stack.Position;

                        if (ih.Opcode == 0xB8) 
                        {
                            // 0xB6 and 0xB8 differ only where 0xB8 also preserves virtual stack base and sp
                            ih.VirtualStackTop -= 4;
                            ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                            ih.Stack.WriteI32(ih.VirtualStackBase); // store virtual base

                            ih.VirtualStackTop -= 4;
                            ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                            ih.Stack.WriteI32(currentSp); // store sp
                        }

                        ih.VirtualStackTop -= 4;
                        ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                        ih.Stack.WriteI32(ih.Script.Position + 1); // push pc+1, this is the fn return address

                        // set the new "virtual absolute stack base" at current sp
                        ih.VirtualStackBase = currentSp;

                        // restore sp, now both virtual base and current sp are identical
                        ih.Stack.Position = currentSp;

                        // relocate program counter
                        byte functionIndex = ih.Script.PeekU8();
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position + 1);
                        // Disassembler.Step.SetJumpDisplacement(ih, ih.Script.PeekU8()); // need to actually get the fn address, then find the difference to get the displacement
                        Function func = ih.Context.Functions[functionIndex];
                        ih.Script.Position = func.EntryPoint;

                        // add extra stack space requested by function
                        int additionalStackSpace = ih.Script.ReadU16();
                        
                        additionalStackSpace *= 2; // no idea why

                        ih.Stack.Seek(additionalStackSpace);

                        // relocate text io ptr
                        ih.TextIOPointer = ih.Stack.Position;

                        ih.ActiveStep.AddDescription($"Call: New EP: 0x{func.EntryPoint:X4} -> {func.Name}");
                        break;
                    }
            }
        }

        public static bool SetEndOfFunction(Interpreter ih)
        {
            // returns false to signal script end
            switch (ih.Opcode)
            {
                case 0xB9:
                    {
                        ih.ActiveStep.AddDescription($"Pushing current SP to prepare for return parameters");
                        if (ih.VirtualStackTop >= ih.HardStackTop) 
                        {
                            ih.ActiveStep.AddDescription($"Already at base of call stack, script will be halted");
                            return false;
                        }
                        int currentSp = ih.Stack.Position;

                        // push current sp to stack top
                        // this marks the "end" sp,
                        // anything else pushed after this insn (and before retn) is a return parameter
                        ih.VirtualStackTop -= 2;
                        ih.Stack.Seek(ih.VirtualStackTop, System.IO.SeekOrigin.Begin);
                        ih.Stack.WriteU16((ushort)currentSp);

                        // restore sp
                        ih.Stack.Position = currentSp;

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unhandled return opcode: 0x{ih.Opcode:X4}");
                    }
            }
            return true;
        }


        public static void Return(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0xBA:
                    {
                        int stackPointerBeforeReturn = ih.Stack.Position;

                        ih.Stack.Position = ih.VirtualStackTop;

                        int functionEndStackSnapshot = ih.Stack.ReadU16();
                        int pcToRestore = ih.Stack.ReadI32();
                        int spToRestore = ih.Stack.ReadI32();
                        int virtualStackBaseToRestore = ih.Stack.ReadI32();

                        // "pop" (return unused stack space), equivalent to += 14
                        ih.VirtualStackTop = ih.Stack.Position;

                        // restore previous virtual stack base
                        ih.VirtualStackBase = virtualStackBaseToRestore;

                        // restore previous sp
                        ih.Stack.Position = spToRestore;

                        // restore previous text io buf
                        ih.TextIOPointer = spToRestore;

                        // restore previous pc
                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);
                        ih.Script.Position = pcToRestore;



                        // go back to the pre-return sp to read the return value
                        // then write it back after restoring the post-return sp
                        int sizeOfReturnValue = stackPointerBeforeReturn - functionEndStackSnapshot;
                        if (sizeOfReturnValue == 2)
                        {
                            // return WORD
                            ih.Stack.Position = stackPointerBeforeReturn - 2;
                            ushort val = ih.Stack.ReadU16();
                            ih.Stack.Position = spToRestore;
                            ih.Stack.WriteU16(val);
                            ih.ActiveStep.AddDescription($"Returning with result {val:X4}");
                        }
                        else if (sizeOfReturnValue == 4)
                        {
                            // return DWORD
                            ih.Stack.Position = stackPointerBeforeReturn - 4;
                            uint val = ih.Stack.ReadU32();
                            ih.Stack.Position = spToRestore;
                            ih.Stack.WriteU32(val);
                            ih.ActiveStep.AddDescription($"Returning with result {val:X8}");
                        }
                        else
                        {
                            throw new Exception($"Failed to return as the parameter size is not 2 or 4. Received: {sizeOfReturnValue}");
                        }
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unhandled return opcode: 0x{ih.Opcode:X4}");
                    }
            }
        }
        public static bool SetEndOfFunctionAndReturn(Interpreter ih)
        {
            // returns false to signal script end
            switch (ih.Opcode)
            {
                case 0xBB:
                    {
                        if (ih.VirtualStackTop >= ih.HardStackTop)
                        {
                            ih.ActiveStep.AddDescription($"Already at base of call stack, script will be halted");
                            return false;
                        }

                        int pcBeforeReturn = ih.Script.Position;

                        Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);
                        ih.Stack.Position = ih.VirtualStackTop;

                        ih.Script.Position = ih.Stack.ReadI32(); // restore previous pc
                        ih.TextIOPointer = ih.Stack.ReadI32(); // restore sp and text ptr, can't write to sp yet
                        ih.VirtualStackBase = ih.Stack.ReadI32(); // restore stack base

                        // since we have popped 3x i32 values, return the top stack space
                        ih.VirtualStackTop = ih.Stack.Position;
                        
                        ih.Stack.Position = ih.TextIOPointer; // able to discard old sp for good

                        /*
                        // this might have been an old hack; c32s does not do this
                        Console.WriteLine($"pcbeforeReturn {pcBeforeReturn:X8} / current {ih.Script.Position:X8}");
                        if ((ih.Script.Position == pcBeforeReturn) || (ih.Script.PeekU8() == 0xBB))
                        {
                            ih.ActiveStep.AddDescription($"No further return possible, script will be halted");
                            return false;
                        }
                        */

                        ih.ActiveStep.AddDescription($"Setting end of function and returning");
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"Unhandled return opcode: 0x{ih.Opcode:X4}");
                    }
            }
            return true;
        }



        public static void SwitchJump(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0xB1:
                    {
                        SwitchJumpByte(ih);
                        break;
                    }
                case 0xB2:
                    {
                        SwitchJumpWord(ih);
                        break;
                    }
            }
        }

        public static void SwitchJumpByte(Interpreter ih)
        {
            // largely 1 byte ops
            int currentPc = ih.Script.Position;
            int tableRelativeOffset = ih.Script.ReadU8();
            int tableAbsoluteOffset = currentPc + tableRelativeOffset;

            // raw value here describes size of table (2*entries), without including default case
            int caseEntries = ih.Script.ReadU8() / 2; 

            // disasm: skip the table, will be shown as data bytes
            Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);

            string pos = $"pc {currentPc:X8} rel {tableRelativeOffset:X8} abs: {tableAbsoluteOffset:X8}";

            ih.Stack.Seek(-2);
            byte caseCompareSource = (byte)(ih.Stack.PeekU16() & 0xFF); // actually a byte

            // seek to table base
            ih.Script.Seek(tableAbsoluteOffset, System.IO.SeekOrigin.Begin);

            // fetch all possible case paths, since this might be required for disassembler
            List<Tuple<byte, int>> branches = new List<Tuple<byte, int>>();
            for (int index = 0; index < caseEntries; index++)
            {
                byte condition = ih.Script.ReadU8();
                int offset = ih.Script.ReadU8();
                branches.Add(new Tuple<byte, int>(condition, offset));
                //Console.WriteLine($"Branch: {condition:X2} offset {offset:X2}");
            }
            int defaultBranch = ih.Script.ReadU8();
            //Console.WriteLine($"Default: {defaultBranch:X2}");


            // find the case that matches our condition on the stack
            var foundBranch = branches.FirstOrDefault(x => x.Item1 == caseCompareSource);

            // if there is no matching branch, use default case
            int matchingOffset = foundBranch is null ? defaultBranch : foundBranch.Item2;
            matchingOffset += currentPc;

            //Console.WriteLine($"Resolved path: {matchingOffset:X8}");
            // jump to correct destination
            ih.Script.Seek(matchingOffset, System.IO.SeekOrigin.Begin);

            ih.ActiveStep.AddDescription($"Switch table comparing with 0x{caseCompareSource:X2}, Source: {caseCompareSource:X2}");
        }
        public static void SwitchJumpWord(Interpreter ih)
        {
            // largely 2byte ops
            int currentPc = ih.Script.Position;
            int tableRelativeOffset = ih.Script.ReadU16();
            int tableAbsoluteOffset = currentPc + tableRelativeOffset;
            int caseEntries = ih.Script.ReadU16();

            string pos = $"pc {currentPc:X8} rel {tableRelativeOffset:X8} abs: {tableAbsoluteOffset:X8}";
            // table will contain..
            // size:byte
            // rows of [value to compare]:u16 , [relative offset]:u16
            // [default case]: u16

            // disasm: skip the table, will be shown as data bytes
            Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);

            ih.Stack.Seek(-2);
            ushort caseCompareSource = ih.Stack.PeekU16();

            // seek to table base
            ih.Script.Seek(tableAbsoluteOffset, System.IO.SeekOrigin.Begin);

            // fetch all possible case paths, since this might be required for disassembler
            List<Tuple<ushort, int>> branches = new List<Tuple<ushort, int>>();
            for (int index = 0; index < caseEntries; index++) 
            {
                ushort condition = ih.Script.ReadU16();
                int offset = ih.Script.ReadU16();
                branches.Add(new Tuple<ushort, int>(condition, offset));
                //Console.WriteLine($"Branch: {condition:X4} offset {offset:X4}");
            }
            int defaultBranch = ih.Script.ReadU16();
            //Console.WriteLine($"Default: {defaultBranch:X4}");

            // find the case that matches our condition on the stack
            var foundBranch = branches.FirstOrDefault(x => x.Item1 == caseCompareSource);

            // if there is no matching branch, use default case
            int matchingOffset = foundBranch is null ? defaultBranch : foundBranch.Item2;
            matchingOffset += currentPc;

            //Console.WriteLine($"Resolved path: {matchingOffset:X8}");
            // jump to correct destination
            ih.Script.Seek(matchingOffset, System.IO.SeekOrigin.Begin);

            ih.ActiveStep.AddDescription($"Switch table comparing with 0x{caseCompareSource:X4}, Source: {caseCompareSource:X4}");
        }

        /*
        // switch before rewrite

        public static void SwitchJumpWord(Interpreter ih)
        {
            // largely 2byte ops
            int currentPc = ih.Script.Position;
            int tableRelativeOffset = ih.Script.ReadU16();
            int tablePcAbsoluteOffset = currentPc + tableRelativeOffset;
            int caseEntries = ih.Script.ReadU16();

            // disasm: skip the table, will be shown as data bytes
            Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);

            ih.Stack.Seek(-2);
            ushort compareDesired = ih.Stack.PeekU16();

            int caseIndex = 0;
            while (caseIndex < caseEntries)
            {
                ih.Script.Position = tablePcAbsoluteOffset + 2 * caseIndex;
                ushort compareSource = ih.Script.PeekU16();
                // ih.ActiveStep.AddDescription($"0x{compareSource:X4} == 0x{compareDesired:X4} = {compareSource == compareDesired}");

                if (compareSource == compareDesired)
                {
                    break;
                }
                else
                {
                    caseIndex += 2;
                }
            }

            // can read pc (not peek) as pc will be restored afterwards
            ih.Script.Position = tablePcAbsoluteOffset + 2 * caseIndex;
            int tableCompareTarget = ih.Script.ReadU16();
            int delta = ih.Script.ReadU16();

            int newPc = currentPc + delta;
            ih.Script.Position = newPc;

            ih.ActiveStep.AddDescription($"Switch table comparing with 0x{compareDesired:X4}, Counter/Max: {caseIndex}/{caseEntries}, delta: for 0x{tableCompareTarget:X4}: 0x{delta:X4}");
        }
        
        public static void SwitchJumpByte(Interpreter ih)
        {
            // largely 1 byte ops
            int currentPc = ih.Script.Position;
            int tableRelativeOffset = ih.Script.ReadU8();
            int tablePcAbsoluteOffset = currentPc + tableRelativeOffset;
            int loopMaximum = ih.Script.ReadU8();

            // disasm: skip the table, will be shown as data bytes
            Disassembler.Step.MarkStepPcEnd(ih, ih.Script.Position);

            ih.Stack.Seek(-2);
            byte compareDesired = (byte)(ih.Stack.PeekU16() & 0xFF); // actually a byte

            // ih.ActiveStep.AddDescription($"current pc: 0x{currentPc:X8}, offset: pc+0x{tableRelativeOffset:X2}, loopmax: 0x{loopMaximum:X2}");
            int loopCounter = 0;
            while (loopCounter < loopMaximum)
            {
                ih.Script.Position = tablePcAbsoluteOffset + loopCounter;
                byte compareSource = ih.Script.PeekU8();
                //ih.ActiveStep.AddDescription($"0x{compareSource:X2} == 0x{compareDesired:X2} = {compareSource == compareDesired}");
                if (compareSource == compareDesired)
                {
                    // loopCounter++; // first value is for comparison, second value is actual jump displace
                    // commented out as it's incremented by the read @ int tableCompareTarget = ih.Script.ReadU8();
                    break;
                }
                else
                {
                    loopCounter += 2;
                }
            }

            // can read pc (not peek) as pc will be restored afterwards
            ih.Script.Position = tablePcAbsoluteOffset + loopCounter;
            int tableCompareTarget = ih.Script.ReadU8();
            int delta = ih.Script.ReadU8();
            int newPc = currentPc + delta;
            ih.Script.Position = newPc;

            ih.ActiveStep.AddDescription($"Switch table comparing with 0x{compareDesired:X2}, Counter/Max: {loopCounter}/{loopMaximum}, delta: for 0x{tableCompareTarget:X2}: 0x{delta:X2}");

        }

         */
    }
}
