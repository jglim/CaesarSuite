using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class DSCTypes
    {
        public enum DSCBasicType
        {
            Undefined = 0,
            Byte = 1,
            Word = 2,
            DWord = 3,
            Unk_1Byte = 4, // float?
            Unk_2Byte = 5, // byte again?
            Unk_4Byte = 6, // word again
            Unk_4Byte_2 = 7, // dword again?
        }

        public enum DSCDerivedType
        {
            Undefined = 0,
            Primitive = 1,
            Array = 2,
            Pointer = 3, // DWORD PTR
        }

        public static int GetSizeOfType(DSCBasicType basicType, DSCDerivedType derivedType)
        {
            // MISizeofVarDataType
            int[] typeSizes = new int[] { -1, 1, 2, 4, 1, 2, 4, 4 };
            // char, word, dword, ??, ??, ??, ??

            if (derivedType == DSCDerivedType.Pointer)
            {
                return 4; // DWORD PTR
            }
            else if ((int)derivedType < 3)
            {
                if ((basicType > 0) && ((int)basicType < 8))
                {
                    return typeSizes[(int)basicType];
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
