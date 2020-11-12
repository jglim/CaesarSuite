using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAE.J2534;
using Caesar;

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

        public delegate void ConnectionStateChanged(string newStateDescription);
        public ConnectionStateChanged ConnectionStateChangeEvent;

        public string FriendlyName = "Simulation";
        public string FriendlyProfileName = "SIMULATION_PROFILE";

        public ConnectionState State;
        public byte[] CanIdentifier = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public ECU EcuContext;

        public enum ConnectionState 
        {
            PendingDeviceSelection,
            DeviceSelectedPendingChannelConnection,
            ChannelConnectedPendingEcuContact,
            EcuContacted
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
            FriendlyName = friendlyName;
            Console.WriteLine($"Initializing new connection to {friendlyName} using {fileName}");
            ConnectionAPI = APIFactory.GetAPI(fileName);
            State = ConnectionState.PendingDeviceSelection;
            ConnectionUpdateState();
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

        public void Connect(ECUInterfaceSubtype profile, ECU ecuContext)
        {
            EcuContext = ecuContext;
            if (ConnectionDevice is null) 
            {
                Console.WriteLine("No interfaces available : please select a J2534 interfaces from the Connection menu");
                return;
            }

            if (!profile.Qualifier.StartsWith("HSCAN"))
            {
                Console.WriteLine("Profile not supported: only HSCAN interfaces are supported.");
                return;
            }

            // actually start fixing up the connection
            if (ConnectionChannel != null) 
            {
                ConnectionChannel.Dispose();
            }
            FriendlyProfileName = profile.Qualifier;

            try
            {
                // only ISO15765 is supported
                // CAN_ID_BOTH : accepts 11-bit and 29-bit CAN messages
                // baudrate is specified by the ECU
                ConnectionChannel = ConnectionDevice.GetChannel(Protocol.ISO15765, (Baud)profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_BAUDRATE), ConnectFlag.CAN_ID_BOTH);
                Console.WriteLine($"Target voltage : {ConnectionChannel.MeasureBatteryVoltage()} mV");

                // setup ecu filter (mimicking vediamo's behavior)
                MessageFilter filter = new MessageFilter();
                CanIdentifier = BitConverter.GetBytes(profile.GetComParameterValue(ECUInterfaceSubtype.ParamName.CP_REQUEST_CANIDENTIFIER));
                // input byte data is in big-endian
                Array.Reverse(CanIdentifier);

                Console.WriteLine($"CAN Identifier: {BitUtility.BytesToHex(CanIdentifier)}");

                filter.StandardISO15765(CanIdentifier);
                ConnectionChannel.DefaultTxFlag = TxFlag.ISO15765_FRAME_PAD;

                /*
                // this should be equivalent to:
                
                MessageFilter FlowControlFilter = new MessageFilter()
                {
                    FilterType = Filter.FLOW_CONTROL_FILTER,
                    Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
                    Pattern = new byte[] { 0x00, 0x00, 0x07, 0xE8 },
                    FlowControl = new byte[] { 0x00, 0x00, 0x07, 0xE0 }
                };
                // note that the FRAME_PAD flag is also enabled (which is desired in the case of MED40, but might not be universal)
                // ISO15765_FRAME_PAD = 0x00000040; ComParams don't seem to have anything on this
                 */

                ConnectionChannel.StartMsgFilter(filter);

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

                ConnectionChannel.ClearRxBuffer();
                ConnectionChannel.ClearTxBuffer();

                // start an extended session
                SetEcuSessionState(EcuSessionType.Extended);

                State = ConnectionState.ChannelConnectedPendingEcuContact;
            }
            catch (Exception e) 
            {
                Console.WriteLine($"{e.Message}");
            }
            ConnectionUpdateState();
        }


        public enum EcuSessionType 
        {
            Normal = 0,
            Extended = 1,
            Programming = 2,
            Standby = 3,
        }

        public string[][] EcuSessionStrings = new string[][]
        {
            new string[] { "default", "normal" },
            new string[] { "extended" },
            new string[] { "programming" },
            new string[] { "standby" },
        };

        public void SetEcuSessionState(EcuSessionType newSessionLevel = EcuSessionType.Extended) 
        {
            if (EcuContext is null) 
            {
                Console.WriteLine($"{nameof(SetEcuSessionState)} : cannot proceed as {nameof(EcuContext)} is null");
                return;
            }
            foreach (DiagService diag in EcuContext.GlobalDiagServices) 
            {
                string diagNameLower = diag.Qualifier.ToLower();

                if (diag.DataClass_ServiceType == (int)DiagService.ServiceType.Session)
                {
                    bool diagIsPhysical = diagNameLower.Contains("physical");
                    bool diagIsFunctional = diagNameLower.Contains("functional");
                    // edit: apparently not always specified (wtf)
                    bool diagIsValid = true;  // choose between "physical" and "functional". no idea what they do, the dumps are identical

                    foreach (string sessionIdentifyingString in EcuSessionStrings[(int)newSessionLevel]) 
                    {
                        diagIsValid &= diagNameLower.Contains(sessionIdentifyingString);
                    }
                    if (diagIsValid) 
                    {
                        // diag.PrintDebug();
                        Console.WriteLine($"Switching ECU session state to {newSessionLevel}");
                        SendDiagRequest(diag);
                        break;
                    }
                }
            }
        }

        public void SendDiagRequest(DiagService diag) 
        {
            Console.WriteLine($"Running diagnostic request : {diag.Qualifier} ({BitUtility.BytesToHex(diag.RequestBytes, true)})");
            SendMessage(diag.RequestBytes);
        }

        public void SendMessage(IEnumerable<byte> message)
        {
            List<byte> packet = new List<byte>(CanIdentifier);
            packet.AddRange(message);
            string messageAsString = BitUtility.BytesToHex(packet.ToArray(), true);
            Console.WriteLine($"J2534 Write: {messageAsString}");

            if (ConnectionDevice is null)
            {
                Console.WriteLine($"Attempted to write into an invalid device, data: {messageAsString}");
                return;
            }
            if (ConnectionChannel is null) 
            {
                Console.WriteLine($"Attempted to write into an invalid channel, data: {messageAsString}");
                return;
            }

            try
            {
                ConnectionChannel.SendMessage(packet);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Exception while sending {messageAsString} : {ex.Message}");
            }
        }

        public void Connect()
        {
            // reference for j2534 library, not actually used
            MessageFilter FlowControlFilter = new MessageFilter()
            {
                FilterType = Filter.FLOW_CONTROL_FILTER,
                Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
                Pattern = new byte[] { 0x00, 0x00, 0x07, 0xE8 },
                FlowControl = new byte[] { 0x00, 0x00, 0x07, 0xE0 }
            };

            ConnectionChannel = ConnectionDevice.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.NONE);

            ConnectionChannel.StartMsgFilter(FlowControlFilter);
            Console.WriteLine($"Voltage is {ConnectionChannel.MeasureBatteryVoltage() / 1000}");
            ConnectionChannel.SendMessage(new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x01, 0x00 });
            GetMessageResults Response = ConnectionChannel.GetMessage();
        }

        public void TryCleanup() 
        {
            try
            {
                if (ConnectionChannel != null) 
                {
                    ConnectionChannel.Dispose();

                }
                if (ConnectionDevice != null) 
                {
                    ConnectionDevice.Dispose();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Cleanup issues: {ex.Message}");
            }
        }

        ~ECUConnection()
        {
            // using these throw exceptions during cleanup
            //ConnectionChannel?.Dispose();
            //ConnectionDevice?.Dispose();
            //ConnectionAPI?.Dispose();
        }
        
    }
}
