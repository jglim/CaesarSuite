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

        public virtual bool IsResponseToTesterPresent(byte[] inBuffer) 
        {
            return false;
        }

        public virtual string GetProtocolName() 
        {
            return "UninitializedProtocol";
        }
        public virtual bool SupportsUnlocking()
        {
            return false;
        }

        public virtual ECUMetadata QueryECUMetadata(ECUConnection connection) 
        {
            return new ECUMetadata() { };
        }

        public virtual List<DTCContext> ReportDtcsByStatusMask(ECUConnection connection, ECUVariant variant, byte inMask = 0)
        {
            return new List<DTCContext>();
        }

        public virtual bool GetDtcSnapshot(DTC dtc, ECUConnection connection, out byte[] snapshotBytes) 
        {
            snapshotBytes = new byte[] { };
            return false;
        }

        public static BaseProtocol GetProtocol(string profileName)
        {
            // fixme: this depends on the cbf author's consistency and so far it's been reliable BUT there should be a better way of specifying the protocol?
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
