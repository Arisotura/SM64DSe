namespace SM64DSe
{
    partial class MinimapEditor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinimapEditor));
            this.pbxMinimapGfx = new System.Windows.Forms.PictureBox();
            this.tsMinimapEditor = new System.Windows.Forms.ToolStrip();
            this.tslBeforeAreaBtns = new System.Windows.Forms.ToolStripLabel();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.gridPalette = new PaletteColourGrid();
            this.lblPaletteTitle = new System.Windows.Forms.Label();
            this.btnSetBackground = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCoordScale = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dmnPaletteRow = new System.Windows.Forms.DomainUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.chk256 = new System.Windows.Forms.CheckBox();
            this.chk128 = new System.Windows.Forms.CheckBox();
            this.chkIsMinimap = new System.Windows.Forms.CheckBox();
            this.chkNSCDcmp = new System.Windows.Forms.CheckBox();
            this.chkNCGDcmp = new System.Windows.Forms.CheckBox();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnSelNSC = new System.Windows.Forms.Button();
            this.btnSelNCL = new System.Windows.Forms.Button();
            this.btnSelNCG = new System.Windows.Forms.Button();
            this.cbxBPP = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dmnHeight = new System.Windows.Forms.DomainUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.dmnWidth = new System.Windows.Forms.DomainUpDown();
            this.txtSelNSC = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSelNCL = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSelNCG = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtZoom = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.splitCVertical = new System.Windows.Forms.SplitContainer();
            this.btnExportToACT = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            //((System.ComponentModel.ISupportInitialize)(this.pbxMinimapGfx)).BeginInit();
            this.tsMinimapEditor.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridPalette)).BeginInit();
            this.groupBox1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCVertical)).BeginInit();
            this.splitCVertical.Panel1.SuspendLayout();
            this.splitCVertical.Panel2.SuspendLayout();
            this.splitCVertical.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbxMinimapGfx
            // 
            this.pbxMinimapGfx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxMinimapGfx.Location = new System.Drawing.Point(3, 3);
            this.pbxMinimapGfx.Name = "pbxMinimapGfx";
            this.pbxMinimapGfx.Size = new System.Drawing.Size(547, 539);
            this.pbxMinimapGfx.TabIndex = 0;
            this.pbxMinimapGfx.TabStop = false;
            // 
            // tsMinimapEditor
            // 
            this.tsMinimapEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslBeforeAreaBtns,
            this.btnImport,
            this.btnExport});
            this.tsMinimapEditor.Location = new System.Drawing.Point(0, 0);
            this.tsMinimapEditor.Name = "tsMinimapEditor";
            this.tsMinimapEditor.Size = new System.Drawing.Size(816, 25);
            this.tsMinimapEditor.TabIndex = 1;
            this.tsMinimapEditor.Text = "toolStrip1";
            // 
            // tslBeforeAreaBtns
            // 
            this.tslBeforeAreaBtns.Name = "tslBeforeAreaBtns";
            this.tslBeforeAreaBtns.Size = new System.Drawing.Size(124, 22);
            this.tslBeforeAreaBtns.Text = "Edit minimap for area:";
            // 
            // btnImport
            // 
            this.btnImport.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(47, 22);
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(44, 22);
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // gridPalette
            // 
            this.gridPalette.Location = new System.Drawing.Point(6, 17);
            this.gridPalette.Name = "gridPalette";
            this.gridPalette.TabIndex = 4;
            this.gridPalette.CurrentCellChanged += new System.EventHandler(this.gridPalette_CurrentCellChanged);
            // 
            // lblPaletteTitle
            // 
            this.lblPaletteTitle.AutoSize = true;
            this.lblPaletteTitle.Location = new System.Drawing.Point(3, 1);
            this.lblPaletteTitle.Name = "lblPaletteTitle";
            this.lblPaletteTitle.Size = new System.Drawing.Size(43, 13);
            this.lblPaletteTitle.TabIndex = 5;
            this.lblPaletteTitle.Text = "Palette:";
            // 
            // btnSetBackground
            // 
            this.btnSetBackground.Location = new System.Drawing.Point(6, 283);
            this.btnSetBackground.Name = "btnSetBackground";
            this.btnSetBackground.Size = new System.Drawing.Size(151, 23);
            this.btnSetBackground.TabIndex = 6;
            this.btnSetBackground.Text = "Set Selected as Background";
            this.btnSetBackground.UseVisualStyleBackColor = true;
            this.btnSetBackground.Click += new System.EventHandler(this.btnSetBackground_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 315);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Co-ordinate Scale";
            // 
            // txtCoordScale
            // 
            this.txtCoordScale.Location = new System.Drawing.Point(97, 312);
            this.txtCoordScale.Name = "txtCoordScale";
            this.txtCoordScale.Size = new System.Drawing.Size(163, 20);
            this.txtCoordScale.TabIndex = 8;
            this.txtCoordScale.TextChanged += new System.EventHandler(this.txtCoordScale_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dmnPaletteRow);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.chk256);
            this.groupBox1.Controls.Add(this.chk128);
            this.groupBox1.Controls.Add(this.chkIsMinimap);
            this.groupBox1.Controls.Add(this.chkNSCDcmp);
            this.groupBox1.Controls.Add(this.chkNCGDcmp);
            this.groupBox1.Controls.Add(this.btnLoadImage);
            this.groupBox1.Controls.Add(this.btnSelNSC);
            this.groupBox1.Controls.Add(this.btnSelNCL);
            this.groupBox1.Controls.Add(this.btnSelNCG);
            this.groupBox1.Controls.Add(this.cbxBPP);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dmnHeight);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dmnWidth);
            this.groupBox1.Controls.Add(this.txtSelNSC);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtSelNCL);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSelNCG);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 338);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 283);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // dmnPaletteRow
            // 
            this.dmnPaletteRow.Location = new System.Drawing.Point(145, 200);
            this.dmnPaletteRow.Name = "dmnPaletteRow";
            this.dmnPaletteRow.Size = new System.Drawing.Size(108, 20);
            this.dmnPaletteRow.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 202);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(125, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Palette Row (4 bpp Only)";
            // 
            // chk256
            // 
            this.chk256.AutoSize = true;
            this.chk256.Location = new System.Drawing.Point(186, 231);
            this.chk256.Name = "chk256";
            this.chk256.Size = new System.Drawing.Size(67, 17);
            this.chk256.TabIndex = 20;
            this.chk256.Text = "256x256";
            this.chk256.UseVisualStyleBackColor = true;
            this.chk256.CheckedChanged += new System.EventHandler(this.chk256_CheckedChanged);
            // 
            // chk128
            // 
            this.chk128.AutoSize = true;
            this.chk128.Location = new System.Drawing.Point(113, 231);
            this.chk128.Name = "chk128";
            this.chk128.Size = new System.Drawing.Size(67, 17);
            this.chk128.TabIndex = 19;
            this.chk128.Text = "128x128";
            this.chk128.UseVisualStyleBackColor = true;
            this.chk128.CheckedChanged += new System.EventHandler(this.chk128_CheckedChanged);
            // 
            // chkIsMinimap
            // 
            this.chkIsMinimap.AutoSize = true;
            this.chkIsMinimap.Checked = true;
            this.chkIsMinimap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsMinimap.Location = new System.Drawing.Point(8, 231);
            this.chkIsMinimap.Name = "chkIsMinimap";
            this.chkIsMinimap.Size = new System.Drawing.Size(76, 17);
            this.chkIsMinimap.TabIndex = 18;
            this.chkIsMinimap.Text = "Is Minimap";
            this.chkIsMinimap.UseVisualStyleBackColor = true;
            // 
            // chkNSCDcmp
            // 
            this.chkNSCDcmp.AutoSize = true;
            this.chkNSCDcmp.Checked = true;
            this.chkNSCDcmp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNSCDcmp.Location = new System.Drawing.Point(169, 99);
            this.chkNSCDcmp.Name = "chkNSCDcmp";
            this.chkNSCDcmp.Size = new System.Drawing.Size(85, 17);
            this.chkNSCDcmp.TabIndex = 17;
            this.chkNSCDcmp.Text = "Decompress";
            this.chkNSCDcmp.UseVisualStyleBackColor = true;
            // 
            // chkNCGDcmp
            // 
            this.chkNCGDcmp.AutoSize = true;
            this.chkNCGDcmp.Checked = true;
            this.chkNCGDcmp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNCGDcmp.Location = new System.Drawing.Point(169, 15);
            this.chkNCGDcmp.Name = "chkNCGDcmp";
            this.chkNCGDcmp.Size = new System.Drawing.Size(85, 17);
            this.chkNCGDcmp.TabIndex = 16;
            this.chkNCGDcmp.Text = "Decompress";
            this.chkNCGDcmp.UseVisualStyleBackColor = true;
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Location = new System.Drawing.Point(6, 254);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(75, 23);
            this.btnLoadImage.TabIndex = 15;
            this.btnLoadImage.Text = "Load Image";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // btnSelNSC
            // 
            this.btnSelNSC.Location = new System.Drawing.Point(224, 120);
            this.btnSelNSC.Name = "btnSelNSC";
            this.btnSelNSC.Size = new System.Drawing.Size(30, 24);
            this.btnSelNSC.TabIndex = 14;
            this.btnSelNSC.Text = "...";
            this.btnSelNSC.UseVisualStyleBackColor = true;
            this.btnSelNSC.Click += new System.EventHandler(this.btnSelNSC_Click);
            // 
            // btnSelNCL
            // 
            this.btnSelNCL.Location = new System.Drawing.Point(224, 75);
            this.btnSelNCL.Name = "btnSelNCL";
            this.btnSelNCL.Size = new System.Drawing.Size(30, 24);
            this.btnSelNCL.TabIndex = 13;
            this.btnSelNCL.Text = "...";
            this.btnSelNCL.UseVisualStyleBackColor = true;
            this.btnSelNCL.Click += new System.EventHandler(this.btnSelNCL_Click);
            // 
            // btnSelNCG
            // 
            this.btnSelNCG.Location = new System.Drawing.Point(224, 36);
            this.btnSelNCG.Name = "btnSelNCG";
            this.btnSelNCG.Size = new System.Drawing.Size(30, 24);
            this.btnSelNCG.TabIndex = 12;
            this.btnSelNCG.Text = "...";
            this.btnSelNCG.UseVisualStyleBackColor = true;
            this.btnSelNCG.Click += new System.EventHandler(this.btnSelNCG_Click);
            // 
            // cbxBPP
            // 
            this.cbxBPP.FormattingEnabled = true;
            this.cbxBPP.Items.AddRange(new object[] {
            "4",
            "8"});
            this.cbxBPP.Location = new System.Drawing.Point(145, 175);
            this.cbxBPP.Name = "cbxBPP";
            this.cbxBPP.Size = new System.Drawing.Size(109, 21);
            this.cbxBPP.TabIndex = 11;
            this.cbxBPP.SelectedIndexChanged += new System.EventHandler(this.cbxBPP_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Bits Per Pixel";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(142, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Height";
            // 
            // dmnHeight
            // 
            this.dmnHeight.Location = new System.Drawing.Point(186, 148);
            this.dmnHeight.Name = "dmnHeight";
            this.dmnHeight.Size = new System.Drawing.Size(68, 20);
            this.dmnHeight.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Width";
            // 
            // dmnWidth
            // 
            this.dmnWidth.Location = new System.Drawing.Point(47, 148);
            this.dmnWidth.Name = "dmnWidth";
            this.dmnWidth.Size = new System.Drawing.Size(68, 20);
            this.dmnWidth.TabIndex = 6;
            // 
            // txtSelNSC
            // 
            this.txtSelNSC.Location = new System.Drawing.Point(6, 122);
            this.txtSelNSC.Name = "txtSelNSC";
            this.txtSelNSC.Size = new System.Drawing.Size(212, 20);
            this.txtSelNSC.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Screen NSC";
            // 
            // txtSelNCL
            // 
            this.txtSelNCL.Location = new System.Drawing.Point(6, 77);
            this.txtSelNCL.Name = "txtSelNCL";
            this.txtSelNCL.Size = new System.Drawing.Size(212, 20);
            this.txtSelNCL.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Palette NCL";
            // 
            // txtSelNCG
            // 
            this.txtSelNCG.Location = new System.Drawing.Point(6, 38);
            this.txtSelNCG.Name = "txtSelNCG";
            this.txtSelNCG.Size = new System.Drawing.Size(212, 20);
            this.txtSelNCG.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Graphic NCG";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 562);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Zoom";
            // 
            // txtZoom
            // 
            this.txtZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtZoom.Location = new System.Drawing.Point(43, 559);
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(86, 20);
            this.txtZoom.TabIndex = 13;
            this.txtZoom.TextChanged += new System.EventHandler(this.txtZoom_TextChanged);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.Location = new System.Drawing.Point(3, 582);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(501, 28);
            this.label9.TabIndex = 14;
            this.label9.Text = "Note: Imported multiple minimaps are stored left to right, top to bottom. Ensure " +
                "you select the right size, you can\'t change the size of a level\'s minimap.";
            // 
            // splitCVertical
            // 
            this.splitCVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitCVertical.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitCVertical.Location = new System.Drawing.Point(0, 25);
            this.splitCVertical.Name = "splitCVertical";
            // 
            // splitCVertical.Panel1
            // 
            this.splitCVertical.Panel1.Controls.Add(this.pbxMinimapGfx);
            this.splitCVertical.Panel1.Controls.Add(this.label9);
            this.splitCVertical.Panel1.Controls.Add(this.label8);
            this.splitCVertical.Panel1.Controls.Add(this.txtZoom);
            // 
            // splitCVertical.Panel2
            // 
            this.splitCVertical.Panel2.Controls.Add(this.btnExportToACT);
            this.splitCVertical.Panel2.Controls.Add(this.lblPaletteTitle);
            this.splitCVertical.Panel2.Controls.Add(this.gridPalette);
            this.splitCVertical.Panel2.Controls.Add(this.btnSetBackground);
            this.splitCVertical.Panel2.Controls.Add(this.label1);
            this.splitCVertical.Panel2.Controls.Add(this.groupBox1);
            this.splitCVertical.Panel2.Controls.Add(this.txtCoordScale);
            this.splitCVertical.Size = new System.Drawing.Size(816, 626);
            this.splitCVertical.SplitterDistance = 535;
            this.splitCVertical.TabIndex = 15;
            // 
            // btnExportToACT
            // 
            this.btnExportToACT.Location = new System.Drawing.Point(175, 283);
            this.btnExportToACT.Name = "btnExportToACT";
            this.btnExportToACT.Size = new System.Drawing.Size(92, 23);
            this.btnExportToACT.TabIndex = 12;
            this.btnExportToACT.Text = "Export to ACT";
            this.toolTip1.SetToolTip(this.btnExportToACT, "Export to Adobe Colour Table (ACT)");
            this.btnExportToACT.UseVisualStyleBackColor = true;
            this.btnExportToACT.Click += new System.EventHandler(this.btnExportToACT_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // MinimapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 651);
            this.Controls.Add(this.splitCVertical);
            this.Controls.Add(this.tsMinimapEditor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MinimapEditor";
            this.Text = "Minimap and Image Editor";
            this.Load += new System.EventHandler(this.MinimapEditor_Load);
            //((System.ComponentModel.ISupportInitialize)(this.pbxMinimapGfx)).EndInit();
            this.tsMinimapEditor.ResumeLayout(false);
            this.tsMinimapEditor.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridPalette)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitCVertical.Panel1.ResumeLayout(false);
            this.splitCVertical.Panel1.PerformLayout();
            this.splitCVertical.Panel2.ResumeLayout(false);
            this.splitCVertical.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCVertical)).EndInit();
            this.splitCVertical.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxMinimapGfx;
        private System.Windows.Forms.ToolStrip tsMinimapEditor;
        private System.Windows.Forms.ToolStripLabel tslBeforeAreaBtns;
        private System.Windows.Forms.ToolStripButton btnImport;
        private PaletteColourGrid gridPalette;
        private System.Windows.Forms.Label lblPaletteTitle;
        private System.Windows.Forms.Button btnSetBackground;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCoordScale;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxBPP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DomainUpDown dmnHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DomainUpDown dmnWidth;
        private System.Windows.Forms.TextBox txtSelNSC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSelNCL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSelNCG;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelNSC;
        private System.Windows.Forms.Button btnSelNCL;
        private System.Windows.Forms.Button btnSelNCG;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.CheckBox chkNSCDcmp;
        private System.Windows.Forms.CheckBox chkNCGDcmp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtZoom;
        private System.Windows.Forms.CheckBox chk256;
        private System.Windows.Forms.CheckBox chk128;
        private System.Windows.Forms.CheckBox chkIsMinimap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.SplitContainer splitCVertical;
        private System.Windows.Forms.DomainUpDown dmnPaletteRow;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnExportToACT;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}