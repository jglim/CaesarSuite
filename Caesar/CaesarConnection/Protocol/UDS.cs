using CaesarConnection.ComParam;
using CaesarConnection.Protocol.Internal;
using CaesarConnection.Target;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaesarConnection.Protocol
{
    public class UDS : BaseProtocol
    {
        List<Envelope> MessagesWaitingForResponse = new List<Envelope>(); // FIXME: this needs to be locked during dispatch/cleanup
        System.Timers.Timer TesterPresentTimer = new System.Timers.Timer();
        bool disposed = false;
        private object TxLock = new object(); // locking object for threadsafe transmit
        private object RxLock = new object();

        public override void Initialize()
        {
            Channel.ChannelReceivedMessage += Channel_ChannelReceivedMessage;
            // should initialize be blocking?
            EnterSessionDiagnostic();            
            InitializeTesterPresent();
        }

        public override int GetEcuVariant()
        {
            byte[] ecuid = Send(new byte[] { 0x22, 0xF1, 0x00 });
            // expects a value like 62 F1 00 00 13 07 03, where 13 07 is the variant in BE
            if ((ecuid.Length >= 6) && (ecuid.Take(2).SequenceEqual(new byte[] { 0x62, 0xF1 }))) 
            {
                return (ecuid[4] << 8) | ecuid[5];
            }
            throw new Exception($"UDS variant detection failed");
        }

        public bool EnterSessionDiagnostic() 
        {
            // enters a diagnostic session. runs once on initialization
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                if (EcuIsPhysical())
                {
                    // physical initialization: tell the ecu directly to switch sessions, straightforward.
                    Console.WriteLine($"EnterSessionDiagnostic : EcuIsPhysical T: {sw.ElapsedMilliseconds}");
                    byte[] sessionChange = Send(
                        message: GetMessageForSessionDiagnostic(),
                        responseRequired: true,
                        recipientType: Envelope.RequestType.Physical,
                        throwExceptionOnError: true
                    );
                    Console.WriteLine($"Exiting physical init T: {sw.ElapsedMilliseconds}");
                    // expecting a 50 03 xx xx..
                    if ((sessionChange.Length > 1) && (sessionChange[0] == 0x50))
                    {
                        return true;
                    }
                    return false;
                }
                else if (EcuIsFunctional())
                {
                    // during functional initialization, vediamo transmits session change (10 92) twice. There isn't a comparam for this value
                    Console.WriteLine($"EnterSessionDiagnostic : EcuIsFunctional T: {sw.ElapsedMilliseconds}");
                    const int FunctionalInitializationSessionInitCount = 2; // FIXME:HARDCODED
                    for (int i = 0; i < FunctionalInitializationSessionInitCount; i++)
                    {
                        Console.WriteLine($"Index: {i}, FunctionalInitializationSessionInitCount: {FunctionalInitializationSessionInitCount} T: {sw.ElapsedMilliseconds}");
                        Send(
                            message: GetMessageForSessionDiagnostic(),
                            responseRequired: false,
                            recipientType: Envelope.RequestType.Functional,
                            throwExceptionOnError: true
                        );
                        // requires functional delay : Default value for the time between the two StartDiagnosticSession-DCXDiagnosticSession (10 92) in functional initialization sequence.
                        // if first r1 is not defined (unlikely), use 200ms as a default
                        int r1 = (int)ComParameters.GetParameterOrDefault(CP.GLOBAL_FIRST_R1_SUG, 200); // FIXME:HARDCODED
                        Console.WriteLine($"R1 interval: {r1}ms, defined: {ComParameters.HasParameter(CP.GLOBAL_FIRST_R1_SUG)}");
                        Task.Delay(r1);
                    }
                    // for functional initialization, there isn't an obvious way to tell if the request was successful
                    // even if there was a reply, the existing filters will discard their responses so it cannot be received here
                    // only TX_INDICATION from the channel will hint if the raw CAN frames were picked up by someone on the bus
                    // i.e. no indication -> most likely failure
                    Console.WriteLine($"Exiting functional init T: {sw.ElapsedMilliseconds}");
                    return true;
                }
                
                // no idea how to handle if it's not physical AND not functional
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnterSessionDiagnostic exception: {ex.Message}, T: {sw.ElapsedMilliseconds}");
                return false;
            }
        }


        public void EnterSessionNormal()
        {
            // returns to a normal operation mode (exit diagnostics session). runs once on deinitialization
            // no real exception handling here, since this sends the last message before the channel is closed
            try
            {
                Send(
                    message: GetMessageForSessionNormal(),
                    responseRequired: EcuIsPhysical(),
                    recipientType: EcuIsFunctional() ? Envelope.RequestType.Functional : Envelope.RequestType.Physical,
                    throwExceptionOnError: false
                );
            }
            catch 
            {
            
            }
            // timings can go out of the window now
        }

        public virtual byte[] GetMessageForSessionDiagnostic()
        {
            return new byte[] { 0x10, 0x03 };
        }
        public virtual byte[] GetMessageForSessionNormal()
        {
            return new byte[] { 0x10, 0x01 };
        }

        public void InitializeTesterPresent() 
        {
            Console.WriteLine($"InitializeTesterPresent");
            // on protocol construction, initialize the testerpresent timer
            // unbind the event in case it was already bound. If nothing was bound, nothing happens (no exception)
            TesterPresentTimer.Elapsed -= TesterPresentTimer_Elapsed;
            TesterPresentTimer.Elapsed += TesterPresentTimer_Elapsed;
            EnsureTesterPresentInterval();
            TesterPresentTimer.Enabled = true;
            TesterPresentTimer.Start();
        }

        public void UninitializeTesterPresent() 
        {
            // this is for cleanup/disposal. to actually stop testerpresent messages, configure the appropriate comparam
            TesterPresentTimer.Stop();
        }

        private void EnsureTesterPresentInterval()
        {
            // checks if the current testerpresent timer interval matches the comparams, if not, update and reconfigure the timer
            TimeSpan tpInterval = TimeSpan.FromSeconds((double)ComParameters.GetParameter(CP.TesterPresentTime));
            if (TesterPresentTimer.Interval != tpInterval.TotalMilliseconds) 
            {
                TesterPresentTimer.Interval = tpInterval.TotalMilliseconds;
            }
        }

        private void TesterPresentTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // checks and sends testerpresent messages. timer ticks based on last configured interval, regardless of testerpresent enable state
            // if the comparams were modified e.g. during interpreter execution, we will only pick up the change here on the next tick
            EnsureTesterPresentInterval();

            // the TP timer will always tick. it is gated here to check if TP is actually enabled
            if ((int)ComParameters.GetParameter(CP.TesterPresentHandling) == 0) // CP NO_TESTERPRESENT , 1 -> tp off, 0 -> tp on
            {
                byte[] tpMessage = new byte[2];
                BinaryPrimitives.WriteUInt16BigEndian(tpMessage, (ushort)ComParameters.GetParameter(CP.TESTERPRESENT_MESSAGE));

                // physical: pick up a reply and discard it anyway. functional: we can't get a reply since it'll be filtered out
                Send(
                    message: tpMessage,
                    responseRequired: EcuIsPhysical(),
                    recipientType: EcuIsFunctional() ? Envelope.RequestType.Functional : Envelope.RequestType.Physical,
                    throwExceptionOnError: false
                );
                // we don't really care about the response as long as it's not a NR
            }
        }


        private void Channel_ChannelReceivedMessage(Envelope env)
        {
            // event source for messages received from the vci

            // isotp messages must have at least 4 bytes, specifying the req/resp id e.g. `00 00 07 E8`
            if (env.Response.Length <= 4) { return; }

            // check if we are the intended recipient
            int id = BinaryPrimitives.ReadInt32BigEndian(env.Response);
            int expectedResponseId = (int)ComParameters.GetParameter(CP.CanRespUSDTId);
            if (id != expectedResponseId) { return; }

            // now that we are certain that the message is for us, find the originating request for this response
            Span<byte> actualContent = env.GetResponseContent();

            // Console.WriteLine($"uds ch rx: {BitConverter.ToString(actualContent.ToArray())}");

            byte originalMessageId = actualContent[0];

            // if this is a negative response, the value will be <7F> <original id> <nrc>
            if (actualContent[0] == UDSMessageType.NegativeResponse)
            {
                if (actualContent.Length < 3)
                {
                    throw new Exception($"Received a negative response with an unhandled length: {actualContent.Length}");
                }
                originalMessageId = actualContent[1];
            }
            else 
            {
                // not a NR, successful response ids are (request id + 0x40), subtracting 0x40 lets us find the original req id
                originalMessageId -= 0x40;
            }

            // search the outbound messages queue, check if any requests match the response id
            Envelope foundMessage = null;

            // fixme: forloop since a foreach gets upset when collection changes (when a message is inserted)
            foreach (var waitingMessage in MessagesWaitingForResponse)
            {
                bool messageRequestLengthValid = waitingMessage.Request.Length > 4;
                bool currentMessageIdMatchesSourceMessageId = waitingMessage.Request[4] == originalMessageId;
                bool currentMessageHasNoResponse = waitingMessage.Response.Length == 0;
                if (messageRequestLengthValid && currentMessageIdMatchesSourceMessageId && currentMessageHasNoResponse)
                {
                    // yay found a matching request
                    // attach the raw response for consistency
                    // the request has the canid prefix, so the response should have it too, even if it is useless now
                    // Console.WriteLine($"found msg to signal");
                    waitingMessage.Response = env.Response;
                    foundMessage = waitingMessage;
                    break;
                }
            }
            // after the search loop, if we found a matching message, signal that it has a response
            if (foundMessage != null)
            {
                MessagesWaitingForResponse.Remove(foundMessage);
                foundMessage.ResponseEvent.Set();
            }
            else
            {
                // throw new Exception($"Debug: orphaned message");
                Console.WriteLine($"Warning: received an orphaned PDU: {BitConverter.ToString(actualContent.ToArray())}");
            }

        }

        public override void Dispose()
        {
            if (disposed) { return; }
            // stop tp messages
            UninitializeTesterPresent();

            // exit diagnostic session
            EnterSessionNormal();

            // stop listening to channel
            Channel.ChannelReceivedMessage -= Channel_ChannelReceivedMessage;

            // remember to call the base function, which cleans up the channel
            base.Dispose();
            disposed = true;
        }


        public override byte[] Send(byte[] message, bool responseRequired = true, Envelope.RequestType recipientType = Envelope.RequestType.Physical, bool throwExceptionOnError = false)
        {
            // high-level wrapper for sending a uds message. request is blocking
            Envelope env = new Envelope() { Request = message, ResponseRequired = true, RecipientType = Envelope.RequestType.Physical };

            // prepend isotp request id
            int requestId = (int)ComParameters.GetParameter(recipientType == Envelope.RequestType.Physical ? CP.CanPhysReqId : CP.CanFuncReqId);
            byte[] reqIdBytes = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(reqIdBytes, requestId);

            byte[] newRequest = new byte[reqIdBytes.Length + env.Request.Length];
            Array.ConstrainedCopy(reqIdBytes, 0, newRequest, 0, reqIdBytes.Length);
            Array.ConstrainedCopy(env.Request, 0, newRequest, reqIdBytes.Length, env.Request.Length);
            env.Request = newRequest;
            env.ResponseRequired = responseRequired;

            bool sendSuccess = false;
            
            // locking for send because threaded access will break wait timings (e.g. simultaneous user request + testerpresent)
            // some ecus will drop messages if wait intervals are not observed
            lock (TxLock) 
            {
                sendSuccess = Send(env);
            }

            // no response required, no further checks required
            if (!responseRequired) 
            {
                return Array.Empty<byte>();
            }

            // if a response is available, grab the actual content without the can id
            if (sendSuccess)
            {
                return env.GetResponseContent().ToArray();
            }

            // response was required, but not received, check if an exception should be thrown
            if (throwExceptionOnError)
            {
                throw new Exception($"Exception while sending {BitConverter.ToString(message)}");
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public bool Send(Envelope env)
        {
            // sends a raw envelope, handles both RepeatReqCountApp and BusyRepeatRequest, and optionally listens for a response
            // returns false on transmit error, or when a response was expected but not received

            CancellationTokenSource cancelSource = new CancellationTokenSource(-1);
            CancellationToken parentCancellationToken = cancelSource.Token;

            // allowable repeat count for retries (e.g. timeout)
            int repeatMax = (int)ComParameters.GetParameter(CP.RepeatReqCountApp);

            bool requestSuccess = false;

            // if the request comes back with a valid response, but is actually 7f xx 21, the request must be repeated
            // this loop handles retries up till repeatMax
            for (int repeatCount = 0; repeatCount < repeatMax; repeatCount++)
            {
                requestSuccess = SendSingleRequest(env, parentCancellationToken);

                if (IsRawMessageTypeofNRC(env, UDSNegativeResponse.BusyRepeatRequest))
                {
                    Task.Delay(Get7F21Timeout());
                    continue;
                }
                else 
                {
                    break;
                }
            }
            return requestSuccess;
        }

        public bool SendSingleRequest(Envelope env, CancellationToken parentCancellationToken) 
        {
            // sends a single request (does not handle BusyRepeatRequest), optionally listens for a response
            // returns false on transmit error, or when no response was expected but not received
            try
            {
                Channel.Send(env);
            }
            catch 
            {
                Console.WriteLine($"Warning: Failing on SendSingleRequest while requesting VCI to send");
                return false;
            }
            
            if (env.ResponseRequired)
            {   
                int repeatMax = (int)ComParameters.GetParameter(CP.I_ROUTINECOUNTER); // allowable repeat count (here, 7f xx 78)
                bool resultSuccess = true;
                TimeSpan waitTime = GetP2Max(); // initial wait time is defined by p2max

                // listen loop is run x times to handle potential 7f xx 78; the loop limit is defined by CP.I_ROUTINECOUNTER
                for (int repeatCount = 0; repeatCount < repeatMax; repeatCount++) 
                {
                    resultSuccess = ListenForResponse(env, waitTime, parentCancellationToken);

                    // if listen timed out, log 
                    if (!resultSuccess) 
                    {
                        Console.WriteLine($"Warning: listen timeout ({waitTime.TotalMilliseconds}ms) for request {BitConverter.ToString(env.Request)}");
                    }

                    // check if response is 7f xx 78
                    if (IsRawMessageTypeofNRC(env, UDSNegativeResponse.RequestCorrectlyReceived_ResponsePending))
                    {
                        // if response pending, re-enter the listen loop, but use 7f xx 78 timings
                        resultSuccess = false; // this was true above, since we technically received a response. should be false now
                        waitTime = Get7F78Timeout(); // different wait time when waiting for 7fxx78
                        // Console.WriteLine($"RequestCorrectlyReceived_ResponsePending, timeout {waitTime.TotalMilliseconds}ms repeatmax {repeatMax}");
                        continue;
                    }

                    // if it's not a 7f xx 78, we don't have to listen anymore,
                    // at this point, we either have a proper, valid response, or an empty buffer (bus timeout)
                    break;
                }
                if (!resultSuccess) 
                {
                    Console.WriteLine($"Warning: transmit failure for single request {BitConverter.ToString(env.Request)}");
                }
                // success state (also) depends if it was cancelled
                return resultSuccess;
            }
            else
            {
                // wait for P3 interval, required as this isn't expecting a response
                // normally when expecting a response, we will wait until the ecu responds, so there is an implicit delay
                // since we are not expecting a response, use P3Wait so that we do not accidentally saturate the bus
                // this delay is particularly important for gateways that are handling 500k <-> 83.3k baud conversion
                P3Wait(env, parentCancellationToken);

                // for send-only, it is a success if it made it here without throwing an exception downstream
                return true;
            }
        }

        public bool ListenForResponse(Envelope env, TimeSpan period, CancellationToken parentCancellationToken)
        {
            // inserts a message envelope into the message waitlist, then wait for it to be either signaled (on successful match) or timed out

            CancellationTokenSource p2TimeoutSource = new CancellationTokenSource(period);
            CancellationTokenSource cancelSource = CancellationTokenSource.CreateLinkedTokenSource(p2TimeoutSource.Token, parentCancellationToken);
            CancellationToken cancellationToken = cancelSource.Token;

            // prepare the waithandle
            env.ResponseEvent.Reset();

            // clear the envelope's response in case it contains data
            env.Response = Array.Empty<byte>();

            // add this envelope to the pool of envelopes that are listening for responses
            MessagesWaitingForResponse.Add(env);

            // wait for a response, either from successfully receiving a message, or from being cancelled
            WaitHandle.WaitAny(new[] { env.ResponseEvent, cancellationToken.WaitHandle });

            // remove from listening pool anyway, as cancelled messages will still be in the pool
            MessagesWaitingForResponse.Remove(env);

            // return success (true) or cancelled (false)
            return !cancellationToken.IsCancellationRequested;
        }

        public bool IsRawMessageTypeofNRC(Envelope env, byte nrc) 
        {
            // given a raw message e.g. `00 00 07 E8 7F 22 01`, check if it is a negative response that matches the specified NR code
            // 4 can id bytes, 3 nrc bytes
            if (env.Response.Length < 7) 
            {
                return false;
            }
            Span<byte> actualContent = env.GetResponseContent();
            return (actualContent.Length >= 3) && (actualContent[0] == UDSMessageType.NegativeResponse) && (actualContent[2] == nrc);
        }

        public void P3Wait(Envelope env, CancellationToken token) 
        {
            // Wait: "Used time before a next request is send after successful transmission of the previous request"
            // Required after sending a message when no response is expected

            if (env.ResponseRequired) 
            {
                // p3phys: "..in case of physical addressing and no response required."
                return;
            }
            // WARNING: P3Phys and P3Func are uds-specific mappings
            TimeSpan delay = TimeSpan.Zero;
            if (env.RecipientType == Envelope.RequestType.Physical) 
            {
                delay = GetP3Physical();
            }
            else if (env.RecipientType == Envelope.RequestType.Functional)
            {
                delay = GetP3Functional();
            }
            Task.Delay(delay, token).Wait(token);
        }

        public TimeSpan GetCanNetworkDelay()
        {
            // CP.CanTransmissionTime is only mapped on uds
            // unofficially added to kw2c3pe as there were false timeouts
            return TimeSpan.FromSeconds((double)ComParameters.GetParameterOrDefault(CP.CanTransmissionTime, 0M));
        }

        public TimeSpan GetP2Max() 
        {
            // Wait: maximum time between request and response.
            // After a message is sent, wait for a maximum of this duration. If no response is available, the message has timed out
            return TimeSpan.FromSeconds((double)ComParameters.GetParameter(CP.P2Max)).Add(GetCanNetworkDelay());
        }

        // P3 Physical and functional are different for uds/kw2c3pe and is therefore virtual so that kw2c3pe can override it
        public virtual TimeSpan GetP3Physical()
        {
            // mandatory wait time after sending a physical message that does not expect a response
            // undefined for kw2c3pe
            // wait minimums are guaranteed so there isn't a need to add network delay
            return TimeSpan.FromSeconds((double)ComParameters.GetParameterOrDefault(CP.P3Phys));
        }
        public virtual TimeSpan GetP3Functional()
        {
            // mandatory wait time after sending a functional message that does not expect a response
            // wait minimums are guaranteed so there isn't a need to add network delay
            if (ComParameters.HasParameter(CP.P3Func))
            {
                return TimeSpan.FromSeconds((double)ComParameters.GetParameter(CP.P3Func));
            }
            return TimeSpan.FromSeconds(0.15); // this isn't defined for kw2c3pe, using 150ms as a sane default
        }

        public virtual TimeSpan Get7F78Timeout()
        {
            // Wait interval when receiving a RequestCorrectlyReceived_ResponsePending. After this period, the message has timed out
            TimeSpan response;
            if (ComParameters.HasParameter(CP.P3_MAX)) // actually kw2c3pe
            {
                response = TimeSpan.FromMilliseconds((double)ComParameters.GetParameter(CP.P3_MAX));
            }
            else if (ComParameters.HasParameter(CP.P2_EXT_TIMEOUT_7F_78)) // uds
            {
                response = TimeSpan.FromMilliseconds((double)ComParameters.GetParameter(CP.P2_EXT_TIMEOUT_7F_78));
            }
            else 
            {
                throw new Exception($"No suitable ComParam was available for Get7F78Timeout");
            }
            return response.Add(GetCanNetworkDelay());
        }
        public virtual TimeSpan Get7F21Timeout()
        {
            // Wait time between receiving BusyRepeatRequest and retransmission
            TimeSpan response;
            if (ComParameters.HasParameter(CP.P3_7F_21))
            {
                response = TimeSpan.FromMilliseconds((double)ComParameters.GetParameter(CP.P3_7F_21));
            }
            else if (ComParameters.HasParameter(CP.P2_EXT_TIMEOUT_7F_21))
            {
                response = TimeSpan.FromMilliseconds((double)ComParameters.GetParameter(CP.P2_EXT_TIMEOUT_7F_21));
            }
            else 
            {
                throw new Exception($"No suitable ComParam was available for Get7F21Timeout");
            }
            return response.Add(GetCanNetworkDelay());
        }

        public override SoftwareBlock[] GetSoftwareBlocks()
        {
            List<SoftwareBlock> result = new List<SoftwareBlock>();

            // read partnumbers
            byte[] response = Send(new byte[] { 0x22, 0xF1, 0x21 });

            // if no blocks, or NR via 7F xx, stop
            if (response.Length <= 3) 
            {
                return result.ToArray();
            }

            const int nameLength = 10;
            int blockCount = (response.Length - 3) / nameLength;
            for (int blockIndex = 0; blockIndex < blockCount; blockIndex++) 
            {
                SoftwareBlock sb = new SoftwareBlock();
                sb.Index = blockIndex;

                byte[] nameBytes = response.Skip(3 + blockIndex * nameLength).Take(nameLength).ToArray();
                sb.PartNumber = Encoding.ASCII.GetString(nameBytes);
                result.Add(sb);
            }

            // read versions
            response = Send(new byte[] { 0x22, 0xF1, 0x51 });
            const int versionLength = 3;
            if (response.Length > 3)
            {
                for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
                {
                    result[blockIndex].Version = response.Skip(3 + blockIndex * versionLength).Take(versionLength).ToArray();
                }
            }

            // read fingerprint, flash date, flash vendor, validity
            response = Send(new byte[] { 0x22, 0xF1, 0x5B });
            // 01   ff ff   ff ff ff   ff ff ff ff 
            // 01   00 04   15 02 13   00 00 00 00
            // pres_gueltige_software @ 3
            // supplier id @ 4, 2bytes
            const int blockMetadataLength = 10;
            if (response.Length > 3)
            {
                for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
                {
                    byte[] row = response.Skip(3 + blockIndex * blockMetadataLength).Take(blockMetadataLength).ToArray();
                    var block = result[blockIndex];
                    block.Valid = row[0] != 0;
                    block.LastFlashVendor = (row[1] << 8) | row[2];
                    block.FlashYear = row[3];
                    block.FlashMonth = row[4];
                    block.FlashDay = row[5];
                    block.Fingerprint = row.Skip(6).ToArray();
                }
            }

            return result.ToArray();
        }

        public override void TransferBlock(long address, Span<byte> payload)
        {
            int blockSize = StartTransfer(address, payload);
            int dataSize = blockSize - 2;

            int fullBlockCount = (int)((double)payload.Length / dataSize);

            byte blockSequenceCounter = 0; // when writing, this starts from 1, increments, can also wrap around 0 

            // write complete blocks
            for (int i = 0; i < fullBlockCount; i++)
            {
                var dataSlice = payload.Slice(dataSize * i, dataSize);
                byte[] request = CreateTransferRequest(ref blockSequenceCounter, dataSlice);
                SendTransferRequest(request);
            }

            int partialData = payload.Length % dataSize;
            if (partialData > 0)
            {
                var dataSlice = payload.Slice(dataSize * fullBlockCount, partialData);
                byte[] request = CreateTransferRequest(ref blockSequenceCounter, dataSlice);
                SendTransferRequest(request);
            }

            EndTransfer();
        }

        private byte[] CreateTransferRequest(ref byte blockSequenceCounter, Span<byte> payload)
        {
            const byte SID = 0x36;

            unchecked { blockSequenceCounter++; }
            byte[] request = new byte[payload.Length + 2];
            request[0] = SID;
            request[1] = blockSequenceCounter;

            Span<byte> payloadSpan = new Span<byte>(request, 2, payload.Length);
            payload.CopyTo(payloadSpan);
            return request;
        }

        private void SendTransferRequest(byte[] request) 
        {
            byte[] response = Send(request, responseRequired: true, recipientType: Envelope.RequestType.Physical, throwExceptionOnError: true);
            if (response.Length < 2) 
            {
                throw new Exception($"Invalid response length from ECU while transferring block");
            }
            if (response[0] == 0x7F) 
            {
                throw new Exception($"Negative Response from ECU while transferring block: {BitConverter.ToString(response.ToArray())}");
            }
            if (response[1] != request[1])
            {
                throw new Exception($"Block index from ECU ({response[1]}) does not match outbound request ({request[1]})");
            }
        }

        private int StartTransfer(long address, Span<byte> payload)
        {
            // get ecu transfer preferences
            int addressFormat = (int)ComParameters.GetParameter("CP_MEM_ADDRESS_FORMAT");
            int lengthFormat = (int)ComParameters.GetParameter("CP_MEM_SIZE_FORMAT");

            const byte SID = 0x34;

            // will always be zero since compression and encryption is unsupported
            byte dataFormatIdentifier = CreateDataFormatIdentifier(DataFormatCompression.NoCompression, DataFormatEncryption.NoEncryption);

            byte alfid = CreateAddressAndLengthFormatIdentifier(addressFormat, lengthFormat);

            // address, size are almost always big-endian
            List<byte> addressBytes = LongToBytesBigEndian(address, addressFormat);
            List<byte> lengthBytes = LongToBytesBigEndian(payload.Length, lengthFormat);

            List<byte> transferRequestBuilder = new List<byte>(3 + addressFormat + lengthFormat);
            transferRequestBuilder.Add(SID);
            transferRequestBuilder.Add(dataFormatIdentifier);
            transferRequestBuilder.Add(alfid);
            transferRequestBuilder.AddRange(addressBytes);
            transferRequestBuilder.AddRange(lengthBytes);

            // example transfer: 00 00 06 0a     34 00 44   00 1f ff 88   00 00 00 48
            // typical response: 00 00 04 81    74 20 0f 02

            byte[] responseBytes = Send(transferRequestBuilder.ToArray(), responseRequired: true, recipientType: Envelope.RequestType.Physical, throwExceptionOnError: true);
            Span<byte> response = new Span<byte>(responseBytes);

            if (response.Length < 2)
            {
                throw new Exception($"Invalid response length from ECU while initiating transfer");
            }
            if (response[0] == 0x7F)
            {
                throw new Exception($"Negative Response from ECU while initiating transfer: {BitConverter.ToString(response.ToArray())}");
                // 0x13: incorrectMessageLengthOrInvalidFormat
                // 0x22: conditionsNotCorrect
                // 0x31: requestOutOfRange
                // 0x33: securityAccessDenied
                // 0x70: uploadDownloadNotAccepted
            }

            int responseLengthSize = response[1] >> 4; // "alfid" without the "a" part
            if (responseLengthSize > 4)
            {
                throw new Exception($"Unsupported response length (more than 32 bits) : {responseLengthSize}");
            }

            // ecu will reply with the maximum block length that it can accept, including non-payload such as SID
            int maxBlockLength = (int)BytesBigEndianToLong(response.Slice(2, responseLengthSize));
            return maxBlockLength;
        }

        private void EndTransfer() 
        {
            byte[] response = Send(new byte[] { 0x37 }, responseRequired: true, recipientType: Envelope.RequestType.Physical, throwExceptionOnError: true);
            if (response.Length != 1)
            {
                throw new Exception($"Invalid response length from ECU while exiting transfer");
            }
            // expects 0x77
            if (response[0] == 0x7F)
            {
                throw new Exception($"Negative Response from ECU while exiting transfer: {BitConverter.ToString(response.ToArray())}");
            }
        }

        public override List<ParameterMapping> GetDefaultProtocolComParamMaps()
        {
            // default mapping for ASAM <-> UDS parameter names
            return new List<ParameterMapping>()
            {
                new ParameterMapping { Source = CP.Baudrate, Destination = CP.BAUDRATE, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.CanTransmissionTime, Destination = CP.CAN_TRANSMIT, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.P2Max, Destination = CP.P2_TIMEOUT, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.P3Func, Destination = CP.P3_TIME_NEXTREQ_FUNC, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.P3Phys, Destination = CP.P3_TIME_NEXTREQ_PHYS, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.TesterPresentHandling, Destination = CP.NO_TESTERPRESENT, ConversionFactor = 1M }, // CPTesterPresentHandlingMappingTable
                new ParameterMapping { Source = CP.TesterPresentTime, Destination = CP.S3_TP_PHYS_TIMER, ConversionFactor = 0.001M },

                new ParameterMapping { Source = CP.CanPhysReqId, Destination = CP.REQUEST_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.CanFuncReqId, Destination = CP.FUNCTIONAL_REQUEST_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.CanRespUSDTId, Destination = CP.RESPONSE_CANIDENTIFIER, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.RequestAddrMode, Destination = CP.REQUESTTYPE, ConversionFactor = 1M }, // CPRequestAddrModeMappingTable
                new ParameterMapping { Source = CP.RepeatReqCountApp, Destination = CP.REQREPCOUNT, ConversionFactor = 1M }, // second param is 1
                new ParameterMapping { Source = CP.BlockSize, Destination = CP.BLOCKSIZE_SUG, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.Bs, Destination = CP.BS_MAX, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.Br, Destination = CP.BR_SUG, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.Cr, Destination = CP.CS_MAX, ConversionFactor = 0.001M },
                new ParameterMapping { Source = CP.ModifyTiming, Destination = CP.USE_TIMING_RECEIVED_FROM_ECU, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.StMin, Destination = CP.STMIN_SUG, ConversionFactor = 1M },
                new ParameterMapping { Source = CP.StMinOverride, Destination = CP.CS_SUG, ConversionFactor = 1M },
                
                // not defined: CP_LOGICAL_ADDRESS_GATEWAY, CP_LOGICAL_SOURCE_ADDRESS, CP_LOGICAL_TARGET_ADDRESS
                // new ParameterMapping { Source = CP.LogicalAddressGateway, Destination = CP.LogicalAddressGateway, ConversionFactor = 1M },
                // new ParameterMapping { Source = CP.LogicalSourceAddress, Destination = CP.LogicalSourceAddress, ConversionFactor = 1M },
                // new ParameterMapping { Source = CP.LogicalTargetAddress, Destination = CP.LogicalTargetAddress, ConversionFactor = 1M },
            };
        }
        
        public override Dictionary<string, decimal> GetDefaultComParamValues()
        {
            // default values for UDS
            // these are typically configured by the GBF which we do not understand well
            return new Dictionary<string, decimal>()
            {
                { CP.BLOCKSEQCOUNTER, 1 },
                { CP.BLOCKSIZE_ECU, 8 },
                { CP.BLOCKSIZE_SUG, 8 },
                { CP.BREAKCONDITION, 2 },
                { CP.BS_MAX, 150 },
                { CP.CAN_TRANSMIT, 130 }, // uds has this forgiveness factor by default?
                { CP.CANBUSOFFREACT, 128 },
                { CP.CANCONTROLLER_STROBES, 0xFFF },
                { CP.CANDEFAULTREACT, 192 },
                { CP.CANECU_CLASS, 2 },
                { CP.CANMONITORING, 1 },
                { CP.CHECKROUTINECOUNTER, 1 },
                { CP.CS_MAX, 150 },
                { CP.CS_SUG, 20 },
                { CP.DEFAULTREACT, 192 },
                { CP.DOWNLOADREACT, 129 },
                { CP.DSC_REPEAT_TIME, 130 },
                { CP.ECUID_OPTION, 254 },
                { CP.FUNCTIONAL_REQUEST_CANIDENTIFIER, 0x441 },
                { CP.GLOBAL_REQUEST_CANIDENTIFIER, 0x441 },
                { CP.IDENTIFIER_LENGTH, 11 },
                { CP.INIT_SESSION_TYPE, 3 },
                { CP.MIRROR_MEMORY_CORRECTION, 4 },
                { CP.P2_EXT_TIMEOUT_7F_21, 200 },
                { CP.P2_EXT_TIMEOUT_7F_78, 2000 },
                { CP.P2_TIMEOUT, 20 },
                { CP.P3_TIME_NEXTREQ_PHYS, 150 },
                { CP.P3_TIME_NEXTREQ_FUNC, 150 },
                { CP.PARTBLOCK, 1 },
                { CP.REQREPCOUNT, 3 },
                { CP.RESPTELCOUNT, 1 },
                { CP.S3_TP_PHYS_TIMER, 2000 },
                { CP.S3_TP_FUNC_TIMER, 2000 },
                { CP.SETCPDEFAULTS, 1 },
                { CP.SWSUPPLIERBLOCK, 1 },
                { CP.SWVERSIONBLOCK, 1 },
                { CP.TESTERPRESENT_MESSAGE, 0x3E00 },
                { CP.TIMEOUTP2CANREACT, 192 },
                { CP.TIMEOUTB12REQREACT, 192 },
                { CP.TIMEOUTCFREACT, 192 },
                { CP.USE_TIMING_RECEIVED_FROM_ECU, 1 },
                { CP.USED_CANDATA_LEN, 8 },

                { CP.I_DOWNLOADSIZE, 1 },
                { CP.I_EXITTYPE, 1 },
                { CP.I_GPDAUTODOWNLOAD, 2 },
                { CP.I_INITTYPE, 1 },
                { CP.I_READIDBLOCK, 1 },
                { CP.I_READTIMING, 0 },
                { CP.I_ROUTINECOUNTER, 30 },
                { CP.I_UPLOADSIZE, 0 },

                { CP.CHECKRESPONSE, 0 },
                { CP.NO_TESTERPRESENT, 0 },
                { CP.RESPONSEMODE, 0 },
                { CP.STOPREQUEST, 0 },
                { CP.MEM_ADDRESS_FORMAT, 0 },
                { CP.MEM_SIZE_FORMAT, 0 },
                { CP.REQUESTTYPE, 0 },
                { CP.ADDRESSEXTENSION, 0 },
                { CP.ADDRESSMODE, 0 },
                { CP.RSP_DSC, 0 },
                { CP.RSP_TP, 0 },
                { CP.IGNITION_REQUIRED, 0 },
                { CP.PARTNUMBERID, 0 },
                { CP.HWVERSIONID, 0 },
                { CP.SWVERSIONID, 0 },
                { CP.SUPPLIERID, 0 },
                { CP.CANCONTROLLER_TIMING, 0 },
                { CP.P3_TIME_NEXTREQ_PHYS_WITH_RESPONSE, 0 },
                { CP.SEGMENTSIZE, 0 },
                { CP.BS_MIN, 0 },
                { CP.BR_SUG, 0 },
                { CP.CS_MIN, 0 },
                { CP.STMIN_SUG, 0 },
                { CP.BAUDRATE, 0 },
                { CP.CANMONITORINGFILTER, 0 },
                { CP.CANMONITORINGIDENTIFIER, 0 },
                { CP.REQUEST_CANIDENTIFIER, 0 },
                { CP.RESPONSE_CANIDENTIFIER, 0 },
                { CP.ROE_RESPONSE_CANIDENTIFIER, 0 },

                { CP.SPARE_1, 0 },
                { CP.SPARE_2, 0 },
                { CP.SPARE_3, 0 },
                { CP.SPARE_4, 0 },
                { CP.SPARE_5, 0 },
                { CP.SPARE_6, 0 },
                { CP.SPARE_7, 0 },
                { CP.SPARE_8, 0 },
                { CP.SPARE_9, 0 },
                { CP.SPARE_10, 0 },
            };
        }
    }
}
