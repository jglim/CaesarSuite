
namespace Diogenes.Forms
{
    partial class MemoryEditorView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSrcAddress = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnFileLoad = new System.Windows.Forms.Button();
            this.btnFileSave = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnEcuWrite = new System.Windows.Forms.Button();
            this.btnEcuRead = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkUseALFID = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nudWriteRequest = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudReadRequest = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudDataWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudAddressWidth = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbSize = new System.Windows.Forms.RadioButton();
            this.rbAddress = new System.Windows.Forms.RadioButton();
            this.txtDestAddress = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWriteRequest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReadRequest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtSrcAddress);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Address (Hex)";
            // 
            // txtSrcAddress
            // 
            this.txtSrcAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSrcAddress.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSrcAddress.Location = new System.Drawing.Point(6, 22);
            this.txtSrcAddress.Name = "txtSrcAddress";
            this.txtSrcAddress.Size = new System.Drawing.Size(232, 22);
            this.txtSrcAddress.TabIndex = 0;
            this.txtSrcAddress.Text = "0";
            this.txtSrcAddress.TextChanged += new System.EventHandler(this.txtSrcAddress_TextChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox6);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox5);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(827, 624);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.lblProgress);
            this.groupBox6.Controls.Add(this.progressBar1);
            this.groupBox6.Location = new System.Drawing.Point(3, 484);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(244, 56);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Status";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(8, 19);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(12, 15);
            this.lblProgress.TabIndex = 3;
            this.lblProgress.Text = "-";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(6, 37);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(232, 10);
            this.progressBar1.TabIndex = 2;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.btnFileLoad);
            this.groupBox5.Controls.Add(this.btnFileSave);
            this.groupBox5.Location = new System.Drawing.Point(3, 396);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(244, 82);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "File Tasks";
            // 
            // btnFileLoad
            // 
            this.btnFileLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileLoad.Location = new System.Drawing.Point(6, 51);
            this.btnFileLoad.Name = "btnFileLoad";
            this.btnFileLoad.Size = new System.Drawing.Size(232, 23);
            this.btnFileLoad.TabIndex = 1;
            this.btnFileLoad.Text = "Load file into current view";
            this.btnFileLoad.UseVisualStyleBackColor = true;
            this.btnFileLoad.Click += new System.EventHandler(this.btnFileLoad_Click);
            // 
            // btnFileSave
            // 
            this.btnFileSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileSave.Location = new System.Drawing.Point(6, 22);
            this.btnFileSave.Name = "btnFileSave";
            this.btnFileSave.Size = new System.Drawing.Size(232, 23);
            this.btnFileSave.TabIndex = 0;
            this.btnFileSave.Text = "Save current view to file";
            this.btnFileSave.UseVisualStyleBackColor = true;
            this.btnFileSave.Click += new System.EventHandler(this.btnFileSave_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnEcuWrite);
            this.groupBox4.Controls.Add(this.btnEcuRead);
            this.groupBox4.Location = new System.Drawing.Point(3, 308);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(244, 82);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ECU Tasks";
            // 
            // btnEcuWrite
            // 
            this.btnEcuWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEcuWrite.Location = new System.Drawing.Point(6, 51);
            this.btnEcuWrite.Name = "btnEcuWrite";
            this.btnEcuWrite.Size = new System.Drawing.Size(232, 23);
            this.btnEcuWrite.TabIndex = 1;
            this.btnEcuWrite.Text = "Write to ECU";
            this.btnEcuWrite.UseVisualStyleBackColor = true;
            this.btnEcuWrite.Click += new System.EventHandler(this.btnEcuWrite_Click);
            // 
            // btnEcuRead
            // 
            this.btnEcuRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEcuRead.Location = new System.Drawing.Point(6, 22);
            this.btnEcuRead.Name = "btnEcuRead";
            this.btnEcuRead.Size = new System.Drawing.Size(232, 23);
            this.btnEcuRead.TabIndex = 0;
            this.btnEcuRead.Text = "Read from ECU";
            this.toolTip1.SetToolTip(this.btnEcuRead, "Read from ECU");
            this.btnEcuRead.UseVisualStyleBackColor = true;
            this.btnEcuRead.Click += new System.EventHandler(this.btnEcuRead_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkUseALFID);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.nudWriteRequest);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.nudReadRequest);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.nudDataWidth);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.nudAddressWidth);
            this.groupBox3.Location = new System.Drawing.Point(3, 143);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(244, 159);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "I/O Configuration";
            // 
            // chkUseALFID
            // 
            this.chkUseALFID.AutoSize = true;
            this.chkUseALFID.Checked = true;
            this.chkUseALFID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseALFID.Location = new System.Drawing.Point(6, 134);
            this.chkUseALFID.Name = "chkUseALFID";
            this.chkUseALFID.Size = new System.Drawing.Size(198, 19);
            this.chkUseALFID.TabIndex = 8;
            this.chkUseALFID.Text = "Use AddressAndLengthFormatID";
            this.chkUseALFID.UseVisualStyleBackColor = true;
            this.chkUseALFID.CheckedChanged += new System.EventHandler(this.chkUseALFID_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Write Request (Hex)";
            // 
            // nudWriteRequest
            // 
            this.nudWriteRequest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudWriteRequest.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudWriteRequest.Hexadecimal = true;
            this.nudWriteRequest.Location = new System.Drawing.Point(138, 106);
            this.nudWriteRequest.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudWriteRequest.Name = "nudWriteRequest";
            this.nudWriteRequest.Size = new System.Drawing.Size(100, 22);
            this.nudWriteRequest.TabIndex = 6;
            this.nudWriteRequest.Value = new decimal(new int[] {
            61,
            0,
            0,
            0});
            this.nudWriteRequest.ValueChanged += new System.EventHandler(this.nudPreview_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Read Request (Hex)";
            // 
            // nudReadRequest
            // 
            this.nudReadRequest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudReadRequest.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudReadRequest.Hexadecimal = true;
            this.nudReadRequest.Location = new System.Drawing.Point(138, 78);
            this.nudReadRequest.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudReadRequest.Name = "nudReadRequest";
            this.nudReadRequest.Size = new System.Drawing.Size(100, 22);
            this.nudReadRequest.TabIndex = 4;
            this.nudReadRequest.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudReadRequest.ValueChanged += new System.EventHandler(this.nudPreview_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data Width (Hex)";
            // 
            // nudDataWidth
            // 
            this.nudDataWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudDataWidth.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudDataWidth.Hexadecimal = true;
            this.nudDataWidth.Location = new System.Drawing.Point(138, 50);
            this.nudDataWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDataWidth.Name = "nudDataWidth";
            this.nudDataWidth.Size = new System.Drawing.Size(100, 22);
            this.nudDataWidth.TabIndex = 2;
            this.nudDataWidth.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudDataWidth.ValueChanged += new System.EventHandler(this.nudPreview_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Address Width (Hex)";
            // 
            // nudAddressWidth
            // 
            this.nudAddressWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAddressWidth.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudAddressWidth.Hexadecimal = true;
            this.nudAddressWidth.Location = new System.Drawing.Point(138, 22);
            this.nudAddressWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAddressWidth.Name = "nudAddressWidth";
            this.nudAddressWidth.Size = new System.Drawing.Size(100, 22);
            this.nudAddressWidth.TabIndex = 0;
            this.nudAddressWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudAddressWidth.ValueChanged += new System.EventHandler(this.nudPreview_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rbSize);
            this.groupBox2.Controls.Add(this.rbAddress);
            this.groupBox2.Controls.Add(this.txtDestAddress);
            this.groupBox2.Location = new System.Drawing.Point(3, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 76);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination (Hex)";
            // 
            // rbSize
            // 
            this.rbSize.AutoSize = true;
            this.rbSize.Location = new System.Drawing.Point(79, 50);
            this.rbSize.Name = "rbSize";
            this.rbSize.Size = new System.Drawing.Size(45, 19);
            this.rbSize.TabIndex = 2;
            this.rbSize.Text = "Size";
            this.rbSize.UseVisualStyleBackColor = true;
            // 
            // rbAddress
            // 
            this.rbAddress.AutoSize = true;
            this.rbAddress.Checked = true;
            this.rbAddress.Location = new System.Drawing.Point(6, 50);
            this.rbAddress.Name = "rbAddress";
            this.rbAddress.Size = new System.Drawing.Size(67, 19);
            this.rbAddress.TabIndex = 1;
            this.rbAddress.TabStop = true;
            this.rbAddress.Text = "Address";
            this.rbAddress.UseVisualStyleBackColor = true;
            // 
            // txtDestAddress
            // 
            this.txtDestAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestAddress.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtDestAddress.Location = new System.Drawing.Point(6, 22);
            this.txtDestAddress.Name = "txtDestAddress";
            this.txtDestAddress.Size = new System.Drawing.Size(232, 22);
            this.txtDestAddress.TabIndex = 0;
            this.txtDestAddress.Text = "0";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 1;
            this.toolTip1.AutoPopDelay = 30000;
            this.toolTip1.InitialDelay = 1;
            this.toolTip1.ReshowDelay = 0;
            // 
            // MemoryEditorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MemoryEditorView";
            this.Size = new System.Drawing.Size(827, 624);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWriteRequest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReadRequest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtSrcAddress;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDestAddress;
        private System.Windows.Forms.RadioButton rbSize;
        private System.Windows.Forms.RadioButton rbAddress;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkUseALFID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudWriteRequest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudReadRequest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudDataWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudAddressWidth;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnEcuRead;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnFileLoad;
        private System.Windows.Forms.Button btnFileSave;
        private System.Windows.Forms.Button btnEcuWrite;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblProgress;
    }
}
