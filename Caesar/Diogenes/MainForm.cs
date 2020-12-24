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
        public ECUConnection Connection;

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

            Connection = new ECUConnection();
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

            if ((Connection != null) && (Connection.UDSCapable))
            {
                parentNode.Nodes.Add(diagUnlockingOptions);
            }
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

        private void LoadTree(bool variantFilter = false)
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
                        if (variantFilter)
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

                    if (!variantFilter)
                    {
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

                        if (variantFilter)
                        {
                            ecuVariantNode.ImageIndex = 25;
                            ecuVariantNode.SelectedImageIndex = 25;
                            ecuVariantNode.Expand();
                        }
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

                    // can we help to skip the modal if the ds doesn't require additional user input? common for data, stored data
                    if ((ds.DataClass_ServiceType == (int)DiagService.ServiceType.StoredData) || (ds.DataClass_ServiceType == (int)DiagService.ServiceType.Data))
                    {
                        ExecUserDiagJob(ds.RequestBytes, ds);
                    }
                    else if ((Connection.UDSCapable) && (ds.RequestBytes.Length == 2) && (ds.RequestBytes[0] == 0x27))
                    {
                        // request seed, no need to prompt
                        ExecUserDiagJob(ds.RequestBytes, ds);
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
                ExecUserDiagJob(runDiagForm.Result, ds);
            }
        }

        private void ExecUserDiagJob(byte[] request, DiagService diagService)
        {
            Console.WriteLine($"\r\nRunning: {diagService.Qualifier}");
            byte[] response = Connection.SendMessage(request);
            foreach (List<DiagPreparation> wtf in diagService.OutputPreparations)
            {
                foreach (DiagPreparation outputPreparation in wtf)
                {
                    //outputPreparation.PrintDebug();
                    DiagPresentation presentation = outputPreparation.ParentECU.GlobalPresentations[outputPreparation.PresPoolIndex];
                    // presentation.PrintDebug();
                    Console.WriteLine($"    -> {presentation.InterpretData(response, outputPreparation)}");
                }
            }
            // check if the response was an ECU seed
            if (Connection.UDSCapable && (response.Length >= 2) && (response[0] == 0x67))
            {
                if (response.Length == 2)
                {
                    Console.WriteLine($"Security level has been successfully changed to 0x{(response[1] - 1):X}");
                }
                else
                {
                    byte[] seedValue = response.Skip(2).ToArray();
                    string seedValueAsString = BitUtility.BytesToHex(seedValue, true);
                    if (MessageBox.Show($"Received a seed value of {seedValueAsString}. \r\nCopy to clipboard?", "Security Access", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Clipboard.SetText(seedValueAsString);
                    }
                }
            }
        }

        private void treeViewSelectVariantCoding(TreeNode node) 
        {
            string domainName = node.Text;
            string variantName = node.Parent.Text;
            string ecuName = node.Parent.Parent.Text;
            Console.WriteLine($"Starting VC Dialog for {ecuName} ({variantName}) with domain as {domainName}");

            CaesarContainer container = Containers.Find(x => x.GetECUVariantByName(variantName) != null);
            VCForm vcForm = new VCForm(container, ecuName, variantName, domainName, Connection);
            if (vcForm.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine($"Operator requesting for VC: {BitUtility.BytesToHex(vcForm.VCValue, true)}");

                RunDiagForm runDiagForm = new RunDiagForm(vcForm.WriteService);

                // again, this is a best guess (second largest optional value is usually the SCN at 16 bytes)
                DiagPreparation largestOutPrep = VCForm.GetLargestPreparation(vcForm.WriteService.InputPreparations);

                /*
                 test: med40

                    jg: dumping pres
                    jg: q: SID_RQ pos byte: 0 size bytes: 1 modecfg:323 fieldtype: IntegerType dump: 2E 00 00 00
                    jg: q: RecordDataIdentifier pos byte: 1 size bytes: 2 modecfg:324 fieldtype: IntegerType dump: 01 10 00 00
                    jg: q: #0 pos byte: 33 size bytes: 16 modecfg:6430 fieldtype: BitDumpType dump: 
                    jg: q: #1 pos byte: 49 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
                    jg: q: #2 pos byte: 50 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
                    jg: q: #3 pos byte: 51 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
                    jg: q: #4 pos byte: 52 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
                    jg: q: #5 pos byte: 3 size bytes: 50 modecfg:6410 fieldtype: ExtendedBitDumpType dump: 
                    jg: done dumping pres

                 */


                // construct a write command from presentations: fill up the VC value first, fill up all available dumps, then inherit the last values (fingerprints, scn) from the read command
                byte[] vcParameter = vcForm.VCValue;
                byte[] writeCommand = vcForm.WriteService.RequestBytes;
                byte[] priorReadCommand = vcForm.UnfilteredReadValue;

                // start with a list of all values that we will have to fill
                List<DiagPreparation> preparationsToProcess = new List<DiagPreparation>(vcForm.WriteService.InputPreparations);

                // fill up vc
                DiagPreparation vcPrep = preparationsToProcess.Find(x => x.FieldType == DiagPreparation.InferredDataType.ExtendedBitDumpType);
                if (vcPrep is null)
                {
                    MessageBox.Show("VC: Could not find the VC ExtendedBitDump prep, stopping early to save your ECU.", "VC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int vcPrepSizeInBytes = vcPrep.SizeInBits / 8;
                int vcPrepBytePosition = vcPrep.BitPosition / 8;
                if (vcPrepSizeInBytes < vcParameter.Length)
                {
                    MessageBox.Show("VC: VC string is longer than the parameter can fit, stopping early to save your ECU.", "VC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // zero out the destination buffer for the param that we intend to write since there's a possibility that our param is shorter than the actual prep's size
                for (int i = vcPrepBytePosition; i < (vcPrepBytePosition + vcPrepSizeInBytes); i++)
                {
                    writeCommand[i] = 0;
                }
                // copy the parameter in
                Array.ConstrainedCopy(vcParameter, 0, writeCommand, vcPrepBytePosition, vcParameter.Length);
                preparationsToProcess.Remove(vcPrep);

                // merge prefilled values such as the (SID_RQ, id..)
                List<DiagPreparation> prefilledValues = new List<DiagPreparation>();
                foreach (DiagPreparation prep in preparationsToProcess)
                {
                    if (prep.Dump.Length > 0)
                    {
                        prefilledValues.Add(prep);
                        if (prep.FieldType == DiagPreparation.InferredDataType.IntegerType)
                        {
                            byte[] fixedDump = prep.Dump.Take(prep.SizeInBits / 8).Reverse().ToArray();
                            Array.ConstrainedCopy(vcParameter, 0, writeCommand, vcPrepBytePosition, vcParameter.Length);
                        }
                        else
                        {
                            Console.WriteLine($"Skipping prefill for {prep.Qualifier} as the data type {prep.FieldType} is unsupported.");
                        }
                    }
                }

                // "mark" the constants as done
                foreach (DiagPreparation prep in prefilledValues)
                {
                    preparationsToProcess.Remove(prep);
                }

                // at this point, whatever that's left in preparationsToProcess are stuff that are variable, but should be copied verbatim from the original read request (e.g. fingerprints, scn)
                // log the assumptions, show it the operator just in case
                StringBuilder assumptionsMade = new StringBuilder();
                if (preparationsToProcess.Count > 0)
                {
                    if (writeCommand.Length != priorReadCommand.Length)
                    {
                        MessageBox.Show("There are some preparations that do not have a default value (e.g. fingerprint, scn). \r\n" +
                            "The input and output values do not have matching lengths, which means that the automatic assumption may be wrong. \r\n" +
                            "Please be very careful when proceeding.", "Warning");
                    }

                    foreach (DiagPreparation prep in preparationsToProcess)
                    {
                        int bytePosition = prep.BitPosition / 8;
                        int byteLength = prep.SizeInBits / 8;
                        Array.ConstrainedCopy(priorReadCommand, bytePosition, writeCommand, bytePosition, byteLength);
                        assumptionsMade.Append($"{prep.Qualifier} : {BitUtility.BytesToHex(priorReadCommand.Skip(bytePosition).Take(byteLength).ToArray(), true)}\r\n");
                    }
                }

                /*
                // lazy me dumping the values
                for (int i = 0; i < vcForm.WriteService.InputPreparations.Count; i++)
                {
                    DiagPreparation prep = vcForm.WriteService.InputPreparations[i];
                    Console.WriteLine($"debug: q: {prep.Qualifier} pos byte: {(prep.BitPosition / 8)} size bytes: {(prep.SizeInBits / 8)} modecfg:{prep.ModeConfig:X} fieldtype: {prep.FieldType} dump: {BitUtility.BytesToHex(prep.Dump, true)}");
                }
                */

                // we are done preparing the command, if we are confident we can send the command straight to the ECU, else, let the user review
                if (assumptionsMade.Length > 0) 
                {
                    if (MessageBox.Show("Some assumptions were made when preparing the write parameters. \r\n\r\n" +
                        "You may wish to review them by selecting Cancel, or select OK to execute the write command immediately.\r\n\r\n" + assumptionsMade.ToString(),
                        "Review assumptions", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        ExecVCWrite(runDiagForm.Result, vcForm.WriteService);
                    }
                    else
                    {
                        runDiagForm.Result = writeCommand;
                        if (runDiagForm.ShowDialog() == DialogResult.OK)
                        {
                            ExecVCWrite(runDiagForm.Result, vcForm.WriteService);
                        }
                    }
                }
            }
        }

        private void ExecVCWrite(byte[] request, DiagService service)
        {
            bool allowVcWrite = allowWriteVariantCodingToolStripMenuItem.Checked;
            if (allowVcWrite)
            {
                ExecUserDiagJob(request, service);
            }
            else
            {
                MessageBox.Show("This VC write action has to be manually enabled under \r\nFile >  Allow Write Variant Coding\r\nPlease make sure that you understand the risks before doing so.", 
                    "Accidental Brick Protection");
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
                            Connection.Connect(subtype, ecu);
                            TryUdsAuto();
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

        private void TryUdsAuto()
        {
            // try to perform session switch and variant detection if user's okay with it
            if (Connection.UDSCapable && (MessageBox.Show(
                "The target appears to be UDS capable. Allow Diogenes to switch session states and detect the variant?",
                "UDS assist",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                )
            {
                if (UDS.TryDetectVariantAndSwitchSession(Connection, Containers))
                {
                    LoadTree(true);
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
    }
}
