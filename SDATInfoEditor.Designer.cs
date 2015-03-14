namespace SM64DSe
{
    partial class SDATInfoEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDATInfoEditor));
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.tvSDAT = new System.Windows.Forms.TreeView();
            this.btnSave = new System.Windows.Forms.Button();
            this.pgData = new System.Windows.Forms.PropertyGrid();
            //((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 0);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.tvSDAT);
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.btnSave);
            this.spcMain.Panel2.Controls.Add(this.pgData);
            this.spcMain.Size = new System.Drawing.Size(571, 326);
            this.spcMain.SplitterDistance = 190;
            this.spcMain.TabIndex = 0;
            // 
            // tvSDAT
            // 
            this.tvSDAT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSDAT.Location = new System.Drawing.Point(0, 0);
            this.tvSDAT.Name = "tvSDAT";
            this.tvSDAT.Size = new System.Drawing.Size(190, 326);
            this.tvSDAT.TabIndex = 0;
            this.tvSDAT.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSDAT_AfterSelect);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pgData
            // 
            this.pgData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgData.Location = new System.Drawing.Point(2, 32);
            this.pgData.Name = "pgData";
            this.pgData.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgData.Size = new System.Drawing.Size(372, 291);
            this.pgData.TabIndex = 0;
            this.pgData.ToolbarVisible = false;
            this.pgData.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgData_PropertyValueChanged);
            // 
            // SDATInfoEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 326);
            this.Controls.Add(this.spcMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SDATInfoEditor";
            this.Text = "SDAT Info Block Editor";
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.TreeView tvSDAT;
        private System.Windows.Forms.PropertyGrid pgData;
        private System.Windows.Forms.Button btnSave;
    }
}