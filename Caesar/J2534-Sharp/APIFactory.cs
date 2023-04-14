#region License
/*Copyright(c) 2018, Brian Humlicek
* https://github.com/BrianHumlicek
* 
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/
#endregion License
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SAE.J2534
{
    public class APIFactory : Common.ManagedDisposable
    {
        private static Dictionary<string, API> Cache = new Dictionary<string, API>();
        /// <summary>
        /// Loads an API, verifies its signature and returns the managed wrapper 
        /// </summary>
        /// <param name="Filename">DLL File to load</param>
        /// <returns>JManaged 2534API</returns>
        public static API GetAPI(string Filename)
        {
            lock (Cache)
            {
                if (!Cache.ContainsKey(Filename))
                {
                    if (!File.Exists(Filename)) throw new DllNotFoundException($"DLL Not Found! {Filename}");

                    var API = new API(Filename);
                    if (API.APISignature.SAE_API != SAE_API.V202_SIGNATURE &&
                        API.APISignature.SAE_API != SAE_API.V404_SIGNATURE &&
                        API.APISignature.SAE_API != SAE_API.V500_SIGNATURE)
                    {
                        API.Dispose();
                        throw new MissingMemberException($"No compatible export signature was found in DLL {Filename}");
                    }

                    API.OnDisposing += () => Cache.Remove(Filename);
                    Cache.Add(Filename, API);
                }
                return Cache[Filename];
            }
        }

        /// <summary>
        /// Queries the registry for registered J2534 API's
        /// </summary>
        /// <returns>APIInfo enumerable</returns>
        public static IEnumerable<APIInfo> GetAPIinfo()
        {
            Dictionary<string, string> DetailOptions = new Dictionary<string, string>();
            DetailOptions.Add("CAN", "CAN Bus");
            DetailOptions.Add("ISO15765", "ISO15765");
            DetailOptions.Add("J1850PWM", "J1850PWM");
            DetailOptions.Add("J1850VPW", "J1850VPW");
            DetailOptions.Add("ISO9141", "ISO9141");
            DetailOptions.Add("ISO14230", "ISO14230");
            DetailOptions.Add("SCI_A_ENGINE", "SCI-A Engine");
            DetailOptions.Add("SCI_A_TRANS", "SCI-A Transmission");
            DetailOptions.Add("SCI_B_ENGINE", "SCI-B Engine");
            DetailOptions.Add("SCI_B_TRANS", "SCI-B Transmission");

            const string PASSTHRU_REGISTRY_PATH = @"Software\PassThruSupport.04.04";
            const string PASSTHRU_REGISTRY_PATH_6432 = @"Software\Wow6432Node\PassThruSupport.04.04";

            RegistryKey RootKey = Registry.LocalMachine.OpenSubKey(PASSTHRU_REGISTRY_PATH, false);
            if (RootKey == null)
            {
                RootKey = Registry.LocalMachine.OpenSubKey(PASSTHRU_REGISTRY_PATH_6432, false);
            }

            if (RootKey == null)
                yield break;

            string[] RegistryEntries = RootKey.GetSubKeyNames();
            foreach (string Entry in RegistryEntries)
            {
                RegistryKey deviceKey = RootKey.OpenSubKey(Entry);
                if (deviceKey == null) continue;

                StringBuilder DetailsBuilder = new StringBuilder();

                var Vendor = (string)deviceKey.GetValue("Vendor", "");
                if (!String.IsNullOrWhiteSpace(Vendor))
                    DetailsBuilder.AppendLine($"Vendor: {Vendor}");
                var ConfigApplication = (string)deviceKey.GetValue("ConfigApplication", "");
                if (!String.IsNullOrWhiteSpace(ConfigApplication))
                    DetailsBuilder.AppendLine($"Configuration Application: {ConfigApplication}");

                foreach (var Option in DetailOptions)
                {
                    if (((int)deviceKey.GetValue(Option.Key, 0)) != 0)
                    {
                        DetailsBuilder.AppendLine(Option.Value);
                    }
                }

                yield return new APIInfo((string)deviceKey.GetValue("Name", ""),
                                         (string)deviceKey.GetValue("FunctionLibrary", ""),
                                         DetailsBuilder.ToString());
            }
        }
        public static void StaticDispose()
        {
            foreach (var API in Cache.Values.ToArray())
            {
                API.Dispose();
            }
        }

        protected override void DisposeManaged()
        {
            StaticDispose();
        }
    }
}
