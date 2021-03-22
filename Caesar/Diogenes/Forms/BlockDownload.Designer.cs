
namespace Diogenes
{
    partial class BlockDownload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlockDownload));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.gbInputOutput = new System.Windows.Forms.GroupBox();
            this.txtIOPrefixHex = new System.Windows.Forms.TextBox();
            this.lblDescCmdRead = new System.Windows.Forms.Label();
            this.nudPayloadWidth = new System.Windows.Forms.NumericUpDown();
            this.lblDescIOWidth = new System.Windows.Forms.Label();
            this.nudAddressWidth = new System.Windows.Forms.NumericUpDown();
            this.lblDescPrefix = new System.Windows.Forms.Label();
            this.gbOperation = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnRemoveBlocks = new System.Windows.Forms.Button();
            this.btnAddBlock = new System.Windows.Forms.Button();
            this.gbMemo = new System.Windows.Forms.GroupBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.gbInputOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPayloadWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).BeginInit();
            this.gbOperation.SuspendLayout();
            this.gbMemo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgvMain);
            this.groupBox1.Location = new System.Drawing.Point(282, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 426);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Flash Payload";
            // 
            // dgvMain
            // 
            this.dgvMain.AllowDrop = true;
            this.dgvMain.AllowUserToAddRows = false;
            this.dgvMain.AllowUserToDeleteRows = false;
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMain.Location = new System.Drawing.Point(3, 16);
            this.dgvMain.MultiSelect = false;
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMain.ShowEditingIcon = false;
            this.dgvMain.Size = new System.Drawing.Size(500, 407);
            this.dgvMain.TabIndex = 1;
            this.dgvMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvMain_DragDrop);
            this.dgvMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvMain_DragEnter);
            // 
            // gbInputOutput
            // 
            this.gbInputOutput.Controls.Add(this.txtIOPrefixHex);
            this.gbInputOutput.Controls.Add(this.lblDescCmdRead);
            this.gbInputOutput.Controls.Add(this.nudPayloadWidth);
            this.gbInputOutput.Controls.Add(this.lblDescIOWidth);
            this.gbInputOutput.Controls.Add(this.nudAddressWidth);
            this.gbInputOutput.Controls.Add(this.lblDescPrefix);
            this.gbInputOutput.Location = new System.Drawing.Point(12, 12);
            this.gbInputOutput.Name = "gbInputOutput";
            this.gbInputOutput.Size = new System.Drawing.Size(264, 214);
            this.gbInputOutput.TabIndex = 8;
            this.gbInputOutput.TabStop = false;
            this.gbInputOutput.Text = "I/O";
            // 
            // txtIOPrefixHex
            // 
            this.txtIOPrefixHex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIOPrefixHex.Font = new System.Drawing.Font("Consolas", 16F);
            this.txtIOPrefixHex.Location = new System.Drawing.Point(6, 37);
            this.txtIOPrefixHex.Name = "txtIOPrefixHex";
            this.txtIOPrefixHex.Size = new System.Drawing.Size(250, 32);
            this.txtIOPrefixHex.TabIndex = 10;
            this.txtIOPrefixHex.Text = "34 00";
            // 
            // lblDescCmdRead
            // 
            this.lblDescCmdRead.AutoSize = true;
            this.lblDescCmdRead.Location = new System.Drawing.Point(5, 149);
            this.lblDescCmdRead.Name = "lblDescCmdRead";
            this.lblDescCmdRead.Size = new System.Drawing.Size(76, 13);
            this.lblDescCmdRead.TabIndex = 7;
            this.lblDescCmdRead.Text = "Payload Width";
            // 
            // nudPayloadWidth
            // 
            this.nudPayloadWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPayloadWidth.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudPayloadWidth.Hexadecimal = true;
            this.nudPayloadWidth.Location = new System.Drawing.Point(6, 168);
            this.nudPayloadWidth.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudPayloadWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPayloadWidth.Name = "nudPayloadWidth";
            this.nudPayloadWidth.Size = new System.Drawing.Size(250, 32);
            this.nudPayloadWidth.TabIndex = 6;
            this.nudPayloadWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lblDescIOWidth
            // 
            this.lblDescIOWidth.AutoSize = true;
            this.lblDescIOWidth.Location = new System.Drawing.Point(5, 85);
            this.lblDescIOWidth.Name = "lblDescIOWidth";
            this.lblDescIOWidth.Size = new System.Drawing.Size(76, 13);
            this.lblDescIOWidth.TabIndex = 5;
            this.lblDescIOWidth.Text = "Address Width";
            // 
            // nudAddressWidth
            // 
            this.nudAddressWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAddressWidth.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudAddressWidth.Hexadecimal = true;
            this.nudAddressWidth.Location = new System.Drawing.Point(6, 104);
            this.nudAddressWidth.Maximum = new decimal(new int[] {
            4,
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
            this.nudAddressWidth.TabIndex = 4;
            this.nudAddressWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // lblDescPrefix
            // 
            this.lblDescPrefix.AutoSize = true;
            this.lblDescPrefix.Location = new System.Drawing.Point(6, 21);
            this.lblDescPrefix.Name = "lblDescPrefix";
            this.lblDescPrefix.Size = new System.Drawing.Size(61, 13);
            this.lblDescPrefix.TabIndex = 3;
            this.lblDescPrefix.Text = "Prefix (Hex)";
            // 
            // gbOperation
            // 
            this.gbOperation.Controls.Add(this.btnDownload);
            this.gbOperation.Controls.Add(this.btnRemoveBlocks);
            this.gbOperation.Controls.Add(this.btnAddBlock);
            this.gbOperation.Location = new System.Drawing.Point(12, 232);
            this.gbOperation.Name = "gbOperation";
            this.gbOperation.Size = new System.Drawing.Size(264, 112);
            this.gbOperation.TabIndex = 9;
            this.gbOperation.TabStop = false;
            this.gbOperation.Text = "Operation";
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Location = new System.Drawing.Point(8, 77);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(250, 23);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnRemoveBlocks
            // 
            this.btnRemoveBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBlocks.Location = new System.Drawing.Point(8, 48);
            this.btnRemoveBlocks.Name = "btnRemoveBlocks";
            this.btnRemoveBlocks.Size = new System.Drawing.Size(250, 23);
            this.btnRemoveBlocks.TabIndex = 1;
            this.btnRemoveBlocks.Text = "Clear All Blocks";
            this.btnRemoveBlocks.UseVisualStyleBackColor = true;
            this.btnRemoveBlocks.Click += new System.EventHandler(this.btnRemoveBlocks_Click);
            // 
            // btnAddBlock
            // 
            this.btnAddBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBlock.Location = new System.Drawing.Point(8, 19);
            this.btnAddBlock.Name = "btnAddBlock";
            this.btnAddBlock.Size = new System.Drawing.Size(250, 23);
            this.btnAddBlock.TabIndex = 0;
            this.btnAddBlock.Text = "Add Block";
            this.btnAddBlock.UseVisualStyleBackColor = true;
            this.btnAddBlock.Click += new System.EventHandler(this.btnAddBlock_Click);
            // 
            // gbMemo
            // 
            this.gbMemo.Controls.Add(this.lblWarning);
            this.gbMemo.Location = new System.Drawing.Point(12, 350);
            this.gbMemo.Name = "gbMemo";
            this.gbMemo.Size = new System.Drawing.Size(264, 88);
            this.gbMemo.TabIndex = 10;
            this.gbMemo.TabStop = false;
            this.gbMemo.Text = "Warning";
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Location = new System.Drawing.Point(6, 16);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(252, 65);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "This feature is intended for developers only.\r\nThe target must be in a state that" +
    " is ready to receive\r\nflash blocks to use this feature.\r\n\r\nPlease check the docs" +
    " for more information";
            // 
            // BlockDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gbMemo);
            this.Controls.Add(this.gbOperation);
            this.Controls.Add(this.gbInputOutput);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BlockDownload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Block Download";
            this.Load += new System.EventHandler(this.BlockDownload_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.gbInputOutput.ResumeLayout(false);
            this.gbInputOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPayloadWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAddressWidth)).EndInit();
            this.gbOperation.ResumeLayout(false);
            this.gbMemo.ResumeLayout(false);
            this.gbMemo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.GroupBox gbInputOutput;
        private System.Windows.Forms.Label lblDescCmdRead;
        private System.Windows.Forms.NumericUpDown nudPayloadWidth;
        private System.Windows.Forms.Label lblDescIOWidth;
        private System.Windows.Forms.NumericUpDown nudAddressWidth;
        private System.Windows.Forms.Label lblDescPrefix;
        private System.Windows.Forms.TextBox txtIOPrefixHex;
        private System.Windows.Forms.GroupBox gbOperation;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnRemoveBlocks;
        private System.Windows.Forms.Button btnAddBlock;
        private System.Windows.Forms.GroupBox gbMemo;
        private System.Windows.Forms.Label lblWarning;
    }
}