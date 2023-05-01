
namespace Diogenes.Forms
{
    partial class VarcodingView
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
            this.dgvVcFragments = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.cbVcdPicker = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExecuteRequest = new System.Windows.Forms.Button();
            this.txtRequestPreview = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVcFragments)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvVcFragments
            // 
            this.dgvVcFragments.AllowUserToAddRows = false;
            this.dgvVcFragments.AllowUserToDeleteRows = false;
            this.dgvVcFragments.AllowUserToResizeRows = false;
            this.dgvVcFragments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVcFragments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVcFragments.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvVcFragments.Location = new System.Drawing.Point(6, 51);
            this.dgvVcFragments.MultiSelect = false;
            this.dgvVcFragments.Name = "dgvVcFragments";
            this.dgvVcFragments.RowHeadersVisible = false;
            this.dgvVcFragments.RowTemplate.Height = 25;
            this.dgvVcFragments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVcFragments.Size = new System.Drawing.Size(838, 354);
            this.dgvVcFragments.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnSaveAll);
            this.groupBox1.Controls.Add(this.cbVcdPicker);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(850, 54);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variant Coding Domain";
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAll.Location = new System.Drawing.Point(769, 21);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAll.TabIndex = 5;
            this.btnSaveAll.Text = "Save All";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            // 
            // cbVcdPicker
            // 
            this.cbVcdPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVcdPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVcdPicker.FormattingEnabled = true;
            this.cbVcdPicker.Location = new System.Drawing.Point(6, 22);
            this.cbVcdPicker.Name = "cbVcdPicker";
            this.cbVcdPicker.Size = new System.Drawing.Size(757, 23);
            this.cbVcdPicker.TabIndex = 0;
            this.cbVcdPicker.SelectedIndexChanged += new System.EventHandler(this.cbVcdPicker_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnExecuteRequest);
            this.groupBox2.Controls.Add(this.txtRequestPreview);
            this.groupBox2.Controls.Add(this.dgvVcFragments);
            this.groupBox2.Location = new System.Drawing.Point(0, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(850, 411);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Variant Coding Fragments";
            // 
            // btnExecuteRequest
            // 
            this.btnExecuteRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteRequest.Location = new System.Drawing.Point(769, 22);
            this.btnExecuteRequest.Name = "btnExecuteRequest";
            this.btnExecuteRequest.Size = new System.Drawing.Size(75, 23);
            this.btnExecuteRequest.TabIndex = 4;
            this.btnExecuteRequest.Text = "Write";
            this.btnExecuteRequest.UseVisualStyleBackColor = true;
            // 
            // txtRequestPreview
            // 
            this.txtRequestPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRequestPreview.HideSelection = false;
            this.txtRequestPreview.Location = new System.Drawing.Point(6, 22);
            this.txtRequestPreview.Name = "txtRequestPreview";
            this.txtRequestPreview.PlaceholderText = "When a variant coding string is available, it will be shown here";
            this.txtRequestPreview.Size = new System.Drawing.Size(757, 23);
            this.txtRequestPreview.TabIndex = 3;
            // 
            // VarcodingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "VarcodingView";
            this.Size = new System.Drawing.Size(850, 471);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVcFragments)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVcFragments;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbVcdPicker;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnExecuteRequest;
        private System.Windows.Forms.TextBox txtRequestPreview;
    }
}
