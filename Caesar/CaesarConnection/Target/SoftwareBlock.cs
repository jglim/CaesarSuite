using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.Target
{
    public class SoftwareBlock
    {
        public int Index { get; set; }
        public string PartNumber { get; set; }
        public byte[] Version = new byte[] { };
        public bool Valid { get; set; }
        public int LastFlashVendor { get; set; }

        public int FlashDay { get; set; }
        public int FlashMonth { get; set; }
        public int FlashYear { get; set; }

        public byte[] Fingerprint;

        public string VersionString
        {
            get
            {
                if (Version.Length == 3)
                {
                    return $"{Version[0]:D2}/{Version[1]:D2}.{Version[2]:D2}";
                }
                return "";
            }
        }
        public string FingerprintString
        {
            get
            {
                return BitConverter.ToString(Fingerprint).Replace("-", " ");
            }
        }
    }
}
