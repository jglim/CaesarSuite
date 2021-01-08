using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes.DiagnosticProtocol
{
    public class BaseProtocol
    {
        public virtual void ConnectionEstablishedHandler(ECUConnection connection)
        {

        }
        public virtual void ConnectionClosingHandler(ECUConnection connection)
        {

        }

        public virtual void SendTesterPresent(ECUConnection connection) 
        {
        
        }

        public virtual string GetProtocolName() 
        {
            return "UninitializedProtocol";
        }
        public virtual bool SupportsUnlocking()
        {
            return false;
        }

        public static BaseProtocol GetProtocol(string profileName)
        {
            if (profileName.Contains("_UDS_"))
            {
                return new UDS();
            }
            else if (profileName.Contains("_KW2C3PE_"))
            {
                return new KW2C3PE();
            }
            return new UnsupportedProtocol();
        }
    }
}
