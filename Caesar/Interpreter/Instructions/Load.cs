using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class Load
    {
        public static void LoadA(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x83:
                    {
                        LoadMode1(ih);
                        break;
                    }
                case 0xA3:
                    {
                        LoadMode2(ih);
                        break;
                    }
            }
        }
        public static void LoadMode1(Interpreter ih)
        {
            ushort loadMode = ih.Script.ReadU8();

            ih.ActiveStep.AddDescription($"Load address (1), load mode is 0x{loadMode:X4}");

            switch (loadMode) 
            {
                case 1:
                case 2:
                case 3:
                    {
                        // potentially problematic
                        int newPointer = InterpreterMemory.CreatePointerForStack(ih.VirtualStackBase + ih.Script.ReadU8());
                        ih.Stack.WriteI32(newPointer);
                        ih.ActiveStep.AddDescription($"Creating pointer for stack: 0x{newPointer:X8}");
                        break;
                    }
                case 0x1F:
                    {
                        int gvIndex = ih.Script.ReadU16();
                        ih.Stack.Seek(-2);
                        int arrayIndex = ih.Stack.PeekU16();
                        int pointer = InterpreterMemory.CreatePointerForGlobalVariable(gvIndex, arrayIndex);
                        ih.Stack.WriteI32(pointer);
                        ih.ActiveStep.AddDescription($"Creating pointer for `{ih.GlobalVariables[gvIndex].Name}` ({gvIndex}) at offset {arrayIndex}, Pointer: 0x{pointer:X8}");
                        break;
                    }
                case 7:
                case 0x332:
                    {
                        ih.Stack.Seek(-2);
                        int stackOffset = ih.Stack.PeekU16();
                        int pcOffset = ih.Script.ReadU8();
                        int newSp = ih.VirtualStackBase + pcOffset + stackOffset;

                        int pointer = InterpreterMemory.CreatePointerForStack(newSp);
                        ih.Stack.WriteI32(pointer);
                        ih.ActiveStep.AddDescription($"Creating pointer for vsp[pc:0x{pcOffset:X2} + stackoffset:0x{stackOffset:X4}], Pointer: {pointer:X8}");
                        break;
                    }
                case 0xD:
                case 0x338:
                    {
                        int pcOffset = ih.Script.ReadU8();
                        ih.Stack.Seek(-2);
                        ushort resultAddition = ih.Stack.PeekU16();

                        int newSp = ih.VirtualStackBase + pcOffset;
                        
                        // read value at newsp, then restoring sp
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Position = newSp;
                        uint readout = ih.Stack.PeekU32();
                        ih.Stack.Position = currentSp;

                        readout += resultAddition;

                        ih.Stack.WriteU32(readout);
                        ih.ActiveStep.AddDescription($"Loading value (dword) at vsp[pc:0x{pcOffset:X2}], then adding 0x{resultAddition:X4} = {readout:X8}");
                        break;
                    }
                case 0x22:
                case 0x34D:
                    {
                        // this was rewritten, potentially broken 1F, 7, 332
                        int gvIndex = ih.Script.ReadU8();
                        ih.Stack.Seek(-2);
                        int resultAddition = ih.Stack.PeekU16();
                        ExtendedBinaryStream reader = new ExtendedBinaryStream(ih.GlobalVariables[gvIndex].Buffer);
                        // access gv buffer, then read out value as int
                        int readout = reader.ReadI32() + resultAddition;

                        ih.Stack.WriteI32(readout);
                        ih.ActiveStep.AddDescription($"Loading value at `{ih.GlobalVariables[gvIndex].Name}` ({gvIndex}), +0x{resultAddition:X4}, Result: {readout:X8}");
                        break;
                    }
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x341:
                case 0x342:
                case 0x343:
                    {
                        int offsetMultiplier = 1;
                        if ((loadMode == 0x17) || (loadMode == 0x342)) 
                        {
                            offsetMultiplier = 2;
                        }
                        else if ((loadMode == 0x18) || (loadMode == 0x343))
                        {
                            offsetMultiplier = 4;
                        }

                        ih.Stack.Seek(-2);
                        int offset = ih.Stack.PeekI16() * offsetMultiplier;

                        int currentSp = ih.Stack.Position;
                        int vsbOffset = ih.Script.ReadU8();
                        int readoutStackPosition = ih.VirtualStackBase - vsbOffset;
                        ih.Stack.Position = readoutStackPosition;
                        int readout = ih.Stack.ReadI32(); // supposed to be u32
                        readout += offset;

                        ih.Stack.Position = currentSp; // restore
                        ih.Stack.WriteI32(readout);

                        ih.ActiveStep.AddDescription($"Loading value at vsb[-0x{vsbOffset:X4}] as dword, + {offset:X4} ({readout:X8}) Result: {readout:X8}");
                        // might be a bit shaky, used in ki211:Unlock
                        break;
                    }
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x344:
                case 0x345:
                case 0x346:
                    {
                        // store pointer to gv
                        int gvIndex = ih.Script.ReadU8();
                        int gvPointer = InterpreterMemory.CreatePointerForGlobalVariable(gvIndex, 0);
                        ih.Stack.WriteI32(gvPointer);
                        ih.ActiveStep.AddDescription($"Writing pointer of {ih.GlobalVariables[gvIndex].Name} to stack: 0x{gvPointer:X8}");
                        break;
                    }
                default:
                    {
                        throw new Exception($"unhandled 0x83 type: {loadMode:X4}");
                    }
            }
        }


        public static void LoadMode2(Interpreter ih)
        {
            ushort loadMode = ih.Script.ReadU8();

            // extend mode
            switch (loadMode)
            {
                case 0xFC:
                    {
                        loadMode = (ushort)(ih.Script.ReadU8() + 0x400);
                        break;
                    }
                case 0xFD:
                    {
                        loadMode = (ushort)(ih.Script.ReadU8() + 0x300);
                        break;
                    }
                case 0xFE:
                    {
                        loadMode = (ushort)(ih.Script.ReadU8() + 0x200);
                        break;
                    }
                case 0xFF:
                    {
                        loadMode = (ushort)(ih.Script.ReadU8() + 0x100);
                        break;
                    }
            }

            // load address based on loadmode
            switch (loadMode)
            {
                case 0xD:
                case 0xE:
                case 0xF:
                case 0x104:
                case 0x338:
                case 0x339:
                case 0x33A:
                    {
                        // load sp with virtualabs pcoffset8
                        byte offset = ih.Script.ReadU8();
                        int newAddress = ih.VirtualStackBase + offset;
                        ih.ActiveStep.AddDescription($"Load address, stack absolute(virtual) base+0x{offset:X2}");
                        ih.Stack.WriteI32(InterpreterMemory.CreatePointerForStack(newAddress));
                        break;
                    }
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x107:
                case 0x341:
                case 0x342:
                case 0x343:
                    {
                        // load sp with virtualabs -pcoffset8
                        byte offset = ih.Script.ReadU8();
                        int newAddress = ih.VirtualStackBase - offset;
                        ih.ActiveStep.AddDescription($"Load address, stack absolute(virtual) base-0x{offset:X2}");
                        ih.Stack.WriteI32(InterpreterMemory.CreatePointerForStack(newAddress));
                        break;
                    }
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x10B:
                case 0x34D:
                case 0x34E:
                case 0x34F:
                    {
                        // load sp with gv scalar pcoffset8
                        byte gvIndex = ih.Script.ReadU8();
                        ih.ActiveStep.AddDescription($"Load address, globalvar: {ih.Context.GlobalVariables[gvIndex].Name}");
                        ih.Stack.WriteI32(InterpreterMemory.CreatePointerForGlobalVariable(gvIndex, 0));
                        break;
                    }
                case 0x25:
                case 0x26:
                case 0x27:
                case 0x10C:
                case 0x350:
                case 0x351:
                case 0x352:
                    {
                        // load sp with gv scalar pcoffset16
                        ushort gvIndex = ih.Script.ReadU16();
                        ih.ActiveStep.AddDescription($"Load address, globalvar: {ih.Context.GlobalVariables[gvIndex].Name}");
                        ih.Stack.WriteI32(InterpreterMemory.CreatePointerForGlobalVariable(gvIndex, 0));
                        break;
                    }
                default: 
                    {
                        throw new Exception($"Unknown address load (2) mode 0x{loadMode:X4}");
                    }
            }



        }

    }
}
