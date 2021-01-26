using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Caesar.DTC;

namespace Diogenes.DiagnosticProtocol
{
    public class UDS : BaseProtocol
    {
        private static string[] NegativeResponseDescriptions = new string[] { };
        private static Dictionary<int, string> MessageDescriptions = new Dictionary<int, string>();

        private static string[] GetNegativeResponseDescriptions() 
        {
            if (NegativeResponseDescriptions.Length == 0) 
            {
                NegativeResponseDescriptions = new string[0xFF];
                for (int i = 0; i < 0xFF; i++) 
                {
                    NegativeResponseDescriptions[i] = "ISO SAE Reserved";
                }
                for (int i = 0x38; i <= 0x4F; i++)
                {
                    NegativeResponseDescriptions[i] = "Reserved By Extended Data Link Security Document";
                }
                for (int i = 0x94; i <= 0xEF; i++)
                {
                    NegativeResponseDescriptions[i] = "Reserved For Specific Conditions Not Correct";
                }
                for (int i = 0xF0; i <= 0xFE; i++)
                {
                    NegativeResponseDescriptions[i] = "Vehicle Manufacturer Specific Conditions Not Correct";
                }
                NegativeResponseDescriptions[0x00] = "Positive Response";
                NegativeResponseDescriptions[0x10] = "General Reject";
                NegativeResponseDescriptions[0x11] = "Service Not Supported";
                NegativeResponseDescriptions[0x12] = "Sub-function Not Supported";
                NegativeResponseDescriptions[0x13] = "Incorrect Message Length Or Invalid Format";
                NegativeResponseDescriptions[0x14] = "Response Too Long";
                NegativeResponseDescriptions[0x21] = "Busy Repeat Request";
                NegativeResponseDescriptions[0x22] = "Conditions Not Correct";
                NegativeResponseDescriptions[0x24] = "Request Sequence Error";
                NegativeResponseDescriptions[0x25] = "No Response From Sub-net Component";
                NegativeResponseDescriptions[0x26] = "Failure Prevents Execution Of Requested Action";
                NegativeResponseDescriptions[0x31] = "Request Out Of Range";
                NegativeResponseDescriptions[0x33] = "Security Access Denied";
                NegativeResponseDescriptions[0x35] = "Invalid Key";
                NegativeResponseDescriptions[0x36] = "Exceed Number Of Attempts";
                NegativeResponseDescriptions[0x37] = "Required Time Delay Not Expired";
                NegativeResponseDescriptions[0x70] = "Upload Download Not Accepted";
                NegativeResponseDescriptions[0x71] = "Transfer Data Suspended";
                NegativeResponseDescriptions[0x72] = "General Programming Failure";
                NegativeResponseDescriptions[0x73] = "Wrong Block Sequence Counter";
                NegativeResponseDescriptions[0x78] = "Request Correctly Received-Response Pending";
                NegativeResponseDescriptions[0x7E] = "Sub-function Not Supported In Active Session";
                NegativeResponseDescriptions[0x7F] = "Service Not Supported In Active Session";
                NegativeResponseDescriptions[0x81] = "Rpm Too High";
                NegativeResponseDescriptions[0x82] = "Rpm Too Low";
                NegativeResponseDescriptions[0x83] = "Engine Is Running";
                NegativeResponseDescriptions[0x84] = "Engine Is Not Running";
                NegativeResponseDescriptions[0x85] = "Engine Run Time Too Low";
                NegativeResponseDescriptions[0x86] = "Temperature is Too High";
                NegativeResponseDescriptions[0x87] = "Temperature is Too Low";
                NegativeResponseDescriptions[0x88] = "Vehicle Speed is Too High";
                NegativeResponseDescriptions[0x89] = "Vehicle Speed is Too Low";
                NegativeResponseDescriptions[0x8A] = "Throttle/Pedal is Too High";
                NegativeResponseDescriptions[0X8B] = "Throttle/Pedal IS Too Low";
                NegativeResponseDescriptions[0X8C] = "Transmission Range Is Not In Neutral";
                NegativeResponseDescriptions[0x8D] = "Transmission Range is Not In Gear";
                NegativeResponseDescriptions[0x8F] = "Brake Switch(es) Not Closed (Brake Pedal not pressed or not applied)";
                NegativeResponseDescriptions[0x90] = "Shifter Lever Not In Park";
                NegativeResponseDescriptions[0x91] = "Torque Converter Clutch is Locked";
                NegativeResponseDescriptions[0x92] = "Voltage is Too High";
                NegativeResponseDescriptions[0x93] = "Voltage Too Low";
            }

            return NegativeResponseDescriptions;
        }

        private static Dictionary<int, string> GetMessageDescriptions() 
        {
            if (MessageDescriptions.Count == 0) 
            {
                MessageDescriptions.Add(0x10, "Diagnostic Session Control");
                MessageDescriptions.Add(0x11, "ECU Reset");
                MessageDescriptions.Add(0x27, "Security Access");
                MessageDescriptions.Add(0x28, "Communication Control");
                MessageDescriptions.Add(0x29, "Authentication");
                MessageDescriptions.Add(0x3E, "Tester Present");
                MessageDescriptions.Add(0x83, "Access Timing Parameters");
                MessageDescriptions.Add(0x84, "Secured Data Transmission");
                MessageDescriptions.Add(0x85, "Control DTC Settings");
                MessageDescriptions.Add(0x86, "Response On Event");
                MessageDescriptions.Add(0x87, "Link Control");
                MessageDescriptions.Add(0x22, "Read Data By Identifier");
                MessageDescriptions.Add(0x23, "Read Memory By Address");
                MessageDescriptions.Add(0x24, "Read Scaling Data By Identifier");
                MessageDescriptions.Add(0x2A, "Read Data By Identifier Periodic");
                MessageDescriptions.Add(0x2C, "Dynamically Define Data Identifier");
                MessageDescriptions.Add(0x2E, "Write Data By Identifier");
                MessageDescriptions.Add(0x3D, "Write Memory By Address");
                MessageDescriptions.Add(0x14, "Clear Diagnostic Information");
                MessageDescriptions.Add(0x19, "Read DTC Information");
                MessageDescriptions.Add(0x2F, "Input Output Control By Identifier");
                MessageDescriptions.Add(0x31, "Routine Control");
                MessageDescriptions.Add(0x34, "Request Download");
                MessageDescriptions.Add(0x35, "Request Upload");
                MessageDescriptions.Add(0x36, "Transfer Data");
                MessageDescriptions.Add(0x37, "Request Transfer Exit");
                MessageDescriptions.Add(0x38, "Request File Transfer");
            }
            return MessageDescriptions;
        }


        public static string GetCommandDescription(byte[] command) 
        {
            if (command.Length == 0) 
            {
                return "Internal error: command buffer is empty";
            }

            // handle NR first
            if (command[0] == 0x7F) 
            {
                string response = "Negative Response";
                if (command.Length > 1) 
                {
                    response = $"{response}: {GetNegativeResponseDescriptions()[command[1]]}";
                }
                return response;
            }

            Dictionary<int, string> descriptions = GetMessageDescriptions();
            if (descriptions.ContainsKey(command[0]))
            {
                return $"Request: {descriptions[command[0]]}";
            }

            int tryAsResponse = command[0] - 0x40;

            if (descriptions.ContainsKey(tryAsResponse))
            {
                return $"Response: {descriptions[tryAsResponse]}";
            }
            return "Unknown";
        }

        private static bool IsNegativeResponse(byte[] command)
        {
            return ((command.Length > 0) && (command[0] == 0x7F));
        }

        private static bool EnterDiagnosticSession(ECUConnection connection)
        {
            Console.WriteLine("UDS: Switching session states");
            byte[] sessionSwitchResponse = connection.SendMessage(new byte[] { 0x10, 0x03 });
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x03 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"Failed to switch session : target responded with [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
                return false;
            }
            return true;
        }
        
        private static bool ExitDiagnosticSession(ECUConnection connection)
        {
            Console.WriteLine("UDS: Switching session states");
            byte[] sessionSwitchResponse = connection.SendMessage(new byte[] { 0x10, 0x01 });
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x01 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"Failed to switch session : target responded with [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
                return false;
            }
            return true;
        }

        private static bool GetVariantID(ECUConnection connection, out int variantId) 
        {
            byte[] variantQueryResponse = connection.SendMessage(new byte[] { 0x22, 0xF1, 0x00 });
            byte[] variantExpectedResponse = new byte[] { 0x62, 0xF1 };

            if (!variantQueryResponse.Take(2).SequenceEqual(variantExpectedResponse))
            {
                Console.WriteLine($"Failed to identify variant (unexpected response) : target responded with [{BitUtility.BytesToHex(variantQueryResponse, true)}]");
                variantId = 0;
                return false;
            }
            else 
            {
                // found a variant id, check loaded ecus if any of them have a match
                variantId = (variantQueryResponse[3] << 16) | (variantQueryResponse[4] << 8) | variantQueryResponse[5];
                return true;
            }
        }

        public override List<DTCContext> ReportDtcsByStatusMask(ECUConnection connection, ECUVariant variant, byte inMask = 0)
        {
            List<DTCContext> dtcCtx = new List<DTCContext>();

            byte mask = (byte)(DTCStatusByte.TestFailedAtRequestTime |
                DTCStatusByte.TestFailedAtCurrentCycle |
                DTCStatusByte.PendingDTC |
                DTCStatusByte.ConfirmedDTC |
                DTCStatusByte.TestFailedSinceLastClear);
            byte[] request = new byte[] { 0x19, 0x02, inMask == 0 ? mask : inMask };
            byte[] expectedResponse = new byte[] { 0x59, 0x02 };

            byte[] response = connection.SendMessage(request);
            if (!response.Take(expectedResponse.Length).SequenceEqual(expectedResponse))
            {
                return new List<DTCContext>();
            }

            for (int i = 3; i < response.Length; i += 4)
            {
                byte[] dtcRow = new byte[4];
                Array.ConstrainedCopy(response, i, dtcRow, 0, 4);
                string dtcIdentifier = BitUtility.BytesToHex(dtcRow.Take(3).ToArray(), false);

                DTC foundDtc = DTC.FindDTCById(dtcIdentifier, variant);
                if (foundDtc is null)
                {
                    Console.WriteLine($"DTC: No matching DTC available for {dtcIdentifier}");
                }
                dtcCtx.Add(new DTCContext() { DTC = foundDtc, StatusByte = dtcRow[3], EnvironmentContext = new List<string[]>() });
            }
            return dtcCtx;
        }

        public override bool GetDtcSnapshot(DTC dtc, ECUConnection connection, out byte[] snapshotBytes)
        {
            byte[] identifier = BitUtility.BytesFromHex(dtc.Qualifier.Substring(1));

            // apparently the existing dtc's mask should be ignored, use FF instead
            byte[] request = new byte[] { 0x19, 0x06, identifier[0], identifier[1], identifier[2], 0xFF };
            byte[] expectedResponse = new byte[] { 0x59, 0x06 };

            byte[] response = connection.SendMessage(request);
            if (response.Take(expectedResponse.Length).SequenceEqual(expectedResponse))
            {
                snapshotBytes = response;
                return true;
            }
            else
            {
                snapshotBytes = new byte[] { };
                return false;
            }
        }

        public override ECUMetadata QueryECUMetadata(ECUConnection connection)
        {
            ECUMetadata metadata = new ECUMetadata();
            if (ReadDataByIdentifier(connection, 0xF100, out byte[] diagInfoRaw))
            {
                byte session = diagInfoRaw[3];
                byte gateway = diagInfoRaw[0];
                uint variant = (uint)((diagInfoRaw[1] << 8) | diagInfoRaw[2]);
                metadata.GatewayMode = gateway == 2;
                metadata.VariantID = variant;
            }

            if (ReadDataByIdentifier(connection, 0xF111, out byte[] hardwareId))
            {
                metadata.HardwarePartNumber = Encoding.ASCII.GetString(hardwareId);
            }
            if (ReadDataByIdentifier(connection, 0xF150, out byte[] hardwareVersion))
            {
                metadata.HardwareVersion = $"{hardwareVersion[0]:D2}/{hardwareVersion[1]:D2}.{hardwareVersion[2]:D2}";
            }
            if (ReadDataByIdentifier(connection, 0xF154, out byte[] hardwareVendor))
            {
                metadata.VendorID = hardwareVendor[1]; // first byte for vendor is usually discarded; value does not fit BE
            }
            if (ReadDataByIdentifier(connection, 0xF153, out byte[] bootVersion))
            {
                metadata.BootVersion = $"{bootVersion[0]:D2}/{bootVersion[1]:D2}.{bootVersion[2]:D2}";
            }
            if (ReadDataByIdentifier(connection, 0xF18C, out byte[] serialNumber))
            {
                metadata.SerialNumber = Encoding.ASCII.GetString(serialNumber);
            }
            if (ReadDataByIdentifier(connection, 0xF190, out byte[] vinOriginal))
            {
                metadata.ChassisNumberOriginal = Encoding.ASCII.GetString(vinOriginal);
            }
            if (ReadDataByIdentifier(connection, 0xF1A0, out byte[] vinCurrent))
            {
                metadata.ChassisNumberCurrent = Encoding.ASCII.GetString(vinCurrent);
            }

            // read flash blocks, normally code/data/flash

            if (ReadDataByIdentifier(connection, 0xF121, out byte[] fwIdentifier))
            {
                ReadDataByIdentifier(connection, 0xF151, out byte[] aggregateFwVersion);
                ReadDataByIdentifier(connection, 0xF155, out byte[] aggregateSupplierIdent);
                ReadDataByIdentifier(connection, 0xF15B, out byte[] aggregateFingerprint);

                int versionWidth = 3;
                int vendorWidth = 2;
                int fingerprintWidth = 10;
                int pnWidth = 10;

                if (fwIdentifier.Length % pnWidth != 0) 
                {
                    Console.WriteLine("[!] Block PartNumber is not boundary aligned");
                }
                int blockCount = fwIdentifier.Length / pnWidth;

                metadata.FlashMetadata = new List<ECUFlashMetadata>();
                for (int i = 0; i < blockCount; i++)
                {
                    byte[] localPn = fwIdentifier.Skip(i * pnWidth).Take(pnWidth).ToArray();
                    byte[] localFwVersion = aggregateFwVersion.Skip(i * versionWidth).Take(versionWidth).ToArray();
                    byte[] localSupplierIdent = aggregateSupplierIdent.Skip(i * vendorWidth).Take(vendorWidth).ToArray();
                    byte[] localFingerprint = aggregateFingerprint.Skip(i * fingerprintWidth).Take(fingerprintWidth).ToArray();

                    ECUFlashMetadata flashMetadata = new ECUFlashMetadata();
                    flashMetadata.Index = i;
                    flashMetadata.PartNumber = Encoding.ASCII.GetString(localPn);
                    flashMetadata.Version = $"{localFwVersion[0]:D2}/{localFwVersion[1]:D2}.{localFwVersion[2]:D2}";
                    flashMetadata.VendorID = localSupplierIdent[1];
                    flashMetadata.StatusID = localFingerprint[0];
                    flashMetadata.LastFlashVendor = localFingerprint[2];
                    flashMetadata.FlashDate = $"{localFingerprint[3]:D2}-{localFingerprint[4]:D2}-{localFingerprint[5]:D2}"; // no idea what sort of date format; all 3 fields can hold values above 12
                    flashMetadata.FlashFingerprint = BitUtility.BytesToHex(localFingerprint.Skip(6).ToArray());
                    metadata.FlashMetadata.Add(flashMetadata);
                }
            }

            return metadata;
        }

        private static bool ReadDataByIdentifier(ECUConnection connection, ushort identifier, out byte[] buffer)
        {
            buffer = new byte[] { };
            byte identifierMsb = (byte)((identifier >> 8) & 0xFF);
            byte identifierLsb = (byte)(identifier & 0xFF);
            byte[] response = connection.SendMessage(new byte[] { 0x22, identifierMsb, identifierLsb });
            if (response.Length < 3) 
            {
                return false;
            }
            if (response[0] != 0x62) 
            {
                return false;
            }
            buffer = response.Skip(3).ToArray();
            return true;
        }

        public override void ConnectionEstablishedHandler(ECUConnection connection)
        {
            if (!EnterDiagnosticSession(connection))
            {
                return;
            }
            if (GetVariantID(connection, out int variantId))
            {
                connection.VariantIsAvailable = true;
                connection.ECUVariantID = variantId;
                Console.WriteLine($"Variant has been successfully configured as {(variantId & 0xFFFF):X4}");
            }
            else 
            {
                return;
            }
        }

        public override void SendTesterPresent(ECUConnection connection)
        {
            connection.SendMessage(new byte[] { 0x3E, 0x00 }, true);
        }

        public override bool IsResponseToTesterPresent(byte[] inBuffer)
        {
            return inBuffer.SequenceEqual(new byte[] { 0x7E, 0x00 });
        }

        public override void ConnectionClosingHandler(ECUConnection connection)
        {
            ExitDiagnosticSession(connection);
        }

        public override string GetProtocolName()
        {
            return "UDS";
        }

        public override bool SupportsUnlocking()
        {
            return true;
        }
    }
}
