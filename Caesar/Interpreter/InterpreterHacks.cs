using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class InterpreterHacks
    {
        // moved to channel interface
        /*
        Queue<byte[]> Inbox = new Queue<byte[]>();
        public List<SimulatedMessageResponse> MessageResponses = new List<SimulatedMessageResponse>();

        public InterpreterHacks(Interpreter _ih) 
        {
        }


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
        */
    }
}
