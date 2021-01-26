using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes.DiagnosticProtocol
{
    public class UnsupportedProtocol : BaseProtocol
    {
        public override void ConnectionEstablishedHandler(ECUConnection connection)
        {
            //Console.WriteLine("The current protocol is unsupported. Most functions will not be available.");
        }

        public override string GetProtocolName()
        {
            return "UnsupportedProtocol";
        }

        public override bool SupportsUnlocking()
        {
            return false;
        }
    }
}
