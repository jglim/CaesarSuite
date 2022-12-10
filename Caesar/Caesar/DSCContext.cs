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
        public DSCContext(byte[] dscContainerBytes) 
        {
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

                Console.WriteLine($"{nameof(dscOffsetA)} : {dscOffsetA} (0x{dscOffsetA:X})\n");
                Console.WriteLine($"{nameof(caesarHash)} : {caesarHash} (0x{caesarHash:X})\n");
                Console.WriteLine($"{nameof(globalVarAllocSize)} : {globalVarAllocSize} (0x{globalVarAllocSize:X})\n");

                Console.WriteLine($"{nameof(globalVariablesBufferPtr)} : {globalVariablesBufferPtr} (0x{globalVariablesBufferPtr:X})");
                Console.WriteLine($"{nameof(globalVariablesCount)} : {globalVariablesCount} (0x{globalVariablesCount:X})\n");

                Console.WriteLine($"{nameof(globalVariablesIdk1)} : {globalVariablesIdk1} (0x{globalVariablesIdk1:X})");
                Console.WriteLine($"{nameof(globalVariablesIdk2)} : {globalVariablesIdk2} (0x{globalVariablesIdk2:X})\n");

                Console.WriteLine($"{nameof(globalVariablesPreinitBufferPtr)} : {globalVariablesPreinitBufferPtr} (0x{globalVariablesPreinitBufferPtr:X})");
                Console.WriteLine($"{nameof(globalVariablesBytesToRead)} : {globalVariablesBytesToRead} (0x{globalVariablesBytesToRead:X})\n");

                // assemble global vars: MIGlobalVarBuild (parent: MIInterpreterRun)
                int gvBytesRemaining = globalVariablesBytesToRead;
                reader.BaseStream.Seek(globalVariablesPreinitBufferPtr, SeekOrigin.Begin);
                while (gvBytesRemaining > 0)
                {
                    int gvAddress = reader.ReadInt16();
                    int gvSize = reader.ReadByte();
                    byte[] gvData = reader.ReadBytes(gvSize);
                    Console.WriteLine($"GV Fill: 0x{gvAddress:X} ({gvSize} bytes) : {BitUtility.BytesToHex(gvData)}");
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

                    Console.WriteLine($"\nVar: {baseType}/{derivedType} [{arraySize}] @ {positionInGlobalBuffer} : {varNameResolved}");
                    Console.WriteLine($"{BitUtility.BytesToHex(varBytes)}");
                    // actual insertion into global var list is in MIGlobalVarCallback, stored in interpreter's ->GlobalVarList
                }

                for (int fnIndex = 0; fnIndex < numberOfFunctions; fnIndex++)
                {
                    long fnBaseAddress = fnTableEntrySize * fnIndex + fnTableOffset;
                    reader.BaseStream.Seek(fnBaseAddress, SeekOrigin.Begin);

                    int fnIdentifier = reader.ReadInt16(); // @ 0
                    int fnNameOffset = reader.ReadInt32(); // @ 2

                    // not exactly sure if int32 is right -- the first fn's ep looks incorrect in both cases. 
                    // 16 bit would limit the filesize to ~32KB which seems unlikely

                    int fnEntryPoint = reader.ReadInt32(); // @ 6
                    //int fnEntryPoint = reader.ReadInt16(); // @ 6
                    //int fnIdkIsThisStandalone = reader.ReadInt16(); // @ 6

                    reader.BaseStream.Seek(fnBaseAddress + 38, SeekOrigin.Begin);
                    int inputParamOffset = reader.ReadInt32(); // @ 38
                    int inputParamCount = reader.ReadInt16(); // @ 42

                    int outputParamOffset = reader.ReadInt32(); // @ 44
                    int outputParamCount = reader.ReadInt16(); // @ 48

                    reader.BaseStream.Seek(fnNameOffset, SeekOrigin.Begin);
                    string fnName = CaesarReader.ReadStringFromBinaryReader(reader);

                    Console.WriteLine($"Fn: {fnName} Ordinal: {fnIdentifier} EP: 0x{fnEntryPoint:X}, InParam: {inputParamCount} @ 0x{inputParamOffset:X}, OutParam: {outputParamCount} @ 0x{outputParamOffset}");

                    // the EP points to an int16 to initialize the stack height
                    // after the EP, the raw bytecode can be directly interpreted
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
