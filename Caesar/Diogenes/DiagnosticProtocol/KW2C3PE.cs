using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Diogenes.DiagnosticProtocol
{
    public class KW2C3PE : BaseProtocol
    {
        private const int Ki211ControlCanId = 0x1C;

        private static bool HasDiagService(ECU ecu, string qualifier)
        {
            return ecu?.GlobalDiagServices?.Any(x => string.Equals(x.Qualifier, qualifier, StringComparison.OrdinalIgnoreCase)) == true;
        }

        private static bool HasInitializationClassHint(ECU ecu)
        {
            if (ecu?.GlobalDiagServices == null)
            {
                return false;
            }

            foreach (DiagService service in ecu.GlobalDiagServices)
            {
                if (!string.Equals(service.Qualifier, "{INITIALIZATION}", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (service.DiagComParameters.Any(cp =>
                    string.Equals(cp.ParamName, "CP_CANECU_CLASS", StringComparison.OrdinalIgnoreCase) &&
                    cp.ComParamValue == 1))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool RequiresDedicatedLscanControlPath(ECUConnection connection)
        {
            if (connection?.EcuContext == null || string.IsNullOrWhiteSpace(connection.FriendlyProfileName))
            {
                return false;
            }

            if (!connection.FriendlyProfileName.StartsWith("LSCAN_", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            ECUConnectionProfile profile = connection.EcuContext.GetConnectionProfileByName(connection.FriendlyProfileName);
            if (profile == null || !string.Equals(profile.InterfaceQualifier, "KW2C3PE", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // The CBF exposes a VDO manufacturing-mode service and initialization class hint, but not the
            // alternate 0x1C control CAN identifier. We use those CBF signals to decide when to enable the
            // dedicated LSCAN control-path handling, while the actual side-channel CAN ID remains a protocol constant.
            return HasDiagService(connection.EcuContext, "FN_VDO_Fertigungs_Mode") &&
                HasInitializationClassHint(connection.EcuContext);
        }

        private static void PrimeDedicatedControlSession(ECUConnection connection, byte sessionSubFunction)
        {
            Console.WriteLine($"[LOG] KW2C3PE: Priming dedicated LSCAN control path via CAN 0x{Ki211ControlCanId:X} with 10 {sessionSubFunction:X2}");
            connection.SendMessageWithoutResponse(new byte[] { 0x10, sessionSubFunction }, Ki211ControlCanId);
            Thread.Sleep(350);
            connection.SendMessageWithoutResponse(new byte[] { 0x10, sessionSubFunction }, Ki211ControlCanId);
            Thread.Sleep(200);
        }

        private static bool EnterDiagnosticSession(ECUConnection connection)
        {
            if (RequiresDedicatedLscanControlPath(connection))
            {
                PrimeDedicatedControlSession(connection, 0x92);
                Console.WriteLine("[LOG] KW2C3PE: Dedicated LSCAN control session primed; skipping physical 10 92 and continuing with diagnostic requests");
                return true;
            }

            Console.WriteLine("[LOG] KW2C3PE: Entering Diagnostic Session (10 92)");
            byte[] sessionSwitchResponse = connection.SendMessage(new byte[] { 0x10, 0x92 });
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x92 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"[LOG] KW2C3PE: Session switch FAILED: expected [50 92], got [{BitUtility.BytesToHex(sessionSwitchResponse, true)}] (len={sessionSwitchResponse.Length})");
                return false;
            }
            Console.WriteLine($"[LOG] KW2C3PE: Session switch OK: [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
            return true;
        }
        
        private static bool ExitDiagnosticSession(ECUConnection connection)
        {
            if (RequiresDedicatedLscanControlPath(connection))
            {
                Console.WriteLine($"[LOG] KW2C3PE: Exiting dedicated LSCAN control session via CAN 0x{Ki211ControlCanId:X} (10 81)");
                connection.SendMessageWithoutResponse(new byte[] { 0x10, 0x81 }, Ki211ControlCanId);
                return true;
            }

            Console.WriteLine("[LOG] KW2C3PE: Exiting Diagnostic Session (10 81)");
            byte[] sessionSwitchResponse = connection.SendMessage(new byte[] { 0x10, 0x81 });
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x81 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"[LOG] KW2C3PE: Exit session FAILED: expected [50 81], got [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
                return false;
            }
            Console.WriteLine("[LOG] KW2C3PE: Exit session OK");
            return true;
        }

        private static bool GetVariantID_1A86(ECUConnection connection, out int variantId)
        {
            byte[] variantQueryResponse = connection.SendMessage(new byte[] { 0x1A, 0x86 });
            byte[] variantExpectedResponse = new byte[] { 0x5A, 0x86 };

            if (!variantQueryResponse.Take(2).SequenceEqual(variantExpectedResponse))
            {
                variantId = 0;
                return false;
            }
            else
            {
                variantId = (variantQueryResponse[12] << 8) | variantQueryResponse[13];
                return true;
            }
        }
        private static bool GetVariantID_1A87(ECUConnection connection, out int variantId)
        {
            byte[] variantQueryResponse = connection.SendMessage(new byte[] { 0x1A, 0x87 });
            byte[] variantExpectedResponse = new byte[] { 0x5A, 0x87 };

            if (!variantQueryResponse.Take(2).SequenceEqual(variantExpectedResponse))
            {
                variantId = 0;
                return false;
            }
            else
            {
                variantId = (variantQueryResponse[4] << 8) | variantQueryResponse[5];
                return true;
            }
        }

        /// <summary>
        /// Try a generic 1A XX variant ID query. Logs and returns the variant ID if successful.
        /// Response format: 5A XX [data...] where the variant ID location depends on the sub-function.
        /// </summary>
        private static bool TryVariantQuery1A(ECUConnection connection, byte subFunc, out int variantId, int idOffset1, int idOffset2)
        {
            Console.WriteLine($"[LOG] KW2C3PE: Trying variant query 1A {subFunc:X2}");
            byte[] response = connection.SendMessage(new byte[] { 0x1A, subFunc });
            if (response.Length >= 2 && response[0] == 0x5A && response[1] == subFunc)
            {
                if (response.Length > idOffset2)
                {
                    variantId = (response[idOffset1] << 8) | response[idOffset2];
                    Console.WriteLine($"[LOG] KW2C3PE: Variant ID from 1A {subFunc:X2} = 0x{variantId:X4} ({variantId}) [offset {idOffset1},{idOffset2}]");
                    Console.WriteLine($"[LOG] KW2C3PE: Full response: [{BitUtility.BytesToHex(response, true)}]");
                    return true;
                }
                // Got positive response but too short for variant ID - log full data
                Console.WriteLine($"[LOG] KW2C3PE: Positive response to 1A {subFunc:X2} but too short for variant ID (len={response.Length}): [{BitUtility.BytesToHex(response, true)}]");
            }
            else if (response.Length >= 3 && response[0] == 0x7F)
            {
                Console.WriteLine($"[LOG] KW2C3PE: 1A {subFunc:X2} rejected: NRC 0x{response[2]:X2}");
            }
            else
            {
                Console.WriteLine($"[LOG] KW2C3PE: 1A {subFunc:X2} unexpected response: [{BitUtility.BytesToHex(response, true)}]");
            }
            variantId = 0;
            return false;
        }

        /// <summary>
        /// Try UDS-style ReadDataByIdentifier (22 F1 XX) as a fallback for ECU identification.
        /// </summary>
        private static bool TryVariantQueryUDS(ECUConnection connection, out int variantId)
        {
            // Try common UDS DID identifiers for ECU identification
            byte[][] dids = new byte[][]
            {
                new byte[] { 0x22, 0xF1, 0x00 }, // ECU Identification
                new byte[] { 0x22, 0xF1, 0x01 }, // ECU Serial Number  
                new byte[] { 0x22, 0xF1, 0x10 }, // VIN
                new byte[] { 0x22, 0xF1, 0x50 }, // System Supplier ECU HW Number
            };

            foreach (byte[] did in dids)
            {
                Console.WriteLine($"[LOG] KW2C3PE: Trying UDS fallback: {BitUtility.BytesToHex(did, true)}");
                byte[] response = connection.SendMessage(did);
                if (response.Length >= 4 && response[0] == 0x62)
                {
                    Console.WriteLine($"[LOG] KW2C3PE: UDS positive response: [{BitUtility.BytesToHex(response, true)}]");
                    // Try to extract a variant ID from the response data
                    if (response.Length >= 6)
                    {
                        variantId = (response[3] << 8) | response[4];
                        Console.WriteLine($"[LOG] KW2C3PE: Extracted variant ID from UDS: 0x{variantId:X4}");
                        return true;
                    }
                }
                else if (response.Length >= 3 && response[0] == 0x7F)
                {
                    Console.WriteLine($"[LOG] KW2C3PE: UDS {BitUtility.BytesToHex(did, true)} rejected: NRC 0x{response[2]:X2}");
                }
            }
            variantId = 0;
            return false;
        }

        private static bool GetVariantID(ECUConnection connection, out int variantId)
        {
            Console.WriteLine("[LOG] KW2C3PE: Starting variant ID query cascade");

            // Try all known KW2C3PE sub-functions with their expected response offsets
            // 1A 86: variant ID at offset [12,13] (standard diagnostic variant)
            if (TryVariantQuery1A(connection, 0x86, out variantId, 12, 13)) return true;
            // 1A 87: variant ID at offset [4,5]
            if (TryVariantQuery1A(connection, 0x87, out variantId, 4, 5)) return true;
            // 1A 88: ECU identification data
            if (TryVariantQuery1A(connection, 0x88, out variantId, 2, 3)) return true;
            // 1A 89: supplier info
            if (TryVariantQuery1A(connection, 0x89, out variantId, 2, 3)) return true;
            // 1A 91: ECU serial number
            if (TryVariantQuery1A(connection, 0x91, out variantId, 2, 3)) return true;
            // 1A 9B: System supplier specific
            if (TryVariantQuery1A(connection, 0x9B, out variantId, 2, 3)) return true;
            // 1A 9C: System name
            if (TryVariantQuery1A(connection, 0x9C, out variantId, 2, 3)) return true;

            // UDS fallback (22 F1 XX)
            Console.WriteLine("[LOG] KW2C3PE: All 1A queries failed, trying UDS ReadDataByIdentifier fallback");
            if (TryVariantQueryUDS(connection, out variantId)) return true;

            Console.WriteLine("[LOG] KW2C3PE: ALL variant ID queries failed - ECU does not support any known identification service");
            variantId = 0;
            return false;
        }

        public override List<DTCContext> ReportDtcsByStatusMask(ECUConnection connection, ECUVariant variant, byte inMask = 0)
        {
            // Known limitation: KW2C3PE likely uses a different command family around 0x18.
            return base.ReportDtcsByStatusMask(connection, variant, inMask);
        }

        public override bool GetDtcSnapshot(DTC dtc, ECUConnection connection, out byte[] snapshotBytes)
        {
            // Known limitation: snapshot retrieval still falls back to the base implementation.
            return base.GetDtcSnapshot(dtc, connection, out snapshotBytes);
        }

        public override void ConnectionEstablishedHandler(ECUConnection connection)
        {
            Console.WriteLine("[LOG] KW2C3PE: ConnectionEstablishedHandler starting");
            if (RequiresDedicatedLscanControlPath(connection))
            {
                connection.TesterPresentTimer.Interval = 1250;
                Console.WriteLine("[LOG] KW2C3PE: Dedicated LSCAN control path enabled with 1250ms TesterPresent interval");
            }
            if (!EnterDiagnosticSession(connection))
            {
                Console.WriteLine("[LOG] KW2C3PE: ConnectionEstablishedHandler aborting - session switch failed");
                return;
            }
            if (GetVariantID(connection, out int variantId))
            {
                connection.VariantIsAvailable = true;
                connection.ECUVariantID = variantId;
                Console.WriteLine($"[LOG] KW2C3PE: Variant has been successfully configured as {(variantId & 0xFFFF):X4}");
            }
            else
            {
                // Don't abort — keep the session alive so the user can interact manually
                Console.WriteLine("[LOG] KW2C3PE: WARNING: Variant ID query failed, but session is active.");
                Console.WriteLine("[LOG] KW2C3PE: Connection will proceed without variant matching.");
                Console.WriteLine("[LOG] KW2C3PE: The user can still send manual commands via the console.");
                connection.VariantIsAvailable = false;
            }
            Console.WriteLine("[LOG] KW2C3PE: ConnectionEstablishedHandler completed successfully");
        }

        public override void SendTesterPresent(ECUConnection connection)
        {
            if (RequiresDedicatedLscanControlPath(connection))
            {
                connection.SendMessageWithoutResponse(new byte[] { 0x3E, 0x02 }, Ki211ControlCanId, true);
                return;
            }

            connection.SendMessage(new byte[] { 0x3E, 0x00 }, true);
        }

        public override bool IsResponseToTesterPresent(byte[] inBuffer)
        {
            // Accept both positive TP replies and negative replies to 3E so they never poison unrelated requests.
            return (inBuffer.Length >= 2 && inBuffer[0] == 0x7E) ||
                (inBuffer.Length >= 3 && inBuffer[0] == 0x7F && inBuffer[1] == 0x3E);
        }

        public override bool CanSendTesterPresentWhileBusy(ECUConnection connection)
        {
            return RequiresDedicatedLscanControlPath(connection);
        }

        public override void ConnectionClosingHandler(ECUConnection connection)
        {
            ExitDiagnosticSession(connection);
        }


        public override string GetProtocolName()
        {
            return "KW2C3PE";
        }

        public override bool SupportsUnlocking()
        {
            return true;
        }
    }
}
