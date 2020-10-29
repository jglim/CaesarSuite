using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    public class TextboxWriter : TextWriter
    {
        private TextBox InputTextbox;
        private Timer timer;
        private StringBuilder sb = new StringBuilder();
        private bool dirty = false;
        public TextboxWriter(TextBox inputTextbox)
        {
            InputTextbox = inputTextbox;
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dirty)
            {
                InputTextbox.Text = sb.ToString();
                dirty = false;
                InputTextbox.SelectionStart = InputTextbox.TextLength;
                InputTextbox.ScrollToCaret();
            }
        }

        public override void Write(char value)
        {
            sb.Append(value);
            dirty = true;
        }

        public override void Write(string value)
        {
            sb.Append(value);
            dirty = true;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}
