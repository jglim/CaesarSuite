using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class ChannelIo
    {
        public static void DebugLine(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2E6:
                    {
                        ih.Stack.Seek(-6);
                        int logLevel = ih.Stack.ReadU16();
                        int stringPointer = ih.Stack.ReadI32();
                        string val = InterpreterMemory.GetCStringAtAddress(ih, stringPointer);
                        ih.Stack.Seek(-6);

                        ConsoleColor backColor = Console.BackgroundColor;
                        ConsoleColor foreColor = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        ih.ActiveStep.AddDescription($"ChannelDebugLine level {logLevel} : `{val.Replace("\n", " ")}`");

                        // Console.WriteLine($"Channel: L:{logLevel} : {val}"); // future: disable
                        ih.HostChannel.DebugLine(logLevel, val);

                        ih.ChannelLog.AppendLine($"CH[{logLevel}] {val}");
                        Console.BackgroundColor = backColor;
                        Console.ForegroundColor = foreColor;

                        break;
                    }
            }
        }
        public static void SetErrorWithText(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3DC:
                    {
                        ih.Stack.Seek(-6);
                        int errorLevel = ih.Stack.ReadU16();
                        int stringPointer = ih.Stack.ReadI32();
                        string val = InterpreterMemory.GetCStringAtAddress(ih, stringPointer);
                        ih.Stack.Seek(-6);

                        ConsoleColor backColor = Console.BackgroundColor;
                        ConsoleColor foreColor = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Black;
                        ih.ActiveStep.AddDescription($"MLError L{errorLevel} : `{val.Replace("\n", " ")}`");

                        Console.WriteLine($"MLError: L:{errorLevel} : {val}"); // future: disable
                        ih.HostChannel.ErrorLine(errorLevel, val);

                        ih.ChannelLog.AppendLine($"MLError[{errorLevel}] {val}");
                        Console.BackgroundColor = backColor;
                        Console.ForegroundColor = foreColor;

                        break;
                    }
            }
        }

        public static void CollectionResponseCreate(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x31C:
                    {
                        ChannelReadResponse crr = new ChannelReadResponse(ih);
                        int newObjPointer = crr.GetPointer();
                        ih.ActiveStep.AddDescription($"Created read response: {crr}, ptr: 0x{newObjPointer:X8}");
                        ih.Stack.WriteI32(newObjPointer);
                        break;
                    }
            }
        }

        public static void CollectionMessageCreate(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x318:
                    {
                        ih.Stack.Seek(-18);
                        ushort LRequestlength = ih.Stack.ReadU16(); // 18 -> 16
                        ushort LRequestcontextstart = ih.Stack.ReadU16(); // 16 -> 14
                        ushort LRequestcontextlength = ih.Stack.ReadU16(); // 14 -> 12
                        byte LRequesttype = (byte)ih.Stack.ReadU16(); // 12 -> 10
                        int LRequestmessage = ih.Stack.ReadI32(); // 10 -> 6
                        byte LGECUAddress = (byte)ih.Stack.ReadU16(); // 6 -> 4
                        byte LGECUFlags = (byte)ih.Stack.ReadU16(); // 4 -> 2
                        byte unkFlagStartsAs1 = (byte)ih.Stack.ReadU16(); // 2 -> 0

                        ChannelRequest cr = new ChannelRequest(ih)
                        {
                            LRequestContextStart = LRequestcontextstart,
                            LRequestContextLength = LRequestcontextlength,
                            LRequestType = LRequesttype,
                            LGECUAddress = LGECUAddress,
                            LGECUFlags = LGECUFlags,
                            UnkFlagStartsAs1 = unkFlagStartsAs1,
                            LRequestMessage = InterpreterMemory.GetMemoryAtAddress(ih, LRequestmessage, LRequestlength)
                        };
                        int newObjPointer = cr.GetPointer();
                        ih.ActiveStep.AddDescription($"Created request: {cr}, ptr: 0x{newObjPointer:X8}");

                        ih.Stack.Seek(-18);
                        ih.Stack.WriteI32(newObjPointer);

                        break;
                    }
            }
        }

        public static void FreeRequest(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x319:
                case 0x31D:
                    {
                        ih.Stack.Seek(-4);
                        int pointer = ih.Stack.PeekI32();
                        TrackedObject.Free(ih, pointer);
                        ih.ActiveStep.AddDescription($"Channel: freed tracked object at 0x{pointer:X8}");
                        break;
                    }
            }
        }
        public static void SetCommunicationParameter(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2EF:
                    {
                        ih.Stack.Seek(-2);
                        int lastParam = ih.Stack.PeekU16();

                        ih.Stack.Seek(-2);
                        int valueType = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int comRawValue = ih.Stack.PeekI32();

                        ih.Stack.Seek(-4);
                        int paramNamePointer = ih.Stack.PeekI32();

                        string comParamName = InterpreterMemory.GetCStringAtAddress(ih, paramNamePointer);

                        int valueSize = 0;
                        switch (valueType) 
                        {
                            case 0:
                                valueSize = 1; // exists
                                break;
                            case 1:
                                valueSize = 2; // most common, probably 16bit
                                break;
                            case 2:
                                valueSize = 4; // assumption, haven't seen this around
                                break;
                        }

                        byte[] comValueBytes = InterpreterMemory.GetMemoryAtAddress(ih, comRawValue, valueSize);
                        int comValue = 0;
                        for (int i = 0; i < valueSize; i++) 
                        {
                            comValue |= (comValueBytes[i] << (i * 8));
                        }

                        // ih.ActiveStep.AddDescription($"{BitUtility.BytesToHex(InterpreterMemory.GetMemoryAtAddress(ih, comRawValue, 20))}"); // 20 to view size
                        ih.ActiveStep.AddDescription($"Channel SetCommunicationParameter: {comParamName} : 0x{comValue:X8} ({comValue}), {valueType:X4}, {lastParam:X4}");
                        
                        bool success = ih.HostChannel.SetCommunicationParameter(comParamName, comValue);
                        
                        ih.Stack.WriteI16(success ? (short)1 : (short)0);

                        break;
                    }
            }
        }


        public static void GetCommunicationParameter(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2F0:
                    {
                        // fixme: hack
                        ih.Stack.Seek(-4);
                        int resultBuffer = ih.Stack.PeekI32();

                        ih.Stack.Seek(-4);
                        int paramNamePointer = ih.Stack.PeekI32();

                        string comParamName = InterpreterMemory.GetCStringAtAddress(ih, paramNamePointer);
                        bool found = ih.HostChannel.GetCommunicationParameter(comParamName, out int comParamVal);

                        // probably 16 bit, bigendian? fixme
                        byte[] comParamBytes = new byte[] { 0, 0 };
                        comParamBytes[0] = (byte)((comParamVal >> 8) & 0xFF);
                        comParamBytes[1] = (byte)(comParamVal & 0xFF);
                        
                        InterpreterMemory.SetMemoryAtAddress(ih, resultBuffer, comParamBytes);
                        ih.ActiveStep.AddDescription($"Channel GetCommunicationParameter: {comParamName}, found? {found}, result {comParamVal} (0x{comParamVal:X}) : ptr {paramNamePointer:X4}");

                        ih.Stack.WriteI16(found ? (short)1 : (short)0);
                        break;
                    }
            }
        }

        public static void CollectionMessageSendRequest(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2EB:
                    {
                        ih.Stack.Seek(-4);
                        int requestPointer = ih.Stack.PeekI32();
                        ChannelRequest req = InterpreterMemory.GetTrackedObjectAtAddress(ih, requestPointer) as ChannelRequest;
                        
                        byte[] message = req.LRequestMessage
                            .Skip(req.LRequestContextStart)
                            .Take(req.LRequestContextLength)
                            .ToArray();

                        bool requestSentSuccessfully = ih.HostChannel.CollectionMessageSend(req, message);

                        ih.CommunicationLog.AppendLine($">> {BitUtility.BytesToHex(message, true)}");
                        ih.ChannelLog.AppendLine($">> {BitUtility.BytesToHex(message, true)}");

                        ih.Stack.WriteI16(requestSentSuccessfully ? (short)1 : (short)0);
                        ih.ActiveStep.AddDescription($"Channel SendCollectionMessage: {req}, result successful: {requestSentSuccessfully}");
                        break;
                    }
            }
        }

        public static void CollectMessageStart(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2EC:
                    {
                        // our network architecture is blocking for isotp, so this isn't necessary
                        // our "send" behaves more like a "transact" so when it returns, a message is guaranteed
                        ih.ActiveStep.AddDescription($"CCCollectMessageStart");
                        break;
                    }
            }
        }

        public static void CollectMessageRun(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2ED:
                    {
                        ih.Stack.Seek(-6);
                        int collectHandle = ih.Stack.PeekI32();
                        ChannelReadResponse response = InterpreterMemory.GetTrackedObjectAtAddress(ih, collectHandle) as ChannelReadResponse;

                        ushort errorCode = ih.HostChannel.CollectMessageRun(response) ? (ushort)1 : (ushort)0; // 1:success 0:error

                        byte[] message = response.Content.ContentBytes
                            .Skip(response.ContextStart)
                            .Take(response.ContextLength)
                            .ToArray();

                        ih.CommunicationLog.AppendLine($"<< {BitUtility.BytesToHex(message, true)}");
                        ih.ChannelLog.AppendLine($"<< {BitUtility.BytesToHex(message, true)}");

                        ih.Stack.WriteU16(errorCode);
                        ih.ActiveStep.AddDescription($"CollectMessageRun, handle: 0x{collectHandle:X8}, response: {errorCode}");
                        break;
                    }
            }
        }

        public static void CollectMessageWriteToPointers(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x31B:
                    {
                        ih.Stack.Seek(-36);

                        ChannelReadResponse response = InterpreterMemory.GetTrackedObjectAtAddress(ih, ih.Stack.ReadI32()) as ChannelReadResponse;
                        int pcLResponseControl = ih.Stack.ReadI32();
                        int pwLResponseContextLength = ih.Stack.ReadI32();
                        int pwLResponseContextStart = ih.Stack.ReadI32();
                        int pcLResponseSourceAddress = ih.Stack.ReadI32();
                        int pcLResponseStatus = ih.Stack.ReadI32();
                        int pcLResponseType = ih.Stack.ReadI32();
                        int pwLResponseLength = ih.Stack.ReadI32();
                        int pLResponseMessage = ih.Stack.ReadI32();

                        ih.Stack.Seek(-36);

                        InterpreterMemory.SetMemoryAtAddress(ih, pcLResponseControl, new byte[] { (byte)(response.ResponseControl & 0xFF) });
                        InterpreterMemory.SetMemoryAtAddress(ih, pwLResponseContextLength, BitConverter.GetBytes((ushort)(response.ContextLength & 0xFFFF)));
                        InterpreterMemory.SetMemoryAtAddress(ih, pwLResponseContextStart, BitConverter.GetBytes((ushort)(response.ContextStart & 0xFFFF)));
                        InterpreterMemory.SetMemoryAtAddress(ih, pcLResponseSourceAddress, new byte[] { (byte)(response.SourceAddress & 0xFF) });
                        InterpreterMemory.SetMemoryAtAddress(ih, pcLResponseStatus, new byte[] { (byte)(response.ResponseStatus & 0xFF) });
                        InterpreterMemory.SetMemoryAtAddress(ih, pcLResponseType, new byte[] { (byte)(response.ResponseType & 0xFF) });
                        InterpreterMemory.SetMemoryAtAddress(ih, pwLResponseLength, BitConverter.GetBytes((byte)(response.Content.ContentBytes.Length & 0xFFFF)));

                        InterpreterMemory.SetMemoryAtAddress(ih, pLResponseMessage, BitConverter.GetBytes(response.Content.GetPointer()));

                        ih.ActiveStep.AddDescription($"CollectMessageWriteToPointers: 1: {response}, 2: {pcLResponseControl:X8}, 3: {pwLResponseContextLength:X8}, 4: {pwLResponseContextStart:X8}, 5: {pcLResponseSourceAddress:X8}, 6: {pcLResponseStatus:X8}, 7: {pcLResponseType:X8}, 8: {pwLResponseLength:X8}, pLResponseMessage: {pLResponseMessage:X8}");

                        break;
                    }
            }
        }
        public static void CollectMessageEnd(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2EE:
                    {
                        ih.ActiveStep.AddDescription($"CCCollectMessageEnd");
                        break;
                    }
            }
        }

        public static void RaiseChannelError(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2E2:
                    {
                        ih.Stack.Seek(-2);
                        int errorCode = ih.Stack.PeekI32();
                        ih.ActiveStep.AddDescription($"Raising channel error (code {errorCode})");
                        ih.HostChannel.RaiseError(errorCode);
                        break;
                    }
            }
        }

        public static void ResetChannelError(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2E3:
                    {
                        ih.ActiveStep.AddDescription($"Reset channel error: doing nothing");
                        ih.HostChannel.ClearError();
                        break;
                    }
            }
        }
        public static void ChannelErrorIsSet(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x2E4:
                    {
                        // write error state
                        // this could be a boolean, or could behave like a "GetLastError"
                        short errorState = 0;
                        errorState = (short)(ih.HostChannel.ErrorConditionActive() ? 1 : 0);
                        ih.Stack.WriteI16(errorState);
                        ih.ActiveStep.AddDescription($"ChannelErrorIsSet: returning {errorState}");
                        break;
                    }
            }
        }

    }
}
