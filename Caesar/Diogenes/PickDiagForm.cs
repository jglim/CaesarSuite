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
    public partial class PickDiagForm : Form
    {
        public DiagService[] DiagServices = new DiagService[] { };
        public DiagService SelectedDiagService = null;
        public PickDiagForm(DiagService[] diagServices)
        {
            DiagServices = diagServices;
            InitializeComponent();
        }

        private void PickDiagForm_Load(object sender, EventArgs e)
        {
            UnmanagedUtility.SendMessage(txtFilter.Handle, UnmanagedUtility.EM_SETCUEBANNER, 0, "Search for a diagnostic service..");
            EnableDoubleBuffer(dgvMain, true);
            PresentDiagServices();
        }

        public static void EnableDoubleBuffer(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        private void PresentDiagServices()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Index", typeof(String));
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("Function Type", typeof(String));
            dt.Columns.Add("Access Level", typeof(String));
            dt.Columns.Add("Security Access Level", typeof(String));
            dt.Columns.Add("Input Parameters", typeof(String));
            dt.Columns.Add("Output Parameters", typeof(String));
            dt.Columns.Add("Command", typeof(String));

            string namePartialMatch = txtFilter.Text.ToLower();

            for (int i = 0; i < DiagServices.Length; i++) 
            {
                DiagService diag = DiagServices[i];

                if (diag.Qualifier.ToLower().Contains(namePartialMatch))
                {
                    dt.Rows.Add(new string[]
                    {
                        i.ToString(),
                        diag.Qualifier,
                        diag.IsExecutable == 1 ? "User" : "System" ,
                        diag.ClientAccessLevel.ToString(),
                        diag.SecurityAccessLevel.ToString(),
                        diag.InputPreparations.Count.ToString(),
                        diag.OutputPresentations.Count.ToString(),
                        BitUtility.BytesToHex(diag.RequestBytes, true),
                    });
                }
            }

            dgvMain.DataSource = dt;

            int colIndex = 0;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[colIndex++].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvMain.Columns[0].Visible = false;
            dgvMain.Sort(dgvMain.Columns[0], ListSortDirection.Ascending);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            PresentDiagServices();
        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMain.SelectedRows.Count == 1)
            {
                int selectedDiagIndex = int.Parse(dgvMain.SelectedRows[0].Cells[0].Value.ToString());
                SelectedDiagService = DiagServices[selectedDiagIndex];
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
