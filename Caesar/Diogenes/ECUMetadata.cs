using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class ECUMetadata
    {
        public bool GatewayMode = false;
        public string BootVersion = "Unspecified";
        public string SerialNumber = "Unspecified";
        public string ChassisNumberOriginal = "Unspecified";
        public string ChassisNumberCurrent = "Unspecified";
        public string HardwarePartNumber = "Unspecified";
        public string HardwareVersion = "Unspecified";
        public uint VariantID = 0;
        public byte VendorID = 0;
        public List<ECUFlashMetadata> FlashMetadata = new List<ECUFlashMetadata>();

        public string GetHtmlTable(ECUConnection connection) 
        {
            ECUMetadata metadata = connection.ConnectionProtocol.QueryECUMetadata(connection);

            string[][] rows = new string[][]
            {
                new string[]{ "Variant ID", metadata.VariantID.ToString("X4") },
                new string[]{ "Gateway Mode", metadata.GatewayMode.ToString() },
                new string[]{ "Boot Version", metadata.BootVersion },
                new string[]{ "Serial Number", metadata.SerialNumber },
                new string[]{ "Chassis Number (Current)", metadata.ChassisNumberCurrent },
                new string[]{ "Chassis Number (Original)", metadata.ChassisNumberOriginal },
                new string[]{ "Hardware Part Number", metadata.HardwarePartNumber },
                new string[]{ "Hardware Version", metadata.HardwareVersion },
                new string[]{ "Hardware Supplier", ECUMetadata.GetVendorName(metadata.VendorID) },
            };

            StringBuilder ecuTableRows = new StringBuilder();
            foreach (string[] row in rows) 
            {
                ecuTableRows.AppendLine($"<tr><td>{row[0]}</td><td>{row[1]}</td></tr>");
            }

            StringBuilder swBlockRows = new StringBuilder();
            foreach (ECUFlashMetadata flash in metadata.FlashMetadata)
            {
                string flashStatus = flash.StatusID == 1 ? "Valid" : "Invalid";
                swBlockRows.AppendLine($@"
<h3>SW Block #{flash.Index}</h3>
<table>
    <tr><td>Part Number</td><td>{flash.PartNumber}</td></tr>
    <tr><td>Version</td><td>{flash.Version}</td></tr>
    <tr><td>Vendor</td><td>{ECUMetadata.GetVendorName(flash.VendorID)}</td></tr>
    <tr><td>Status</td><td>{flashStatus}</td></tr>
    <tr><td>Last Flash Vendor</td><td>{ECUMetadata.GetVendorName(flash.LastFlashVendor)}</td></tr>
    <tr><td>Last Flash Date</td><td>{flash.FlashDate}</td></tr>
    <tr><td>Flash Fingerprint</td><td>{flash.FlashFingerprint}</td></tr>
</table>
");
            }

            return $@"
    <hr>
    <h3>Hardware</h3>
    <table>
        {ecuTableRows}
    </table>
    {swBlockRows}
";
        }
        public static string GetVendorName(byte vendorId) 
        {
            string vendorName = "Undefined";
            if (Enum.IsDefined(typeof(Vendor), vendorId)) 
            {
                vendorName = ((Vendor)vendorId).ToString().Replace("VENDOR_", "").Replace("_", " ");
            }
            return vendorName;
        }

        public static void ShowMetadataModal(ECUConnection connection) 
        {
            if (connection.ConnectionProtocol is null)
            {
                Console.WriteLine("Please initiate contact with a target first.");
                return;
            }
            ECUMetadata metadata = connection.ConnectionProtocol.QueryECUMetadata(connection);

            string[][] rows = new string[][]
            {
                new string[]{ "Variant ID", metadata.VariantID.ToString("X4") },
                new string[]{ "Gateway Mode", metadata.GatewayMode.ToString() },
                new string[]{ "Boot Version", metadata.BootVersion },
                new string[]{ "Serial Number", metadata.SerialNumber },
                new string[]{ "Chassis Number (Current)", metadata.ChassisNumberCurrent },
                new string[]{ "Chassis Number (Original)", metadata.ChassisNumberOriginal },
                new string[]{ "Hardware Part Number", metadata.HardwarePartNumber },
                new string[]{ "Hardware Version", metadata.HardwareVersion },
                new string[]{ "Hardware Supplier", ECUMetadata.GetVendorName(metadata.VendorID) },
            };

            List<string[]> rowsAsList = new List<string[]>(rows);
            foreach (ECUFlashMetadata flash in metadata.FlashMetadata)
            {
                string blockPrefix = $"SW Block #{flash.Index} : ";
                string flashStatus = flash.StatusID == 1 ? "Valid" : "Invalid";

                rowsAsList.Add(new string[] { $"{blockPrefix}Descriptor", $"{flash.PartNumber} [Version {flash.Version} from {ECUMetadata.GetVendorName(flash.VendorID)}]" });
                rowsAsList.Add(new string[] { $"{blockPrefix}Status", $"{flashStatus}, Last flashed by {ECUMetadata.GetVendorName(flash.LastFlashVendor)} on {flash.FlashDate} (Fingerprint: {flash.FlashFingerprint})" });
            }

            GenericPicker picker = new GenericPicker(rowsAsList.ToArray(), new string[] { "Attribute", "Value" });
            picker.Text = "ECU Metadata";
            picker.ShowDialog();
        }

        public enum Vendor : byte
        {
            VENDOR_Unspecified = 0,
            VENDOR_Becker = 1,
            VENDOR_Blaupunkt = 2,
            VENDOR_Bosch = 3,
            VENDOR_MB = 4,
            VENDOR_HuF = 5,
            VENDOR_Kammerer = 6,
            VENDOR_Kostal = 7,
            VENDOR_Siemens = 8,
            VENDOR_Stribel = 9,
            VENDOR_MicroHeat = 10,
            VENDOR_JATCO = 11,
            VENDOR_SWF = 16,
            VENDOR_VDO = 17,
            VENDOR_Webasto = 18,
            VENDOR_Dornier = 19,
            VENDOR_TEG = 20,
            VENDOR_Hella = 21,
            VENDOR_Lucas = 22,
            VENDOR_GKR = 23,
            VENDOR_MBB = 24,
            VENDOR_Motometer = 25,
            VENDOR_Borg = 32,
            VENDOR_Temic = 33,
            VENDOR_Teves = 34,
            VENDOR_Borg_Warner = 35,
            VENDOR_MED_SPA = 36,
            VENDOR_DENSO = 37,
            VENDOR_ZF = 38,
            VENDOR_TRW = 39,
            VENDOR_Dunlop = 40,
            VENDOR_LUK = 41,
            VENDOR_Magneti_Marelli = 48,
            VENDOR_DODUCO = 49,
            VENDOR_Alpine = 50,
            VENDOR_AMC_AEG_Mobile = 51,
            VENDOR_Bose = 52,
            VENDOR_Dasa = 53,
            VENDOR_Motorola = 54,
            VENDOR_Nokia = 55,
            VENDOR_Panasonic = 56,
            VENDOR_APAG = 57,
            VENDOR_Rialtosoft = 58,
            VENDOR_Applicom = 59,
            VENDOR_Conti_Temic = 60,
            VENDOR_Cherry = 61,
            VENDOR_TI_Automotive = 62,
            VENDOR_Kongsberg_Company = 63,
            VENDOR_Delphi = 64,
            VENDOR_Alfmeier = 65,
            VENDOR_Sidler = 66,
            VENDOR_Marquardt = 67,
            VENDOR_Wehrle = 68,
            VENDOR_megamos = 69,
            VENDOR_ADC = 70,
            VENDOR_BERU = 71,
            VENDOR_Valeo = 72,
            VENDOR_Magna = 73,
            VENDOR_Allison = 74,
            VENDOR_Isringhausen = 75,
            VENDOR_Grammer = 76,
            VENDOR_Funkwerk_Dabendorf = 77,
            VENDOR_Hella_Behr = 78,
            VENDOR_Pollak = 79,
            VENDOR_AKG = 80,
            VENDOR_Automotive_Lighting = 81,
            VENDOR_TAG = 82,
            VENDOR_UNITED_PARTS = 83,
            VENDOR_catem = 84,
            VENDOR_Alge = 85,
            VENDOR_Pierburg = 86,
            VENDOR_Brusa = 87,
            VENDOR_Ecostar = 88,
            VENDOR_Xcellsis = 89,
            VENDOR_Wabco_Automotive = 90,
            VENDOR_Voith = 91,
            VENDOR_Knorr = 92,
            VENDOR_TVI = 93,
            VENDOR_Stoneridge = 94,
            VENDOR_Telma = 95,
            VENDOR_STW = 96,
            VENDOR_Koyo = 97,
            VENDOR_Eberspacher = 98,
            VENDOR_ADVICS = 99,
            VENDOR_OMRON = 100,
            VENDOR_Mitsubishi_Heavy_Industry = 101,
            VENDOR_Methode = 102,
            VENDOR_UNISIAJECS = 103,
            VENDOR_UNISIA_JKC_Steering_Systems = 104,
            VENDOR_AISIN = 105,
            VENDOR_Zexel_Valeo = 106,
            VENDOR_Schrader = 107,
            VENDOR_Ballard = 108,
            VENDOR_SFT = 112,
            VENDOR_Kieckert_AG = 113,
            VENDOR_Behr = 114,
            VENDOR_MB_Lenkungen = 115,
            VENDOR_Sachs_Automotive = 116,
            VENDOR_Peiker = 117,
            VENDOR_Petri = 118,
            VENDOR_Autoliv = 119,
            VENDOR_Thien_Electronic = 120,
            VENDOR_Siemens_VDO = 121,
            VENDOR_Dornier_Consulting_GmbH = 122,
            VENDOR_Alps = 123,
            VENDOR_PREH = 124,
            VENDOR_Hitachi_Unisia = 125,
            VENDOR_Hitachi = 126,
            VENDOR_Reserved_127 = 127,
            VENDOR_Huntsville = 128,
            VENDOR_Yazaki = 129,
            VENDOR_Lear = 130,
            VENDOR_Johnson_Controls = 131,
            VENDOR_HarmanBecker = 132,
            VENDOR_Mitsubishi_Electric = 133,
            VENDOR_Tokico_USA_Inc = 134,
            VENDOR_Nippon_Seiki_International = 135,
            VENDOR_Inalfa = 136,
            VENDOR_Nippon_Seiki_UK = 137,
            VENDOR_GHSP = 138,
            VENDOR_Vector = 139,
            VENDOR_Gentex = 140,
            VENDOR_Visteon = 141,
            VENDOR_Tochigi_Fuji = 142,
            VENDOR_DCA = 143,
            VENDOR_May_and_Scofield = 144,
            VENDOR_DaimlerChrysler_Hamburg_Plant = 145,
            VENDOR_AISIN_AW = 146,
            VENDOR_TOYODA_MACHINE_WORKS = 147,
            VENDOR_Solectron_Invotronics = 148,
            VENDOR_Kicker = 149,
            VENDOR_American_Axle_Company = 150,
            VENDOR_GETRAG = 151,
            VENDOR_Promate = 152,
            VENDOR_ArvinMeritor = 153,
            VENDOR_Reserved_MMC = 160,
            VENDOR_Reserved_MMC_SMART = 161,
            VENDOR_Reserved_162 = 162,
            VENDOR_After_Market_Supplier = 254,
            VENDOR_Unidentified = 255,
        }
    }
}
