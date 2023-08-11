using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Host
{
    public interface IChannel
    {
        void DebugLine(int level, string message);
        void ErrorLine(int level, string message);
        void RaiseError(int id);
        void ClearError();
        int GetLastError();
        bool ErrorConditionActive();
        bool CollectionMessageSend(ChannelRequest request, byte[] receivedMessage);
        bool CollectMessageRun(ChannelReadResponse response);
        bool SetCommunicationParameter(string paramName, int newParameter);
        bool GetCommunicationParameter(string paramName, out int comParam);
    }
}
