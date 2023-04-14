using SAE.J2534;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaesarConnection.VCI.Channel;
using CaesarConnection.ComParam;
using System.IO;

namespace CaesarConnection.VCI
{
    public class J2534Device : BaseDevice
    {
        API Api;
        Device PhysicalDevice;

        public void Connect()
        {
            APIFactory.GetAPI(Parameter);
        }

        public static J2534Device[] GetJ2534Devices() 
        {
            List<J2534Device> result = new List<J2534Device>();
            foreach (APIInfo apiInfo in APIFactory.GetAPIinfo())
            {
                result.Add(new J2534Device
                {
                    Name = apiInfo.Name,
                    Parameter = apiInfo.Filename,
                });
            }
            return result.ToArray();
        }

        public override void OpenVCI()
        {
            Api = APIFactory.GetAPI(Parameter); // parameter here refers to the dll path
            PhysicalDevice = Api.GetDevice();
            Console.WriteLine($"J2534 device {Path.GetFileName(Parameter)} opened");
        }

        public override BaseChannel CreateChannel(ParameterSet cp, System.IO.TextWriter traceWriter)
        {
            J2534Channel newChannel = new J2534Channel(PhysicalDevice, cp);
            newChannel.TraceWriter = traceWriter;
            Channels.Add(newChannel);
            return newChannel;
        }

        public override void Dispose()
        {
            foreach (var channel in Channels) 
            {
                try
                {
                    channel.Dispose();
                }
                catch 
                {
                    // nothing to do here
                }
            }
            Channels.Clear();

            PhysicalDevice?.Dispose();
            Api?.Dispose();
            Console.WriteLine($"Disposed J2534Device");
            base.Dispose();
        }

        public override string ToString()
        {
            return $"J2534 : {this.Name}";
        }

        public override int GetPersistentHash()
        {
            string uniqueHandle = $"{GetType().Name}-{Name}-{Parameter}";
            // lazy 32-bit hash to store as user pref
            byte[] chunks = Encoding.UTF8.GetBytes(uniqueHandle);
            int a = 0, b = 0;
            foreach (byte chunk in chunks)
            {
                a ^= chunk;
                b += chunk;
                if ((a & 1) != 0)
                {
                    a >>= 1;
                    a |= 0x800000;
                    b <<= 1;
                    b |= 1;
                }
            }
            return a ^ b;
        }
    }
}
