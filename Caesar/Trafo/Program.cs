using System;
using System.Collections.Generic;
using System.Linq;
using Caesar;
using System.IO;
using Newtonsoft.Json;

namespace Trafo
{
    class Program
    {
        static List<object> VariantList;

        static void ReadScales (CaesarContainer container, DiagPresentation presentation, List<object> scaleslist)
        {
            foreach (Scale Scale in presentation.Scales)
            {
                var ScaleRow = new
                {
                    LowBound = Scale.EnumLowBound,
                    UpBound = Scale.EnumUpBound,
                    //PrepLowBound = Scale.PrepLowBound, //commented out fields purpose unknown
                    //PrepUpBound = Scale.PrepUpBound,
                    MultiplyFactor = Scale.MultiplyFactor,
                    AddConstOffs = Scale.AddConstOffset,
                    //SICount =Scale.SICount,
                    //OffsetSI = Scale.OffsetSI,
                    //USCount = Scale.USCount,
                    //OffsetUS = Scale.OffsetUS,
                    EnumDesc = container.CaesarCTFHeader.CtfLanguages[0].GetString(Scale.EnumDescription)
                };

                scaleslist.Add(ScaleRow);
            }
        }

        static void ReadPreparation (CaesarContainer container, DiagPreparation prep, List<object> preplist)
        {
            DiagPresentation Presentation = container.CaesarECUs[0].GlobalPresentations[prep.PresPoolIndex];
            List<object> ScalesList = new List<object>();

            ReadScales(container, Presentation, ScalesList);

            var PresentationRow = new
            {
                Qualifier = Presentation.Qualifier,
                Desc = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.Description_CTF),
                Units = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.DisplayedUnit_CTF),
                EnumMaxVal = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.EnumMaxValue),
                Desc2 = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.Description2_CTF),
                Scales = ScalesList
            };

            var PreparationRow = new
            {
                Name = prep.Qualifier,
                LongName = container.CaesarCTFHeader.CtfLanguages[0].GetString(prep.Name_CTF),
                BitPos = prep.BitPosition,
                BitLen = prep.SizeInBits,
                Presentation = PresentationRow
            };

            preplist.Add(PreparationRow);
        }

        static void ReadDiagService (CaesarContainer container, DiagService diagservice, List<object> diagserviceslist)
        {
            List<object> InputPrepList = new List<object>();

            foreach (DiagPreparation InputPrep in diagservice.InputPreparations)
                ReadPreparation(container, InputPrep, InputPrepList);

            List<object> OutputPrepList = new List<object>();

            foreach (List<DiagPreparation> OutputPreps in diagservice.OutputPreparations)
            {
                foreach (DiagPreparation OutputPrep in OutputPreps)
                    ReadPreparation(container, OutputPrep, OutputPrepList);
            }

            string RequestArr = string.Join("", diagservice.RequestBytes.Select(b => string.Format("{0:X2} ", b)));
            var RequestBytesRow = new { RequestBytes = RequestArr };
            var servicetype = (DiagService.ServiceType) diagservice.DataClass_ServiceType;

            var DiagServiceRow = new
            {
                DiagServiceName = diagservice.Qualifier,
                Desc = container.CaesarCTFHeader.CtfLanguages[0].GetString(diagservice.Description_CTF),
                Type = servicetype.ToString(),
                ClientAccessLevel = diagservice.ClientAccessLevel,
                SecurityAccessLevel = diagservice.SecurityAccessLevel,
                RequestBytes = RequestArr,
                InputPreps = InputPrepList,
                OutputPreps = OutputPrepList
            };

            diagserviceslist.Add(DiagServiceRow);
        }

        static void ReadECUVariant (CaesarContainer container, ECUVariant variant )
        {
            List<object> domainList = new List<object>();

            foreach (VCDomain domain in variant.VCDomains)
            {
                DiagService ReadService = variant.GetDiagServiceByName(domain.ReadServiceName);
                DiagService WriteService = variant.GetDiagServiceByName(domain.WriteServiceName);
                string ReadArr = string.Join("", ReadService.RequestBytes.Select(b => string.Format("{0:X2} ", b)));
                string WriteArr = string.Join("", WriteService.RequestBytes.Select(b => string.Format("{0:X2} ", b)));
                
                List<object> fragmentList = new List<object>();

                foreach (VCFragment fragment in domain.VCFragments)
                {
                    List<object> PresList = new List<object>();
                    DiagPresentation Presentation = container.CaesarECUs[0].GlobalPresentations[fragment.MeaningB];
                    List<object> ScalesList = new List<object>();
                    ReadScales(container, Presentation, ScalesList);

                    var FragmentInfo = new
                    {
                        Qualifier = Presentation.Qualifier,
                        Desc = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.Description_CTF),
                        Units = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.DisplayedUnit_CTF),
                        EnumMaxVal = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.EnumMaxValue),
                        Desc2 = container.CaesarCTFHeader.CtfLanguages[0].GetString(Presentation.Description2_CTF),
                        Scales = ScalesList
                    };

                    var fragmentRow = new
                    {
                        FragName = container.CaesarCTFHeader.CtfLanguages[0].GetString(fragment.Name_CTF),
                        FragDesc = container.CaesarCTFHeader.CtfLanguages[0].GetString(fragment.Description_CTF),
                        BitPos = fragment.ByteBitPos,
                        BitLen = fragment.BitLength,
                        ReadAccessLevel = fragment.ReadAccessLevel,
                        WriteAccessLevel = fragment.WriteAccessLevel,
                        ByteOrder = fragment.ByteOrder,
                        Info = FragmentInfo
                    };

                    fragmentList.Add(fragmentRow);
                }
                var domainRow = new
                {
                    VCDName = domain.Qualifier,
                    DumpSize = domain.DumpSize,
                    ReadService = domain.ReadServiceName,
                    WriteService = domain.WriteServiceName,
                    ReadBytes = ReadArr,
                    WriteBytes = WriteArr,
                    VCDFragments = fragmentList
                };

                var RequestBytesRow = new { ReadRequestBytes = ReadArr, WriteRequestBytes = WriteArr };
                domainList.Add(domainRow);
            }

            List<object> DTCList = new List<object>();

            foreach (DTC dtc in variant.DTCs)
            {
                List<string> XrefsList = new List<string>();
                List<DiagService> EnvCtxForDTC = variant.GetEnvironmentContextsForDTC(dtc);

                for (int i = 0; i < dtc.XrefCount; i++)
                    XrefsList.Add(EnvCtxForDTC[i].Qualifier);

                var DTCRow = new
                {
                    Code = dtc.Qualifier,
                    Desc = container.CaesarCTFHeader.CtfLanguages[0].GetString(dtc.Description_CTF),
                    Ref = container.CaesarCTFHeader.CtfLanguages[0].GetString(dtc.Reference_CTF),
                    Xrefs = XrefsList
                };

                DTCList.Add(DTCRow);
            }

            List<object> EnvCtxList = new List<object>();

            foreach (DiagService EnvCtx in variant.EnvironmentContexts)
                ReadDiagService(container, EnvCtx, EnvCtxList);

            List<object> DiagServicesList = new List<object>();

            foreach (DiagService DiagService in variant.DiagServices)
                ReadDiagService(container, DiagService, DiagServicesList);

            var variantRow = new
            {
                VariantName = variant.Qualifier,
                VarcodingDomains = domainList,
                DTCs = DTCList,
                EnvironmentContexts = EnvCtxList,
                DiagServices = DiagServicesList
            };
            VariantList.Add(variantRow);
        }

        static void Main(string[] args)
        {

#if DEBUG
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\temp.CBF";
#else
            if ((args.Length == 0) || (args.Length > 2))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("trafo.exe [target CBF file] <optional>[parameter]");
                Console.WriteLine("[parameter] can be:");
                Console.WriteLine("listvar: list all variant names present in CBF file");
                Console.WriteLine("[variant name]: export varcoding, DTC, environment context, diagnostic services sections of one specified variant");
                Console.WriteLine("if no [parameter] is specified - export varcoding, DTC, environment context, diagnostic services sections of all variants");
                return;
            }

            if (!File.Exists(args[0])) 
            {
                Console.WriteLine("Specified CBF file does not exist, exiting");
                return;
            }

            string path = args[0];
#endif
            byte[] cbfBytes = File.ReadAllBytes(path);
            
            CaesarContainer container = new CaesarContainer(cbfBytes);
            string mode = "";

            if (args.Length == 2)
                mode = args[1];

            bool VariantFound = false;

            if (mode == "listvar")
            {
                string combinedString = "";

                foreach (ECU ecu in container.CaesarECUs)
                {
                    foreach (ECUVariant variant in ecu.ECUVariants)
                        combinedString = combinedString + " " + variant.Qualifier;
                }

                Console.WriteLine(combinedString);
            }
            else
            {
                VariantList = new List<object>();

                foreach (ECU ecu in container.CaesarECUs)
                {
                    foreach (ECUVariant variant in ecu.ECUVariants)
                    {
                        if (mode == "")
                        {
                            ReadECUVariant(container, variant);
                        }
                        else if (mode == variant.Qualifier)
                        {
                            VariantFound = true;
                            ReadECUVariant(container, variant);
                            break;
                        }
                    }
                }

                if (mode != "" && !VariantFound)
                {
                    Console.WriteLine("Specified variant does not exist in provided CBF file, exiting");
                    return;
                }

                string newFilename = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(path)}_{mode}.json";

                var caesarContainerJson = new
                {
                    TrafoVersion = GetVersion(),
                    OriginalFile = Path.GetFileName(path),
                    container.CaesarCFFHeader,
                    Variants = VariantList
                };

                File.WriteAllText(newFilename, JsonConvert.SerializeObject(caesarContainerJson));
                Console.WriteLine($"Converted CBF file {mode} variant to JSON at {newFilename}");
            }
            
#if DEBUG
            Console.ReadKey();
#endif
        }

        public static string GetVersion() 
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}
