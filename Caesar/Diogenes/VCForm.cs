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
        ECUVariant ECUVariant;
        VCDomain VariantCodingDomain;
        string ECUName;
        string VariantName;
        string VCDomainName;
        public byte[] VCValue;

        DiagService ReadService;
        DiagService WriteService;

        public VCForm(CaesarContainer container, string ecuName, string variantName, string vcDomain, ECUConnection connection)
        {
            InitializeComponent();

            ECUName = ecuName;
            VariantName = variantName;
            VCDomainName = vcDomain;

            ECUVariant = container.GetECUVariantByName(variantName);
            VariantCodingDomain = ECUVariant.GetVCDomainByName(VCDomainName);

            ReadService = ECUVariant.GetDiagServiceByName(VariantCodingDomain.ReadServiceName);
            WriteService = ECUVariant.GetDiagServiceByName(VariantCodingDomain.WriteServiceName);
            if ((ReadService is null) || (WriteService is null))
            {
                Console.WriteLine("VC Dialog: Unable to proceed - could not find referenced diagnostic services");
                this.Close();
            }

            VCValue = new byte[VariantCodingDomain.DumpSize];
            foreach (Tuple<string, byte[]> row in VariantCodingDomain.DefaultData)
            {
                if (row.Item1.ToLower() == "default" && (row.Item2.Length == VariantCodingDomain.DumpSize))
                {
                    VCValue = row.Item2;
                    Console.WriteLine("Default variant coding data has been found and loaded");
                    break;
                }
            }

            if (connection.State == ECUConnection.ConnectionState.EcuContacted)
            {
                Console.WriteLine($"Requesting variant coding read: {ReadService.Qualifier} : ({BitUtility.BytesToHex(ReadService.RequestBytes)})");
            }
            else
            {
                Console.WriteLine("Please check for connectivity to the target ECU (could not read variant coding data)");
                MessageBox.Show("Variant Coding dialog will operate as a simulation using default values.", "Unable to read ECU variant coding data", MessageBoxButtons.OK);
                btnApply.Enabled = false;
            }

            VCSanityCheck();
            IntepretVC();
            PresentVC();
        }

        private void VCSanityCheck() 
        {
            VariantCodingDomain.VCFragments.ForEach(x => Console.WriteLine($"Fragment: {x.Qualifier}, R:{x.ReadAccessLevel} W:{x.WriteAccessLevel}"));
            Console.WriteLine($"ReadSvc level {ReadService.ClientAccessLevel}/{ReadService.SecurityAccessLevel}, WriteSvc level {WriteService.ClientAccessLevel}/{WriteService.SecurityAccessLevel}");

            Console.WriteLine($"ReadService: {ReadService.Qualifier} : {BitUtility.BytesToHex(ReadService.RequestBytes)} ({ReadService.RequestBytes.Length * 8})");
            ReadService.InputPreparations.ForEach(x => Console.WriteLine($"{x.Qualifier} @ {x.BitPosition}, size: {x.SizeInBits}"));

            Console.WriteLine($"WriteService: {WriteService.Qualifier} : {BitUtility.BytesToHex(WriteService.RequestBytes)} ({WriteService.RequestBytes.Length * 8})");
            WriteService.InputPreparations.ForEach(x => Console.WriteLine($"{x.Qualifier} @ {x.BitPosition}, size: {x.SizeInBits}"));
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

                dt.Rows.Add(new string[] { currentFragment.Qualifier, subfragment is null ? "(warning: no matching subfragment)" : subfragment.NameCTFResolved });
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
            if (!BitUtility.CheckHexValid(txtCodingString.Text)) 
            {
                MessageBox.Show("Hex data could not be parsed. Please check if there are any invalid values", "Write Variant Coding");
                return;
            }

            VCValue = BitUtility.BytesFromHex(txtCodingString.Text);
            ReinterpretVC();
            PresentVC();

            if (MessageBox.Show("The SCN (Software Calibration Number) will be reset for some ECUs when the variant coding is modified. Continue?", "SCN Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            {
                Console.WriteLine($"Write: {BitUtility.BytesToHex(WriteService.RequestBytes)}");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnReinterpret_Click(object sender, EventArgs e)
        {
            if (!BitUtility.CheckHexValid(txtCodingString.Text))
            {
                MessageBox.Show("Hex data could not be parsed. Please check if there are any invalid values", "Reinterpret Variant Coding");
                return;
            }

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
                VCFragment fragment = VariantCodingDomain.VCFragments.Find(x => x.Qualifier == fragmentName);
                if (fragment is null)
                {
                    return;
                }

                int contextMenuLimit = 100;

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
                    tsItem.Text = subfragment.NameCTFResolved;
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

            VCFragment fragment = VariantCodingDomain.VCFragments.Find(x => x.Qualifier == fragmentName);
            if (fragment is null) 
            {
                Console.WriteLine("Coding context menu: couldn't find a matching fragment");
                return;
            }
            VCSubfragment subfragment = fragment.Subfragments.Find(x => x.NameCTFResolved == fragmentValue);
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
