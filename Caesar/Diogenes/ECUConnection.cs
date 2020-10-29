using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.J2534;

namespace Diogenes
{
    class ECUConnection
    {
        public API ConnectionAPI;
        public Device ConnectionDevice;
        public Channel ConnectionChannel;

        public ECUConnection(string fileName)
        {
            Console.WriteLine($"Initializing new connection with file: {fileName}");
            ConnectionAPI = APIFactory.GetAPI(fileName);
            ConnectionDevice = ConnectionAPI.GetDevice();
        }

        public void Connect()
        {
            MessageFilter FlowControlFilter = new MessageFilter()
            {
                FilterType = Filter.FLOW_CONTROL_FILTER,
                Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
                Pattern = new byte[] { 0x00, 0x00, 0x07, 0xE8 },
                FlowControl = new byte[] { 0x00, 0x00, 0x07, 0xE0 }
            };

            ConnectionChannel = ConnectionDevice.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.NONE);

            ConnectionChannel.StartMsgFilter(FlowControlFilter);
            Console.WriteLine($"Voltage is {ConnectionChannel.MeasureBatteryVoltage() / 1000}");
            ConnectionChannel.SendMessage(new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x01, 0x00 });
            GetMessageResults Response = ConnectionChannel.GetMessage();
        }

        ~ECUConnection() 
        {
            ConnectionChannel?.Dispose();
            ConnectionDevice?.Dispose();
            ConnectionAPI?.Dispose();
        }
        
    }
}
