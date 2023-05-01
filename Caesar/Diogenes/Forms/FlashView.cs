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
using System.IO;
using CaesarConnection.Target;

namespace Diogenes.Forms
{
    public partial class FlashView : UserControl
    {
        BindingList<FlashItem> FlashItems = new BindingList<FlashItem>();
        BindingList<SoftwareBlock> EcuSoftwareBlocks = new BindingList<SoftwareBlock>();

        public FlashView()
        {
            InitializeComponent();

            dgvFlashCollection.DataSource = FlashItems;
            for (int i = 1; i < dgvFlashCollection.Columns.Count; i++)
            {
                dgvFlashCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dgvFlashCollection.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvEcuState.DataSource = EcuSoftwareBlocks;
            for (int i = 1; i < dgvEcuState.Columns.Count; i++)
            {
                dgvEcuState.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            //dgvEcuState.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CFF File";
            ofd.Filter = "CFF files (*.cff)|*.cff|All files (*.*)|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    LoadCFF(file);
                }
            }
        }

        private void LoadCFF(string fileName) 
        {
            CaesarFlashContainer container = new CaesarFlashContainer(File.ReadAllBytes(fileName));

            if (FlashItems.Count(x => x.Container.ProvidedChecksum == container.ProvidedChecksum) > 0)
            {
                Console.WriteLine($"Skipping CFF load as the requested CFF has already been loaded");
            }
            else
            {
                FlashItems.Add(new FlashItem(container));
                Console.WriteLine($"Added {container.CaesarFlashHeader.FlashName}");
            }
            FlashItems.OrderBy(x => x.Priority);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvFlashCollection.SelectedRows.Count > 0) 
            {
                FlashItems.Remove(dgvFlashCollection.SelectedRows[0].DataBoundItem as FlashItem);
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            QueryFlashState();
        }

        private void QueryFlashState() 
        {
            // with an active connection, query available flash blocks
            // no idea how this works on kw2 devices
            if (DiogenesSharedContext.Singleton.Channel is null) 
            {
                Console.WriteLine($"An active ECU connection is required");
                return;
            }

            // 22 F1 21
            // 2049022702, 2049022802
            // 00 00 04 81    62 f1 21     32 30 34 39 30 32 32 37 30 32    32 30 34 39 30 32 32 38 30 32
            EcuSoftwareBlocks.Clear();
            var blocks = DiogenesSharedContext.Singleton.Channel.GetSoftwareBlocks();
            foreach (var block in blocks) 
            {
                EcuSoftwareBlocks.Add(block);
            }
            
        }

        private async void btnFlash_Click(object sender, EventArgs e)
        {
            string messageBody = "The flash capability is preliminary and has a very high possibility of failure, and should only be used if you are absolutely sure of what you are doing. \r\n\r\n" +
                "Please ensure that you have an alternative recovery mechanism e.g. Vediamo in case the flash process is interrupted.\r\n\r\n" +
                "Also, remember to back up your varcoding parameters as some ECUs will reset those values after flashing.\r\n\r\n\r\n" +
                "Would you like to continue with the flash process?";
            if (MessageBox.Show(messageBody, "Flash Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) 
            {
                await Task.Run(() => Flash());
            }
        }

        private void Flash() 
        {
            // ensure connection
            if (DiogenesSharedContext.Singleton.Channel is null)
            {
                Console.WriteLine($"An active ECU connection is required");
                return;
            }

            // an active variant is required. even for softbricked targets, they should still be available e.g. bootvariante
            if (DiogenesSharedContext.Singleton.PrimaryVariant is null)
            {
                Console.WriteLine($"A valid ECU variant is required");
                return;
            }

            // currently missing block query AND download for non-uds targets
            if (DiogenesSharedContext.Singleton.Channel.GetType() != typeof(CaesarConnection.Protocol.UDS)) 
            {
                Console.WriteLine($"Non-UDS targets are currently unsupported at this time");
                return;
            }

            // check if the flash collection is intended for the active ecu
            List<FlashItem> BlocksToDownload = new List<FlashItem>();
            BlocksToDownload.AddRange(FlashItems.Where(x => x.AllowedECU == DiogenesSharedContext.Singleton.PrimaryEcu.Qualifier));
            
            // order flashware by priority
            BlocksToDownload.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            if (BlocksToDownload.Count == 0)
            {
                Console.WriteLine($"No compatible flash files could be found for the current ECU ({DiogenesSharedContext.Singleton.PrimaryEcu.Qualifier})");
                return;
            }

            // count the total segments required for progress display
            int totalSegmentCount = BlocksToDownload.Sum(x => x.Segments);
            int completedSegments = 0;

            // set exclusive access
            if (!DiogenesSharedContext.Singleton.StartUserInitiatedRequest())
            {
                Console.WriteLine($"Another request is being processed at this time. Please try again after it has completed.");
                return;
            }

            // crank up the stmin -- this isn't done in the script automatically
            decimal priorStMin = DiogenesSharedContext.Singleton.Channel.ComParameters.GetParameter(CaesarConnection.ComParam.CP.StMinOverride);
            DiogenesSharedContext.Singleton.Channel.ComParameters.SetParameter(CaesarConnection.ComParam.CP.StMinOverride, 65535);
            DiogenesSharedContext.Singleton.Channel.Channel.ReloadIsoTpTimings();

            // start transfer
            foreach (var block in BlocksToDownload)
            {
                // retrieve the flashservice which contains the flash script
                var flashService = DiogenesSharedContext.Singleton.PrimaryVariant.DiagServices.FirstOrDefault(x => x.Qualifier == block.Job);
                if (flashService is null)
                {
                    throw new Exception($"Flash service could not be found for {block.Job}");
                }

                Console.WriteLine($"Running {flashService.Qualifier}");
                // run the flash script
                DiagServiceRunner dsr = new DiagServiceRunner(flashService);
                dsr.AttachFlashHost(block);

                // this updates the progress
                dsr.FlashProgressChanged = () => {
                    Invoke((Action)(() =>
                    {
                        var fh = dsr.FlashHost;
                        int totalCompletedSegments = completedSegments + fh.SegmentsDownloaded;
                        int percentageCompleted = (int)(100.0 * totalCompletedSegments / totalSegmentCount);
                        string labelValue = $"{fh.BlockName} ({fh.SegmentName}) {fh.SegmentsDownloaded}/{fh.SegmentsTotal} ({totalCompletedSegments}/{totalSegmentCount}) : {percentageCompleted}%";
                        lblProgress.Text = labelValue;
                        pbFlashProgress.Value = percentageCompleted;
                    }));
                };
                dsr.DoDiagService();
                completedSegments += dsr.FlashHost.SegmentsTotal;
            }

            // restore isotp timings
            DiogenesSharedContext.Singleton.Channel.ComParameters.SetParameter(CaesarConnection.ComParam.CP.StMinOverride, priorStMin);
            DiogenesSharedContext.Singleton.Channel.Channel.ReloadIsoTpTimings();

            Invoke((Action)(() =>
            {
                pbFlashProgress.Value = 0;
                lblProgress.Text = "Flash completed";
            }));

            // release exclusive access
            DiogenesSharedContext.Singleton.EndUserInitiatedRequest();
        }
    }
}
