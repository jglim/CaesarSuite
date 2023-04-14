#region License
/*Copyright(c) 2018, Brian Humlicek
* https://github.com/BrianHumlicek
* 
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/
#endregion License
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAE.J2534
{
    public class Channel : Common.ManagedDisposable
    {
        private object sync { get; }
        private int channelId;
        private HeapMessageArray hJ2534MessageArray { get; }
        private List<PeriodicMessage> periodicMsgList { get; } = new List<PeriodicMessage>();
        private List<MessageFilter> filterList { get; } = new List<MessageFilter>();
        internal API API { get; }
        internal Device Device { get; }
        public Protocol ProtocolID { get; }
        public ConnectFlag ConnectFlags { get; }
        public IList<PeriodicMessage> PeriodicMsgList { get { return periodicMsgList.AsReadOnly(); } }
        public IList<MessageFilter> FilterList { get { return filterList.AsReadOnly(); } }
        public int DefaultTxTimeout { get; set; }
        public int DefaultRxTimeout { get; set; }
        public TxFlag DefaultTxFlag { get; set; }

        internal Channel(Device Device, Protocol ProtocolID, Baud Baud, ConnectFlag ConnectFlags, int ChannelID, object Sync)
        {
            sync = Sync;
            channelId = ChannelID;
            hJ2534MessageArray = new HeapMessageArray(ProtocolID, CONST.HEAPMESSAGEBUFFERSIZE);
            API = Device.API;
            this.Device = Device;
            this.ProtocolID = ProtocolID;
            this.ConnectFlags = ConnectFlags;
            DefaultTxTimeout = 100;
            DefaultRxTimeout = 300;
            DefaultTxFlag = TxFlag.NONE;
        }
        /// <summary>
        /// Gets a single message using the DefaultRxTimeout
        /// </summary>
        /// <returns></returns>
        public GetMessageResults GetMessage()
        {
            return GetMessages(1, DefaultRxTimeout);
        }

        /// <summary>
        /// Reads 'NumMsgs' messages from the input buffer using the DefaultRxTimeout
        /// </summary>
        /// <param name="NumMsgs">The number of messages to return. Due to timeout, the number of messages returned may be less than the number requested.  Number must be less than or equal to J2534.CONST.HEAPMESSAGEBUFFERSIZE (default is 200)</param>
        public GetMessageResults GetMessages(int NumMsgs)
        {
            return GetMessages(NumMsgs, DefaultRxTimeout);
        }
        /// <summary>
        /// Reads 'NumMsgs' messages from the input buffer.
        /// </summary>
        /// <param name="NumMsgs">The number of messages to return. Due to timeout, the number of messages returned may be less than the number requested.  Number must be less than or equal to J2534.CONST.HEAPMESSAGEBUFFERSIZE (default is 200)</param>
        /// <param name="Timeout">Timeout (in milliseconds) for read completion. A value of zero reads buffered messages and returns immediately. A non-zero value blocks (does not return) until the specified number of messages have been read, or until the timeout expires.</param>
        public GetMessageResults GetMessages(int NumMsgs, int Timeout)
        {
            lock (sync)
            {
                hJ2534MessageArray.Length = NumMsgs;
                ResultCode Result = API.PTReadMsgs(channelId, (IntPtr)hJ2534MessageArray, hJ2534MessageArray.LengthPtr, Timeout);
                if(Result != ResultCode.TIMEOUT &&
                   Result != ResultCode.BUFFER_EMPTY)
                {
                    API.CheckResult(Result);
                }
                return new GetMessageResults(hJ2534MessageArray.ToMessageArray(), Result);
            }
        }
        /// <summary>
        /// Sends a single message 'Message' created from raw bytes
        /// </summary>
        /// <param name="Message">Raw message bytes to send</param>
        public void SendMessage(IEnumerable<byte> Message)
        {
            lock (sync)
            {
                hJ2534MessageArray.FromDataBytes(Message, DefaultTxFlag);
                SendMessages(hJ2534MessageArray);
            }
        }
        /// <summary>
        /// Sends an array of messages created from raw bytes
        /// </summary>
        /// <param name="Messages">Array of raw message bytes</param>
        public void SendMessages(IEnumerable<byte>[] Messages)
        {
            lock (sync)
            {
                hJ2534MessageArray.FromDataBytesArray(Messages, DefaultTxFlag);
                SendMessages(hJ2534MessageArray);
            }
        }
        /// <summary>
        /// Sends a single J2534Message
        /// </summary>
        /// <param name="Message">J2534Message</param>
        public void SendMessage(Message Message)
        {
            lock (sync)
            {
                hJ2534MessageArray.FromMessage(Message);
                SendMessages(hJ2534MessageArray);
            }
        }
        /// <summary>
        /// Sends an array of J2534Messages
        /// </summary>
        /// <param name="Messages">J2534Message Array</param>
        public void SendMessages(Message[] Messages)
        {
            lock (sync)
            {
                hJ2534MessageArray.FromMessageArray(Messages);
                SendMessages(hJ2534MessageArray);
            }
        }
        /// <summary>
        /// Sends the contents of a HeapMessageArray
        /// </summary>
        /// <param name="hJ2534MessageArray_Local">HeapMessageArray to send</param>
        public void SendMessages(HeapMessageArray hJ2534MessageArray_Local)
        {
            lock (sync)
                API.CheckResult(API.PTWriteMsgs(channelId,
                                                (IntPtr)this.hJ2534MessageArray,
                                                hJ2534MessageArray_Local.LengthPtr,
                                                DefaultTxTimeout));
        }
        /// <summary>
        /// Starts automated periodic transmission of a message using the channel protocol
        /// </summary>
        /// <param name="PeriodicMessage">Periodic message object</param>
        /// <returns>Message index</returns>
        public int StartPeriodicMessage(PeriodicMessage PeriodicMessage)
        {
            return StartPeriodicMessage(PeriodicMessage, ProtocolID);
        }
        /// <summary>
        /// Starts automated periodic transmission of a message
        /// </summary>
        /// <param name="PeriodicMessage">Periodic message object</param>
        /// <param name="PeriodicMessageProtocolID">Periodic message protocol</param>
        /// <returns>Message Id</returns>
        public int StartPeriodicMessage(PeriodicMessage PeriodicMessage, Protocol PeriodicMessageProtocolID)
        {
            using (HeapInt hMessageID = new HeapInt())
            using(HeapMessage hPeriodicMessage = new HeapMessage(PeriodicMessageProtocolID, PeriodicMessage))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTStartPeriodicMsg(channelId,
                                                           (IntPtr)hPeriodicMessage,
                                                           (IntPtr)hMessageID,
                                                           PeriodicMessage.Interval));
                    PeriodicMessage.MessageID = hMessageID.Value;
                    periodicMsgList.Add(PeriodicMessage);
                }
                return PeriodicMessage.MessageID;
            }
        }

        /// <summary>
        /// Stops automated transmission of a periodic message.
        /// </summary>
        /// <param name="Index"Message index>Message index</param>
        public void StopPeriodicMsg(int MessageId)
        {
            lock (sync)
            {
                API.CheckResult(API.PTStopPeriodicMsg(channelId, MessageId));
                periodicMsgList.RemoveAll(m => m.MessageID == MessageId);
            }
        }
        /// <summary>
        /// Starts a message filter for the channel protocol
        /// </summary>
        /// <param name="Filter">Message filter object</param>
        /// <returns>Filter Id</returns>
        public int StartMsgFilter(MessageFilter Filter)
        {
            return StartMsgFilter(Filter, ProtocolID);
        }
        /// <summary>
        /// Starts a message filter
        /// </summary>
        /// <param name="Filter">Message filter object</param>
        /// <param name="FilterProtocolID">Message filter protocol</param>
        /// <returns>Filter Id</returns>
        public int StartMsgFilter(MessageFilter Filter, Protocol FilterProtocolID)
        {
            using (HeapInt hFilterID = new HeapInt())
            using (HeapMessage hMask = new HeapMessage(FilterProtocolID, Filter.TxFlags, Filter.Mask))
            using (HeapMessage hPattern = new HeapMessage(FilterProtocolID, Filter.TxFlags, Filter.Pattern))
            using (HeapMessage hFlowControl = new HeapMessage(FilterProtocolID, Filter.TxFlags, Filter.FlowControl))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTStartMsgFilter(channelId,
                                                         (int)Filter.FilterType,
                                                         (IntPtr)hMask,
                                                         (IntPtr)hPattern,
                                                         Filter.FilterType == J2534.Filter.FLOW_CONTROL_FILTER ? (IntPtr)hFlowControl : IntPtr.Zero,
                                                         (IntPtr)hFilterID));
                    Filter.FilterId = hFilterID.Value;
                    filterList.Add(Filter);
                }
                return Filter.FilterId;
            }
        }
        /// <summary>
        /// Stops a message filter
        /// </summary>
        /// <param name="FilterId">Filter Id</param>
        public void StopMsgFilter(int FilterId)
        {
            lock (sync)
            {
                API.CheckResult(API.PTStopMsgFilter(channelId, FilterId));
                filterList.RemoveAll(f => f.FilterId == FilterId);
            }
        }
        /// <summary>
        /// Stops all message filters
        /// Alternative to ClearMsgFilters()
        /// </summary>
        public void StopAllMsgFilters()
        {
            lock (sync)
            {
                var filterIdList = filterList.Select(f => f.FilterId).ToList();
                foreach (int filterId in filterIdList)
                {
                    StopMsgFilter(filterId);
                }
            }
        }
        /// <summary>
        /// Gets a configuration parameter for the channel
        /// </summary>
        /// <param name="Parameter">Parameter to return</param>
        /// <returns>Parameter value</returns>
        public int GetConfig(Parameter Parameter)
        {
            using (HeapSConfigArray hSConfigArray = new HeapSConfigArray(new SConfig(Parameter, 0)))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.GET_CONFIG, (IntPtr)hSConfigArray, IntPtr.Zero));
                }
                return hSConfigArray[0].Value;
            }
        }
        /// <summary>
        /// Sets a configuration parameter for the channel
        /// </summary>
        /// <param name="Parameter">Parameter to set</param>
        /// <param name="Value">Parameter value</param>
        public void SetConfig(Parameter Parameter, int Value)
        {
            using (HeapSConfigArray hSConfigList = new HeapSConfigArray(new SConfig(Parameter, Value)))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.SET_CONFIG, (IntPtr)hSConfigList, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Gets a list of configuration parameters for the channel
        /// </summary>
        /// <param name="Parameter">List of parameters to get</param>
        /// <returns>Parameter list</returns>
        public SConfig[] GetConfig(SConfig[] SConfig)
        {
            using (HeapSConfigArray hSConfigArray = new HeapSConfigArray(SConfig))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.GET_CONFIG, (IntPtr)hSConfigArray, IntPtr.Zero));
                }
                return hSConfigArray.ToArray();
            }
        }
        /// <summary>
        /// Sets a list of configuration parameters for the channel
        /// </summary>
        /// <param name="Parameter">List of parameters to set</param>
        public void SetConfig(SConfig[] SConfig)
        {
            using (HeapSConfigArray hSConfigList = new HeapSConfigArray(SConfig))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.SET_CONFIG, (IntPtr)hSConfigList, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Empties the transmit buffer for this channel
        /// </summary>
        public void ClearTxBuffer()
        {
            lock (sync)
            {
                API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.CLEAR_TX_BUFFER, IntPtr.Zero, IntPtr.Zero));
            }
        }
        /// <summary>
        /// Empties the receive buffer for this channel
        /// </summary>
        public void ClearRxBuffer()
        {
            lock (sync)
            {
                API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.CLEAR_RX_BUFFER, IntPtr.Zero, IntPtr.Zero));
            }
        }
        /// <summary>
        /// Stops and clears any periodic messages that have been configured for this channel
        /// </summary>
        public void ClearPeriodicMsgs()
        {
            lock (sync)
            {
                API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.CLEAR_PERIODIC_MSGS, IntPtr.Zero, IntPtr.Zero));
            }
        }
        /// <summary>
        /// Stops and clears any message filters that have been configured for this channel
        /// </summary>
        public void ClearMsgFilters()
        {
            lock (sync)
            {
                API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.CLEAR_MSG_FILTERS, IntPtr.Zero, IntPtr.Zero));
                filterList.Clear();
            }
        }
        /// <summary>
        /// Stops and clears all functional message address filters configured for this channel
        /// </summary>
        public void ClearFunctMsgLookupTable()
        {
            lock (sync)
            {
                API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.CLEAR_FUNCT_MSG_LOOKUP_TABLE, IntPtr.Zero, IntPtr.Zero));
            }
        }
        /// <summary>
        /// Starts a functional message address filter for this channel
        /// </summary>
        /// <param name="Addr">Address to pass</param>
        public void AddToFunctMsgLookupTable(byte Addr)
        {
            using (HeapSByteArray hSByteArray = new HeapSByteArray(Addr))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, (IntPtr)hSByteArray, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Starts a list of functional message address filters for this channel
        /// </summary>
        /// <param name="AddressList">Address list to pass</param>
        public void AddToFunctMsgLookupTable(List<byte> AddressList)
        {
            using (HeapSByteArray hSByteArray = new HeapSByteArray(AddressList.ToArray()))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, (IntPtr)hSByteArray, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Stops and clears a single functional address message filter for this channel
        /// </summary>
        /// <param name="Addr">Address to remove</param>
        public void DeleteFromFunctMsgLookupTable(byte Addr)
        {
            using (HeapSByteArray hSByteArray = new HeapSByteArray(Addr))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, (IntPtr)hSByteArray, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Stops and clears a list of functional address filters for this channel
        /// </summary>
        /// <param name="AddressList">Address list to stop</param>
        public void DeleteFromFunctMsgLookupTable(IEnumerable<byte> AddressList)
        {
            using (HeapSByteArray hSByteArray = new HeapSByteArray(AddressList.ToArray()))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, (IntPtr)hSByteArray, IntPtr.Zero));
                }
            }
        }
        /// <summary>
        /// Performs a 5 baud handshake for ISO9141 initialization
        /// </summary>
        /// <param name="TargetAddress">Address to handshake with</param>
        /// <returns>byte[2]</returns>
        public byte[] FiveBaudInit(byte TargetAddress)
        {
            using (HeapSByteArray hInput = new HeapSByteArray(new byte[] { TargetAddress }))
            using (HeapSByteArray hOutput = new HeapSByteArray(new byte[2]))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.FIVE_BAUD_INIT, (IntPtr)hInput, (IntPtr)hOutput));
                }
                return hOutput.ToSByteArray();
            }
        }
        /// <summary>
        /// Performs a fast initialization sequence
        /// </summary>
        /// <param name="TxMessage"></param>
        /// <returns></returns>
        public Message FastInit(Message TxMessage)
        {
            using (HeapMessage hInput = new HeapMessage(ProtocolID, TxMessage.TxFlags, TxMessage.Data))
            using (HeapMessage hOutput = new HeapMessage(ProtocolID))
            {
                lock (sync)
                {
                    API.CheckResult(API.PTIoctl(channelId, (int)IOCTL.FAST_INIT, (IntPtr)hInput, (IntPtr)hOutput));
                }
                return hOutput.ToMessage();
            }
        }
        /// <summary>
        /// Turns on the programming voltage for the device
        /// </summary>
        /// <param name="PinNumber">Pin number</param>
        /// <param name="Voltage">voltage (mV)</param>
        public void SetProgrammingVoltage(Pin PinNumber, int Voltage)
        {
            Device.SetProgrammingVoltage(PinNumber, Voltage);
        }
        /// <summary>
        /// Measures the delivered programming voltage
        /// </summary>
        /// <returns>Voltage (mV)</returns>
        public int MeasureProgrammingVoltage()
        {
            return Device.MeasureProgrammingVoltage();
        }
        /// <summary>
        /// Measures the vehicle supply voltage
        /// </summary>
        /// <returns>Voltage (mV)</returns>
        public int MeasureBatteryVoltage()
        {
            return Device.MeasureBatteryVoltage();
        }

        protected override void DisposeManaged()
        {
            API.PTDisconnect(channelId);
            hJ2534MessageArray?.Dispose();
        }
    }
}
