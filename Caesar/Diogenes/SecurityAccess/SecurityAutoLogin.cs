using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Diogenes.SecurityAccess.NativeUnlock;

namespace Diogenes.SecurityAccess
{
    public class SecurityAutoLogin
    {
        public static void ReceiveSecurityResponse(byte[] response, ECU parentEcu, ECUConnection connection) 
        {
            if (response.Length == 2)
            {
                // level change ack
                Console.WriteLine($"Security level has been successfully changed to 0x{(response[1] - 1):X}");
            }
            else
            {
                // seed received
                byte[] seedValue = response.Skip(2).ToArray();
                string seedValueAsString = BitUtility.BytesToHex(seedValue, true);
                int receiveLevel = response[1];

                bool manualUnlockRequired = true;
                if (QueryUnlockEcu(seedValue, parentEcu.Qualifier, receiveLevel, out byte[] key))
                {
                    if (RequestUnlock(connection, receiveLevel, key)) 
                    {
                        Console.WriteLine($"ECU has been automatically unlocked for level {receiveLevel}");
                        manualUnlockRequired = false;
                    }
                }
                if (manualUnlockRequired)
                {
                    PromptClipboardCopyOfSeed(seedValueAsString);
                }
            }
        }

        private static bool RequestUnlock(ECUConnection connection, int receiveLevel, byte[] key) 
        {
            List<byte> keyResponse = new List<byte>();
            keyResponse.Add(0x27);
            keyResponse.Add((byte)(receiveLevel + 1));
            keyResponse.AddRange(key);

            byte[] response = connection.SendMessage(keyResponse);

            if ((response.Length == 2) && (response[0] == 0x67))
            {
                return true;
            }
            return false;
        }

        public static bool QueryUnlockEcu(byte[] seed, string ecuName, int level, out byte[] key)
        {
            key = new byte[] { };

            if (NativeUnlockService.TryGeneratePayload(ecuName, level, seed, out byte[] nativePayload, out UnlockDefinition nativeDefinition, out string nativeError))
            {
                key = nativePayload;
                Console.WriteLine($"Native unlock returns {BitUtility.BytesToHex(key)} for seed {BitUtility.BytesToHex(seed)} (ECU: {ecuName}, Level: {level}, Provider: {nativeDefinition.Provider}, Origin: {nativeDefinition.Origin})");
                return true;
            }

            Console.WriteLine($"Automatic unlock is unavailable (native match: {nativeError})");
            return false;
        }

        private static void PromptClipboardCopyOfSeed(string seed)
        {
            if (MessageBox.Show($"Received a seed value of {seed}. \r\nCopy to clipboard?", "Security Access", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Clipboard.SetText(seed);
            }
        }
    }
}
