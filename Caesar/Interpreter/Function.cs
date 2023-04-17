using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class Function
    {
        public string Name = "";
        public int Ordinal;
        public int EntryPoint;

        public override string ToString()
        {
            return $"Fn: {Name}, Ordinal: 0x{Ordinal:X}, EntryPoint: 0x{EntryPoint:X}";
        }
    }
}
