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
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

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
                treeImages.Images.Add(Resources.blank);
                treeImages.Images.Add(Resources.box);
                treeImages.Images.Add(Resources.brick);
                treeImages.Images.Add(Resources.cog);
                treeImages.Images.Add(Resources.house);
                treeImages.Images.Add(Resources.connect);
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
                    // does not seem relevant to users
                    foreach (ECUInterfaceSubtype subtype in ecu.ECUInterfaceSubtypes)
                    {
                        TreeNode subtypeNode = new TreeNode(subtype.ctName, 5, 5);
                        subtypeNode.Tag = nameof(ECUInterfaceSubtype);
                        ecuNode.Nodes.Add(subtypeNode);
                    }
                    */

                    foreach (ECUInterface ecuInterface in ecu.ECUInterfaces)
                    {
                        TreeNode interfaceNode = new TreeNode(ecuInterface.interfaceName, 5, 5);
                        interfaceNode.Tag = nameof(ECUInterface);
                        /*
                        // does not seem relevant to users
                        foreach (string comParameter in ecuInterface.comParameters)
                        {
                            TreeNode comNode = new TreeNode(comParameter, 5, 5);
                            comNode.Tag = "COMPARAMETER";
                            interfaceNode.Nodes.Add(comNode);
                        }
                        */
                        ecuNode.Nodes.Add(interfaceNode);
                    }

                    foreach (ECUVariant variant in ecu.ECUVariants) 
                    {
                        TreeNode ecuVariantNode = new TreeNode(variant.variantName, 2, 2);
                        ecuVariantNode.Tag = nameof(ECUVariant);
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
            if (node.Tag.ToString() != nameof(VCDomain)) 
            {
                return;
            }
            string domainName = node.Text;
            string variantName = node.Parent.Text;
            string ecuName = node.Parent.Parent.Text;
            Console.WriteLine($"Starting VC for {ecuName} ({variantName}) with domain as {domainName}");

            CaesarContainer container = Containers.Find(x => x.GetECUVariantByName(variantName) != null);
            VCForm vcForm = new VCForm(container, ecuName, variantName, domainName, new byte[] { });
            if (vcForm.ShowDialog() == DialogResult.OK) 
            {
                Console.WriteLine($"VC Confirmation: {domainName} : {BitUtility.BytesToHex(vcForm.VCValue)}");
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

            Console.WriteLine("Enumerating J2534 devices");
            foreach (APIInfo apiInfo in APIFactory.GetAPIList()) 
            {
                defaultItem.Visible = false;
                Console.WriteLine($"Found {apiInfo.Name} from {apiInfo.Filename}");
                ToolStripItem newItem = j2534InterfacesToolStripMenuItem.DropDownItems.Add(apiInfo.Name);
                newItem.Tag = apiInfo.Filename;
                newItem.Click += J2534InterfaceItem_Click;
            }

            Console.WriteLine("Completed enumeration of J2534 devices");
        }

        private void J2534InterfaceItem_Click(object sender, EventArgs e)
        {
            ToolStripItem caller = (ToolStripItem)sender;
            if (Connection != null) 
            {
                // close connection first
            }
            Connection = new ECUConnection(caller.Tag.ToString());
            //lblConnectionType.Text = $"Loaded: {Connection.ConnectionDevice.DeviceName} (Pending connection)";
        }
    }
}
