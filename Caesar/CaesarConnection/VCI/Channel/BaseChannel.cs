using CaesarConnection.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.VCI.Channel
{
    public class BaseChannel : IDisposable
    {
        public delegate void ReceiveEvent(Envelope env);
        public event ReceiveEvent ChannelReceivedMessage;

        public delegate void LogIOEvent(byte[] content);
        public event LogIOEvent LogReceive;
        public event LogIOEvent LogTransmit;
        public System.IO.TextWriter TraceWriter = null;
        protected Stopwatch TraceStopwatch = new Stopwatch();

        bool disposed = false;

        protected void LogMessageReceive(byte[] content)
        {
            if (LogReceive != null) 
            { 
                LogReceive(content);
            }
            string traceMessage = $"{TraceStopwatch.ElapsedMilliseconds:D10}] RX: {BitConverter.ToString(content).Replace("-", " ")}";
            //Console.WriteLine(traceMessage);
            TraceWriter?.WriteLine(traceMessage);
        }
        protected void LogMessageTransmit(byte[] content)
        {
            if (LogTransmit != null) 
            {
                LogTransmit(content);
            }
            string traceMessage = $"{TraceStopwatch.ElapsedMilliseconds:D10}] TX: {BitConverter.ToString(content).Replace("-", " ")}";
            //Console.WriteLine(traceMessage);
            TraceWriter?.WriteLine(traceMessage);
        }


        public virtual void Dispose() 
        {
            if (disposed) { return; }
            disposed = true;
            TraceStopwatch.Stop();
        }


        public virtual void Send(Envelope env)
        {

        }

        protected void RaiseReceiveEvent(Envelope env)
        {
            if (ChannelReceivedMessage != null) 
            {
                ChannelReceivedMessage(env);
            }
        }

        public virtual void ReloadIsoTpTimings() 
        {
        
        }
    }
}
