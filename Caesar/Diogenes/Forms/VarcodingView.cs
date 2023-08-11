using Caesar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes.Forms
{
    public partial class VarcodingView : UserControl
    {

        BindingList<VCFragmentView> Fragments = new BindingList<VCFragmentView>();

        public VarcodingView()
        {
            InitializeComponent();
            PopulateVcdPicker();

            // fragment datagrid
            dgvVcFragments.DataSource = Fragments;
            for (int i = 1; i < dgvVcFragments.Columns.Count; i++)
            {
                dgvVcFragments.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvVcFragments.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        public void NotifyCbfOrVariantChange()
        {
            PopulateVcdPicker();
        }

        private void PopulateVcdPicker() 
        {
            cbVcdPicker.Items.Clear();
            cbVcdPicker.Enabled = true;

            if (DiogenesSharedContext.Singleton.PrimaryVariant != null) 
            {
                DiogenesSharedContext.Singleton.PrimaryVariant.VCDomains.ForEach(x => cbVcdPicker.Items.Add(x));
            }
            if (cbVcdPicker.Items.Count == 0)
            {
                cbVcdPicker.Enabled = false;
            }
        }

        private void cbVcdPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vcd = cbVcdPicker.SelectedItem as VCDomain;
            if (vcd is null) 
            {
                return;
            }
            Console.WriteLine($"R:{vcd.ReadServiceName}, W:{vcd.WriteServiceName}");

            Fragments.Clear();
            foreach (var fragment in vcd.VCFragments) 
            {
                //Console.WriteLine($"{fragment}");
                Fragments.Add(new VCFragmentView(vcd, fragment));
            }

            foreach (var huh in vcd.DefaultData) 
            {
                Console.WriteLine($"{huh.Item1} {BitConverter.ToString(huh.Item2)}");
            }
            
        }
    }
}
