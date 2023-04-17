using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class TypeConversion
    {
        public static void Convert(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x353:
                    {
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        uint result = src;
                        ih.Stack.WriteU32(result);
                        ih.ActiveStep.AddDescription($"Convert u16 0x{src:X4} -> u32 0x{result:X8}");
                        break;
                    }
                case 0x35E:
                    {
                        ih.Stack.Seek(-2);
                        ushort src = ih.Stack.PeekU16();
                        ushort result = (ushort)(src & 0xFF);
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"Convert u16 0x{src:X4} -> u8(2) 0x{result:X4}");
                        break;
                    }
                case 0x35F:
                    {
                        ih.Stack.Seek(-4);
                        uint src = ih.Stack.PeekU32();
                        ushort result = (ushort)(src & 0xFF);
                        ih.Stack.WriteU16(result);
                        ih.ActiveStep.AddDescription($"Convert u32 0x{src:X8} -> u8(2) 0x{result:X4}");
                        break;
                    }
                default:
                    {
                        /*
                        // unhandled conversions
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
                        */
                        throw new NotImplementedException($"Unhandled conversion opcode {ih.Opcode:X4}");
                    }
            }
        }
    }
}
