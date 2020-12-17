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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnectionType = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCBFFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadExistingFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugJ2534ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.j2534InterfacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultJ2534InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eCUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSecurityLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cFFFlashSplicerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cFFExportFlashSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixClientAccessPermissionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvMain = new System.Windows.Forms.TreeView();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pbResourcePlaceholder = new System.Windows.Forms.PictureBox();
            this.txtJ2534Input = new System.Windows.Forms.TextBox();
            this.allowWriteVariantCodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.eCUToolStripMenuItem});
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
            this.allowWriteVariantCodingToolStripMenuItem,
            this.preferencesToolStripMenuItem,
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
            this.loadCBFFilesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.loadCBFFilesToolStripMenuItem.Text = "Load CBF Files";
            this.loadCBFFilesToolStripMenuItem.Click += new System.EventHandler(this.loadCBFFilesToolStripMenuItem_Click);
            // 
            // unloadExistingFilesToolStripMenuItem
            // 
            this.unloadExistingFilesToolStripMenuItem.Name = "unloadExistingFilesToolStripMenuItem";
            this.unloadExistingFilesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.unloadExistingFilesToolStripMenuItem.Text = "Unload Existing Files";
            this.unloadExistingFilesToolStripMenuItem.Click += new System.EventHandler(this.unloadExistingFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Enabled = false;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.aboutToolStripMenuItem.Text = "About..";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(213, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugJ2534ToolStripMenuItem,
            this.j2534InterfacesToolStripMenuItem});
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.connectionToolStripMenuItem.Text = "Connection";
            // 
            // debugJ2534ToolStripMenuItem
            // 
            this.debugJ2534ToolStripMenuItem.Name = "debugJ2534ToolStripMenuItem";
            this.debugJ2534ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.debugJ2534ToolStripMenuItem.Text = "Debug: J2534";
            this.debugJ2534ToolStripMenuItem.Visible = false;
            this.debugJ2534ToolStripMenuItem.Click += new System.EventHandler(this.debugJ2534ToolStripMenuItem_Click);
            // 
            // j2534InterfacesToolStripMenuItem
            // 
            this.j2534InterfacesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultJ2534InterfaceToolStripMenuItem});
            this.j2534InterfacesToolStripMenuItem.Name = "j2534InterfacesToolStripMenuItem";
            this.j2534InterfacesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.j2534InterfacesToolStripMenuItem.Text = "J2534 Interfaces";
            this.j2534InterfacesToolStripMenuItem.DropDownOpening += new System.EventHandler(this.j2534InterfacesToolStripMenuItem_DropDownOpening);
            // 
            // defaultJ2534InterfaceToolStripMenuItem
            // 
            this.defaultJ2534InterfaceToolStripMenuItem.Name = "defaultJ2534InterfaceToolStripMenuItem";
            this.defaultJ2534InterfaceToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.defaultJ2534InterfaceToolStripMenuItem.Text = "Default J2534 Interface";
            // 
            // eCUToolStripMenuItem
            // 
            this.eCUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setSecurityLevelToolStripMenuItem,
            this.cFFFlashSplicerToolStripMenuItem,
            this.cFFExportFlashSegmentsToolStripMenuItem,
            this.fixClientAccessPermissionsToolStripMenuItem,
            this.showTraceToolStripMenuItem});
            this.eCUToolStripMenuItem.Name = "eCUToolStripMenuItem";
            this.eCUToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.eCUToolStripMenuItem.Text = "Tools";
            // 
            // setSecurityLevelToolStripMenuItem
            // 
            this.setSecurityLevelToolStripMenuItem.Name = "setSecurityLevelToolStripMenuItem";
            this.setSecurityLevelToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.setSecurityLevelToolStripMenuItem.Text = "Set Security Level (DLL)";
            this.setSecurityLevelToolStripMenuItem.Click += new System.EventHandler(this.setSecurityLevelToolStripMenuItem_Click);
            // 
            // cFFFlashSplicerToolStripMenuItem
            // 
            this.cFFFlashSplicerToolStripMenuItem.Name = "cFFFlashSplicerToolStripMenuItem";
            this.cFFFlashSplicerToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.cFFFlashSplicerToolStripMenuItem.Text = "CFF: Flash Splicer";
            this.cFFFlashSplicerToolStripMenuItem.Click += new System.EventHandler(this.cFFFlashSplicerToolStripMenuItem_Click);
            // 
            // cFFExportFlashSegmentsToolStripMenuItem
            // 
            this.cFFExportFlashSegmentsToolStripMenuItem.Name = "cFFExportFlashSegmentsToolStripMenuItem";
            this.cFFExportFlashSegmentsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.cFFExportFlashSegmentsToolStripMenuItem.Text = "CFF: Export Flash Segments";
            this.cFFExportFlashSegmentsToolStripMenuItem.Click += new System.EventHandler(this.cFFExportFlashSegmentsToolStripMenuItem_Click);
            // 
            // fixClientAccessPermissionsToolStripMenuItem
            // 
            this.fixClientAccessPermissionsToolStripMenuItem.Name = "fixClientAccessPermissionsToolStripMenuItem";
            this.fixClientAccessPermissionsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.fixClientAccessPermissionsToolStripMenuItem.Text = "Fix Client Access Permissions";
            this.fixClientAccessPermissionsToolStripMenuItem.Click += new System.EventHandler(this.fixClientAccessPermissionsToolStripMenuItem_Click);
            // 
            // tvMain
            // 
            this.tvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMain.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvMain.Location = new System.Drawing.Point(3, 3);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(1117, 480);
            this.tvMain.TabIndex = 2;
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
            this.pbResourcePlaceholder.Image = global::Diogenes.Properties.Resources.accept;
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
            // allowWriteVariantCodingToolStripMenuItem
            // 
            this.allowWriteVariantCodingToolStripMenuItem.Name = "allowWriteVariantCodingToolStripMenuItem";
            this.allowWriteVariantCodingToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.allowWriteVariantCodingToolStripMenuItem.Text = "Allow Write Variant Coding";
            this.allowWriteVariantCodingToolStripMenuItem.Click += new System.EventHandler(this.allowWriteVariantCodingToolStripMenuItem_Click);
            // 
            // showTraceToolStripMenuItem
            // 
            this.showTraceToolStripMenuItem.Enabled = false;
            this.showTraceToolStripMenuItem.Name = "showTraceToolStripMenuItem";
            this.showTraceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.showTraceToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.showTraceToolStripMenuItem.Text = "Show Trace";
            this.showTraceToolStripMenuItem.Click += new System.EventHandler(this.showTraceToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbResourcePlaceholder;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eCUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setSecurityLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugJ2534ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem j2534InterfacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultJ2534InterfaceToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtJ2534Input;
        private System.Windows.Forms.ToolStripMenuItem cFFExportFlashSegmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixClientAccessPermissionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cFFFlashSplicerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allowWriteVariantCodingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTraceToolStripMenuItem;
    }
}

