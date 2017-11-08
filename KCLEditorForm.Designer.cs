namespace SM64DSe
{
    partial class KCLEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KCLEditorForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitCHorizontal = new System.Windows.Forms.SplitContainer();
            this.lbxPlanes = new System.Windows.Forms.ListBox();
            this.tabDetailsImport = new System.Windows.Forms.TabControl();
            this.tpgDetails = new System.Windows.Forms.TabPage();
            this.lblV1 = new System.Windows.Forms.Label();
            this.txtD3 = new System.Windows.Forms.TextBox();
            this.lblV2 = new System.Windows.Forms.Label();
            this.lblD3 = new System.Windows.Forms.Label();
            this.lblV3 = new System.Windows.Forms.Label();
            this.txtD2 = new System.Windows.Forms.TextBox();
            this.lblColType = new System.Windows.Forms.Label();
            this.txtD1 = new System.Windows.Forms.TextBox();
            this.txtColType = new System.Windows.Forms.TextBox();
            this.txtNormal = new System.Windows.Forms.TextBox();
            this.txtV1 = new System.Windows.Forms.TextBox();
            this.lblD2 = new System.Windows.Forms.Label();
            this.txtV2 = new System.Windows.Forms.TextBox();
            this.lblD1 = new System.Windows.Forms.Label();
            this.txtV3 = new System.Windows.Forms.TextBox();
            this.lblNormal = new System.Windows.Forms.Label();
            this.tpgImport = new System.Windows.Forms.TabPage();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnImportColMap = new System.Windows.Forms.Button();
            this.btnAssignTypes = new System.Windows.Forms.Button();
            this.gridColTypes = new System.Windows.Forms.DataGridView();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOpenModel = new System.Windows.Forms.Button();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.glModelView = new SM64DSe.FormControls.ModelGLControlWithPicking();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportKCLModel = new System.Windows.Forms.ToolStripButton();
            this.cmbPolygonMode = new System.Windows.Forms.ComboBox();
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splitCHorizontal)).BeginInit();
            this.splitCHorizontal.Panel1.SuspendLayout();
            this.splitCHorizontal.Panel2.SuspendLayout();
            this.splitCHorizontal.SuspendLayout();
            this.tabDetailsImport.SuspendLayout();
            this.tpgDetails.SuspendLayout();
            this.tpgImport.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridColTypes)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitCHorizontal);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.glModelView);
            this.splitContainer1.Size = new System.Drawing.Size(900, 523);
            this.splitContainer1.SplitterDistance = 246;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitCHorizontal
            // 
            this.splitCHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitCHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitCHorizontal.Name = "splitCHorizontal";
            this.splitCHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitCHorizontal.Panel1
            // 
            this.splitCHorizontal.Panel1.Controls.Add(this.lbxPlanes);
            // 
            // splitCHorizontal.Panel2
            // 
            this.splitCHorizontal.Panel2.Controls.Add(this.tabDetailsImport);
            this.splitCHorizontal.Size = new System.Drawing.Size(246, 523);
            this.splitCHorizontal.SplitterDistance = 245;
            this.splitCHorizontal.TabIndex = 18;
            // 
            // lbxPlanes
            // 
            this.lbxPlanes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxPlanes.FormattingEnabled = true;
            this.lbxPlanes.Location = new System.Drawing.Point(0, 0);
            this.lbxPlanes.Name = "lbxPlanes";
            this.lbxPlanes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxPlanes.Size = new System.Drawing.Size(246, 245);
            this.lbxPlanes.TabIndex = 0;
            this.lbxPlanes.SelectedIndexChanged += new System.EventHandler(this.lbxPlanes_SelectedIndexChanged);
            // 
            // tabDetailsImport
            // 
            this.tabDetailsImport.Controls.Add(this.tpgDetails);
            this.tabDetailsImport.Controls.Add(this.tpgImport);
            this.tabDetailsImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDetailsImport.Location = new System.Drawing.Point(0, 0);
            this.tabDetailsImport.Name = "tabDetailsImport";
            this.tabDetailsImport.SelectedIndex = 0;
            this.tabDetailsImport.Size = new System.Drawing.Size(246, 274);
            this.tabDetailsImport.TabIndex = 17;
            // 
            // tpgDetails
            // 
            this.tpgDetails.Controls.Add(this.lblV1);
            this.tpgDetails.Controls.Add(this.txtD3);
            this.tpgDetails.Controls.Add(this.lblV2);
            this.tpgDetails.Controls.Add(this.lblD3);
            this.tpgDetails.Controls.Add(this.lblV3);
            this.tpgDetails.Controls.Add(this.txtD2);
            this.tpgDetails.Controls.Add(this.lblColType);
            this.tpgDetails.Controls.Add(this.txtD1);
            this.tpgDetails.Controls.Add(this.txtColType);
            this.tpgDetails.Controls.Add(this.txtNormal);
            this.tpgDetails.Controls.Add(this.txtV1);
            this.tpgDetails.Controls.Add(this.lblD2);
            this.tpgDetails.Controls.Add(this.txtV2);
            this.tpgDetails.Controls.Add(this.lblD1);
            this.tpgDetails.Controls.Add(this.txtV3);
            this.tpgDetails.Controls.Add(this.lblNormal);
            this.tpgDetails.Location = new System.Drawing.Point(4, 22);
            this.tpgDetails.Name = "tpgDetails";
            this.tpgDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tpgDetails.Size = new System.Drawing.Size(238, 248);
            this.tpgDetails.TabIndex = 0;
            this.tpgDetails.Text = "Details";
            this.tpgDetails.UseVisualStyleBackColor = true;
            // 
            // lblV1
            // 
            this.lblV1.AutoSize = true;
            this.lblV1.Location = new System.Drawing.Point(7, 7);
            this.lblV1.Name = "lblV1";
            this.lblV1.Size = new System.Drawing.Size(46, 13);
            this.lblV1.TabIndex = 1;
            this.lblV1.Text = "Vertex 1";
            // 
            // txtD3
            // 
            this.txtD3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtD3.Location = new System.Drawing.Point(77, 138);
            this.txtD3.Name = "txtD3";
            this.txtD3.ReadOnly = true;
            this.txtD3.Size = new System.Drawing.Size(151, 20);
            this.txtD3.TabIndex = 16;
            // 
            // lblV2
            // 
            this.lblV2.AutoSize = true;
            this.lblV2.Location = new System.Drawing.Point(7, 29);
            this.lblV2.Name = "lblV2";
            this.lblV2.Size = new System.Drawing.Size(46, 13);
            this.lblV2.TabIndex = 2;
            this.lblV2.Text = "Vertex 2";
            // 
            // lblD3
            // 
            this.lblD3.AutoSize = true;
            this.lblD3.Location = new System.Drawing.Point(7, 141);
            this.lblD3.Name = "lblD3";
            this.lblD3.Size = new System.Drawing.Size(29, 13);
            this.lblD3.TabIndex = 15;
            this.lblD3.Text = "Dir 3";
            // 
            // lblV3
            // 
            this.lblV3.AutoSize = true;
            this.lblV3.Location = new System.Drawing.Point(7, 51);
            this.lblV3.Name = "lblV3";
            this.lblV3.Size = new System.Drawing.Size(46, 13);
            this.lblV3.TabIndex = 3;
            this.lblV3.Text = "Vertex 3";
            // 
            // txtD2
            // 
            this.txtD2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtD2.Location = new System.Drawing.Point(77, 115);
            this.txtD2.Name = "txtD2";
            this.txtD2.ReadOnly = true;
            this.txtD2.Size = new System.Drawing.Size(151, 20);
            this.txtD2.TabIndex = 14;
            // 
            // lblColType
            // 
            this.lblColType.AutoSize = true;
            this.lblColType.Location = new System.Drawing.Point(7, 164);
            this.lblColType.Name = "lblColType";
            this.lblColType.Size = new System.Drawing.Size(72, 13);
            this.lblColType.TabIndex = 4;
            this.lblColType.Text = "Collision Type";
            // 
            // txtD1
            // 
            this.txtD1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtD1.Location = new System.Drawing.Point(77, 93);
            this.txtD1.Name = "txtD1";
            this.txtD1.ReadOnly = true;
            this.txtD1.Size = new System.Drawing.Size(151, 20);
            this.txtD1.TabIndex = 13;
            // 
            // txtColType
            // 
            this.txtColType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColType.Location = new System.Drawing.Point(77, 161);
            this.txtColType.Name = "txtColType";
            this.txtColType.Size = new System.Drawing.Size(151, 20);
            this.txtColType.TabIndex = 5;
            this.txtColType.TextChanged += new System.EventHandler(this.txtColType_TextChanged);
            // 
            // txtNormal
            // 
            this.txtNormal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNormal.Location = new System.Drawing.Point(77, 71);
            this.txtNormal.Name = "txtNormal";
            this.txtNormal.ReadOnly = true;
            this.txtNormal.Size = new System.Drawing.Size(151, 20);
            this.txtNormal.TabIndex = 12;
            // 
            // txtV1
            // 
            this.txtV1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtV1.Location = new System.Drawing.Point(77, 4);
            this.txtV1.Name = "txtV1";
            this.txtV1.ReadOnly = true;
            this.txtV1.Size = new System.Drawing.Size(151, 20);
            this.txtV1.TabIndex = 6;
            // 
            // lblD2
            // 
            this.lblD2.AutoSize = true;
            this.lblD2.Location = new System.Drawing.Point(7, 118);
            this.lblD2.Name = "lblD2";
            this.lblD2.Size = new System.Drawing.Size(29, 13);
            this.lblD2.TabIndex = 11;
            this.lblD2.Text = "Dir 2";
            // 
            // txtV2
            // 
            this.txtV2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtV2.Location = new System.Drawing.Point(77, 26);
            this.txtV2.Name = "txtV2";
            this.txtV2.ReadOnly = true;
            this.txtV2.Size = new System.Drawing.Size(151, 20);
            this.txtV2.TabIndex = 7;
            // 
            // lblD1
            // 
            this.lblD1.AutoSize = true;
            this.lblD1.Location = new System.Drawing.Point(7, 98);
            this.lblD1.Name = "lblD1";
            this.lblD1.Size = new System.Drawing.Size(29, 13);
            this.lblD1.TabIndex = 10;
            this.lblD1.Text = "Dir 1";
            // 
            // txtV3
            // 
            this.txtV3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtV3.Location = new System.Drawing.Point(77, 48);
            this.txtV3.Name = "txtV3";
            this.txtV3.ReadOnly = true;
            this.txtV3.Size = new System.Drawing.Size(151, 20);
            this.txtV3.TabIndex = 8;
            // 
            // lblNormal
            // 
            this.lblNormal.AutoSize = true;
            this.lblNormal.Location = new System.Drawing.Point(7, 74);
            this.lblNormal.Name = "lblNormal";
            this.lblNormal.Size = new System.Drawing.Size(40, 13);
            this.lblNormal.TabIndex = 9;
            this.lblNormal.Text = "Normal";
            // 
            // tpgImport
            // 
            this.tpgImport.Controls.Add(this.txtScale);
            this.tpgImport.Controls.Add(this.label3);
            this.tpgImport.Controls.Add(this.btnImportColMap);
            this.tpgImport.Controls.Add(this.btnAssignTypes);
            this.tpgImport.Controls.Add(this.gridColTypes);
            this.tpgImport.Controls.Add(this.txtThreshold);
            this.tpgImport.Controls.Add(this.label2);
            this.tpgImport.Controls.Add(this.btnOpenModel);
            this.tpgImport.Controls.Add(this.txtModelName);
            this.tpgImport.Controls.Add(this.label1);
            this.tpgImport.Location = new System.Drawing.Point(4, 22);
            this.tpgImport.Name = "tpgImport";
            this.tpgImport.Padding = new System.Windows.Forms.Padding(3);
            this.tpgImport.Size = new System.Drawing.Size(238, 248);
            this.tpgImport.TabIndex = 1;
            this.tpgImport.Text = "Import Model";
            this.tpgImport.UseVisualStyleBackColor = true;
            // 
            // txtScale
            // 
            this.txtScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScale.Location = new System.Drawing.Point(109, 56);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(119, 20);
            this.txtScale.TabIndex = 13;
            this.txtScale.Text = "1.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Scale";
            // 
            // btnImportColMap
            // 
            this.btnImportColMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImportColMap.Location = new System.Drawing.Point(3, 222);
            this.btnImportColMap.Name = "btnImportColMap";
            this.btnImportColMap.Size = new System.Drawing.Size(119, 23);
            this.btnImportColMap.TabIndex = 11;
            this.btnImportColMap.Text = "Import Collision Map";
            this.btnImportColMap.UseVisualStyleBackColor = true;
            this.btnImportColMap.Click += new System.EventHandler(this.btnImportColMap_Click);
            // 
            // btnAssignTypes
            // 
            this.btnAssignTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAssignTypes.Location = new System.Drawing.Point(3, 195);
            this.btnAssignTypes.Name = "btnAssignTypes";
            this.btnAssignTypes.Size = new System.Drawing.Size(87, 23);
            this.btnAssignTypes.TabIndex = 10;
            this.btnAssignTypes.Text = "Assign Types";
            this.btnAssignTypes.UseVisualStyleBackColor = true;
            this.btnAssignTypes.Click += new System.EventHandler(this.btnAssignTypes_Click);
            // 
            // gridColTypes
            // 
            this.gridColTypes.AllowUserToAddRows = false;
            this.gridColTypes.AllowUserToDeleteRows = false;
            this.gridColTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridColTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridColTypes.Location = new System.Drawing.Point(0, 77);
            this.gridColTypes.Name = "gridColTypes";
            this.gridColTypes.RowHeadersVisible = false;
            this.gridColTypes.Size = new System.Drawing.Size(231, 112);
            this.gridColTypes.TabIndex = 9;
            // 
            // txtThreshold
            // 
            this.txtThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThreshold.Location = new System.Drawing.Point(109, 30);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(119, 20);
            this.txtThreshold.TabIndex = 4;
            this.txtThreshold.Text = "0.0005";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Drop faces below:";
            // 
            // btnOpenModel
            // 
            this.btnOpenModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenModel.Location = new System.Drawing.Point(73, 2);
            this.btnOpenModel.Name = "btnOpenModel";
            this.btnOpenModel.Size = new System.Drawing.Size(30, 23);
            this.btnOpenModel.TabIndex = 2;
            this.btnOpenModel.Text = "...";
            this.btnOpenModel.UseVisualStyleBackColor = true;
            this.btnOpenModel.Click += new System.EventHandler(this.btnOpenModel_Click);
            // 
            // txtModelName
            // 
            this.txtModelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModelName.Location = new System.Drawing.Point(109, 4);
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.Size = new System.Drawing.Size(119, 20);
            this.txtModelName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model:";
            // 
            // glModelView
            // 
            this.glModelView.BackColor = System.Drawing.Color.Black;
            this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glModelView.Location = new System.Drawing.Point(0, 0);
            this.glModelView.Name = "glModelView";
            this.glModelView.Size = new System.Drawing.Size(650, 523);
            this.glModelView.TabIndex = 0;
            this.glModelView.VSync = false;
            this.glModelView.Load += new System.EventHandler(this.glModelView_Load);
            this.glModelView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glModelView_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnExportKCLModel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(900, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(40, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 22);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExportKCLModel
            // 
            this.btnExportKCLModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportKCLModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportKCLModel.Name = "btnExportKCLModel";
            this.btnExportKCLModel.Size = new System.Drawing.Size(105, 22);
            this.btnExportKCLModel.Text = "Export KCL Model";
            this.btnExportKCLModel.Click += new System.EventHandler(this.btnExportKCLModel_Click);
            // 
            // cmbPolygonMode
            // 
            this.cmbPolygonMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPolygonMode.FormattingEnabled = true;
            this.cmbPolygonMode.Location = new System.Drawing.Point(779, 1);
            this.cmbPolygonMode.Name = "cmbPolygonMode";
            this.cmbPolygonMode.Size = new System.Drawing.Size(121, 21);
            this.cmbPolygonMode.TabIndex = 2;
            this.cmbPolygonMode.SelectedIndexChanged += new System.EventHandler(this.cmbPolygonMode_SelectedIndexChanged);
            // 
            // KCLEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 548);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.cmbPolygonMode);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KCLEditorForm";
            this.Text = "KCL Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitCHorizontal.Panel1.ResumeLayout(false);
            this.splitCHorizontal.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.splitCHorizontal)).EndInit();
            this.splitCHorizontal.ResumeLayout(false);
            this.tabDetailsImport.ResumeLayout(false);
            this.tpgDetails.ResumeLayout(false);
            this.tpgDetails.PerformLayout();
            this.tpgImport.ResumeLayout(false);
            this.tpgImport.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridColTypes)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private FormControls.ModelGLControlWithPicking glModelView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ListBox lbxPlanes;
        private System.Windows.Forms.TextBox txtV2;
        private System.Windows.Forms.TextBox txtV1;
        private System.Windows.Forms.TextBox txtColType;
        private System.Windows.Forms.Label lblColType;
        private System.Windows.Forms.Label lblV3;
        private System.Windows.Forms.Label lblV2;
        private System.Windows.Forms.Label lblV1;
        private System.Windows.Forms.TextBox txtV3;
        private System.Windows.Forms.ComboBox cmbPolygonMode;
        private System.Windows.Forms.TextBox txtD3;
        private System.Windows.Forms.Label lblD3;
        private System.Windows.Forms.TextBox txtD2;
        private System.Windows.Forms.TextBox txtD1;
        private System.Windows.Forms.TextBox txtNormal;
        private System.Windows.Forms.Label lblD2;
        private System.Windows.Forms.Label lblD1;
        private System.Windows.Forms.Label lblNormal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnExportKCLModel;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.TabControl tabDetailsImport;
        private System.Windows.Forms.TabPage tpgDetails;
        private System.Windows.Forms.TabPage tpgImport;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOpenModel;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridColTypes;
        private System.Windows.Forms.Button btnImportColMap;
        private System.Windows.Forms.Button btnAssignTypes;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitCHorizontal;
    }
}