using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    public partial class GenericLoader : Form
    {
        public GenericLoader()
        {
            InitializeComponent();
        }
        public void SetProgressValue(int value)
        {
            progressBar1.Value = value;
        }
        public void SetProgressMax(int value)
        {
            progressBar1.Maximum = value;
        }
    }
}
