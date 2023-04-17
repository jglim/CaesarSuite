using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaesarConnection.Protocol;
using CaesarInterpreter;
using CaesarInterpreter.Host;

namespace Diogenes
{
    class PhysicalChannel : IChannel
    {

        Queue<byte[]> Inbox = new Queue<byte[]>();
        public List<SimulatedMessageResponse> MessageResponses = new List<SimulatedMessageResponse>();

        bool ErrorActive = false;
        int ErrorId = 0;

        public bool CollectionMessageSend(ChannelRequest request, byte[] receivedMessage)
        {
            if (DiogenesSharedContext.Singleton.Channel is null) 
            {
                throw new Exception("Attempted to send a message through CollectionMessageSend without a valid channel");
            }

            try
            {
                Envelope.RequestType ecuType = DiogenesSharedContext.Singleton.Channel.EcuIsFunctional() ? Envelope.RequestType.Functional : Envelope.RequestType.Physical;
                Console.WriteLine($"[I] ECU >> {BitUtility.BytesToHex(receivedMessage, true)}");
                byte[] response = DiogenesSharedContext.Singleton.Channel.Send(receivedMessage, true, ecuType, true);
                Console.WriteLine($"[I] ECU << {BitUtility.BytesToHex(response, true)}");
                Inbox.Enqueue(response);
            }
            catch
            {
                // should log to interpreter error channel
                RaiseError(999); // randomly picked nonzero number, haven't seen this in use yet
                return false;
            }

            return true;
        }

        public bool CollectMessageRun(ChannelReadResponse response)
        {
            if (Inbox.Count == 0)
            {
                return false; // nothing received (normally the transaction should be done during tx)
            }

            byte[] message = Inbox.Dequeue();

            response.Content.ContentBytes = message;
            response.ContextLength = message.Length;
            response.ContextStart = 0;
            response.SourceAddress = 0;
            response.ResponseStatus = 1;
            response.ResponseType = 0x4C;
            response.ResponseControl = 0x5A;
            return true;
        }

        public void ErrorLine(int level, string message)
        {
            Console.WriteLine($"![{level}]: {message}");
        }
        public void DebugLine(int level, string message)
        {
            Console.WriteLine($"#[{level}]: {message}");
        }

        public void RaiseError(int id)
        {
            // Console.WriteLine($"CH Raised error: {id}");
            ErrorId = id;
            ErrorActive = true;
        }
        public void ClearError()
        {
            // Console.WriteLine($"CH cleared error");
            ErrorActive = false;
            ErrorId = 0;
        }
        public int GetLastError()
        {
            if (ErrorActive)
            {
                return ErrorId;
            }
            return 0;
        }

        public bool ErrorConditionActive()
        {
            // Console.WriteLine($"ErrorConditionActive: {ErrorActive}");
            return ErrorActive;
        }

        public bool SetCommunicationParameter(string paramName, int newParameter)
        {
            DiogenesSharedContext.Singleton.Channel.ComParameters.SetParameter(paramName, newParameter);
            return true;
        }
    }
}
