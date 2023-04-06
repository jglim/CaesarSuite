using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes.DiagnosticProtocol
{
    public class KW2C3PE : BaseProtocol
    {
        private static bool EnterDiagnosticSession(ECUConnection connection)
        {
            Console.WriteLine("KW2C3PE: Switching session states");

            // vediamo sends this twice https://github.com/jglim/CaesarSuite/issues/52#issuecomment-1399474867
            // cgmb sends this 5x https://github.com/jglim/CaesarSuite/issues/52#issuecomment-1403566348

            for (int i = 0; i < 5; i++) 
            {
                connection.SendMessage(
                    message: new byte[] { 0x10, 0x92 },
                    expectsResponse: false,
                    destinationIdOverride: new byte[] { 0x00, 0x00, 0x00, 0x1C }
                );
                System.Threading.Thread.Sleep(35);
            }

            /*
            byte[] sessionSwitchResponse = connection.SendMessage(
                message: new byte[] { 0x10, 0x92 },
                expectsResponse: false,
                destinationIdOverride: new byte[] { 0x00, 0x00, 0x00, 0x1C }
            );

            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x92 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"Failed to switch session : target responded with [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
                return false;
            }
            */
            return true;
        }
        
        private static bool ExitDiagnosticSession(ECUConnection connection)
        {
            // only sent once by vediamo even in situations where 10 92 is sent twice
            Console.WriteLine("KW2C3PE: Switching session states");
            connection.SendMessage(
                message: new byte[] { 0x10, 0x81 },
                expectsResponse: false,
                destinationIdOverride: new byte[] { 0x00, 0x00, 0x00, 0x1C }
            );
            /*
            byte[] sessionSwitchResponse = connection.SendMessage(
                message: new byte[] { 0x10, 0x81 },
                expectsResponse: false,
                destinationIdOverride: new byte[] { 0x00, 0x00, 0x00, 0x1C }
            );
            byte[] sessionExpectedResponse = new byte[] { 0x50, 0x81 };
            if (!sessionSwitchResponse.Take(2).SequenceEqual(sessionExpectedResponse))
            {
                Console.WriteLine($"Failed to switch session : target responded with [{BitUtility.BytesToHex(sessionSwitchResponse, true)}]");
                return false;
            }
            */
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

        private static bool GetVariantID(ECUConnection connection, out int variantId)
        {
            if (GetVariantID_1A86(connection, out int idFor1A86))
            {
                variantId = idFor1A86;
                return true;
            }
            if (GetVariantID_1A87(connection, out int idFor1A87))
            {
                variantId = idFor1A87;
                return true;
            }
            variantId = 0;
            return false;
        }

        public override List<DTCContext> ReportDtcsByStatusMask(ECUConnection connection, ECUVariant variant, byte inMask = 0)
        {
            // FIXME : KW2C3PE probably uses a different set of commands at 0x18
            return base.ReportDtcsByStatusMask(connection, variant, inMask);
        }

        public override bool GetDtcSnapshot(DTC dtc, ECUConnection connection, out byte[] snapshotBytes)
        {
            // FIXME
            return base.GetDtcSnapshot(dtc, connection, out snapshotBytes);
        }

        public override void ConnectionEstablishedHandler(ECUConnection connection)
        {
            // changing this to debug ki211 as it doesn't seem to have sessions
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
                Console.WriteLine("KW2C3PE: Could not identify variant (1A86, 1A87)");
                return;
            }
        }

        public override void SendTesterPresent(ECUConnection connection)
        {
            // looks like 3E 01 for KW2C3PE 
            // changing this to debug ki211 : no concept of session?
            //connection.SendMessage(message: new byte[] { 0x3E, 0x01 }, testerPresenceRequest: true); // was 3E 01

            connection.SendMessage(
                message: new byte[] { 0x3E, 0x02 },
                testerPresenceRequest: true,
                expectsResponse: false,
                destinationIdOverride: new byte[] { 0x00, 0x00, 0x00, 0x1C }
            );

        }

        public override bool IsResponseToTesterPresent(byte[] inBuffer)
        {
            return inBuffer.SequenceEqual(new byte[] { 0x7E, 0x01 });
        }

        public override void ConnectionClosingHandler(ECUConnection connection)
        {
            // changing this to debug ki211 : no concept of session?
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
