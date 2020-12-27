using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caesar;

namespace Diogenes
{
    public partial class SecurityLevelForm : Form
    {
        public int RequestedSecurityLevel = -1;
        public byte[] InputChallenge = new byte[] { };
        public byte[] KeyResponse = new byte[] { };
        DllContext Context;

        private const string invalidSeedPrompt = "(double-click to request for seed)";
        public SecurityLevelForm(DllContext context)
        {
            InitializeComponent();
            Context = context;
        }

        private void PresentKeys(byte[] seed) 
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Access Level", typeof(String));
            dt.Columns.Add("Key", typeof(String));

            for (int i = 0; i < Context.AccessLevels.Count; i++)
            {
                if (Context.AccessLevels[i].Item3 == seed.Length)
                {
                    DataRow row = dt.Rows.Add(new string[] { Context.AccessLevels[i].Item1.ToString(), Context.GenerateKeyAuto(Context.AccessLevels[i].Item1, seed) });
                }
                else
                {
                    dt.Rows.Add(new string[] { Context.AccessLevels[i].Item1.ToString(), invalidSeedPrompt });
                }
            }

            dgvMain.DataSource = dt;
            dgvMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMain.Sort(dgvMain.Columns[0], ListSortDirection.Ascending);
        }

        private void SecurityLevelForm_Load(object sender, EventArgs e)
        {
            // dj_zugriffsberechtigung
            this.Text = $"Configure Security Level - {Context.ECUName}";
            txtChallenge.Text = BitUtility.BytesToHex(InputChallenge, true);
            PresentKeys(new byte[] { });
        }

        private void txtCodingString_TextChanged(object sender, EventArgs e)
        {
            TryRefreshKey();
        }
        public void TryRefreshKey()
        {
            bool validHex = true;
            string cleanedText = txtChallenge.Text.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("-", "").ToUpper();
            if (cleanedText.Length % 2 != 0)
            {
                validHex = false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanedText, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                validHex = false;
            }

            if (validHex)
            {
                byte[] seed = BitUtility.BytesFromHex(cleanedText);
                txtChallenge.BackColor = System.Drawing.SystemColors.Window;
                PresentKeys(seed);
            }
            else
            {
                if (cleanedText.Length == 0)
                {
                    PresentKeys(new byte[] { });
                }
                txtChallenge.BackColor = System.Drawing.Color.LavenderBlush;
            }
        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMain.SelectedRows.Count == 1)
            {
                int selectedLevel = int.Parse(dgvMain.SelectedRows[0].Cells[0].Value.ToString());
                string selectedValue = dgvMain.SelectedRows[0].Cells[1].Value.ToString();
                if (selectedValue == invalidSeedPrompt)
                {
                    Console.WriteLine($"Security Access: Requesting seed for level {selectedLevel}");
                    RequestSeed(selectedLevel);
                }
                else 
                {
                    KeyResponse = BitUtility.BytesFromHex(selectedValue);
                    RequestedSecurityLevel = selectedLevel;
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void RequestSeed(int accessLevel) 
        {
            // since we have no connection at the moment, feed in a dummy value
            int seedSize = Context.AccessLevels.Find(x => x.Item1 == accessLevel).Item3;
            txtChallenge.Text = BitUtility.BytesToHex(new byte[seedSize]);
        }
    }
}
