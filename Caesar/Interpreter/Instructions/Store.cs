using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class Store
    {
        public static void TwoStageStore(Interpreter ih)
        {
            ushort destType = ih.Script.ReadU8();

            // fetch from pc until destType has a value
            while (destType == 0) 
            {
                destType = ih.Script.ReadU8();
            }

            // extend instruction if required
            if (destType == 0xFD) 
            {
                destType = (ushort)(ih.Script.ReadU8() + 0x300);
            }
            else if (destType == 0xFE)
            {
                destType = (ushort)(ih.Script.ReadU8() + 0x200);
            }
            else if (destType == 0xFF)
            {
                destType = (ushort)(ih.Script.ReadU8() + 0x100);
            }

            // stage 1 : set destination pointer
            byte castType = 0;
            int storePointer = 0;

            ih.ActiveStep.AddDescription($"Store stage1 (OP: {ih.Opcode:X4}) type: {destType:X2}");

            switch (destType) 
            {
                case 1:
                case 2:
                case 3:
                case 0x100: // 4
                case 0x32C:
                case 0x32D:
                case 0x32E:
                    {
                        if (destType < 4)
                        {
                            castType = (byte)destType;
                        }
                        else if (destType == 0x100) 
                        {
                            castType = 4;
                        }
                        else if (destType >= 0x32C)
                        {
                            castType = (byte)(destType - 0x32C + 5);
                        }

                        storePointer = InterpreterMemory.CreatePointerForStack(ih.VirtualStackBase + ih.Script.ReadU8());
                        ih.ActiveStep.AddDescription($"Dest: stack absolute: {destType:X4} offset: {storePointer:X8}");
                        break;
                    }
                case 7:
                case 8:
                case 9:
                case 0xA: // 4
                case 0xB:
                case 0xC:
                    {
                        int pcOffset = 0;
                        if ((destType == 7) || (destType == 8) || (destType == 9))
                        {
                            pcOffset = ih.Script.ReadU8();
                        }
                        else if ((destType == 0xA) || (destType == 0xB) || (destType == 0xC))
                        {
                            pcOffset = ih.Script.ReadU16();
                        }

                        ih.Stack.Seek(-2);
                        int stackOffset = ih.Stack.PeekU16();
                        
                        if ((destType == 7) || (destType == 0xA)) 
                        {
                            castType = 1;
                        }
                        else if ((destType == 8) || (destType == 0xB))
                        {
                            stackOffset *= 2;
                            castType = 2;
                        }
                        else if ((destType == 0x9) || (destType == 0xC))
                        {
                            stackOffset *= 4;
                            castType = 3;
                        }
                        storePointer = InterpreterMemory.CreatePointerForStack(ih.VirtualStackBase + pcOffset + stackOffset);
                        ih.ActiveStep.AddDescription($"Dest: vsp[pcoffset 0x{pcOffset:X4} + stackoffset 0x{stackOffset:X4}] offset: {storePointer:X8}");
                        break;
                    }
                case 0xD:
                case 0xE:
                case 0xF:
                case 0x10:
                case 0x11:
                case 0x12:
                    {
                        /* 10, 11, 12 seem to use a 2byte pc but are identical otherwise*/

                        int pcOffset = 0;
                        if (destType <= 0xF)
                        {
                            castType = (byte)(destType - 0xD + 1);
                            pcOffset = ih.Script.ReadU8();
                        }
                        else if (destType >= 0x10) 
                        {
                            castType = (byte)(destType - 0x10 + 1);
                            pcOffset = ih.Script.ReadU16();
                        }

                        ih.Stack.Seek(-2);
                        uint stackOffset = ih.Stack.PeekU16();

                        if (castType == 2)
                        {
                            stackOffset *= 2;
                        }
                        else if (castType == 3) 
                        {
                            stackOffset *= 4;
                        }

                        // read from stackoffset
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Position = ih.VirtualStackBase + pcOffset; // *(_DWORD *)&stackBuffer_virtual[pcoffset]
                        uint readout = ih.Stack.ReadU32();
                        readout += stackOffset;
                        ih.Stack.Position = currentSp;

                        storePointer = (int)readout;
                        ih.ActiveStep.AddDescription($"Dest: *(u32*)vsb[0x{pcOffset:X}] + 0x{stackOffset:X8} offset: {storePointer:X8}");
                        break;
                    }
                case 0x16:
                case 0x17:
                case 0x18:
                    {
                        // identical to D/E/F after multiplying pc by -1
                        // this is largely copied and pasted from above
                        int pcOffset;
                        
                        castType = (byte)(destType - 0x16 + 1);
                        pcOffset = ih.Script.ReadU8();
                        pcOffset *= -1; // <----

                        ih.Stack.Seek(-2);
                        uint stackOffset = ih.Stack.PeekU16();

                        if (castType == 2)
                        {
                            stackOffset *= 2;
                        }
                        else if (castType == 3)
                        {
                            stackOffset *= 4;
                        }

                        // read from stackoffset
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Position = ih.VirtualStackBase + pcOffset; // *(_DWORD *)&stackBuffer_virtual[pcoffset]
                        uint readout = ih.Stack.ReadU32();
                        readout += stackOffset;
                        ih.Stack.Position = currentSp;

                        storePointer = (int)readout;
                        ih.ActiveStep.AddDescription($"Dest: *(u32*)vsb[-0x{pcOffset:X}] + 0x{stackOffset:X8} offset: {storePointer:X8}");
                        break;
                    }
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x108: // 4
                case 0x344:
                case 0x345:
                case 0x346:
                    {
                        // pc as u8
                        if (destType >= 0x344)
                        {
                            castType = (byte)(destType - 0x344 + 5);
                        }
                        else if (destType == 0x108)
                        {
                            castType = 4;
                        }
                        else
                        {
                            castType = (byte)(destType - 0x19 + 1); // 19->1 1A->2 1B->3
                        }
                        
                        byte globalVarIndex = ih.Script.ReadU8();
                        storePointer = InterpreterMemory.CreatePointerForGlobalVariable(globalVarIndex, 0);
                        ih.ActiveStep.AddDescription($"Dest: globalvar scalar [{ih.GlobalVariables[globalVarIndex].Name}], type: {destType:X4} offset: {storePointer:X8}");
                        break;
                    }
                case 0x1C:
                case 0x1D:
                case 0x1E:
                case 0x109: // 4
                case 0x347:
                case 0x348:
                case 0x349:
                    {
                        // pc as u16
                        if (destType >= 0x347)
                        {
                            castType = (byte)(destType - 0x347 + 5); // 5,6,7
                        }
                        else if (destType == 0x109) 
                        {
                            castType = 4;
                        }
                        else
                        {
                            castType = (byte)(destType - 0x1C + 1); // 1,2,3
                        }

                        ushort globalVarIndex = ih.Script.ReadU16();
                        storePointer = InterpreterMemory.CreatePointerForGlobalVariable(globalVarIndex, 0);
                        ih.ActiveStep.AddDescription($"Dest: globalvar scalar [{ih.GlobalVariables[globalVarIndex].Name}], type: {destType:X4} offset: {storePointer:X8}");
                        break;
                    }


                case 0x1F:
                case 0x20:
                case 0x21:
                    {
                        // gv, pc readout as u16
                        castType = (byte)(destType - 0x1F + 1);

                        int globalVarIndex = ih.Script.ReadU16();
                        ih.Stack.Seek(-2);
                        int arrayIndex = ih.Stack.PeekU16();

                        if (castType == 2)
                        {
                            arrayIndex *= 2;
                        }
                        else if (castType == 3) 
                        {
                            arrayIndex *= 4;
                        }

                        storePointer = InterpreterMemory.CreatePointerForGlobalVariable(globalVarIndex, arrayIndex);
                        ih.ActiveStep.AddDescription($"Dest: globalvar vector [{ih.GlobalVariables[globalVarIndex].Name}], type: {destType:X4} offset: {storePointer:X8}");
                        break;
                    }
                case 0x22:
                case 0x23:
                case 0x24:
                    {
                        // gv, pc readout as u8
                        castType = (byte)(destType - 0x22 + 1);

                        int globalVarIndex = ih.Script.ReadU8();
                        ih.Stack.Seek(-2);
                        int arrayIndex = ih.Stack.PeekU16();

                        if (castType == 2)
                        {
                            arrayIndex *= 2;
                        }
                        else if (castType == 3)
                        {
                            arrayIndex *= 4;
                        }

                        storePointer = InterpreterMemory.CreatePointerForGlobalVariable(globalVarIndex, arrayIndex);
                        ih.ActiveStep.AddDescription($"Dest: globalvar vector [{ih.GlobalVariables[globalVarIndex].Name}], type: {destType:X4} offset: {storePointer:X8}");
                        break;
                    }


                case 0x3D:
                case 0x3E:
                case 0x3F:
                    {
                        // seems like pointer dereference, this store op writes obj into a pointer
                        castType = (byte)(destType - 0x3C);
                        ih.Stack.Seek(-4);
                        storePointer = ih.Stack.PeekI32();
                        ih.ActiveStep.AddDescription($"Dest: stack after pop (4), type: {destType:X2} dereferenced ptr value: {storePointer:X8}");
                        break;
                    }
                default: 
                    {
                        throw new Exception($"Unhandled store stage1 destination: {destType:X8}");
                    }
            }


            // above is executed as one cycle in the original interpreter
            ih.CycleCount++;

            // stage2 : fetch values to write
            switch (ih.Opcode)
            {
                case 0x79:
                case 0x296: // seems identical to 0x79
                    {
                        // write unmodified (4 bytes)

                        byte[] valueToWrite = new byte[] { };
                        ih.Stack.Seek(-4);

                        if ((castType == 1) || (castType == 5))
                        {
                            valueToWrite = new byte[] { ih.Stack.PeekU8() };
                        }
                        else if ((castType == 2) || (castType == 6))
                        {
                            valueToWrite = BitConverter.GetBytes(ih.Stack.PeekU16());
                        }
                        else if ((castType == 3) || (castType == 7))
                        {
                            valueToWrite = BitConverter.GetBytes(ih.Stack.PeekU32());
                        }
                        else if (castType == 4)
                        {
                            valueToWrite = BitConverter.GetBytes((float)ih.Stack.PeekU32());
                        }
                        else
                        {
                            throw new Exception("Unhandled store conversion");
                        }
                        ih.ActiveStep.AddDescription($"Store stage2 value to write: {BitUtility.BytesToHex(valueToWrite, true)}");
                        InterpreterMemory.SetMemoryAtAddress(ih, storePointer, valueToWrite);
                        break;
                    }
                case 0x7A:
                    {
                        // add assign += (2 bytes), unsigned, see 289 for signed

                        byte[] valueToWrite = new byte[] { };
                        ih.Stack.Seek(-2);

                        if ((castType == 1) || (castType == 5))
                        {
                            byte baseValue = InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 1)[0];
                            unchecked { baseValue += ih.Stack.PeekU8(); }
                            valueToWrite = new byte[] { baseValue };
                        }
                        else if ((castType == 2) || (castType == 6))
                        {
                            ushort baseValue = BitConverter.ToUInt16(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 2), 0);
                            valueToWrite = BitConverter.GetBytes((ushort)(ih.Stack.PeekU16() + baseValue)); // also clamped, needs to be checked
                        }
                        else if ((castType == 3) || (castType == 7))
                        {
                            uint baseValue = BitConverter.ToUInt32(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 4), 0);
                            valueToWrite = BitConverter.GetBytes((ushort)(ih.Stack.PeekU16() + baseValue)); // seems to require clamping to 2byte
                        }
                        else if (castType == 4)
                        {
                            throw new Exception("probably wrong, should be a float-y value");
                            //float baseValue = BitConverter.ToSingle(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 4), 0);
                            //valueToWrite = BitConverter.GetBytes(ih.Stack.PeekI16() + baseValue);
                        }
                        else
                        {
                            throw new Exception("Unhandled store");
                        }
                        ih.ActiveStep.AddDescription($"Store stage2 value to write: {BitUtility.BytesToHex(valueToWrite, true)}");
                        InterpreterMemory.SetMemoryAtAddress(ih, storePointer, valueToWrite);
                        break;
                    }
                case 0x289:
                    {
                        // add assign += (2 bytes), signed, see 7A for unsigned

                        byte[] valueToWrite = new byte[] { };
                        ih.Stack.Seek(-2);

                        if ((castType == 1) || (castType == 5))
                        {
                            byte baseValue = InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 1)[0];
                            unchecked { baseValue += ih.Stack.PeekU8(); }
                            valueToWrite = new byte[] { baseValue };
                        }
                        else if ((castType == 2) || (castType == 6))
                        {
                            ushort baseValue = BitConverter.ToUInt16(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 2), 0);
                            valueToWrite = BitConverter.GetBytes((short)(ih.Stack.PeekI16() + baseValue)); // also clamped, needs to be checked
                        }
                        else if ((castType == 3) || (castType == 7))
                        {
                            uint baseValue = BitConverter.ToUInt32(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 4), 0);
                            valueToWrite = BitConverter.GetBytes((short)(ih.Stack.PeekI16() + baseValue)); // seems to require clamping to 2byte
                        }
                        else if (castType == 4)
                        {
                            throw new Exception("probably wrong, should be a float-y value");
                            //float baseValue = BitConverter.ToSingle(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 4), 0);
                            //valueToWrite = BitConverter.GetBytes(ih.Stack.PeekI16() + baseValue);
                        }
                        else
                        {
                            throw new Exception("Unhandled store");
                        }
                        ih.ActiveStep.AddDescription($"Store stage2 value to write: {BitUtility.BytesToHex(valueToWrite, true)}");
                        InterpreterMemory.SetMemoryAtAddress(ih, storePointer, valueToWrite);
                        break;
                    }
                case 0x28F:
                    {
                        // multiply assign *= (2 bytes), signed?

                        byte[] valueToWrite = new byte[] { };
                        ih.Stack.Seek(-2);

                        if ((castType == 1) || (castType == 5))
                        {
                            byte baseValue = InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 1)[0];
                            unchecked { baseValue *= ih.Stack.PeekU8(); }
                            valueToWrite = new byte[] { baseValue };
                        }
                        else if ((castType == 2) || (castType == 6))
                        {
                            ushort baseValue = BitConverter.ToUInt16(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 2), 0);
                            valueToWrite = BitConverter.GetBytes((ushort)(ih.Stack.PeekU16() * baseValue));
                        }
                        else if ((castType == 3) || (castType == 7))
                        {
                            uint baseValue = BitConverter.ToUInt32(InterpreterMemory.GetMemoryAtAddress(ih, storePointer, 4), 0);
                            valueToWrite = BitConverter.GetBytes((short)(ih.Stack.PeekI16() * baseValue)); // seems to require clamping to 2byte
                        }
                        else if (castType == 4)
                        {
                            throw new Exception("probably wrong, should be a float-y value");
                        }
                        else
                        {
                            throw new Exception("Unhandled store");
                        }
                        ih.ActiveStep.AddDescription($"Store stage2 value to write: {BitUtility.BytesToHex(valueToWrite, true)}");
                        InterpreterMemory.SetMemoryAtAddress(ih, storePointer, valueToWrite);
                        break;
                    }
                case 0x78:
                case 0x295:
                    {
                        // write unmodified (2 bytes)

                        byte[] valueToWrite = new byte[] { };
                        ih.Stack.Seek(-2);

                        if ((castType == 1) || (castType == 5))
                        {
                            valueToWrite = new byte[] { ih.Stack.PeekU8() };
                        }
                        else if ((castType == 2) || (castType == 6))
                        {
                            valueToWrite = BitConverter.GetBytes(ih.Stack.PeekU16());
                        }
                        else if ((castType == 3) || (castType == 7))
                        {
                            valueToWrite = BitConverter.GetBytes((int)ih.Stack.PeekI16());
                        }
                        else if (castType == 4)
                        {
                            valueToWrite = BitConverter.GetBytes((float)ih.Stack.PeekI16());
                        }
                        else
                        {
                            throw new Exception("Unhandled store conversion");
                        }
                        ih.ActiveStep.AddDescription($"Store stage2 value to write: {BitUtility.BytesToHex(valueToWrite, true)}");
                        InterpreterMemory.SetMemoryAtAddress(ih, storePointer, valueToWrite);
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unhandled store action: {ih.Opcode:X4}");
                    }
            }


        }

        public static void LoadPointerValue(Interpreter ih)
        {
            // this instruction doesn't quite fit well in any category
            switch (ih.Opcode)
            {
                case 0xD:
                case 0xE:
                case 0x339:
                case 0xF:
                case 0x33A:
                    {
                        ih.Stack.Seek(-2);
                        int addOffset = ih.Stack.PeekI16();
                        int vsbOffset = ih.VirtualStackBase + ih.Script.ReadU8();

                        // read ptr on stack (relative to vsb)
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Position = vsbOffset;
                        int ptrOnStack = ih.Stack.ReadI32();
                        ih.Stack.Position = currentSp;

                        if (ih.Opcode == 0xD)
                        {
                            byte[] valueAtPointer = InterpreterMemory.GetMemoryAtAddress(ih, ptrOnStack + addOffset * 1, 1);
                            ExtendedBinaryStream reader = new ExtendedBinaryStream(valueAtPointer);

                            // debug stuff
                            // ih.ActiveStep.AddDescription($"ptrOnStack: 0x{ptrOnStack:X8}, addOffset {addOffset} v@p {valueAtPointer[0]:X2}");
                            // InterpreterMemory.DescribeObject(ih, ptrOnStack);
                            // Console.ReadKey();

                            byte readValue = reader.ReadU8();
                            ih.Stack.WriteU16(readValue);
                            ih.ActiveStep.AddDescription($"Pushing value {readValue:X2} from *(0x{ptrOnStack:X8}+0x{addOffset * 1})");

                        }
                        else if ((ih.Opcode == 0xE) || (ih.Opcode == 0x339))
                        {
                            throw new Exception("check");
                            byte[] valueAtPointer = InterpreterMemory.GetMemoryAtAddress(ih, ptrOnStack + addOffset * 2, 2);
                            ExtendedBinaryStream reader = new ExtendedBinaryStream(valueAtPointer);
                            ushort readValue = reader.ReadU16();
                            ih.Stack.WriteU16(readValue);
                            ih.ActiveStep.AddDescription($"Pushing value {readValue:X4} from *(0x{ptrOnStack:X8}+0x{addOffset * 2})");
                        }
                        else if ((ih.Opcode == 0xF) || (ih.Opcode == 0x33A))
                        {
                            throw new Exception("check");
                            byte[] valueAtPointer = InterpreterMemory.GetMemoryAtAddress(ih, ptrOnStack + addOffset * 4, 4);
                            ExtendedBinaryStream reader = new ExtendedBinaryStream(valueAtPointer);
                            uint readValue = reader.ReadU32();
                            ih.Stack.WriteU32(readValue);
                            ih.ActiveStep.AddDescription($"Pushing value {readValue:X8} from *(0x{ptrOnStack:X8}+0x{addOffset * 4})");
                        }

                        break;
                    }
            }
        }
        public static void LoadPointerValueNegative(Interpreter ih)
        {
            // see LoadPointerValue, same thing, negative pc
            switch (ih.Opcode)
            {
                case 0x16:
                    {
                        ih.Stack.Seek(-2);
                        int addOffset = ih.Stack.PeekI16();
                        int vsbOffset = ih.VirtualStackBase - ih.Script.ReadU8(); // negative pc here

                        // read ptr on stack (relative to vsb)
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Position = vsbOffset;
                        int ptrOnStack = ih.Stack.ReadI32();
                        ih.Stack.Position = currentSp;

                        byte[] valueAtPointer = InterpreterMemory.GetMemoryAtAddress(ih, ptrOnStack + addOffset * 1, 1);
                        ExtendedBinaryStream reader = new ExtendedBinaryStream(valueAtPointer);

                        byte readValue = reader.ReadU8();
                        ih.Stack.WriteU16(readValue);
                        ih.ActiveStep.AddDescription($"Pushing value {readValue:X2} from *(0x{ptrOnStack:X8}+0x{addOffset * 1})");
                        break;
                    }
            }
        }
    }
}
