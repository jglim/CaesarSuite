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

        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        public const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);


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
            SendMessage(txtJ2534Input.Handle, UnmanagedUtility.EM_SETCUEBANNER, 0, "J2534 Console : Enter hex values (01 23 45 57) and press enter to send a raw J2534 command");

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
                treeImages.Images.Add(Resources.lock_edit);
                treeImages.Images.Add(Resources.key);

                tvMain.ImageList = treeImages;

                SendMessage(tvMain.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
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

                    TreeNode ecuNode = new TreeNode(ecu.ecuName, 1, 1);
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

                    foreach (ECUInterfaceSubtype subtype in ecu.ECUInterfaceSubtypes)
                    {
                        TreeNode interfaceNode = new TreeNode(subtype.ctName, 5, 5);
                        interfaceNode.Tag = "";

                        TreeNode initiateContactNode = new TreeNode("Initiate Contact", 18, 18);
                        initiateContactNode.Tag = $"{nameof(ECUInterfaceSubtype)}:{subtype.ctName}";
                        interfaceNode.Nodes.Add(initiateContactNode);

                        foreach (ComParameter parameter in subtype.CommunicationParameters)
                        {
                            TreeNode comNode = new TreeNode($"{parameter.ParamName} : {parameter.comValue} (0x{parameter.comValue:X})", 9, 9);
                            comNode.Tag = nameof(ComParameter);
                            interfaceNode.Nodes.Add(comNode);
                        }
                        ecuNode.Nodes.Add(interfaceNode);
                    }

                    foreach (ECUVariant variant in ecu.ECUVariants) 
                    {
                        TreeNode ecuVariantNode = new TreeNode(variant.variantName, 2, 2);
                        ecuVariantNode.Tag = nameof(ECUVariant);

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
                            TreeNode vcDomainNode = new TreeNode(domain.vcdName, 3, 3);
                            vcDomainNode.Tag = nameof(VCDomain);
                            ecuVariantNode.Nodes.Add(vcDomainNode);
                        }

                        ecuNode.Nodes.Add(ecuVariantNode);
                    }
                    tvMain.Nodes.Add(ecuNode);
                }
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
            else if (node.Tag.ToString().StartsWith(nameof(ECUInterfaceSubtype)))
            {
                string connectionProfileName = node.Tag.ToString().Substring(nameof(ECUInterfaceSubtype).Length + 1);
                string ecuName = node.Parent.Parent.Text;

                foreach (CaesarContainer container in Containers) 
                {
                    ECU ecu = container.CaesarECUs.Find(x => x.ecuName == ecuName);
                    if (ecu != null)
                    {
                        ECUInterfaceSubtype subtype = ecu.ECUInterfaceSubtypes.Find(x => x.ctName == connectionProfileName);
                        if (subtype != null)
                        {
                            Console.WriteLine($"Attempting to open a connection to ({ecuName}) with profile '{connectionProfileName}'");
                            Connection.Connect(subtype, ecu);
                            break;
                        }
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
