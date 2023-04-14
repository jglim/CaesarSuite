using CaesarConnection.ComParam;
using CaesarConnection.VCI.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.VCI
{
    public class BaseDevice : IDisposable
    {
        public string Name; 
        public string Parameter;
        internal List<BaseChannel> Channels = new List<BaseChannel>();

        public static BaseDevice[] GetDevices() 
        {
            return J2534Device.GetJ2534Devices();
        }

        public virtual void OpenVCI() 
        {
            // for j2534 devices, this loads the driver and opens a connection to the j2534 device
            // for doip, probably attempt to connect to the remote device
        }
        public virtual void Dispose() 
        {
        }

        public virtual void EnsurePhysicalProtocolCompatibility(string physicalProtocol) 
        {
            // only handle CAN, others like KW2000 require more parameters and protocol specifc handling
            bool isCanConnection = (physicalProtocol == "HSCAN") || (physicalProtocol == "LSCAN");
            if (!isCanConnection)
            {
                throw new Exception($"Physical connection type does not appear to run on CAN: {physicalProtocol}");
            }
        }

        public virtual Channel.BaseChannel CreateChannel(ParameterSet cp, System.IO.TextWriter traceWriter) 
        {
            throw new Exception($"BaseDevice: unable to create channel");
        }

        public virtual void ReleaseChannel(BaseChannel channel) 
        {
            Channels.Remove(channel);
            channel.Dispose();
        }
        public virtual int GetPersistentHash()
        {
            return 0;
        }
    }
}
