using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    // TextWriter that writes to a textbox control. Used when redirect console output to a winforms textbox
    public class TextboxWriter : TextWriter
    {
        private TextBox InputTextbox;
        private Timer timer;
        private StringBuilder sb = new StringBuilder();
        private readonly object WriteLock = new object();
        bool RenderActive = true;

        public TextboxWriter(TextBox inputTextbox, int refreshInterval = 50)
        {
            InputTextbox = inputTextbox;
            InputTextbox.Text = string.Empty;

            timer = new Timer();
            timer.Interval = refreshInterval;
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
        }

        public void PauseRendering() 
        {
            RenderActive = false;
        }

        public void ResumeRendering() 
        {
            RenderActive = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!RenderActive) 
            {
                return;
            }

            lock (WriteLock)
            {
                // using a timer since invoking an event on every character write gets expensive quickly
                if (sb.Length > 0) //  || (InputTextbox.Text.Length != sb.Length)
                {
                    try
                    {
                        InputTextbox.AppendText(sb.ToString());
                        sb = new StringBuilder();
                        InputTextbox.SelectionStart = InputTextbox.TextLength;
                        InputTextbox.ScrollToCaret();
                    }
                    catch 
                    {
                        // sometimes this hits during form closing and the textbox is disposed, nothing that we need to do in this case
                    }
                }
            }
        }

        // fix append for crossthread : https://stackoverflow.com/questions/12645351/stringbuilder-tostring-throw-an-index-out-of-range-exception
        public override void Write(char value)
        {
            lock (WriteLock)
            {
                sb.Append(value);
            }
        }

        public override void Write(string value)
        {
            lock (WriteLock)
            {
                sb.Append(value);
            }
        }

        public void Clear()
        {
            lock (WriteLock)
            {
                sb = new StringBuilder();
                InputTextbox.Text = string.Empty;
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        public override void Close()
        {
            Console.WriteLine("Closing writer");
            lock (WriteLock)
            {
                timer.Enabled = false;
            }
            base.Close();
        }
    }
}
