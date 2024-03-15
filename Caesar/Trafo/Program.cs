using System;
using System.Text;
using Caesar;
using System.IO;

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

            string newFilename = $"{Path.GetDirectoryName(path)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(path)}.json";

            File.WriteAllBytes(newFilename, Encoding.UTF8.GetBytes(CaesarContainer.SerializeContainer(container)));
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
