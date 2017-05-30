namespace SM64DSe
{
    partial class ModelImporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelImporter));
            this.tsToolBar = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tbModelName = new System.Windows.Forms.ToolStripTextBox();
            this.btnOpenModel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnEditTextures = new System.Windows.Forms.ToolStripButton();
            this.ssStatusBar = new System.Windows.Forms.StatusStrip();
            this.slStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.glModelView = new FormControls.ModelGLControlWithMarioSizeReference();
            this.spcMainContainer = new System.Windows.Forms.SplitContainer();
            this.txtInGameSizePreview = new System.Windows.Forms.TextBox();
            this.chkInGamePreview = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAssignTypes = new System.Windows.Forms.Button();
            this.gridColTypes = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAlwaysCompress = new System.Windows.Forms.RadioButton();
            this.rbNeverCompress = new System.Windows.Forms.RadioButton();
            this.rbBetterQualityWhereSensible = new System.Windows.Forms.RadioButton();
            this.chkVFlipAllTextures = new System.Windows.Forms.CheckBox();
            this.chkKeepVertexOrderDuringStripping = new System.Windows.Forms.CheckBox();
            this.chkStripify = new System.Windows.Forms.CheckBox();
            this.chkAlwaysWriteFullVertexCmd23h = new System.Windows.Forms.CheckBox();
            this.lbl02 = new System.Windows.Forms.Label();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.lbl01 = new System.Windows.Forms.Label();
            this.cbGenerateCollision = new System.Windows.Forms.CheckBox();
            this.lblScale = new System.Windows.Forms.Label();
            this.tbScale = new System.Windows.Forms.TextBox();
            this.ofdLoadModel = new System.Windows.Forms.OpenFileDialog();
            this.btnClearTypes = new System.Windows.Forms.Button();
            this.tsToolBar.SuspendLayout();
            this.ssStatusBar.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).BeginInit();
            this.spcMainContainer.Panel1.SuspendLayout();
            this.spcMainContainer.Panel2.SuspendLayout();
            this.spcMainContainer.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridColTypes)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsToolBar
            // 
            this.tsToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tbModelName,
            this.btnOpenModel,
            this.toolStripSeparator1,
            this.btnImport,
            this.btnEditTextures});
            this.tsToolBar.Location = new System.Drawing.Point(0, 0);
            this.tsToolBar.Name = "tsToolBar";
            this.tsToolBar.Size = new System.Drawing.Size(787, 25);
            this.tsToolBar.TabIndex = 0;
            this.tsToolBar.Text = "loluseless";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel1.Text = "Model:";
            // 
            // tbModelName
            // 
            this.tbModelName.Name = "tbModelName";
            this.tbModelName.ReadOnly = true;
            this.tbModelName.Size = new System.Drawing.Size(200, 25);
            this.tbModelName.Text = "<model name here>";
            // 
            // btnOpenModel
            // 
            this.btnOpenModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpenModel.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenModel.Image")));
            this.btnOpenModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenModel.Name = "btnOpenModel";
            this.btnOpenModel.Size = new System.Drawing.Size(49, 22);
            this.btnOpenModel.Text = "Open...";
            this.btnOpenModel.Click += new System.EventHandler(this.btnOpenModel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImport.Image = ((System.Drawing.Image)(resources.GetObject("btnImport.Image")));
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(47, 22);
            this.btnImport.Text = "Import";
            this.btnImport.ToolTipText = "Import the thing into the game!";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnEditTextures
            // 
            this.btnEditTextures.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnEditTextures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditTextures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditTextures.Name = "btnEditTextures";
            this.btnEditTextures.Size = new System.Drawing.Size(131, 22);
            this.btnEditTextures.Text = "Texture and BTP Editor";
            this.btnEditTextures.Click += new System.EventHandler(this.btnEditTextures_Click);
            // 
            // ssStatusBar
            // 
            this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slStatus});
            this.ssStatusBar.Location = new System.Drawing.Point(0, 535);
            this.ssStatusBar.Name = "ssStatusBar";
            this.ssStatusBar.Size = new System.Drawing.Size(787, 22);
            this.ssStatusBar.TabIndex = 1;
            this.ssStatusBar.Text = "loluseless";
            // 
            // slStatus
            // 
            this.slStatus.Name = "slStatus";
            this.slStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // glModelView
            // 
            this.glModelView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glModelView.Margin = new System.Windows.Forms.Padding(0);
            this.glModelView.Name = "glModelView";
            this.glModelView.TabIndex = 2;
            this.glModelView.BackColor = System.Drawing.Color.Black;
            this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glModelView.Location = new System.Drawing.Point(0, 0);
            this.glModelView.VSync = false;
            // 
            // spcMainContainer
            // 
            this.spcMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spcMainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcMainContainer.IsSplitterFixed = true;
            this.spcMainContainer.Location = new System.Drawing.Point(0, 25);
            this.spcMainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.spcMainContainer.Name = "spcMainContainer";
            // 
            // spcMainContainer.Panel1
            // 
            this.spcMainContainer.Panel1.Controls.Add(this.btnClearTypes);
            this.spcMainContainer.Panel1.Controls.Add(this.txtInGameSizePreview);
            this.spcMainContainer.Panel1.Controls.Add(this.chkInGamePreview);
            this.spcMainContainer.Panel1.Controls.Add(this.label1);
            this.spcMainContainer.Panel1.Controls.Add(this.btnAssignTypes);
            this.spcMainContainer.Panel1.Controls.Add(this.gridColTypes);
            this.spcMainContainer.Panel1.Controls.Add(this.groupBox1);
            this.spcMainContainer.Panel1MinSize = 30;
            // 
            // spcMainContainer.Panel2
            // 
            this.spcMainContainer.Panel2.Controls.Add(this.glModelView);
            this.spcMainContainer.Size = new System.Drawing.Size(787, 510);
            this.spcMainContainer.SplitterDistance = 260;
            this.spcMainContainer.TabIndex = 3;
            // 
            // txtInGameSizePreview
            // 
            this.txtInGameSizePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInGameSizePreview.Location = new System.Drawing.Point(153, 487);
            this.txtInGameSizePreview.Name = "txtInGameSizePreview";
            this.txtInGameSizePreview.Size = new System.Drawing.Size(90, 20);
            this.txtInGameSizePreview.TabIndex = 13;
            this.txtInGameSizePreview.TextChanged += new System.EventHandler(this.txtInGameSizePreview_TextChanged);
            // 
            // chkInGamePreview
            // 
            this.chkInGamePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkInGamePreview.AutoSize = true;
            this.chkInGamePreview.Checked = true;
            this.chkInGamePreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInGamePreview.Location = new System.Drawing.Point(3, 490);
            this.chkInGamePreview.Name = "chkInGamePreview";
            this.chkInGamePreview.Size = new System.Drawing.Size(130, 17);
            this.chkInGamePreview.TabIndex = 12;
            this.chkInGamePreview.Text = "In-Game Size Preview";
            this.chkInGamePreview.UseVisualStyleBackColor = true;
            this.chkInGamePreview.CheckedChanged += new System.EventHandler(this.chkInGamePreview_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 294);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Per Material Collision Types:";
            // 
            // btnAssignTypes
            // 
            this.btnAssignTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAssignTypes.Location = new System.Drawing.Point(3, 461);
            this.btnAssignTypes.Name = "btnAssignTypes";
            this.btnAssignTypes.Size = new System.Drawing.Size(87, 23);
            this.btnAssignTypes.TabIndex = 9;
            this.btnAssignTypes.Text = "Assign Types";
            this.btnAssignTypes.UseVisualStyleBackColor = true;
            this.btnAssignTypes.Click += new System.EventHandler(this.btnAssignTypes_Click);
            // 
            // gridColTypes
            // 
            this.gridColTypes.AllowUserToAddRows = false;
            this.gridColTypes.AllowUserToDeleteRows = false;
            this.gridColTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridColTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridColTypes.Location = new System.Drawing.Point(3, 310);
            this.gridColTypes.Name = "gridColTypes";
            this.gridColTypes.RowHeadersVisible = false;
            this.gridColTypes.Size = new System.Drawing.Size(249, 145);
            this.gridColTypes.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.chkVFlipAllTextures);
            this.groupBox1.Controls.Add(this.chkKeepVertexOrderDuringStripping);
            this.groupBox1.Controls.Add(this.chkStripify);
            this.groupBox1.Controls.Add(this.chkAlwaysWriteFullVertexCmd23h);
            this.groupBox1.Controls.Add(this.lbl02);
            this.groupBox1.Controls.Add(this.txtThreshold);
            this.groupBox1.Controls.Add(this.lbl01);
            this.groupBox1.Controls.Add(this.cbGenerateCollision);
            this.groupBox1.Controls.Add(this.lblScale);
            this.groupBox1.Controls.Add(this.tbScale);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(254, 288);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Model settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbAlwaysCompress);
            this.groupBox2.Controls.Add(this.rbNeverCompress);
            this.groupBox2.Controls.Add(this.rbBetterQualityWhereSensible);
            this.groupBox2.Location = new System.Drawing.Point(12, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 82);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Texture Quality Setting";
            // 
            // rbAlwaysCompress
            // 
            this.rbAlwaysCompress.AutoSize = true;
            this.rbAlwaysCompress.Checked = true;
            this.rbAlwaysCompress.Location = new System.Drawing.Point(26, 17);
            this.rbAlwaysCompress.Name = "rbAlwaysCompress";
            this.rbAlwaysCompress.Size = new System.Drawing.Size(87, 17);
            this.rbAlwaysCompress.TabIndex = 0;
            this.rbAlwaysCompress.TabStop = true;
            this.rbAlwaysCompress.Text = "Smallest Size";
            this.rbAlwaysCompress.UseVisualStyleBackColor = true;
            this.rbAlwaysCompress.CheckedChanged += new System.EventHandler(this.rbAlwaysCompress_CheckedChanged);
            // 
            // rbNeverCompress
            // 
            this.rbNeverCompress.AutoSize = true;
            this.rbNeverCompress.Location = new System.Drawing.Point(26, 63);
            this.rbNeverCompress.Name = "rbNeverCompress";
            this.rbNeverCompress.Size = new System.Drawing.Size(81, 17);
            this.rbNeverCompress.TabIndex = 2;
            this.rbNeverCompress.TabStop = true;
            this.rbNeverCompress.Text = "Best Quality";
            this.rbNeverCompress.UseVisualStyleBackColor = true;
            this.rbNeverCompress.CheckedChanged += new System.EventHandler(this.rbNeverCompress_CheckedChanged);
            // 
            // rbBetterQualityWhereSensible
            // 
            this.rbBetterQualityWhereSensible.AutoSize = true;
            this.rbBetterQualityWhereSensible.Location = new System.Drawing.Point(26, 40);
            this.rbBetterQualityWhereSensible.Name = "rbBetterQualityWhereSensible";
            this.rbBetterQualityWhereSensible.Size = new System.Drawing.Size(166, 17);
            this.rbBetterQualityWhereSensible.TabIndex = 1;
            this.rbBetterQualityWhereSensible.TabStop = true;
            this.rbBetterQualityWhereSensible.Text = "Better Quality Where Sensible";
            this.rbBetterQualityWhereSensible.UseVisualStyleBackColor = true;
            this.rbBetterQualityWhereSensible.CheckedChanged += new System.EventHandler(this.rbBetterQualityWhereSensible_CheckedChanged);
            // 
            // chkVFlipAllTextures
            // 
            this.chkVFlipAllTextures.AutoSize = true;
            this.chkVFlipAllTextures.Location = new System.Drawing.Point(12, 265);
            this.chkVFlipAllTextures.Name = "chkVFlipAllTextures";
            this.chkVFlipAllTextures.Size = new System.Drawing.Size(145, 17);
            this.chkVFlipAllTextures.TabIndex = 10;
            this.chkVFlipAllTextures.Text = "Vertically Flip All Textures";
            this.chkVFlipAllTextures.UseVisualStyleBackColor = true;
            this.chkVFlipAllTextures.CheckedChanged += new System.EventHandler(this.chkVFlipAllTextures_CheckedChanged);
            // 
            // chkKeepVertexOrderDuringStripping
            // 
            this.chkKeepVertexOrderDuringStripping.AutoSize = true;
            this.chkKeepVertexOrderDuringStripping.Location = new System.Drawing.Point(29, 130);
            this.chkKeepVertexOrderDuringStripping.Name = "chkKeepVertexOrderDuringStripping";
            this.chkKeepVertexOrderDuringStripping.Size = new System.Drawing.Size(220, 17);
            this.chkKeepVertexOrderDuringStripping.TabIndex = 7;
            this.chkKeepVertexOrderDuringStripping.Text = "Keep Vertex Order (Keep False if Unsure)";
            this.chkKeepVertexOrderDuringStripping.UseVisualStyleBackColor = true;
            this.chkKeepVertexOrderDuringStripping.CheckedChanged += new System.EventHandler(this.chkKeepVertexOrderDuringStripping_CheckedChanged);
            // 
            // chkStripify
            // 
            this.chkStripify.AutoSize = true;
            this.chkStripify.Checked = true;
            this.chkStripify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStripify.Location = new System.Drawing.Point(12, 107);
            this.chkStripify.Name = "chkStripify";
            this.chkStripify.Size = new System.Drawing.Size(241, 17);
            this.chkStripify.TabIndex = 6;
            this.chkStripify.Text = "Convert to Tri Strips (Strongly Recommended)";
            this.chkStripify.UseVisualStyleBackColor = true;
            this.chkStripify.CheckedChanged += new System.EventHandler(this.chkStripify_CheckedChanged);
            // 
            // chkAlwaysWriteFullVertexCmd23h
            // 
            this.chkAlwaysWriteFullVertexCmd23h.AutoSize = true;
            this.chkAlwaysWriteFullVertexCmd23h.Checked = true;
            this.chkAlwaysWriteFullVertexCmd23h.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlwaysWriteFullVertexCmd23h.Location = new System.Drawing.Point(12, 153);
            this.chkAlwaysWriteFullVertexCmd23h.Name = "chkAlwaysWriteFullVertexCmd23h";
            this.chkAlwaysWriteFullVertexCmd23h.Size = new System.Drawing.Size(219, 17);
            this.chkAlwaysWriteFullVertexCmd23h.TabIndex = 8;
            this.chkAlwaysWriteFullVertexCmd23h.Text = "Always Write Full X,Y,Z Vertex Command";
            this.chkAlwaysWriteFullVertexCmd23h.UseVisualStyleBackColor = true;
            this.chkAlwaysWriteFullVertexCmd23h.CheckedChanged += new System.EventHandler(this.chkAlwaysWriteFullVertexCmd23h_CheckedChanged);
            // 
            // lbl02
            // 
            this.lbl02.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbl02.Location = new System.Drawing.Point(26, 87);
            this.lbl02.Name = "lbl02";
            this.lbl02.Size = new System.Drawing.Size(119, 17);
            this.lbl02.TabIndex = 10;
            this.lbl02.Text = "Zero to keep all faces";
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(150, 70);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(86, 20);
            this.txtThreshold.TabIndex = 5;
            this.txtThreshold.TextChanged += new System.EventHandler(this.txtThreshold_TextChanged);
            // 
            // lbl01
            // 
            this.lbl01.AutoSize = true;
            this.lbl01.Location = new System.Drawing.Point(26, 73);
            this.lbl01.Name = "lbl01";
            this.lbl01.Size = new System.Drawing.Size(118, 13);
            this.lbl01.TabIndex = 8;
            this.lbl01.Text = "Drop faces smaller than";
            // 
            // cbGenerateCollision
            // 
            this.cbGenerateCollision.AutoSize = true;
            this.cbGenerateCollision.Location = new System.Drawing.Point(12, 49);
            this.cbGenerateCollision.Name = "cbGenerateCollision";
            this.cbGenerateCollision.Size = new System.Drawing.Size(133, 17);
            this.cbGenerateCollision.TabIndex = 4;
            this.cbGenerateCollision.Text = "Generate collision map";
            this.cbGenerateCollision.UseVisualStyleBackColor = true;
            this.cbGenerateCollision.CheckedChanged += new System.EventHandler(this.cbGenerateCollision_CheckedChanged);
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(9, 26);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(37, 13);
            this.lblScale.TabIndex = 0;
            this.lblScale.Text = "Scale:";
            // 
            // tbScale
            // 
            this.tbScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbScale.Location = new System.Drawing.Point(52, 23);
            this.tbScale.Name = "tbScale";
            this.tbScale.Size = new System.Drawing.Size(188, 20);
            this.tbScale.TabIndex = 3;
            this.tbScale.TextChanged += new System.EventHandler(this.tbScale_TextChanged);
            // 
            // ofdLoadModel
            // 
            this.ofdLoadModel.Filter = Strings.MODEL_FORMATS_FILTER;
            this.ofdLoadModel.Title = "Load model file...";
            // 
            // btnClearTypes
            // 
            this.btnClearTypes.Location = new System.Drawing.Point(177, 461);
            this.btnClearTypes.Name = "btnClearTypes";
            this.btnClearTypes.Size = new System.Drawing.Size(75, 23);
            this.btnClearTypes.TabIndex = 14;
            this.btnClearTypes.Text = "Clear";
            this.btnClearTypes.UseVisualStyleBackColor = true;
            this.btnClearTypes.Click += new System.EventHandler(this.btnClearTypes_Click);
            // 
            // ModelImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 557);
            this.Controls.Add(this.spcMainContainer);
            this.Controls.Add(this.ssStatusBar);
            this.Controls.Add(this.tsToolBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ModelImporter";
            this.Text = "Model importer - Super Mario 64 DS Editor lolol";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelImporter_FormClosed);
            this.Load += new System.EventHandler(this.ModelImporter_Load);
            this.tsToolBar.ResumeLayout(false);
            this.tsToolBar.PerformLayout();
            this.ssStatusBar.ResumeLayout(false);
            this.ssStatusBar.PerformLayout();
            this.spcMainContainer.Panel1.ResumeLayout(false);
            this.spcMainContainer.Panel1.PerformLayout();
            this.spcMainContainer.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).EndInit();
            this.spcMainContainer.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.gridColTypes)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsToolBar;
        private System.Windows.Forms.StatusStrip ssStatusBar;
        private FormControls.ModelGLControlWithMarioSizeReference glModelView;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnOpenModel;
        private System.Windows.Forms.SplitContainer spcMainContainer;
        private System.Windows.Forms.ToolStripTextBox tbModelName;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.OpenFileDialog ofdLoadModel;
        private System.Windows.Forms.TextBox tbScale;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel slStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbGenerateCollision;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.Label lbl01;
        private System.Windows.Forms.Label lbl02;
        private System.Windows.Forms.DataGridView gridColTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAssignTypes;
        private System.Windows.Forms.TextBox txtInGameSizePreview;
        private System.Windows.Forms.CheckBox chkInGamePreview;
        private System.Windows.Forms.CheckBox chkAlwaysWriteFullVertexCmd23h;
        private System.Windows.Forms.CheckBox chkStripify;
        private System.Windows.Forms.CheckBox chkKeepVertexOrderDuringStripping;
        private System.Windows.Forms.CheckBox chkVFlipAllTextures;
        private System.Windows.Forms.ToolStripButton btnEditTextures;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbAlwaysCompress;
        private System.Windows.Forms.RadioButton rbNeverCompress;
        private System.Windows.Forms.RadioButton rbBetterQualityWhereSensible;
        private System.Windows.Forms.Button btnClearTypes;
    }
}