using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    class CaesarStructure
    {
        // offsets
        public enum StructureName
        {
            CBFHEADER = 0,
            UNK1 = 1,
            UNK2 = 2,
            UNK3 = 3,
            UNK4 = 4,
            PRESENTATION_STRUCTURE = 0x5,
            UNK6 = 6,
            UNK7 = 7,
            UNK8 = 8,
            UNK9 = 9,
            UNK10 = 0xA,
            SCALEINTERVAL_STRUCTURE = 0xB,
            UNK12 = 0xC,
            UNK13 = 0xD,
            UNK14 = 0xE,
            UNK15 = 0xF,
            FLASH_DESCRIPTION_HEADER = 0x10,
            FLASH_TABLE_STRUCTURE = 0x11,
            UNK18 = 0x12,
            UNK19 = 0x13,
            SESSION_TABLE_STRUCTURE = 0x14,
            UNK21 = 0x15,
            DATA_BLOCK_TABLE_STRUCTURE = 0x16,
            UNK23 = 0x17,
            UNK24 = 0x18,
            UNK25 = 0x19,
            UNK26 = 0x1A,
            SEGMENT_TABLE_STRUCTURE = 0x1B,
            UNK28 = 0x1C,
            CTFHEADER = 0x1D,
            LANGUAGE_TABLE = 0x1E,
            CCFHEADER = 0x1F,
            UNK32 = 0x20,
            CCFFRAGMENT = 0x21,
            UNK34 = 0x22,
            UNK35 = 0x23,
            UNK36 = 0x24,
        }

        /*
        raw values from ida:
        [
	        [2,4,4,4,4,4,4,4,4,4,4,4,4],
	        [4,4,4,2,2,4,4,4,4,4,2,2,2,4,4,4,4,4,4,4,4,4,4,1,1,2,1],
	        [6,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,2],
	        [6,4,4,2,4,4,4,4,4,4,4,4,4,2,4,4,4,4,4,4,2,2,4,4,2,1,4,4,4,4,4,4],
	        [4,4,4,4,4,4,4,2,2,2,2,1,1,1,1,1,5,1,1,1,1,4,4,4,4,4],
	        [6,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,4,4,4,4,4,4,4,4,4,4,4,1,1,1,1,1,4,4,4,2,4,4,4],
	        [2,4,4,2,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4,2,4,2,4],
	        [2,4,4,4,4,4,4,4,4,4,4,2],
	        [2,4,4,4,4,4,4,4,4],
	        [2,4,4,4],
	        [2,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,1,1],
	        [2,4,2,2,2,4,2,2,4,4,4],
	        [2,4,4,4,4,4,4,4,4,1,1],
	        [6,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,1],
	        [4,4,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4],
	        [2,4,4,4,4,4,4,4,4,4,2],
	        [2,4,4,4,4,4],
	        [6,4,4,4,4,4,4,4,4,4,4,4,2,4,4,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4],
	        [2,4,4,4,4,4,4],
	        [2,4,4],
	        [2,2,4,4,2,4,4,2,4,4,2,4,4],
	        [2,4,4,4,4,4,4,4],
	        [2,4,4],
	        [2,4,4,2,4,4,4,4,4],
	        [2,4,2,4,4,4],
	        [6,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2],
	        [6,4,4,4,4,4,4,4,4,4,4,4,4],
	        [2,4,4,4,4,4,2,4],
	        [2,4,4,4,4,4,2,4],
	        [2,4,4,4,4,4,4,2,4],
	        [2,2,2,2,2,2,2,2,4,4,0,0,0]
        ]
         */
        public static List<byte[]> CaesarTypes = new List<byte[]>();

        public static void GetOffset(StructureName name, int memberIndex) 
        {
            FillCaesarTypes();
        }

        public static byte[] GetCaesarLayout(StructureName name) 
        {
            FillCaesarTypes();
            return CaesarTypes[(int)name];
        }

        public static void FillCaesarTypes() 
        {
            if (CaesarTypes.Count != 0) 
            {
                return;
            }
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // CBFHEADER
            CaesarTypes.Add(new byte[] { 4, 4, 4, 2, 2, 4, 4, 4, 4, 4, 2, 2, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 2, 1 }); // UNK1
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2 }); // UNK2
            CaesarTypes.Add(new byte[] { 6, 4, 4, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 4, 4, 4, 4, 4, 4, 2, 2, 4, 4, 2, 1, 4, 4, 4, 4, 4, 4 }); // UNK3
            CaesarTypes.Add(new byte[] { 4, 4, 4, 4, 4, 4, 4, 2, 2, 2, 2, 1, 1, 1, 1, 1, 5, 1, 1, 1, 1, 4, 4, 4, 4, 4 }); // UNK4
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 4, 4, 4, 2, 4, 4, 4 }); // PRESENTATION_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // UNK6
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 2, 4, 2, 4 }); // UNK7
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2 }); // UNK8
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4 }); // UNK9
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4 }); // UNK10
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // SCALEINTERVAL_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 1, 1 }); // UNK12
            CaesarTypes.Add(new byte[] { 2, 4, 2, 2, 2, 4, 2, 2, 4, 4, 4 }); // UNK13
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1 }); // UNK14
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1 }); // UNK15
            CaesarTypes.Add(new byte[] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // FLASH_DESCRIPTION_HEADER
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // FLASH_TABLE_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // UNK18
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4 }); // UNK19
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2 }); // SESSION_TABLE_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4 }); // UNK21
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // DATA_BLOCK_TABLE_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4 }); // UNK23
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4 }); // UNK24
            CaesarTypes.Add(new byte[] { 2, 4, 4 }); // UNK25
            CaesarTypes.Add(new byte[] { 2, 2, 4, 4, 2, 4, 4, 2, 4, 4, 2, 4, 4 }); // UNK26
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 4 }); // SEGMENT_TABLE_STRUCTURE
            CaesarTypes.Add(new byte[] { 2, 4, 4 }); // UNK28
            CaesarTypes.Add(new byte[] { 2, 4, 4, 2, 4, 4, 4, 4, 4 }); // CTFHEADER
            CaesarTypes.Add(new byte[] { 2, 4, 2, 4, 4, 4 }); // LANGUAGE_TABLE
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2 }); // CCFHEADER
            CaesarTypes.Add(new byte[] { 6, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }); // UNK32
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 2, 4 }); // CCFFRAGMENT
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 2, 4 }); // UNK34
            CaesarTypes.Add(new byte[] { 2, 4, 4, 4, 4, 4, 4, 2, 4 }); // UNK35
            CaesarTypes.Add(new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 0, 0, 0 }); // UNK36
        }

        public static int ReadCBFWithOffset(int memberIndex, StructureName structureName, byte[] input)
        {
            int byteOffset = CaesarStructure.GetCBFOffset(memberIndex, structureName, input);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(input)))
            {
                byte[] layout = CaesarStructure.GetCaesarLayout(structureName);
                return CaesarReader.ReadIntWithSize(reader, layout[memberIndex], byteOffset);
            }
        }
        public static uint ReadCBFWithOffsetUnsigned(int memberIndex, StructureName structureName, byte[] input)
        {
            int byteOffset = CaesarStructure.GetCBFOffset(memberIndex, structureName, input);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(input)))
            {
                byte[] layout = CaesarStructure.GetCaesarLayout(structureName);
                return CaesarReader.ReadUIntWithSize(reader, layout[memberIndex], byteOffset);
            }
        }

        public static int GetCBFOffset(int memberIndex, StructureName structureName, byte[] cbfInput)
        {
            // test:
            // byte[] cbfInput = new byte[] { 0x03, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x63, 0x54, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x50, 0x52, 0x45, 0x50, 0x5F, 0x55, 0x6E, 0x73, 0x69, 0x67, 0x6E, 0x65, 0x64, 0x5F, 0x31, 0x42, 0x79, 0x74, 0x65, 0x00 };
            // int member_type = 0x1C; 
            // CaesarStructure.StructureName.PRESENTATION_STRUCTURE
            // result =  0x13

            // essentially checks if a bitflag is active, then returns the byte offset to the member

            byte[] structureLayout = GetCaesarLayout(structureName);

            byte bitmask = 1;
            int arrayOffset = structureLayout[0]; // first structure element is always a static offset, to skip past the bitflags
            int cbfOffset = 0;

            for (int i = 1; i < memberIndex; ++i)
            {
                bool bitflagIsEnabled = (bitmask & cbfInput[cbfOffset]) > 0;
                if (bitflagIsEnabled)
                {
                    if (memberIndex != i)
                    {
                        arrayOffset += structureLayout[i];
                    }
                }
                else if ((memberIndex == i) && !bitflagIsEnabled)
                {
                    // found the requested member, but the member is marked as absent in the bitflag, so there is no value
                    arrayOffset = 0;
                }

                // move on to the next bit, and if our bitflag is fully read, move to the next bitflag
                if (bitmask == 0x80)
                {
                    ++cbfOffset;
                    bitmask = 1;
                }
                else
                {
                    bitmask *= 2;
                }
            }
            return arrayOffset;
        }
    }
}
