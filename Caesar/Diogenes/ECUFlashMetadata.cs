using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class ECUFlashMetadata
    {
        public int Index = 0;
        public string PartNumber = "Unspecified";
        public string Version = "Unspecified";
        public byte VendorID = 0;
        public byte StatusID = 0;
        public string FlashDate; // string bc i don't know what sort of datetime format they're on
        public byte LastFlashVendor = 0;
        public string FlashFingerprint = "Unspecified";
    }
}
