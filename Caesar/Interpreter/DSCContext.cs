using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaesarInterpreter.DSCTypes;

namespace CaesarInterpreter
{
    public class DSCContext
    {
        public byte[] PALBytes = new byte[] { };
        public List<GlobalVariable> GlobalVariables = new List<GlobalVariable>();
        public List<Function> Functions = new List<Function>();

        public void GVFillInitializedData(BinaryReader reader, int gvOffset, int gvPreinitBufferSize, byte[] gvPreinitBuffer) 
        {
            reader.BaseStream.Seek(gvOffset, SeekOrigin.Begin);
            while (gvPreinitBufferSize > 0)
            {
                int gvAddress = reader.ReadInt16();
                int gvSize = reader.ReadByte();
                byte[] gvData = reader.ReadBytes(gvSize);
                System.Buffer.BlockCopy(gvData, 0, gvPreinitBuffer, gvAddress, gvSize);

                gvPreinitBufferSize -= gvSize;
                gvPreinitBufferSize -= 3;
            }
            if (gvPreinitBufferSize != 0)
            {
                throw new Exception("Global variable preinit has leftover data in the read cursor");
            }
        }

        public void CreateFunctions(BinaryReader reader, int fnTableOffset, int numberOfFunctions)
        {
            const int fnTableEntrySize = 50; // verified

            Functions.Clear();
            // the zero-th fn element does not seem to be valid, seems to hold chunks of previously-accessed memory
            // sometimes it will look like a duplicated function entry, but it is still invalid
            Functions.Add(new Function { Name = "PlaceholderAlignment", Ordinal = 0, EntryPoint = 0 });

            for (int fnIndex = 1; fnIndex < numberOfFunctions; fnIndex++)
            {
                long fnBaseAddress = fnTableEntrySize * fnIndex + fnTableOffset;
                reader.BaseStream.Seek(fnBaseAddress, SeekOrigin.Begin);

                int fnIdentifier = reader.ReadInt16(); // @ 0
                int fnNameOffset = reader.ReadInt32(); // @ 2

                int fnEntryPoint = reader.ReadInt32(); // @ 6 // confirmed as 32-bit

                // position 18: 16-bit value, if lower 3 bits are set (0x7), fails on loading program counter 

                reader.BaseStream.Seek(fnBaseAddress + 38, SeekOrigin.Begin);
                int inputParamOffset = reader.ReadInt32(); // @ 38
                int inputParamCount = reader.ReadInt16(); // @ 42

                int outputParamOffset = reader.ReadInt32(); // @ 44
                int outputParamCount = reader.ReadInt16(); // @ 48

                reader.BaseStream.Seek(fnNameOffset, SeekOrigin.Begin);
                string fnName = CaesarReaderStandalone.ReadStringFromBinaryReader(reader);

                // not storing inputParamCount/inputParamOffset and output/xx as they are not fully parsed.
                // Console.WriteLine($"Fn: {fnName} Ordinal: {fnIdentifier} EP: 0x{fnEntryPoint:X}, InParam: {inputParamCount} @ 0x{inputParamOffset:X}, OutParam: {outputParamCount} @ 0x{outputParamOffset}");
                Functions.Add(new Function { Name = fnName, Ordinal = fnIdentifier, EntryPoint = fnEntryPoint });
                // the EP points to an int16 to initialize the stack height
                // after the EP, the raw bytecode can be directly interpreted
            }

            // Functions.ForEach(x => Console.WriteLine(x));
        }

        public void CreateGlobalVars(BinaryReader reader, int expectedGvCount, int palGvOffset, byte[] gvPreinitBuffer) 
        {
            GlobalVariables.Clear();
            for (int gvIndex = 0; gvIndex < expectedGvCount; gvIndex++)
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
                reader.BaseStream.Seek(palGvOffset + (gvIndex * 12), SeekOrigin.Begin);
                int varName = reader.ReadInt32();
                DSCBasicType baseType = (DSCBasicType)reader.ReadInt16();
                DSCDerivedType derivedType = (DSCDerivedType)reader.ReadInt16();
                int arraySize = reader.ReadInt16();
                int positionInGlobalBuffer = reader.ReadInt16();
                reader.BaseStream.Seek(varName, SeekOrigin.Begin);
                string varNameResolved = CaesarReaderStandalone.ReadStringFromBinaryReader(reader, CaesarReaderStandalone.DefaultEncoding);

                int dataSizeInBytes = GetSizeOfType(baseType, derivedType);
                if (derivedType == DSCDerivedType.Array)
                {
                    dataSizeInBytes *= arraySize;
                }
                byte[] varBytes = new byte[dataSizeInBytes];
                System.Buffer.BlockCopy(gvPreinitBuffer, positionInGlobalBuffer, varBytes, 0, dataSizeInBytes);

                // Console.WriteLine($"\nVar: {baseType}/{derivedType} [{arraySize}] @ {positionInGlobalBuffer} : {varNameResolved}");
                // Console.WriteLine($"{BitUtility.BytesToHex(varBytes)}");

                // array element count is not stored, it can be inferred from size/typesize
                GlobalVariable gv = new GlobalVariable { Name = varNameResolved, Buffer = varBytes, BaseType = baseType, DerivedType = derivedType, Ordinal = positionInGlobalBuffer };
                GlobalVariables.Add(gv);
                // actual insertion into global var list is in MIGlobalVarCallback, stored in interpreter's ->GlobalVarList
            }
            // GlobalVariables.ForEach(x => Console.WriteLine(x));
        }


        public DSCContext(byte[] dscContainerBytes)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(dscContainerBytes, 0, dscContainerBytes.Length, true, true)))
            {
                PALBytes = dscContainerBytes;

                //reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                uint magic = reader.ReadUInt32(); // expects 0x004C4150 "PAL\0"
                uint unk0 = reader.ReadUInt32(); // expects 0, version?
                int sizeOrOffset = reader.ReadInt32(); // seems to be an offset to file crc, filesize-4
                int debugFlagUnk = reader.ReadInt32(); // seems to be checked for debug stuff ( >= 0x178)

                int fnTableOffset = reader.ReadInt32(); // @ 0x10, originally i16
                int numberOfFunctions = reader.ReadInt16(); // @ 0x14
                int fnTableHashmapOffset = reader.ReadInt32(); // @ 0x16, originally i16
                int fnTableHashmapItemCount = reader.ReadInt16(); // @ 0x1A, size is u32?

                int idk_field_1c = reader.ReadInt16(); // ?? @ 1C, typically 0x1000?

                int globalVarAllocSize = reader.ReadInt16(); // @ 1E

                int idk_field_20 = reader.ReadInt16(); // ?? @ 20, padding
                int globalVariablesBufferPtr = reader.ReadInt32(); // ?? @ 22
                int globalVariablesCount = reader.ReadInt16(); // ?? @ 26

                int globalVariablesIdk1 = reader.ReadInt32(); // ?? @ 28
                int globalVariablesIdk2 = reader.ReadInt16(); // ?? @ 2C

                int gvPreinitBufferOffset = reader.ReadInt32(); // ?? @ 2E
                int gvPreinitBufferSize = reader.ReadInt16(); // ?? @ 32

                byte[] gvPreinitBuffer = new byte[globalVarAllocSize];

                /*
                Console.WriteLine($"{nameof(dscOffsetA)} : {dscOffsetA} (0x{dscOffsetA:X})");
                Console.WriteLine($"{nameof(caesarHash)} : {caesarHash} (0x{caesarHash:X})");
                Console.WriteLine($"{nameof(globalVarAllocSize)} : {globalVarAllocSize} (0x{globalVarAllocSize:X})");

                Console.WriteLine($"{nameof(globalVariablesBufferPtr)} : {globalVariablesBufferPtr} (0x{globalVariablesBufferPtr:X})");
                Console.WriteLine($"{nameof(globalVariablesCount)} : {globalVariablesCount} (0x{globalVariablesCount:X})");

                Console.WriteLine($"{nameof(globalVariablesIdk1)} : {globalVariablesIdk1} (0x{globalVariablesIdk1:X})");
                Console.WriteLine($"{nameof(globalVariablesIdk2)} : {globalVariablesIdk2} (0x{globalVariablesIdk2:X})");

                Console.WriteLine($"{nameof(gvPreinitBufferOffset)} : {gvPreinitBufferOffset} (0x{gvPreinitBufferOffset:X})");
                Console.WriteLine($"{nameof(gvPreinitBufferSize)} : {gvPreinitBufferSize} (0x{gvPreinitBufferSize:X})");
                */

                // assemble global vars: MIGlobalVarBuild (parent: MIInterpreterRun)

                GVFillInitializedData(reader, gvPreinitBufferOffset, gvPreinitBufferSize, gvPreinitBuffer);

                // CreateGlobalVar
                CreateGlobalVars(reader, globalVariablesCount, globalVariablesBufferPtr, gvPreinitBuffer);

                CreateFunctions(reader, fnTableOffset, numberOfFunctions);

            }
        }

    }
    
}
