using Caesar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            string unlockEcuFolder = $"{Application.StartupPath}{Path.DirectorySeparatorChar}UnlockECU{Path.DirectorySeparatorChar}";
            string binaryPath = $"{unlockEcuFolder}ConsoleUnlockECU.exe";
            string dbPath = $"{unlockEcuFolder}db.json";

            key = new byte[] { };

            if (!(File.Exists(binaryPath) && File.Exists(dbPath)))
            {
                Console.WriteLine("Automatic unlock is unavailable (executable or database could not be found)");
                return false;
            }

            string successIdentifier = "DIOGENES";

            string args = $"-d \"{dbPath}\" -n {ecuName} -l {level} -s {BitUtility.BytesToHex(seed)} -p {successIdentifier}";
            string result = RunProcessCaptureOutput(binaryPath, args);
            if (!result.StartsWith(successIdentifier)) 
            {
                return false;
            }
            string responseAsString = result.Substring(successIdentifier.Length);
            Console.WriteLine($"UnlockECU (automatic) returns {responseAsString} for seed {BitUtility.BytesToHex(seed)} (ECU: {ecuName}, Level: {level})");

            key = BitUtility.BytesFromHex(responseAsString);
            return true;
        }


        private static string RunProcessCaptureOutput(string filePath, string args) 
        {
            // see https://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net

            StringBuilder outputBuilder;
            ProcessStartInfo processStartInfo;
            Process process;

            outputBuilder = new StringBuilder();

            processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.Arguments = args;
            processStartInfo.FileName = filePath;

            process = new Process();
            process.StartInfo = processStartInfo;
            // enable raising events because Process does not raise events by default
            process.EnableRaisingEvents = true;
            // attach the event handler for OutputDataReceived before starting the process
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    outputBuilder.Append(e.Data);
                }
            );
            // start the process
            // then begin asynchronously reading the output
            // then wait for the process to exit
            // then cancel asynchronously reading the output
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            // use the output
            return outputBuilder.ToString();
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
