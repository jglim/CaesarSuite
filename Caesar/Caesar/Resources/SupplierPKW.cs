using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caesar.Resources
{
    public class SupplierPKW
    {
        private static Dictionary<int, string> Supplier = new Dictionary<int, string>();
        public static string GetSupplierName(int id) 
        {
            EnsureSupplierDictionary();
            if (Supplier.ContainsKey(id)) 
            {
                return Supplier[id];
            }
            return Supplier[0xFF]; // unidentified
        }

        private static void EnsureSupplierDictionary() 
        {
            if (Supplier.Count == 0) 
            {
                Supplier.Add(0x01, "Becker");
                Supplier.Add(0x02, "Blaupunkt");
                Supplier.Add(0x03, "Bosch");
                Supplier.Add(0x04, "MB");
                Supplier.Add(0x05, "HuF");
                Supplier.Add(0x06, "Kammerer");
                Supplier.Add(0x07, "Kostal");
                Supplier.Add(0x08, "Siemens");
                Supplier.Add(0x09, "Stribel");
                Supplier.Add(0x0A, "MicroHeat");
                Supplier.Add(0x0B, "JATCO");
                Supplier.Add(0x0C, "Cummins");
                Supplier.Add(0x0D, "ZF Lenksysteme");
                Supplier.Add(0x0E, "Nidec Motors & Actuators");
                Supplier.Add(0x0F, "S&T Daewoo (Science & Technology Daewoo)");
                Supplier.Add(0x10, "SWF");
                Supplier.Add(0x11, "VDO");
                Supplier.Add(0x12, "Webasto");
                Supplier.Add(0x13, "Dornier");
                Supplier.Add(0x14, "TEG");
                Supplier.Add(0x15, "Hella");
                Supplier.Add(0x16, "Lucas");
                Supplier.Add(0x17, "GKR");
                Supplier.Add(0x18, "MBB");
                Supplier.Add(0x19, "Motometer");
                Supplier.Add(0x1A, "Daimler");
                Supplier.Add(0x1B, "Sanden");
                Supplier.Add(0x1C, "IEE");
                Supplier.Add(0x1D, "ASK");
                Supplier.Add(0x1E, "U-Shin");
                Supplier.Add(0x1F, "Volkswagen");
                Supplier.Add(0x20, "Borg");
                Supplier.Add(0x21, "Temic");
                Supplier.Add(0x22, "Teves");
                Supplier.Add(0x23, "Borg Warner");
                Supplier.Add(0x24, "MED S.P.A");
                Supplier.Add(0x25, "DENSO");
                Supplier.Add(0x26, "ZF");
                Supplier.Add(0x27, "TRW");
                Supplier.Add(0x28, "Dunlop");
                Supplier.Add(0x29, "LuK");
                Supplier.Add(0x2A, "Hyundai Autonet");
                Supplier.Add(0x2B, "Freightliner");
                Supplier.Add(0x2C, "TAKATA-PETRI");
                Supplier.Add(0x2D, "Haldex");
                Supplier.Add(0x2E, "Hirschmann");
                Supplier.Add(0x2F, "e2v Technology");
                Supplier.Add(0x30, "Magneti Marelli");
                Supplier.Add(0x31, "DODUCO");
                Supplier.Add(0x32, "Alpine");
                Supplier.Add(0x33, "AMC (AEG Mobile Com.)");
                Supplier.Add(0x34, "Bose");
                Supplier.Add(0x35, "DASA");
                Supplier.Add(0x36, "Motorola");
                Supplier.Add(0x37, "Nokia");
                Supplier.Add(0x38, "Panasonic");
                Supplier.Add(0x39, "APAG");
                Supplier.Add(0x3A, "Rialtosoft");
                Supplier.Add(0x3B, "Applicom");
                Supplier.Add(0x3C, "Conti Temic");
                Supplier.Add(0x3D, "Cherry");
                Supplier.Add(0x3E, "TI Automotive");
                Supplier.Add(0x3F, "Kongsberg Automotive");
                Supplier.Add(0x40, "Delphi");
                Supplier.Add(0x41, "Alfmeier");
                Supplier.Add(0x42, "Sidler");
                Supplier.Add(0x43, "Marquardt");
                Supplier.Add(0x44, "Wehrle");
                Supplier.Add(0x45, "megamos");
                Supplier.Add(0x46, "ADC");
                Supplier.Add(0x47, "BERU");
                Supplier.Add(0x48, "Valeo");
                Supplier.Add(0x49, "Magna");
                Supplier.Add(0x4A, "Allison");
                Supplier.Add(0x4B, "Isringhausen");
                Supplier.Add(0x4C, "Grammer");
                Supplier.Add(0x4D, "Funkwerk Dabendorf");
                Supplier.Add(0x4E, "Hella-Behr");
                Supplier.Add(0x4F, "Pollack");
                Supplier.Add(0x50, "AKG");
                Supplier.Add(0x51, "Automotive Lighting");
                Supplier.Add(0x52, "TAG");
                Supplier.Add(0x53, "UNITED PARTS");
                Supplier.Add(0x54, "catem");
                Supplier.Add(0x55, "Alge");
                Supplier.Add(0x56, "Pierburg");
                Supplier.Add(0x57, "Brusa");
                Supplier.Add(0x58, "Ecostar");
                Supplier.Add(0x59, "NuCellSys");
                Supplier.Add(0x5A, "Wabco Automotive");
                Supplier.Add(0x5B, "Voith");
                Supplier.Add(0x5C, "Knorr");
                Supplier.Add(0x5D, "TVI");
                Supplier.Add(0x5E, "Stoneridge");
                Supplier.Add(0x5F, "Telma");
                Supplier.Add(0x60, "STW");
                Supplier.Add(0x61, "Koyo");
                Supplier.Add(0x62, "Eberspaecher");
                Supplier.Add(0x63, "ADVICS");
                Supplier.Add(0x64, "OMRON");
                Supplier.Add(0x65, "Mitsubishi Heavy Industry");
                Supplier.Add(0x66, "Methode");
                Supplier.Add(0x67, "UNISIAJECS");
                Supplier.Add(0x68, "UNISIA JKC Steering Systems");
                Supplier.Add(0x69, "AISIN");
                Supplier.Add(0x6A, "Zexel Valeo");
                Supplier.Add(0x6B, "Schrader");
                Supplier.Add(0x6C, "Ballard");
                Supplier.Add(0x6D, "Alcoa Fujikura");
                Supplier.Add(0x6E, "Transtron");
                Supplier.Add(0x6F, "Iteris");
                Supplier.Add(0x70, "SFT");
                Supplier.Add(0x71, "Kieckert AG");
                Supplier.Add(0x72, "Behr");
                Supplier.Add(0x73, "MB Lenkungen");
                Supplier.Add(0x74, "Sachs Automotive");
                Supplier.Add(0x75, "Peiker");
                Supplier.Add(0x76, "Petri");
                Supplier.Add(0x77, "Autoliv");
                Supplier.Add(0x78, "Thien electronic");
                Supplier.Add(0x79, "Siemens VDO");
                Supplier.Add(0x7A, "Dornier Consulting GmbH");
                Supplier.Add(0x7B, "Alps");
                Supplier.Add(0x7C, "PREH");
                Supplier.Add(0x7D, "Hitachi Unisia");
                Supplier.Add(0x7E, "Hitachi");
                Supplier.Add(0x80, "Huntsville");
                Supplier.Add(0x81, "Yazaki");
                Supplier.Add(0x82, "Lear");
                Supplier.Add(0x83, "Johnson Controls");
                Supplier.Add(0x84, "Harman / Becker");
                Supplier.Add(0x85, "Mitsubishi Electric");
                Supplier.Add(0x86, "Tokico USA Inc.");
                Supplier.Add(0x87, "Nippon Seiki (NS Intl)");
                Supplier.Add(0x88, "Inalfa");
                Supplier.Add(0x89, "Nippon Seiki (UK)");
                Supplier.Add(0x8A, "GHSP");
                Supplier.Add(0x8B, "Vector");
                Supplier.Add(0x8C, "Gentex");
                Supplier.Add(0x8D, "Visteon");
                Supplier.Add(0x8E, "Tochigi Fuji");
                Supplier.Add(0x8F, "Chrysler");
                Supplier.Add(0x90, "May and Scofield");
                Supplier.Add(0x91, "Mercedes-Benz Hamburg Plant");
                Supplier.Add(0x92, "AISIN AW");
                Supplier.Add(0x93, "TOYODA MACHINE WORKS");
                Supplier.Add(0x94, "Solectron-Invotronics");
                Supplier.Add(0x95, "KICKER");
                Supplier.Add(0x96, "American Axle Company");
                Supplier.Add(0x97, "GETRAG");
                Supplier.Add(0x98, "Promate");
                Supplier.Add(0x99, "ArvinMeritor");
                Supplier.Add(0x9A, "Autometer");
                Supplier.Add(0x9B, "Valeo Sylvania");
                Supplier.Add(0x9C, "Cobasys");
                Supplier.Add(0x9D, "Helbako");
                Supplier.Add(0x9E, "Continental");
                Supplier.Add(0xA2, "FUSO");
                Supplier.Add(0xA3, "Autokabel");
                Supplier.Add(0xA4, "Hyundai Mobis");
                Supplier.Add(0xA5, "Festo");
                Supplier.Add(0xA6, "Schmidhauser");
                Supplier.Add(0xA7, "Sphere DesignGmbH");
                Supplier.Add(0xA8, "Deutsche Accumotive GmbH & Co KG");
                Supplier.Add(0xA9, "BRC Gas Equipment");
                Supplier.Add(0xAA, "Delta Energy Systems");
                Supplier.Add(0xAB, "A123 Systems");
                Supplier.Add(0xAC, "Mercedes AMG");
                Supplier.Add(0xAD, "Huber Automotive AG");
                Supplier.Add(0xB0, "M/A-COM");
                Supplier.Add(0xB1, "TBK (Tokai Bussan Corp)");
                Supplier.Add(0xB2, "DDC (Detroit Diesel Corp)");
                Supplier.Add(0xB3, "3SOFT");
                Supplier.Add(0xB4, "MB-Tech");
                Supplier.Add(0xB5, "E-T-A");
                Supplier.Add(0xB6, "Ssangyong");
                Supplier.Add(0xB7, "Paragon");
                Supplier.Add(0xB8, "ThyssenKrupp");
                Supplier.Add(0xB9, "Hoerbiger");
                Supplier.Add(0xBA, "Bang and Olufsen");
                Supplier.Add(0xBB, "Hughes");
                Supplier.Add(0xBC, "Flextronics");
                Supplier.Add(0xFE, "After Market Supplier");
                Supplier.Add(0xFF, "Unidentified");
                Supplier.Add(0xEFFE, "After Market Supplier");
                Supplier.Add(0xEFFF, "Unidentified");
            }
        }
    }
}
