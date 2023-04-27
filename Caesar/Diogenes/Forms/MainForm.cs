using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caesar;
using CaesarConnection.ComParam;
using CaesarConnection.Protocol;

namespace Diogenes.Forms
{

    public partial class MainForm : Form
    {

        /*
        // dbg: remove
        string[] cbfPool = { CBF_ic204, CBF_ki211, CBF_crd3, CBF_cr4nfz, CBF_clamp15, CBF_med40, CBF_ki169, CBF_eis204, CBF_ezs169 };

        const string CBF_ic204 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\IC_204.CBF";
        const string CBF_ki211 = @"C:\Users\jg\Downloads\Benz\DTSProjects\211\cbf\KI211.CBF";
        const string CBF_crd3 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\CRD3.CBF";
        const string CBF_cr4nfz = @"C:\Users\jg\Downloads\Vediamo 5_0_0\CBF VAN\CR4_NFZ.cbf";
        const string CBF_clamp15 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\CLAMP15.CBF";
        const string CBF_med40 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\MED40.CBF";
        const string CBF_ki169 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\KI169.CBF";
        const string CBF_eis204 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\EIS_204.CBF";
        const string CBF_ezs169 = @"C:\Users\jg\Downloads\Vediamo 5_0_0\Data05.00.00\EZS169.CBF";
        const string CFF_204_appl = @"C:\Users\jg\Downloads\Benz\CxFViewer\jg_cff\2049022702_001.cff";
        const string CFF_204_data = @"C:\Users\jg\Downloads\Benz\CxFViewer\jg_cff\2049022802_001.cff";
        */


        const string UITextBtnConnect = "Connect";
        const string UITextBtnDisconnect = "Disconnect";

        const string UITextLblCbfNotLoaded = "CBF not loaded";
        const string UITextLblCbfNoVariant = "No Variant";

        const string UITextLblOffline = "Offline";
        const string UITextLblOnline = "Online";

        public TextWriter TraceWriter = null;
        PersistentRecentFiles RecentCbfFiles = null;
        DiagServicesView DiagServicesViewControl = null;
        FlashView FlashViewControl = null;

        public MainForm()
        {
            InitializeComponent();

            dgvPreConnectComParams.DataSource = DiogenesSharedContext.Singleton.PreConnectParameters;
            dgvPreConnectComParams.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            UpdateUIForConnectionStateChange();
            UpdateUIForCbfLoadUnload();
            UpdateUIForVariantChange();

            TraceWriter = new TextboxWriter(txtTrace, 200);
            if (!Program.SeparateConsole) 
            {
                Console.SetOut(new TextboxWriter(txtConsoleLog));
            }
            
            RecentCbfFiles = new PersistentRecentFiles(tsmiOpenRecent, LoadCBF);

            var memoryEditor = new MemoryEditorView();
            tabMemoryEditor.Controls.Add(memoryEditor);
            memoryEditor.Dock = DockStyle.Fill;

            DiagServicesViewControl = new DiagServicesView();
            tabDiagServices.Controls.Add(DiagServicesViewControl);
            DiagServicesViewControl.Dock = DockStyle.Fill;

            FlashViewControl = new FlashView();
            FlashViewControl.Dock = DockStyle.Fill;
            tabPage5.Controls.Add(FlashViewControl);

            this.Text = $"Diogenes II (Build: {LinkerTime.GetLinkerTime().ToShortDateString()})";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadComDeviceList();
        }

        private void LoadCBF(string path)
        {
            if (!File.Exists(path)) 
            {
                Console.WriteLine($"CBF file could not be found at {path}");
                RecentCbfFiles.RemoveEntry(path);
                return;
            }
            UnloadCBF();
            DiogenesSharedContext.Singleton.TryLoadCBF(path);
            RefreshProtocolList();
            RecentCbfFiles?.AddRecentFile(path);
            UpdateUIForCbfLoadUnload();
            DiagServicesViewControl.NotifyCbfOrVariantChange();
        }

        private void LoadComDeviceList()
        {
            cbComDevices.Items.Clear();
            var devices = CaesarConnection.Connection.GetDevices();
            int lastDevice = Properties.Settings.Default.LastSelectedDevice;
            foreach (var device in devices)
            {
                cbComDevices.Items.Add(device);
                if (device.GetPersistentHash() == lastDevice)
                {
                    cbComDevices.SelectedItem = device;
                }
            }
            if ((cbComDevices.SelectedItem is null) && (cbComDevices.Items.Count > 0))
            {
                cbComDevices.SelectedItem = cbComDevices.Items[0];
            }
        }

        private void RefreshProtocolList()
        {
            cbProtocol.Items.Clear();
            if (DiogenesSharedContext.Singleton.PrimaryContainer is null) 
            {
                return;
            }
            foreach (var iface in DiogenesSharedContext.Singleton.PrimaryEcu.ECUInterfaceSubtypes)
            {
                var physicalProtocol = iface.PhysicalProtocol;
                var diagProtocol = DiogenesSharedContext.Singleton.PrimaryEcu.ECUInterfaces[iface.ParentInterfaceIndex].Qualifier;
                cbProtocol.Items.Add(iface.Qualifier);
            }
            if (cbProtocol.Items.Count > 0)
            {
                cbProtocol.SelectedItem = cbProtocol.Items[0];
                cbProtocol_SelectionChangeCommitted(null, null);
            }
        }

        /*
        private async void InitTest()
        {
            // dbg: remove
            var devices = CaesarConnection.Connection.GetDevices();
            foreach (var device in devices)
            {
                Console.WriteLine($"{device.GetType().Name} : {device.Name} {device.Parameter}");
            }
            var selectedDevice = devices.FirstOrDefault(x => x.Name == "OpenPort 2.0 J2534 ISO/CAN/VPW/PWM"); // OkayJ2534 or J2534-Shim or `OpenPort 2.0 J2534 ISO/CAN/VPW/PWM`


            DiogenesSharedContext.Singleton.TryLoadCBF(CBF_ic204);

            var primaryEcu = DiogenesSharedContext.Singleton.PrimaryEcu();

            var ifacetype = primaryEcu.ECUInterfaceSubtypes[0];
            var comParams = ifacetype.CommunicationParameters.ToDictionary(t => t.ParamName, t => t.ComParamValue);
            comParams.ToList().ForEach(x => Console.WriteLine($"{x.Key} -> {x.Value}"));

            var physicalProtocol = ifacetype.PhysicalProtocol;
            var diagProtocol = primaryEcu.ECUInterfaces[ifacetype.ParentInterfaceIndex].Qualifier;

            var connection = CaesarConnection.Connection.Create(selectedDevice);

            Console.WriteLine("cc open");

            var channel = connection.OpenChannel($"{physicalProtocol}", diagProtocol, comParams, TraceWriter);
            Console.WriteLine("ch open");

            int variant = channel.GetEcuVariant();
            Console.WriteLine($"variant: {variant:X8}");

            byte[] sk = channel.Send(new byte[] { 0x27, 0x09 });
            Console.WriteLine($"sk: {BitConverter.ToString(sk)}");

            List<byte> resp1 = new List<byte>(new byte[] { 0x27, 0x0A });
            resp1.AddRange(BitUtility.BytesFromHex(Console.ReadLine()));
            byte[] sk2 = channel.Send(resp1.ToArray());
            Console.WriteLine($"sk2: {BitConverter.ToString(sk2)}");

            byte[] sk3 = channel.Send(new byte[] { 0x27, 0x0D });
            Console.WriteLine($"sk3: {BitConverter.ToString(sk3)}");

            List<byte> resp2 = new List<byte>(new byte[] { 0x27, 0x0E });
            resp2.AddRange(BitUtility.BytesFromHex(Console.ReadLine()));
            byte[] sk4 = channel.Send(resp1.ToArray());
            Console.WriteLine($"sk4: {BitConverter.ToString(sk4)}");

            byte[] wait = channel.Send(new byte[] { 0x23, 0x14, 0x80, 0x00, 0x00, 0x00, 0x20 });
            Console.WriteLine($"wait resp: {BitConverter.ToString(wait)}");

            await Task.Delay(7000);

            channel.Dispose();
            Console.WriteLine("ch closed");
            connection.Dispose();
            Console.WriteLine($"cc close");
        }
        */

        private void cbComDevices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // remember the last used vci so that it can be automatically picked on next restart
            if ((cbComDevices.SelectedItem != null) && (cbComDevices.SelectedItem.GetType().BaseType == typeof(CaesarConnection.VCI.BaseDevice)))
            {
                int deviceHash = (cbComDevices.SelectedItem as CaesarConnection.VCI.BaseDevice).GetPersistentHash();
                Properties.Settings.Default.LastSelectedDevice = deviceHash;
                Properties.Settings.Default.Save();
            }
        }

        private void cbProtocol_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvPreConnectComParams.Suspend();

            DiogenesSharedContext.Singleton.PreConnectParameters.Clear();
            foreach (var iface in DiogenesSharedContext.Singleton.PrimaryEcu.ECUInterfaceSubtypes)
            {
                if (iface.Qualifier != cbProtocol.SelectedItem.ToString())
                {
                    continue;
                }
                var comParams = iface.CommunicationParameters.ToDictionary(t => t.ParamName, t => t.ComParamValue);
                comParams.ToList().ForEach(x => DiogenesSharedContext.Singleton.PreConnectParameters.Add(new DiogenesSharedContext.RawComParam { Name = x.Key, Value = x.Value }));
            }

            dgvPreConnectComParams.Resume();
        }

        private void btnConnectDisconnect_Click(object sender, EventArgs e)
        {
            cbComDevices.Enabled = false;
            cbProtocol.Enabled = false;
            dgvPreConnectComParams.Enabled = false;
            btnConnectDisconnect.Enabled = false;
            cbAutoVariantID.Enabled = false;

            if (DiogenesSharedContext.Singleton.Channel is null)
            {
                Connect();
            }
            else
            {
                CloseConnection();
            }
            UpdateUIForConnectionStateChange();
        }

        // refreshes the ui, toggling enable state for left panel controls based on connection state
        private void UpdateUIForConnectionStateChange() 
        {
            bool connected = DiogenesSharedContext.Singleton.Channel != null;
            btnConnectDisconnect.Text = connected ? UITextBtnDisconnect : UITextBtnConnect;
            
            cbComDevices.Enabled = !connected;
            cbProtocol.Enabled = !connected;
            dgvPreConnectComParams.Enabled = !connected;
            cbAutoVariantID.Enabled = !connected;
            btnConnectDisconnect.Enabled = true;

            if (connected)
            {
                tsslConnectionState.Text = UITextLblOnline;
                tsslConnectionState.BackColor = SystemColors.Highlight;
            }
            else 
            {
                tsslConnectionState.Text = UITextLblOffline;
                tsslConnectionState.BackColor = SystemColors.ControlDark;
            }
        }

        private void UpdateUIForCbfLoadUnload()
        {
            tsslLoadedCbf.Text = DiogenesSharedContext.Singleton.PrimaryEcu?.Qualifier ?? UITextLblCbfNotLoaded;
            if (DiogenesSharedContext.Singleton.PrimaryEcu is null)
            {
                tsslLoadedCbf.BackColor = SystemColors.ControlDark;
            }
            else
            {
                tsslLoadedCbf.BackColor = SystemColors.Highlight;
            }
            DiagServicesViewControl?.NotifyCbfOrVariantChange();
        }
        private void UpdateUIForVariantChange()
        {
            tsslVariant.Text = DiogenesSharedContext.Singleton.PrimaryVariant?.Qualifier ?? UITextLblCbfNoVariant;
            if (DiogenesSharedContext.Singleton.PrimaryVariant is null)
            {
                tsslVariant.BackColor = SystemColors.ControlDark;
            }
            else
            {
                tsslVariant.BackColor = SystemColors.Highlight;
            }
            DiagServicesViewControl?.NotifyCbfOrVariantChange();
        }

        // attempts a connection to an ecu
        private void Connect()
        {
            if (DiogenesSharedContext.Singleton.PrimaryContainer is null)
            {
                Console.WriteLine($"Please load a CBF file first");
                return;
            }

            var ecu = DiogenesSharedContext.Singleton.PrimaryEcu;
            var iface = ecu.ECUInterfaceSubtypes.FirstOrDefault(x => x.Qualifier == cbProtocol.SelectedItem.ToString());
            if (iface is null)
            {
                Console.WriteLine($"Please select a connection interface under Connection > Interface");
                return;
            }

            var physicalProtocol = iface.PhysicalProtocol;
            var diagProtocol = ecu.ECUInterfaces[iface.ParentInterfaceIndex].Qualifier;

            if (cbComDevices.SelectedItem is null)
            {
                Console.WriteLine($"Please select a VCI under Connection > Device");
                return;
            }

            try
            {
                // create a connection and channel
                DiogenesSharedContext.Singleton.Connection = CaesarConnection.Connection.Create(cbComDevices.SelectedItem as CaesarConnection.VCI.BaseDevice);

                Dictionary<string, int> comParams = new Dictionary<string, int>();
                foreach (var row in DiogenesSharedContext.Singleton.PreConnectParameters)
                {
                    comParams.Add(row.Name, row.Value);
                }
                DiogenesSharedContext.Singleton.Channel = DiogenesSharedContext.Singleton.Connection.OpenChannel($"{physicalProtocol}", diagProtocol, comParams, TraceWriter);
                Console.WriteLine($"Channel opened");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open connection: {ex.Message}");
                CloseConnection();
                return;
            }

            if (cbAutoVariantID.Checked)
            {
                IdentifyEcuVariant();
            }
        }

        private void IdentifyEcuVariant() 
        {
            try
            {
                // variant detection will throw an exception if it fails
                int variantId = DiogenesSharedContext.Singleton.Channel.GetEcuVariant();
                Console.WriteLine($"ECU responded with variant ID: {variantId:X4}");
                
                ECUVariant result = null;
                foreach (var variant in DiogenesSharedContext.Singleton.PrimaryEcu.ECUVariants) 
                {
                    if (variant.VariantPatterns.Any(x => (x.VariantID & 0xFFFF) == variantId))
                    {
                        Console.WriteLine($"Found matching variant: {variant.Qualifier}");
                        result = variant;
                        break;
                    }
                }
                if (result is null) 
                {
                    Console.WriteLine($"No variant was found within this CBF that matches {variantId:X4}");
                }
                SetEcuVariant(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to identify ECU variant: {ex.Message}");
                SetEcuVariant(null);
            }
        }

        private void SetEcuVariant(ECUVariant variant) 
        {
            // also accepts null to clear active variant
            DiogenesSharedContext.Singleton.PrimaryVariant = variant;
            // optionally refresh ui later
            UpdateUIForVariantChange();
        }

        private void UnloadCBF() 
        {
            CloseConnection();
            DiogenesSharedContext.Singleton.PreConnectParameters.Clear();
            DiogenesSharedContext.Singleton.PrimaryContainer = null;
            RefreshProtocolList();
            UpdateUIForCbfLoadUnload();
        }

        private void CloseConnection()
        {
            SetEcuVariant(null);
            DiogenesSharedContext.Singleton.Channel?.Dispose();
            DiogenesSharedContext.Singleton.Connection?.Dispose();
            DiogenesSharedContext.Singleton.Channel = null;
            DiogenesSharedContext.Singleton.Connection = null;
            UpdateUIForConnectionStateChange();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }

        private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox txtSender = sender as TextBox;
                e.Handled = true;
                e.SuppressKeyPress = true;
                string inText = txtSender.Text;
                txtSender.Text = "";

                // if disconnected, discard and ignore
                if (DiogenesSharedContext.Singleton.Channel is null)
                {
                    return;
                }
                // if content is valid hex, send raw j2534 request
                if (BitUtility.TryParseHex(inText, out byte[] requestData))
                {
                    Console.WriteLine($"REQ: {BitUtility.BytesToHex(requestData, true)}");
                    byte[] response = DiogenesSharedContext.Singleton.Channel.Send(requestData, true);
                    Console.WriteLine($"ECU: {BitUtility.BytesToHex(response, true)}");
                }
                else
                {
                    Console.WriteLine($"Could not understand provided hex input: '{inText}'");
                }
            }
        }

        private void tsmiOpenCbf_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CBF File";
            ofd.Filter = "CBF files (*.cbf)|*.cbf|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadCBF(ofd.FileName);
            }
        }

        private void tsmiUnloadCbf_Click(object sender, EventArgs e)
        {
            UnloadCBF();
        }

        private void tsmiExplorerHere_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", ".");
        }
    }
}
