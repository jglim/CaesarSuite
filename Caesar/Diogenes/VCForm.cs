using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caesar;

namespace Diogenes
{
    public partial class VCForm : Form
    {
        CaesarContainer Container;
        ECUVariant ECUVariant;
        VCDomain VariantCodingDomain;
        string ECUName;
        string VariantName;
        string VCDomainName;
        public byte[] VCValue;

        public VCForm(CaesarContainer container, string ecuName, string variantName, string vcDomain, byte[] existingCodingValue)
        {
            InitializeComponent();

            ECUName = ecuName;
            VariantName = variantName;
            VCDomainName = vcDomain;
            VCValue = existingCodingValue;

            ECUVariant = container.GetECUVariantByName(variantName);
            VariantCodingDomain = ECUVariant.GetVCDomainByName(VCDomainName);

            if (existingCodingValue.Length != VariantCodingDomain.vcdDumpSize) 
            {
                Console.WriteLine("Existing variant coding data is unavailable, please check for connectivity to the target ECU.");
                // try to read out a default if it is available
                VCValue = new byte[VariantCodingDomain.vcdDumpSize];
                foreach (Tuple<string, byte[]> row in VariantCodingDomain.DefaultData) 
                {
                    if (row.Item1.ToLower() == "default" && (row.Item2.Length == VariantCodingDomain.vcdDumpSize)) 
                    {
                        VCValue = row.Item2;
                        Console.WriteLine("Default variant coding data has been found and loaded");
                        break;
                    }
                }
            }
            IntepretVC();
            PresentVC();
        }

        private void PresentVC() 
        {
            txtCodingString.Text = rbHex.Checked ? BitUtility.BytesToHex(VCValue, true) : BitUtility.BytesToDecimalString(VCValue);
            txtCodingString.Select(0, 0);
        }

        private void ReinterpretVC() 
        {
            VCValue = rbHex.Checked ? BitUtility.BytesFromHex(txtCodingString.Text) : BitUtility.BytesFromDecimalString(txtCodingString.Text);
            IntepretVC();
        }

        private void IntepretVC()
        {
            // save view state
            int saveRow = 0;
            if (dgvMain.Rows.Count > 0 && dgvMain.FirstDisplayedCell != null)
            {
                saveRow = dgvMain.FirstDisplayedCell.RowIndex;
            }
            List<int> saveSelectedIndices = new List<int>();
            foreach (DataGridViewRow row in dgvMain.SelectedRows)
            {
                saveSelectedIndices.Add(row.Index);
            }

            // actually interpret vc
            DataTable dt = new DataTable();
            dt.Columns.Add("Fragment", typeof(String));
            dt.Columns.Add("Value", typeof(String));

            for (int i = 0; i < VariantCodingDomain.VCFragments.Count; i++)
            {
                VCFragment currentFragment = VariantCodingDomain.VCFragments[i];
                // DataRow row = dt.Rows.Add(new object[] { currentFragment.fragmentName, new string[] {"hi", "bye" } });
                VCSubfragment subfragment = currentFragment.GetSubfragmentConfiguration(VCValue);

                dt.Rows.Add(new string[] { currentFragment.fragmentName, subfragment is null ? "(warning: no matching subfragment)" : subfragment.subfragmentNameResolved });
            }

            dgvMain.DataSource = dt;
            dgvMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMain.Sort(dgvMain.Columns[0], ListSortDirection.Ascending);

            // restore view state
            if (saveRow != 0 && saveRow < dgvMain.Rows.Count)
            {
                dgvMain.FirstDisplayedScrollingRowIndex = saveRow;
            }
            foreach (int rowIndex in saveSelectedIndices)
            {
                foreach (DataGridViewRow row in dgvMain.Rows)
                {
                    row.Selected = row.Index == rowIndex;
                }
            }

        }

        private void VCForm_Load(object sender, EventArgs e)
        {
            SetDoubleBuffer(dgvMain, true);
        }

        static void SetDoubleBuffer(Control ctl, bool DoubleBuffered)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, ctl, new object[] { DoubleBuffered });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            VCValue = BitUtility.BytesFromHex(txtCodingString.Text);
            ReinterpretVC();
            PresentVC();

            if (MessageBox.Show("The SCN (Software Calibration Number) will be reset for some ECUs when the variant coding is modified. Continue?", "SCN Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnReinterpret_Click(object sender, EventArgs e)
        {
            ReinterpretVC();
        }

        private void rbHex_CheckedChanged(object sender, EventArgs e)
        {
            PresentVC();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            PresentVC();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtCodingString.Text);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (dgvMain.SelectedRows.Count > 0)
            {
                string fragmentName = dgvMain.SelectedRows[0].Cells[0].Value.ToString();
                string fragmentValue = dgvMain.SelectedRows[0].Cells[1].Value.ToString();
                VCFragment fragment = VariantCodingDomain.VCFragments.Find(x => x.fragmentName == fragmentName);
                if (fragment is null)
                {
                    return;
                }

                int contextMenuLimit = 50;

                contextMenuStrip1.Items.Clear();
                ToolStripMenuItem tsHeader = new ToolStripMenuItem();
                tsHeader.Text = fragmentName;
                if (fragment.Subfragments.Count > contextMenuLimit) 
                {
                    tsHeader.Text += " (Warning: too many entries to show)";
                }
                tsHeader.Enabled = false;
                contextMenuStrip1.Items.Add(tsHeader);
                contextMenuStrip1.Items.Add(new ToolStripSeparator());

                foreach (VCSubfragment subfragment in fragment.Subfragments) 
                {
                    ToolStripMenuItem tsItem = new ToolStripMenuItem();
                    tsItem.Text = subfragment.subfragmentNameResolved;
                    tsItem.Tag = fragmentName;
                    tsItem.Click += VCContextMenu_Click;
                    if (fragmentValue == tsItem.Text) 
                    {
                        tsItem.Checked = true;
                    }
                    contextMenuStrip1.Items.Add(tsItem);
                    if (--contextMenuLimit == 0) 
                    {
                        break;
                    }
                }
            }
            // we *want* to show the context menu, this makes it happen
            e.Cancel = false;
        }

        private void VCContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderItem = (ToolStripMenuItem)sender;
            string fragmentName = senderItem.Tag.ToString();
            string fragmentValue = senderItem.Text;

            VCFragment fragment = VariantCodingDomain.VCFragments.Find(x => x.fragmentName == fragmentName);
            if (fragment is null) 
            {
                Console.WriteLine("Coding context menu: couldn't find a matching fragment");
                return;
            }
            VCSubfragment subfragment = fragment.Subfragments.Find(x => x.subfragmentNameResolved == fragmentValue);
            if (subfragment is null)
            {
                Console.WriteLine("Coding context menu: couldn't find a matching subfragment");
                return;
            }
            VCValue = fragment.SetSubfragmentConfiguration(VCValue, subfragment);
            IntepretVC();
            PresentVC();
        }

    }
}
