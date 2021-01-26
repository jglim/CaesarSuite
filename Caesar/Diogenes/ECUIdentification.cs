using Caesar;
using SAE.J2534;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    // this whole class makes the assumption that MB is consistent with their request identifiers (which thankfully seems generally true)
    public class ECUIdentification
    {
        // FIN = Fahrzeug-Identifizierungs-Nummer
        // VIN = Vehicle Identification Number
        public static bool TryReadChassisNumber(ECUConnection connection, out string chassisNumber) 
        {
            chassisNumber = "";

            // we need a existing hardware connection to a J2534 device
            if (connection is null) 
            {
                return false;
            }
            if (connection.IsSimulation()) 
            {
                return false;
            }
            if (connection.ConnectionDevice is null)
            {
                // no valid j2534 device initialized yet, can't do anything
                return false;
            }

            bool connectionWasEstablished = connection.ConnectionProtocol != null;

            // if the connection is already open, we can use the protocol's vin query
            int[] baudratesToTest = new int[] { 500000, 800000 };
            foreach (int baudrateToTest in baudratesToTest)
            {
                // if there is an established connection (i.e. already connected and identified a target, do not reconfigure the connection
                if (!connectionWasEstablished) 
                {
                    SetupConnectionForBaud(connection, baudrateToTest);
                }
                
                // try KW2C3PE first, since it is a legacy protocol; UDS seems to be somewhat aware, and deliberately avoids known KW2C3PE identifiers
                if (TryReadKW2C3PEChassisNumber(connection, out string KW2C3PEChassisNumber))
                {
                    chassisNumber = KW2C3PEChassisNumber;
                    return true;
                }
                
                if (TryReadUDSChassisNumber(connection, out string udsChassisNumber))
                {
                    chassisNumber = udsChassisNumber;
                    return true;
                }
                
                // if there's an established connection that failed both UDS/KW2C3PE, the loop is no longer required
                if (connection.ConnectionProtocol != null) 
                {
                    return false;
                }
            }

            // if a connection was unavailable, it would have been established earlier on; clean it up
            if (!connectionWasEstablished) 
            {
                connection.TryCleanup();
                connection = null;
            }

            return false;
        }

        public static bool TryReadUDSChassisNumber(ECUConnection connection, out string chassisNumber)
        {
            chassisNumber = "";
            byte[] response = connection.SendMessage(new byte[] { 0x22, 0xF1, 0xA0 });
            if (response.Length != 20) // 62 F1 A0 + 17byte vin
            {
                return false;
            }
            if (response[0] != 0x62)
            {
                return false;
            }
            chassisNumber = Encoding.ASCII.GetString(response.Skip(3).ToArray());
            return true;
        }
        public static bool TryReadKW2C3PEChassisNumber(ECUConnection connection, out string chassisNumber)
        {
            chassisNumber = "";
            byte[] response = connection.SendMessage(new byte[] { 0x1A, 0x90 }); // VIN current
            if (response.Length != 19) // 5A 90 + 17byte vin
            {
                return false;
            }
            if (response[0] != 0x5A)
            {
                return false;
            }
            chassisNumber = Encoding.ASCII.GetString(response.Skip(2).ToArray());
            return true;
        }

        // eventually move this to ecuconnection, since it touches J2534Sharp and this should be kept local
        public static void SetupConnectionForBaud(ECUConnection connection, int baud) 
        {
            if (connection.ConnectionChannel != null)
            {
                connection.ConnectionChannel.ClearMsgFilters(); // this bit here for good luck since there are some misbehaving j2534 devices
                connection.ConnectionChannel.Dispose();
                connection.ConnectionChannel = null;
            }
            try
            {
                // create a new channel; the protocol is almost always ISO15765
                connection.ConnectionChannel = connection.ConnectionDevice.GetChannel(Protocol.ISO15765, (Baud)baud, ConnectFlag.CAN_ID_BOTH);
                connection.ConnectionProtocol = new DiagnosticProtocol.UnsupportedProtocol();
                connection.ConnectionChannel.DefaultTxFlag = TxFlag.ISO15765_FRAME_PAD;
                connection.InternalTimeout = 100;

                // setup filters
                connection.SetCANIdentifiers(0x7E0, 0x7E8);
                connection.J2534SetFilters();

                // setup config with some sane defaults since we are guessing our target
                List<SConfig> sconfigList = new List<SConfig>();
                sconfigList.Add(new SConfig(Parameter.STMIN_TX, 0));
                sconfigList.Add(new SConfig(Parameter.ISO15765_STMIN, 0));
                sconfigList.Add(new SConfig(Parameter.ISO15765_BS, 8));
                connection.ConnectionChannel.SetConfig(sconfigList.ToArray());
                
                // flush the buffer
                connection.ConnectionChannel.ClearRxBuffer();
                connection.ConnectionChannel.ClearTxBuffer();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[!] ECUIdentification exception in SetupConnectionForBaud: {e.Message}");
            }
        }
    }
}
