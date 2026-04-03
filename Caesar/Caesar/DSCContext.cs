using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Caesar
{
    public class DSCContext
    {
        public DSCContext(byte[] dscContainerBytes, Action<string> logCallback = null) 
        {
            Action<string> log = logCallback ?? ((msg) => Console.WriteLine(msg));
            const int fnTableEntrySize = 50;
            using (BinaryReader reader = new BinaryReader(new MemoryStream(dscContainerBytes, 0, dscContainerBytes.Length, true, true)))
            {
                reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                int fnTableOffset = reader.ReadInt32(); // @ 0x10, originally i16
                int numberOfFunctions = reader.ReadInt16(); // @ 0x14
                int dscOffsetA = reader.ReadInt32(); // @ 0x16, originally i16
                int caesarHash = reader.ReadInt16(); // @ 0x1A, size is u32?

                int idk_field_1c = reader.ReadInt16(); // ?? @ 1C, padding

                int globalVarAllocSize = reader.ReadInt16(); // @ 1E

                int idk_field_20 = reader.ReadInt16(); // ?? @ 20, padding
                int globalVariablesBufferPtr = reader.ReadInt32(); // ?? @ 22
                int globalVariablesCount = reader.ReadInt16(); // ?? @ 26

                int globalVariablesIdk1 = reader.ReadInt32(); // ?? @ 28
                int globalVariablesIdk2 = reader.ReadInt16(); // ?? @ 2C

                int globalVariablesPreinitBufferPtr = reader.ReadInt32(); // ?? @ 2E
                int globalVariablesBytesToRead = reader.ReadInt16(); // ?? @ 32

                byte[] globalVarByteBuffer = new byte[globalVarAllocSize];

                log($"{nameof(dscOffsetA)} : {dscOffsetA} (0x{dscOffsetA:X})");
                log($"{nameof(caesarHash)} : {caesarHash} (0x{caesarHash:X})");
                log($"{nameof(globalVarAllocSize)} : {globalVarAllocSize} (0x{globalVarAllocSize:X})");

                log($"{nameof(globalVariablesBufferPtr)} : {globalVariablesBufferPtr} (0x{globalVariablesBufferPtr:X})");
                log($"{nameof(globalVariablesCount)} : {globalVariablesCount} (0x{globalVariablesCount:X})");

                log($"{nameof(globalVariablesIdk1)} : {globalVariablesIdk1} (0x{globalVariablesIdk1:X})");
                log($"{nameof(globalVariablesIdk2)} : {globalVariablesIdk2} (0x{globalVariablesIdk2:X})");

                log($"{nameof(globalVariablesPreinitBufferPtr)} : {globalVariablesPreinitBufferPtr} (0x{globalVariablesPreinitBufferPtr:X})");
                log($"{nameof(globalVariablesBytesToRead)} : {globalVariablesBytesToRead} (0x{globalVariablesBytesToRead:X})");

                // assemble global vars: MIGlobalVarBuild (parent: MIInterpreterRun)
                int gvBytesRemaining = globalVariablesBytesToRead;
                reader.BaseStream.Seek(globalVariablesPreinitBufferPtr, SeekOrigin.Begin);
                while (gvBytesRemaining > 0)
                {
                    int gvAddress = reader.ReadInt16();
                    int gvSize = reader.ReadByte();
                    byte[] gvData = reader.ReadBytes(gvSize);
                    log($"GV Fill: 0x{gvAddress:X} ({gvSize} bytes) : {BitUtility.BytesToHex(gvData)}");
                    Buffer.BlockCopy(gvData, 0, globalVarByteBuffer, gvAddress, gvSize);

                    gvBytesRemaining -= gvSize;
                    gvBytesRemaining -= 3;
                }

                if (gvBytesRemaining != 0) 
                {
                    throw new Exception("Global variable preinit has leftover data in the read cursor");
                }

                // CreateGlobalVar
                for (int gvIndex = 0; gvIndex < globalVariablesCount; gvIndex++)
                {
                    /*
                     guesses:
                    base
                    1 -> char
                    2 -> word
                    3 -> dword


                    derived
                    1 -> native
                    2 -> array
                    3 -> pointer

                     */
                    reader.BaseStream.Seek(globalVariablesBufferPtr + (gvIndex * 12), SeekOrigin.Begin);
                    int varName = reader.ReadInt32();
                    DSCBasicType baseType = (DSCBasicType)reader.ReadInt16();
                    DSCDerivedType derivedType = (DSCDerivedType)reader.ReadInt16();
                    int arraySize = reader.ReadInt16();
                    int positionInGlobalBuffer = reader.ReadInt16();
                    reader.BaseStream.Seek(varName, SeekOrigin.Begin);
                    string varNameResolved = CaesarReader.ReadStringFromBinaryReader(reader, CaesarReader.DefaultEncoding);

                    int dataSizeInBytes = GetDscTypeSize((int)baseType, (int)derivedType);
                    if (derivedType == DSCDerivedType.Array) 
                    {
                        dataSizeInBytes *= arraySize;
                    }
                    byte[] varBytes = new byte[dataSizeInBytes];
                    Buffer.BlockCopy(globalVarByteBuffer, positionInGlobalBuffer, varBytes, 0, dataSizeInBytes);

                    log($"\nVar: {baseType}/{derivedType} [{arraySize}] @ {positionInGlobalBuffer} : {varNameResolved}");
                    log($"{BitUtility.BytesToHex(varBytes)}");
                    // actual insertion into global var list is in MIGlobalVarCallback, stored in interpreter's ->GlobalVarList
                }

                // Collect function info first pass
                var functions = new List<(string Name, int Id, int EP, int InCount, int InOff, int OutCount, int OutOff)>();
                for (int fnIndex = 0; fnIndex < numberOfFunctions; fnIndex++)
                {
                    long fnBaseAddress = fnTableEntrySize * fnIndex + fnTableOffset;
                    reader.BaseStream.Seek(fnBaseAddress, SeekOrigin.Begin);

                    int fnIdentifier = reader.ReadInt16(); // @ 0
                    int fnNameOffset = reader.ReadInt32(); // @ 2
                    int fnEntryPoint = reader.ReadInt32(); // @ 6

                    reader.BaseStream.Seek(fnBaseAddress + 38, SeekOrigin.Begin);
                    int inputParamOffset = reader.ReadInt32(); // @ 38
                    int inputParamCount = reader.ReadInt16(); // @ 42

                    int outputParamOffset = reader.ReadInt32(); // @ 44
                    int outputParamCount = reader.ReadInt16(); // @ 48

                    reader.BaseStream.Seek(fnNameOffset, SeekOrigin.Begin);
                    string fnName = CaesarReader.ReadStringFromBinaryReader(reader);

                    log($"Fn: {fnName} Ordinal: {fnIdentifier} EP: 0x{fnEntryPoint:X}, InParam: {inputParamCount} @ 0x{inputParamOffset:X}, OutParam: {outputParamCount} @ 0x{outputParamOffset}");
                    functions.Add((fnName, fnIdentifier, fnEntryPoint, inputParamCount, inputParamOffset, outputParamCount, outputParamOffset));
                }

                // Export raw DSC bytecode region and per-function hex dumps
                int bytecodeStart = dscOffsetA;
                int bytecodeEnd = globalVariablesPreinitBufferPtr;
                if (bytecodeEnd > bytecodeStart && bytecodeEnd <= dscContainerBytes.Length)
                {
                    log($"\n=== DSC BYTECODE REGION: 0x{bytecodeStart:X} - 0x{bytecodeEnd:X} ({bytecodeEnd - bytecodeStart} bytes) ===");

                    // Sort functions by EP for sequential analysis
                    var sortedFns = functions.Where(f => f.EP >= bytecodeStart && f.EP < bytecodeEnd)
                                             .OrderBy(f => f.EP).ToList();

                    foreach (var fn in sortedFns)
                    {
                        if (fn.EP < 0 || fn.EP >= dscContainerBytes.Length) continue;

                        // Read stack height (int16 at EP)
                        reader.BaseStream.Seek(fn.EP, SeekOrigin.Begin);
                        int stackHeight = reader.ReadInt16();

                        // Determine function size (distance to next function or max 256 bytes)
                        int fnIdx = sortedFns.IndexOf(fn);
                        int nextEP = (fnIdx + 1 < sortedFns.Count) ? sortedFns[fnIdx + 1].EP : fn.EP + 256;
                        int dumpSize = Math.Min(nextEP - fn.EP, 256);
                        dumpSize = Math.Min(dumpSize, dscContainerBytes.Length - fn.EP);

                        log($"\n--- {fn.Name} @ EP 0x{fn.EP:X} (stack={stackHeight}) ---");
                        // Hex dump with offset markers
                        for (int off = 0; off < dumpSize; off += 16)
                        {
                            int lineLen = Math.Min(16, dumpSize - off);
                            var hex = new StringBuilder();
                            var ascii = new StringBuilder();
                            for (int b = 0; b < lineLen; b++)
                            {
                                byte val = dscContainerBytes[fn.EP + off + b];
                                hex.AppendFormat("{0:X2} ", val);
                                ascii.Append(val >= 0x20 && val <= 0x7E ? (char)val : '.');
                            }
                            log($"  {fn.EP + off:X4}: {hex.ToString().PadRight(48)} |{ascii}|");
                        }
                    }
                }
            }
        }

        public enum DSCBasicType 
        {
            Undefined,
            Char,
            Word,
            DWord,
            Unk_1Byte,
            Unk_2Byte,
            Unk_4Byte,
            Unk_4Byte_2,
        }

        public enum DSCDerivedType 
        {
            Undefined,
            Primitive,
            Array,
            Pointer, // DWORD PTR
        }

        public static int GetDscTypeSize(int basicType, int derivedType)
        {
            // MISizeofVarDataType
            int[] typeSizes = new int[] { -1, 1, 2, 4, 1, 2, 4, 4 };
            // char, word, dword, ??, ??, ??, ??

            if (derivedType == 3)
            {
                return 4; // DWORD PTR
            }
            else if (derivedType < 3)
            {
                if ((basicType > 0) && (basicType < 8))
                {
                    return typeSizes[basicType];
                }
                else
                {
                    throw new Exception("Unrecognized DSC Type: basic type is out of bounds");
                }
            }
            else
            {
                throw new Exception("Unrecognized DSC Type: derived type is out of bounds");
            }
        }
    }
}
