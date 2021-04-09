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
using Diogenes.Properties;
using System.Runtime.InteropServices;
using Diogenes.SecurityAccess;

namespace Diogenes
{
    public partial class MainForm : Form
    {
        public ECUConnection Connection = null;

        public MainForm()
        {
            InitializeComponent();
        }

        TraceForm TraceFormSingleInstance;
        List<CaesarContainer> Containers = new List<CaesarContainer>();
        ImageList treeImages = null;
        TextboxWriter LogTextbox;

        private void MainForm_Load(object sender, EventArgs e)
        {
            RedirectConsole();
            LoadContainers();
            UnmanagedUtility.SendMessage(txtJ2534Input.Handle, UnmanagedUtility.EM_SETCUEBANNER, 0, "J2534 Console : Enter hex values (01 23 45 57) and press enter to send a raw J2534 command");
#if (!DEBUG)
            genericDebugToolStripMenuItem.Visible = false;
            downloadBlocksToolStripMenuItem.Visible = false;
#endif
            SetDisconnectedState(false);
        }

        private void RedirectConsole()
        {
            LogTextbox = new TextboxWriter(txtLog);
            Console.SetOut(LogTextbox);
        }

        private void LoadContainers()
        {
            Containers.Clear();
            foreach (string file in Directory.GetFiles(Application.StartupPath))
            {
                if (Path.GetExtension(file).ToLower() == ".cbf")
                {
                    CaesarContainer cbfContainer = new CaesarContainer(File.ReadAllBytes(file));
                    Containers.Add(cbfContainer);
                    //PostInitDebug(cbfContainer);
                }
            }
            LoadTree();
        }

        private void PostInitDebug(CaesarContainer cbfContainer) 
        {
        }

        private void InitializeTree()
        {
            if (treeImages is null)
            {
                treeImages = new ImageList();
                treeImages.Images.Add(Resources.blank); // 0
                treeImages.Images.Add(Resources.box);
                treeImages.Images.Add(Resources.brick);
                treeImages.Images.Add(Resources.cog);
                treeImages.Images.Add(Resources.house);
                treeImages.Images.Add(Resources.connect);
                treeImages.Images.Add(Resources.information); // 6

                treeImages.Images.Add(Resources.bullet_go); // 7
                treeImages.Images.Add(Resources.bullet_star);

                treeImages.Images.Add(Resources.bullet_black); // 9
                treeImages.Images.Add(Resources.bullet_blue);
                treeImages.Images.Add(Resources.bullet_green);
                treeImages.Images.Add(Resources.bullet_orange);
                treeImages.Images.Add(Resources.bullet_pink);
                treeImages.Images.Add(Resources.bullet_purple);
                treeImages.Images.Add(Resources.bullet_red);
                treeImages.Images.Add(Resources.bullet_white);
                treeImages.Images.Add(Resources.bullet_yellow); // 17

                treeImages.Images.Add(Resources.computer_go); // 18
                treeImages.Images.Add(Resources.lock_edit); // 19
                treeImages.Images.Add(Resources.key); // 20
                treeImages.Images.Add(Resources.application_xp_terminal); // 21
                treeImages.Images.Add(Resources.page_white_edit); // 22

                treeImages.Images.Add(Resources.asterisk_orange); // 23
                treeImages.Images.Add(Resources.folder); // 24
                treeImages.Images.Add(Resources.accept); // 25
                treeImages.Images.Add(Resources.report); // 26

                tvMain.ImageList = treeImages;

                UnmanagedUtility.SendMessage(tvMain.Handle, UnmanagedUtility.TVM_SETEXTENDEDSTYLE, (IntPtr)UnmanagedUtility.TVS_EX_DOUBLEBUFFER, (IntPtr)UnmanagedUtility.TVS_EX_DOUBLEBUFFER);
            }
        }

        private void AddDiagServicesToNode(TreeNode parentNode, ECUVariant variant)
        {

            string newTag = $"Exec{nameof(DiagService)}:{variant.Qualifier}";

            TreeNode diagUnlockingOptions = new TreeNode($"Security Access", 19, 19);
            diagUnlockingOptions.Tag = newTag;

            TreeNode diagStoredData = new TreeNode($"Diagnostics: Stored Data", 24, 24);
            diagStoredData.Tag = newTag;

            TreeNode diagData = new TreeNode($"Diagnostics: Data", 24, 24);
            diagData.Tag = newTag;

            TreeNode diagFunction = new TreeNode($"Diagnostics: Function", 24, 24);
            diagFunction.Tag = newTag;

            TreeNode diagRoutine = new TreeNode($"Diagnostics: Routine", 24, 24);
            diagRoutine.Tag = newTag;

            TreeNode diagIO = new TreeNode($"Diagnostics: IO", 24, 24);
            diagIO.Tag = newTag;

            TreeNode diagDownload = new TreeNode($"Diagnostics: Download", 24, 24);
            diagDownload.Tag = newTag;


            for (int i = 0; i < variant.DiagServices.Length; i++)
            {
                DiagService currentDiagService = variant.DiagServices[i];

                TreeNode diagNode = new TreeNode(currentDiagService.Qualifier, 9, 9);
                diagNode.Tag = i.ToString();

                if ((currentDiagService.RequestBytes.Length > 1) && (currentDiagService.RequestBytes[0] == 0x27))
                {
                    diagUnlockingOptions.Nodes.Add(diagNode);
                    continue;
                }

                if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.StoredData)
                {
                    diagStoredData.Nodes.Add(diagNode);
                }
                else if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.DiagnosticFunction)
                {
                    diagFunction.Nodes.Add(diagNode);
                }
                else if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.Data)
                {
                    diagData.Nodes.Add(diagNode);
                }
                else if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.Routine)
                {
                    diagRoutine.Nodes.Add(diagNode);
                }
                else if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.IoControl)
                {
                    diagIO.Nodes.Add(diagNode);
                }
                else if (currentDiagService.DataClass_ServiceType == (int)DiagService.ServiceType.Download)
                {
                    diagDownload.Nodes.Add(diagNode);
                }
            }

            parentNode.Nodes.Add(diagUnlockingOptions);
            parentNode.Nodes.Add(diagStoredData);
            parentNode.Nodes.Add(diagData);
            parentNode.Nodes.Add(diagFunction);
            parentNode.Nodes.Add(diagRoutine);
            parentNode.Nodes.Add(diagIO);
            parentNode.Nodes.Add(diagDownload);
        }


        private void AddEcuMetadataToNode(TreeNode parentNode, CaesarContainer container, ECU ecu)
        {
            TreeNode rootMetadata = new TreeNode("Metadata", 26, 26);
            rootMetadata.Tag = $"RootMetadata";

            TreeNode metadataRowNode;

            metadataRowNode = new TreeNode($"Container File Size: {container.GetFileSize()}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);

            metadataRowNode = new TreeNode($"Container Checksum: {container.FileChecksum:X8}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);

            metadataRowNode = new TreeNode($"CBF Version: {container.CaesarCFFHeader.CbfVersionString}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);

            metadataRowNode = new TreeNode($"GPD Version: {container.CaesarCFFHeader.GpdVersionString}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);

            metadataRowNode = new TreeNode($"ECU Version: {ecu.EcuXmlVersion}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);

            metadataRowNode = new TreeNode($"ECU Ignition Required: {ecu.IgnitionRequired}", 9, 9);
            metadataRowNode.Tag = "RootMetadataEntry";
            rootMetadata.Nodes.Add(metadataRowNode);


            parentNode.Nodes.Add(rootMetadata);
        }

        private void LoadTree()
        {
            InitializeTree();
            tvMain.Nodes.Clear();

            foreach (CaesarContainer container in Containers)
            {
                foreach (ECU ecu in container.CaesarECUs)
                {
                    TreeNode ecuNode = new TreeNode(ecu.Qualifier, 1, 1);
                    ecuNode.Tag = nameof(ECU);

                    TreeNode execDiagAtRoot = new TreeNode("Execute Diagnostic Service (Root)", 21, 21);
                    execDiagAtRoot.Tag = $"{nameof(DiagService)}:{nameof(ECU)}:{ecu.Qualifier}";
                    ecuNode.Nodes.Add(execDiagAtRoot);

                    AddEcuMetadataToNode(ecuNode, container, ecu);

                    foreach (ECUInterfaceSubtype subtype in ecu.ECUInterfaceSubtypes)
                    {
                        if (Connection?.VariantIsAvailable ?? false)
                        {
                            // interfaces don't matter anymore when we are connected
                            break;
                        }
                        TreeNode interfaceNode = new TreeNode(subtype.Qualifier, 5, 5);
                        interfaceNode.Tag = "";

                        TreeNode initiateContactNode = new TreeNode("Initiate Contact", 18, 18);
                        initiateContactNode.Tag = $"{nameof(ECUInterfaceSubtype)}:{subtype.Qualifier}";
                        interfaceNode.Nodes.Add(initiateContactNode);

                        TreeNode comparamParentNode = new TreeNode("Communications Parameters", 6, 6);
                        comparamParentNode.Tag = $"ComParamParent";

                        foreach (ComParameter parameter in subtype.CommunicationParameters)
                        {
                            TreeNode comNode = new TreeNode($"{parameter.ParamName} : {parameter.ComParamValue} (0x{parameter.ComParamValue:X})", 9, 9);
                            // Console.WriteLine(comNode.Text);
                            comNode.Tag = nameof(ComParameter);
                            comparamParentNode.Nodes.Add(comNode);
                        }
                        interfaceNode.Nodes.Add(comparamParentNode);

                        ecuNode.Nodes.Add(interfaceNode);
                        interfaceNode.Expand();
                    }

                    // offer the ability to switch sessions at all times, in case user runs functions like FN_Reset
                    TreeNode sessionContainer = new TreeNode("Session", 23, 23);
                    sessionContainer.Tag = $"Session";
                    foreach (DiagService ds in ecu.GlobalDiagServices)
                    {
                        if (ds.DataClass_ServiceType == (ushort)DiagService.ServiceType.Session)
                        {
                            TreeNode dsNode = new TreeNode(ds.Qualifier, 12, 12);
                            dsNode.Tag = ds.Qualifier;
                            sessionContainer.Nodes.Add(dsNode);
                        }
                    }
                    ecuNode.Nodes.Add(sessionContainer);

                    foreach (ECUVariant variant in ecu.ECUVariants)
                    {
                        TreeNode ecuVariantNode = new TreeNode(variant.Qualifier, 2, 2);
                        ecuVariantNode.Tag = nameof(ECUVariant);

                        // check if variant should be filtered
                        if (Connection?.VariantIsAvailable ?? false) 
                        {
                            bool foundCorrectVariant = false;
                            foreach (ECUVariantPattern pattern in variant.VariantPatterns) 
                            {
                                if (pattern.VariantID == Connection.ECUVariantID) 
                                {
                                    foundCorrectVariant = true;
                                    break;
                                }
                            }
                            if (!foundCorrectVariant) 
                            {
                                continue;
                            }
                            ecuVariantNode.ImageIndex = 25;
                            ecuVariantNode.SelectedImageIndex = 25;
                            ecuVariantNode.Expand();
                        }


                        // exec diag button
                        TreeNode execDiagAtVariant = new TreeNode("Execute Diagnostic Service", 21, 21);
                        execDiagAtVariant.Tag = $"{nameof(DiagService)}:{nameof(ECUVariant)}:{variant.Qualifier}";
                        ecuVariantNode.Nodes.Add(execDiagAtVariant);

                        // metadata
                        TreeNode metadataNode = new TreeNode($"Metadata", 6, 6);
                        metadataNode.Tag = $"{nameof(ECUVariant)}Metadata";
                        foreach (ECUVariantPattern pattern in variant.VariantPatterns)
                        {
                            string vendorText = pattern.PatternType == 3 ? $", Vendor: {pattern.VendorName}" : "";
                            TreeNode patternNode = new TreeNode($"Variant ID: {pattern.VariantID} ({pattern.VariantID:X4}){vendorText}", 9, 9);
                            patternNode.Tag = nameof(ECUVariantPattern);
                            metadataNode.Nodes.Add(patternNode);
                        }

                        AddDiagServicesToNode(ecuVariantNode, variant);

                        ecuVariantNode.Nodes.Add(metadataNode);

                        // vc domains
                        foreach (VCDomain domain in variant.VCDomains)
                        {
                            TreeNode vcDomainNode = new TreeNode(domain.Qualifier, 3, 3);
                            vcDomainNode.Tag = nameof(VCDomain);
                            ecuVariantNode.Nodes.Add(vcDomainNode);
                        }

                        TreeNode backupNode = new TreeNode("Backup Variant Strings", 3, 3);
                        backupNode.Tag = "VCBackup";
                        ecuVariantNode.Nodes.Add(backupNode);

                        ecuNode.Nodes.Add(ecuVariantNode);
                    }
                    tvMain.Nodes.Add(ecuNode);
                    ecuNode.Expand();
                }
            }
        }

        private void FixCALs(CaesarContainer container)
        {
            int newLevel = 1;
            byte[] newFile = new byte[container.FileBytes.Length];
            Buffer.BlockCopy(container.FileBytes, 0, newFile, 0, container.FileBytes.Length);

            Console.WriteLine($"Creating a new CBF with access level requirements set at {newLevel}");
            List<DiagService> dsPendingFix = new List<DiagService>();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(container.FileBytes)))
            {
                foreach (ECU ecu in container.CaesarECUs)
                {
                    foreach (DiagService ds in ecu.GlobalDiagServices)
                    {
                        if (ds.ClientAccessLevel > newLevel)
                        {
                            dsPendingFix.Add(ds);
                            Console.WriteLine($"-> {ds.Qualifier} (Level {ds.ClientAccessLevel})");
                            long fileOffset = ds.GetCALInt16Offset(reader);
                            if (fileOffset != -1)
                            {
                                newFile[fileOffset] = (byte)newLevel;
                                newFile[fileOffset + 1] = (byte)(newLevel >> 8);
                            }
                        }
                    }
                }
                uint checksum = CaesarReader.ComputeFileChecksum(newFile);
                byte[] checksumBytes = BitConverter.GetBytes(checksum);
                Array.ConstrainedCopy(checksumBytes, 0, newFile, newFile.Length - 4, checksumBytes.Length);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your new CBF file";
            sfd.Filter = "CBF files (*.cbf)|*.cbf|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, newFile);
            }
        }

        private void TreeViewDoubleClickCheckIfSession(TreeNode node)
        {
            if (node.Parent != null && node.Parent.Tag.ToString() == "Session")
            {
                string ecuName = node.Parent.Parent.Text;
                string serviceName = node.Tag.ToString();

                foreach (CaesarContainer container in Containers)
                {
                    ECU ecu = container.CaesarECUs.Find(x => x.Qualifier == ecuName);
                    if (ecu != null)
                    {
                        DiagService ds = ecu.GlobalDiagServices.Find(x => x.Qualifier == serviceName);
                        if (ds != null)
                        {
                            PresentRunDiagDialog(ds);
                            break;
                        }
                    }
                }
            }
        }

        private void TreeViewDoubleClickCheckIfVariantDiag(TreeNode node)
        {
            string validNodePrefix = $"Exec{nameof(DiagService)}:";

            if (node.Parent is null)
            {
                return;
            }
            if (node.Parent.Tag.ToString().StartsWith(validNodePrefix))
            {
                string variantName = node.Parent.Tag.ToString().Substring(validNodePrefix.Length);

                ECUVariant foundVariant = null;
                foreach (CaesarContainer container in Containers)
                {
                    foreach (ECU ecu in container.CaesarECUs)
                    {
                        foreach (ECUVariant variant in ecu.ECUVariants)
                        {
                            if (variant.Qualifier == variantName)
                            {
                                foundVariant = variant;
                            }
                        }
                    }
                }
                // variant found, exec the diag service
                if (foundVariant != null)
                {
                    DiagService ds = foundVariant.DiagServices[int.Parse(node.Tag.ToString())];

                    bool connectionSupportsUnlocking = Connection?.ConnectionProtocol?.SupportsUnlocking() ?? false;

                    // can we help to skip the modal if the ds doesn't require additional user input? common for data, stored data
                    if ((ds.DataClass_ServiceType == (int)DiagService.ServiceType.StoredData) || (ds.DataClass_ServiceType == (int)DiagService.ServiceType.Data))
                    {
                        Connection?.ExecUserDiagJob(ds.RequestBytes, ds);
                    }
                    else if (connectionSupportsUnlocking && (ds.RequestBytes.Length == 2) && (ds.RequestBytes[0] == 0x27))
                    {
                        // request seed, no need to prompt
                        Connection?.ExecUserDiagJob(ds.RequestBytes, ds);
                    }
                    else
                    {
                        PresentRunDiagDialog(ds);
                    }
                }
            }
        }

        private void PresentRunDiagDialog(DiagService ds)
        {
            RunDiagForm runDiagForm = new RunDiagForm(ds);
            if (runDiagForm.ShowDialog() == DialogResult.OK)
            {
                Connection.ExecUserDiagJob(runDiagForm.Result, ds);
            }
        }


        private void treeViewSelectVariantCoding(TreeNode node) 
        {
            string domainName = node.Text;
            string variantName = node.Parent.Text;
            string ecuName = node.Parent.Parent.Text;

            Console.WriteLine($"Starting VC Dialog for {ecuName} ({variantName}) with domain as {domainName}");
            CaesarContainer container = Containers.Find(x => x.GetECUVariantByName(variantName) != null);

            // prompt the user for vc changes via VCForm
            VCForm vcForm = new VCForm(container, ecuName, variantName, domainName, Connection);
            if (vcForm.ShowDialog() == DialogResult.OK)
            {
                VariantCoding.DoVariantCoding(Connection, vcForm, allowWriteVariantCodingToolStripMenuItem.Checked);
            }
        }

        private void tvMain_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = tvMain.SelectedNode;
            if (node is null)
            {
                return;
            }

            if (node.Tag.ToString() == nameof(VCDomain))
            {
                // variant coding
                treeViewSelectVariantCoding(node);
            }
            else if (node.Tag.ToString() == "VCBackup")
            {
                // variant coding backup
                VCReport.treeViewSelectVariantCodingBackup(node, Connection, Containers);
            }
            else if (node.Tag.ToString().StartsWith(nameof(ECUInterfaceSubtype)))
            {
                // initiate contact
                string connectionProfileName = node.Tag.ToString().Substring(nameof(ECUInterfaceSubtype).Length + 1);
                string ecuName = node.Parent.Parent.Text;

                foreach (CaesarContainer container in Containers)
                {
                    ECU ecu = container.CaesarECUs.Find(x => x.Qualifier == ecuName);
                    if (ecu != null)
                    {
                        ECUInterfaceSubtype subtype = ecu.ECUInterfaceSubtypes.Find(x => x.Qualifier == connectionProfileName);
                        if (subtype != null)
                        {
                            Console.WriteLine($"Attempting to open a connection to ({ecuName}) with profile '{connectionProfileName}'");
                            ECUConnection.ConnectResponse response = Connection.Connect(subtype, ecu);
                            if (response == ECUConnection.ConnectResponse.OK)
                            {
                                ProtocolPostConnect();
                            }
                            else if (response == ECUConnection.ConnectResponse.NoValidInterface)
                            {
                                BlinkConnectionMenu();
                                connectionToolStripMenuItem.ShowDropDown();
                                j2534InterfacesToolStripMenuItem.ShowDropDown();
                            }
                            else 
                            {
                                // uhoh
                                Console.WriteLine($"ECU connection was unsuccessful : {response}");
                            }
                            break;
                        }
                    }
                }
            }
            else if (node.Tag.ToString().StartsWith(nameof(DiagService)))
            {
                // execute diag service (modal)
                string diagOrigin = node.Tag.ToString().Substring(nameof(DiagService).Length + 1);
                string variantName = "";
                string ecuName = "";

                if (diagOrigin.StartsWith($"{nameof(ECUVariant)}:"))
                {
                    variantName = node.Parent.Text;
                    ecuName = node.Parent.Parent.Text;
                }
                else 
                {
                    ecuName = node.Parent.Text;
                }

                foreach (CaesarContainer container in Containers)
                {
                    ECU ecu = container.CaesarECUs.Find(x => x.Qualifier == ecuName);
                    if (ecu != null)
                    {
                        PickDiagForm picker;
                        ECUVariant variant = ecu.ECUVariants.Find(x => x.Qualifier == variantName);
                        if (variant != null)
                        {
                            //Console.WriteLine($"Starting Diagnostic Service picker modal for variant {variantName}");
                            picker = new PickDiagForm(variant.DiagServices);
                        }
                        else
                        {
                            //Console.WriteLine($"Starting Diagnostic Service picker modal for root {ecuName}");
                            picker = new PickDiagForm(ecu.GlobalDiagServices.ToArray());
                        }
                        if (picker.ShowDialog() == DialogResult.OK) 
                        {
                            PresentRunDiagDialog(picker.SelectedDiagService);
                        }
                        break;
                    }
                }
            }
            TreeViewDoubleClickCheckIfSession(node);
            TreeViewDoubleClickCheckIfVariantDiag(node);
        }

        private void ProtocolPostConnect()
        {
            if (Connection.ConnectionProtocol != null) 
            {
                Connection.ConnectionProtocol.ConnectionEstablishedHandler(Connection);
                LoadTree();
            }
        }

        private void ShowAbout() 
        {
            AboutForm about = new AboutForm($"Diogenes {GetVersion()} (Caesar {CaesarContainer.GetCaesarVersionString()})");
            about.ShowDialog();
        }
        public static string GetVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void unloadExistingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Containers.Clear();
            LoadTree();
        }

        private void loadCBFFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CBF File";
            ofd.Filter = "CBF files (*.cbf)|*.cbf|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK) 
            {
                TryLoadFile(ofd.FileName);
            }
        }

        private void TryLoadFile(string fileName)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);
            if (CaesarContainer.VerifyChecksum(fileBytes, out uint checksum))
            {
                Containers.Add(new CaesarContainer(fileBytes));
                LoadTree();
            }
            else 
            {
                Console.WriteLine($"File {Path.GetFileName(fileName)} was not loaded as the checksum is invalid");
            }
        }

        private void setSecurityLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Security DLL File";
            ofd.Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                DllContext ctx = new DllContext(ofd.FileName);
                if (ctx.KeyGenerationCapability)
                {
                    SecurityLevelForm slForm = new SecurityLevelForm(ctx);
                    if (slForm.ShowDialog() == DialogResult.OK)
                    {
                        Console.WriteLine($"Authentication: Selected level {slForm.RequestedSecurityLevel} : {BitUtility.BytesToHex(slForm.KeyResponse)}");
                    }
                }
                else 
                {
                    MessageBox.Show("The selected DLL is not capable of configuring ECU security levels. Please try another file.", "Security Level Configuration");
                }
            }
        }

        private void debugJ2534ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void j2534InterfacesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            j2534InterfacesToolStripMenuItem.DropDownItems.Clear();
            ToolStripItem defaultItem = j2534InterfacesToolStripMenuItem.DropDownItems.Add("(No devices found)");
            defaultItem.Enabled = false;

            foreach (Tuple<string,string> device in ECUConnection.GetAvailableJ2534NamesAndDrivers()) 
            {
                defaultItem.Visible = false;
                ToolStripItem newItem = j2534InterfacesToolStripMenuItem.DropDownItems.Add(device.Item1);
                newItem.Tag = device.Item2;
                newItem.Click += J2534InterfaceItem_Click;
            }
        }

        private void J2534InterfaceItem_Click(object sender, EventArgs e)
        {
            ToolStripItem caller = (ToolStripItem)sender;
            if (Connection != null) 
            {
                Connection.TryCleanup();
            }
            Connection = new ECUConnection(caller.Tag.ToString(), caller.Text);
            Connection.ConnectionStateChangeEvent += ConnectionStateChangedHandler;
            Connection.OpenDevice();
            // loadtree should not be necessary if the prior state was disconnected
            // LoadTree();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDisconnectedState();
        }

        private void SetDisconnectedState(bool refresh = true) 
        {
            // disconnected really means "running in simulation mode"
            if (Connection != null)
            {
                Connection.TryCleanup();
            }
            Connection = new ECUConnection();
            Connection.ConnectionStateChangeEvent += ConnectionStateChangedHandler;
            if (refresh) 
            {
                LoadTree();
            }
        }

        private void ConnectionStateChangedHandler(string newStateDescription)
        {
            lblConnectionType.Text = newStateDescription;
            txtJ2534Input.Enabled = Connection.State > ECUConnection.ConnectionState.DeviceSelectedPendingChannelConnection;
        }

        private void txtJ2534Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                e.Handled = true;
                string inText = txtJ2534Input.Text;
                txtJ2534Input.Text = "";
                if (BitUtility.CheckHexValid(inText))
                {
                    byte[] requestData = BitUtility.BytesFromHex(inText.Replace(" ", "").ToUpper());
                    byte[] response = Connection.SendMessage(requestData);
                    Console.WriteLine($"ECU:  {BitUtility.BytesToHex(response, true)}");
                }
                else 
                {
                    Console.WriteLine($"Could not understand provided hex input: '{inText}'");
                }
            }
        }

        private void cFFExportFlashSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CFF File";
            ofd.Filter = "CFF files (*.cff)|*.cff|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CaesarFlashContainer.ExportCFFMemorySegments(ofd.FileName);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"CFF Export failed: {ex.Message}");
                }
            }
        }

        private void fixClientAccessPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a CBF File to apply new permissions on";
            ofd.Filter = "CBF files (*.cbf)|*.cbf|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FixCALs(new CaesarContainer(File.ReadAllBytes(ofd.FileName)));
            }
        }

        private void cFFFlashSplicerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlashSplicer splicer = new FlashSplicer();
            splicer.Show();
        }

        private void allowWriteVariantCodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowWriteVariantCodingToolStripMenuItem.Checked = !allowWriteVariantCodingToolStripMenuItem.Checked;
            Preferences.SetValue(Preferences.PreferenceKey.AllowVC, allowWriteVariantCodingToolStripMenuItem.Checked ? "true" : "false");
        }

        private void showTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TraceFormSingleInstance is null || TraceFormSingleInstance.IsDisposed)
            {
                TraceFormSingleInstance = new TraceForm(this);
            }
            TraceFormSingleInstance.Show();
        }

        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogTextbox?.Clear();
        }

        // originally this was intended to blink the menuitem, but this requires overriding the draw call
        private void tmrBlinkConnectionMenu_Tick(object sender, EventArgs e)
        {
            if (connectionToolStripMenuItem.ForeColor != SystemColors.ControlText)
            {
                connectionToolStripMenuItem.ForeColor = SystemColors.ControlText;
                ConnectionMenuBlinksRemaining--;
                if (ConnectionMenuBlinksRemaining <= 0)
                {
                    ConnectionMenuBlinksRemaining = 0;
                    tmrBlinkConnectionMenu.Enabled = false;
                }
            }
            else 
            {
                if (ConnectionMenuBlinksRemaining > 0) 
                {
                    connectionToolStripMenuItem.ForeColor = SystemColors.Control;
                }
            }
        }

        private int ConnectionMenuBlinksRemaining = 0;
        private void BlinkConnectionMenu() 
        {
            ConnectionMenuBlinksRemaining = 8;
            tmrBlinkConnectionMenu.Enabled = true;
        }

        private void connectionToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            disconnectToolStripMenuItem.Enabled = Connection?.ConnectionDevice != null;
            j2534InterfacesToolStripMenuItem.Enabled = Connection?.ConnectionDevice == null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetDisconnectedState();
        }

        private void copyConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtLog.Text);
        }

        private void uDSHexEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connection.ConnectionProtocol is null)
            {
                MessageBox.Show("Please initiate contact with a target first.");
                return;
            }
            string protocolName = Connection.ConnectionProtocol.GetProtocolName();
            if (protocolName != "UDS")
            {
                MessageBox.Show($"Only UDS targets are officially supported (current protocol: {protocolName}). \r\n\r\n" +
                    $"The editor will still open, however please ensure that the ECU accepts UDS-like read and write commands");
            }
            UDSHexEditor editor = new UDSHexEditor(Connection); 
            editor.ShowDialog();
        }

        private void preferencesToolStripMenuItem1_DropDownOpening(object sender, EventArgs e)
        {
            RefreshPreferencesDropdown();
        }

        private void RefreshPreferencesDropdown()
        {
            // VC safety switch
            allowWriteVariantCodingToolStripMenuItem.Checked = Preferences.GetValue(Preferences.PreferenceKey.AllowVC) == "true";

            // scn mode
            bool scnZero = Preferences.GetValue(Preferences.PreferenceKey.EnableSCNZero) == "true";
            writeZerosVediamoToolStripMenuItem.Checked = scnZero;
            useLastSCNToolStripMenuItem.Checked = !writeZerosVediamoToolStripMenuItem.Checked;

            // fingerprint mode
            bool fingerprintClone = Preferences.GetValue(Preferences.PreferenceKey.EnableFingerprintClone) == "true";
            useLastFingerprintToolStripMenuItem.Checked = fingerprintClone;
            customValueToolStripMenuItem.Checked = !fingerprintClone;

            // fingerprint custom value
            uint customFingerprint = uint.Parse(Preferences.GetValue(Preferences.PreferenceKey.FingerprintValue));
            customValueToolStripMenuItem.Text = $"Custom Value: {customFingerprint}";
        }

        private void useLastSCNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences.SetValue(Preferences.PreferenceKey.EnableSCNZero, "false");
            RefreshPreferencesDropdown();
        }

        private void writeZerosVediamoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences.SetValue(Preferences.PreferenceKey.EnableSCNZero, "true");
            RefreshPreferencesDropdown();
        }

        private void useLastFingerprintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences.SetValue(Preferences.PreferenceKey.EnableFingerprintClone, "true");
            RefreshPreferencesDropdown();
        }

        private void customValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences.SetValue(Preferences.PreferenceKey.EnableFingerprintClone, "false");
            // prompt for new fingerprint value
            RefreshPreferencesDropdown();
        }

        private void tvMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void tvMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                TryLoadFile(file);
            }
        }

        // maybe this should belong in ECUConnection
        private ECUVariant GetCurrentVariantInstance()
        {
            if (!(Connection?.VariantIsAvailable ?? false))
            {
                return null;
            }

            foreach (CaesarContainer container in Containers)
            {
                foreach (ECU ecu in container.CaesarECUs)
                {
                    foreach (ECUVariant variant in ecu.ECUVariants)
                    {
                        foreach (ECUVariantPattern pattern in variant.VariantPatterns)
                        {
                            if (pattern.VariantID == Connection.ECUVariantID)
                            {
                                return variant;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void diagnosticTroubleCodesDTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connection.ConnectionProtocol is null)
            {
                MessageBox.Show("Please initiate contact with a target first.");
                return;
            }
            if (Connection.ConnectionProtocol.GetProtocolName() != "UDS")
            {
                MessageBox.Show("Sorry, only UDS is supported at this time.");
                // return was removed from here, to allow for kw2c3pe debugging
            }
            if (!(Connection?.VariantIsAvailable ?? false))
            {
                MessageBox.Show("DTCs require the variant to be identified first");
                return;
            }
            ECUVariant currentVariant = GetCurrentVariantInstance();

            DTCForm dtcForm = new DTCForm(Connection, currentVariant);
            dtcForm.ShowDialog();
        }

        private void viewECUMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ECUMetadata.ShowMetadataModal(Connection);
        }

        private void identifyECUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ECUIdentification.TryReadChassisNumber(Connection, out string vin))
            {
                Console.WriteLine($"VIN: {vin}");
            }
            else
            {
                Console.WriteLine($"Target could not be identified");
            }
        }

        private void loadCompressedJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Compressed Caesar Binary (JSON) File";
            ofd.Filter = "CCB files (*.ccb)|*.ccb|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Containers.Add(CaesarContainer.DeserializeCompressedContainer(File.ReadAllBytes(ofd.FileName)));
                LoadTree();
            }
        }

        private CaesarContainer PickContainer() 
        {
            if (Containers.Count == 0)
            {
                MessageBox.Show("No containers have been loaded yet.");
                return null;
            }
            CaesarContainer targetContainer = Containers[0];
            if (Containers.Count > 1)
            {
                // there isn't an embedded qualifier to identify containers easily; the ecu name is probably an easier name to identify with
                List<string[]> table = new List<string[]>();
                foreach (CaesarContainer container in Containers)
                {
                    if (container.CaesarECUs.Count > 0)
                    {
                        table.Add(new string[] { container.CaesarECUs[0].Qualifier });
                    }
                }
                GenericPicker picker = new GenericPicker(table.ToArray(), new string[] { "Container" }, 0);
                picker.Text = "Please select a container";
                if (picker.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }
                string selectedEcuQualifier = picker.SelectedResult[0];
                targetContainer = Containers.Find(x => ((x.CaesarECUs.Count > 0) && (x.CaesarECUs[0].Qualifier == selectedEcuQualifier)));
            }
            return targetContainer;
        }

        private void exportContainerAsCompressedJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaesarContainer targetContainer = PickContainer();
            if (targetContainer is null)
            {
                Console.WriteLine("Internal error: target container is null");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a location to save your new Compressed Caesar Binary (JSON) file";
                sfd.Filter = "CCB files (*.ccb)|*.ccb|All files (*.*)|*.*";
                sfd.FileName = targetContainer.CaesarECUs[0].Qualifier;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, CaesarContainer.SerializeCompressedContainer(targetContainer));
                }
            }
        }

        private void dSCDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a PAL File";
            ofd.Filter = "PAL files (*.pal)|*.pal|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DSCContext ctx = new DSCContext(File.ReadAllBytes(ofd.FileName));
            }
        }

        private void loadJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a Caesar JSON File";
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Containers.Add(CaesarContainer.DeserializeContainer(Encoding.UTF8.GetString(File.ReadAllBytes(ofd.FileName))));
                LoadTree();
            }
        }

        private void exportContainerAsJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaesarContainer targetContainer = PickContainer();
            if (targetContainer is null)
            {
                Console.WriteLine("Internal error: target container is null");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a location to save your new Caesar JSON file";
                sfd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                sfd.FileName = targetContainer.CaesarECUs[0].Qualifier;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, Encoding.UTF8.GetBytes(CaesarContainer.SerializeContainer(targetContainer)));
                }
            }
        }

        // this is normally not exposed to the user, the button has to be manually enabled in the Designer
        private void genericDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (CaesarContainer container in Containers)
            {
                foreach (ECU ecu in container.CaesarECUs)
                {
                    foreach (DTC dtc in ecu.GlobalDTCs) 
                    {
                        byte[] dtcQualBytes = BitUtility.BytesFromHex(dtc.Qualifier.Substring(1));
                        int dtcInt = (dtcQualBytes[0] << 16) | (dtcQualBytes[1] << 8) | dtcQualBytes[2];
                        long remainder = dtcInt & 0xFFC00000;
                        if (remainder > 0) 
                        {
                            throw new NotImplementedException("fail");
                        }
                        Console.WriteLine($"Q: {dtc.Qualifier} {dtcInt:X8} : {dtc.Description}");
                    }
                }
            }
            return;
            foreach (CaesarContainer container in Containers) 
            {
                foreach (ECU ecu in container.CaesarECUs) 
                {
                    foreach (DiagService ds in ecu.GlobalDiagServices) 
                    {
                        if (ds.Qualifier != "DT_Istgang") 
                        {
                            //continue;
                        }
                        foreach (List<DiagPreparation> dpl in ds.OutputPreparations) 
                        {
                            foreach (DiagPreparation prep in dpl) 
                            {
                                DiagPresentation pres = ecu.GlobalPresentations[prep.PresPoolIndex];
                                if (pres.EnumMaxValue == 0) 
                                {
                                    continue;
                                }
                                foreach (Scale scale in pres.Scales) 
                                {
                                    if (scale.EnumUpBound >= 0) 
                                    {
                                        string presOut = pres.InterpretData(BitUtility.BytesFromHex("0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B0B"), prep);
                                        Console.WriteLine($"{ds.Qualifier} : {prep.Qualifier} @ {presOut} = {pres.Unk1b}, {pres.EnumMaxValue}");
                                    }
                                }
                                if (pres.Qualifier == "PRES_ZIELGANG") 
                                {
                                    pres.PrintDebug();
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("done");
        }

        private void listVariantIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaesarContainer targetContainer = PickContainer();
            if (targetContainer is null)
            {
                Console.WriteLine("Internal error: target container is null");
            }
            else
            {
                foreach (ECU ecu in targetContainer.CaesarECUs) 
                {
                    foreach (ECUVariant variant in ecu.ECUVariants) 
                    {
                        foreach (ECUVariantPattern pattern in variant.VariantPatterns) 
                        {
                            Console.WriteLine($"{variant.Qualifier}: {pattern.VariantID:X4}");
                        }
                    }
                }
            }
        }

        private void downloadBlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connection.ConnectionProtocol is null)
            {
                //MessageBox.Show("Please initiate contact with a target first.");
                //return;
            }
            BlockDownload blockDownload = new BlockDownload(Connection);
            blockDownload.ShowDialog();
        }
    }
}
