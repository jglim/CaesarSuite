using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter
{
    public class TrackedObject
    {
        public int ObjectIndex;
        const int MaxObjects = 0xFFFF - 1;

        public TrackedObject(Interpreter ih)
        {
            for (int i = 1; i < MaxObjects; i++) 
            {
                if (!ih.TrackedObjects.ContainsKey(i)) 
                {
                    ObjectIndex = i;
                    ih.TrackedObjects.Add(ObjectIndex, this);
                    break;
                }
            }
        }


        public int GetPointer()
        {
            return InterpreterMemory.CreatePointerForTrackedObject(this);
        }

        public static void Free(Interpreter ih, int objectPointer)
        {
            int index = InterpreterMemory.GetTrackedObjectAtAddress(ih, objectPointer).ObjectIndex;
            if (ih.TrackedObjects.ContainsKey(index)) 
            {
                ih.TrackedObjects.Remove(index);
            }
        }
    }


    public class ChannelRequest : TrackedObject 
    {
        // public ushort LRequestLength; // 18 -> 16
        public ushort LRequestContextStart; // 16 -> 14
        public ushort LRequestContextLength; // 14 -> 12
        public byte LRequestType; // 12 -> 10
        public byte[] LRequestMessage; // 10 -> 6
        public byte LGECUAddress; // 6 -> 4
        public byte LGECUFlags; // 4 -> 2
        public byte UnkFlagStartsAs1; // 2 -> 0

        public ChannelRequest(Interpreter ih) : base(ih) 
        {
        
        }

        public override string ToString()
        {
            return $"T#{ObjectIndex}: Message[{LRequestMessage.Length}]: {BitUtility.BytesToHex(LRequestMessage)}, CtxStart: {LRequestContextStart}, CtxLength: {LRequestContextLength}, Type: {LRequestType}, ECUAddress: 0x{LGECUAddress:X2} ECUFlags: 0x{LGECUFlags:X2}, Unk: {UnkFlagStartsAs1}";
        }
    }



    public class ChannelReadResponse : TrackedObject
    {
        public int ResponseControl; // byte
        
        public int ContextLength; // word
        public int ContextStart; // word

        public int SourceAddress; // byte
        public int ResponseStatus; // byte
        public int ResponseType; // byte

        public Buffer Content;

        Interpreter m_ih;

        public ChannelReadResponse(Interpreter ih) : base(ih)
        {
            m_ih = ih;
            Content = new Buffer(ih, $"Attached to ChannelReadResponse");
        }

        ~ChannelReadResponse() 
        {
            m_ih.TrackedObjects.Remove(Content.ObjectIndex);
        }

        public override string ToString()
        {
            return $"T#{ObjectIndex}: ChannelReadResponse Control: {ResponseControl:X2}, CtxStart: {ContextStart}, CtxLength: {ContextLength}, SourceAddress: {SourceAddress:X2}, StatusFlag: {ResponseStatus:X2}, ContentType: {ResponseType:X2} [{Content.ContentBytes.Length}] : `{BitUtility.BytesToHex(Content.ContentBytes)}`";
        }
    }

    public class Buffer : TrackedObject 
    {
        public byte[] ContentBytes;
        public string Origin = "Unset";
        public Buffer(Interpreter ih, string origin) : base(ih) 
        {
            ContentBytes = new byte[] { };
            Origin = origin;
        }
        public override string ToString()
        {
            return $"T#{ObjectIndex}: Buffer origin: {Origin}, contents: {BitUtility.BytesToHex(ContentBytes)}";
        }
    }



    public class FlashSessionDataBlock : TrackedObject
    {
        public int BlockIndex;
        public FlashSessionDataBlock(Interpreter ih) : base(ih) { }

        public override string ToString()
        {
            return $"T#{ObjectIndex}: SessionDataBlock Index: {BlockIndex}";
        }
    }
}
