using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class UDS
    {
        private static string[] NegativeResponseDescriptions = new string[] { };
        private static Dictionary<int, string> MessageDescriptions = new Dictionary<int, string>();

        public static string[] GetNegativeResponseDescriptions() 
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
                NegativeResponseDescriptions[0x36] = "exceed Number Of Attempts";
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

        public static Dictionary<int, string> GetMessageDescriptions() 
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

        public static string GetDescriptionForCommand(byte[] command) 
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

        // try to switch the session and filter the list of containers if a variant match is found
        public static bool TryDetectVariantAndSwitchSession(ECUConnection connection, List<CaesarContainer> containers) 
        {
            Console.WriteLine("Switching session states..");

            byte[] sessionSwitchResponse = connection.SendMessage(new byte[] { 0x10, 0x03 });
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x03 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"Failed to switch session : target responded with {BitUtility.BytesToHex(sessionSwitchResponse, true)}");
            }
            else
            {
                Console.WriteLine("Querying variant.. ");
                // this is NOT uds specific (!)
                byte[] variantQueryResponse = connection.SendMessage(new byte[] { 0x22, 0xF1, 0x00 });
                byte[] variantExpectedResponse = new byte[] { 0x62, 0xF1 };

                if (!variantQueryResponse.Take(2).SequenceEqual(variantExpectedResponse))
                {
                    Console.WriteLine($"Failed to identify variant (unexpected response) : target responded with {BitUtility.BytesToHex(variantQueryResponse, true)}");
                }
                else
                {
                    // found a variant id, check loaded ecus if any of them have a match
                    int variantId = (variantQueryResponse[3] << 16) | (variantQueryResponse[4] << 8) | variantQueryResponse[5];

                    ECUVariant matchingVariant = null;
                    ECU matchingEcu = null;
                    CaesarContainer matchingContainer = null;

                    foreach (CaesarContainer container in containers)
                    {
                        foreach (ECU ecu in container.CaesarECUs)
                        {
                            foreach (ECUVariant variant in ecu.ECUVariants)
                            {
                                foreach (ECUVariantPattern pattern in variant.VariantPatterns)
                                {
                                    if (variantId == pattern.VariantID)
                                    {
                                        matchingVariant = variant;
                                        matchingEcu = ecu;
                                        matchingContainer = container;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (matchingVariant != null)
                    {
                        // if a match was found, clean up the tree to show relevant content only
                        matchingEcu.ECUVariants = new List<ECUVariant>() { matchingEcu.ECUVariants.Find(x => x.Qualifier == matchingVariant.Qualifier) };
                        matchingContainer.CaesarECUs = new List<ECU>() { matchingContainer.CaesarECUs.Find(x => x.Qualifier == matchingEcu.Qualifier) };
                        containers = new List<CaesarContainer>() { containers.Find(x => x.FileChecksum == matchingContainer.FileChecksum) };
                        Console.WriteLine($"Variant has been successfully configured as {matchingVariant.Qualifier}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"No matching variants found. Please check if the loaded CBF files are valid for your target. \n\nVariant ID: {variantId}");
                    }
                }
            }
            return false;
        }

    }
}
