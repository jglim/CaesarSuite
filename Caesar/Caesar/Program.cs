using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Caesar
{
    class Program
    {

        // The project can be temporarily switched from class library to console application to run as a standalone binary
        static void Main(string[] args)
        {
            Console.WriteLine("Caesar (running as console application)");

            //byte[] cbfBytes = File.ReadAllBytes("MED40.CBF");
            //byte[] cbfBytes = File.ReadAllBytes("LRSM222.CBF");
            byte[] cbfBytes = File.ReadAllBytes("VGSNAG2.CBF");

            CaesarContainer container = new CaesarContainer(cbfBytes);
            //TryDecodeVC(container);
            // DebugPrint(container);
            DebugFindVCDExtras(container);
            Console.WriteLine("Done, press any key to exit");
            Console.ReadKey();

        }

        private static void TryDecodeVC(CaesarContainer container) 
        {
            // 160km/h
            //byte[] vc = new byte[] { 0x36, 0x36, 0x00, 0xE5, 0x17, 0x41, 0x41, 0x54, 0x86, 0x51, 0x85, 0x3E, 0x50, 0x14, 0x44, 0xC0, 0x00, 0x21, 0x40, 0x01, 0x10, 0x41, 0x04, 0x01, 0x00, 0x10, 0x08, 0x08, 0xA0, 0x01 };
            // 250km/h
            byte[] vc = new byte[] { 0x36, 0x36, 0x00, 0xE5, 0x17, 0x41, 0x41, 0x53, 0x86, 0x51, 0x85, 0x3E, 0x50, 0x14, 0x44, 0xC0, 0x00, 0x21, 0x40, 0x01, 0x10, 0x41, 0x04, 0x01, 0x00, 0x10, 0x08, 0x08, 0xA0, 0x01 };
            string ecuVariantName = "VC8_Update_6_CAM";
            string domainName = "VCD_Implizite_Variantenkodierung";
            ECUVariant variant = container.GetECUVariantByName(ecuVariantName);
            if (variant is null) 
            {
                throw new Exception("couldn't load ECU variant");
            }

            VCDomain vcdomain = variant.GetVCDomainByName(domainName);

            if (vcdomain is null)
            {
                throw new Exception("couldn't load domain");
            }

            foreach (VCFragment fragment in vcdomain.VCFragments) 
            {
                VCSubfragment subfragment = fragment.GetSubfragmentConfiguration(vc);
                string name = subfragment is null ? "(null)" : subfragment.subfragmentNameResolved;
                Console.WriteLine($"DVC: {fragment.fragmentName} : {name}");
            }

            Console.WriteLine(BitUtility.BytesToDecimalString(vc));
            Console.WriteLine(BitUtility.BytesToHex(vc));

            foreach (VCFragment fragment in vcdomain.VCFragments) 
            {
                if (fragment.fragmentName == "Vmax") 
                {
                    byte[] newVc = fragment.SetSubfragmentConfiguration(vc, "_160 km/h / BR906 140 km/h");
                    Console.WriteLine(BitUtility.BytesToHex(newVc));
                }
            }

        }

        private static void DebugFindVCDExtras(CaesarContainer container) 
        {
            // 

            foreach (ECU ecu in container.CaesarECUs)
            {
                foreach (ECUVariant variant in ecu.ECUVariants)
                {
                    if (variant.variantName != "VGS2_8101")
                    {
                        continue;
                    }
                    foreach (VCDomain domain in variant.VCDomains)
                    {
                        if (domain.vcdName != "VCD_SCN_Variantencodierung_VGS_72")
                        {
                            continue;
                        }
                        Console.WriteLine($"Domain: {domain.vcdName}");
                        domain.PrintDebug();
                    }
                }
            }
        }

        private static void DebugPrint(CaesarContainer container) 
        {

            foreach (ECU ecu in container.CaesarECUs)
            {
                Console.WriteLine($"ECU Name: {ecu.ecuName}");
                foreach (ECUVariant variant in ecu.ECUVariants)
                {
                    if (variant.variantName != "VGS3_8402")
                    {
                        //continue;
                    }

                    if (true)
                    {
                        Console.WriteLine($"ECU Variant: {variant.variantName}");
                        Console.WriteLine($"------------------------------------------");
                        foreach (VCDomain domain in variant.VCDomains)
                        {
                            if (domain.vcdName != "VCD_Entwicklung_Variantencodierung_VGS_73")
                            {
                                //continue;
                            }
                            Console.WriteLine($"Domain: {domain.vcdName}");
                            //domain.PrintDebug();
                            Console.WriteLine($"Domain debug end");
                            foreach (VCFragment fragment in domain.VCFragments)
                            {
                                if (fragment.fragmentName != "fbl8_variante_ver_fls_asc1_k")
                                {
                                    //continue;
                                }
                                //Console.WriteLine($"Fragment: {fragment.fragmentName}");
                                //fragment.PrintDebug();
                                foreach (VCSubfragment subfragment in fragment.Subfragments)
                                {
                                    //Console.WriteLine($"SF: {BitUtility.BytesToHex(subfragment.subfragmentDump)} {subfragment.subfragmentName2Resolved}");
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
