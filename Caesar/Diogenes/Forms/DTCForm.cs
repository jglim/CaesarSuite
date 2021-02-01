using Caesar;
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
using static Caesar.DTC;

namespace Diogenes
{
    public partial class DTCForm : Form
    {
        ECUConnection Connection;
        ECUVariant Variant;
        
        //List<Tuple<DTC, byte>> DisplayedDTCs;
        
        List<DTCContext> DTCContexts = new List<DTCContext>();

        public DTCForm(ECUConnection connection, ECUVariant variant)
        {
            Connection = connection;
            Variant = variant;
            InitializeComponent();
        }

        private void DTCForm_Load(object sender, EventArgs e)
        {
            EnableDoubleBuffer(dgvMain, true);
            EnableDoubleBuffer(dgvContext, true);

            QueryDTC();

            PresentDtcData();
        }

        private void PresentDtcData()
        {
            if (DTCContexts.Count == 0)
            {
                MessageBox.Show("ECU reports zero Diagnostic Trouble Codes", "No DTCs");
            }
            PresentDTCs();
            PresentEnvironmentContext();
        }

        private void QueryDTC() 
        {
            GenericLoader loader = new GenericLoader();
            loader.Text = "Querying DTCs";
            loader.Show();
            Application.DoEvents();

            // fetch the base dtc value first
            DTCContexts = Connection.ConnectionProtocol.ReportDtcsByStatusMask(Connection, Variant);
            loader.SetProgressMax(DTCContexts.Count);
            Application.DoEvents();

            // query individual dtcs to retrieve the environment context
            for (int j = 0; j < DTCContexts.Count; j++)
            {
                DTCContext dtcCtx = DTCContexts[j];
                loader.Text = $"Querying DTC {dtcCtx.DTC.Qualifier} [{j + 1}/{DTCContexts.Count}]";
                loader.SetProgressValue(j);
                Application.DoEvents();

                if (!Connection.ConnectionProtocol.GetDtcSnapshot(dtcCtx.DTC, Connection, out byte[] snapshot)) 
                {
                    dtcCtx.EnvironmentContext = new List<string[]>();
                    dtcCtx.EnvironmentContext.Add(new string[] { "N/A", "N/A" });
                    continue;
                }
                List<DiagService> dtcEnvs = Variant.GetEnvironmentContextsForDTC(dtcCtx.DTC);
                List<string[]> envCtx = new List<string[]>();

                for (int i = 0; i < dtcEnvs.Count; i++)
                {
                    DiagService currentService = dtcEnvs[i];

                    StringBuilder presentationOutput = new StringBuilder();
                    foreach (List<DiagPreparation> wtf in currentService.OutputPreparations)
                    {
                        foreach (DiagPreparation outputPreparation in wtf)
                        {
                            /*
                            // crd3: uncomment to find a misbehaving presentation; enum does not have the correct scales, maybe there's something shaped like a scale that needs parsing
                            if (currentService.GetName().Contains("ufigkeitsz")) 
                            {
                                Console.WriteLine("dbg");
                            }
                            */
                            DiagPresentation presentation = outputPreparation.ParentECU.GlobalPresentations[outputPreparation.PresPoolIndex];
#if DEBUG
                            // way too verbose
                            /*
                            if (false)
                            {
                                presentationOutput.Append($"Position: {outputPreparation.BitPosition}, BitWidth: {outputPreparation.SizeInBits}, ");
                                presentationOutput.Append($"\r\n{ presentation.CopyMinDebug()}\r\n");
                            }
                            */
#endif
                            presentationOutput.Append(presentation.InterpretData(snapshot, outputPreparation, false));
                        }
                    }
                    envCtx.Add(new string[] { currentService.GetName(), presentationOutput.ToString() });
                }
                dtcCtx.EnvironmentContext = envCtx;
            }

            loader.Close();
        }


        private void PresentDTCs()
        {
            string[] Headers = new string[] { "Code", "Description", "Status", "Current", "Stored", "MIL On",
                "Failed: Request Time", "Failed: Current Cycle", "Failed: Last Clear", "Incomplete: Current Cycle", "Incomplete: Last Clear" };
            DataTable dt = new DataTable();

            dt.Columns.Add("Index", typeof(int));
            foreach (string header in Headers)
            {
                dt.Columns.Add(header, typeof(String));
            }
            dgvMain.DataSource = dt;

            for (int i = 0; i < DTCContexts.Count; i++)
            {
                List<string> row = new List<string>();
                DTC currentDtc = DTCContexts[i].DTC;
                byte statusByte = DTCContexts[i].StatusByte;

                bool dtcStatusIsCurrent = (statusByte & (int)DTCStatusByte.PendingDTC) > 0;
                bool dtcStatusIsStored = (statusByte & (int)DTCStatusByte.ConfirmedDTC) > 0;
                bool dtcStatusIsMILActive = (statusByte & (int)DTCStatusByte.WarningIndicatorActive) > 0;

                bool dtcStatusFailedAtRequestTime = (statusByte & (int)DTCStatusByte.TestFailedAtRequestTime) > 0;
                bool dtcStatusFailedAtCurrentCycle = (statusByte & (int)DTCStatusByte.TestFailedAtCurrentCycle) > 0;
                bool dtcStatusFailedSinceLastClear = (statusByte & (int)DTCStatusByte.TestFailedSinceLastClear) > 0;
                bool dtcStatusIncompleteAtCurrentCycle = (statusByte & (int)DTCStatusByte.TestIncompleteAtCurrentCycle) > 0;
                bool dtcStatusIncompleteSinceLastClear = (statusByte & (int)DTCStatusByte.TestIncompleteSinceLastClear) > 0;

                row.Add(currentDtc.Qualifier);
                row.Add(currentDtc.Description);
                row.Add($"{statusByte:X2}");

                row.Add($"{dtcStatusIsCurrent}");
                row.Add($"{dtcStatusIsStored}");
                row.Add($"{dtcStatusIsMILActive}");

                row.Add($"{dtcStatusFailedAtRequestTime}");
                row.Add($"{dtcStatusFailedAtCurrentCycle}");
                row.Add($"{dtcStatusFailedSinceLastClear}");
                row.Add($"{dtcStatusIncompleteAtCurrentCycle}");
                row.Add($"{dtcStatusIncompleteSinceLastClear}");

                row.Insert(0, i.ToString());
                dt.Rows.Add(row.ToArray());
            }

            for (int i = 3; i < Headers.Length; i++)
            {
                dgvMain.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            dgvMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvMain.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // was Fill, 
            dgvMain.Columns[0].Visible = false;
        }


        private void PresentEnvironmentContext()
        {
            if (dgvMain.SelectedRows.Count != 1)
            {
                return;
            }

            int selectedIndex = int.Parse(dgvMain.SelectedRows[0].Cells[0].Value.ToString());
            List<string[]> envCtx = DTCContexts[selectedIndex].EnvironmentContext;

            string[] Headers = new string[] { "Environment", "Description" };
            DataTable dt = new DataTable();

            dt.Columns.Add("Index", typeof(int));
            foreach (string header in Headers)
            {
                dt.Columns.Add(header, typeof(String));
            }
            dgvContext.DataSource = dt;
            for (int i = 0; i < envCtx.Count; i++)
            {
                List<string> row = new List<string>();
                row.Insert(0, i.ToString());
                row.AddRange(envCtx[i]);
                dt.Rows.Add(row.ToArray());
            }

            dgvContext.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvContext.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvContext.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvContext.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvContext.AutoResizeRows();

            dgvContext.Columns[0].Visible = false;
        }

        public static void EnableDoubleBuffer(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        private void clearAllDTCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Clear all DTCs?", "Confirm DTC Clear", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ClearAllDTCs();
            }
        }

        private void clearSelectedDTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvMain.SelectedRows.Count == 1)
            {
                int selectedIndex = int.Parse(dgvMain.SelectedRows[0].Cells[0].Value.ToString());
                DTC selectedDtc = DTCContexts[selectedIndex].DTC;
                if (MessageBox.Show($"Clear {selectedDtc.Qualifier}?", "Confirm DTC Clear", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ClearDTC(selectedDtc);
                }
            }
        }

        private void ClearAllDTCs()
        {
            byte[] request = new byte[] { 0x14, 0xFF, 0xFF, 0xFF }; // essentially ClearDTC a mask of FFFFFF
            byte[] expectedResponse = new byte[] { 0x59 };
            byte[] response = Connection.SendMessage(request);
            if (!(response.Take(expectedResponse.Length).SequenceEqual(expectedResponse)))
            {
                MessageBox.Show($"Clear request (for all DTCs) was rejected", "Negative Response");
            }
            else
            {
                QueryDTC();
                PresentDtcData();
            }
        }

        private void ClearDTC(DTC dtc)
        {
            if (dtc.Qualifier.Length != 7)
            {
                MessageBox.Show("Internal error: DTC qualifier is invalid, exiting prematurely.");
                return;
            }
            byte[] identifier = BitUtility.BytesFromHex(dtc.Qualifier.Substring(1));
            byte[] request = new byte[] { 0x14, identifier[0], identifier[1], identifier[2] };
            byte[] expectedResponse = new byte[] { 0x59 };
            byte[] response = Connection.SendMessage(request);
            if (!(response.Take(expectedResponse.Length).SequenceEqual(expectedResponse)))
            {
                MessageBox.Show($"Clear request ({dtc.Qualifier}) was rejected", "Negative Response");
            }
            else
            {
                QueryDTC();
                PresentDtcData();
            }
        }

        private void refreshDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryDTC();
            PresentDtcData();
        }

        private void dgvMain_SelectionChanged(object sender, EventArgs e)
        {
            PresentEnvironmentContext();
        }

        private void exportDTCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DTCReport.CreateDTCReport(DTCContexts, Connection, Variant);
        }

        private void viewAllAvailableDTCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string[]> rows = new List<string[]>();
            foreach (DTC dtc in Variant.DTCs) 
            {
                rows.Add(new string[] { dtc.Qualifier, dtc.Description });
            }
            GenericPicker picker = new GenericPicker(rows.ToArray(), new string[] { "Identifier", "Description" }, 0, true);
            picker.Text = $"All DTCs for {Variant.Qualifier}";
            picker.Show();
        }
    }

    public class DTCContext
    {
        public DTC DTC;
        public byte StatusByte;
        public List<string[]> EnvironmentContext;
    }
}
