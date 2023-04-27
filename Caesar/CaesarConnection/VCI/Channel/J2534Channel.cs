using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaesarConnection.ComParam;
using CaesarConnection.Protocol;
using SAE.J2534;

namespace CaesarConnection.VCI.Channel
{
    public class J2534Channel : BaseChannel
    {
        SAE.J2534.Channel PhysicalChannel;
        ParameterSet ComParams;
        CancellationTokenSource MessageLoopCancellationToken = new CancellationTokenSource();
        bool disposed = false;

        public J2534Channel(Device PhysicalDevice, ParameterSet cp) 
        {
            TraceStopwatch.Start();

            ComParams = cp;
            Console.WriteLine($"Opening J2534Channel");
            int baudrate = (int)ComParams.GetParameter(CP.Baudrate);

            // ISOTP mode and ConnectFlag.CAN_ID_BOTH are currently hardcoded, need to check if this is fetched from comparam
            PhysicalChannel = PhysicalDevice.GetChannel(SAE.J2534.Protocol.ISO15765, (Baud)baudrate, ConnectFlag.CAN_ID_BOTH);
            Console.WriteLine($"Target Voltage: {PhysicalChannel.MeasureBatteryVoltage()} mV");

            SetJ2534Filter();
            SetIsoTpTimings();

            PhysicalChannel.ClearRxBuffer();
            PhysicalChannel.ClearTxBuffer();
            Task.Run(MessageLoop);
        }

        private void MessageLoop() 
        {
            try
            {
                while (!MessageLoopCancellationToken.IsCancellationRequested) 
                {
                    Receive();
                    // j2534-specific poll interval, should be fairly quick if the mailbox is empty
                    Task.Delay(10, MessageLoopCancellationToken.Token).Wait(-1); 
                }
            }
            catch (OperationCanceledException) 
            {
                return;
            }
        }

        private void SetJ2534Filter() 
        {
            int requestId = (int)ComParams.GetParameter(CP.CanPhysReqId);
            int responseId = (int)ComParams.GetParameter(CP.CanRespUSDTId);
            byte[] reqIdBytes = new byte[4];
            byte[] respIdBytes = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(reqIdBytes, requestId);
            BinaryPrimitives.WriteInt32BigEndian(respIdBytes, responseId);

            MessageFilter filter = new MessageFilter();
            filter.FilterType = Filter.FLOW_CONTROL_FILTER;
            filter.Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            filter.Pattern = respIdBytes;
            filter.FlowControl = reqIdBytes;
            filter.TxFlags = TxFlag.ISO15765_FRAME_PAD;

            PhysicalChannel.ClearMsgFilters();
            PhysicalChannel.StartMsgFilter(filter);

            Console.WriteLine($"Configured filter, Pattern {BitConverter.ToString(respIdBytes)},  FlowControl {BitConverter.ToString(reqIdBytes)}");
        }

        public void SetIsoTpTimings()
        {
            List<SConfig> config = new List<SConfig>();
            if (ComParams.HasParameter(CP.StMinOverride))
            {
                int stminTx = (int)ComParams.GetParameter(CP.StMinOverride);
                config.Add(new SConfig(Parameter.STMIN_TX, stminTx));
            }

            // block size
            if (ComParams.HasParameter(CP.BlockSize))
            {
                int isotpBlocksize = (int)ComParams.GetParameter(CP.BlockSize);
                config.Add(new SConfig(Parameter.ISO15765_BS, isotpBlocksize));
            }

            // isotp stmin
            if (ComParams.HasParameter(CP.StMin))
            {
                int isotpStmin = (int)ComParams.GetParameter(CP.StMin);
                config.Add(new SConfig(Parameter.ISO15765_STMIN, isotpStmin));
            }

            // debug: view values
            foreach (var configRow in config) 
            {
                Console.WriteLine($"{configRow.Parameter} : {configRow.Value}");
            }
            PhysicalChannel.SetConfig(config.ToArray());
        }

        public override void Dispose()
        {
            if (disposed) { return; }
            Console.WriteLine($"Closing J2534Channel");
            MessageLoopCancellationToken.Cancel();
            PhysicalChannel.ClearMsgFilters();
            PhysicalChannel.Dispose();
            PhysicalChannel = null;
            disposed = true;
        }

        public override void Send(Envelope e)
        {
            // this specific msg creation arrangement is required to include frame_pad
            // normally 0x00000040 (ISO15765_FRAME_PAD)
            // some ecus (e.g. ic204) will survive without frame_pad, others (e.g. CRD3) will not accept those frames
            try
            {
                Message msg = new Message(e.Request, TxFlag.ISO15765_FRAME_PAD);
                PhysicalChannel.SendMessage(msg);
            }
            catch (Exception ex) 
            {
                string log = $"Exception raised from physical channel: {ex.Message}";
                Console.WriteLine(log);
                // System.Exception: 'Exception raised from physical channel: GetLastError failed with result: TIMEOUT'
                throw new Exception(log);
            }
            
            // Console.WriteLine($"sent to phys channel");

            // log to trace
            LogMessageTransmit(e.Request);
        }

        public void Receive() 
        {
            GetMessageResults result = PhysicalChannel.GetMessage();

            // Console.WriteLine($"rx result = {result.Result} count {result.Messages.Length}");
            // no data available
            if (result.Result == ResultCode.BUFFER_EMPTY) 
            {
                return;
            }

            foreach (var message in result.Messages)
            {
                // Console.WriteLine($"rx result msg = {BitConverter.ToString(message.Data)} flags {message.RxStatus}");
                // frame was acknowledged on the bus, not needed
                if ((message.RxStatus & RxFlag.TX_INDICATION) > 0)
                {
                    continue;
                }
                // j2534 device is still processing an inbound message
                if ((message.RxStatus & RxFlag.START_OF_MESSAGE) > 0)
                {
                    continue;
                }
                // if there is still no content, warn and discard the message
                if (message.Data.Length == 0) 
                {
                    Console.WriteLine($"Received a zero-length message with rxflags {message.FlagsAsInt:X8}");
                    continue;
                }
                // if there's only an id and no content, warn and discard the message
                if (message.Data.Length == 4)
                {
                    Console.WriteLine($"Received an ID only with rxflags {message.FlagsAsInt:X8}");
                    continue;
                }

                // log to trace
                LogMessageReceive(message.Data);

                Envelope env = new Envelope()
                {
                    Response = message.Data,
                };
                RaiseReceiveEvent(env);

            }
        }

        public override void ReloadIsoTpTimings()
        {
            SetIsoTpTimings();
        }
    }
}
