using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    static class Program
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        const bool SeparateConsole = false;


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && SeparateConsole) 
            {
                AllocConsole();
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Title = "Diogenes Console";
                Console.Clear();
            }
            Application.Run(new Forms.MainForm());
        }
    }
}
