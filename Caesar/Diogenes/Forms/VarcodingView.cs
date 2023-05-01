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
        public VarcodingView()
        {
            InitializeComponent();
            PopulateVcdPicker();
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
            foreach (var fragment in vcd.VCFragments) 
            {
                Console.WriteLine($"{fragment.Qualifier}");
            }
            
        }
    }
}
