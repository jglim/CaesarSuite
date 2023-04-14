using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaesarConnection.ComParam;
using CaesarConnection.Protocol;
using CaesarConnection.VCI;


namespace CaesarConnection
{
    public class Connection : IDisposable
    {
        BaseDevice Device;

        public static Connection Create(BaseDevice device /*, string physicalProtocol, string diagnosticProtocol, Dictionary<string, int> comParameters*/) 
        {
            Connection connection = new Connection
            {
                // VCI device
                Device = device,
            };
            device.OpenVCI();
            return connection;
        }

        public BaseProtocol OpenChannel(string physicalProtocol, string diagnosticProtocol, Dictionary<string, int> cpDictionary, System.IO.TextWriter traceWriter)
        {
            return BaseProtocol.Create(physicalProtocol, diagnosticProtocol, Device, cpDictionary, traceWriter);
        }

        public void Dispose() 
        {
            this.Device?.Dispose();
            this.Device = null;
        }

        public static BaseDevice[] GetDevices() 
        {
            return BaseDevice.GetDevices();
        }
    }
}
