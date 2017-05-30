namespace SM64DSe
{
    partial class TextureEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureEditorForm));
            this.lblTexture = new System.Windows.Forms.Label();
            this.lbxTextures = new System.Windows.Forms.ListBox();
            this.pbxTexture = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPalette = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnReplaceSelected = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.btnExportAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoadBTP = new System.Windows.Forms.ToolStripButton();
            this.lbxPalettes = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnMatPreviewStop = new System.Windows.Forms.Button();
            this.txtBTPMaterialName = new System.Windows.Forms.TextBox();
            this.btnBTPRemoveMaterial = new System.Windows.Forms.Button();
            this.btnBTPAddMaterial = new System.Windows.Forms.Button();
            this.btnBTPRemoveFrame = new System.Windows.Forms.Button();
            this.btnSaveBTP = new System.Windows.Forms.Button();
            this.btnBTPAddFrame = new System.Windows.Forms.Button();
            this.btnMatPreview = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBTPMatNumFrameChanges = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtBTPMatStartOffsetFrameChanges = new System.Windows.Forms.TextBox();
            this.lbxBTPMaterials = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbxBTPFrames = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBTPFrameTexID = new System.Windows.Forms.TextBox();
            this.txtBTPFrameLength = new System.Windows.Forms.TextBox();
            this.txtBTPFramePalID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.rbTexAllInBMD = new System.Windows.Forms.RadioButton();
            this.rbTexAsRefInBTP = new System.Windows.Forms.RadioButton();
            this.btnBTPAddTexture = new System.Windows.Forms.Button();
            this.btnBTPRemoveTexture = new System.Windows.Forms.Button();
            this.btnBTPRemovePalette = new System.Windows.Forms.Button();
            this.btnBTPAddPalette = new System.Windows.Forms.Button();
            this.txtBTPTextureName = new System.Windows.Forms.TextBox();
            this.txtBTPPaletteName = new System.Windows.Forms.TextBox();
            this.btnBTPRenameTexture = new System.Windows.Forms.Button();
            this.btnBTPRenamePalette = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkCompressReplacedTextures = new System.Windows.Forms.CheckBox();
            //((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTexture
            // 
            this.lblTexture.AutoSize = true;
            this.lblTexture.Location = new System.Drawing.Point(3, 0);
            this.lblTexture.Name = "lblTexture";
            this.lblTexture.Size = new System.Drawing.Size(46, 13);
            this.lblTexture.TabIndex = 0;
            this.lblTexture.Text = "Texture:";
            // 
            // lbxTextures
            // 
            this.lbxTextures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxTextures.FormattingEnabled = true;
            this.lbxTextures.Location = new System.Drawing.Point(6, 16);
            this.lbxTextures.Name = "lbxTextures";
            this.lbxTextures.Size = new System.Drawing.Size(200, 199);
            this.lbxTextures.TabIndex = 1;
            this.lbxTextures.SelectedIndexChanged += new System.EventHandler(this.lbxTextures_SelectedIndexChanged);
            // 
            // pbxTexture
            // 
            this.pbxTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxTexture.Location = new System.Drawing.Point(416, 16);
            this.pbxTexture.Name = "pbxTexture";
            this.pbxTexture.Size = new System.Drawing.Size(234, 199);
            this.pbxTexture.TabIndex = 2;
            this.pbxTexture.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(413, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Preview:";
            // 
            // lblPalette
            // 
            this.lblPalette.AutoSize = true;
            this.lblPalette.Location = new System.Drawing.Point(208, 0);
            this.lblPalette.Name = "lblPalette";
            this.lblPalette.Size = new System.Drawing.Size(43, 13);
            this.lblPalette.TabIndex = 9;
            this.lblPalette.Text = "Palette:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnReplaceSelected,
            this.toolStripSeparator1,
            this.btnExport,
            this.btnExportAll,
            this.toolStripSeparator2,
            this.btnLoadBTP});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(659, 25);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Enabled = false;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReplaceSelected
            // 
            this.btnReplaceSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReplaceSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReplaceSelected.Name = "btnReplaceSelected";
            this.btnReplaceSelected.Size = new System.Drawing.Size(99, 22);
            this.btnReplaceSelected.Text = "Replace Selected";
            this.btnReplaceSelected.Click += new System.EventHandler(this.btnReplaceSelected_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(44, 22);
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnExportAll
            // 
            this.btnExportAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.Size = new System.Drawing.Size(61, 22);
            this.btnExportAll.Text = "Export All";
            this.btnExportAll.Click += new System.EventHandler(this.btnExportAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnLoadBTP
            // 
            this.btnLoadBTP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadBTP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadBTP.Name = "btnLoadBTP";
            this.btnLoadBTP.Size = new System.Drawing.Size(61, 22);
            this.btnLoadBTP.Text = "Load BTP";
            this.btnLoadBTP.Click += new System.EventHandler(this.btnLoadBTP_Click);
            // 
            // lbxPalettes
            // 
            this.lbxPalettes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxPalettes.FormattingEnabled = true;
            this.lbxPalettes.Location = new System.Drawing.Point(211, 16);
            this.lbxPalettes.Name = "lbxPalettes";
            this.lbxPalettes.Size = new System.Drawing.Size(200, 199);
            this.lbxPalettes.TabIndex = 12;
            this.lbxPalettes.SelectedIndexChanged += new System.EventHandler(this.lbxPalettes_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnMatPreviewStop);
            this.groupBox1.Controls.Add(this.txtBTPMaterialName);
            this.groupBox1.Controls.Add(this.btnBTPRemoveMaterial);
            this.groupBox1.Controls.Add(this.btnBTPAddMaterial);
            this.groupBox1.Controls.Add(this.btnBTPRemoveFrame);
            this.groupBox1.Controls.Add(this.btnSaveBTP);
            this.groupBox1.Controls.Add(this.btnBTPAddFrame);
            this.groupBox1.Controls.Add(this.btnMatPreview);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtBTPMatNumFrameChanges);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtBTPMatStartOffsetFrameChanges);
            this.groupBox1.Controls.Add(this.lbxBTPMaterials);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lbxBTPFrames);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtBTPFrameTexID);
            this.groupBox1.Controls.Add(this.txtBTPFrameLength);
            this.groupBox1.Controls.Add(this.txtBTPFramePalID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(659, 206);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BTP (Texture Sequence Animation)";
            // 
            // btnMatPreviewStop
            // 
            this.btnMatPreviewStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMatPreviewStop.Enabled = false;
            this.btnMatPreviewStop.Location = new System.Drawing.Point(562, 133);
            this.btnMatPreviewStop.Name = "btnMatPreviewStop";
            this.btnMatPreviewStop.Size = new System.Drawing.Size(42, 23);
            this.btnMatPreviewStop.TabIndex = 27;
            this.btnMatPreviewStop.Text = "Stop";
            this.btnMatPreviewStop.UseVisualStyleBackColor = true;
            this.btnMatPreviewStop.Click += new System.EventHandler(this.btnMatPreviewStop_Click);
            // 
            // txtBTPMaterialName
            // 
            this.txtBTPMaterialName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBTPMaterialName.Location = new System.Drawing.Point(376, 164);
            this.txtBTPMaterialName.Name = "txtBTPMaterialName";
            this.txtBTPMaterialName.Size = new System.Drawing.Size(85, 20);
            this.txtBTPMaterialName.TabIndex = 23;
            // 
            // btnBTPRemoveMaterial
            // 
            this.btnBTPRemoveMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRemoveMaterial.Location = new System.Drawing.Point(347, 162);
            this.btnBTPRemoveMaterial.Name = "btnBTPRemoveMaterial";
            this.btnBTPRemoveMaterial.Size = new System.Drawing.Size(23, 23);
            this.btnBTPRemoveMaterial.TabIndex = 26;
            this.btnBTPRemoveMaterial.Text = "-";
            this.btnBTPRemoveMaterial.UseVisualStyleBackColor = true;
            this.btnBTPRemoveMaterial.Click += new System.EventHandler(this.btnBTPRemoveMaterial_Click);
            // 
            // btnBTPAddMaterial
            // 
            this.btnBTPAddMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPAddMaterial.Location = new System.Drawing.Point(318, 162);
            this.btnBTPAddMaterial.Name = "btnBTPAddMaterial";
            this.btnBTPAddMaterial.Size = new System.Drawing.Size(23, 23);
            this.btnBTPAddMaterial.TabIndex = 25;
            this.btnBTPAddMaterial.Text = "+";
            this.btnBTPAddMaterial.UseVisualStyleBackColor = true;
            this.btnBTPAddMaterial.Click += new System.EventHandler(this.btnBTPAddMaterial_Click);
            // 
            // btnBTPRemoveFrame
            // 
            this.btnBTPRemoveFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRemoveFrame.Location = new System.Drawing.Point(35, 162);
            this.btnBTPRemoveFrame.Name = "btnBTPRemoveFrame";
            this.btnBTPRemoveFrame.Size = new System.Drawing.Size(23, 23);
            this.btnBTPRemoveFrame.TabIndex = 24;
            this.btnBTPRemoveFrame.Text = "-";
            this.btnBTPRemoveFrame.UseVisualStyleBackColor = true;
            this.btnBTPRemoveFrame.Click += new System.EventHandler(this.btnBTPRemoveFrame_Click);
            // 
            // btnSaveBTP
            // 
            this.btnSaveBTP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveBTP.Location = new System.Drawing.Point(487, 162);
            this.btnSaveBTP.Name = "btnSaveBTP";
            this.btnSaveBTP.Size = new System.Drawing.Size(117, 23);
            this.btnSaveBTP.TabIndex = 17;
            this.btnSaveBTP.Text = "Save BTP";
            this.btnSaveBTP.UseVisualStyleBackColor = true;
            this.btnSaveBTP.Click += new System.EventHandler(this.btnSaveBTP_Click);
            // 
            // btnBTPAddFrame
            // 
            this.btnBTPAddFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPAddFrame.Location = new System.Drawing.Point(6, 162);
            this.btnBTPAddFrame.Name = "btnBTPAddFrame";
            this.btnBTPAddFrame.Size = new System.Drawing.Size(23, 23);
            this.btnBTPAddFrame.TabIndex = 23;
            this.btnBTPAddFrame.Text = "+";
            this.btnBTPAddFrame.UseVisualStyleBackColor = true;
            this.btnBTPAddFrame.Click += new System.EventHandler(this.btnBTPAddFrame_Click);
            // 
            // btnMatPreview
            // 
            this.btnMatPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMatPreview.Location = new System.Drawing.Point(487, 133);
            this.btnMatPreview.Name = "btnMatPreview";
            this.btnMatPreview.Size = new System.Drawing.Size(69, 23);
            this.btnMatPreview.TabIndex = 16;
            this.btnMatPreview.Text = "Preview";
            this.btnMatPreview.UseVisualStyleBackColor = true;
            this.btnMatPreview.Click += new System.EventHandler(this.btnMatPreview_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(484, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Num. Frame Changes:";
            // 
            // txtBTPMatNumFrameChanges
            // 
            this.txtBTPMatNumFrameChanges.Location = new System.Drawing.Point(487, 90);
            this.txtBTPMatNumFrameChanges.Name = "txtBTPMatNumFrameChanges";
            this.txtBTPMatNumFrameChanges.Size = new System.Drawing.Size(117, 20);
            this.txtBTPMatNumFrameChanges.TabIndex = 15;
            this.txtBTPMatNumFrameChanges.TextChanged += new System.EventHandler(this.txtBTPMatNumFrameChanges_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(484, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Material Settings:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(484, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Frame Start Offset:";
            // 
            // txtBTPMatStartOffsetFrameChanges
            // 
            this.txtBTPMatStartOffsetFrameChanges.Location = new System.Drawing.Point(487, 51);
            this.txtBTPMatStartOffsetFrameChanges.Name = "txtBTPMatStartOffsetFrameChanges";
            this.txtBTPMatStartOffsetFrameChanges.Size = new System.Drawing.Size(117, 20);
            this.txtBTPMatStartOffsetFrameChanges.TabIndex = 12;
            this.txtBTPMatStartOffsetFrameChanges.TextChanged += new System.EventHandler(this.txtBTPMatStartOffsetFrameChanges_TextChanged);
            // 
            // lbxBTPMaterials
            // 
            this.lbxBTPMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxBTPMaterials.FormattingEnabled = true;
            this.lbxBTPMaterials.Location = new System.Drawing.Point(318, 35);
            this.lbxBTPMaterials.Name = "lbxBTPMaterials";
            this.lbxBTPMaterials.Size = new System.Drawing.Size(143, 121);
            this.lbxBTPMaterials.TabIndex = 10;
            this.lbxBTPMaterials.SelectedIndexChanged += new System.EventHandler(this.lbxBTPMaterials_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(315, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Material:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Frame Change:";
            // 
            // lbxBTPFrames
            // 
            this.lbxBTPFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxBTPFrames.FormattingEnabled = true;
            this.lbxBTPFrames.Location = new System.Drawing.Point(6, 35);
            this.lbxBTPFrames.Name = "lbxBTPFrames";
            this.lbxBTPFrames.Size = new System.Drawing.Size(143, 121);
            this.lbxBTPFrames.TabIndex = 0;
            this.lbxBTPFrames.SelectedIndexChanged += new System.EventHandler(this.lbxBTPFrames_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(171, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Frame Change Settings:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Texture ID:";
            // 
            // txtBTPFrameTexID
            // 
            this.txtBTPFrameTexID.Location = new System.Drawing.Point(174, 51);
            this.txtBTPFrameTexID.Name = "txtBTPFrameTexID";
            this.txtBTPFrameTexID.Size = new System.Drawing.Size(117, 20);
            this.txtBTPFrameTexID.TabIndex = 3;
            this.txtBTPFrameTexID.TextChanged += new System.EventHandler(this.txtBTPFrameTexID_TextChanged);
            // 
            // txtBTPFrameLength
            // 
            this.txtBTPFrameLength.Location = new System.Drawing.Point(174, 129);
            this.txtBTPFrameLength.Name = "txtBTPFrameLength";
            this.txtBTPFrameLength.Size = new System.Drawing.Size(117, 20);
            this.txtBTPFrameLength.TabIndex = 7;
            this.txtBTPFrameLength.TextChanged += new System.EventHandler(this.txtBTPFrameLength_TextChanged);
            // 
            // txtBTPFramePalID
            // 
            this.txtBTPFramePalID.Location = new System.Drawing.Point(174, 90);
            this.txtBTPFramePalID.Name = "txtBTPFramePalID";
            this.txtBTPFramePalID.Size = new System.Drawing.Size(117, 20);
            this.txtBTPFramePalID.TabIndex = 5;
            this.txtBTPFramePalID.TextChanged += new System.EventHandler(this.txtBTPFramePalID_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(171, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Palette ID:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(171, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Frame Length:";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 245);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Show Textures:";
            // 
            // rbTexAllInBMD
            // 
            this.rbTexAllInBMD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbTexAllInBMD.AutoSize = true;
            this.rbTexAllInBMD.Checked = true;
            this.rbTexAllInBMD.Location = new System.Drawing.Point(90, 243);
            this.rbTexAllInBMD.Name = "rbTexAllInBMD";
            this.rbTexAllInBMD.Size = new System.Drawing.Size(74, 17);
            this.rbTexAllInBMD.TabIndex = 15;
            this.rbTexAllInBMD.TabStop = true;
            this.rbTexAllInBMD.Text = "All in BMD";
            this.rbTexAllInBMD.UseVisualStyleBackColor = true;
            this.rbTexAllInBMD.CheckedChanged += new System.EventHandler(this.rbTexAllInBMD_CheckedChanged);
            // 
            // rbTexAsRefInBTP
            // 
            this.rbTexAsRefInBTP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbTexAsRefInBTP.AutoSize = true;
            this.rbTexAsRefInBTP.Enabled = false;
            this.rbTexAsRefInBTP.Location = new System.Drawing.Point(170, 243);
            this.rbTexAsRefInBTP.Name = "rbTexAsRefInBTP";
            this.rbTexAsRefInBTP.Size = new System.Drawing.Size(201, 17);
            this.rbTexAsRefInBTP.TabIndex = 16;
            this.rbTexAsRefInBTP.Text = "As Referenced in BTP (Different ID\'s)";
            this.rbTexAsRefInBTP.UseVisualStyleBackColor = true;
            this.rbTexAsRefInBTP.CheckedChanged += new System.EventHandler(this.rbTexAsRefInBTP_CheckedChanged);
            // 
            // btnBTPAddTexture
            // 
            this.btnBTPAddTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPAddTexture.Enabled = false;
            this.btnBTPAddTexture.Location = new System.Drawing.Point(6, 216);
            this.btnBTPAddTexture.Name = "btnBTPAddTexture";
            this.btnBTPAddTexture.Size = new System.Drawing.Size(23, 25);
            this.btnBTPAddTexture.TabIndex = 17;
            this.btnBTPAddTexture.Text = "+";
            this.btnBTPAddTexture.UseVisualStyleBackColor = true;
            this.btnBTPAddTexture.Visible = false;
            this.btnBTPAddTexture.Click += new System.EventHandler(this.btnBTPAddTexture_Click);
            // 
            // btnBTPRemoveTexture
            // 
            this.btnBTPRemoveTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRemoveTexture.Enabled = false;
            this.btnBTPRemoveTexture.Location = new System.Drawing.Point(35, 216);
            this.btnBTPRemoveTexture.Name = "btnBTPRemoveTexture";
            this.btnBTPRemoveTexture.Size = new System.Drawing.Size(23, 25);
            this.btnBTPRemoveTexture.TabIndex = 18;
            this.btnBTPRemoveTexture.Text = "-";
            this.btnBTPRemoveTexture.UseVisualStyleBackColor = true;
            this.btnBTPRemoveTexture.Visible = false;
            this.btnBTPRemoveTexture.Click += new System.EventHandler(this.btnBTPRemoveTexture_Click);
            // 
            // btnBTPRemovePalette
            // 
            this.btnBTPRemovePalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRemovePalette.Enabled = false;
            this.btnBTPRemovePalette.Location = new System.Drawing.Point(240, 216);
            this.btnBTPRemovePalette.Name = "btnBTPRemovePalette";
            this.btnBTPRemovePalette.Size = new System.Drawing.Size(23, 25);
            this.btnBTPRemovePalette.TabIndex = 20;
            this.btnBTPRemovePalette.Text = "-";
            this.btnBTPRemovePalette.UseVisualStyleBackColor = true;
            this.btnBTPRemovePalette.Visible = false;
            this.btnBTPRemovePalette.Click += new System.EventHandler(this.btnBTPRemovePalette_Click);
            // 
            // btnBTPAddPalette
            // 
            this.btnBTPAddPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPAddPalette.Enabled = false;
            this.btnBTPAddPalette.Location = new System.Drawing.Point(211, 216);
            this.btnBTPAddPalette.Name = "btnBTPAddPalette";
            this.btnBTPAddPalette.Size = new System.Drawing.Size(23, 25);
            this.btnBTPAddPalette.TabIndex = 19;
            this.btnBTPAddPalette.Text = "+";
            this.btnBTPAddPalette.UseVisualStyleBackColor = true;
            this.btnBTPAddPalette.Visible = false;
            this.btnBTPAddPalette.Click += new System.EventHandler(this.btnBTPAddPalette_Click);
            // 
            // txtBTPTextureName
            // 
            this.txtBTPTextureName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBTPTextureName.Enabled = false;
            this.txtBTPTextureName.Location = new System.Drawing.Point(90, 217);
            this.txtBTPTextureName.Name = "txtBTPTextureName";
            this.txtBTPTextureName.Size = new System.Drawing.Size(116, 20);
            this.txtBTPTextureName.TabIndex = 21;
            this.txtBTPTextureName.Visible = false;
            // 
            // txtBTPPaletteName
            // 
            this.txtBTPPaletteName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBTPPaletteName.Enabled = false;
            this.txtBTPPaletteName.Location = new System.Drawing.Point(295, 218);
            this.txtBTPPaletteName.Name = "txtBTPPaletteName";
            this.txtBTPPaletteName.Size = new System.Drawing.Size(116, 20);
            this.txtBTPPaletteName.TabIndex = 22;
            this.txtBTPPaletteName.Visible = false;
            // 
            // btnBTPRenameTexture
            // 
            this.btnBTPRenameTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRenameTexture.Enabled = false;
            this.btnBTPRenameTexture.Location = new System.Drawing.Point(64, 216);
            this.btnBTPRenameTexture.Name = "btnBTPRenameTexture";
            this.btnBTPRenameTexture.Size = new System.Drawing.Size(23, 25);
            this.btnBTPRenameTexture.TabIndex = 23;
            this.btnBTPRenameTexture.Text = ".I";
            this.btnBTPRenameTexture.UseVisualStyleBackColor = true;
            this.btnBTPRenameTexture.Visible = false;
            this.btnBTPRenameTexture.Click += new System.EventHandler(this.btnBTPRenameTexture_Click);
            // 
            // btnBTPRenamePalette
            // 
            this.btnBTPRenamePalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBTPRenamePalette.Enabled = false;
            this.btnBTPRenamePalette.Location = new System.Drawing.Point(269, 216);
            this.btnBTPRenamePalette.Name = "btnBTPRenamePalette";
            this.btnBTPRenamePalette.Size = new System.Drawing.Size(23, 25);
            this.btnBTPRenamePalette.TabIndex = 24;
            this.btnBTPRenamePalette.Text = ".I";
            this.btnBTPRenamePalette.UseVisualStyleBackColor = true;
            this.btnBTPRenamePalette.Visible = false;
            this.btnBTPRenamePalette.Click += new System.EventHandler(this.btnBTPRenamePalette_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkCompressReplacedTextures);
            this.splitContainer1.Panel1.Controls.Add(this.lblTexture);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPRenamePalette);
            this.splitContainer1.Panel1.Controls.Add(this.pbxTexture);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPRenameTexture);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.txtBTPPaletteName);
            this.splitContainer1.Panel1.Controls.Add(this.lblPalette);
            this.splitContainer1.Panel1.Controls.Add(this.txtBTPTextureName);
            this.splitContainer1.Panel1.Controls.Add(this.lbxTextures);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPRemovePalette);
            this.splitContainer1.Panel1.Controls.Add(this.lbxPalettes);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPAddPalette);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPRemoveTexture);
            this.splitContainer1.Panel1.Controls.Add(this.rbTexAllInBMD);
            this.splitContainer1.Panel1.Controls.Add(this.btnBTPAddTexture);
            this.splitContainer1.Panel1.Controls.Add(this.rbTexAsRefInBTP);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(659, 481);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 25;
            // 
            // chkCompressReplacedTextures
            // 
            this.chkCompressReplacedTextures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCompressReplacedTextures.AutoSize = true;
            this.chkCompressReplacedTextures.Location = new System.Drawing.Point(416, 244);
            this.chkCompressReplacedTextures.Name = "chkCompressReplacedTextures";
            this.chkCompressReplacedTextures.Size = new System.Drawing.Size(165, 17);
            this.chkCompressReplacedTextures.TabIndex = 25;
            this.chkCompressReplacedTextures.Text = "Compress Replaced Textures";
            this.chkCompressReplacedTextures.UseVisualStyleBackColor = true;
            // 
            // TextureEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 506);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextureEditorForm";
            this.Text = "Texture and BTP Editor";
            this.Load += new System.EventHandler(TextureEditorForm_Load);
            //((System.ComponentModel.ISupportInitialize)(this.pbxTexture)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTexture;
        private System.Windows.Forms.ListBox lbxTextures;
        private System.Windows.Forms.PictureBox pbxTexture;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPalette;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoadBTP;
        private System.Windows.Forms.ListBox lbxPalettes;
        private System.Windows.Forms.ToolStripButton btnReplaceSelected;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripButton btnExportAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbxBTPFrames;
        private System.Windows.Forms.TextBox txtBTPFrameLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBTPFramePalID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBTPFrameTexID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnMatPreview;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtBTPMatNumFrameChanges;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtBTPMatStartOffsetFrameChanges;
        private System.Windows.Forms.ListBox lbxBTPMaterials;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSaveBTP;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton rbTexAllInBMD;
        private System.Windows.Forms.RadioButton rbTexAsRefInBTP;
        private System.Windows.Forms.Button btnBTPAddTexture;
        private System.Windows.Forms.Button btnBTPRemoveTexture;
        private System.Windows.Forms.Button btnBTPRemovePalette;
        private System.Windows.Forms.Button btnBTPAddPalette;
        private System.Windows.Forms.TextBox txtBTPTextureName;
        private System.Windows.Forms.TextBox txtBTPPaletteName;
        private System.Windows.Forms.Button btnBTPRemoveMaterial;
        private System.Windows.Forms.Button btnBTPAddMaterial;
        private System.Windows.Forms.Button btnBTPRemoveFrame;
        private System.Windows.Forms.Button btnBTPAddFrame;
        private System.Windows.Forms.TextBox txtBTPMaterialName;
        private System.Windows.Forms.Button btnMatPreviewStop;
        private System.Windows.Forms.Button btnBTPRenameTexture;
        private System.Windows.Forms.Button btnBTPRenamePalette;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkCompressReplacedTextures;
    }
}