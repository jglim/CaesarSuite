using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar
{
    public class BasicEnum
    {
        public int EnumValue;
        public int EnumName_CTF;
        public string EnumName;

        // behaves like a basic c# enum, no scaling etc
        public BasicEnum(BinaryReader reader, long baseAddress, int value, int enumName, CTFLanguage language) 
        {
            EnumValue = value;
            EnumName_CTF = enumName;
            EnumName = language.GetString(enumName);
        }
    }
}
