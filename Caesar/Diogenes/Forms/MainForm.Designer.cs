
namespace Diogenes.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenCbf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUnloadCbf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExplorerHere = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslLoadedCbf = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslConnectionState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslVariant = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cbAutoVariantID = new System.Windows.Forms.CheckBox();
            this.btnConnectDisconnect = new System.Windows.Forms.Button();
            this.dgvPreConnectComParams = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.cbProtocol = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbComDevices = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.txtTrace = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabDiagServices = new System.Windows.Forms.TabPage();
            this.tabVarCoding = new System.Windows.Forms.TabPage();
            this.tabMemoryEditor = new System.Windows.Forms.TabPage();
            this.tabFlash = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtConsoleInput = new System.Windows.Forms.TextBox();
            this.txtConsoleLog = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreConnectComParams)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1424, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenCbf,
            this.tsmiOpenRecent,
            this.tsmiUnloadCbf,
            this.tsmiExplorerHere,
            this.toolStripSeparator1,
            this.tsmiExit});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // tsmiOpenCbf
            // 
            this.tsmiOpenCbf.Name = "tsmiOpenCbf";
            this.tsmiOpenCbf.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpenCbf.Size = new System.Drawing.Size(210, 22);
            this.tsmiOpenCbf.Text = "&Open CBF File";
            this.tsmiOpenCbf.Click += new System.EventHandler(this.tsmiOpenCbf_Click);
            // 
            // tsmiOpenRecent
            // 
            this.tsmiOpenRecent.Name = "tsmiOpenRecent";
            this.tsmiOpenRecent.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.tsmiOpenRecent.Size = new System.Drawing.Size(210, 22);
            this.tsmiOpenRecent.Text = "Open Recent..";
            // 
            // tsmiUnloadCbf
            // 
            this.tsmiUnloadCbf.Name = "tsmiUnloadCbf";
            this.tsmiUnloadCbf.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.U)));
            this.tsmiUnloadCbf.Size = new System.Drawing.Size(210, 22);
            this.tsmiUnloadCbf.Text = "Unload CBF";
            this.tsmiUnloadCbf.Click += new System.EventHandler(this.tsmiUnloadCbf_Click);
            // 
            // tsmiExplorerHere
            // 
            this.tsmiExplorerHere.Name = "tsmiExplorerHere";
            this.tsmiExplorerHere.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.tsmiExplorerHere.Size = new System.Drawing.Size(210, 22);
            this.tsmiExplorerHere.Text = "&Explorer Here";
            this.tsmiExplorerHere.Click += new System.EventHandler(this.tsmiExplorerHere_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(210, 22);
            this.tsmiExit.Text = "E&xit";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslLoadedCbf,
            this.tsslConnectionState,
            this.tsslVariant});
            this.statusStrip1.Location = new System.Drawing.Point(0, 830);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1424, 31);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslLoadedCbf
            // 
            this.tsslLoadedCbf.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tsslLoadedCbf.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.tsslLoadedCbf.Margin = new System.Windows.Forms.Padding(4, 4, 2, 4);
            this.tsslLoadedCbf.Name = "tsslLoadedCbf";
            this.tsslLoadedCbf.Padding = new System.Windows.Forms.Padding(4);
            this.tsslLoadedCbf.Size = new System.Drawing.Size(47, 23);
            this.tsslLoadedCbf.Text = "CBF: -";
            // 
            // tsslConnectionState
            // 
            this.tsslConnectionState.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tsslConnectionState.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.tsslConnectionState.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tsslConnectionState.Name = "tsslConnectionState";
            this.tsslConnectionState.Padding = new System.Windows.Forms.Padding(4);
            this.tsslConnectionState.Size = new System.Drawing.Size(56, 23);
            this.tsslConnectionState.Text = "Offline-";
            // 
            // tsslVariant
            // 
            this.tsslVariant.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tsslVariant.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.tsslVariant.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.tsslVariant.Name = "tsslVariant";
            this.tsslVariant.Padding = new System.Windows.Forms.Padding(4);
            this.tsslVariant.Size = new System.Drawing.Size(75, 23);
            this.tsslVariant.Text = "No Variant-";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1424, 806);
            this.splitContainer1.SplitterDistance = 349;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(349, 806);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbAutoVariantID);
            this.tabPage3.Controls.Add(this.btnConnectDisconnect);
            this.tabPage3.Controls.Add(this.dgvPreConnectComParams);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.cbProtocol);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.cbComDevices);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(341, 778);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Connection";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbAutoVariantID
            // 
            this.cbAutoVariantID.AutoSize = true;
            this.cbAutoVariantID.Checked = true;
            this.cbAutoVariantID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoVariantID.Location = new System.Drawing.Point(8, 101);
            this.cbAutoVariantID.Name = "cbAutoVariantID";
            this.cbAutoVariantID.Size = new System.Drawing.Size(207, 19);
            this.cbAutoVariantID.TabIndex = 6;
            this.cbAutoVariantID.Text = "Automatically Identify ECU Variant";
            this.cbAutoVariantID.UseVisualStyleBackColor = true;
            // 
            // btnConnectDisconnect
            // 
            this.btnConnectDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnectDisconnect.Location = new System.Drawing.Point(8, 126);
            this.btnConnectDisconnect.Name = "btnConnectDisconnect";
            this.btnConnectDisconnect.Size = new System.Drawing.Size(323, 46);
            this.btnConnectDisconnect.TabIndex = 5;
            this.btnConnectDisconnect.Text = "Connect";
            this.btnConnectDisconnect.UseVisualStyleBackColor = true;
            this.btnConnectDisconnect.Click += new System.EventHandler(this.btnConnectDisconnect_Click);
            // 
            // dgvPreConnectComParams
            // 
            this.dgvPreConnectComParams.AllowUserToAddRows = false;
            this.dgvPreConnectComParams.AllowUserToDeleteRows = false;
            this.dgvPreConnectComParams.AllowUserToResizeRows = false;
            this.dgvPreConnectComParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreConnectComParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreConnectComParams.Location = new System.Drawing.Point(8, 178);
            this.dgvPreConnectComParams.Name = "dgvPreConnectComParams";
            this.dgvPreConnectComParams.RowHeadersVisible = false;
            this.dgvPreConnectComParams.RowTemplate.Height = 25;
            this.dgvPreConnectComParams.Size = new System.Drawing.Size(323, 596);
            this.dgvPreConnectComParams.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Interface:";
            // 
            // cbProtocol
            // 
            this.cbProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProtocol.FormattingEnabled = true;
            this.cbProtocol.Location = new System.Drawing.Point(8, 72);
            this.cbProtocol.Name = "cbProtocol";
            this.cbProtocol.Size = new System.Drawing.Size(323, 23);
            this.cbProtocol.TabIndex = 2;
            this.cbProtocol.SelectionChangeCommitted += new System.EventHandler(this.cbProtocol_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Device:";
            // 
            // cbComDevices
            // 
            this.cbComDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbComDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComDevices.FormattingEnabled = true;
            this.cbComDevices.Location = new System.Drawing.Point(8, 28);
            this.cbComDevices.Name = "cbComDevices";
            this.cbComDevices.Size = new System.Drawing.Size(323, 23);
            this.cbComDevices.TabIndex = 0;
            this.cbComDevices.SelectionChangeCommitted += new System.EventHandler(this.cbComDevices_SelectionChangeCommitted);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.txtTrace);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(341, 778);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Trace";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // txtTrace
            // 
            this.txtTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTrace.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTrace.Location = new System.Drawing.Point(3, 3);
            this.txtTrace.Multiline = true;
            this.txtTrace.Name = "txtTrace";
            this.txtTrace.ReadOnly = true;
            this.txtTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTrace.Size = new System.Drawing.Size(335, 771);
            this.txtTrace.TabIndex = 1;
            this.txtTrace.WordWrap = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(1071, 806);
            this.splitContainer2.SplitterDistance = 598;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabDiagServices);
            this.tabControl3.Controls.Add(this.tabVarCoding);
            this.tabControl3.Controls.Add(this.tabMemoryEditor);
            this.tabControl3.Controls.Add(this.tabFlash);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(1071, 598);
            this.tabControl3.TabIndex = 0;
            // 
            // tabDiagServices
            // 
            this.tabDiagServices.Location = new System.Drawing.Point(4, 24);
            this.tabDiagServices.Name = "tabDiagServices";
            this.tabDiagServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabDiagServices.Size = new System.Drawing.Size(1063, 570);
            this.tabDiagServices.TabIndex = 0;
            this.tabDiagServices.Text = "Diagnostic Services";
            this.tabDiagServices.UseVisualStyleBackColor = true;
            // 
            // tabVarCoding
            // 
            this.tabVarCoding.Location = new System.Drawing.Point(4, 24);
            this.tabVarCoding.Name = "tabVarCoding";
            this.tabVarCoding.Padding = new System.Windows.Forms.Padding(3);
            this.tabVarCoding.Size = new System.Drawing.Size(1063, 570);
            this.tabVarCoding.TabIndex = 1;
            this.tabVarCoding.Text = "Variant Coding";
            this.tabVarCoding.UseVisualStyleBackColor = true;
            // 
            // tabMemoryEditor
            // 
            this.tabMemoryEditor.Location = new System.Drawing.Point(4, 24);
            this.tabMemoryEditor.Name = "tabMemoryEditor";
            this.tabMemoryEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabMemoryEditor.Size = new System.Drawing.Size(1063, 570);
            this.tabMemoryEditor.TabIndex = 2;
            this.tabMemoryEditor.Text = "Memory Editor";
            this.tabMemoryEditor.UseVisualStyleBackColor = true;
            // 
            // tabFlash
            // 
            this.tabFlash.Location = new System.Drawing.Point(4, 24);
            this.tabFlash.Name = "tabFlash";
            this.tabFlash.Padding = new System.Windows.Forms.Padding(3);
            this.tabFlash.Size = new System.Drawing.Size(1063, 570);
            this.tabFlash.TabIndex = 3;
            this.tabFlash.Text = "Flash";
            this.tabFlash.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1071, 204);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtConsoleInput);
            this.tabPage1.Controls.Add(this.txtConsoleLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1063, 176);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Interactive Console";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtConsoleInput
            // 
            this.txtConsoleInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleInput.Location = new System.Drawing.Point(3, 149);
            this.txtConsoleInput.Name = "txtConsoleInput";
            this.txtConsoleInput.PlaceholderText = "J2534 Console : Enter hex values (01 23 CA FE) and press enter to send a raw J253" +
    "4 message";
            this.txtConsoleInput.Size = new System.Drawing.Size(1057, 23);
            this.txtConsoleInput.TabIndex = 1;
            this.txtConsoleInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleInput_KeyDown);
            // 
            // txtConsoleLog
            // 
            this.txtConsoleLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtConsoleLog.Location = new System.Drawing.Point(3, 3);
            this.txtConsoleLog.Multiline = true;
            this.txtConsoleLog.Name = "txtConsoleLog";
            this.txtConsoleLog.ReadOnly = true;
            this.txtConsoleLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsoleLog.Size = new System.Drawing.Size(1057, 140);
            this.txtConsoleLog.TabIndex = 0;
            this.txtConsoleLog.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1063, 176);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Interpreter Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1424, 861);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Diogenes II";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreConnectComParams)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenCbf;
        private System.Windows.Forms.ToolStripMenuItem tsmiExplorerHere;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbComDevices;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbProtocol;
        private System.Windows.Forms.DataGridView dgvPreConnectComParams;
        private System.Windows.Forms.Button btnConnectDisconnect;
        private System.Windows.Forms.TextBox txtConsoleInput;
        private System.Windows.Forms.TextBox txtConsoleLog;
        private System.Windows.Forms.TextBox txtTrace;
        private System.Windows.Forms.CheckBox cbAutoVariantID;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenRecent;
        private System.Windows.Forms.ToolStripMenuItem tsmiUnloadCbf;
        private System.Windows.Forms.ToolStripStatusLabel tsslLoadedCbf;
        private System.Windows.Forms.ToolStripStatusLabel tsslConnectionState;
        private System.Windows.Forms.ToolStripStatusLabel tsslVariant;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabDiagServices;
        private System.Windows.Forms.TabPage tabVarCoding;
        private System.Windows.Forms.TabPage tabMemoryEditor;
        private System.Windows.Forms.TabPage tabFlash;
    }
}

