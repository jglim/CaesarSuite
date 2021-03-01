using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    // trace is generally quite hacky since I did not plan for this early
    public partial class TraceForm : Form
    {
        MainForm ParentMainForm = new MainForm();
        Timer RefreshTimer = new Timer();
        public TraceForm(MainForm parentMainForm)
        {
            ParentMainForm = parentMainForm;
            InitializeComponent();
        }

        private void TraceForm_Load(object sender, EventArgs e)
        {
            RefreshTimer.Interval = 50;
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if ((ParentMainForm is null) || (ParentMainForm.Connection is null)) 
            {
                return;
            }
            if (txtTrace.Text.Length != ParentMainForm.Connection.CommunicationsLogHighLevel.Length) 
            {
                lock (ParentMainForm.Connection.WriteLock) 
                {
                    txtTrace.Text = ParentMainForm.Connection.CommunicationsLogHighLevel.ToString();
                }
                txtTrace.SelectionStart = txtTrace.TextLength;
                txtTrace.ScrollToCaret();
            }
            
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtTrace.Text);
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your trace";
            sfd.Filter = "txt file (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.FileName = $"Trace_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, txtTrace.Text);
            }

        }
    }
}
