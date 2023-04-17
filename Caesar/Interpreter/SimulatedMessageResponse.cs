using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class SimulatedMessageResponse
    {
        public byte[] MatchValue = new byte[] { };
        public byte[] MatchMask = new byte[] { };
        public byte[] ResponseValue = new byte[] { };
        public Action<byte[]> Callback;
    }
}
