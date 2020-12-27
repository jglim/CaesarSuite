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
    public partial class RunDiagForm : Form
    {
        public DiagService CurrentDiagService;
        public byte[] Result = Array.Empty<byte>();

        public RunDiagForm(DiagService diagService)
        {
            CurrentDiagService = diagService;
            Result = CurrentDiagService.RequestBytes;
            InitializeComponent();
        }

        private void RunDiagForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Run Diagnostics Service: {CurrentDiagService.Qualifier}";
            TrimIntegerDumps();
            txtDiagCommand.Text = BitUtility.BytesToHex(Result, true);

            if (!CheckBitAlignment()) 
            {
                MessageBox.Show("One or more parameters are placed between byte boundaries. This isn't supported yet.", "Parameter misalignment");
                this.Close();
                return;
            }

            PresentDiagService();
        }

        private void TrimIntegerDumps() 
        {
            // integer dumps are always saved as LE int32, even if they are not int32
            foreach (DiagPreparation prep in CurrentDiagService.InputPreparations)
            {
                if (prep.FieldType == DiagPreparation.InferredDataType.IntegerType)
                {
                    int byteSize = prep.SizeInBits / 8;
                    if (prep.Dump.Length > byteSize) 
                    {
                        prep.Dump = prep.Dump.Take(byteSize).ToArray();
                    }
                }
            }
        }

        private bool CheckBitAlignment() 
        {
            // example service that fails this check:
            /*
                MED40:
             	IOC_Getriebeanwaermung_Absperrventil_Getriebeanwaermung_Absperrventil	User	1	0	5	1	2F D1 05 03 00
             */
            foreach (DiagPreparation prep in CurrentDiagService.InputPreparations)
            {
                if (prep.BitPosition % 8 != 0)
                {
                    return false;
                }
                if (prep.SizeInBits % 8 != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void PresentDiagService() 
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Index", typeof(String));
            dt.Columns.Add("Name", typeof(String));
            dt.Columns.Add("Byte Position", typeof(String));
            dt.Columns.Add("Data Size", typeof(String));
            dt.Columns.Add("Data Type", typeof(String));
            dt.Columns.Add("Inferred Type", typeof(String));
            dt.Columns.Add("Direction", typeof(String));
            dt.Columns.Add("Raw Data", typeof(String));

            int listUniqueIndex = 0;
            for (int i = 0; i < CurrentDiagService.InputPreparations.Count; i++)
            {
                DiagPreparation prep = CurrentDiagService.InputPreparations[i];

                dt.Rows.Add(new string[]
                {
                        listUniqueIndex.ToString(),
                        prep.Qualifier,
                        (prep.BitPosition / 8).ToString(),
                        (prep.SizeInBits / 8).ToString(),
                        prep.ModeConfig.ToString("X"),
                        prep.FieldType.ToString(),
                        "Input",
                        BitUtility.BytesToHex(prep.Dump, true),
                });
                listUniqueIndex++;
            }
            for (int i = 0; i < CurrentDiagService.OutputPreparations.Count; i++)
            {
                List<DiagPreparation> currentDiagList = CurrentDiagService.OutputPreparations[i];
                for (int j = 0; j < currentDiagList.Count; j++) 
                {
                    DiagPreparation prep = CurrentDiagService.OutputPreparations[i][j];

                    dt.Rows.Add(new string[]
                    {
                        listUniqueIndex.ToString(),
                        prep.Qualifier,
                        (prep.BitPosition / 8).ToString(),
                        (prep.SizeInBits / 8).ToString(),
                        prep.ModeConfig.ToString("X"),
                        prep.FieldType.ToString(),
                        $"Output ({i})",
                        BitUtility.BytesToHex(prep.Dump, true),
                    });
                    listUniqueIndex++;
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
            dgvMain.Columns[4].Visible = false;
            // dgvMain.Columns[6].Visible = false;
            dgvMain.Sort(dgvMain.Columns[0], ListSortDirection.Ascending);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string commandAsString = txtDiagCommand.Text;
            if (!BitUtility.CheckHexValid(commandAsString))
            {
                MessageBox.Show("Hex data could not be parsed. Please check if there are any invalid values", "Write Variant Coding");
                return;
            }
            Result = BitUtility.BytesFromHex(commandAsString);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
