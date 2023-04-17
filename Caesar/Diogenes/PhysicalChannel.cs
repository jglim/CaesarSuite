using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Inbox.Enqueue(receivedMessage);
            Console.WriteLine($"HACK: enqueued message `{BitUtility.BytesToHex(receivedMessage)}`");

            return true;
        }

        public bool CollectMessageRun(ChannelReadResponse response)
        {
            if (Inbox.Count == 0)
            {
                throw new Exception("this shouldn't normally happen in a simulation");
                // return false;
            }

            byte[] message = Inbox.Dequeue();
            bool messageAnswered = false;

            foreach (SimulatedMessageResponse mr in MessageResponses)
            {
                if (mr.MatchMask.Length != mr.MatchValue.Length)
                {
                    throw new Exception("Simulated response message and mask must be of equal length");
                }

                // length is not constrained
                int shortestBuffer = message.Length;
                if (mr.MatchMask.Length < shortestBuffer)
                {
                    shortestBuffer = mr.MatchMask.Length;
                }

                bool isMatch = true;
                for (int i = 0; i < shortestBuffer; i++)
                {
                    byte messageByte = message[i];
                    messageByte &= mr.MatchMask[i];
                    if (messageByte != mr.MatchValue[i])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    response.Content.ContentBytes = mr.ResponseValue;
                    response.ContextLength = mr.ResponseValue.Length;
                    response.ContextStart = 0;
                    response.SourceAddress = 0;
                    response.ResponseStatus = 1;
                    response.ResponseType = 0x4C;
                    response.ResponseControl = 0x5A;
                    messageAnswered = true;
                    Console.WriteLine($"HACK: responding to collectmessage {BitUtility.BytesToHex(message)} with {response}");

                    if (mr.Callback != null)
                    {
                        mr.Callback(message);
                    }
                }

            }

            if (!messageAnswered)
            {
                throw new Exception($"No message handler specified for request: {BitUtility.BytesToHex(message)}");
            }
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
            // sim: don't need to do anything
            return true;
        }
    }
}
