
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
            components = new System.ComponentModel.Container();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            groupBox1 = new System.Windows.Forms.GroupBox();
            dgvDiagPicker = new System.Windows.Forms.DataGridView();
            chkVariantFilter = new System.Windows.Forms.CheckBox();
            txtDiagFilter = new System.Windows.Forms.TextBox();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            gbRequestBuilder = new System.Windows.Forms.GroupBox();
            btnExecuteRequest = new System.Windows.Forms.Button();
            dgvRequestBuilder = new System.Windows.Forms.DataGridView();
            txtRequestPreview = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDiagPicker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            gbRequestBuilder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRequestBuilder).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new System.Drawing.Size(934, 627);
            splitContainer1.SplitterDistance = 352;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dgvDiagPicker);
            groupBox1.Controls.Add(chkVariantFilter);
            groupBox1.Controls.Add(txtDiagFilter);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(934, 352);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Diagnostic Services List";
            // 
            // dgvDiagPicker
            // 
            dgvDiagPicker.AllowUserToAddRows = false;
            dgvDiagPicker.AllowUserToDeleteRows = false;
            dgvDiagPicker.AllowUserToResizeRows = false;
            dgvDiagPicker.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvDiagPicker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDiagPicker.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            dgvDiagPicker.Location = new System.Drawing.Point(6, 86);
            dgvDiagPicker.MultiSelect = false;
            dgvDiagPicker.Name = "dgvDiagPicker";
            dgvDiagPicker.RowHeadersVisible = false;
            dgvDiagPicker.RowHeadersWidth = 45;
            dgvDiagPicker.RowTemplate.Height = 25;
            dgvDiagPicker.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvDiagPicker.Size = new System.Drawing.Size(922, 260);
            dgvDiagPicker.TabIndex = 0;
            dgvDiagPicker.SelectionChanged += dgvDiagPicker_SelectionChanged;
            // 
            // chkVariantFilter
            // 
            chkVariantFilter.AutoSize = true;
            chkVariantFilter.Checked = true;
            chkVariantFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            chkVariantFilter.Location = new System.Drawing.Point(6, 58);
            chkVariantFilter.Name = "chkVariantFilter";
            chkVariantFilter.Size = new System.Drawing.Size(153, 21);
            chkVariantFilter.TabIndex = 2;
            chkVariantFilter.Text = "Filter by active variant";
            chkVariantFilter.UseVisualStyleBackColor = true;
            chkVariantFilter.CheckedChanged += chkVariantFilter_CheckedChanged;
            // 
            // txtDiagFilter
            // 
            txtDiagFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDiagFilter.Location = new System.Drawing.Point(6, 25);
            txtDiagFilter.Name = "txtDiagFilter";
            txtDiagFilter.PlaceholderText = "Search for a diagnostic service..";
            txtDiagFilter.Size = new System.Drawing.Size(922, 25);
            txtDiagFilter.TabIndex = 1;
            txtDiagFilter.TextChanged += txtDiagFilter_TextChanged;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(gbRequestBuilder);
            splitContainer2.Size = new System.Drawing.Size(934, 270);
            splitContainer2.SplitterDistance = 178;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 5;
            // 
            // gbRequestBuilder
            // 
            gbRequestBuilder.Controls.Add(btnExecuteRequest);
            gbRequestBuilder.Controls.Add(dgvRequestBuilder);
            gbRequestBuilder.Controls.Add(txtRequestPreview);
            gbRequestBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            gbRequestBuilder.Location = new System.Drawing.Point(0, 0);
            gbRequestBuilder.Name = "gbRequestBuilder";
            gbRequestBuilder.Size = new System.Drawing.Size(934, 178);
            gbRequestBuilder.TabIndex = 4;
            gbRequestBuilder.TabStop = false;
            gbRequestBuilder.Text = "Request Builder";
            // 
            // btnExecuteRequest
            // 
            btnExecuteRequest.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExecuteRequest.Location = new System.Drawing.Point(853, 25);
            btnExecuteRequest.Name = "btnExecuteRequest";
            btnExecuteRequest.Size = new System.Drawing.Size(75, 26);
            btnExecuteRequest.TabIndex = 2;
            btnExecuteRequest.Text = "Execute";
            btnExecuteRequest.UseVisualStyleBackColor = true;
            btnExecuteRequest.Click += btnExecuteRequest_Click;
            // 
            // dgvRequestBuilder
            // 
            dgvRequestBuilder.AllowUserToAddRows = false;
            dgvRequestBuilder.AllowUserToDeleteRows = false;
            dgvRequestBuilder.AllowUserToResizeRows = false;
            dgvRequestBuilder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvRequestBuilder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRequestBuilder.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            dgvRequestBuilder.Location = new System.Drawing.Point(6, 58);
            dgvRequestBuilder.MultiSelect = false;
            dgvRequestBuilder.Name = "dgvRequestBuilder";
            dgvRequestBuilder.RowHeadersVisible = false;
            dgvRequestBuilder.RowHeadersWidth = 45;
            dgvRequestBuilder.RowTemplate.Height = 25;
            dgvRequestBuilder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvRequestBuilder.Size = new System.Drawing.Size(922, 114);
            dgvRequestBuilder.TabIndex = 0;
            dgvRequestBuilder.SelectionChanged += dgvRequestBuilder_SelectionChanged;
            // 
            // txtRequestPreview
            // 
            txtRequestPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtRequestPreview.HideSelection = false;
            txtRequestPreview.Location = new System.Drawing.Point(6, 25);
            txtRequestPreview.Name = "txtRequestPreview";
            txtRequestPreview.PlaceholderText = "When a diagnostic service is selected, the assembled request will be shown here";
            txtRequestPreview.Size = new System.Drawing.Size(841, 25);
            txtRequestPreview.TabIndex = 1;
            // 
            // DiagServicesView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "DiagServicesView";
            Size = new System.Drawing.Size(934, 627);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDiagPicker).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            gbRequestBuilder.ResumeLayout(false);
            gbRequestBuilder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRequestBuilder).EndInit();
            ResumeLayout(false);
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
