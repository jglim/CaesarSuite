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
            this.tvMain = new System.Windows.Forms.TreeView();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.eCUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSecurityLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbResourcePlaceholder = new System.Windows.Forms.PictureBox();
            this.debugJ2534ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.j2534InterfacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultJ2534InterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.lblConnectionType.Size = new System.Drawing.Size(64, 17);
            this.lblConnectionType.Text = "Simulation";
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
            this.loadCBFFilesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadCBFFilesToolStripMenuItem.Text = "Load CBF Files";
            this.loadCBFFilesToolStripMenuItem.Click += new System.EventHandler(this.loadCBFFilesToolStripMenuItem_Click);
            // 
            // unloadExistingFilesToolStripMenuItem
            // 
            this.unloadExistingFilesToolStripMenuItem.Name = "unloadExistingFilesToolStripMenuItem";
            this.unloadExistingFilesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.unloadExistingFilesToolStripMenuItem.Text = "Unload Existing Files";
            this.unloadExistingFilesToolStripMenuItem.Click += new System.EventHandler(this.unloadExistingFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Enabled = false;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aboutToolStripMenuItem.Text = "About..";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
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
            // tvMain
            // 
            this.tvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMain.Location = new System.Drawing.Point(0, 27);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(1123, 514);
            this.tvMain.TabIndex = 2;
            this.tvMain.DoubleClick += new System.EventHandler(this.tvMain_DoubleClick);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(0, 541);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1123, 162);
            this.txtLog.TabIndex = 3;
            // 
            // eCUToolStripMenuItem
            // 
            this.eCUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setSecurityLevelToolStripMenuItem});
            this.eCUToolStripMenuItem.Name = "eCUToolStripMenuItem";
            this.eCUToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.eCUToolStripMenuItem.Text = "ECU";
            // 
            // setSecurityLevelToolStripMenuItem
            // 
            this.setSecurityLevelToolStripMenuItem.Name = "setSecurityLevelToolStripMenuItem";
            this.setSecurityLevelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.setSecurityLevelToolStripMenuItem.Text = "Set Security Level";
            this.setSecurityLevelToolStripMenuItem.Click += new System.EventHandler(this.setSecurityLevelToolStripMenuItem_Click);
            // 
            // pbResourcePlaceholder
            // 
            this.pbResourcePlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbResourcePlaceholder.Image = global::Diogenes.Properties.Resources.blank;
            this.pbResourcePlaceholder.Location = new System.Drawing.Point(1061, 39);
            this.pbResourcePlaceholder.Name = "pbResourcePlaceholder";
            this.pbResourcePlaceholder.Size = new System.Drawing.Size(50, 50);
            this.pbResourcePlaceholder.TabIndex = 4;
            this.pbResourcePlaceholder.TabStop = false;
            this.pbResourcePlaceholder.Visible = false;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 728);
            this.Controls.Add(this.pbResourcePlaceholder);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.tvMain);
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
    }
}

