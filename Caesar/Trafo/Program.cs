using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caesar;
using System.IO;
using Newtonsoft.Json;

namespace Trafo
{
    class Program
    {
        static void Main(string[] args)
        {

#if DEBUG
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\VGSNAG2.CBF";
#else
            if (args.Length == 0)
            {
                Console.WriteLine("Please run Trafo with a target CBF file as a parameter");
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

            List<object> ecuList = new List<object>();
            foreach (ECU ecu in container.CaesarECUs) 
            {
                List<object> variantList = new List<object>();
                foreach (ECUVariant variant in ecu.ECUVariants) 
                {
                    List<object> domainList = new List<object>();
                    foreach (VCDomain domain in variant.VCDomains)
                    {
                        List<object> fragmentList = new List<object>();
                        foreach (VCFragment fragment in domain.VCFragments) 
                        {
                            var subfragmentList = new List<object>();
                            foreach (VCSubfragment subfragment in fragment.Subfragments) 
                            {
                                var subfragmentRow = new { SubfragmentName = subfragment.NameCTFResolved, HexData = BitUtility.BytesToHex(subfragment.Dump) };
                                subfragmentList.Add(subfragmentRow);
                            }
                            var fragmentRow = new { FragmentName = fragment.Qualifier, BitPosition = fragment.ByteBitPos, BitSize = fragment.BitLength, Subfragments = subfragmentList };
                            fragmentList.Add(fragmentRow);
                        }
                        var domainRow = new { DomainName = domain.Qualifier, ByteSize = domain.DumpSize, ReadService = domain.ReadServiceName, WriteService = domain.WriteServiceName, VarcodingFragments = fragmentList };
                        domainList.Add(domainRow);
                    }
                    var variantRow = new { VariantName = variant.Qualifier, VarcodingDomains = domainList };
                    variantList.Add(variantRow);
                }
                var ecuRow = new { ECUName = ecu.Qualifier, ECUDescription = ecu.ECUDescription, ECUVariants = variantList };
                ecuList.Add(ecuRow);
            }

            var caesarContainerJson = new { TrafoVersion = GetVersion(), OriginalFile = Path.GetFileName(path), ECUs = ecuList };

            string newFilename = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(path)}.json";

            File.WriteAllText(newFilename, JsonConvert.SerializeObject(caesarContainerJson));
            Console.WriteLine($"Converted CBF file to JSON at {newFilename}");
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
