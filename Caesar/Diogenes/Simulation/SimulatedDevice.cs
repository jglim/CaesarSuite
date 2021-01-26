using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes.Simulation
{
    // this whole simulation setup is required for my debugging as I don't always have access to my ECU/J2534 device
    public class SimulatedDevice
    {
        public virtual byte[] ReceiveRequest(IEnumerable<byte> request)
        {
            Console.WriteLine("ReceiveRequest was not overridden!");
            return new byte[] { };
        }
    }
}
