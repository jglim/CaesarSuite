namespace Diogenes
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnectionType = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCBFFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadExistingFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadCompressedJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportContainerAsCompressedJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportContainerAsJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.j2534InterfacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultJ2534InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eCUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSecurityLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixClientAccessPermissionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cFFFlashSplicerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cFFExportFlashSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.uDSHexEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagnosticTroubleCodesDTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewECUMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.showTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.identifyECUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dSCDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixStringsDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listVariantIDsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fingerprintModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLastFingerprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sCNModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLastSCNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeZerosVediamoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowWriteVariantCodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvMain = new System.Windows.Forms.TreeView();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pbResourcePlaceholder = new System.Windows.Forms.PictureBox();
            this.txtJ2534Input = new System.Windows.Forms.TextBox();
            this.tmrBlinkConnectionMenu = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResourcePlaceholder)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnectionType});
            this.statusStrip1.Location = new System.Drawing.Point(0, 706);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1123, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblConnectionType
            // 
            this.lblConnectionType.Name = "lblConnectionType";
            this.lblConnectionType.Size = new System.Drawing.Size(201, 17);
            this.lblConnectionType.Text = "No interface selected (Disconnected)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.connectionToolStripMenuItem,
            this.eCUToolStripMenuItem,
            this.preferencesToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1123, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadCBFFilesToolStripMenuItem,
            this.unloadExistingFilesToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadCompressedJsonToolStripMenuItem,
            this.exportContainerAsCompressedJSONToolStripMenuItem,
            this.loadJSONToolStripMenuItem,
            this.exportContainerAsJSONToolStripMenuItem,
            this.toolStripSeparator6,
            this.aboutToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadCBFFilesToolStripMenuItem
            // 
            this.loadCBFFilesToolStripMenuItem.Name = "loadCBFFilesToolStripMenuItem";
            this.loadCBFFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadCBFFilesToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.loadCBFFilesToolStripMenuItem.Text = "Load CBF Files";
            this.loadCBFFilesToolStripMenuItem.Click += new System.EventHandler(this.loadCBFFilesToolStripMenuItem_Click);
            // 
            // unloadExistingFilesToolStripMenuItem
            // 
            this.unloadExistingFilesToolStripMenuItem.Name = "unloadExistingFilesToolStripMenuItem";
            this.unloadExistingFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.unloadExistingFilesToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.unloadExistingFilesToolStripMenuItem.Text = "Unload Existing Files";
            this.unloadExistingFilesToolStripMenuItem.Click += new System.EventHandler(this.unloadExistingFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(273, 6);
            // 
            // loadCompressedJsonToolStripMenuItem
            // 
            this.loadCompressedJsonToolStripMenuItem.Name = "loadCompressedJsonToolStripMenuItem";
            this.loadCompressedJsonToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.loadCompressedJsonToolStripMenuItem.Text = "Load Compressed JSON";
            this.loadCompressedJsonToolStripMenuItem.Click += new System.EventHandler(this.loadCompressedJsonToolStripMenuItem_Click);
            // 
            // exportContainerAsCompressedJSONToolStripMenuItem
            // 
            this.exportContainerAsCompressedJSONToolStripMenuItem.Name = "exportContainerAsCompressedJSONToolStripMenuItem";
            this.exportContainerAsCompressedJSONToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.exportContainerAsCompressedJSONToolStripMenuItem.Text = "Export Container as Compressed JSON";
            this.exportContainerAsCompressedJSONToolStripMenuItem.Click += new System.EventHandler(this.exportContainerAsCompressedJSONToolStripMenuItem_Click);
            // 
            // loadJSONToolStripMenuItem
            // 
            this.loadJSONToolStripMenuItem.Name = "loadJSONToolStripMenuItem";
            this.loadJSONToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.loadJSONToolStripMenuItem.Text = "Load JSON";
            this.loadJSONToolStripMenuItem.Click += new System.EventHandler(this.loadJSONToolStripMenuItem_Click);
            // 
            // exportContainerAsJSONToolStripMenuItem
            // 
            this.exportContainerAsJSONToolStripMenuItem.Name = "exportContainerAsJSONToolStripMenuItem";
            this.exportContainerAsJSONToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.exportContainerAsJSONToolStripMenuItem.Text = "Export Container as JSON";
            this.exportContainerAsJSONToolStripMenuItem.Click += new System.EventHandler(this.exportContainerAsJSONToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(273, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.aboutToolStripMenuItem.Text = "About..";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(273, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.j2534InterfacesToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.connectionToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.connectionToolStripMenuItem.Text = "Connection";
            this.connectionToolStripMenuItem.DropDownOpening += new System.EventHandler(this.connectionToolStripMenuItem_DropDownOpening);
            // 
            // j2534InterfacesToolStripMenuItem
            // 
            this.j2534InterfacesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultJ2534InterfaceToolStripMenuItem});
            this.j2534InterfacesToolStripMenuItem.Name = "j2534InterfacesToolStripMenuItem";
            this.j2534InterfacesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.j2534InterfacesToolStripMenuItem.Text = "J2534 Interfaces";
            this.j2534InterfacesToolStripMenuItem.DropDownOpening += new System.EventHandler(this.j2534InterfacesToolStripMenuItem_DropDownOpening);
            // 
            // defaultJ2534InterfaceToolStripMenuItem
            // 
            this.defaultJ2534InterfaceToolStripMenuItem.Name = "defaultJ2534InterfaceToolStripMenuItem";
            this.defaultJ2534InterfaceToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.defaultJ2534InterfaceToolStripMenuItem.Text = "Default J2534 Interface";
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // eCUToolStripMenuItem
            // 
            this.eCUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setSecurityLevelToolStripMenuItem,
            this.fixClientAccessPermissionsToolStripMenuItem,
            this.toolStripSeparator4,
            this.cFFFlashSplicerToolStripMenuItem,
            this.cFFExportFlashSegmentsToolStripMenuItem,
            this.toolStripSeparator3,
            this.uDSHexEditorToolStripMenuItem,
            this.diagnosticTroubleCodesDTCToolStripMenuItem,
            this.viewECUMetadataToolStripMenuItem,
            this.toolStripSeparator5,
            this.showTraceToolStripMenuItem,
            this.copyConsoleToolStripMenuItem,
            this.clearConsoleToolStripMenuItem,
            this.identifyECUToolStripMenuItem,
            this.dSCDebugToolStripMenuItem,
            this.fixStringsDebugToolStripMenuItem,
            this.listVariantIDsToolStripMenuItem});
            this.eCUToolStripMenuItem.Name = "eCUToolStripMenuItem";
            this.eCUToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.eCUToolStripMenuItem.Text = "Tools";
            // 
            // setSecurityLevelToolStripMenuItem
            // 
            this.setSecurityLevelToolStripMenuItem.Name = "setSecurityLevelToolStripMenuItem";
            this.setSecurityLevelToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.setSecurityLevelToolStripMenuItem.Text = "Set Security Level (DLL)";
            this.setSecurityLevelToolStripMenuItem.Click += new System.EventHandler(this.setSecurityLevelToolStripMenuItem_Click);
            // 
            // fixClientAccessPermissionsToolStripMenuItem
            // 
            this.fixClientAccessPermissionsToolStripMenuItem.Name = "fixClientAccessPermissionsToolStripMenuItem";
            this.fixClientAccessPermissionsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.fixClientAccessPermissionsToolStripMenuItem.Text = "Fix Client Access Permissions";
            this.fixClientAccessPermissionsToolStripMenuItem.Click += new System.EventHandler(this.fixClientAccessPermissionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(238, 6);
            // 
            // cFFFlashSplicerToolStripMenuItem
            // 
            this.cFFFlashSplicerToolStripMenuItem.Name = "cFFFlashSplicerToolStripMenuItem";
            this.cFFFlashSplicerToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.cFFFlashSplicerToolStripMenuItem.Text = "CFF: Flash Splicer";
            this.cFFFlashSplicerToolStripMenuItem.Click += new System.EventHandler(this.cFFFlashSplicerToolStripMenuItem_Click);
            // 
            // cFFExportFlashSegmentsToolStripMenuItem
            // 
            this.cFFExportFlashSegmentsToolStripMenuItem.Name = "cFFExportFlashSegmentsToolStripMenuItem";
            this.cFFExportFlashSegmentsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.cFFExportFlashSegmentsToolStripMenuItem.Text = "CFF: Export Flash Segments";
            this.cFFExportFlashSegmentsToolStripMenuItem.Click += new System.EventHandler(this.cFFExportFlashSegmentsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(238, 6);
            // 
            // uDSHexEditorToolStripMenuItem
            // 
            this.uDSHexEditorToolStripMenuItem.Name = "uDSHexEditorToolStripMenuItem";
            this.uDSHexEditorToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.uDSHexEditorToolStripMenuItem.Text = "UDS Hex Editor";
            this.uDSHexEditorToolStripMenuItem.Click += new System.EventHandler(this.uDSHexEditorToolStripMenuItem_Click);
            // 
            // diagnosticTroubleCodesDTCToolStripMenuItem
            // 
            this.diagnosticTroubleCodesDTCToolStripMenuItem.Name = "diagnosticTroubleCodesDTCToolStripMenuItem";
            this.diagnosticTroubleCodesDTCToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.diagnosticTroubleCodesDTCToolStripMenuItem.Text = "Diagnostic Trouble Codes (DTC)";
            this.diagnosticTroubleCodesDTCToolStripMenuItem.Click += new System.EventHandler(this.diagnosticTroubleCodesDTCToolStripMenuItem_Click);
            // 
            // viewECUMetadataToolStripMenuItem
            // 
            this.viewECUMetadataToolStripMenuItem.Name = "viewECUMetadataToolStripMenuItem";
            this.viewECUMetadataToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.viewECUMetadataToolStripMenuItem.Text = "View ECU Metadata";
            this.viewECUMetadataToolStripMenuItem.Click += new System.EventHandler(this.viewECUMetadataToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(238, 6);
            // 
            // showTraceToolStripMenuItem
            // 
            this.showTraceToolStripMenuItem.Name = "showTraceToolStripMenuItem";
            this.showTraceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.showTraceToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.showTraceToolStripMenuItem.Text = "Show Trace";
            this.showTraceToolStripMenuItem.Click += new System.EventHandler(this.showTraceToolStripMenuItem_Click);
            // 
            // copyConsoleToolStripMenuItem
            // 
            this.copyConsoleToolStripMenuItem.Name = "copyConsoleToolStripMenuItem";
            this.copyConsoleToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.copyConsoleToolStripMenuItem.Text = "Copy Console";
            this.copyConsoleToolStripMenuItem.Click += new System.EventHandler(this.copyConsoleToolStripMenuItem_Click);
            // 
            // clearConsoleToolStripMenuItem
            // 
            this.clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            this.clearConsoleToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.clearConsoleToolStripMenuItem.Text = "Clear Console";
            this.clearConsoleToolStripMenuItem.Click += new System.EventHandler(this.clearConsoleToolStripMenuItem_Click);
            // 
            // identifyECUToolStripMenuItem
            // 
            this.identifyECUToolStripMenuItem.Name = "identifyECUToolStripMenuItem";
            this.identifyECUToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.identifyECUToolStripMenuItem.Text = "Identify ECU";
            this.identifyECUToolStripMenuItem.Visible = false;
            this.identifyECUToolStripMenuItem.Click += new System.EventHandler(this.identifyECUToolStripMenuItem_Click);
            // 
            // dSCDebugToolStripMenuItem
            // 
            this.dSCDebugToolStripMenuItem.Name = "dSCDebugToolStripMenuItem";
            this.dSCDebugToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.dSCDebugToolStripMenuItem.Text = "DSC Debug";
            this.dSCDebugToolStripMenuItem.Visible = false;
            this.dSCDebugToolStripMenuItem.Click += new System.EventHandler(this.dSCDebugToolStripMenuItem_Click);
            // 
            // fixStringsDebugToolStripMenuItem
            // 
            this.fixStringsDebugToolStripMenuItem.Name = "fixStringsDebugToolStripMenuItem";
            this.fixStringsDebugToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.fixStringsDebugToolStripMenuItem.Text = "FixStrings Debug";
            this.fixStringsDebugToolStripMenuItem.Click += new System.EventHandler(this.fixStringsDebugToolStripMenuItem_Click);
            // 
            // listVariantIDsToolStripMenuItem
            // 
            this.listVariantIDsToolStripMenuItem.Name = "listVariantIDsToolStripMenuItem";
            this.listVariantIDsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.listVariantIDsToolStripMenuItem.Text = "List Variant IDs";
            this.listVariantIDsToolStripMenuItem.Click += new System.EventHandler(this.listVariantIDsToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem1
            // 
            this.preferencesToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fingerprintModeToolStripMenuItem,
            this.sCNModeToolStripMenuItem,
            this.allowWriteVariantCodingToolStripMenuItem});
            this.preferencesToolStripMenuItem1.Name = "preferencesToolStripMenuItem1";
            this.preferencesToolStripMenuItem1.Size = new System.Drawing.Size(80, 20);
            this.preferencesToolStripMenuItem1.Text = "Preferences";
            this.preferencesToolStripMenuItem1.DropDownOpening += new System.EventHandler(this.preferencesToolStripMenuItem1_DropDownOpening);
            // 
            // fingerprintModeToolStripMenuItem
            // 
            this.fingerprintModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useLastFingerprintToolStripMenuItem,
            this.customValueToolStripMenuItem});
            this.fingerprintModeToolStripMenuItem.Name = "fingerprintModeToolStripMenuItem";
            this.fingerprintModeToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.fingerprintModeToolStripMenuItem.Text = "Fingerprint Mode";
            // 
            // useLastFingerprintToolStripMenuItem
            // 
            this.useLastFingerprintToolStripMenuItem.Name = "useLastFingerprintToolStripMenuItem";
            this.useLastFingerprintToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.useLastFingerprintToolStripMenuItem.Text = "Use Last Fingerprint";
            this.useLastFingerprintToolStripMenuItem.Click += new System.EventHandler(this.useLastFingerprintToolStripMenuItem_Click);
            // 
            // customValueToolStripMenuItem
            // 
            this.customValueToolStripMenuItem.Name = "customValueToolStripMenuItem";
            this.customValueToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.customValueToolStripMenuItem.Text = "Custom Value";
            this.customValueToolStripMenuItem.Click += new System.EventHandler(this.customValueToolStripMenuItem_Click);
            // 
            // sCNModeToolStripMenuItem
            // 
            this.sCNModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useLastSCNToolStripMenuItem,
            this.writeZerosVediamoToolStripMenuItem});
            this.sCNModeToolStripMenuItem.Name = "sCNModeToolStripMenuItem";
            this.sCNModeToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.sCNModeToolStripMenuItem.Text = "SCN Mode";
            // 
            // useLastSCNToolStripMenuItem
            // 
            this.useLastSCNToolStripMenuItem.Name = "useLastSCNToolStripMenuItem";
            this.useLastSCNToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.useLastSCNToolStripMenuItem.Text = "Use Last SCN";
            this.useLastSCNToolStripMenuItem.Click += new System.EventHandler(this.useLastSCNToolStripMenuItem_Click);
            // 
            // writeZerosVediamoToolStripMenuItem
            // 
            this.writeZerosVediamoToolStripMenuItem.Name = "writeZerosVediamoToolStripMenuItem";
            this.writeZerosVediamoToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.writeZerosVediamoToolStripMenuItem.Text = "Write Zeros (Vediamo)";
            this.writeZerosVediamoToolStripMenuItem.Click += new System.EventHandler(this.writeZerosVediamoToolStripMenuItem_Click);
            // 
            // allowWriteVariantCodingToolStripMenuItem
            // 
            this.allowWriteVariantCodingToolStripMenuItem.Name = "allowWriteVariantCodingToolStripMenuItem";
            this.allowWriteVariantCodingToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.allowWriteVariantCodingToolStripMenuItem.Text = "Allow Write Variant Coding";
            this.allowWriteVariantCodingToolStripMenuItem.Click += new System.EventHandler(this.allowWriteVariantCodingToolStripMenuItem_Click);
            // 
            // tvMain
            // 
            this.tvMain.AllowDrop = true;
            this.tvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMain.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvMain.Location = new System.Drawing.Point(3, 3);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(1117, 480);
            this.tvMain.TabIndex = 2;
            this.tvMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvMain_DragDrop);
            this.tvMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvMain_DragEnter);
            this.tvMain.DoubleClick += new System.EventHandler(this.tvMain_DoubleClick);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1117, 160);
            this.txtLog.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvMain);
            this.splitContainer1.Panel1.Controls.Add(this.pbResourcePlaceholder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtJ2534Input);
            this.splitContainer1.Panel2.Controls.Add(this.txtLog);
            this.splitContainer1.Size = new System.Drawing.Size(1123, 682);
            this.splitContainer1.SplitterDistance = 486;
            this.splitContainer1.TabIndex = 5;
            // 
            // pbResourcePlaceholder
            // 
            this.pbResourcePlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbResourcePlaceholder.Image = global::Diogenes.Properties.Resources.report;
            this.pbResourcePlaceholder.Location = new System.Drawing.Point(1070, 3);
            this.pbResourcePlaceholder.Name = "pbResourcePlaceholder";
            this.pbResourcePlaceholder.Size = new System.Drawing.Size(50, 50);
            this.pbResourcePlaceholder.TabIndex = 4;
            this.pbResourcePlaceholder.TabStop = false;
            this.pbResourcePlaceholder.Visible = false;
            // 
            // txtJ2534Input
            // 
            this.txtJ2534Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJ2534Input.Enabled = false;
            this.txtJ2534Input.Location = new System.Drawing.Point(3, 169);
            this.txtJ2534Input.Name = "txtJ2534Input";
            this.txtJ2534Input.Size = new System.Drawing.Size(1117, 20);
            this.txtJ2534Input.TabIndex = 4;
            this.txtJ2534Input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtJ2534Input_KeyDown);
            // 
            // tmrBlinkConnectionMenu
            // 
            this.tmrBlinkConnectionMenu.Interval = 60;
            this.tmrBlinkConnectionMenu.Tick += new System.EventHandler(this.tmrBlinkConnectionMenu_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 728);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Diogenes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbResourcePlaceholder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblConnectionType;
        private System.Windows.Forms.TreeView tvMain;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCBFFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unloadExistingFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbResourcePlaceholder;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eCUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setSecurityLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem j2534InterfacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultJ2534InterfaceToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtJ2534Input;
        private System.Windows.Forms.ToolStripMenuItem cFFExportFlashSegmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixClientAccessPermissionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cFFFlashSplicerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTraceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem clearConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Timer tmrBlinkConnectionMenu;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uDSHexEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fingerprintModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useLastFingerprintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sCNModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useLastSCNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem writeZerosVediamoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allowWriteVariantCodingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diagnosticTroubleCodesDTCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewECUMetadataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem identifyECUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCompressedJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportContainerAsCompressedJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem dSCDebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportContainerAsJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixStringsDebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listVariantIDsToolStripMenuItem;
    }
}

