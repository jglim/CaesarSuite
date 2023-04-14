
namespace Diogenes.Forms
{
    partial class DiagServicesView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvDiagPicker = new System.Windows.Forms.DataGridView();
            this.chkVariantFilter = new System.Windows.Forms.CheckBox();
            this.txtDiagFilter = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.gbRequestBuilder = new System.Windows.Forms.GroupBox();
            this.btnExecuteRequest = new System.Windows.Forms.Button();
            this.dgvRequestBuilder = new System.Windows.Forms.DataGridView();
            this.txtRequestPreview = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiagPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.gbRequestBuilder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequestBuilder)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(934, 553);
            this.splitContainer1.SplitterDistance = 311;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvDiagPicker);
            this.groupBox1.Controls.Add(this.chkVariantFilter);
            this.groupBox1.Controls.Add(this.txtDiagFilter);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(934, 311);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Diagnostic Services List";
            // 
            // dgvDiagPicker
            // 
            this.dgvDiagPicker.AllowUserToAddRows = false;
            this.dgvDiagPicker.AllowUserToDeleteRows = false;
            this.dgvDiagPicker.AllowUserToResizeRows = false;
            this.dgvDiagPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDiagPicker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDiagPicker.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDiagPicker.Location = new System.Drawing.Point(6, 76);
            this.dgvDiagPicker.MultiSelect = false;
            this.dgvDiagPicker.Name = "dgvDiagPicker";
            this.dgvDiagPicker.RowHeadersVisible = false;
            this.dgvDiagPicker.RowTemplate.Height = 25;
            this.dgvDiagPicker.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDiagPicker.Size = new System.Drawing.Size(922, 229);
            this.dgvDiagPicker.TabIndex = 0;
            this.dgvDiagPicker.SelectionChanged += new System.EventHandler(this.dgvDiagPicker_SelectionChanged);
            // 
            // chkVariantFilter
            // 
            this.chkVariantFilter.AutoSize = true;
            this.chkVariantFilter.Checked = true;
            this.chkVariantFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVariantFilter.Location = new System.Drawing.Point(6, 51);
            this.chkVariantFilter.Name = "chkVariantFilter";
            this.chkVariantFilter.Size = new System.Drawing.Size(141, 19);
            this.chkVariantFilter.TabIndex = 2;
            this.chkVariantFilter.Text = "Filter by active variant";
            this.chkVariantFilter.UseVisualStyleBackColor = true;
            this.chkVariantFilter.CheckedChanged += new System.EventHandler(this.chkVariantFilter_CheckedChanged);
            // 
            // txtDiagFilter
            // 
            this.txtDiagFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiagFilter.Location = new System.Drawing.Point(6, 22);
            this.txtDiagFilter.Name = "txtDiagFilter";
            this.txtDiagFilter.PlaceholderText = "Search for a diagnostic service..";
            this.txtDiagFilter.Size = new System.Drawing.Size(922, 23);
            this.txtDiagFilter.TabIndex = 1;
            this.txtDiagFilter.TextChanged += new System.EventHandler(this.txtDiagFilter_TextChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.gbRequestBuilder);
            this.splitContainer2.Size = new System.Drawing.Size(934, 238);
            this.splitContainer2.SplitterDistance = 160;
            this.splitContainer2.TabIndex = 5;
            // 
            // gbRequestBuilder
            // 
            this.gbRequestBuilder.Controls.Add(this.btnExecuteRequest);
            this.gbRequestBuilder.Controls.Add(this.dgvRequestBuilder);
            this.gbRequestBuilder.Controls.Add(this.txtRequestPreview);
            this.gbRequestBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRequestBuilder.Location = new System.Drawing.Point(0, 0);
            this.gbRequestBuilder.Name = "gbRequestBuilder";
            this.gbRequestBuilder.Size = new System.Drawing.Size(934, 160);
            this.gbRequestBuilder.TabIndex = 4;
            this.gbRequestBuilder.TabStop = false;
            this.gbRequestBuilder.Text = "Request Builder";
            // 
            // btnExecuteRequest
            // 
            this.btnExecuteRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteRequest.Location = new System.Drawing.Point(853, 22);
            this.btnExecuteRequest.Name = "btnExecuteRequest";
            this.btnExecuteRequest.Size = new System.Drawing.Size(75, 23);
            this.btnExecuteRequest.TabIndex = 2;
            this.btnExecuteRequest.Text = "Execute";
            this.btnExecuteRequest.UseVisualStyleBackColor = true;
            this.btnExecuteRequest.Click += new System.EventHandler(this.btnExecuteRequest_Click);
            // 
            // dgvRequestBuilder
            // 
            this.dgvRequestBuilder.AllowUserToAddRows = false;
            this.dgvRequestBuilder.AllowUserToDeleteRows = false;
            this.dgvRequestBuilder.AllowUserToResizeRows = false;
            this.dgvRequestBuilder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRequestBuilder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRequestBuilder.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvRequestBuilder.Location = new System.Drawing.Point(6, 51);
            this.dgvRequestBuilder.MultiSelect = false;
            this.dgvRequestBuilder.Name = "dgvRequestBuilder";
            this.dgvRequestBuilder.RowHeadersVisible = false;
            this.dgvRequestBuilder.RowTemplate.Height = 25;
            this.dgvRequestBuilder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequestBuilder.Size = new System.Drawing.Size(922, 103);
            this.dgvRequestBuilder.TabIndex = 0;
            this.dgvRequestBuilder.SelectionChanged += new System.EventHandler(this.dgvRequestBuilder_SelectionChanged);
            // 
            // txtRequestPreview
            // 
            this.txtRequestPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRequestPreview.HideSelection = false;
            this.txtRequestPreview.Location = new System.Drawing.Point(6, 22);
            this.txtRequestPreview.Name = "txtRequestPreview";
            this.txtRequestPreview.PlaceholderText = "When a diagnostic service is selected, the assembled request will be shown here";
            this.txtRequestPreview.Size = new System.Drawing.Size(841, 23);
            this.txtRequestPreview.TabIndex = 1;
            // 
            // DiagServicesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DiagServicesView";
            this.Size = new System.Drawing.Size(934, 553);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiagPicker)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.gbRequestBuilder.ResumeLayout(false);
            this.gbRequestBuilder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequestBuilder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvDiagPicker;
        private System.Windows.Forms.CheckBox chkVariantFilter;
        private System.Windows.Forms.TextBox txtDiagFilter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbRequestBuilder;
        private System.Windows.Forms.DataGridView dgvRequestBuilder;
        private System.Windows.Forms.TextBox txtRequestPreview;
        private System.Windows.Forms.Button btnExecuteRequest;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}
