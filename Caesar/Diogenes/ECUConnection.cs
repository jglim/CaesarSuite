using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.J2534;
using Caesar;
using System.Timers;
using System.Diagnostics;
using Diogenes.DiagnosticProtocol;

namespace Diogenes
{
    /*
    example communication params:

    HSCAN_UDS_500

    CP_BAUDRATE : 500000 (0x7A120)

    CP_GLOBAL_REQUEST_CANIDENTIFIER : 1089 (0x441)
    CP_FUNCTIONAL_REQUEST_CANIDENTIFIER : 1089 (0x441)
    CP_REQUEST_CANIDENTIFIER : 2016 (0x7E0)
    CP_RESPONSE_CANIDENTIFIER : 2024 (0x7E8)
    
    CP_PARTNUMBERID : 0 (0x0)
    CP_PARTBLOCK : 1 (0x1)
    
    CP_HWVERSIONID : 0 (0x0)
    CP_SWVERSIONID : 0 (0x0)
    CP_SWVERSIONBLOCK : 1 (0x1)
    CP_SUPPLIERID : 61780 (0xF154)
    CP_SWSUPPLIERBLOCK : 1 (0x1)
    
    CP_ADDRESSMODE : 0 (0x0)
    CP_ADDRESSEXTENSION : 0 (0x0)
    
    CP_ROE_RESPONSE_CANIDENTIFIER : 0 (0x0)
    CP_USE_TIMING_RECEIVED_FROM_ECU : 0 (0x0)
    
    CP_STMIN_SUG : 0 (0x0)
    CP_BLOCKSIZE_SUG : 8 (0x8)
    
    CP_P2_TIMEOUT : 2100 (0x834)
    CP_P2_EXT_TIMEOUT_7F_78 : 4500 (0x1194)
    CP_S3_TP_PHYS_TIMER : 2000 (0x7D0)
    CP_S3_TP_FUNC_TIMER : 2000 (0x7D0)
    
    CP_BR_SUG : 0 (0x0)
    CP_CAN_TRANSMIT : 0 (0x0)
    CP_BS_MAX : 2000 (0x7D0)
    CP_CS_MAX : 2000 (0x7D0)
    
    CP_P2_EXT_TIMEOUT_7F_21 : 200 (0xC8)
    CPI_ROUTINECOUNTER : 30 (0x1E)
    CP_REQREPCOUNT : 3 (0x3)

     */
    public class ECUConnection
    {
        public API ConnectionAPI;
        public Device ConnectionDevice;
        public Channel ConnectionChannel;
        public string DriverPath = "";

        public delegate void ConnectionStateChanged(string newStateDescription);
        public ConnectionStateChanged ConnectionStateChangeEvent;

        public string FriendlyName = "Simulation";
        public string FriendlyProfileName = "SIMULATION_PROFILE";

        public ConnectionState State;
        public byte[] CanIdentifier = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public byte[] RxCanIdentifier = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public ECU EcuContext;

        public int ECUVariantID = 0;
        public bool VariantIsAvailable = false;
        public BaseProtocol ConnectionProtocol = null;


        //public bool UDSCapable = false;

        public StringBuilder CommunicationsLogHighLevel = new StringBuilder();

        // constant 2000 ms as recomended by the ISO 15765-3 standard (§6.3.3).
        // strangely it times out too quickly
        Timer TesterPresentTimer = new Timer(2000);

        public volatile bool TransactionInProgress = false;

        public enum ConnectionState
        {
            PendingDeviceSelection,
            DeviceSelectedPendingChannelConnection,
            ChannelConnectedPendingEcuContact,
            EcuContacted
        }

        public enum ConnectResponse 
        {
            OK,
            NoValidInterface,
            UnsupportedProtocol,
            FailedWithException
        }

        public ECUConnection()
        {
            // create a dummy connection
            FriendlyName = "SIMULATION";
            FriendlyProfileName = "SIMULATION_PROFILE";
            State = ConnectionState.PendingDeviceSelection;
            ConnectionUpdateState();
        }

        public ECUConnection(string fileName, string friendlyName)
        {
            DriverPath = fileName;

            // apparently AVDI embeds their hardware identifier in the device's name and path, which might be regarded as sensitive when sharing
            // this redacts it (somewhat) to help save some time for testers
            if (DriverIsAVDI())
            {
                FriendlyName = "AVDI-PT";
                Console.WriteLine($"Initializing new connection to {friendlyName}");
            }
            else
            {
                FriendlyName = friendlyName;
                Console.WriteLine($"Initializing new connection to {friendlyName} using {fileName}");
            }
            SetConnectionDefaults();

            ConnectionAPI = APIFactory.GetAPI(fileName);
            State = ConnectionState.PendingDeviceSelection;
            ConnectionUpdateState();
            TesterPresentTimer.Elapsed += TesterPresentTimer_Elapsed;
            TesterPresentTimer.Start();
            // RxTimer.Start();
        }


        private void TesterPresentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // normally we would wait until the session was switched to extended, but we don't know for sure. seems to be probably OK to send this as-is?
            if (State > ConnectionState.DeviceSelectedPendingChannelConnection)
            {
                // TesterPresent, expects 0x7E, 0x00
                SendMessage(new byte[] { 0x3E, 0x00 }, true);
            }
        }

        public void OpenDevice()
        {
            try
            {
                ConnectionDevice = ConnectionAPI.GetDevice();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            State = ConnectionState.DeviceSelectedPendingChannelConnection;
            ConnectionUpdateState();
        }

        private void ConnectionUpdateState()
        {
            string connectionState = "No interface selected (disconnected)";
            if (ConnectionDevice != null)
            {
                connectionState = $"Device: {FriendlyName} online";
                if (ConnectionChannel != null)
                {
                    connectionState = $"{connectionState}, connected with profile '{FriendlyProfileName}'";
                }
                else
                {
                    connectionState = $"{connectionState}, disconnected";
                }
            }
            ConnectionStateChangeEvent?.Invoke(connectionState);
        }

        public void SetConnectionDefaults() 
        {
            ECUVariantID = 0;
            VariantIsAvailable = false;
            ConnectionProtocol = null;
        }

        public ConnectResponse Connect(ECUInterfaceSubtype profile, ECU ecuContext)
        {
            State = ConnectionState.PendingDeviceSelection;
            EcuContext = ecuContext;
            if (ConnectionDevice is null)
            {
                Console.WriteLine("No interfaces available : please select a J2534 interface from the Connection menu");
                return ConnectResponse.NoValidInterface;
            }

            if (!profile.Qualifier.StartsWith("HSCAN"))
            {
                Console.WriteLine("Profile not supported: only HSCAN interfaces are supported.");
                return ConnectResponse.UnsupportedProtocol;
            }

            ConnectionProtocol = BaseProtocol.GetProtocol(profile.Qualifier);

            // actually start fixing up the connection
            if (ConnectionChannel != null)
            {
                ConnectionChannel.Dispose();
                ConnectionChannel = null;
            }
            FriendlyProfileName = profile.Qualifier;

            try
            {
                // only ISO15765 is supported
                // CAN_ID_BOTH : accepts 11-bit and 29-bit CAN messages
                // baudrate is specified by the ECU
                ConnectionChannel = ConnectionDevice.GetChannel(Protocol.ISO15765, (Baud)profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_BAUDRATE), ConnectFlag.CAN_ID_BOTH);
                Console.WriteLine($"Target voltage : {ConnectionChannel.MeasureBatteryVoltage()} mV");
                ConnectionChannel.DefaultTxFlag = TxFlag.ISO15765_FRAME_PAD;

                J2534SetFilters(profile);
                J2534SetConfig(profile);
                J2534FlushBuffers();


            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection failed with exception : {e.Message}");
                return ConnectResponse.FailedWithException;
            }

            // this chunk is repeated for AVDI devices; OpenPort2 does not care, Scanmatik refuses to continue if reconfigured without clearing prior filters
            // wrap the second attempt in a separate try block, so that we can suppress any potential filter errors
            if (DriverIsAVDI())
            {
                try
                {
                    ConnectionChannel.ClearMsgFilters();
                    J2534SetFilters(profile);
                    J2534SetConfig(profile);
                    J2534FlushBuffers();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AVDI Second config exception suppressed: {ex.Message}");
                }
            }
            
            State = ConnectionState.ChannelConnectedPendingEcuContact;

            ConnectionUpdateState();
            return ConnectResponse.OK;
        }

        public bool DriverIsAVDI() 
        {
            return DriverPath.ToUpper().EndsWith("ABRPT32.DLL");
        }

        public void J2534SetFilters(ECUInterfaceSubtype profile)
        {
            // setup ecu filter (mimicking vediamo's behavior)

            // convert the CBF's identifier integers to byte arrays
            CanIdentifier = BitConverter.GetBytes(profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_REQUEST_CANIDENTIFIER));
            RxCanIdentifier = BitConverter.GetBytes(profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_RESPONSE_CANIDENTIFIER));
            // input byte data is in big-endian
            Array.Reverse(CanIdentifier);
            Array.Reverse(RxCanIdentifier);

            MessageFilter filter = new MessageFilter();

            // Apparently in the EIS series, the RX identifier is !! NOT !! CanIdentifier+8 per ISO15765, so the automatic config in J2534-Sharp will fail

            // manually configure a ISO15765 filter
            filter.FilterType = Filter.FLOW_CONTROL_FILTER;
            filter.Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            filter.Pattern = RxCanIdentifier; // RX address, typically CanIdentifier+8, EXCEPT EIS
            filter.FlowControl = CanIdentifier; // TX address
            filter.TxFlags = TxFlag.ISO15765_FRAME_PAD;

            ConnectionChannel.StartMsgFilter(filter);
        }

        public void J2534SetConfig(ECUInterfaceSubtype profile)
        {
            List<SConfig> sconfigList = new List<SConfig>();

            List<Tuple<Parameter, ECUInterfaceSubtype.ParamName>> comPairs = new List<Tuple<Parameter, ECUInterfaceSubtype.ParamName>>();
            comPairs.Add(new Tuple<Parameter, ECUInterfaceSubtype.ParamName>(Parameter.STMIN_TX, ECUInterfaceSubtype.ParamName.CP_STMIN_SUG));
            comPairs.Add(new Tuple<Parameter, ECUInterfaceSubtype.ParamName>(Parameter.ISO15765_STMIN, ECUInterfaceSubtype.ParamName.CP_STMIN_SUG));
            comPairs.Add(new Tuple<Parameter, ECUInterfaceSubtype.ParamName>(Parameter.ISO15765_BS, ECUInterfaceSubtype.ParamName.CP_BLOCKSIZE_SUG));

            foreach (Tuple<Parameter, ECUInterfaceSubtype.ParamName> comPair in comPairs)
            {
                // apparently some are optional so we have to check for the presence of known comparams to configure the target j2534 device
                if (profile.GetComParameterValue(comPair.Item2, out int comValue))
                {
                    sconfigList.Add(new SConfig(comPair.Item1, comValue));
                }
            }
            ConnectionChannel.SetConfig(sconfigList.ToArray());
        }

        public void J2534FlushBuffers()
        {
            ConnectionChannel.ClearRxBuffer();
            ConnectionChannel.ClearTxBuffer();
        }

        public byte[] SendDiagRequest(DiagService diag) 
        {
            Console.WriteLine($"Running diagnostic request : {diag.Qualifier} ({BitUtility.BytesToHex(diag.RequestBytes, true)})");
            byte[] response = SendMessage(diag.RequestBytes);
            return response;
        }

        public byte[] SendMessage(IEnumerable<byte> message, bool quiet = false)
        {
            byte[] response = Array.Empty<byte>();

            // hack: "quiet" is almost always for tester presence, this prevents tester presence from interrupting existing transactions
            if (TransactionInProgress && quiet) 
            {
                return response;
            }
            TransactionInProgress = true;

            CommunicationsLogHighLevel.Append($"W {BitUtility.BytesToHex(message.ToArray(), true)}\r\n");

            // prepare data to send
            List<byte> packet = new List<byte>(CanIdentifier);
            packet.AddRange(message);
            string messageAsString = BitUtility.BytesToHex(message.ToArray(), true);
            if (!quiet)
            {
                // LogPacket(message, true);
            }

            if (ConnectionDevice is null)
            {
                Console.WriteLine($"[!] Attempted to write into an invalid device, data: {messageAsString}");
                TransactionInProgress = false;
                return response;
            }
            if (ConnectionChannel is null) 
            {
                Console.WriteLine($"[!] Attempted to write into an invalid channel, data: {messageAsString}");
                TransactionInProgress = false;
                return response;
            }

            // try to send the message
            try
            {
                ConnectionChannel.SendMessage(packet);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"[!] Exception while sending {messageAsString} : {ex.Message}");
                TransactionInProgress = false;
                return response;
            }

            // reset the heartbeat timer
            TesterPresentTimer.Stop();
            TesterPresentTimer.Start();


            // read response from ecu
            Stopwatch sw = new Stopwatch();
            sw.Start();

            bool waitingForPacket = true;
            while (waitingForPacket)
            {
                if (sw.ElapsedMilliseconds > 2500) // is this P2_TIMEOUT? initially picked 2000 since that is the minimum for tester presence 
                {
                    Console.WriteLine($"[!] Internally timed out: {messageAsString}");
                    sw.Stop();
                    break;
                }

                GetMessageResults readResult = ConnectionChannel.GetMessage();

                if (readResult.Result == ResultCode.STATUS_NOERROR)
                {
                    foreach (Message row in readResult.Messages)
                    {
                        if (row.Data.Length < 4)
                        {
                            Console.WriteLine($"[!] Discarding received message (invalid size):  {BitUtility.BytesToHex(row.Data, true)}");
                            continue;
                        }
                        byte[] identifier = row.Data.Take(4).ToArray();
                        if (!identifier.SequenceEqual(RxCanIdentifier))
                        {
                            if (identifier.SequenceEqual(CanIdentifier))
                            {
                                // quietly ignore if it is our can id, usually empty packet
                                continue;
                            }
                            Console.WriteLine($"[!] Discarding received message (unknown sender):  {BitUtility.BytesToHex(row.Data, true)} expects {BitUtility.BytesToHex(RxCanIdentifier, true)}");
                            continue;
                        }

                        // skip can identifier
                        byte[] rxMessageBody = row.Data.Skip(4).ToArray();

                        if (rxMessageBody.Length == 0)
                        {
                            continue;
                        }

                        if (!quiet)
                        {
                            // LogPacket(rxMessageBody, false);
                        }
                        response = rxMessageBody;
                        waitingForPacket = false;
                        break;
                        //Console.WriteLine($"ECU:  {BitUtility.BytesToHex(messageBody, true)}");
                    }
                }
                else if (readResult.Result == ResultCode.BUFFER_EMPTY) 
                {
                    // nothing in the mailbox, try again
                    Console.WriteLine($"[!] Retrying: empty buffer: {readResult.Result} for request {BitUtility.BytesToHex(message.ToArray())}");
                }
                else
                {
                    Console.WriteLine($"[!] Error in receive result: {readResult.Result}");
                    break;
                }
            }
            CommunicationsLogHighLevel.Append($"R {BitUtility.BytesToHex(response.ToArray(), true)}\r\n");
            
            TransactionInProgress = false;
            return response;
        }

        public void TryCleanup() 
        {
            try
            {
                if (FriendlyProfileName != "SIMULATION_PROFILE")
                {
                    Console.WriteLine("Cleaning up existing connection");
                }
                TesterPresentTimer.Enabled = false;
                if (ConnectionChannel != null) 
                {
                    ConnectionChannel.Dispose();
                    ConnectionChannel = null;
                }
                if (ConnectionDevice != null) 
                {
                    ConnectionDevice.Dispose();
                    ConnectionDevice = null;
                }
                if (ConnectionAPI != null) 
                {
                    ConnectionAPI.Dispose();
                    ConnectionAPI = null;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Cleanup issues: {ex.Message}");
            }
        }


    }
}
