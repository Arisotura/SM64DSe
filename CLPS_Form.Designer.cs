namespace SM64DSe
{
    partial class CLPS_Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CLPS_Form));
            this.label1 = new System.Windows.Forms.Label();
            this.txtNumEntries = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.cbxLevels = new System.Windows.Forms.ComboBox();
            this.gridCLPSData = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnShiftUp = new System.Windows.Forms.Button();
            this.btnShiftDown = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridCLPSData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of entries:";
            // 
            // txtNumEntries
            // 
            this.txtNumEntries.Enabled = false;
            this.txtNumEntries.Location = new System.Drawing.Point(261, 26);
            this.txtNumEntries.Name = "txtNumEntries";
            this.txtNumEntries.Size = new System.Drawing.Size(121, 20);
            this.txtNumEntries.TabIndex = 1;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(15, 72);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(137, 23);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "Copy CLPS From Level:";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // cbxLevels
            // 
            this.cbxLevels.FormattingEnabled = true;
            this.cbxLevels.Location = new System.Drawing.Point(196, 74);
            this.cbxLevels.Name = "cbxLevels";
            this.cbxLevels.Size = new System.Drawing.Size(186, 21);
            this.cbxLevels.TabIndex = 4;
            // 
            // gridCLPSData
            // 
            this.gridCLPSData.AllowUserToAddRows = false;
            this.gridCLPSData.AllowUserToDeleteRows = false;
            this.gridCLPSData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCLPSData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCLPSData.Location = new System.Drawing.Point(15, 110);
            this.gridCLPSData.MultiSelect = false;
            this.gridCLPSData.Name = "gridCLPSData";
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.gridCLPSData.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCLPSData.Size = new System.Drawing.Size(860, 268);
            this.gridCLPSData.StandardTab = true;
            this.gridCLPSData.TabIndex = 6;
            this.gridCLPSData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCLPSData_CellEndEdit);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(15, 385);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 23);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(43, 385);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(22, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnShiftUp
            // 
            this.btnShiftUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShiftUp.Location = new System.Drawing.Point(881, 275);
            this.btnShiftUp.Name = "btnShiftUp";
            this.btnShiftUp.Size = new System.Drawing.Size(22, 23);
            this.btnShiftUp.TabIndex = 9;
            this.btnShiftUp.Text = "^";
            this.btnShiftUp.UseVisualStyleBackColor = true;
            this.btnShiftUp.Click += new System.EventHandler(this.btnShiftUp_Click);
            // 
            // btnShiftDown
            // 
            this.btnShiftDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShiftDown.Location = new System.Drawing.Point(881, 304);
            this.btnShiftDown.Name = "btnShiftDown";
            this.btnShiftDown.Size = new System.Drawing.Size(22, 23);
            this.btnShiftDown.TabIndex = 10;
            this.btnShiftDown.Text = "v";
            this.btnShiftDown.UseVisualStyleBackColor = true;
            this.btnShiftDown.Click += new System.EventHandler(this.btnShiftDown_Click);
            // 
            // CLPS_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 430);
            this.Controls.Add(this.btnShiftDown);
            this.Controls.Add(this.btnShiftUp);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.gridCLPSData);
            this.Controls.Add(this.cbxLevels);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtNumEntries);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CLPS_Form";
            this.Text = "CLPS Data";
            ((System.ComponentModel.ISupportInitialize)(this.gridCLPSData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumEntries;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ComboBox cbxLevels;
        private System.Windows.Forms.DataGridView gridCLPSData;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnShiftUp;
        private System.Windows.Forms.Button btnShiftDown;
    }
}