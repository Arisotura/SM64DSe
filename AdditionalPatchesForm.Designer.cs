namespace SM64DSe
{
    partial class AdditionalPatchesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdditionalPatchesForm));
            this.gridPatches = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApplyPatch = new System.Windows.Forms.Button();
            this.btnUndoPatch = new System.Windows.Forms.Button();
            this.txtPatchName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnNewPatch = new System.Windows.Forms.Button();
            this.btnEditPatch = new System.Windows.Forms.Button();
            this.btnDeletePatch = new System.Windows.Forms.Button();
            this.txtApplyToFile = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.chkApplyToFile = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.chkDecompressAllOverlays = new System.Windows.Forms.CheckBox();
            this.txtPatchData = new System.Windows.Forms.TextBox();
            this.btnSavePatch = new System.Windows.Forms.Button();
            this.pnlNewEditPatches = new System.Windows.Forms.Panel();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOverlayID = new System.Windows.Forms.TextBox();
            this.chkApplyToOverlay = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlExistingPatches = new System.Windows.Forms.Panel();
            this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
            //((System.ComponentModel.ISupportInitialize)(this.gridPatches)).BeginInit();
            this.pnlNewEditPatches.SuspendLayout();
            this.pnlExistingPatches.SuspendLayout();
            this.tlpTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridPatches
            // 
            this.gridPatches.AllowUserToAddRows = false;
            this.gridPatches.AllowUserToDeleteRows = false;
            this.gridPatches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPatches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridPatches.Location = new System.Drawing.Point(0, 20);
            this.gridPatches.MultiSelect = false;
            this.gridPatches.Name = "gridPatches";
            this.gridPatches.Size = new System.Drawing.Size(666, 194);
            this.gridPatches.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Patches:";
            // 
            // btnApplyPatch
            // 
            this.btnApplyPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApplyPatch.Location = new System.Drawing.Point(2, 223);
            this.btnApplyPatch.Name = "btnApplyPatch";
            this.btnApplyPatch.Size = new System.Drawing.Size(96, 23);
            this.btnApplyPatch.TabIndex = 2;
            this.btnApplyPatch.Text = "Apply Patch";
            this.btnApplyPatch.UseVisualStyleBackColor = true;
            this.btnApplyPatch.Click += new System.EventHandler(this.btnApplyPatch_Click);
            // 
            // btnUndoPatch
            // 
            this.btnUndoPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUndoPatch.Location = new System.Drawing.Point(104, 223);
            this.btnUndoPatch.Name = "btnUndoPatch";
            this.btnUndoPatch.Size = new System.Drawing.Size(96, 23);
            this.btnUndoPatch.TabIndex = 3;
            this.btnUndoPatch.Text = "Undo Patch";
            this.btnUndoPatch.UseVisualStyleBackColor = true;
            this.btnUndoPatch.Click += new System.EventHandler(this.btnUndoPatch_Click);
            // 
            // txtPatchName
            // 
            this.txtPatchName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchName.Location = new System.Drawing.Point(137, 3);
            this.txtPatchName.Name = "txtPatchName";
            this.txtPatchName.Size = new System.Drawing.Size(351, 20);
            this.txtPatchName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name/Description:";
            // 
            // btnNewPatch
            // 
            this.btnNewPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewPatch.Location = new System.Drawing.Point(410, 223);
            this.btnNewPatch.Name = "btnNewPatch";
            this.btnNewPatch.Size = new System.Drawing.Size(96, 23);
            this.btnNewPatch.TabIndex = 6;
            this.btnNewPatch.Text = "New Patch";
            this.btnNewPatch.UseVisualStyleBackColor = true;
            this.btnNewPatch.Click += new System.EventHandler(this.btnNewPatch_Click);
            // 
            // btnEditPatch
            // 
            this.btnEditPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditPatch.Location = new System.Drawing.Point(206, 223);
            this.btnEditPatch.Name = "btnEditPatch";
            this.btnEditPatch.Size = new System.Drawing.Size(96, 23);
            this.btnEditPatch.TabIndex = 31;
            this.btnEditPatch.Text = "Edit Patch";
            this.btnEditPatch.UseVisualStyleBackColor = true;
            this.btnEditPatch.Click += new System.EventHandler(this.btnEditPatch_Click);
            // 
            // btnDeletePatch
            // 
            this.btnDeletePatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeletePatch.Location = new System.Drawing.Point(308, 223);
            this.btnDeletePatch.Name = "btnDeletePatch";
            this.btnDeletePatch.Size = new System.Drawing.Size(96, 23);
            this.btnDeletePatch.TabIndex = 32;
            this.btnDeletePatch.Text = "Delete Patch";
            this.btnDeletePatch.UseVisualStyleBackColor = true;
            this.btnDeletePatch.Click += new System.EventHandler(this.btnDeletePatch_Click);
            // 
            // txtApplyToFile
            // 
            this.txtApplyToFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtApplyToFile.Location = new System.Drawing.Point(137, 29);
            this.txtApplyToFile.Name = "txtApplyToFile";
            this.txtApplyToFile.Size = new System.Drawing.Size(168, 20);
            this.txtApplyToFile.TabIndex = 35;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFile.Location = new System.Drawing.Point(311, 27);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(30, 23);
            this.btnSelectFile.TabIndex = 36;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // chkApplyToFile
            // 
            this.chkApplyToFile.AutoSize = true;
            this.chkApplyToFile.Location = new System.Drawing.Point(6, 31);
            this.chkApplyToFile.Name = "chkApplyToFile";
            this.chkApplyToFile.Size = new System.Drawing.Size(86, 17);
            this.chkApplyToFile.TabIndex = 37;
            this.chkApplyToFile.Text = "Apply to File:";
            this.chkApplyToFile.UseVisualStyleBackColor = true;
            this.chkApplyToFile.CheckedChanged += new System.EventHandler(this.chkApplyToFile_CheckedChanged);
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.Location = new System.Drawing.Point(347, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(320, 18);
            this.label15.TabIndex = 38;
            this.label15.Text = "Note: Addresses should be relative to the start of the file selected.";
            // 
            // chkDecompressAllOverlays
            // 
            this.chkDecompressAllOverlays.AutoSize = true;
            this.chkDecompressAllOverlays.Location = new System.Drawing.Point(6, 77);
            this.chkDecompressAllOverlays.Name = "chkDecompressAllOverlays";
            this.chkDecompressAllOverlays.Size = new System.Drawing.Size(143, 17);
            this.chkDecompressAllOverlays.TabIndex = 39;
            this.chkDecompressAllOverlays.Text = "Decompress All Overlays";
            this.chkDecompressAllOverlays.UseVisualStyleBackColor = true;
            // 
            // txtPatchData
            // 
            this.txtPatchData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchData.Location = new System.Drawing.Point(6, 139);
            this.txtPatchData.Multiline = true;
            this.txtPatchData.Name = "txtPatchData";
            this.txtPatchData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPatchData.Size = new System.Drawing.Size(660, 210);
            this.txtPatchData.TabIndex = 40;
            // 
            // btnSavePatch
            // 
            this.btnSavePatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSavePatch.Location = new System.Drawing.Point(6, 354);
            this.btnSavePatch.Name = "btnSavePatch";
            this.btnSavePatch.Size = new System.Drawing.Size(90, 31);
            this.btnSavePatch.TabIndex = 41;
            this.btnSavePatch.Text = "Save";
            this.btnSavePatch.UseVisualStyleBackColor = true;
            this.btnSavePatch.Click += new System.EventHandler(this.btnSavePatch_Click);
            // 
            // pnlNewEditPatches
            // 
            this.pnlNewEditPatches.Controls.Add(this.txtAuthor);
            this.pnlNewEditPatches.Controls.Add(this.label4);
            this.pnlNewEditPatches.Controls.Add(this.txtOverlayID);
            this.pnlNewEditPatches.Controls.Add(this.chkApplyToOverlay);
            this.pnlNewEditPatches.Controls.Add(this.label3);
            this.pnlNewEditPatches.Controls.Add(this.label2);
            this.pnlNewEditPatches.Controls.Add(this.btnSavePatch);
            this.pnlNewEditPatches.Controls.Add(this.txtPatchName);
            this.pnlNewEditPatches.Controls.Add(this.txtPatchData);
            this.pnlNewEditPatches.Controls.Add(this.txtApplyToFile);
            this.pnlNewEditPatches.Controls.Add(this.chkDecompressAllOverlays);
            this.pnlNewEditPatches.Controls.Add(this.btnSelectFile);
            this.pnlNewEditPatches.Controls.Add(this.label15);
            this.pnlNewEditPatches.Controls.Add(this.chkApplyToFile);
            this.pnlNewEditPatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNewEditPatches.Location = new System.Drawing.Point(3, 268);
            this.pnlNewEditPatches.Name = "pnlNewEditPatches";
            this.pnlNewEditPatches.Size = new System.Drawing.Size(669, 392);
            this.pnlNewEditPatches.TabIndex = 42;
            // 
            // txtAuthor
            // 
            this.txtAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAuthor.Location = new System.Drawing.Point(538, 3);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(122, 20);
            this.txtAuthor.TabIndex = 46;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(494, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Author";
            // 
            // txtOverlayID
            // 
            this.txtOverlayID.Location = new System.Drawing.Point(137, 52);
            this.txtOverlayID.Name = "txtOverlayID";
            this.txtOverlayID.Size = new System.Drawing.Size(168, 20);
            this.txtOverlayID.TabIndex = 44;
            // 
            // chkApplyToOverlay
            // 
            this.chkApplyToOverlay.AutoSize = true;
            this.chkApplyToOverlay.Location = new System.Drawing.Point(6, 54);
            this.chkApplyToOverlay.Name = "chkApplyToOverlay";
            this.chkApplyToOverlay.Size = new System.Drawing.Size(126, 17);
            this.chkApplyToOverlay.TabIndex = 43;
            this.chkApplyToOverlay.Text = "Apply to Overlay (ID):";
            this.chkApplyToOverlay.UseVisualStyleBackColor = true;
            this.chkApplyToOverlay.CheckedChanged += new System.EventHandler(this.chkApplyToOverlay_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(2, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 38);
            this.label3.TabIndex = 42;
            this.label3.Text = "Enter patch data in the format (Hex) \r\nADDRESS N: \r\nBYTE 1 BYTE 2 ... BYTE N";
            // 
            // pnlExistingPatches
            // 
            this.pnlExistingPatches.Controls.Add(this.label1);
            this.pnlExistingPatches.Controls.Add(this.gridPatches);
            this.pnlExistingPatches.Controls.Add(this.btnDeletePatch);
            this.pnlExistingPatches.Controls.Add(this.btnApplyPatch);
            this.pnlExistingPatches.Controls.Add(this.btnEditPatch);
            this.pnlExistingPatches.Controls.Add(this.btnUndoPatch);
            this.pnlExistingPatches.Controls.Add(this.btnNewPatch);
            this.pnlExistingPatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlExistingPatches.Location = new System.Drawing.Point(3, 3);
            this.pnlExistingPatches.Name = "pnlExistingPatches";
            this.pnlExistingPatches.Size = new System.Drawing.Size(669, 259);
            this.pnlExistingPatches.TabIndex = 43;
            // 
            // tlpTop
            // 
            this.tlpTop.ColumnCount = 1;
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTop.Controls.Add(this.pnlExistingPatches, 0, 0);
            this.tlpTop.Controls.Add(this.pnlNewEditPatches, 0, 1);
            this.tlpTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTop.Location = new System.Drawing.Point(0, 0);
            this.tlpTop.Name = "tlpTop";
            this.tlpTop.RowCount = 2;
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpTop.Size = new System.Drawing.Size(675, 663);
            this.tlpTop.TabIndex = 44;
            // 
            // AdditionalPatchesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(675, 663);
            this.Controls.Add(this.tlpTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AdditionalPatchesForm";
            this.Text = "Additional Patches";
            this.Load += new System.EventHandler(this.AdditionalPatchesForm_Load);
            //((System.ComponentModel.ISupportInitialize)(this.gridPatches)).EndInit();
            this.pnlNewEditPatches.ResumeLayout(false);
            this.pnlNewEditPatches.PerformLayout();
            this.pnlExistingPatches.ResumeLayout(false);
            this.pnlExistingPatches.PerformLayout();
            this.tlpTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridPatches;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApplyPatch;
        private System.Windows.Forms.Button btnUndoPatch;
        private System.Windows.Forms.TextBox txtPatchName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnNewPatch;
        private System.Windows.Forms.Button btnEditPatch;
        private System.Windows.Forms.Button btnDeletePatch;
        private System.Windows.Forms.TextBox txtApplyToFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.CheckBox chkApplyToFile;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chkDecompressAllOverlays;
        private System.Windows.Forms.TextBox txtPatchData;
        private System.Windows.Forms.Button btnSavePatch;
        private System.Windows.Forms.Panel pnlNewEditPatches;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlExistingPatches;
        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.TextBox txtOverlayID;
        private System.Windows.Forms.CheckBox chkApplyToOverlay;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label label4;
    }
}