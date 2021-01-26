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
            // RunLibraryTest();
            Console.WriteLine("Done, press any key to exit");
            Console.ReadKey();
        }

        static void RunLibraryTest() 
        {
            // debug: step through files to observe potential faults, missing bitflags etc.
            List<string> paths = new List<string>();
            string basePath = @"";
            LoadFilePaths(basePath + @"Data05.00.00\", paths);
            LoadFilePaths(basePath + @"CBF VAN\", paths);
            foreach (string file in paths)
            {
                Console.WriteLine(file);
                CaesarContainer container = new CaesarContainer(File.ReadAllBytes(file));
                //Console.ReadKey();
            }
        }

        static void LoadFilePaths(string path, List<string> result)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file).ToLower() == ".cbf")
                {
                    result.Add(file);
                }
            }
        }
    }
}
