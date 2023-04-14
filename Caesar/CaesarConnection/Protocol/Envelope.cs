using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol
{
    /// <summary>
    /// Wrapper class used to shuttle data between Application<->Protocol as well as Protocol<->VCI
    /// J2534-Sharp has a functionally equivalent Messages class, however it is not reused here to avoid adding another mandatory dependency for a CaesarConnection user
    /// </summary>
    public class Envelope
    {
        public byte[] Request = new byte[] { };
        public byte[] Response = new byte[] { };

        public bool ResponseRequired = true;
        public ManualResetEvent ResponseEvent = new ManualResetEvent(false);

        // Haven't had issues with envelopes being stuck in the receive queue yet, but if that happens, consider a CancellationToken attached to the envelope

        public RequestType RecipientType = RequestType.Physical;
        public enum RequestType 
        {
            Physical,
            Functional,
            Gateway,
        }

        public Span<byte> GetResponseContent() 
        {
            return new Span<byte>(Response).Slice(4);
        }
    }
}
