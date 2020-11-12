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
using SAE.J2534;

namespace Diogenes
{
    public partial class MainForm : Form
    {
        private ECUConnection Connection;


        public MainForm()
        {
            InitializeComponent();
        }

        List<CaesarContainer> Containers = new List<CaesarContainer>();
        ImageList treeImages = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            RedirectConsole();
            LoadContainers(); 
            UnmanagedUtility.SendMessage(txtJ2534Input.Handle, UnmanagedUtility.EM_SETCUEBANNER, 0, "J2534 Console : Enter hex values (01 23 45 57) and press enter to send a raw J2534 command");

            Connection = new ECUConnection();
        }

        private void RedirectConsole() 
        {
            TextboxWriter writer = new TextboxWriter(txtLog);
            Console.SetOut(writer);
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
                    Console.WriteLine($"Loaded {Path.GetFileName(file)}");
                }
            }
            LoadTree();
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

                tvMain.ImageList = treeImages;

                UnmanagedUtility.SendMessage(tvMain.Handle, UnmanagedUtility.TVM_SETEXTENDEDSTYLE, (IntPtr)UnmanagedUtility.TVS_EX_DOUBLEBUFFER, (IntPtr)UnmanagedUtility.TVS_EX_DOUBLEBUFFER);
            }
        }

        private void LoadTree()
        {
            InitializeTree();
            tvMain.Nodes.Clear();

            foreach (CaesarContainer container in Containers) 
            {
                foreach (ECU ecu in container.CaesarECUs)
                {
                    // cbfContainer.CaesarECUs.ForEach(x => x.ECUInterfaces.ForEach(y => y.PrintDebug()));
                    // cbfContainer.CaesarECUs.ForEach(x => x.ECUInterfaceSubtypes.ForEach(y => Console.WriteLine(y.ctName)));

                    TreeNode ecuNode = new TreeNode(ecu.Qualifier, 1, 1);
                    ecuNode.Tag = nameof(ECU);
                    //ecuNode.ImageIndex = -1;

                    /*
                    foreach (ECUInterface ecuInterface in ecu.ECUInterfaces)
                    {
                        TreeNode interfaceNode = new TreeNode(ecuInterface.interfaceNameQualifier, 5, 5);
                        interfaceNode.Tag = nameof(ECUInterface);

                        // does not seem relevant to users
                        foreach (string comParameter in ecuInterface.comParameters)
                        {
                            TreeNode comNode = new TreeNode(comParameter, 9, 9);
                            comNode.Tag = "COMPARAMETER";
                            interfaceNode.Nodes.Add(comNode);
                        }

                        ecuNode.Nodes.Add(interfaceNode);
                    }
                    */
                    TreeNode execDiagAtRoot = new TreeNode("Execute Diagnostic Service (Root)", 21, 21);
                    execDiagAtRoot.Tag = $"{nameof(DiagService)}:{nameof(ECU)}:{ecu.Qualifier}";
                    ecuNode.Nodes.Add(execDiagAtRoot);

                    TreeNode fixCbfPermissions = new TreeNode("Fix Client Access Permissions (Export)", 22, 22);
                    fixCbfPermissions.Tag = $"CALFix";
                    ecuNode.Nodes.Add(fixCbfPermissions);

                    foreach (ECUInterfaceSubtype subtype in ecu.ECUInterfaceSubtypes)
                    {
                        TreeNode interfaceNode = new TreeNode(subtype.Qualifier, 5, 5);
                        interfaceNode.Tag = "";

                        TreeNode initiateContactNode = new TreeNode("Initiate Contact", 18, 18);
                        initiateContactNode.Tag = $"{nameof(ECUInterfaceSubtype)}:{subtype.Qualifier}";
                        interfaceNode.Nodes.Add(initiateContactNode);

                        foreach (ComParameter parameter in subtype.CommunicationParameters)
                        {
                            TreeNode comNode = new TreeNode($"{parameter.ParamName} : {parameter.ComParamValue} (0x{parameter.ComParamValue:X})", 9, 9);
                            comNode.Tag = nameof(ComParameter);
                            interfaceNode.Nodes.Add(comNode);
                        }
                        ecuNode.Nodes.Add(interfaceNode);
                    }

                    foreach (ECUVariant variant in ecu.ECUVariants) 
                    {
                        TreeNode ecuVariantNode = new TreeNode(variant.Qualifier, 2, 2);
                        ecuVariantNode.Tag = nameof(ECUVariant);

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
                            TreeNode patternNode = new TreeNode($"Variant ID: {pattern.VariantID}{vendorText}", 9, 9);
                            patternNode.Tag = nameof(ECUVariantPattern);
                            metadataNode.Nodes.Add(patternNode);
                        }
                        
                        ecuVariantNode.Nodes.Add(metadataNode);

                        // vc domains
                        foreach (VCDomain domain in variant.VCDomains) 
                        {
                            TreeNode vcDomainNode = new TreeNode(domain.Qualifier, 3, 3);
                            vcDomainNode.Tag = nameof(VCDomain);
                            ecuVariantNode.Nodes.Add(vcDomainNode);
                        }

                        ecuNode.Nodes.Add(ecuVariantNode);
                    }
                    tvMain.Nodes.Add(ecuNode);
                }
            }
        }

        private void FixCALs(CaesarContainer container, ECU ecu) 
        {
            int newLevel = 1;
            byte[] newFile = new byte[container.FileBytes.Length];
            Buffer.BlockCopy(container.FileBytes, 0, newFile, 0, container.FileBytes.Length);

            Console.WriteLine($"Creating a new CBF with access level requirements set at {newLevel}");
            List<DiagService> dsPendingFix = new List<DiagService>();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(container.FileBytes)))
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

                uint checksum = Caesar.CaesarReader.ComputeFileChecksum(newFile);
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

        private void tvMain_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = tvMain.SelectedNode;
            if (node is null)
            {
                return;
            }

            if (node.Tag.ToString() == nameof(VCDomain))
            {
                string domainName = node.Text;
                string variantName = node.Parent.Text;
                string ecuName = node.Parent.Parent.Text;
                Console.WriteLine($"Starting VC for {ecuName} ({variantName}) with domain as {domainName}");

                CaesarContainer container = Containers.Find(x => x.GetECUVariantByName(variantName) != null);
                VCForm vcForm = new VCForm(container, ecuName, variantName, domainName, Connection);
                if (vcForm.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine($"VC Confirmation: {domainName} : {BitUtility.BytesToHex(vcForm.VCValue)}");
                }
            }
            else if (node.Tag.ToString() == "CALFix")
            {
                foreach (CaesarContainer container in Containers)
                {
                    ECU ecu = container.CaesarECUs.Find(x => x.Qualifier == node.Parent.Text);
                    if (ecu != null)
                    {
                        FixCALs(container, ecu);
                        break;
                    }
                }
            }
            else if (node.Tag.ToString().StartsWith(nameof(ECUInterfaceSubtype)))
            {
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
                            Connection.Connect(subtype, ecu);
                            break;
                        }
                    }
                }
            }
            else if (node.Tag.ToString().StartsWith(nameof(DiagService)))
            {
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
                            Console.WriteLine($"Starting Diagnostic Service picker modal for variant {variantName}");
                            picker = new PickDiagForm(variant.DiagServices);
                        }
                        else
                        {
                            Console.WriteLine($"Starting Diagnostic Service picker modal for root {ecuName}");
                            picker = new PickDiagForm(ecu.GlobalDiagServices.ToArray());
                        }
                        if (picker.ShowDialog() == DialogResult.OK) 
                        {
                            RunDiagForm runDiagForm = new RunDiagForm(picker.SelectedDiagService);
                            runDiagForm.ShowDialog();
                            // Console.WriteLine($"Picker returned: {picker.SelectedDiagService.qualifierName}");
                        }
                        break;
                    }
                }

            }
        }
        private void ShowAbout() 
        {
            // please change this if you fork the project, thanks!
            MessageBox.Show($"Diogenes {GetVersion()}\nCaesar {CaesarContainer.GetCaesarVersionString()}\n\nIcons from famfamfam\nhttps://github.com/jglim/CaesarSuite", "About", MessageBoxButtons.OK);
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
                Containers.Add(new CaesarContainer(File.ReadAllBytes(ofd.FileName)));
                LoadTree();
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

            foreach (APIInfo apiInfo in APIFactory.GetAPIList()) 
            {
                defaultItem.Visible = false;
                // Console.WriteLine($"Found {apiInfo.Name} from {apiInfo.Filename}");
                ToolStripItem newItem = j2534InterfacesToolStripMenuItem.DropDownItems.Add(apiInfo.Name);
                newItem.Tag = apiInfo.Filename;
                newItem.Click += J2534InterfaceItem_Click;
            }

            // Console.WriteLine("Completed enumeration of J2534 devices");
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
            //lblConnectionType.Text = $"Loaded: {Connection.ConnectionDevice.DeviceName} (Pending connection)";
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
                    byte[] requestData = BitUtility.BytesFromHex(inText);
                    Connection.SendMessage(requestData);
                }
                else 
                {
                    Console.WriteLine($"Could not understand provided hex input: '{inText}'");
                }

            }
        }
    }
}
