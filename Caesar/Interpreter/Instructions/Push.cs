using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class Push
    {
        public static void PushImmediate(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x41:
                    {
                        byte val = ih.Script.ReadU8();
                        ih.Stack.WriteU16(val); // stores u8 as u16
                        ih.ActiveStep.AddDescription($"Push immediate {val:X2}");
                        break;
                    }
                case 0x42:
                    {
                        ushort val = ih.Script.ReadU16();
                        ih.Stack.WriteU16(val); // stores u8 as u16
                        ih.ActiveStep.AddDescription($"Push immediate {val:X4}");
                        break;
                    }
                case 0x43:
                    {
                        uint val = ih.Script.ReadU32();
                        ih.Stack.WriteU32(val); // stores u8 as u16
                        ih.ActiveStep.AddDescription($"Push immediate {val:X8}");
                        break;
                    }
            }
        }

        public static void PushValuePointedByStackBase(Interpreter ih)
        {
            ih.Stack.Seek(-2);
            int currentSp = ih.Stack.Position;
            int spOffset = ih.Stack.PeekI16();
            int pcOffset = ih.Script.PeekU8();

            switch (ih.Opcode)
            {
                case 0x7:
                case 0x8:
                case 0x333:
                case 0x9:
                case 0x334: 
                    {
                        pcOffset = ih.Script.ReadU8();
                        break;
                    }

                case 0xA:
                case 0xB:
                case 0x336:
                case 0xC:
                case 0x337:

                    {
                        pcOffset = ih.Script.ReadU16();
                        break;
                    }
            }

            // read from vsp, write to current sp
            switch (ih.Opcode)
            {
                case 0x7:
                case 0xA:
                    {
                        ih.Stack.Position = ih.VirtualStackBase + pcOffset + spOffset * 1;
                        byte result = ih.Stack.ReadU8();
                        ih.ActiveStep.AddDescription($"Pushing {result:X2} from vsb[pc 0x{pcOffset:X4} + stackoffset 0x{(spOffset * 1):X4}] @ 0x{ih.Stack.Position:X8}");
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(result);
                        break;
                    }

                case 0x8:
                case 0x333:
                case 0xB:
                case 0x336:
                    {
                        ih.Stack.Position = ih.VirtualStackBase + pcOffset + spOffset * 2;
                        ushort result = ih.Stack.ReadU16();
                        ih.ActiveStep.AddDescription($"Pushing {result:X4} from vsb[pc 0x{pcOffset:X4} + stackoffset 0x{(spOffset * 2):X4}] @ 0x{ih.Stack.Position:X8}");
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(result);
                        break;
                    }

                case 0x9:
                case 0x334:
                case 0xC:
                case 0x337:
                    {
                        ih.Stack.Position = ih.VirtualStackBase + pcOffset + spOffset * 4;
                        uint result = ih.Stack.ReadU32();
                        ih.ActiveStep.AddDescription($"Pushing {result:X8} from vsb[pc 0x{pcOffset:X4} + stackoffset 0x{(spOffset * 4):X4}] @ 0x{ih.Stack.Position:X8}");
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU32(result);
                        break;
                    }
            }
        }

        public static void PushFromGlobalVarVector(Interpreter ih) 
        {
            int gvIndex = 0;
            switch (ih.Opcode)
            {
                case 0x1F:
                case 0x20:
                case 0x34B:
                case 0x21:
                case 0x34C:
                    gvIndex = ih.Script.ReadU16();
                    break;

                default:
                    {
                        throw new Exception($"Unhandled push gv (vector) opcode: 0x{ih.Opcode:X8}");
                    }
            }
            GlobalVariable gv = ih.GlobalVariables[gvIndex];
            ExtendedBinaryStream reader = new ExtendedBinaryStream(gv.Buffer);

            // read array index
            ih.Stack.Seek(-2);
            int arrayIndex = ih.Stack.PeekU16();

            // multiply by type size
            // write back into stack
            switch (ih.Opcode)
            {
                // regular array index
                case 0x1F:
                    {
                        // read u8 write u16
                        reader.Seek(arrayIndex, System.IO.SeekOrigin.Begin);
                        byte val = reader.ReadU8();
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar vector {gv.Name}[{arrayIndex}] : 0x{val:X2}");
                        break;
                    }
                case 0x20:
                case 0x34B:
                    {
                        // read u16 write u16
                        reader.Seek(arrayIndex * 2, System.IO.SeekOrigin.Begin);
                        ushort val = reader.ReadU16();
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar vector {gv.Name}[{arrayIndex}] : 0x{val:X4}");
                        break;
                    }
                case 0x21:
                case 0x34C:
                    {
                        // read u32 write u32
                        reader.Seek(arrayIndex * 4, System.IO.SeekOrigin.Begin);
                        uint val = reader.ReadU32();
                        ih.Stack.WriteU32(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar vector {gv.Name}[{arrayIndex}] : 0x{val:X8}");
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unhandled push gv (vector) opcode: 0x{ih.Opcode:X8}");
                    }
            }

        }


        public static void PushFromGlobalVarDereference(Interpreter ih)
        {
            int gvIndex = 0;
            switch (ih.Opcode)
            {
                case 0x22:
                case 0x23:
                case 0x34E:
                case 0x24:
                case 0x34F:
                    gvIndex = ih.Script.ReadU8();
                    break;

                case 0x25:
                case 0x26:
                case 0x351:
                case 0x27:
                case 0x352:
                    gvIndex = ih.Script.ReadU16();
                    break;

                default:
                    {
                        throw new Exception($"Unhandled push gv (vector) opcode: 0x{ih.Opcode:X8}");
                    }
            }
            GlobalVariable gv = ih.GlobalVariables[gvIndex];
            ExtendedBinaryStream reader = new ExtendedBinaryStream(gv.Buffer);
            int gvValueAsInt = reader.ReadI32();

            // read sum offset
            ih.Stack.Seek(-2);
            int sumOffset = ih.Stack.PeekU16();


            // multiply by type size
            // write back into stack
            switch (ih.Opcode)
            {
                // pointer ops : read out the gv as a value then add the extra offset
                case 0x22:
                case 0x25:
                    {
                        sumOffset *= 1;
                        break;
                    }
                case 0x23:
                case 0x34E:
                case 0x26:
                case 0x351:
                    {
                        sumOffset *= 2;
                        break;
                    }
                case 0x24:
                case 0x34F:
                case 0x27:
                case 0x352:
                    {
                        sumOffset *= 4;
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unhandled push gv (deref) opcode: 0x{ih.Opcode:X8}");
                    }
            }

            int resultPtr = gvValueAsInt + sumOffset;
            ih.ActiveStep.AddDescription($"Attempting to read from 0x{resultPtr:X8}");
            // InterpreterMemory.DescribeObject(ih, resultPtr);

            byte[] valueAtResultPtr = new byte[] { };

            switch (ih.Opcode)
            {
                // pointer ops : read out the gv as a value then add the extra offset
                case 0x22:
                case 0x25:
                    {
                        valueAtResultPtr = InterpreterMemory.GetMemoryAtAddress(ih, resultPtr, 1);
                        reader = new ExtendedBinaryStream(valueAtResultPtr);
                        ih.Stack.WriteU16(reader.ReadU8());
                        break;
                    }
                case 0x23:
                case 0x34E:
                case 0x26:
                case 0x351:
                    {
                        valueAtResultPtr = InterpreterMemory.GetMemoryAtAddress(ih, resultPtr, 2);
                        reader = new ExtendedBinaryStream(valueAtResultPtr);
                        ih.Stack.WriteU16(reader.ReadU16());
                        break;
                    }
                case 0x24:
                case 0x34F:
                case 0x27:
                case 0x352:
                    {
                        valueAtResultPtr = InterpreterMemory.GetMemoryAtAddress(ih, resultPtr, 4);
                        reader = new ExtendedBinaryStream(valueAtResultPtr);
                        ih.Stack.WriteU32(reader.ReadU32());
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unhandled push gv (deref) opcode: 0x{ih.Opcode:X8}");
                    }
            }

            ih.ActiveStep.AddDescription($"Push [{gv.Name} as int + {sumOffset}] = 0x{resultPtr:X8}, dereferenced resultPtr: {BitUtility.BytesToHex(valueAtResultPtr)}");
            

        }

        public static void PushFromGlobalVarScalar(Interpreter ih)
        {
            int gvIndex = 0;
            switch (ih.Opcode)
            {
                case 0x19:
                case 0x1A:
                case 0x345:
                case 0x1B:
                case 0x346:
                    gvIndex = ih.Script.ReadU8();
                    break;
                    
                case 0x1C:
                case 0x1D:
                case 0x348:
                case 0x1E:
                case 0x349:
                    gvIndex = ih.Script.ReadU16();
                    break;

                default:
                    {
                        throw new Exception($"Unhandled push gv opcode: 0x{ih.Opcode:X8}");
                    }
            }



            GlobalVariable gv = ih.GlobalVariables[gvIndex];
            ExtendedBinaryStream reader = new ExtendedBinaryStream(gv.Buffer);

            switch (ih.Opcode)
            {
                case 0x19:
                case 0x1C:
                    {
                        byte val = reader.ReadU8();
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar scalar [{gv.Name}] : 0x{val:X2}");
                        break;
                    }
                case 0x1A:
                case 0x345:
                case 0x1D:
                case 0x348:
                    {
                        ushort val = reader.ReadU16();
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar scalar [{gv.Name}] : 0x{val:X4}");
                        break;
                    }
                case 0x1B:
                case 0x346:
                case 0x1E:
                case 0x349:
                    {
                        uint val = reader.ReadU32();
                        ih.Stack.WriteU32(val);
                        ih.ActiveStep.AddDescription($"Push GlobalVar scalar [{gv.Name}] : 0x{val:X8}");
                        break;
                    }
                default: 
                    {
                        throw new Exception($"Unhandled push gv opcode: 0x{ih.Opcode:X8}");
                    }
            }
        }

        public static void PushStackAbsolute(Interpreter ih)
        {
            // preserve current position
            int currentSp = ih.Stack.Position;

            // relocate to virtual stack position, add offset parameter
            ih.Stack.Position = ih.VirtualStackBase + ih.Script.ReadU8();

            string logText = $"Push stack absolute: source address 0x{ih.Stack.Position:X8}";
            switch (ih.Opcode)
            {
                case 1:
                    {
                        byte val = ih.Stack.ReadU8();
                        logText += $", value: 0x{val:X2}";
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(val);
                        break;
                    }
                case 2:
                case 0x32D:
                    {
                        ushort val = ih.Stack.ReadU16();
                        logText += $", value: 0x{val:X4}";
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(val);
                        break;
                    }
                case 3:
                case 0x32E:
                    {
                        uint val = ih.Stack.ReadU32();
                        logText += $", value: 0x{val:X8}";
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU32(val);
                        break;
                    }
            }

            ih.ActiveStep.AddDescription(logText);
        }

        public static void PushFunctionParameter(Interpreter ih)
        {
            // if you're here because of an exception, there's a good chance that the currently running fn requires fn params
            // fn params can be emulated when the interpreter is created
            switch (ih.Opcode)
            {
                case 0x13:
                    {
                        byte index = ih.Script.ReadU8();
                        int currentSp = ih.Stack.Position;
                        int newAbsoluteStackPosition = ih.VirtualStackBase - index;
                        if (newAbsoluteStackPosition < 0) 
                        {
                            throw new Exception($"Interpreter: Attempted to fetch a function parameter that does not exist (sp{newAbsoluteStackPosition})");
                        }
                        ih.Stack.Position = newAbsoluteStackPosition;
                        ushort val = ih.Stack.ReadU8();
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(val); // reads a byte but writes a word
                        ih.ActiveStep.AddDescription($"Pushing function parameter [{index}] : 0x{val:X2}");
                        break;
                    }
                case 0x14:
                    {
                        byte index = ih.Script.ReadU8();
                        int currentSp = ih.Stack.Position;
                        int newAbsoluteStackPosition = ih.VirtualStackBase - index;
                        if (newAbsoluteStackPosition < 0)
                        {
                            throw new Exception($"Interpreter: Attempted to fetch a function parameter that does not exist (sp{newAbsoluteStackPosition})");
                        }
                        ih.Stack.Position = newAbsoluteStackPosition;
                        ushort val = ih.Stack.ReadU16();
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU16(val);
                        ih.ActiveStep.AddDescription($"Pushing function parameter [{index}] : 0x{val:X4}");
                        break;
                    }
                case 0x15:
                    {
                        byte index = ih.Script.ReadU8();
                        int currentSp = ih.Stack.Position;
                        int newAbsoluteStackPosition = ih.VirtualStackBase - index;
                        if (newAbsoluteStackPosition < 0)
                        {
                            throw new Exception($"Interpreter: Attempted to fetch a function parameter that does not exist (sp{newAbsoluteStackPosition})");
                        }
                        ih.Stack.Position = newAbsoluteStackPosition;
                        uint val = ih.Stack.ReadU32();
                        ih.Stack.Position = currentSp;
                        ih.Stack.WriteU32(val);
                        ih.ActiveStep.AddDescription($"Pushing function parameter [{index}] : 0x{val:X8}");
                        break;
                    }
            }
        }

        public static void PushScriptOffset(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x10E:
                    {
                        int offset = ih.Script.PeekU16();
                        int newFileOffset = ih.Script.Position + offset;
                        ih.Script.Seek(2);
                        ih.Stack.WriteI32(InterpreterMemory.CreatePointerForScript(newFileOffset));
                        ih.ActiveStep.AddDescription($"Push script offset: {newFileOffset:X8}");
                        break;
                    }
            }
        }
        public static void PushZero(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0xBE:
                    {
                        ih.Stack.WriteU16(0);
                        ih.ActiveStep.AddDescription($"Push zero");
                        break;
                    }
            }
        }
    }
}
