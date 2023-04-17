using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Instructions
{
    public class DSIO
    {
        public static void CreateDSIO(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x30B:
                    {
                        ih.Stack.Seek(-2);
                        ushort unk = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int namePointer = ih.Stack.PeekI32();

                        string diagServiceName = InterpreterMemory.GetCStringAtAddress(ih, namePointer);
                        ih.ActiveStep.AddDescription($"Creating DSIO, unk: {unk:X4}, DiagService: {diagServiceName}");

                        DiagServiceIO io = new DiagServiceIO(ih) { Name = diagServiceName, Unknown1 = unk };
                        ih.Stack.WriteI32(io.GetPointer());
                        break;
                    }
            }
        }
        public static void IsNegativeResponse(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3B1:
                    {
                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        bool hasError = dsio.IsNegativeResponse();

                        ih.Stack.WriteU16(hasError ? (ushort)1 : (ushort)0);
                        ih.ActiveStep.AddDescription($"DiagServiceIOIsNegativeResponse: {dsio.Name}, Error: {hasError}");
                        break;
                    }
            }
        }
        public static void DoDiagService(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x30C:
                    {
                        ih.Stack.Position = ih.TextIOPointer;
                        int dsioPointer = ih.Stack.ReadI32();
                        ushort unkB = ih.Stack.ReadU16();
                        ih.Stack.Position = ih.TextIOPointer;

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        // actually execute the prepared dsio here
                        dsio.DoDiagService();
                        ih.ActiveStep.AddDescription($"DoDiagService: {dsio.Name}, Unknown: {unkB:X4}");

                        // result/status is manually fetched by another instruction
                        break;
                    }
            }
        }
        public static void DeleteDSIO(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x30D:
                    {
                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        // nothing to do here
                        ih.ActiveStep.AddDescription($"Deleting DSIO: {dsio.Name}");
                        TrackedObject.Free(ih, dsioPointer);
                        break;
                    }
            }
        }

        public static void GetPresParamType(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x0389:
                    {
                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        int paramType = dsio.GetPrepPresType(paramIndex);

                        ih.Stack.WriteU16((ushort)paramType);
                        ih.ActiveStep.AddDescription($"GetPresParamType: {dsio.Name} with paramindex {paramIndex}u, returning type {paramType}");
                        break;
                    }
            }
        }

        public static void GetPrepParamDumpLength(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x03A5:
                    {
                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16(); // presentation index (typ. 0)

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        // fixme
                        int length = dsio.GetPrepParamDumpLength(paramIndex);
                        ih.Stack.WriteI32(length);
                        ih.ActiveStep.AddDescription($"GetPrepParamDumpLength: {dsio.Name} with paramindex {paramIndex}u, returning {length}");
                        //Console.ReadKey();
                        break;
                    }
            }
        }
        public static void GetPresParamDumpLength(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x38C:
                    {
                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16(); // pres index : 0

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        // this really needs to be a full diagservice from caesar
                        // see complaints in GetPresentationDumpValue

                        int length = dsio.GetPresParamDumpLength(paramIndex);
                        
                        ih.Stack.WriteI32(length);

                        ih.ActiveStep.AddDescription($"GetPresParamDumpLength: {dsio.Name} with paramindex {paramIndex}u, returning {length}");
                        break;
                    }
            }
        }

        public static void GetDumpPresParam(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x39C:
                    {
                        ih.Stack.Seek(-4);
                        int dumpLengthOutput = ih.Stack.PeekI32();
                        ih.Stack.Seek(-4);
                        int dumpValueOutput = ih.Stack.PeekI32();
                        ih.Stack.Seek(-2);
                        int presParamIndex = ih.Stack.PeekI16();
                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        Buffer buf = new Buffer(ih, "GetDumpPresParam placeholder");
                        buf.ContentBytes = dsio.GetDumpPresParam(presParamIndex);
                        InterpreterMemory.SetMemoryAtAddress(ih, dumpValueOutput, BitConverter.GetBytes(buf.GetPointer()));

                        // content length should not exceed i16_max
                        byte[] lengthPayload = new byte[] {
                                (byte)(buf.ContentBytes.Length & 0xFF),
                                (byte)((buf.ContentBytes.Length >> 8) & 0xFF), 
                            0, 0 };

                        InterpreterMemory.SetMemoryAtAddress(ih, dumpLengthOutput, lengthPayload);

                        ih.ActiveStep.AddDescription($"GetDumpPresParam: {dsio.Name} length out: 0x{dumpLengthOutput:X8}, val out: 0x{dumpValueOutput:X8}, paramindex: {presParamIndex}u");
                        break;
                    }
            }
        }

        public static void GetPresentationDumpValue(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x38E:
                    {
                        ih.Stack.Seek(-2);
                        int presIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();
                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        // this really needs to be a full diagservice from caesar
                        Buffer buf = new Buffer(ih, "GetPresentationDumpValue placeholder");

                        buf.ContentBytes = dsio.GetPresentationDumpValue(presIndex);

                        ih.Stack.WriteI32(buf.GetPointer());

                        ih.ActiveStep.AddDescription($"GetPresentationDumpValue: {dsio.Name} with NumPresentationParams (index?) {presIndex}u");
                        // throw an error if the service fails, else continue execution normally
                        break;
                    }
            }
        }
        public static void SetDumpPreparationParam(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3B0:
                    {
                        ih.Stack.Seek(-2);
                        int bufferSize = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int bufferPointer = ih.Stack.PeekI32();

                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;
                        byte[] dataToWrite = InterpreterMemory.GetMemoryAtAddress(ih, bufferPointer, bufferSize);

                        dsio.SetDumpPreparationParam(paramIndex, dataToWrite);

                        // ki211 writes the computed key back into a diagservice, then uses the same diagservice to send the result
                        ih.ActiveStep.AddDescription($"SetDumpPreparationParam: {dsio.Name}, parameter#{paramIndex}, buffer: {BitUtility.BytesToHex(dataToWrite)}");
                        break;
                    }
            }
        }
        public static void SetUWordPreparationParam(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3A7:
                    {
                        ih.Stack.Seek(-2);
                        ushort valueToWrite = ih.Stack.PeekU16();

                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;
                        dsio.SetUWordPreparationParam(paramIndex, valueToWrite);

                        // ki211 writes the computed key back into a diagservice, then uses the same diagservice to send the result
                        ih.ActiveStep.AddDescription($"SetUWordPreparationParam: {dsio.Name}, parameter#{paramIndex}, value: 0x{valueToWrite:X4}");
                        break;
                    }
            }
        }
        public static void SetSLongPreparationParam(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x3A9:
                    {
                        ih.Stack.Seek(-4);
                        int valueToWrite = ih.Stack.PeekI32();

                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;
                        dsio.SetSLongPreparationParam(paramIndex, valueToWrite);

                        ih.ActiveStep.AddDescription($"SetSLongPreparationParam: {dsio.Name}, parameter#{paramIndex}, value: 0x{valueToWrite:X4}");
                        break;
                    }
            }
        }

        public static void GetStringPresentationParam(Interpreter ih)
        {
            switch (ih.Opcode)
            {
                case 0x393:
                    {
                        ih.Stack.Seek(-2);
                        int paramIndex = ih.Stack.PeekU16();

                        ih.Stack.Seek(-4);
                        int dsioPointer = ih.Stack.PeekI32();

                        DiagServiceIO dsio = InterpreterMemory.GetTrackedObjectAtAddress(ih, dsioPointer) as DiagServiceIO;

                        Buffer buf = new Buffer(ih, "GetStringPresentationParam placeholder");
                        buf.ContentBytes = Encoding.ASCII.GetBytes(dsio.GetStringPresentationParam(paramIndex));

                        ih.Stack.WriteI32(buf.GetPointer());

                        ih.ActiveStep.AddDescription($"GetStringPresentationParam: {dsio.Name}, parameter#{paramIndex}");
                        break;
                    }
            }
        }

    }
}
