using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class GlobalVariable
    {
        public string Name = "";
        public DSCTypes.DSCBasicType BaseType;
        public DSCTypes.DSCDerivedType DerivedType;
        public byte[] Buffer = new byte[] { };
        public int Ordinal;

        public override string ToString()
        {
            string arrayLabel = DerivedType == DSCTypes.DSCDerivedType.Array ? $"[{Buffer.Length / DSCTypes.GetSizeOfType(BaseType, DerivedType)}]" : "";
            return $"GV: {Name}{arrayLabel}, Type: {BaseType}/{DerivedType}";
        }

        public GlobalVariable DeepCopy()
        {
            GlobalVariable var = new GlobalVariable { Name = Name, BaseType = BaseType, DerivedType = DerivedType, Buffer = new byte[Buffer.Length] };
            Array.ConstrainedCopy(Buffer, 0, var.Buffer, 0, Buffer.Length);
            return var;
        }

        public int GetSizeOfType() 
        {
            return DSCTypes.GetSizeOfType(BaseType, DerivedType);
        }

        public void Store(byte[] value, int arrayOffset = 0) 
        {
            if (DerivedType == DSCTypes.DSCDerivedType.Array) 
            {
                arrayOffset *= DSCTypes.GetSizeOfType(BaseType, DerivedType);
            }
            Array.ConstrainedCopy(value, 0, Buffer, arrayOffset, value.Length);

        }
    }
}
