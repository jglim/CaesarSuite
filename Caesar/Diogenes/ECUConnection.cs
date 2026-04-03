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
using Diogenes.SecurityAccess;

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
        public Simulation.SimulatedDevice SimulationChannel;

        public string DriverPath = "";

        public delegate void ConnectionStateChanged(string newStateDescription);
        public ConnectionStateChanged ConnectionStateChangeEvent;
        public delegate void ConnectionTrafficActivity(TrafficDirection direction);
        public event ConnectionTrafficActivity TrafficActivityEvent;

        public string FriendlyName = "Simulation";
        public string FriendlyProfileName = "SIMULATION_PROFILE";

        public ConnectionState State;
        public byte[] CanIdentifier = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public byte[] RxCanIdentifier = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public int InternalTimeout = 2000;
        public ECU EcuContext;

        public int ECUVariantID = 0;
        public bool VariantIsAvailable = false;
        public BaseProtocol ConnectionProtocol = null;

        // this should really go into a dedicated logger
        public StringBuilder CommunicationsLogHighLevel = new StringBuilder();
        public readonly object WriteLock = new object();
        public bool EnableTesterPresentLogging { get; set; } = false;

        // constant 2000 ms as recomended by the ISO 15765-3 standard (§6.3.3).
        public Timer TesterPresentTimer = new Timer(2000);

        // holds out-of-order, non testerpresent bytes since packets (apparently) can be received in any order
        Queue<byte[]> OutOfOrderBytesList = new Queue<byte[]>();
        private readonly object SendReceiveLock = new object();
        private readonly object ChannelWriteLock = new object();

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

        public enum TrafficDirection
        {
            Tx,
            Rx
        }

        public ECUConnection()
        {
            // create a dummy connection
            FriendlyName = "Simulation";
            FriendlyProfileName = "SIMULATION_PROFILE";
            State = ConnectionState.PendingDeviceSelection;
            ConnectionUpdateState();
        }

        public ECUConnection(string fileName, string friendlyName)
        {
            DriverPath = fileName;
            InternalTimeout = 2000;

            if (!IsSimulation())
            {
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
                ConnectionAPI = APIFactory.GetAPI(fileName);
            }
            else 
            {
                FriendlyName = "Simulation";
                ConnectionAPI = null;
            }

            SetConnectionDefaults();
            State = ConnectionState.PendingDeviceSelection;
            ConnectionUpdateState();
            TesterPresentTimer.Elapsed += TesterPresentTimer_Elapsed;
            TesterPresentTimer.Start();
        }

        public bool IsSimulation() 
        {
            return DriverPath == "SIMULATION";
        }

        private void LogTesterPresent(string message)
        {
            if (EnableTesterPresentLogging)
            {
                Console.WriteLine(message);
            }
        }

        private void NotifyTrafficActivity(TrafficDirection direction)
        {
            TrafficActivityEvent?.Invoke(direction);
        }

        public static List<Tuple<string, string>> GetAvailableJ2534NamesAndDrivers() 
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            foreach (APIInfo apiInfo in APIFactory.GetAPIList())
            {
                result.Add(new Tuple<string, string>(apiInfo.Name, apiInfo.Filename));
            }
#if DEBUG
            result.Add(new Tuple<string, string>("Simulation", "SIMULATION"));
#endif
            return result;
        }

        private void TesterPresentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // normally we would wait until the session was switched to extended, but we don't know for sure. seems to be probably OK to send this as-is?
            if (State > ConnectionState.DeviceSelectedPendingChannelConnection)
            {
                // TesterPresent, expects 0x7E, 0x00
                if (ConnectionProtocol != null)
                {
                    if (ConnectionProtocol.CanSendTesterPresentWhileBusy(this))
                    {
                        ConnectionProtocol.SendTesterPresent(this);
                        return;
                    }

                    bool lockTaken = false;
                    try
                    {
                        lockTaken = System.Threading.Monitor.TryEnter(SendReceiveLock);
                        if (!lockTaken)
                        {
                            Console.WriteLine("[LOG] TesterPresent: Skipping keep-alive because another request is already in flight");
                            return;
                        }

                        ConnectionProtocol.SendTesterPresent(this);
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            System.Threading.Monitor.Exit(SendReceiveLock);
                        }
                    }
                }
            }
        }

        public void OpenDevice()
        {
            if (!IsSimulation())
            {
                try
                {
                    Console.WriteLine($"[LOG] OpenDevice: Attempting to get device from API (driver: {DriverPath})");
                    ConnectionDevice = ConnectionAPI.GetDevice();
                    Console.WriteLine($"[LOG] OpenDevice: Device acquired successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LOG] OpenDevice FAILED: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[LOG] OpenDevice Stack: {ex.StackTrace}");
                }
            }
            State = ConnectionState.DeviceSelectedPendingChannelConnection;
            ConnectionUpdateState();
        }

        private void ConnectionUpdateState()
        {
            if (IsSimulation())
            {
                ConnectionStateChangeEvent?.Invoke($"Operating in simulation mode");
                return;
            }

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
            OutOfOrderBytesList.Clear();
        }

        public ConnectResponse Connect(ECUInterfaceSubtype profile, ECU ecuContext)
        {
            Console.WriteLine($"\r\n{'='+ new string('=', 59)}");
            Console.WriteLine($"[LOG] Connect: Starting connection attempt");
            Console.WriteLine($"[LOG] Connect: ECU Qualifier = {ecuContext.Qualifier}");
            Console.WriteLine($"[LOG] Connect: Profile Qualifier = {profile.Qualifier}");
            Console.WriteLine($"[LOG] Connect: Timestamp = {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

            // Dump all communication parameters from the profile
            Console.WriteLine($"[LOG] Connect: --- Communication Parameters ---");
            foreach (ComParameter param in profile.CommunicationParameters)
            {
                Console.WriteLine($"[LOG]   {param.ParamName} = {param.ComParamValue} (0x{param.ComParamValue:X})");
            }
            Console.WriteLine($"[LOG] Connect: --- End Parameters ---");

            State = ConnectionState.PendingDeviceSelection;
            EcuContext = ecuContext;

            if (!IsSimulation())
            {
                if (ConnectionDevice is null)
                {
                    Console.WriteLine("[LOG] Connect FAILED: No interfaces available : please select a J2534 interface from the Connection menu");
                    return ConnectResponse.NoValidInterface;
                }
                Console.WriteLine($"[LOG] Connect: Device is available (FriendlyName: {FriendlyName})");
            }

            // Accept HSCAN (High Speed CAN), LSCAN (Low Speed/Fault-Tolerant CAN), and MSCAN (Medium Speed CAN)
            bool isSupportedBus = profile.Qualifier.StartsWith("HSCAN") ||
                                  profile.Qualifier.StartsWith("LSCAN") ||
                                  profile.Qualifier.StartsWith("MSCAN");
            if (!isSupportedBus)
            {
                Console.WriteLine($"[LOG] Connect FAILED: Profile '{profile.Qualifier}' not supported: only HSCAN, LSCAN, MSCAN interfaces are supported.");
                return ConnectResponse.UnsupportedProtocol;
            }
            Console.WriteLine($"[LOG] Connect: Bus type accepted (profile prefix: '{profile.Qualifier.Split('_')[0]}')");

            ConnectionProtocol = BaseProtocol.GetProtocol(profile.Qualifier);
            Console.WriteLine($"[LOG] Connect: Protocol resolved to '{ConnectionProtocol.GetProtocolName()}' (from profile '{profile.Qualifier}')");

            // actually start fixing up the connection
            if (ConnectionChannel != null)
            {
                Console.WriteLine($"[LOG] Connect: Disposing existing channel before reconnect");
                ConnectionChannel.Dispose();
                ConnectionChannel = null;
            }
            FriendlyProfileName = profile.Qualifier;

            if (IsSimulation()) 
            {
                SimulationChannel = new Simulation.Simulated_CRD3();
                Console.WriteLine($"Connected (Simulation: {SimulationChannel.GetType().Name})");
            }
            else 
            {
                try
                {
                    int baudrate = profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_BAUDRATE);
                    Console.WriteLine($"[LOG] Connect: Opening ISO15765 channel at baudrate {baudrate} (0x{baudrate:X}) with CAN_ID_BOTH");
                    
                    // only ISO15765 is supported
                    // CAN_ID_BOTH : accepts 11-bit and 29-bit CAN messages
                    // baudrate is specified by the ECU
                    ConnectionChannel = ConnectionDevice.GetChannel(Protocol.ISO15765, (Baud)baudrate, ConnectFlag.CAN_ID_BOTH);
                    
                    int voltage = ConnectionChannel.MeasureBatteryVoltage();
                    Console.WriteLine($"[LOG] Connect: Channel opened OK. Target voltage: {voltage} mV ({voltage / 1000.0:F2} V)");
                    
                    ConnectionChannel.DefaultTxFlag = TxFlag.ISO15765_FRAME_PAD;
                    Console.WriteLine($"[LOG] Connect: DefaultTxFlag set to ISO15765_FRAME_PAD");

                    SetCANIdentifiers(profile);
                    J2534SetFilters();
                    J2534SetConfig(profile);
                    J2534FlushBuffers();
                    Console.WriteLine($"[LOG] Connect: J2534 channel fully configured");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[LOG] Connect FAILED with exception: {e.GetType().Name}: {e.Message}");
                    Console.WriteLine($"[LOG] Connect Stack: {e.StackTrace}");
                    return ConnectResponse.FailedWithException;
                }

                // this chunk is repeated for AVDI devices; OpenPort2 does not care, Scanmatik refuses to continue if reconfigured without clearing prior filters
                // wrap the second attempt in a separate try block, so that we can suppress any potential filter errors
                if (DriverIsAVDI())
                {
                    Console.WriteLine($"[LOG] Connect: AVDI device detected, performing second configuration pass");
                    try
                    {
                        ConnectionChannel.ClearMsgFilters();
                        SetCANIdentifiers(profile);
                        J2534SetFilters();
                        J2534SetConfig(profile);
                        J2534FlushBuffers();
                        Console.WriteLine($"[LOG] Connect: AVDI second pass completed OK");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LOG] Connect: AVDI second config exception suppressed: {ex.GetType().Name}: {ex.Message}");
                    }
                }
            }
            
            
            State = ConnectionState.ChannelConnectedPendingEcuContact;
            ConnectionUpdateState();
            Console.WriteLine($"[LOG] Connect: Connection state is now ChannelConnectedPendingEcuContact. Proceeding to protocol handler.");
            Console.WriteLine($"{'='+ new string('=', 59)}\r\n");
            return ConnectResponse.OK;
        }

        public bool DriverIsAVDI() 
        {
            return DriverPath.ToUpper().EndsWith("ABRPT32.DLL");
        }

        // this (and the overloaded variant) will be an issue when operating in gateway mode;
        // gateway mode will likely require the two separate can ids to be defined
        public void SetCANIdentifiers(ECUInterfaceSubtype profile)
        {
            int txId = profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_REQUEST_CANIDENTIFIER);
            int rxId = profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_RESPONSE_CANIDENTIFIER);
            Console.WriteLine($"[LOG] SetCANIdentifiers: TX (Request) CAN ID = 0x{txId:X} ({txId}), RX (Response) CAN ID = 0x{rxId:X} ({rxId})");

            // Also log functional/global request IDs if available
            if (profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_FUNCTIONAL_REQUEST_CANIDENTIFIER, out int funcId))
            {
                Console.WriteLine($"[LOG] SetCANIdentifiers: Functional Request CAN ID = 0x{funcId:X} ({funcId})");
            }
            if (profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_GLOBAL_REQUEST_CANIDENTIFIER, out int globalId))
            {
                Console.WriteLine($"[LOG] SetCANIdentifiers: Global Request CAN ID = 0x{globalId:X} ({globalId})");
            }

            SetCANIdentifiers(txId, rxId);
        }
        public void SetCANIdentifiers(int canIdentifier, int rxCanIdentifier)
        {
            // convert the CBF's identifier integers to byte arrays
            CanIdentifier = BitConverter.GetBytes(canIdentifier);
            RxCanIdentifier = BitConverter.GetBytes(rxCanIdentifier);
            // input byte data is in big-endian
            Array.Reverse(CanIdentifier);
            Array.Reverse(RxCanIdentifier);
            Console.WriteLine($"[LOG] SetCANIdentifiers: TX bytes = [{BitUtility.BytesToHex(CanIdentifier, true)}], RX bytes = [{BitUtility.BytesToHex(RxCanIdentifier, true)}]");
        }

        public void J2534SetFilters()
        {
            Console.WriteLine($"[LOG] J2534SetFilters: Configuring ISO15765 flow control filter");
            // setup ecu filter (mimicking vediamo's behavior)
            MessageFilter filter = new MessageFilter();
            // Apparently in the EIS series, the RX identifier is !! NOT !! CanIdentifier+8 per ISO15765, so the automatic config in J2534-Sharp will fail

            // manually configure a ISO15765 filter
            filter.FilterType = Filter.FLOW_CONTROL_FILTER;
            filter.Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            filter.Pattern = RxCanIdentifier; // RX address, typically CanIdentifier+8, EXCEPT EIS
            filter.FlowControl = CanIdentifier; // TX address
            filter.TxFlags = TxFlag.ISO15765_FRAME_PAD;

            Console.WriteLine($"[LOG] J2534SetFilters: FilterType = FLOW_CONTROL_FILTER");
            Console.WriteLine($"[LOG] J2534SetFilters: Mask     = [{BitUtility.BytesToHex(filter.Mask, true)}]");
            Console.WriteLine($"[LOG] J2534SetFilters: Pattern  = [{BitUtility.BytesToHex(filter.Pattern, true)}] (RX/Response CAN ID)");
            Console.WriteLine($"[LOG] J2534SetFilters: FlowCtrl = [{BitUtility.BytesToHex(filter.FlowControl, true)}] (TX/Request CAN ID)");
            Console.WriteLine($"[LOG] J2534SetFilters: TxFlags  = ISO15765_FRAME_PAD");

            ConnectionChannel.ClearMsgFilters();
            Console.WriteLine($"[LOG] J2534SetFilters: Cleared existing filters");
            ConnectionChannel.StartMsgFilter(filter);
            Console.WriteLine($"[LOG] J2534SetFilters: Filter applied successfully");
        }

        public void J2534SetConfig(ECUInterfaceSubtype profile)
        {
            Console.WriteLine($"[LOG] J2534SetConfig: Configuring J2534 channel parameters");
            List<SConfig> sconfigList = new List<SConfig>();

            bool TryGetProfileValue(string parameterName, out int value)
            {
                ComParameter parameter = profile.GetComParameterByName(parameterName);
                if (parameter is null)
                {
                    value = 0;
                    return false;
                }

                value = parameter.ComParamValue;
                return true;
            }

            void AddConfig(Parameter parameter, int value, string sourceDescription)
            {
                Console.WriteLine($"[LOG] J2534SetConfig:   {parameter} = {value} ({sourceDescription})");
                sconfigList.Add(new SConfig(parameter, value));
            }

            if (TryGetProfileValue("CP_C_REQ_SUG", out int requestSeparationTime))
            {
                AddConfig(Parameter.STMIN_TX, requestSeparationTime, "from CP_C_REQ_SUG");
            }
            else
            {
                AddConfig(Parameter.STMIN_TX, 0, "defaulted to 0 (CP_C_REQ_SUG not found)");
            }

            if (profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_BLOCKSIZE_SUG, out int blockSizeSuggestion))
            {
                AddConfig(Parameter.ISO15765_BS, blockSizeSuggestion, $"from {ECUInterfaceSubtype.ParamName.CP_BLOCKSIZE_SUG}");
            }
            else
            {
                AddConfig(Parameter.ISO15765_BS, 8, "defaulted to 8 (CP_BLOCKSIZE_SUG not found)");
            }

            if (profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_STMIN_SUG, out int stminSuggestion))
            {
                AddConfig(Parameter.ISO15765_STMIN, stminSuggestion, $"from {ECUInterfaceSubtype.ParamName.CP_STMIN_SUG}");
            }
            else
            {
                Console.WriteLine($"[LOG] J2534SetConfig:   {Parameter.ISO15765_STMIN} skipped (param {ECUInterfaceSubtype.ParamName.CP_STMIN_SUG} not found in profile)");
            }

            ConnectionChannel.SetConfig(sconfigList.ToArray());
            Console.WriteLine($"[LOG] J2534SetConfig: Applied {sconfigList.Count} config parameter(s)");
        }

        public void J2534FlushBuffers()
        {
            ConnectionChannel.ClearRxBuffer();
            ConnectionChannel.ClearTxBuffer();
            Console.WriteLine($"[LOG] J2534FlushBuffers: RX and TX buffers cleared");
        }

        public byte[] SendDiagRequest(DiagService diag) 
        {
            Console.WriteLine($"Running diagnostic request : {diag.Qualifier} ({BitUtility.BytesToHex(diag.RequestBytes, true)})");
            byte[] response = SendMessage(diag.RequestBytes);
            return response;
        }

        public void ExecUserDiagJob(byte[] request, DiagService diagService)
        {
            Console.WriteLine($"\r\n{diagService.Qualifier}");
            byte[] response = SendMessage(request);
            foreach (List<DiagPreparation> outputPreparationSet in diagService.OutputPreparations)
            {
                foreach (DiagPreparation outputPreparation in outputPreparationSet)
                {
                    //outputPreparation.PrintDebug();
                    DiagPresentation presentation = outputPreparation.ParentECU.GlobalPresentations[outputPreparation.PresPoolIndex];
                    // presentation.PrintDebug();
                    Console.WriteLine($"{presentation.InterpretData(response, outputPreparation)}");
                }
            }
            // check if the response was an ECU seed
            if ((ConnectionProtocol?.SupportsUnlocking() ?? false) && (response.Length >= 2) && (response[0] == 0x67))
            {
                SecurityAutoLogin.ReceiveSecurityResponse(response, diagService.ParentECU, this);
            }
        }

        public byte[] SendMessage(IEnumerable<byte> message, bool testerPresenceRequest = false)
        {
            lock (SendReceiveLock)
            {
                LogWrite(message);
                if (IsSimulation()) 
                {
                    if (SimulationChannel is null) 
                    {
                        throw new Exception("Simulation channel was not initialized");
                    }
                    NotifyTrafficActivity(TrafficDirection.Tx);
                    byte[] simResponse = SimulationChannel.ReceiveRequest(message);
                    if (simResponse.Length > 0)
                    {
                        NotifyTrafficActivity(TrafficDirection.Rx);
                    }
                    LogRead(simResponse);
                    return simResponse;
                }

                byte[] response = Array.Empty<byte>();

                // prepare data to send
                List<byte> packet = new List<byte>(CanIdentifier);
                packet.AddRange(message);
                string messageAsString = BitUtility.BytesToHex(message.ToArray(), true);
                string messageWithText = BitUtility.BytesToHexWithPrintableText(message.ToArray(), true);
                if (!testerPresenceRequest)
                {
                    Console.WriteLine($"[LOG] TX >> [{BitUtility.BytesToHex(packet.ToArray(), true)}] (payload: {messageWithText}) @ {DateTime.Now:HH:mm:ss.fff}");
                }
                else
                {
                    LogTesterPresent($"[LOG] TX >> [TesterPresent] {messageWithText} @ {DateTime.Now:HH:mm:ss.fff}");
                }

                if (ConnectionDevice is null)
                {
                    Console.WriteLine($"[!] Attempted to write into an invalid device, data: {messageAsString}");
                    return response;
                }
                if (ConnectionChannel is null) 
                {
                    Console.WriteLine($"[!] Attempted to write into an invalid channel, data: {messageAsString}");
                    return response;
                }

                // try to send the message
                try
                {
                    lock (ChannelWriteLock)
                    {
                        ConnectionChannel.SendMessage(packet);
                    }
                    NotifyTrafficActivity(TrafficDirection.Tx);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"[!] Exception while sending {messageAsString} : {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[!] Send Stack: {ex.StackTrace}");
                    return response;
                }

                // reset the heartbeat timer; I don't know the actual behavior per the spec
                TesterPresentTimer.Stop();
                TesterPresentTimer.Start();

                // this loop catches 7F xx 78 reqeuests from the ecu, where it needs more time to complete an action
                bool responseIsValid = false;
                while (!responseIsValid)
                {
                    response = ReadResponse(messageAsString, testerPresenceRequest);
                    responseIsValid = !IsECURequestingForWait(response);
                    if (!responseIsValid) 
                    {
                        Console.WriteLine($"[LOG] RX: ECU requesting wait (7F xx 78), retrying...");
                    }
                }

                return response;
            }
        }

        public void SendMessageWithoutResponse(IEnumerable<byte> message, int canIdentifier, bool testerPresenceRequest = false)
        {
            byte[] payload = message.ToArray();
            LogWrite(payload);

            if (IsSimulation())
            {
                if (SimulationChannel != null)
                {
                    NotifyTrafficActivity(TrafficDirection.Tx);
                    SimulationChannel.ReceiveRequest(payload);
                }
                return;
            }

            if (ConnectionDevice is null)
            {
                Console.WriteLine($"[!] Attempted to write into an invalid device, data: {BitUtility.BytesToHex(payload, true)}");
                return;
            }
            if (ConnectionChannel is null)
            {
                Console.WriteLine($"[!] Attempted to write into an invalid channel, data: {BitUtility.BytesToHex(payload, true)}");
                return;
            }

            byte[] customIdentifier = BitConverter.GetBytes(canIdentifier);
            Array.Reverse(customIdentifier);

            List<byte> packet = new List<byte>(customIdentifier);
            packet.AddRange(payload);
            string messageAsString = BitUtility.BytesToHex(payload, true);
            string messageWithText = BitUtility.BytesToHexWithPrintableText(payload, true);

            if (!testerPresenceRequest)
            {
                Console.WriteLine($"[LOG] TX >> [{BitUtility.BytesToHex(packet.ToArray(), true)}] (payload: {messageWithText}) @ {DateTime.Now:HH:mm:ss.fff} [no response expected]");
            }
            else
            {
                LogTesterPresent($"[LOG] TX >> [TesterPresent] {messageWithText} @ {DateTime.Now:HH:mm:ss.fff} [CAN 0x{canIdentifier:X}, no response expected]");
            }

            try
            {
                lock (ChannelWriteLock)
                {
                    ConnectionChannel.SendMessage(packet);
                }
                NotifyTrafficActivity(TrafficDirection.Tx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Exception while sending fire-and-forget message {messageAsString} on CAN 0x{canIdentifier:X} : {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"[!] Send Stack: {ex.StackTrace}");
                return;
            }

            TesterPresentTimer.Stop();
            TesterPresentTimer.Start();
        }

        public byte[] ReadResponse(string originalMessageAsStringForDebug, bool testerPresenceRequest) 
        {
            byte[] response = Array.Empty<byte>();
            // before reading from ecu, check if there were out-of-order responses that were stored
            while (OutOfOrderBytesList.Count > 0)
            {
                byte[] queued = OutOfOrderBytesList.Dequeue();
                if (ConnectionProtocol != null && ConnectionProtocol.IsResponseToTesterPresent(queued))
                {
                    LogTesterPresent($"[LOG] RX: Discarding queued TesterPresent response [{BitUtility.BytesToHex(queued, true)}]");
                    if (testerPresenceRequest)
                    {
                        return Array.Empty<byte>();
                    }
                    continue;
                }
                Console.WriteLine($"[LOG] RX << [Out-of-order dequeue] {BitUtility.BytesToHexWithPrintableText(queued, true)} @ {DateTime.Now:HH:mm:ss.fff}");
                return queued;
            }

            // read response from ecu
            Stopwatch sw = new Stopwatch();
            sw.Start();

            bool waitingForPacket = true;
            int pollCount = 0;
            while (waitingForPacket)
            {
                if (sw.ElapsedMilliseconds > 18500) // is this P2_TIMEOUT? initially picked 2000 since that is the minimum for tester presence 
                {
                    Console.WriteLine($"[!] Internally timed out after {sw.ElapsedMilliseconds}ms waiting for response to: {originalMessageAsStringForDebug} (polled {pollCount} times)");
                    sw.Stop();
                    break;
                }

                GetMessageResults readResult = ConnectionChannel.GetMessage();
                pollCount++;

                if (readResult.Result == ResultCode.STATUS_NOERROR)
                {
                    List<Message> receivedMessages = readResult.Messages.ToList();
                    List<Message> visibleMessages = receivedMessages
                        .Where(row =>
                        {
                            string rxStatusText = row.RxStatus.ToString();
                            return !(rxStatusText.Contains("TX_MSG_TYPE") || rxStatusText.Contains("TX_INDICATION"));
                        })
                        .ToList();

                    if (visibleMessages.Count > 0)
                    {
                        Console.WriteLine($"[LOG] RX: GetMessage returned {receivedMessages.Count} message(s) after {sw.ElapsedMilliseconds}ms");
                    }

                    foreach (Message row in receivedMessages)
                    {
                        string rxStatusText = row.RxStatus.ToString();
                        if (rxStatusText.Contains("TX_MSG_TYPE") || rxStatusText.Contains("TX_INDICATION"))
                        {
                            // Some J2534 drivers surface our own writes as TX indications; they are not ECU responses.
                            continue;
                        }

                        Console.WriteLine($"[LOG] RX RAW: [{BitUtility.BytesToHex(row.Data, true)}] (len={row.Data.Length}, RxStatus={row.RxStatus}, Timestamp={row.Timestamp})");

                        if (row.Data.Length < 4)
                        {
                            Console.WriteLine($"[!] Discarding received message (invalid size, need >=4 bytes):  {BitUtility.BytesToHex(row.Data, true)}");
                            continue;
                        }
                        byte[] identifier = row.Data.Take(4).ToArray();
                        if (!identifier.SequenceEqual(RxCanIdentifier))
                        {
                            if (identifier.SequenceEqual(CanIdentifier))
                            {
                                Console.WriteLine($"[LOG] RX: Ignoring echo of own TX CAN ID [{BitUtility.BytesToHex(identifier, true)}]");
                                continue;
                            }
                            Console.WriteLine($"[!] Discarding received message (unknown sender CAN ID [{BitUtility.BytesToHex(identifier, true)}]):  full data [{BitUtility.BytesToHex(row.Data, true)}], expected RX ID [{BitUtility.BytesToHex(RxCanIdentifier, true)}]");
                            continue;
                        }

                        // skip can identifier
                        byte[] rxMessageBody = row.Data.Skip(4).ToArray();

                        if (rxMessageBody.Length == 0)
                        {
                            Console.WriteLine($"[LOG] RX: Empty payload after CAN ID strip, continuing");
                            continue;
                        }

                        response = rxMessageBody;
                        NotifyTrafficActivity(TrafficDirection.Rx);
                        Console.WriteLine($"[LOG] RX << [{BitUtility.BytesToHexWithPrintableText(response, true)}] @ {DateTime.Now:HH:mm:ss.fff} (after {sw.ElapsedMilliseconds}ms)");

                        LogRead(response);
                        // if it's a tester presence response, skip it and retry for another packet
                        if (ConnectionProtocol.IsResponseToTesterPresent(response))
                        {
                            // if it is a TP request, we can exit now
                            if (testerPresenceRequest)
                            {
                                LogTesterPresent("[LOG] RX: TesterPresent response received (expected)");
                                return Array.Empty<byte>();
                            }
                            else
                            {
                                // accidentally received an out-of-order TP response, silently discard it
                                LogTesterPresent("[LOG] RX: Discarding out-of-order TesterPresent response");
                                continue;
                            }
                        }
                        else
                        {
                            // TP receiving someone else's valid command, push it back into the queue and exit
                            if (testerPresenceRequest)
                            {
                                Console.WriteLine($"[LOG] RX: Non-TP response during TP poll, queuing for later");
                                OutOfOrderBytesList.Enqueue(response);
                                return Array.Empty<byte>();
                            }
                            else
                            {
                                // received a packet normally, check in parent caller if the ECU was asking us to wait
                                waitingForPacket = false;
                                break;
                            }
                        }
                    }
                }
                else if (readResult.Result == ResultCode.BUFFER_EMPTY)
                {
                    // nothing in the mailbox, try again
                    // Log every 500th poll to avoid flooding, but also log the first few
                    if (pollCount <= 3 || pollCount % 500 == 0)
                    {
                        Console.WriteLine($"[LOG] RX: Buffer empty (poll #{pollCount}, {sw.ElapsedMilliseconds}ms elapsed)");
                    }
                }
                else
                {
                    Console.WriteLine($"[!] Error in receive result: {readResult.Result} after {sw.ElapsedMilliseconds}ms");
                    break;
                }
            }
            if (response.Length == 0 && !testerPresenceRequest)
            {
                Console.WriteLine($"[LOG] RX: No response received for request {originalMessageAsStringForDebug}");
            }
            return response;
        }

        public bool IsECURequestingForWait(byte[] response)
        {
            if ((response.Length == 3) && (response[0] == 0x7F) && (response[2] == 0x78))
            {
                Console.WriteLine($"[LOG] IsECURequestingForWait: Received NR 7F {response[1]:X2} 78 (Response Pending) for service 0x{response[1]:X2}");
                if ((ConnectionProtocol.GetProtocolName() == "UDS") || (ConnectionProtocol.GetProtocolName() == "KW2C3PE"))
                {
                    // ecu requesting for more time
                    return true;
                }
                else 
                {
                    Console.WriteLine($"[LOG] IsECURequestingForWait: NR looks like a wait request, but protocol '{ConnectionProtocol.GetProtocolName()}' does not seem to support it.");
                }
            }
            else if ((response.Length >= 3) && (response[0] == 0x7F))
            {
                Console.WriteLine($"[LOG] Negative Response: 7F {response[1]:X2} {response[2]:X2}");
            }
            return false;
        }

        public void LogRead(IEnumerable<byte> inBuffer)
        {
            lock (WriteLock) 
            {
                CommunicationsLogHighLevel.Append($"R {BitUtility.BytesToHex(inBuffer.ToArray(), true)}\r\n");
            }
        }
        public void LogWrite(IEnumerable<byte> inBuffer)
        {
            lock (WriteLock)
            {
                CommunicationsLogHighLevel.Append($"W {BitUtility.BytesToHex(inBuffer.ToArray(), true)}\r\n");
            }
        }

        public void TryCleanup()
        {
            try
            {
                if (FriendlyProfileName != "SIMULATION_PROFILE")
                {
                    Console.WriteLine("Cleaning up existing connection");
                }
                if (ConnectionChannel != null) 
                {
                    if (ConnectionProtocol != null)
                    {
                        ConnectionProtocol?.ConnectionClosingHandler(this);
                    }
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
                TesterPresentTimer.Stop();
                TesterPresentTimer.Enabled = false;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Cleanup issues: {ex.Message}");
            }
        }


    }
}
