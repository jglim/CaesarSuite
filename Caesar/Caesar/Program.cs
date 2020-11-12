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
        // During normal operation, this class is completely ignored
        // The project can be temporarily switched from class library to console application to run as a standalone binary
        static void Main(string[] args)
        {
            Console.WriteLine("Caesar (running as console application)");

            byte[] cbfBytes = File.ReadAllBytes("MED40.CBF");
            //byte[] cbfBytes = File.ReadAllBytes("LRSM222.CBF");
            //byte[] cbfBytes = File.ReadAllBytes("CRD3S2.CBF");
            //byte[] cbfBytes = File.ReadAllBytes("IC222.CBF");
            //byte[] cbfBytes = File.ReadAllBytes("VGSNAG2.CBF");

            using (System.Security.Cryptography.SHA1 hashInstance = System.Security.Cryptography.SHA1.Create())
            {
                Console.WriteLine($"SHA1: {BitUtility.BytesToHex(hashInstance.ComputeHash(cbfBytes))}");
            }


            CaesarContainer container = new CaesarContainer(cbfBytes);
            //TryDecodeVC(container);
            // DebugPrint(container);
            // DebugFindVCDExtras(container);
            // container.CaesarCFFHeader.PrintDebug();
            // DebugFindEcuVariantIdentifier(container);
            // DebugFindDiag(container);
            // container.CaesarECUs[0].PrintDebug();
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
                string name = subfragment is null ? "(null)" : subfragment.NameCTFResolved;
                Console.WriteLine($"DVC: {fragment.Qualifier} : {name}");
            }

            Console.WriteLine(BitUtility.BytesToDecimalString(vc));
            Console.WriteLine(BitUtility.BytesToHex(vc));

            foreach (VCFragment fragment in vcdomain.VCFragments) 
            {
                if (fragment.Qualifier == "Vmax") 
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
                    if (variant.Qualifier != "VGS2_8101")
                    {
                        continue;
                    }
                    foreach (VCDomain domain in variant.VCDomains)
                    {
                        if (domain.Qualifier != "VCD_SCN_Variantencodierung_VGS_72")
                        {
                            continue;
                        }
                        Console.WriteLine($"Domain: {domain.Qualifier}");
                        domain.PrintDebug();
                    }
                }
            }
        }
        private static void DebugFindDiag(CaesarContainer container)
        {

            foreach (ECU ecu in container.CaesarECUs)
            {
                ECUVariant variant = ecu.ECUVariants.Find(x => x.Qualifier == "VC8_Update_6_CAM");
                if (variant != null)
                {
                    foreach (DiagService diag in variant.DiagServices)
                    {
                        // SES_Extended_P2_CAN_ECU_max_1_physical
                        // DNU_Send_Key_Variantcoding_Send
                        if (diag.Qualifier == "WVC_Exhaust_Regulation_Or_Type_Approval_Number_Write")
                        //if (diag.DataClass_ServiceType == (int)DiagService.ServiceType.Session)
                        {
                            Console.WriteLine($"D: {diag.Qualifier} : {BitUtility.BytesToHex(diag.RequestBytes)} ({diag.DataClass_ServiceType})");
                            diag.PrintDebug();

                            /*
                            foreach (ComParameter cp in diag.DiagComParameters)
                            {
                                Console.WriteLine("\n---comparam---");
                                cp.PrintDebug();
                            }
                            comparams: CP_REQUESTTYPE for both subinterfaces (what is this used for?)
                             */

                            foreach (DiagPreparation prep in diag.InputPreparations)
                            {
                                Console.WriteLine("\n---in prep---");
                                prep.PrintDebug();
                            }
                            foreach (List<DiagPreparation> prepList in diag.OutputPresentations)
                            {
                                foreach (DiagPreparation prep in prepList)
                                {
                                    Console.WriteLine("\n---out pres---");
                                    prep.PrintDebug();
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("------- ecu debug -----------");
                ecu.PrintDebug();
            }
        }
        private static void DebugFindEcuVariantIdentifier(CaesarContainer container)
        {
            // 
            // container.CaesarCFFHeader.PrintDebug();
            foreach (ECU ecu in container.CaesarECUs)
            {
                /*
                foreach (ECUInterface iface in ecu.ECUInterfaces)
                {
                    iface.PrintDebug();
                }

                foreach (ECUInterfaceSubtype iface in ecu.ECUInterfaceSubtypes)
                {
                    iface.PrintDebug();
                }
                foreach (ECUVariant variant in ecu.ECUVariants) 
                {

                }
                */
                foreach (ECUVariant variant in ecu.ECUVariants) 
                {
                    if (variant.Qualifier == "VC8_Update_6_CAM" || true)
                    {
                        // variant.DiagServices.ForEach(x => Console.WriteLine(x.qualifierName));
                        // DT_RVC_HEX_Variantencodierung ?
                        /*
WVC_Implizite_Variantenkodierung_Write
RVC_Implizite_Variantenkodierung_Read
DL_WVC_HEX_Variantencodierung
DNU_Level10_Key_Send
DNU_Level9_Seed_Request
DNU_Request_Seed_Reprogramming_Request
DNU_Request_Seed_Variantcoding_Request
DNU_Send_Key_Reprogramming_Send
DNU_Send_Key_Variantcoding_Send
DJ_Zugriffsberechtigung
DJ_Zugriffsberechtigung_Abgleich
                         */
                        /*
                        >>MED40 seed request : 27 0B (DNU_Request_Seed_Variantcoding_Request)
                        MED40>> 67 0B XX XX XX XX XX
                        >>MED40 accesslevel change request: 27 0C XX XX XX XX   (DNU_Send_Key_Variantcoding_Send)
                        MED40>> 67 0C (OK), 7F XX XX (negative response)


                        DiagServices does not have any 7F prefixes (NegResponse) or 67 prefixes (REQ)
                         */


                        // vgs: DL_Notlaufvariante_PT_SHW has blocks that have more than 1byte
                        //Console.WriteLine("\n--------------------\n");
                        DiagService vread = variant.GetDiagServiceByName("DL_Notlaufvariante_PT_SHW");
                        if (vread is null) 
                        {
                            continue;
                        }
                        //Console.WriteLine($"{vread.qualifierName} : command: {BitUtility.BytesToHex(vread.RequestBytes)}");
                        //vread.PrintDebug();

                        foreach (DiagPreparation prep in vread.InputPreparations) 
                        {
                            //prep.PrintDebug();
                        }
                        //vread.Preparations.ForEach(x => Console.WriteLine($"{x.qualifier} : DumpSize: {x.DumpSize}"));

                    }
                }
            }
        }
        private static void DebugParseDiagjobs(CaesarContainer container)
        {
            foreach (ECU ecu in container.CaesarECUs)
            {
                
            }
        }

        private static void DebugPrint(CaesarContainer container) 
        {

            foreach (ECU ecu in container.CaesarECUs)
            {
                Console.WriteLine($"ECU Name: {ecu.Qualifier}");
                foreach (ECUVariant variant in ecu.ECUVariants)
                {
                    if (variant.Qualifier != "VGS3_8402")
                    {
                        //continue;
                    }

                    if (true)
                    {
                        Console.WriteLine($"ECU Variant: {variant.Qualifier}");
                        Console.WriteLine($"------------------------------------------");
                        foreach (VCDomain domain in variant.VCDomains)
                        {
                            if (domain.Qualifier != "VCD_Entwicklung_Variantencodierung_VGS_73")
                            {
                                //continue;
                            }
                            Console.WriteLine($"Domain: {domain.Qualifier}");
                            //domain.PrintDebug();
                            Console.WriteLine($"Domain debug end");
                            foreach (VCFragment fragment in domain.VCFragments)
                            {
                                if (fragment.Qualifier != "fbl8_variante_ver_fls_asc1_k")
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
