using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class ArithmeticLogic
    {

        public static void Add(Interpreter ih)
        {
            // 0x10F..0x123
            switch (ih.Opcode)
            {
                case 0x47:
                    {
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        ih.Stack.Seek(-4);
                        uint src = ih.Stack.PeekU32();
                        uint preExecutionValue = src;
                        unchecked { src += op; }
                        ih.Stack.WriteU32(src);
                        ih.ActiveStep.AddDescription($"Add: 0x{preExecutionValue:X8} += 0x{op:X8} = 0x{src:X8}");
                        break;
                    }
                case 0x112:
                    {
                        // potentially wrong signed types
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        ih.Stack.Seek(-2);
                        int src = ih.Stack.PeekI16();
                        int preExecutionValue = src;
                        unchecked { src += (int)op; }
                        ih.Stack.WriteI32(src);
                        ih.ActiveStep.AddDescription($"Add: 0x{preExecutionValue:X8} += 0x{op:X8} = 0x{src:X4}");
                        break;
                    }
                case 0x115:
                case 0x117:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        ushort preExecutionValue = src;
                        unchecked { src += op; }
                        ih.Stack.WriteU16(src);
                        ih.ActiveStep.AddDescription($"Add: 0x{preExecutionValue:X4} += 0x{op:X4} = 0x{src:X4}");
                        break;
                    }
                case 0x116:
                    {
                        ih.Stack.Seek(-2);
                        short op = ih.Stack.PeekI16();
                        ih.Stack.Seek(-4);
                        int src = ih.Stack.PeekI32();
                        int preExecutionValue = src;
                        unchecked { src += op; }
                        ih.Stack.WriteI32(src);
                        ih.ActiveStep.AddDescription($"Add: 0x{preExecutionValue:X8} += 0x{op:X4} = 0x{src:X8}");
                        break;
                    }
                default:
                    {
                        throw new Exception("Unhandled add opcode");
                    }
            }
        }

        public static void Multiply(Interpreter ih)
        {
            // 0x139...0x14D
            switch (ih.Opcode)
            {
                case 0x4F:
                    {
                        ih.Stack.Seek(-4);
                        uint op = ih.Stack.PeekU32();
                        ih.Stack.Seek(-4);
                        uint src = ih.Stack.PeekU32();
                        uint preExecutionValue = src;
                        unchecked { src *= op; }
                        ih.Stack.WriteU32(src);
                        ih.ActiveStep.AddDescription($"Multiply: 0x{preExecutionValue:X8} *= 0x{op:X8} = 0x{src:X8}");
                        break;
                    }
                case 0x141:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        uint preExecutionValue = src;
                        unchecked { src *= op; }
                        ih.Stack.WriteU16(src);
                        ih.ActiveStep.AddDescription($"Multiply: 0x{preExecutionValue:X4} *= 0x{op:X4} = 0x{src:X4}");
                        break;
                    }
                default: 
                    {
                        throw new Exception("Unhandled multiply opcode");
                    }
            }
        }


        public static void Subtract(Interpreter ih)
        {
            // 0x124..0x138
            switch (ih.Opcode)
            {
                case 0x12A:
                case 0x12C:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        uint preExecutionValue = src;
                        unchecked { src -= op; }
                        ih.Stack.WriteU16(src);
                        ih.ActiveStep.AddDescription($"0x{preExecutionValue:X4} -= 0x{op:X4} = 0x{src:X4}");
                        break;
                    }
                default:
                    {
                        throw new Exception("Unhandled subtract opcode");
                    }
            }
        }


        public static void BitwiseAnd(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x250:
                    {
                        ih.Stack.Seek(-2);
                        ushort andMask = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort source = ih.Stack.PeekU16();
                        ushort result = source;
                        source &= andMask;
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"0x{source:X4} &= 0x{andMask:X4} = 0x{result:X4}");
                        break;
                    }
                case 0x251:
                    {
                        ih.Stack.Seek(-2);
                        int andMask = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        int source = ih.Stack.PeekI32();
                        int result = source & andMask;
                        ih.Stack.WriteI32(result);
                        ih.ActiveStep.AddDescription($"0x{source:X8} &= 0x{andMask:X4} = 0x{result:X8}");
                        break;
                    }
                case 0x252:
                    {
                        ih.Stack.Seek(-2);
                        ushort andMask = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort source = ih.Stack.PeekU16();
                        ushort result = source;
                        result &= andMask;
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"0x{source:X4} &= 0x{andMask:X4} = 0x{result:X4}");
                        break;
                    }
                case 0x24E:
                case 0x256:
                    {
                        ih.Stack.Seek(-4);
                        int andMask = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int source = ih.Stack.PeekI32();
                        int result = source & andMask;
                        ih.Stack.WriteI32(result);
                        ih.ActiveStep.AddDescription($"0x{source:X8} &= 0x{andMask:X8} = 0x{result:X8}");
                        break;
                    }
            }
        }

        public static void BitwiseOr(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x9B:
                case 0x26B: // semms identical
                    {
                        ih.Stack.Seek(-4);
                        int op = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int source = ih.Stack.PeekI32();
                        int result = source | op;
                        ih.Stack.WriteI32(result);
                        ih.ActiveStep.AddDescription($"0x{source:X8} |= 0x{op:X8} = 0x{result:X8}");
                        break;
                    }
                case 0x266:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint source = ih.Stack.PeekU32();
                        uint result = source | op;
                        ih.Stack.WriteU32(result);
                        ih.ActiveStep.AddDescription($"0x{source:X8} |= 0x{op:X4} = 0x{result:X8}");
                        break;
                    }
                case 0x267:
                    {
                        ih.Stack.Seek(-2);
                        ushort op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort source = ih.Stack.PeekU16();
                        ushort result = source;
                        result |= op;
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"0x{source:X4} |= 0x{op:X4} = 0x{result:X4}");
                        break;
                    }
            }
        }
        public static void BitwiseXor(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x9F:
                    {
                        ih.Stack.Seek(-4);
                        int op = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int source = ih.Stack.PeekI32();
                        int result = source ^ op;
                        ih.Stack.WriteI32(result);
                        ih.ActiveStep.AddDescription($"0x{source:X8} ^= 0x{op:X8} = 0x{result:X8}");
                        break;
                    }
                case 0x27C:
                    {
                        ih.Stack.Seek(-2);
                        int op = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        int source = ih.Stack.PeekU16();
                        int result = source ^ op;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"0x{source:X4} ^= 0x{op:X4} = 0x{result:X4}");
                        break;
                    }
            }
        }
        public static void BitwiseInvert(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x88:
                case 0x29E:
                    {
                        ih.Stack.Seek(-2);
                        ushort val = ih.Stack.PeekU16();
                        ushort invert = (ushort)~val;
                        ih.Stack.WriteU16(invert);
                        ih.ActiveStep.AddDescription($"0x{val:X4} bitwise invert = 0x{invert:X4}");
                        break;
                    }
            }
        }
        public static void RightShift(Interpreter ih)
        {
            // shifts in unsigned ints to prevent accidental sign extension
            switch (ih.Opcode)
            {
                case 0x194:
                    {
                        ih.Stack.Seek(-2);
                        int shiftCount = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint src = ih.Stack.PeekU32();
                        uint result = src >> shiftCount;
                        ih.Stack.WriteU32(result);
                        ih.ActiveStep.AddDescription($"RShift {src:X8} >> {shiftCount} = {result:X8}");
                        break;
                    }
                case 0x195:
                    {
                        ih.Stack.Seek(-2);
                        int shiftCount = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        uint src = ih.Stack.PeekU16();
                        uint result = src >> shiftCount;
                        ih.Stack.WriteU16((ushort)result);
                        ih.ActiveStep.AddDescription($"RShift {src:X4} >> {shiftCount} = {result:X4}");
                        break;
                    }
            }
        }
        public static void LeftShift(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x178:
                case 0x17E:
                case 0x180:
                    {
                        ih.Stack.Seek(-2);
                        ushort shiftCount = ih.Stack.PeekU16();
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        ushort result = src;
                        result <<= shiftCount;
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"LShift {src:X4} << {shiftCount} = {result:X4}");
                        break;
                    }
                case 0x179:
                case 0x17F:
                case 0x181:
                    {
                        ih.Stack.Seek(-2);
                        int shiftCount = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        uint src = ih.Stack.PeekU32();
                        uint result = src << shiftCount;
                        ih.Stack.WriteU32(result);
                        ih.ActiveStep.AddDescription($"LShift {src:X8} << {shiftCount} = {result:X8}");
                        break;
                    }
            }
        }
    }
}
