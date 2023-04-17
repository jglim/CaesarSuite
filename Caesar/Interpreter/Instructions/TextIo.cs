using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class TextIo
    {
        public static void BiSprintf(Interpreter ih) 
        {
            switch (ih.Opcode) 
            {
                case 0x2D1:
                    {
                        ih.Stack.Position = ih.TextIOPointer;

                        // debug: dump for checking stack alignment
                        // ih.ActiveStep.AddDescription($"{BitUtility.BytesToHex(ih.Stack.ReadBytes(0x20))}");
                        // ih.Stack.Position = ih.TextIOPointer;

                        int destAddress = ih.Stack.ReadI32();
                        int textTemplateAddress = ih.Stack.ReadI32();
                        int substituteValueAddress = InterpreterMemory.CreatePointerForStack(ih.Stack.Position);

                        ih.ActiveStep.AddDescription($"BiSprintf: ioptr 0x{ih.TextIOPointer:X8}, dest 0x{destAddress:X8}, template: 0x{textTemplateAddress:X8} substitute: 0x{substituteValueAddress:X8}`");

                        // actually do sprintf now
                        // overwrite first 2 bytes with sprintf result (string length)
                        // behaves like a bstr
                        string template = InterpreterMemory.GetCStringAtAddress(ih, textTemplateAddress);
                        string output = FormatString.InterpreterSprintf(template, ih);
                        ih.ActiveStep.AddDescription($"BiSprintf: dest 0x{destAddress:X8}, template: 0x{textTemplateAddress:X8} `{template}`, substitute: 0x{substituteValueAddress:X8}, result: `{output}`");

                        // commit new string to dest
                        int bytesWritten = InterpreterMemory.SetCStringAtAddress(ih, destAddress, output);

                        // move destAddress so that subsequent writes do not trample over existing data
                        ih.Stack.Position = ih.TextIOPointer;
                        ih.Stack.WriteI32(destAddress + bytesWritten);

                        // restore sp
                        ih.Stack.Position = ih.TextIOPointer + 2;
                        break;
                    }
            }
        }

        public static void Strcmp(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2C3:
                    {
                        // this implementation has different behavior from c strcmp for nonzero values
                        ih.Stack.Seek(-4);
                        int compare1 = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int compare2 = ih.Stack.PeekI32();

                        string compare1A = InterpreterMemory.GetCStringAtAddress(ih, compare1);
                        string compare2A = InterpreterMemory.GetCStringAtAddress(ih, compare2);

                        bool equal = compare1A == compare2A;
                        ih.ActiveStep.AddDescription($"String compare (equal: {equal}) 1@{compare1:X8}: '{compare1A}', 2@{compare2:X8}: '{compare2A}'`");

                        ih.Stack.WriteI16(equal ? (short)0 : (short)1);
                        break;
                    }
            }
        }
        public static void Unk1(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3D8:
                    {
                        ih.Stack.Position = ih.TextIOPointer;

                        int stringTemplateOffset = ih.Stack.ReadI32();
                        int writeDestination = ih.Stack.ReadI32();

                        string template = InterpreterMemory.GetCStringAtAddress(ih, stringTemplateOffset);
                        ih.ActiveStep.AddDescription($"Unk1 text io: ioptr 0x{ih.TextIOPointer:X8}, template: 0x{stringTemplateOffset:X8} ({template}), dest: 0x{writeDestination:X8}`");
                        // supposed to write a value to writeDestination after checking if it fits the string template
                        ih.Stack.Position = ih.TextIOPointer;
                        break;
                    }
            }
        }
        public static void Unk2(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2D7:
                    {
                        ih.Stack.Position = ih.TextIOPointer;

                        int stringTemplateOffset = ih.Stack.ReadI32();
                        int writeDestination = ih.Stack.ReadI32();

                        string template = InterpreterMemory.GetCStringAtAddress(ih, stringTemplateOffset);
                        byte[] testRead = InterpreterMemory.GetMemoryAtAddress(ih, writeDestination, 4);

                        ih.ActiveStep.AddDescription($"Unk2 text io: ioptr 0x{ih.TextIOPointer:X8}, template: 0x{stringTemplateOffset:X8} ({template}), dest: 0x{writeDestination:X8}`, testread {BitConverter.ToString(testRead)}");
                        ih.Stack.Position = ih.TextIOPointer;
                        break;
                    }
            }
        }

        public static void Strcpy(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2C4:
                    {
                        // og
                        ih.Stack.Seek(-4);
                        int sourceString = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int destBufferPointer = ih.Stack.PeekI32();

                        string src = InterpreterMemory.GetCStringAtAddress(ih, sourceString);

                        // on crd3:bc, a specific call attempts to modify the script content (corrupted stack?)
                        InterpreterMemory.SetMemoryAtAddress(ih, destBufferPointer, Encoding.ASCII.GetBytes(src + "\0"));

                        ih.ActiveStep.AddDescription($"Strcpy: 0x{sourceString:X8} to 0x{destBufferPointer:X8}, src: `{src}`");
                        ih.Stack.WriteI32(destBufferPointer); // write back result
                        break;
                    }
            }
        }


    }
}
