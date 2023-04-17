using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class Core
    {
        public static void NoOperation(Interpreter ih) 
        {
            switch (ih.Opcode)
            {
                case 0:
                    {
                        ih.ActiveStep.AddDescription($"No-op");
                        break;
                    }
            }
        }

        public static void StackPopDiscard(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0xB3:
                case 0xBD:
                    {
                        ih.Stack.Seek(-2);
                        ih.ActiveStep.AddDescription($"Stack pop and discard, new sp: {ih.Stack.Position:X8}");
                        break;
                    }
                case 0xB4:
                    {
                        ih.Stack.Seek(-4);
                        ih.ActiveStep.AddDescription($"Stack pop and discard, new sp: {ih.Stack.Position:X8}");
                        break;
                    }
            }
        }

        public static void ExtendInstruction(Interpreter ih) 
        {
            ushort newInstruction = 0;

            switch (ih.Opcode) 
            {
                case 0xFD:
                    {
                        newInstruction = (ushort)(ih.Script.ReadU8() + 0x300);
                        break;
                    }
                case 0xFE:
                    {
                        newInstruction = (ushort)(ih.Script.ReadU8() + 0x200);
                        break;
                    }
                case 0xFF:
                    {
                        newInstruction = (ushort)(ih.Script.ReadU8() + 0x100);
                        break;
                    }
            }

            ih.ActiveStep.AddDescription($"EI:{newInstruction:X4}");

            // extend instruction "ends" here

            ih.Opcode = newInstruction;
            ih.FetchInstructionOnNextCycle = false;

        }

        public static void AllocateBuffer(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2DB:
                    {
                        ih.Stack.Seek(-2);
                        int allocSize = ih.Stack.PeekU16();
                        Buffer newBuffer = new Buffer(ih, $"Alloc by insn: 0x2DB at {ih.Script.Position:X4}");
                        newBuffer.ContentBytes = new byte[allocSize];

                        ih.Stack.WriteI32(newBuffer.GetPointer());
                        ih.ActiveStep.AddDescription($"Allocated buffer[{allocSize}u] at {newBuffer.GetPointer():X8}");
                        break;
                    }
            }
        }


        public static void FreeBuffer(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2DD:
                    {
                        ih.Stack.Seek(-4);
                        int pointerToObject = ih.Stack.PeekI32();
                        TrackedObject obj = InterpreterMemory.GetTrackedObjectAtAddress(ih, pointerToObject);
                        string objText = obj.ToString();
                        ih.TrackedObjects.Remove(obj.ObjectIndex);
                        ih.ActiveStep.AddDescription($"Freed buffer at {pointerToObject:X8} [{objText}]");
                        break;
                    }
            }
        }

        public static void MemoryCopy(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2BE:
                    {
                        ih.Stack.Seek(-2);
                        int copySize = ih.Stack.PeekU16();
                        ih.Stack.Seek(-4);
                        int source = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int dest = ih.Stack.PeekI32();

                        byte[] buffer = InterpreterMemory.GetMemoryAtAddress(ih, source, copySize);
                        InterpreterMemory.SetMemoryAtAddress(ih, dest, buffer);

                        // memcpy: returns void* (destination)
                        ih.Stack.WriteI32(dest);
                        ih.ActiveStep.AddDescription($"Memory copy[{copySize}u] from 0x{source:X8} to 0x{dest:X8} : {BitUtility.BytesToHex(buffer)}");
                        break;
                    }
            }
        }

        public static void Sleep(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x362:
                    {
                        ih.Stack.Seek(-4);
                        int delay = ih.Stack.PeekI32();

                        const bool sleepEnabled = true;
                        if (sleepEnabled) 
                        {
                            System.Threading.Thread.Sleep(delay);
                        }

                        ih.ActiveStep.AddDescription($"Sleep: {delay}u milliseconds. Sleep Enabled: {sleepEnabled}");
                        break;
                    }
            }
        }

        public static void SetMessageToPresent(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3C8:
                    {
                        /*
                        int currentSp = ih.Stack.Position;
                        ih.Stack.Seek(-8);
                        ih.ActiveStep.AddDescription($"dbg: {BitUtility.BytesToHex(ih.Stack.ReadBytes(8))}");
                        */

                        ih.Stack.Seek(-2);
                        ushort unk = ih.Stack.PeekU16();

                        ih.Stack.Seek(-2);
                        ushort contentSize = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int byteBufferPtr = ih.Stack.PeekI32();

                        byte[] dbgMem = InterpreterMemory.GetMemoryAtAddress(ih, byteBufferPtr, contentSize);

                        // InterpreterMemory.DescribeObject(ih, byteBufferPtr);
                        ih.ActiveStep.AddDescription($"SetMessageToPresent: Unknown (byte): 0x{unk:X4}, Content: [{dbgMem.Length}u]: {BitUtility.BytesToHex(dbgMem)}");
                        break;
                    }
            }
        }

        public static void GetTesterID(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3C5:
                    {
                        ih.Stack.Seek(-2);
                        int mode = ih.Stack.PeekU16();

                        ushort testerId = 1;
                        ih.Stack.WriteU16(testerId);
                        ih.ActiveStep.AddDescription($"GetTesterID: in 0x{mode:X4}, out {testerId}u");
                        break;
                    }
            }
        }
        public static void GetCardID(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3C4:
                    {
                        ih.Stack.Seek(-4);
                        int cardId = 1;
                        ih.Stack.WriteI32(cardId);
                        ih.ActiveStep.AddDescription($"GetCardID: in 0x{cardId:X8}");
                        break;
                    }
            }
        }


    }
}
