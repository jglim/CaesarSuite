using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class InterpreterMemory
    {
        // emulated pointers
        enum MemoryRegion 
        {
            Undefined = 0,
            Stack = 1,
            GlobalVariable = 2,
            Script = 3,
            TrackedObject = 4,
        }

        public static void DescribeObject(Interpreter ih, int address) 
        {
            int ptrType = (address >> 28) & 0xF;
            MemoryRegion region = (MemoryRegion)ptrType;

            if (region == MemoryRegion.GlobalVariable)
            {
                int globalVarIndex = (address >> 16) & 0xFFF;
                int globalVarByteOffset = address & 0xFFFF;
                GlobalVariable gv = ih.GlobalVariables[globalVarIndex];
                Console.WriteLine($"GV at address: {gv}");
            }
            else if (region == MemoryRegion.TrackedObject)
            {
                TrackedObject obj = GetTrackedObjectAtAddress(ih, address);
                Console.WriteLine($"TrackedObject at address: {obj}");
            }
            else if (region == MemoryRegion.Script)
            {
                Console.WriteLine($"Script bytes at address: {address:X8}");
            }
            else if (region == MemoryRegion.Stack)
            {
                Console.WriteLine($"Stack bytes at address: {address:X8}");
            }
            else 
            {
                Console.WriteLine($"No further description available");
            }
        }

        public static int CreatePointerForStack(int stackAddress)
        {
            return stackAddress | ((int)MemoryRegion.Stack << 28);
        }
        public static int CreatePointerForScript(int scriptAddress)
        {
            return scriptAddress | ((int)MemoryRegion.Script << 28);
        }

        public static int CreatePointerForTrackedObject(TrackedObject obj) 
        {
            return CreatePointerForTrackedObject(obj.ObjectIndex);
        }
        public static int CreatePointerForTrackedObject(int objIndex, int reserved = 0)
        {
            return ((int)MemoryRegion.TrackedObject << 28) |
                ((objIndex & 0xFFF) << 16) |
                (reserved & 0xFFFF);
        }
        public static int CreatePointerForGlobalVariable(int gvIndex, int rawOffset)
        {
            return ((int)MemoryRegion.GlobalVariable << 28) |
                ((gvIndex & 0xFFF) << 16) |
                (rawOffset & 0xFFFF);
        }
        public static int CreatePointerForGlobalVariable(GlobalVariable gv, int offset, bool raw = true)
        {
            int finalOffset = raw ? offset : (offset * gv.GetSizeOfType());
            return ((int)MemoryRegion.GlobalVariable << 28) |
                ((gv.Ordinal & 0xFFF) << 16) |
                (finalOffset & 0xFFFF);
        }


        public static void SetMemoryAtAddress(Interpreter ih, int address, byte[] data)
        {
            MemoryRegion region = (MemoryRegion)((address >> 28) & 0xF);
            // Console.WriteLine($"Setting {address:X8} ({region}) : {BitUtility.BytesToHex(data)}"); // too verbose

            if (region == MemoryRegion.Stack)
            {
                int stackOffset = address & 0xFFFFFFF;
                int previousPosition = ih.Stack.Position;
                ih.Stack.Position = stackOffset;
                ih.Stack.WriteBytes(data);
                ih.Stack.Position = previousPosition;
            }
            else if (region == MemoryRegion.GlobalVariable)
            {
                int globalVarIndex = (address >> 16) & 0xFFF;
                int globalVarByteOffset = address & 0xFFFF;
                GlobalVariable gv = ih.GlobalVariables[globalVarIndex];

                // break on write
                /*
                if (gv.Name == "Seed") 
                {
                    Console.WriteLine("fixme--------------------------- Writing to seed--------------------------- ");

                    Interpreter.DumpState(ih);
                    Console.ReadKey();
                }
                */

                Array.ConstrainedCopy(data, 0, gv.Buffer, globalVarByteOffset, data.Length);
            }
            else if (region == MemoryRegion.TrackedObject)
            {
                // only tracked buffers are writable
                TrackedObject obj = GetTrackedObjectAtAddress(ih, address);
                if (obj.GetType() == typeof(Buffer))
                {
                    int offset = address & 0xFFFF;
                    Buffer buf = obj as Buffer;

                    /*
                    // break on access
                    if (buf.Origin.Contains("at 14AA"))
                    {
                        Console.WriteLine($"check buf access write {BitUtility.BytesToHex(data)}");
                        //Interpreter.DumpState(ih);
                        Console.ReadKey();
                    }
                    */
                    Array.ConstrainedCopy(data, 0, buf.ContentBytes, offset, data.Length);
                }
                else
                {
                    throw new Exception($"Attempted to write into a tracked object {obj.GetType().Name} that isn't a buffer. ");
                }
            }
            else if (region == MemoryRegion.Script) 
            {
                // wtf: self-modifying script
                // crd3:bc attempts to write a new string over an existing string in the executable memory
                // not sure if it's an implementation error since there isn't a good use case for this

                if (ih.AllowSelfModifyingCode)
                {
                    int scriptOffset = address & 0xF_FF_FF_FF;
                    byte[] scriptBuffer = ih.Script.GetUnderlyingBuffer();
                    for (int i = 0; i < data.Length; i++) 
                    {
                        scriptBuffer[scriptOffset + i] = data[i];
                    }
                }
                else 
                {
                    throw new Exception($"Script attempted to modify itself by writing to {address:X8}");
                }
            }
            else
            {
                throw new Exception($"Invalid memory write to address {address:X8}");
            }
        }
        public static byte[] GetMemoryAtAddress(Interpreter ih, int address, int size)
        {
            MemoryRegion region = (MemoryRegion)((address >> 28) & 0xF);
            // Console.WriteLine($"Getting {address:X8} ({region}) : {size} bytes"); // too verbose

            if (region == MemoryRegion.Stack)
            {
                int stackOffset = address & 0xFFFFFFF;
                int previousPosition = ih.Stack.Position;
                ih.Stack.Position = stackOffset;
                byte[] result = ih.Stack.ReadBytes(size);
                ih.Stack.Position = previousPosition;
                return result;
            }
            else if (region == MemoryRegion.GlobalVariable)
            {
                int globalVarIndex = (address >> 16) & 0xFFF;
                int globalVarByteOffset = address & 0xFFFF;
                byte[] result = new byte[size];
                Array.ConstrainedCopy(ih.GlobalVariables[globalVarIndex].Buffer, globalVarByteOffset, result, 0, result.Length);
                return result;
            }
            else if (region == MemoryRegion.Script)
            {
                int scriptOffset = address & 0xFFFFFFF;
                int previousPosition = ih.Script.Position;
                ih.Script.Position = scriptOffset;
                byte[] result = ih.Script.ReadBytes(size);
                ih.Script.Position = previousPosition;
                return result;
            }
            else if (region == MemoryRegion.TrackedObject)
            {
                // only tracked buffers are readable
                TrackedObject obj = GetTrackedObjectAtAddress(ih, address);
                if (obj.GetType() == typeof(Buffer))
                {
                    int offset = address & 0xFFFF;
                    Buffer buf = obj as Buffer;

                    /*
                    // break on access
                    if (buf.Origin.Contains("ChannelReadResponse"))
                    {
                        Console.WriteLine($"check buf access read");
                        //Interpreter.DumpState(ih);
                        Console.ReadKey();
                    }
                    */

                    byte[] result = new byte[size];
                    Array.ConstrainedCopy(buf.ContentBytes, offset, result, 0, size);
                    return result;
                }
                else 
                {
                    throw new Exception($"Attempted to write into a tracked object {obj.GetType().Name} that isn't a buffer. ");
                }
            }
            else
            {
                throw new Exception($"Invalid memory read at address {address:X8}");
            }
        }
        public static string GetCStringAtAddress(Interpreter ih, int address, int lengthLimit = 5000)
        {
            // expensive
            byte[] buffer = new byte[lengthLimit];
            int stringLength = 0;

            for (int i = 0; i < lengthLimit; i++) 
            {
                byte inChar = GetMemoryAtAddress(ih, address + i, 1)[0];
                buffer[i] = inChar;
                if (inChar == 0) 
                {
                    stringLength = i;
                    break;
                }
            }
            return Encoding.ASCII.GetString(buffer.Take(stringLength).ToArray());
        }

        // returns number of bytes written
        public static int SetCStringAtAddress(Interpreter ih, int address, string value)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(value + "\0");
            SetMemoryAtAddress(ih, address, stringBytes);
            return stringBytes.Length;
        }

        public static TrackedObject GetTrackedObjectAtAddress(Interpreter ih, int address) 
        {
            int index = (address >> 16) & 0xFFF;
            return ih.TrackedObjects[index];
        }
    }
}
