using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    /// <summary>
    /// Writes to two TextWriter destinations simultaneously.
    /// Used to tee Console.Out to both the UI and a log file.
    /// </summary>
    public class TeeWriter : TextWriter
    {
        private readonly TextWriter _first;
        private readonly TextWriter _second;

        public TeeWriter(TextWriter first, TextWriter second)
        {
            _first = first;
            _second = second;
        }

        public override Encoding Encoding => _first.Encoding;

        public override void Write(char value)
        {
            _first.Write(value);
            _second.Write(value);
        }

        public override void Write(string value)
        {
            _first.Write(value);
            _second.Write(value);
        }

        public override void WriteLine(string value)
        {
            _first.WriteLine(value);
            _second.WriteLine(value);
        }

        public override void Flush()
        {
            _first.Flush();
            _second.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _first?.Dispose();
                _second?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    static class Program
    {
        public static StreamWriter LogFileWriter { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set up log file next to the executable
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string logFileName = $"Diogenes_{DateTime.Now:yyyyMMdd_HHmmss}.log";
            string logFilePath = Path.Combine(exeDir, logFileName);

            try
            {
                LogFileWriter = new StreamWriter(logFilePath, append: false, encoding: Encoding.UTF8)
                {
                    AutoFlush = true
                };

                // Tee Console.Out to both the existing writer and the log file
                TextWriter originalOut = Console.Out;
                Console.SetOut(new TeeWriter(originalOut, LogFileWriter));

                Console.WriteLine($"[LOG] Session started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"[LOG] Log file: {logFilePath}");
            }
            catch (Exception ex)
            {
                // Non-fatal: log file couldn't be created, just continue without it
                System.Diagnostics.Debug.WriteLine($"Failed to create log file: {ex.Message}");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            LogFileWriter?.Flush();
            LogFileWriter?.Close();
        }
    }
}
