
namespace Diogenes.Forms
{
    partial class FlashView
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
            this.gbFlashCollection = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvFlashCollection = new System.Windows.Forms.DataGridView();
            this.btnFlash = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dgvEcuState = new System.Windows.Forms.DataGridView();
            this.pbFlashProgress = new System.Windows.Forms.ProgressBar();
            this.gbFlashCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlashCollection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEcuState)).BeginInit();
            this.SuspendLayout();
            // 
            // gbFlashCollection
            // 
            this.gbFlashCollection.Controls.Add(this.btnRemove);
            this.gbFlashCollection.Controls.Add(this.btnAdd);
            this.gbFlashCollection.Controls.Add(this.dgvFlashCollection);
            this.gbFlashCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbFlashCollection.Location = new System.Drawing.Point(0, 0);
            this.gbFlashCollection.Name = "gbFlashCollection";
            this.gbFlashCollection.Size = new System.Drawing.Size(801, 214);
            this.gbFlashCollection.TabIndex = 5;
            this.gbFlashCollection.TabStop = false;
            this.gbFlashCollection.Text = "Flashware Collection";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(720, 182);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(639, 182);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvFlashCollection
            // 
            this.dgvFlashCollection.AllowUserToAddRows = false;
            this.dgvFlashCollection.AllowUserToDeleteRows = false;
            this.dgvFlashCollection.AllowUserToResizeRows = false;
            this.dgvFlashCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFlashCollection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFlashCollection.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFlashCollection.Location = new System.Drawing.Point(6, 22);
            this.dgvFlashCollection.MultiSelect = false;
            this.dgvFlashCollection.Name = "dgvFlashCollection";
            this.dgvFlashCollection.RowHeadersVisible = false;
            this.dgvFlashCollection.RowTemplate.Height = 25;
            this.dgvFlashCollection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFlashCollection.Size = new System.Drawing.Size(789, 154);
            this.dgvFlashCollection.TabIndex = 0;
            // 
            // btnFlash
            // 
            this.btnFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlash.Location = new System.Drawing.Point(720, 182);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(75, 23);
            this.btnFlash.TabIndex = 3;
            this.btnFlash.Text = "Flash";
            this.btnFlash.UseVisualStyleBackColor = true;
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.gbFlashCollection);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(801, 429);
            this.splitContainer1.SplitterDistance = 214;
            this.splitContainer1.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbFlashProgress);
            this.groupBox1.Controls.Add(this.lblProgress);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.btnFlash);
            this.groupBox1.Controls.Add(this.dgvEcuState);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(801, 211);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ECU Flash state";
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(6, 186);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(12, 15);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "-";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Location = new System.Drawing.Point(639, 182);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 4;
            this.btnQuery.Text = "Query";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dgvEcuState
            // 
            this.dgvEcuState.AllowUserToAddRows = false;
            this.dgvEcuState.AllowUserToDeleteRows = false;
            this.dgvEcuState.AllowUserToResizeRows = false;
            this.dgvEcuState.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEcuState.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEcuState.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEcuState.Location = new System.Drawing.Point(6, 22);
            this.dgvEcuState.MultiSelect = false;
            this.dgvEcuState.Name = "dgvEcuState";
            this.dgvEcuState.RowHeadersVisible = false;
            this.dgvEcuState.RowTemplate.Height = 25;
            this.dgvEcuState.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEcuState.Size = new System.Drawing.Size(789, 138);
            this.dgvEcuState.TabIndex = 0;
            // 
            // pbFlashProgress
            // 
            this.pbFlashProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFlashProgress.Location = new System.Drawing.Point(6, 166);
            this.pbFlashProgress.Name = "pbFlashProgress";
            this.pbFlashProgress.Size = new System.Drawing.Size(789, 10);
            this.pbFlashProgress.TabIndex = 6;
            // 
            // FlashView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FlashView";
            this.Size = new System.Drawing.Size(801, 429);
            this.gbFlashCollection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlashCollection)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEcuState)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFlashCollection;
        private System.Windows.Forms.DataGridView dgvFlashCollection;
        private System.Windows.Forms.Button btnFlash;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvEcuState;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar pbFlashProgress;
    }
}
