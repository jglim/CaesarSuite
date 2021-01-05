
namespace Diogenes
{
    partial class UDSHexEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDSHexEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbInputOutput = new System.Windows.Forms.GroupBox();
            this.lblDescCmdWrite = new System.Windows.Forms.Label();
            this.nudWriteCmd = new System.Windows.Forms.NumericUpDown();
            this.lblDescCmdRead = new System.Windows.Forms.Label();
            this.nudReadCmd = new System.Windows.Forms.NumericUpDown();
            this.lblDescIOWidth = new System.Windows.Forms.Label();
            this.nudIOWidth = new System.Windows.Forms.NumericUpDown();
            this.lblDescWidth = new System.Windows.Forms.Label();
            this.nudAddressWidth = new System.Windows.Forms.NumericUpDown();
            this.gbOperation = new System.Windows.Forms.GroupBox();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.gbDestination = new System.Windows.Forms.GroupBox();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.rbSize = new System.Windows.Forms.RadioButton();
            this.rbAddress = new System.Windows.Forms.RadioButton();
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.txtSource = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbInputOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWriteCmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReadCmd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIOWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).BeginInit();
            this.gbOperation.SuspendLayout();
            this.gbDestination.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbInputOutput);
            this.splitContainer1.Panel1.Controls.Add(this.gbOperation);
            this.splitContainer1.Panel1.Controls.Add(this.gbDestination);
            this.splitContainer1.Panel1.Controls.Add(this.gbSource);
            this.splitContainer1.Size = new System.Drawing.Size(929, 593);
            this.splitContainer1.SplitterDistance = 285;
            this.splitContainer1.TabIndex = 0;
            // 
            // gbInputOutput
            // 
            this.gbInputOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInputOutput.Controls.Add(this.lblDescCmdWrite);
            this.gbInputOutput.Controls.Add(this.nudWriteCmd);
            this.gbInputOutput.Controls.Add(this.lblDescCmdRead);
            this.gbInputOutput.Controls.Add(this.nudReadCmd);
            this.gbInputOutput.Controls.Add(this.lblDescIOWidth);
            this.gbInputOutput.Controls.Add(this.nudIOWidth);
            this.gbInputOutput.Controls.Add(this.lblDescWidth);
            this.gbInputOutput.Controls.Add(this.nudAddressWidth);
            this.gbInputOutput.Location = new System.Drawing.Point(12, 289);
            this.gbInputOutput.Name = "gbInputOutput";
            this.gbInputOutput.Size = new System.Drawing.Size(264, 274);
            this.gbInputOutput.TabIndex = 7;
            this.gbInputOutput.TabStop = false;
            this.gbInputOutput.Text = "I/O";
            // 
            // lblDescCmdWrite
            // 
            this.lblDescCmdWrite.AutoSize = true;
            this.lblDescCmdWrite.Location = new System.Drawing.Point(5, 213);
            this.lblDescCmdWrite.Name = "lblDescCmdWrite";
            this.lblDescCmdWrite.Size = new System.Drawing.Size(145, 13);
            this.lblDescCmdWrite.TabIndex = 9;
            this.lblDescCmdWrite.Text = "WriteMemoryByAddress (hex)";
            // 
            // nudWriteCmd
            // 
            this.nudWriteCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudWriteCmd.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudWriteCmd.Hexadecimal = true;
            this.nudWriteCmd.Location = new System.Drawing.Point(6, 232);
            this.nudWriteCmd.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudWriteCmd.Name = "nudWriteCmd";
            this.nudWriteCmd.Size = new System.Drawing.Size(250, 32);
            this.nudWriteCmd.TabIndex = 8;
            this.nudWriteCmd.Value = new decimal(new int[] {
            61,
            0,
            0,
            0});
            // 
            // lblDescCmdRead
            // 
            this.lblDescCmdRead.AutoSize = true;
            this.lblDescCmdRead.Location = new System.Drawing.Point(5, 149);
            this.lblDescCmdRead.Name = "lblDescCmdRead";
            this.lblDescCmdRead.Size = new System.Drawing.Size(148, 13);
            this.lblDescCmdRead.TabIndex = 7;
            this.lblDescCmdRead.Text = "ReadMemoryByAddress (Hex)";
            // 
            // nudReadCmd
            // 
            this.nudReadCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudReadCmd.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudReadCmd.Hexadecimal = true;
            this.nudReadCmd.Location = new System.Drawing.Point(6, 168);
            this.nudReadCmd.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudReadCmd.Name = "nudReadCmd";
            this.nudReadCmd.Size = new System.Drawing.Size(250, 32);
            this.nudReadCmd.TabIndex = 6;
            this.nudReadCmd.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            // 
            // lblDescIOWidth
            // 
            this.lblDescIOWidth.AutoSize = true;
            this.lblDescIOWidth.Location = new System.Drawing.Point(5, 85);
            this.lblDescIOWidth.Name = "lblDescIOWidth";
            this.lblDescIOWidth.Size = new System.Drawing.Size(82, 13);
            this.lblDescIOWidth.TabIndex = 5;
            this.lblDescIOWidth.Text = "I/O Width (Hex)";
            // 
            // nudIOWidth
            // 
            this.nudIOWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudIOWidth.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudIOWidth.Hexadecimal = true;
            this.nudIOWidth.Location = new System.Drawing.Point(6, 104);
            this.nudIOWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudIOWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudIOWidth.Name = "nudIOWidth";
            this.nudIOWidth.Size = new System.Drawing.Size(250, 32);
            this.nudIOWidth.TabIndex = 4;
            this.nudIOWidth.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // lblDescWidth
            // 
            this.lblDescWidth.AutoSize = true;
            this.lblDescWidth.Location = new System.Drawing.Point(6, 21);
            this.lblDescWidth.Name = "lblDescWidth";
            this.lblDescWidth.Size = new System.Drawing.Size(76, 13);
            this.lblDescWidth.TabIndex = 3;
            this.lblDescWidth.Text = "Address Width";
            // 
            // nudAddressWidth
            // 
            this.nudAddressWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAddressWidth.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudAddressWidth.Hexadecimal = true;
            this.nudAddressWidth.Location = new System.Drawing.Point(8, 40);
            this.nudAddressWidth.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudAddressWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAddressWidth.Name = "nudAddressWidth";
            this.nudAddressWidth.Size = new System.Drawing.Size(250, 32);
            this.nudAddressWidth.TabIndex = 2;
            this.nudAddressWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // gbOperation
            // 
            this.gbOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOperation.Controls.Add(this.btnSaveToFile);
            this.gbOperation.Controls.Add(this.btnWrite);
            this.gbOperation.Controls.Add(this.btnRead);
            this.gbOperation.Location = new System.Drawing.Point(12, 171);
            this.gbOperation.Name = "gbOperation";
            this.gbOperation.Size = new System.Drawing.Size(264, 112);
            this.gbOperation.TabIndex = 6;
            this.gbOperation.TabStop = false;
            this.gbOperation.Text = "Operation";
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveToFile.Location = new System.Drawing.Point(8, 77);
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(250, 23);
            this.btnSaveToFile.TabIndex = 2;
            this.btnSaveToFile.Text = "Save Displayed Data";
            this.btnSaveToFile.UseVisualStyleBackColor = true;
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWrite.Location = new System.Drawing.Point(8, 48);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(250, 23);
            this.btnWrite.TabIndex = 1;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRead.Location = new System.Drawing.Point(8, 19);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(250, 23);
            this.btnRead.TabIndex = 0;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // gbDestination
            // 
            this.gbDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDestination.Controls.Add(this.txtDestination);
            this.gbDestination.Controls.Add(this.rbSize);
            this.gbDestination.Controls.Add(this.rbAddress);
            this.gbDestination.Location = new System.Drawing.Point(12, 78);
            this.gbDestination.Name = "gbDestination";
            this.gbDestination.Size = new System.Drawing.Size(264, 87);
            this.gbDestination.TabIndex = 3;
            this.gbDestination.TabStop = false;
            this.gbDestination.Text = "Destination (Hex)";
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Font = new System.Drawing.Font("Consolas", 16F);
            this.txtDestination.Location = new System.Drawing.Point(8, 19);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(250, 32);
            this.txtDestination.TabIndex = 3;
            this.txtDestination.Text = "0";
            // 
            // rbSize
            // 
            this.rbSize.AutoSize = true;
            this.rbSize.Location = new System.Drawing.Point(77, 57);
            this.rbSize.Name = "rbSize";
            this.rbSize.Size = new System.Drawing.Size(45, 17);
            this.rbSize.TabIndex = 5;
            this.rbSize.Text = "Size";
            this.rbSize.UseVisualStyleBackColor = true;
            // 
            // rbAddress
            // 
            this.rbAddress.AutoSize = true;
            this.rbAddress.Checked = true;
            this.rbAddress.Location = new System.Drawing.Point(8, 57);
            this.rbAddress.Name = "rbAddress";
            this.rbAddress.Size = new System.Drawing.Size(63, 17);
            this.rbAddress.TabIndex = 4;
            this.rbAddress.TabStop = true;
            this.rbAddress.Text = "Address";
            this.rbAddress.UseVisualStyleBackColor = true;
            // 
            // gbSource
            // 
            this.gbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSource.Controls.Add(this.txtSource);
            this.gbSource.Location = new System.Drawing.Point(12, 12);
            this.gbSource.Name = "gbSource";
            this.gbSource.Size = new System.Drawing.Size(264, 60);
            this.gbSource.TabIndex = 2;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "Source Address (Hex)";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Font = new System.Drawing.Font("Consolas", 16F);
            this.txtSource.Location = new System.Drawing.Point(8, 19);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(250, 32);
            this.txtSource.TabIndex = 2;
            this.txtSource.Text = "0";
            // 
            // UDSHexEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 593);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UDSHexEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDS Hex Editor";
            this.Load += new System.EventHandler(this.UDSHexEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbInputOutput.ResumeLayout(false);
            this.gbInputOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWriteCmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReadCmd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIOWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).EndInit();
            this.gbOperation.ResumeLayout(false);
            this.gbDestination.ResumeLayout(false);
            this.gbDestination.PerformLayout();
            this.gbSource.ResumeLayout(false);
            this.gbSource.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbOperation;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.GroupBox gbDestination;
        private System.Windows.Forms.RadioButton rbSize;
        private System.Windows.Forms.RadioButton rbAddress;
        private System.Windows.Forms.GroupBox gbSource;
        private System.Windows.Forms.GroupBox gbInputOutput;
        private System.Windows.Forms.Label lblDescWidth;
        private System.Windows.Forms.NumericUpDown nudAddressWidth;
        private System.Windows.Forms.Label lblDescCmdWrite;
        private System.Windows.Forms.NumericUpDown nudWriteCmd;
        private System.Windows.Forms.Label lblDescCmdRead;
        private System.Windows.Forms.NumericUpDown nudReadCmd;
        private System.Windows.Forms.Label lblDescIOWidth;
        private System.Windows.Forms.NumericUpDown nudIOWidth;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnSaveToFile;
    }
}