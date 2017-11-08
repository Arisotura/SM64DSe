namespace SM64DSe
{
    partial class ModelAndCollisionMapEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelAndCollisionMapEditor));
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lblMainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpgModel = new System.Windows.Forms.TabPage();
            this.splModel = new System.Windows.Forms.SplitContainer();
            this.tcModelSettings = new System.Windows.Forms.TabControl();
            this.tpgModelGeneral = new System.Windows.Forms.TabPage();
            this.grpModelGeneralScale = new System.Windows.Forms.GroupBox();
            this.txtModelGeneralScale = new System.Windows.Forms.TextBox();
            this.lblModelGeneralScale = new System.Windows.Forms.Label();
            this.grpModelGeneralTarget = new System.Windows.Forms.GroupBox();
            this.lblModelGeneralTargetName = new System.Windows.Forms.Label();
            this.txtModelGeneralTargetName = new System.Windows.Forms.TextBox();
            this.btnModelGeneralSelectTarget = new System.Windows.Forms.Button();
            this.grpModelGeneralImportationSettings = new System.Windows.Forms.GroupBox();
            this.chkModelGeneralStripify = new System.Windows.Forms.CheckBox();
            this.chkModelGeneralKeepVertexOrderDuringStripping = new System.Windows.Forms.CheckBox();
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h = new System.Windows.Forms.CheckBox();
            this.grpModelGeneralTextureQuality = new System.Windows.Forms.GroupBox();
            this.rbModelGeneralTextureAlwaysCompress = new System.Windows.Forms.RadioButton();
            this.rbModelGeneralTextureNeverCompress = new System.Windows.Forms.RadioButton();
            this.rbModelGeneralTextureBetterQualityWhereSensible = new System.Windows.Forms.RadioButton();
            this.chkModelGeneralVFlipAllTextures = new System.Windows.Forms.CheckBox();
            this.tpgModelBones = new System.Windows.Forms.TabPage();
            this.btnModelBonesRenameBone = new System.Windows.Forms.Button();
            this.txtModelBonesName = new System.Windows.Forms.TextBox();
            this.grpModelBonesGeometries = new System.Windows.Forms.GroupBox();
            this.btnModelBonesCopyGeometry = new System.Windows.Forms.Button();
            this.btnModelBonesPasteToGeometry = new SM64DSe.FormControls.MenuButton();
            this.mnuModelBonesPasteToGeometry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnitModelBonesPasteToGeometryPreserveWeights = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget = new System.Windows.Forms.ToolStripMenuItem();
            this.lbxModelBonesGeometries = new System.Windows.Forms.ListBox();
            this.grpModelBonesPolylists = new System.Windows.Forms.GroupBox();
            this.btnModelBonesCopyPolylist = new System.Windows.Forms.Button();
            this.lblModelBonesPolylistMaterial = new System.Windows.Forms.Label();
            this.cmbModelBonesPolylistMaterial = new System.Windows.Forms.ComboBox();
            this.lbxModelBonesPolylists = new System.Windows.Forms.ListBox();
            this.btnModelBonesCutPolylist = new System.Windows.Forms.Button();
            this.btnModelBonesRemovePolylist = new System.Windows.Forms.Button();
            this.btnModelBonesCutGeometry = new System.Windows.Forms.Button();
            this.btnModelBonesRemoveGeometry = new System.Windows.Forms.Button();
            this.tvModelBonesBones = new System.Windows.Forms.TreeView();
            this.btnModelBonesPasteToBone = new SM64DSe.FormControls.MenuButton();
            this.mnuModelBonesPasteToBone = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnitModelBonesPasteToBonePreserveWeights = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget = new System.Windows.Forms.ToolStripMenuItem();
            this.btnModelBonesAddBone = new SM64DSe.FormControls.MenuButton();
            this.mnuModelBonesAddBoneOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnitModelBonesAddBoneSibling = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitModelBonesAddBoneChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tpgModelMaterials = new System.Windows.Forms.TabPage();
            this.txtModelMaterialName = new System.Windows.Forms.TextBox();
            this.grpModelMaterialSettings = new System.Windows.Forms.GroupBox();
            this.btnModelMaterialApplySettings = new System.Windows.Forms.Button();
            this.grpModelMaterialTextureSettings = new System.Windows.Forms.GroupBox();
            this.cmbModelMaterialTextureID = new System.Windows.Forms.ComboBox();
            this.cmbModelMaterialTexGenMode = new System.Windows.Forms.ComboBox();
            this.lblModelMaterialTexGenMode = new System.Windows.Forms.Label();
            this.txtModelMaterialTextureTranslationY = new System.Windows.Forms.TextBox();
            this.txtModelMaterialTextureTranslationX = new System.Windows.Forms.TextBox();
            this.lblModelMaterialTextureTranslation = new System.Windows.Forms.Label();
            this.txtModelMaterialTextureRotation = new System.Windows.Forms.TextBox();
            this.lblModelMaterialTextureRotation = new System.Windows.Forms.Label();
            this.txtModelMaterialTextureScaleY = new System.Windows.Forms.TextBox();
            this.txtModelMaterialTextureScaleX = new System.Windows.Forms.TextBox();
            this.lblModelMaterialTextureScale = new System.Windows.Forms.Label();
            this.cmbModelMaterialTextureTilingY = new System.Windows.Forms.ComboBox();
            this.cmbModelMaterialTextureTilingX = new System.Windows.Forms.ComboBox();
            this.lblModelMaterialTextureTiling = new System.Windows.Forms.Label();
            this.chkModelMaterialTextureEnabled = new System.Windows.Forms.CheckBox();
            this.lblModelMaterialTexture = new System.Windows.Forms.Label();
            this.grpModelMaterialColours = new System.Windows.Forms.GroupBox();
            this.btnModelMaterialEmission = new System.Windows.Forms.Button();
            this.lblModelMaterialEmission = new System.Windows.Forms.Label();
            this.btnModelMaterialSpecular = new System.Windows.Forms.Button();
            this.lblModelMaterialSpecular = new System.Windows.Forms.Label();
            this.nudModelMaterialAlpha = new System.Windows.Forms.NumericUpDown();
            this.btnModelMaterialAmbient = new System.Windows.Forms.Button();
            this.lblModelMaterialAlpha = new System.Windows.Forms.Label();
            this.lblModelMaterialAmbient = new System.Windows.Forms.Label();
            this.btnModelMaterialDiffuse = new System.Windows.Forms.Button();
            this.lblModelMaterialDiffuse = new System.Windows.Forms.Label();
            this.grpModelMaterialFlags = new System.Windows.Forms.GroupBox();
            this.chkModelMaterialShiniessTable = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialFarClipping = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialRenderOnePixelPolygons = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialDepthTestDecal = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialFog = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialWireMode = new System.Windows.Forms.CheckBox();
            this.cmbModelMaterialPolygonMode = new System.Windows.Forms.ComboBox();
            this.lblModelMaterialPolygonMode = new System.Windows.Forms.Label();
            this.cmbModelMaterialPolygonDrawingFace = new System.Windows.Forms.ComboBox();
            this.lblModelMaterialPolygonDrawingFace = new System.Windows.Forms.Label();
            this.chkModelMaterialLight4 = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialLight3 = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialLight2 = new System.Windows.Forms.CheckBox();
            this.chkModelMaterialLight1 = new System.Windows.Forms.CheckBox();
            this.lblModelMaterialLights = new System.Windows.Forms.Label();
            this.btnModelMaterialRemoveMaterial = new System.Windows.Forms.Button();
            this.btnModelMaterialRenameMaterial = new System.Windows.Forms.Button();
            this.btnModelMaterialAddMaterial = new System.Windows.Forms.Button();
            this.lbxModelMaterials = new System.Windows.Forms.ListBox();
            this.tpgModelTextures = new System.Windows.Forms.TabPage();
            this.btnModelTexturesReplace = new System.Windows.Forms.Button();
            this.grpModelTexturesPreview = new System.Windows.Forms.GroupBox();
            this.pnlModelTexturesPreviewPanel = new System.Windows.Forms.Panel();
            this.pbxModelTexturesPreview = new System.Windows.Forms.PictureBox();
            this.grpModelTexturesSettings = new System.Windows.Forms.GroupBox();
            this.cmbModelTexturesPalette = new System.Windows.Forms.ComboBox();
            this.lblModelTexturesPalette = new System.Windows.Forms.Label();
            this.cmbModelTexturesHeight = new System.Windows.Forms.ComboBox();
            this.cmbModelTexturesWidth = new System.Windows.Forms.ComboBox();
            this.lblModelTexturesDimensions = new System.Windows.Forms.Label();
            this.cmbModelTexturesFormat = new System.Windows.Forms.ComboBox();
            this.lblModelTexturesFormat = new System.Windows.Forms.Label();
            this.lbxModelTextures = new System.Windows.Forms.ListBox();
            this.btnModelTexturesAddTexture = new System.Windows.Forms.Button();
            this.btnModelTexturesRenameTexture = new System.Windows.Forms.Button();
            this.txtModelTexturesName = new System.Windows.Forms.TextBox();
            this.btnModelTexturesRemoveTexture = new System.Windows.Forms.Button();
            this.tpgPalettes = new System.Windows.Forms.TabPage();
            this.grpModelPalettesColours = new System.Windows.Forms.GroupBox();
            this.gridModelPalettesPaletteColours = new SM64DSe.PaletteColourGrid();
            this.btnModelPalettesSelectedColour = new System.Windows.Forms.Button();
            this.lblModelPalettesPaletteSelectedColour = new System.Windows.Forms.Label();
            this.lbxModelPalettes = new System.Windows.Forms.ListBox();
            this.btnModelPalettesRenamePalette = new System.Windows.Forms.Button();
            this.txtModelPalettesName = new System.Windows.Forms.TextBox();
            this.txtModelPreviewScale = new System.Windows.Forms.TextBox();
            this.tsModelPreview = new System.Windows.Forms.ToolStrip();
            this.lblModelPreviewScale = new System.Windows.Forms.ToolStripLabel();
            this.glModelView = new SM64DSe.FormControls.ModelGLControlWithMarioSizeReference();
            this.tpgCollisionMap = new System.Windows.Forms.TabPage();
            this.splCollisionMap = new System.Windows.Forms.SplitContainer();
            this.tcCollisionMapSettings = new System.Windows.Forms.TabControl();
            this.tpgCollisionMapGeneral = new System.Windows.Forms.TabPage();
            this.grpCollisionMapGeneralScale = new System.Windows.Forms.GroupBox();
            this.txtCollisionMapGeneralTargetScale = new System.Windows.Forms.TextBox();
            this.txtCollisionMapGeneralScale = new System.Windows.Forms.TextBox();
            this.lblCollisionMapGeneralScale = new System.Windows.Forms.Label();
            this.CollisionMapGeneralTargetScale = new System.Windows.Forms.Label();
            this.grpCollisionMapGeneralTarget = new System.Windows.Forms.GroupBox();
            this.lblCollisionMapGeneralTarget = new System.Windows.Forms.Label();
            this.txtCollisionMapGeneralTargetName = new System.Windows.Forms.TextBox();
            this.btnCollisionMapGeneralSelectTarget = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gridCollisionMapGeneralMaterialCollisionTypes = new System.Windows.Forms.DataGridView();
            this.btnCollisionMapGeneralResetMaterialCollisionTypes = new System.Windows.Forms.Button();
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes = new System.Windows.Forms.Button();
            this.lblCollisionMapGeneralMaterialCollisionTypes = new System.Windows.Forms.Label();
            this.txtCollisionMapGeneralFaceSizeThreshold = new System.Windows.Forms.TextBox();
            this.lblCollisionMapGeneralFaceSizeThreshold = new System.Windows.Forms.Label();
            this.tpgCollisionMapPlanes = new System.Windows.Forms.TabPage();
            this.grpCollisionMapPlanesDetails = new System.Windows.Forms.GroupBox();
            this.lblCollisionMapPlanesVertex1 = new System.Windows.Forms.Label();
            this.txtCollisionMapPlanesDirection3 = new System.Windows.Forms.TextBox();
            this.lblCollisionMapPlanesVertex2 = new System.Windows.Forms.Label();
            this.lblCollisionMapPlanesDirection3 = new System.Windows.Forms.Label();
            this.lblCollisionMapPlanesVertex3 = new System.Windows.Forms.Label();
            this.txtCollisionMapPlanesDirection2 = new System.Windows.Forms.TextBox();
            this.lblColType = new System.Windows.Forms.Label();
            this.txtCollisionMapPlanesDirection1 = new System.Windows.Forms.TextBox();
            this.txtCollisionMapPlanesCollisionType = new System.Windows.Forms.TextBox();
            this.txtCollisionMapPlanesNormal = new System.Windows.Forms.TextBox();
            this.txtCollisionMapPlanesVertex1 = new System.Windows.Forms.TextBox();
            this.lblCollisionMapPlanesDirection2 = new System.Windows.Forms.Label();
            this.txtCollisionMapPlanesVertex2 = new System.Windows.Forms.TextBox();
            this.lblCollisionMapPlanesDirection1 = new System.Windows.Forms.Label();
            this.txtCollisionMapPlanesVertex3 = new System.Windows.Forms.TextBox();
            this.lblNormal = new System.Windows.Forms.Label();
            this.lbxCollisionMapPlanes = new System.Windows.Forms.ListBox();
            this.cmbCollisionMapPreviewFillMode = new System.Windows.Forms.ComboBox();
            this.tsCollisionMapPreview = new System.Windows.Forms.ToolStrip();
            this.glCollisionMapView = new SM64DSe.FormControls.ModelGLControlWithPicking();
            this.mstrMain = new System.Windows.Forms.MenuStrip();
            this.mnitLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitLoadExternalModel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitLoadInternalModelCollisionMap = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitLoadRevertChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitImport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitImportModelAndCollisionMap = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitImportModelOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitImportCollisionMapOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExportModel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExportCollisionMap = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExportTextures = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExportTexturesPNG = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExportTexturesBMP = new System.Windows.Forms.ToolStripMenuItem();
            this.grpModelBonesSettings = new System.Windows.Forms.GroupBox();
            this.chkModelBonesSettingsBillboard = new System.Windows.Forms.CheckBox();
            this.ssMain.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpgModel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splModel)).BeginInit();
            this.splModel.Panel1.SuspendLayout();
            this.splModel.Panel2.SuspendLayout();
            this.splModel.SuspendLayout();
            this.tcModelSettings.SuspendLayout();
            this.tpgModelGeneral.SuspendLayout();
            this.grpModelGeneralScale.SuspendLayout();
            this.grpModelGeneralTarget.SuspendLayout();
            this.grpModelGeneralImportationSettings.SuspendLayout();
            this.grpModelGeneralTextureQuality.SuspendLayout();
            this.tpgModelBones.SuspendLayout();
            this.grpModelBonesGeometries.SuspendLayout();
            this.mnuModelBonesPasteToGeometry.SuspendLayout();
            this.grpModelBonesPolylists.SuspendLayout();
            this.mnuModelBonesPasteToBone.SuspendLayout();
            this.mnuModelBonesAddBoneOptions.SuspendLayout();
            this.tpgModelMaterials.SuspendLayout();
            this.grpModelMaterialSettings.SuspendLayout();
            this.grpModelMaterialTextureSettings.SuspendLayout();
            this.grpModelMaterialColours.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.nudModelMaterialAlpha)).BeginInit();
            this.grpModelMaterialFlags.SuspendLayout();
            this.tpgModelTextures.SuspendLayout();
            this.grpModelTexturesPreview.SuspendLayout();
            this.pnlModelTexturesPreviewPanel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.pbxModelTexturesPreview)).BeginInit();
            this.grpModelTexturesSettings.SuspendLayout();
            this.tpgPalettes.SuspendLayout();
            this.grpModelPalettesColours.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridModelPalettesPaletteColours)).BeginInit();
            this.tsModelPreview.SuspendLayout();
            this.tpgCollisionMap.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splCollisionMap)).BeginInit();
            this.splCollisionMap.Panel1.SuspendLayout();
            this.splCollisionMap.Panel2.SuspendLayout();
            this.splCollisionMap.SuspendLayout();
            this.tcCollisionMapSettings.SuspendLayout();
            this.tpgCollisionMapGeneral.SuspendLayout();
            this.grpCollisionMapGeneralScale.SuspendLayout();
            this.grpCollisionMapGeneralTarget.SuspendLayout();
            this.groupBox1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridCollisionMapGeneralMaterialCollisionTypes)).BeginInit();
            this.tpgCollisionMapPlanes.SuspendLayout();
            this.grpCollisionMapPlanesDetails.SuspendLayout();
            this.mstrMain.SuspendLayout();
            this.grpModelBonesSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMainStatus});
            this.ssMain.Location = new System.Drawing.Point(0, 576);
            this.ssMain.Name = "ssMain";
            this.ssMain.Size = new System.Drawing.Size(888, 22);
            this.ssMain.TabIndex = 1;
            this.ssMain.Text = "statusStrip1";
            // 
            // lblMainStatus
            // 
            this.lblMainStatus.Name = "lblMainStatus";
            this.lblMainStatus.Size = new System.Drawing.Size(39, 17);
            this.lblMainStatus.Text = "Ready";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpgModel);
            this.tcMain.Controls.Add(this.tpgCollisionMap);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 24);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(888, 552);
            this.tcMain.TabIndex = 2;
            // 
            // tpgModel
            // 
            this.tpgModel.Controls.Add(this.splModel);
            this.tpgModel.Location = new System.Drawing.Point(4, 22);
            this.tpgModel.Name = "tpgModel";
            this.tpgModel.Padding = new System.Windows.Forms.Padding(3);
            this.tpgModel.Size = new System.Drawing.Size(880, 526);
            this.tpgModel.TabIndex = 0;
            this.tpgModel.Text = "Model (BMD)";
            this.tpgModel.UseVisualStyleBackColor = true;
            // 
            // splModel
            // 
            this.splModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splModel.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splModel.IsSplitterFixed = true;
            this.splModel.Location = new System.Drawing.Point(3, 3);
            this.splModel.Name = "splModel";
            // 
            // splModel.Panel1
            // 
            this.splModel.Panel1.Controls.Add(this.tcModelSettings);
            // 
            // splModel.Panel2
            // 
            this.splModel.Panel2.Controls.Add(this.txtModelPreviewScale);
            this.splModel.Panel2.Controls.Add(this.tsModelPreview);
            this.splModel.Panel2.Controls.Add(this.glModelView);
            this.splModel.Size = new System.Drawing.Size(874, 520);
            this.splModel.SplitterDistance = 308;
            this.splModel.TabIndex = 0;
            // 
            // tcModelSettings
            // 
            this.tcModelSettings.Controls.Add(this.tpgModelGeneral);
            this.tcModelSettings.Controls.Add(this.tpgModelBones);
            this.tcModelSettings.Controls.Add(this.tpgModelMaterials);
            this.tcModelSettings.Controls.Add(this.tpgModelTextures);
            this.tcModelSettings.Controls.Add(this.tpgPalettes);
            this.tcModelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcModelSettings.Location = new System.Drawing.Point(0, 0);
            this.tcModelSettings.Name = "tcModelSettings";
            this.tcModelSettings.SelectedIndex = 0;
            this.tcModelSettings.Size = new System.Drawing.Size(308, 520);
            this.tcModelSettings.TabIndex = 0;
            this.tcModelSettings.SelectedIndexChanged += new System.EventHandler(this.tcModelSettings_SelectedIndexChanged);
            // 
            // tpgModelGeneral
            // 
            this.tpgModelGeneral.Controls.Add(this.grpModelGeneralScale);
            this.tpgModelGeneral.Controls.Add(this.grpModelGeneralTarget);
            this.tpgModelGeneral.Controls.Add(this.grpModelGeneralImportationSettings);
            this.tpgModelGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpgModelGeneral.Name = "tpgModelGeneral";
            this.tpgModelGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpgModelGeneral.Size = new System.Drawing.Size(300, 494);
            this.tpgModelGeneral.TabIndex = 0;
            this.tpgModelGeneral.Text = "General";
            this.tpgModelGeneral.UseVisualStyleBackColor = true;
            // 
            // grpModelGeneralScale
            // 
            this.grpModelGeneralScale.Controls.Add(this.txtModelGeneralScale);
            this.grpModelGeneralScale.Controls.Add(this.lblModelGeneralScale);
            this.grpModelGeneralScale.Location = new System.Drawing.Point(6, 259);
            this.grpModelGeneralScale.Name = "grpModelGeneralScale";
            this.grpModelGeneralScale.Size = new System.Drawing.Size(287, 41);
            this.grpModelGeneralScale.TabIndex = 21;
            this.grpModelGeneralScale.TabStop = false;
            this.grpModelGeneralScale.Text = "Scale";
            // 
            // txtModelGeneralScale
            // 
            this.txtModelGeneralScale.Location = new System.Drawing.Point(175, 13);
            this.txtModelGeneralScale.Name = "txtModelGeneralScale";
            this.txtModelGeneralScale.Size = new System.Drawing.Size(106, 20);
            this.txtModelGeneralScale.TabIndex = 1;
            this.txtModelGeneralScale.TextChanged += new System.EventHandler(this.txtModelGeneralScale_TextChanged);
            // 
            // lblModelGeneralScale
            // 
            this.lblModelGeneralScale.AutoSize = true;
            this.lblModelGeneralScale.Location = new System.Drawing.Point(6, 16);
            this.lblModelGeneralScale.Name = "lblModelGeneralScale";
            this.lblModelGeneralScale.Size = new System.Drawing.Size(37, 13);
            this.lblModelGeneralScale.TabIndex = 0;
            this.lblModelGeneralScale.Text = "Scale:";
            // 
            // grpModelGeneralTarget
            // 
            this.grpModelGeneralTarget.Controls.Add(this.lblModelGeneralTargetName);
            this.grpModelGeneralTarget.Controls.Add(this.txtModelGeneralTargetName);
            this.grpModelGeneralTarget.Controls.Add(this.btnModelGeneralSelectTarget);
            this.grpModelGeneralTarget.Location = new System.Drawing.Point(6, 6);
            this.grpModelGeneralTarget.Name = "grpModelGeneralTarget";
            this.grpModelGeneralTarget.Size = new System.Drawing.Size(287, 40);
            this.grpModelGeneralTarget.TabIndex = 20;
            this.grpModelGeneralTarget.TabStop = false;
            this.grpModelGeneralTarget.Text = "Target";
            // 
            // lblModelGeneralTargetName
            // 
            this.lblModelGeneralTargetName.AutoSize = true;
            this.lblModelGeneralTargetName.Location = new System.Drawing.Point(6, 16);
            this.lblModelGeneralTargetName.Name = "lblModelGeneralTargetName";
            this.lblModelGeneralTargetName.Size = new System.Drawing.Size(38, 13);
            this.lblModelGeneralTargetName.TabIndex = 16;
            this.lblModelGeneralTargetName.Text = "Name:";
            // 
            // txtModelGeneralTargetName
            // 
            this.txtModelGeneralTargetName.Location = new System.Drawing.Point(50, 13);
            this.txtModelGeneralTargetName.Name = "txtModelGeneralTargetName";
            this.txtModelGeneralTargetName.ReadOnly = true;
            this.txtModelGeneralTargetName.Size = new System.Drawing.Size(200, 20);
            this.txtModelGeneralTargetName.TabIndex = 17;
            // 
            // btnModelGeneralSelectTarget
            // 
            this.btnModelGeneralSelectTarget.Location = new System.Drawing.Point(256, 11);
            this.btnModelGeneralSelectTarget.Name = "btnModelGeneralSelectTarget";
            this.btnModelGeneralSelectTarget.Size = new System.Drawing.Size(25, 23);
            this.btnModelGeneralSelectTarget.TabIndex = 18;
            this.btnModelGeneralSelectTarget.Text = "...";
            this.btnModelGeneralSelectTarget.UseVisualStyleBackColor = true;
            this.btnModelGeneralSelectTarget.Click += new System.EventHandler(this.btnModelGeneralSelectTarget_Click);
            // 
            // grpModelGeneralImportationSettings
            // 
            this.grpModelGeneralImportationSettings.Controls.Add(this.chkModelGeneralStripify);
            this.grpModelGeneralImportationSettings.Controls.Add(this.chkModelGeneralKeepVertexOrderDuringStripping);
            this.grpModelGeneralImportationSettings.Controls.Add(this.chkModelGeneralAlwaysWriteFullVertexCmd23h);
            this.grpModelGeneralImportationSettings.Controls.Add(this.grpModelGeneralTextureQuality);
            this.grpModelGeneralImportationSettings.Controls.Add(this.chkModelGeneralVFlipAllTextures);
            this.grpModelGeneralImportationSettings.Location = new System.Drawing.Point(6, 53);
            this.grpModelGeneralImportationSettings.Name = "grpModelGeneralImportationSettings";
            this.grpModelGeneralImportationSettings.Size = new System.Drawing.Size(287, 200);
            this.grpModelGeneralImportationSettings.TabIndex = 19;
            this.grpModelGeneralImportationSettings.TabStop = false;
            this.grpModelGeneralImportationSettings.Text = "Importation Settings";
            // 
            // chkModelGeneralStripify
            // 
            this.chkModelGeneralStripify.AutoSize = true;
            this.chkModelGeneralStripify.Location = new System.Drawing.Point(6, 19);
            this.chkModelGeneralStripify.Name = "chkModelGeneralStripify";
            this.chkModelGeneralStripify.Size = new System.Drawing.Size(241, 17);
            this.chkModelGeneralStripify.TabIndex = 1;
            this.chkModelGeneralStripify.Text = "Convert to Tri Strips (Strongly Recommended)";
            this.chkModelGeneralStripify.UseVisualStyleBackColor = true;
            this.chkModelGeneralStripify.CheckedChanged += new System.EventHandler(this.chkModelGeneralStripify_CheckedChanged);
            // 
            // chkModelGeneralKeepVertexOrderDuringStripping
            // 
            this.chkModelGeneralKeepVertexOrderDuringStripping.AutoSize = true;
            this.chkModelGeneralKeepVertexOrderDuringStripping.Location = new System.Drawing.Point(23, 42);
            this.chkModelGeneralKeepVertexOrderDuringStripping.Name = "chkModelGeneralKeepVertexOrderDuringStripping";
            this.chkModelGeneralKeepVertexOrderDuringStripping.Size = new System.Drawing.Size(220, 17);
            this.chkModelGeneralKeepVertexOrderDuringStripping.TabIndex = 2;
            this.chkModelGeneralKeepVertexOrderDuringStripping.Text = "Keep Vertex Order (Keep False if Unsure)";
            this.chkModelGeneralKeepVertexOrderDuringStripping.UseVisualStyleBackColor = true;
            this.chkModelGeneralKeepVertexOrderDuringStripping.CheckedChanged += new System.EventHandler(this.chkModelGeneralKeepVertexOrderDuringStripping_CheckedChanged);
            // 
            // chkModelGeneralAlwaysWriteFullVertexCmd23h
            // 
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.AutoSize = true;
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.Location = new System.Drawing.Point(6, 66);
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.Name = "chkModelGeneralAlwaysWriteFullVertexCmd23h";
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.Size = new System.Drawing.Size(219, 17);
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.TabIndex = 3;
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.Text = "Always Write Full X,Y,Z Vertex Command";
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.UseVisualStyleBackColor = true;
            this.chkModelGeneralAlwaysWriteFullVertexCmd23h.CheckedChanged += new System.EventHandler(this.chkModelGeneralAlwaysWriteFullVertexCmd23h_CheckedChanged);
            // 
            // grpModelGeneralTextureQuality
            // 
            this.grpModelGeneralTextureQuality.Controls.Add(this.rbModelGeneralTextureAlwaysCompress);
            this.grpModelGeneralTextureQuality.Controls.Add(this.rbModelGeneralTextureNeverCompress);
            this.grpModelGeneralTextureQuality.Controls.Add(this.rbModelGeneralTextureBetterQualityWhereSensible);
            this.grpModelGeneralTextureQuality.Location = new System.Drawing.Point(6, 89);
            this.grpModelGeneralTextureQuality.Name = "grpModelGeneralTextureQuality";
            this.grpModelGeneralTextureQuality.Size = new System.Drawing.Size(275, 82);
            this.grpModelGeneralTextureQuality.TabIndex = 12;
            this.grpModelGeneralTextureQuality.TabStop = false;
            this.grpModelGeneralTextureQuality.Text = "Default Texture Quality Setting";
            // 
            // rbModelGeneralTextureAlwaysCompress
            // 
            this.rbModelGeneralTextureAlwaysCompress.AutoSize = true;
            this.rbModelGeneralTextureAlwaysCompress.Checked = true;
            this.rbModelGeneralTextureAlwaysCompress.Location = new System.Drawing.Point(26, 17);
            this.rbModelGeneralTextureAlwaysCompress.Name = "rbModelGeneralTextureAlwaysCompress";
            this.rbModelGeneralTextureAlwaysCompress.Size = new System.Drawing.Size(87, 17);
            this.rbModelGeneralTextureAlwaysCompress.TabIndex = 0;
            this.rbModelGeneralTextureAlwaysCompress.TabStop = true;
            this.rbModelGeneralTextureAlwaysCompress.Text = "Smallest Size";
            this.rbModelGeneralTextureAlwaysCompress.UseVisualStyleBackColor = true;
            this.rbModelGeneralTextureAlwaysCompress.CheckedChanged += new System.EventHandler(this.rbModelGeneralTextureAlwaysCompress_CheckedChanged);
            // 
            // rbModelGeneralTextureNeverCompress
            // 
            this.rbModelGeneralTextureNeverCompress.AutoSize = true;
            this.rbModelGeneralTextureNeverCompress.Location = new System.Drawing.Point(26, 63);
            this.rbModelGeneralTextureNeverCompress.Name = "rbModelGeneralTextureNeverCompress";
            this.rbModelGeneralTextureNeverCompress.Size = new System.Drawing.Size(81, 17);
            this.rbModelGeneralTextureNeverCompress.TabIndex = 2;
            this.rbModelGeneralTextureNeverCompress.TabStop = true;
            this.rbModelGeneralTextureNeverCompress.Text = "Best Quality";
            this.rbModelGeneralTextureNeverCompress.UseVisualStyleBackColor = true;
            this.rbModelGeneralTextureNeverCompress.CheckedChanged += new System.EventHandler(this.rbModelGeneralTextureNeverCompress_CheckedChanged);
            // 
            // rbModelGeneralTextureBetterQualityWhereSensible
            // 
            this.rbModelGeneralTextureBetterQualityWhereSensible.AutoSize = true;
            this.rbModelGeneralTextureBetterQualityWhereSensible.Location = new System.Drawing.Point(26, 40);
            this.rbModelGeneralTextureBetterQualityWhereSensible.Name = "rbModelGeneralTextureBetterQualityWhereSensible";
            this.rbModelGeneralTextureBetterQualityWhereSensible.Size = new System.Drawing.Size(166, 17);
            this.rbModelGeneralTextureBetterQualityWhereSensible.TabIndex = 1;
            this.rbModelGeneralTextureBetterQualityWhereSensible.TabStop = true;
            this.rbModelGeneralTextureBetterQualityWhereSensible.Text = "Better Quality Where Sensible";
            this.rbModelGeneralTextureBetterQualityWhereSensible.UseVisualStyleBackColor = true;
            this.rbModelGeneralTextureBetterQualityWhereSensible.CheckedChanged += new System.EventHandler(this.rbModelGeneralTextureBetterQualityWhereSensible_CheckedChanged);
            // 
            // chkModelGeneralVFlipAllTextures
            // 
            this.chkModelGeneralVFlipAllTextures.AutoSize = true;
            this.chkModelGeneralVFlipAllTextures.Location = new System.Drawing.Point(6, 177);
            this.chkModelGeneralVFlipAllTextures.Name = "chkModelGeneralVFlipAllTextures";
            this.chkModelGeneralVFlipAllTextures.Size = new System.Drawing.Size(145, 17);
            this.chkModelGeneralVFlipAllTextures.TabIndex = 13;
            this.chkModelGeneralVFlipAllTextures.Text = "Vertically Flip All Textures";
            this.chkModelGeneralVFlipAllTextures.UseVisualStyleBackColor = true;
            this.chkModelGeneralVFlipAllTextures.CheckedChanged += new System.EventHandler(this.chkModelGeneralVFlipAllTextures_CheckedChanged);
            // 
            // tpgModelBones
            // 
            this.tpgModelBones.AutoScroll = true;
            this.tpgModelBones.Controls.Add(this.grpModelBonesSettings);
            this.tpgModelBones.Controls.Add(this.btnModelBonesRenameBone);
            this.tpgModelBones.Controls.Add(this.txtModelBonesName);
            this.tpgModelBones.Controls.Add(this.grpModelBonesGeometries);
            this.tpgModelBones.Controls.Add(this.tvModelBonesBones);
            this.tpgModelBones.Controls.Add(this.btnModelBonesPasteToBone);
            this.tpgModelBones.Controls.Add(this.btnModelBonesAddBone);
            this.tpgModelBones.Location = new System.Drawing.Point(4, 22);
            this.tpgModelBones.Name = "tpgModelBones";
            this.tpgModelBones.Padding = new System.Windows.Forms.Padding(3);
            this.tpgModelBones.Size = new System.Drawing.Size(300, 494);
            this.tpgModelBones.TabIndex = 1;
            this.tpgModelBones.Tag = "Model.Bones";
            this.tpgModelBones.Text = "Bones";
            this.tpgModelBones.UseVisualStyleBackColor = true;
            // 
            // btnModelBonesRenameBone
            // 
            this.btnModelBonesRenameBone.Location = new System.Drawing.Point(131, 173);
            this.btnModelBonesRenameBone.Name = "btnModelBonesRenameBone";
            this.btnModelBonesRenameBone.Size = new System.Drawing.Size(59, 23);
            this.btnModelBonesRenameBone.TabIndex = 20;
            this.btnModelBonesRenameBone.Text = "Rename";
            this.btnModelBonesRenameBone.UseVisualStyleBackColor = true;
            this.btnModelBonesRenameBone.Click += new System.EventHandler(this.btnModelBonesRenameBone_Click);
            // 
            // txtModelBonesName
            // 
            this.txtModelBonesName.Location = new System.Drawing.Point(196, 175);
            this.txtModelBonesName.Name = "txtModelBonesName";
            this.txtModelBonesName.Size = new System.Drawing.Size(81, 20);
            this.txtModelBonesName.TabIndex = 19;
            // 
            // grpModelBonesGeometries
            // 
            this.grpModelBonesGeometries.Controls.Add(this.btnModelBonesCopyGeometry);
            this.grpModelBonesGeometries.Controls.Add(this.btnModelBonesPasteToGeometry);
            this.grpModelBonesGeometries.Controls.Add(this.lbxModelBonesGeometries);
            this.grpModelBonesGeometries.Controls.Add(this.grpModelBonesPolylists);
            this.grpModelBonesGeometries.Controls.Add(this.btnModelBonesCutGeometry);
            this.grpModelBonesGeometries.Controls.Add(this.btnModelBonesRemoveGeometry);
            this.grpModelBonesGeometries.Location = new System.Drawing.Point(6, 252);
            this.grpModelBonesGeometries.Name = "grpModelBonesGeometries";
            this.grpModelBonesGeometries.Size = new System.Drawing.Size(271, 338);
            this.grpModelBonesGeometries.TabIndex = 15;
            this.grpModelBonesGeometries.TabStop = false;
            this.grpModelBonesGeometries.Text = "Geometries";
            // 
            // btnModelBonesCopyGeometry
            // 
            this.btnModelBonesCopyGeometry.Location = new System.Drawing.Point(3, 120);
            this.btnModelBonesCopyGeometry.Name = "btnModelBonesCopyGeometry";
            this.btnModelBonesCopyGeometry.Size = new System.Drawing.Size(55, 23);
            this.btnModelBonesCopyGeometry.TabIndex = 17;
            this.btnModelBonesCopyGeometry.Text = "Copy";
            this.btnModelBonesCopyGeometry.UseVisualStyleBackColor = true;
            this.btnModelBonesCopyGeometry.Click += new System.EventHandler(this.btnModelBonesCopyGeometry_Click);
            // 
            // btnModelBonesPasteToGeometry
            // 
            this.btnModelBonesPasteToGeometry.Location = new System.Drawing.Point(125, 120);
            this.btnModelBonesPasteToGeometry.Menu = this.mnuModelBonesPasteToGeometry;
            this.btnModelBonesPasteToGeometry.Name = "btnModelBonesPasteToGeometry";
            this.btnModelBonesPasteToGeometry.Size = new System.Drawing.Size(60, 23);
            this.btnModelBonesPasteToGeometry.TabIndex = 9;
            this.btnModelBonesPasteToGeometry.Text = "Paste";
            this.btnModelBonesPasteToGeometry.UseVisualStyleBackColor = true;
            // 
            // mnuModelBonesPasteToGeometry
            // 
            this.mnuModelBonesPasteToGeometry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitModelBonesPasteToGeometryPreserveWeights,
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget});
            this.mnuModelBonesPasteToGeometry.Name = "mnuModelBonesPasteToGeometry";
            this.mnuModelBonesPasteToGeometry.Size = new System.Drawing.Size(237, 48);
            // 
            // mnitModelBonesPasteToGeometryPreserveWeights
            // 
            this.mnitModelBonesPasteToGeometryPreserveWeights.Name = "mnitModelBonesPasteToGeometryPreserveWeights";
            this.mnitModelBonesPasteToGeometryPreserveWeights.Size = new System.Drawing.Size(236, 22);
            this.mnitModelBonesPasteToGeometryPreserveWeights.Text = "Preserve Weights";
            this.mnitModelBonesPasteToGeometryPreserveWeights.Click += new System.EventHandler(this.mnitModelBonesPasteToGeometryPreserveWeights_Click);
            // 
            // mnitModelBonesPasteToGeometryAssignWeightsToTarget
            // 
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget.Name = "mnitModelBonesPasteToGeometryAssignWeightsToTarget";
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget.Size = new System.Drawing.Size(236, 22);
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget.Text = "Assign Weights to Target Bone";
            this.mnitModelBonesPasteToGeometryAssignWeightsToTarget.Click += new System.EventHandler(this.mnitModelBonesPasteToGeometryAssignWeightsToTarget_Click);
            // 
            // lbxModelBonesGeometries
            // 
            this.lbxModelBonesGeometries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxModelBonesGeometries.FormattingEnabled = true;
            this.lbxModelBonesGeometries.Location = new System.Drawing.Point(3, 19);
            this.lbxModelBonesGeometries.Name = "lbxModelBonesGeometries";
            this.lbxModelBonesGeometries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxModelBonesGeometries.Size = new System.Drawing.Size(262, 95);
            this.lbxModelBonesGeometries.TabIndex = 6;
            this.lbxModelBonesGeometries.SelectedIndexChanged += new System.EventHandler(this.lbxModelBonesGeometries_SelectedIndexChanged);
            // 
            // grpModelBonesPolylists
            // 
            this.grpModelBonesPolylists.Controls.Add(this.btnModelBonesCopyPolylist);
            this.grpModelBonesPolylists.Controls.Add(this.lblModelBonesPolylistMaterial);
            this.grpModelBonesPolylists.Controls.Add(this.cmbModelBonesPolylistMaterial);
            this.grpModelBonesPolylists.Controls.Add(this.lbxModelBonesPolylists);
            this.grpModelBonesPolylists.Controls.Add(this.btnModelBonesCutPolylist);
            this.grpModelBonesPolylists.Controls.Add(this.btnModelBonesRemovePolylist);
            this.grpModelBonesPolylists.Location = new System.Drawing.Point(6, 149);
            this.grpModelBonesPolylists.Name = "grpModelBonesPolylists";
            this.grpModelBonesPolylists.Size = new System.Drawing.Size(259, 178);
            this.grpModelBonesPolylists.TabIndex = 16;
            this.grpModelBonesPolylists.TabStop = false;
            this.grpModelBonesPolylists.Text = "Polylists";
            // 
            // btnModelBonesCopyPolylist
            // 
            this.btnModelBonesCopyPolylist.Location = new System.Drawing.Point(3, 120);
            this.btnModelBonesCopyPolylist.Name = "btnModelBonesCopyPolylist";
            this.btnModelBonesCopyPolylist.Size = new System.Drawing.Size(55, 23);
            this.btnModelBonesCopyPolylist.TabIndex = 15;
            this.btnModelBonesCopyPolylist.Text = "Copy";
            this.btnModelBonesCopyPolylist.UseVisualStyleBackColor = true;
            this.btnModelBonesCopyPolylist.Click += new System.EventHandler(this.btnModelBonesCopyPolylist_Click);
            // 
            // lblModelBonesPolylistMaterial
            // 
            this.lblModelBonesPolylistMaterial.AutoSize = true;
            this.lblModelBonesPolylistMaterial.Location = new System.Drawing.Point(6, 152);
            this.lblModelBonesPolylistMaterial.Name = "lblModelBonesPolylistMaterial";
            this.lblModelBonesPolylistMaterial.Size = new System.Drawing.Size(47, 13);
            this.lblModelBonesPolylistMaterial.TabIndex = 14;
            this.lblModelBonesPolylistMaterial.Text = "Material:";
            // 
            // cmbModelBonesPolylistMaterial
            // 
            this.cmbModelBonesPolylistMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelBonesPolylistMaterial.FormattingEnabled = true;
            this.cmbModelBonesPolylistMaterial.Location = new System.Drawing.Point(120, 149);
            this.cmbModelBonesPolylistMaterial.Name = "cmbModelBonesPolylistMaterial";
            this.cmbModelBonesPolylistMaterial.Size = new System.Drawing.Size(133, 21);
            this.cmbModelBonesPolylistMaterial.TabIndex = 13;
            this.cmbModelBonesPolylistMaterial.SelectedIndexChanged += new System.EventHandler(this.cmbModelBonesPolylistMaterial_SelectedIndexChanged);
            // 
            // lbxModelBonesPolylists
            // 
            this.lbxModelBonesPolylists.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxModelBonesPolylists.FormattingEnabled = true;
            this.lbxModelBonesPolylists.Location = new System.Drawing.Point(3, 19);
            this.lbxModelBonesPolylists.Name = "lbxModelBonesPolylists";
            this.lbxModelBonesPolylists.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxModelBonesPolylists.Size = new System.Drawing.Size(250, 95);
            this.lbxModelBonesPolylists.TabIndex = 10;
            this.lbxModelBonesPolylists.SelectedIndexChanged += new System.EventHandler(this.lbxModelBonesPolylists_SelectedIndexChanged);
            // 
            // btnModelBonesCutPolylist
            // 
            this.btnModelBonesCutPolylist.Location = new System.Drawing.Point(64, 120);
            this.btnModelBonesCutPolylist.Name = "btnModelBonesCutPolylist";
            this.btnModelBonesCutPolylist.Size = new System.Drawing.Size(55, 23);
            this.btnModelBonesCutPolylist.TabIndex = 11;
            this.btnModelBonesCutPolylist.Text = "Cut";
            this.btnModelBonesCutPolylist.UseVisualStyleBackColor = true;
            this.btnModelBonesCutPolylist.Click += new System.EventHandler(this.btnModelBonesCutPolylist_Click);
            // 
            // btnModelBonesRemovePolylist
            // 
            this.btnModelBonesRemovePolylist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModelBonesRemovePolylist.Location = new System.Drawing.Point(190, 120);
            this.btnModelBonesRemovePolylist.Name = "btnModelBonesRemovePolylist";
            this.btnModelBonesRemovePolylist.Size = new System.Drawing.Size(63, 23);
            this.btnModelBonesRemovePolylist.TabIndex = 12;
            this.btnModelBonesRemovePolylist.Text = "Remove";
            this.btnModelBonesRemovePolylist.UseVisualStyleBackColor = true;
            this.btnModelBonesRemovePolylist.Click += new System.EventHandler(this.btnModelBonesRemovePolyList_Click);
            // 
            // btnModelBonesCutGeometry
            // 
            this.btnModelBonesCutGeometry.Location = new System.Drawing.Point(64, 120);
            this.btnModelBonesCutGeometry.Name = "btnModelBonesCutGeometry";
            this.btnModelBonesCutGeometry.Size = new System.Drawing.Size(55, 23);
            this.btnModelBonesCutGeometry.TabIndex = 7;
            this.btnModelBonesCutGeometry.Text = "Cut";
            this.btnModelBonesCutGeometry.UseVisualStyleBackColor = true;
            this.btnModelBonesCutGeometry.Click += new System.EventHandler(this.btnModelBonesCutGeometry_Click);
            // 
            // btnModelBonesRemoveGeometry
            // 
            this.btnModelBonesRemoveGeometry.Location = new System.Drawing.Point(191, 120);
            this.btnModelBonesRemoveGeometry.Name = "btnModelBonesRemoveGeometry";
            this.btnModelBonesRemoveGeometry.Size = new System.Drawing.Size(74, 23);
            this.btnModelBonesRemoveGeometry.TabIndex = 8;
            this.btnModelBonesRemoveGeometry.Text = "Remove";
            this.btnModelBonesRemoveGeometry.UseVisualStyleBackColor = true;
            this.btnModelBonesRemoveGeometry.Click += new System.EventHandler(this.btnModelBonesRemoveGeometry_Click);
            // 
            // tvModelBonesBones
            // 
            this.tvModelBonesBones.HideSelection = false;
            this.tvModelBonesBones.Location = new System.Drawing.Point(6, 6);
            this.tvModelBonesBones.Name = "tvModelBonesBones";
            this.tvModelBonesBones.Size = new System.Drawing.Size(271, 161);
            this.tvModelBonesBones.TabIndex = 0;
            this.tvModelBonesBones.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvModelBonesBones_AfterSelect);
            // 
            // btnModelBonesPasteToBone
            // 
            this.btnModelBonesPasteToBone.Location = new System.Drawing.Point(65, 173);
            this.btnModelBonesPasteToBone.Menu = this.mnuModelBonesPasteToBone;
            this.btnModelBonesPasteToBone.Name = "btnModelBonesPasteToBone";
            this.btnModelBonesPasteToBone.Size = new System.Drawing.Size(60, 23);
            this.btnModelBonesPasteToBone.TabIndex = 18;
            this.btnModelBonesPasteToBone.Text = "Paste";
            this.btnModelBonesPasteToBone.UseVisualStyleBackColor = true;
            // 
            // mnuModelBonesPasteToBone
            // 
            this.mnuModelBonesPasteToBone.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitModelBonesPasteToBonePreserveWeights,
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget});
            this.mnuModelBonesPasteToBone.Name = "mnuModelBonesPasteToBone";
            this.mnuModelBonesPasteToBone.Size = new System.Drawing.Size(237, 48);
            // 
            // mnitModelBonesPasteToBonePreserveWeights
            // 
            this.mnitModelBonesPasteToBonePreserveWeights.Name = "mnitModelBonesPasteToBonePreserveWeights";
            this.mnitModelBonesPasteToBonePreserveWeights.Size = new System.Drawing.Size(236, 22);
            this.mnitModelBonesPasteToBonePreserveWeights.Text = "Preserve Weights";
            this.mnitModelBonesPasteToBonePreserveWeights.Click += new System.EventHandler(this.mnitModelBonesPasteToBonePreserveWeights_Click);
            // 
            // mnitModelBonesPasteToBoneAssignWeightsToTarget
            // 
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget.Name = "mnitModelBonesPasteToBoneAssignWeightsToTarget";
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget.Size = new System.Drawing.Size(236, 22);
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget.Text = "Assign Weights to Target Bone";
            this.mnitModelBonesPasteToBoneAssignWeightsToTarget.Click += new System.EventHandler(this.mnitModelBonesPasteToBoneAssignWeightsToTarget_Click);
            // 
            // btnModelBonesAddBone
            // 
            this.btnModelBonesAddBone.Location = new System.Drawing.Point(6, 173);
            this.btnModelBonesAddBone.Menu = this.mnuModelBonesAddBoneOptions;
            this.btnModelBonesAddBone.Name = "btnModelBonesAddBone";
            this.btnModelBonesAddBone.Size = new System.Drawing.Size(53, 23);
            this.btnModelBonesAddBone.TabIndex = 17;
            this.btnModelBonesAddBone.Text = "Add";
            this.btnModelBonesAddBone.UseVisualStyleBackColor = true;
            // 
            // mnuModelBonesAddBoneOptions
            // 
            this.mnuModelBonesAddBoneOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitModelBonesAddBoneSibling,
            this.mnitModelBonesAddBoneChild});
            this.mnuModelBonesAddBoneOptions.Name = "cntxModelBonesAddBoneOptions";
            this.mnuModelBonesAddBoneOptions.Size = new System.Drawing.Size(111, 48);
            // 
            // mnitModelBonesAddBoneSibling
            // 
            this.mnitModelBonesAddBoneSibling.Name = "mnitModelBonesAddBoneSibling";
            this.mnitModelBonesAddBoneSibling.Size = new System.Drawing.Size(110, 22);
            this.mnitModelBonesAddBoneSibling.Text = "Sibling";
            this.mnitModelBonesAddBoneSibling.Click += new System.EventHandler(this.mnitModelBonesAddBoneSibling_Click);
            // 
            // mnitModelBonesAddBoneChild
            // 
            this.mnitModelBonesAddBoneChild.Name = "mnitModelBonesAddBoneChild";
            this.mnitModelBonesAddBoneChild.Size = new System.Drawing.Size(110, 22);
            this.mnitModelBonesAddBoneChild.Text = "Child";
            this.mnitModelBonesAddBoneChild.Click += new System.EventHandler(this.mnitModelBonesAddBoneChild_Click);
            // 
            // tpgModelMaterials
            // 
            this.tpgModelMaterials.AutoScroll = true;
            this.tpgModelMaterials.Controls.Add(this.txtModelMaterialName);
            this.tpgModelMaterials.Controls.Add(this.grpModelMaterialSettings);
            this.tpgModelMaterials.Controls.Add(this.btnModelMaterialRemoveMaterial);
            this.tpgModelMaterials.Controls.Add(this.btnModelMaterialRenameMaterial);
            this.tpgModelMaterials.Controls.Add(this.btnModelMaterialAddMaterial);
            this.tpgModelMaterials.Controls.Add(this.lbxModelMaterials);
            this.tpgModelMaterials.Location = new System.Drawing.Point(4, 22);
            this.tpgModelMaterials.Name = "tpgModelMaterials";
            this.tpgModelMaterials.Padding = new System.Windows.Forms.Padding(3);
            this.tpgModelMaterials.Size = new System.Drawing.Size(300, 494);
            this.tpgModelMaterials.TabIndex = 2;
            this.tpgModelMaterials.Text = "Materials";
            this.tpgModelMaterials.UseVisualStyleBackColor = true;
            // 
            // txtModelMaterialName
            // 
            this.txtModelMaterialName.Location = new System.Drawing.Point(142, 109);
            this.txtModelMaterialName.Name = "txtModelMaterialName";
            this.txtModelMaterialName.Size = new System.Drawing.Size(135, 20);
            this.txtModelMaterialName.TabIndex = 6;
            // 
            // grpModelMaterialSettings
            // 
            this.grpModelMaterialSettings.Controls.Add(this.btnModelMaterialApplySettings);
            this.grpModelMaterialSettings.Controls.Add(this.grpModelMaterialTextureSettings);
            this.grpModelMaterialSettings.Controls.Add(this.grpModelMaterialColours);
            this.grpModelMaterialSettings.Controls.Add(this.grpModelMaterialFlags);
            this.grpModelMaterialSettings.Controls.Add(this.cmbModelMaterialPolygonMode);
            this.grpModelMaterialSettings.Controls.Add(this.lblModelMaterialPolygonMode);
            this.grpModelMaterialSettings.Controls.Add(this.cmbModelMaterialPolygonDrawingFace);
            this.grpModelMaterialSettings.Controls.Add(this.lblModelMaterialPolygonDrawingFace);
            this.grpModelMaterialSettings.Controls.Add(this.chkModelMaterialLight4);
            this.grpModelMaterialSettings.Controls.Add(this.chkModelMaterialLight3);
            this.grpModelMaterialSettings.Controls.Add(this.chkModelMaterialLight2);
            this.grpModelMaterialSettings.Controls.Add(this.chkModelMaterialLight1);
            this.grpModelMaterialSettings.Controls.Add(this.lblModelMaterialLights);
            this.grpModelMaterialSettings.Location = new System.Drawing.Point(6, 136);
            this.grpModelMaterialSettings.Name = "grpModelMaterialSettings";
            this.grpModelMaterialSettings.Size = new System.Drawing.Size(271, 581);
            this.grpModelMaterialSettings.TabIndex = 5;
            this.grpModelMaterialSettings.TabStop = false;
            this.grpModelMaterialSettings.Text = "Material Settings";
            // 
            // btnModelMaterialApplySettings
            // 
            this.btnModelMaterialApplySettings.Location = new System.Drawing.Point(6, 551);
            this.btnModelMaterialApplySettings.Name = "btnModelMaterialApplySettings";
            this.btnModelMaterialApplySettings.Size = new System.Drawing.Size(75, 23);
            this.btnModelMaterialApplySettings.TabIndex = 16;
            this.btnModelMaterialApplySettings.Text = "Apply";
            this.btnModelMaterialApplySettings.UseVisualStyleBackColor = true;
            this.btnModelMaterialApplySettings.Click += new System.EventHandler(this.btnModelMaterialApplySettings_Click);
            // 
            // grpModelMaterialTextureSettings
            // 
            this.grpModelMaterialTextureSettings.Controls.Add(this.cmbModelMaterialTextureID);
            this.grpModelMaterialTextureSettings.Controls.Add(this.cmbModelMaterialTexGenMode);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTexGenMode);
            this.grpModelMaterialTextureSettings.Controls.Add(this.txtModelMaterialTextureTranslationY);
            this.grpModelMaterialTextureSettings.Controls.Add(this.txtModelMaterialTextureTranslationX);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTextureTranslation);
            this.grpModelMaterialTextureSettings.Controls.Add(this.txtModelMaterialTextureRotation);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTextureRotation);
            this.grpModelMaterialTextureSettings.Controls.Add(this.txtModelMaterialTextureScaleY);
            this.grpModelMaterialTextureSettings.Controls.Add(this.txtModelMaterialTextureScaleX);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTextureScale);
            this.grpModelMaterialTextureSettings.Controls.Add(this.cmbModelMaterialTextureTilingY);
            this.grpModelMaterialTextureSettings.Controls.Add(this.cmbModelMaterialTextureTilingX);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTextureTiling);
            this.grpModelMaterialTextureSettings.Controls.Add(this.chkModelMaterialTextureEnabled);
            this.grpModelMaterialTextureSettings.Controls.Add(this.lblModelMaterialTexture);
            this.grpModelMaterialTextureSettings.Location = new System.Drawing.Point(6, 355);
            this.grpModelMaterialTextureSettings.Name = "grpModelMaterialTextureSettings";
            this.grpModelMaterialTextureSettings.Size = new System.Drawing.Size(259, 190);
            this.grpModelMaterialTextureSettings.TabIndex = 15;
            this.grpModelMaterialTextureSettings.TabStop = false;
            this.grpModelMaterialTextureSettings.Text = "Texture Settings";
            // 
            // cmbModelMaterialTextureID
            // 
            this.cmbModelMaterialTextureID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialTextureID.FormattingEnabled = true;
            this.cmbModelMaterialTextureID.Location = new System.Drawing.Point(119, 37);
            this.cmbModelMaterialTextureID.Name = "cmbModelMaterialTextureID";
            this.cmbModelMaterialTextureID.Size = new System.Drawing.Size(134, 21);
            this.cmbModelMaterialTextureID.TabIndex = 20;
            // 
            // cmbModelMaterialTexGenMode
            // 
            this.cmbModelMaterialTexGenMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialTexGenMode.FormattingEnabled = true;
            this.cmbModelMaterialTexGenMode.Items.AddRange(new object[] {
            "None",
            "Texture Co-ordinates",
            "Normals",
            "Position"});
            this.cmbModelMaterialTexGenMode.Location = new System.Drawing.Point(119, 158);
            this.cmbModelMaterialTexGenMode.Name = "cmbModelMaterialTexGenMode";
            this.cmbModelMaterialTexGenMode.Size = new System.Drawing.Size(134, 21);
            this.cmbModelMaterialTexGenMode.TabIndex = 19;
            // 
            // lblModelMaterialTexGenMode
            // 
            this.lblModelMaterialTexGenMode.AutoSize = true;
            this.lblModelMaterialTexGenMode.Location = new System.Drawing.Point(6, 161);
            this.lblModelMaterialTexGenMode.Name = "lblModelMaterialTexGenMode";
            this.lblModelMaterialTexGenMode.Size = new System.Drawing.Size(87, 13);
            this.lblModelMaterialTexGenMode.TabIndex = 18;
            this.lblModelMaterialTexGenMode.Text = "Co-ord. T. Mode:";
            // 
            // txtModelMaterialTextureTranslationY
            // 
            this.txtModelMaterialTextureTranslationY.Location = new System.Drawing.Point(189, 134);
            this.txtModelMaterialTextureTranslationY.Name = "txtModelMaterialTextureTranslationY";
            this.txtModelMaterialTextureTranslationY.Size = new System.Drawing.Size(64, 20);
            this.txtModelMaterialTextureTranslationY.TabIndex = 17;
            // 
            // txtModelMaterialTextureTranslationX
            // 
            this.txtModelMaterialTextureTranslationX.Location = new System.Drawing.Point(119, 134);
            this.txtModelMaterialTextureTranslationX.Name = "txtModelMaterialTextureTranslationX";
            this.txtModelMaterialTextureTranslationX.Size = new System.Drawing.Size(64, 20);
            this.txtModelMaterialTextureTranslationX.TabIndex = 16;
            // 
            // lblModelMaterialTextureTranslation
            // 
            this.lblModelMaterialTextureTranslation.AutoSize = true;
            this.lblModelMaterialTextureTranslation.Location = new System.Drawing.Point(6, 137);
            this.lblModelMaterialTextureTranslation.Name = "lblModelMaterialTextureTranslation";
            this.lblModelMaterialTextureTranslation.Size = new System.Drawing.Size(62, 13);
            this.lblModelMaterialTextureTranslation.TabIndex = 15;
            this.lblModelMaterialTextureTranslation.Text = "Translation:";
            // 
            // txtModelMaterialTextureRotation
            // 
            this.txtModelMaterialTextureRotation.Location = new System.Drawing.Point(119, 110);
            this.txtModelMaterialTextureRotation.Name = "txtModelMaterialTextureRotation";
            this.txtModelMaterialTextureRotation.Size = new System.Drawing.Size(134, 20);
            this.txtModelMaterialTextureRotation.TabIndex = 14;
            // 
            // lblModelMaterialTextureRotation
            // 
            this.lblModelMaterialTextureRotation.AutoSize = true;
            this.lblModelMaterialTextureRotation.Location = new System.Drawing.Point(6, 113);
            this.lblModelMaterialTextureRotation.Name = "lblModelMaterialTextureRotation";
            this.lblModelMaterialTextureRotation.Size = new System.Drawing.Size(50, 13);
            this.lblModelMaterialTextureRotation.TabIndex = 13;
            this.lblModelMaterialTextureRotation.Text = "Rotation:";
            // 
            // txtModelMaterialTextureScaleY
            // 
            this.txtModelMaterialTextureScaleY.Location = new System.Drawing.Point(189, 86);
            this.txtModelMaterialTextureScaleY.Name = "txtModelMaterialTextureScaleY";
            this.txtModelMaterialTextureScaleY.Size = new System.Drawing.Size(64, 20);
            this.txtModelMaterialTextureScaleY.TabIndex = 12;
            // 
            // txtModelMaterialTextureScaleX
            // 
            this.txtModelMaterialTextureScaleX.Location = new System.Drawing.Point(119, 86);
            this.txtModelMaterialTextureScaleX.Name = "txtModelMaterialTextureScaleX";
            this.txtModelMaterialTextureScaleX.Size = new System.Drawing.Size(64, 20);
            this.txtModelMaterialTextureScaleX.TabIndex = 11;
            // 
            // lblModelMaterialTextureScale
            // 
            this.lblModelMaterialTextureScale.AutoSize = true;
            this.lblModelMaterialTextureScale.Location = new System.Drawing.Point(6, 89);
            this.lblModelMaterialTextureScale.Name = "lblModelMaterialTextureScale";
            this.lblModelMaterialTextureScale.Size = new System.Drawing.Size(37, 13);
            this.lblModelMaterialTextureScale.TabIndex = 10;
            this.lblModelMaterialTextureScale.Text = "Scale:";
            // 
            // cmbModelMaterialTextureTilingY
            // 
            this.cmbModelMaterialTextureTilingY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialTextureTilingY.FormattingEnabled = true;
            this.cmbModelMaterialTextureTilingY.Items.AddRange(new object[] {
            "Clamp",
            "Repeat",
            "Flip"});
            this.cmbModelMaterialTextureTilingY.Location = new System.Drawing.Point(189, 63);
            this.cmbModelMaterialTextureTilingY.Name = "cmbModelMaterialTextureTilingY";
            this.cmbModelMaterialTextureTilingY.Size = new System.Drawing.Size(64, 21);
            this.cmbModelMaterialTextureTilingY.TabIndex = 9;
            // 
            // cmbModelMaterialTextureTilingX
            // 
            this.cmbModelMaterialTextureTilingX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialTextureTilingX.FormattingEnabled = true;
            this.cmbModelMaterialTextureTilingX.Items.AddRange(new object[] {
            "Clamp",
            "Repeat",
            "Flip"});
            this.cmbModelMaterialTextureTilingX.Location = new System.Drawing.Point(119, 63);
            this.cmbModelMaterialTextureTilingX.Name = "cmbModelMaterialTextureTilingX";
            this.cmbModelMaterialTextureTilingX.Size = new System.Drawing.Size(64, 21);
            this.cmbModelMaterialTextureTilingX.TabIndex = 8;
            // 
            // lblModelMaterialTextureTiling
            // 
            this.lblModelMaterialTextureTiling.AutoSize = true;
            this.lblModelMaterialTextureTiling.Location = new System.Drawing.Point(6, 66);
            this.lblModelMaterialTextureTiling.Name = "lblModelMaterialTextureTiling";
            this.lblModelMaterialTextureTiling.Size = new System.Drawing.Size(35, 13);
            this.lblModelMaterialTextureTiling.TabIndex = 7;
            this.lblModelMaterialTextureTiling.Text = "Tiling:";
            // 
            // chkModelMaterialTextureEnabled
            // 
            this.chkModelMaterialTextureEnabled.AutoSize = true;
            this.chkModelMaterialTextureEnabled.Location = new System.Drawing.Point(6, 19);
            this.chkModelMaterialTextureEnabled.Name = "chkModelMaterialTextureEnabled";
            this.chkModelMaterialTextureEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkModelMaterialTextureEnabled.TabIndex = 0;
            this.chkModelMaterialTextureEnabled.Text = "Enabled";
            this.chkModelMaterialTextureEnabled.UseVisualStyleBackColor = true;
            this.chkModelMaterialTextureEnabled.CheckedChanged += new System.EventHandler(this.chkModelMaterialTextureEnabled_CheckedChanged);
            // 
            // lblModelMaterialTexture
            // 
            this.lblModelMaterialTexture.AutoSize = true;
            this.lblModelMaterialTexture.Location = new System.Drawing.Point(6, 40);
            this.lblModelMaterialTexture.Name = "lblModelMaterialTexture";
            this.lblModelMaterialTexture.Size = new System.Drawing.Size(21, 13);
            this.lblModelMaterialTexture.TabIndex = 5;
            this.lblModelMaterialTexture.Text = "ID:";
            // 
            // grpModelMaterialColours
            // 
            this.grpModelMaterialColours.Controls.Add(this.btnModelMaterialEmission);
            this.grpModelMaterialColours.Controls.Add(this.lblModelMaterialEmission);
            this.grpModelMaterialColours.Controls.Add(this.btnModelMaterialSpecular);
            this.grpModelMaterialColours.Controls.Add(this.lblModelMaterialSpecular);
            this.grpModelMaterialColours.Controls.Add(this.nudModelMaterialAlpha);
            this.grpModelMaterialColours.Controls.Add(this.btnModelMaterialAmbient);
            this.grpModelMaterialColours.Controls.Add(this.lblModelMaterialAlpha);
            this.grpModelMaterialColours.Controls.Add(this.lblModelMaterialAmbient);
            this.grpModelMaterialColours.Controls.Add(this.btnModelMaterialDiffuse);
            this.grpModelMaterialColours.Controls.Add(this.lblModelMaterialDiffuse);
            this.grpModelMaterialColours.Location = new System.Drawing.Point(6, 250);
            this.grpModelMaterialColours.Name = "grpModelMaterialColours";
            this.grpModelMaterialColours.Size = new System.Drawing.Size(259, 99);
            this.grpModelMaterialColours.TabIndex = 14;
            this.grpModelMaterialColours.TabStop = false;
            this.grpModelMaterialColours.Text = "Colours";
            // 
            // btnModelMaterialEmission
            // 
            this.btnModelMaterialEmission.Location = new System.Drawing.Point(170, 40);
            this.btnModelMaterialEmission.Name = "btnModelMaterialEmission";
            this.btnModelMaterialEmission.Size = new System.Drawing.Size(60, 23);
            this.btnModelMaterialEmission.TabIndex = 7;
            this.btnModelMaterialEmission.Text = "#XXXXXX";
            this.btnModelMaterialEmission.UseVisualStyleBackColor = true;
            this.btnModelMaterialEmission.Click += new System.EventHandler(this.btnModelMaterialEmission_Click);
            // 
            // lblModelMaterialEmission
            // 
            this.lblModelMaterialEmission.AutoSize = true;
            this.lblModelMaterialEmission.Location = new System.Drawing.Point(121, 45);
            this.lblModelMaterialEmission.Name = "lblModelMaterialEmission";
            this.lblModelMaterialEmission.Size = new System.Drawing.Size(51, 13);
            this.lblModelMaterialEmission.TabIndex = 6;
            this.lblModelMaterialEmission.Text = "Emission:";
            // 
            // btnModelMaterialSpecular
            // 
            this.btnModelMaterialSpecular.Location = new System.Drawing.Point(55, 40);
            this.btnModelMaterialSpecular.Name = "btnModelMaterialSpecular";
            this.btnModelMaterialSpecular.Size = new System.Drawing.Size(60, 23);
            this.btnModelMaterialSpecular.TabIndex = 5;
            this.btnModelMaterialSpecular.Text = "#XXXXXX";
            this.btnModelMaterialSpecular.UseVisualStyleBackColor = true;
            this.btnModelMaterialSpecular.Click += new System.EventHandler(this.btnModelMaterialSpecular_Click);
            // 
            // lblModelMaterialSpecular
            // 
            this.lblModelMaterialSpecular.AutoSize = true;
            this.lblModelMaterialSpecular.Location = new System.Drawing.Point(6, 45);
            this.lblModelMaterialSpecular.Name = "lblModelMaterialSpecular";
            this.lblModelMaterialSpecular.Size = new System.Drawing.Size(52, 13);
            this.lblModelMaterialSpecular.TabIndex = 4;
            this.lblModelMaterialSpecular.Text = "Specular:";
            // 
            // nudModelMaterialAlpha
            // 
            this.nudModelMaterialAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nudModelMaterialAlpha.Location = new System.Drawing.Point(55, 72);
            this.nudModelMaterialAlpha.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudModelMaterialAlpha.Name = "nudModelMaterialAlpha";
            this.nudModelMaterialAlpha.Size = new System.Drawing.Size(79, 20);
            this.nudModelMaterialAlpha.TabIndex = 10;
            // 
            // btnModelMaterialAmbient
            // 
            this.btnModelMaterialAmbient.Location = new System.Drawing.Point(170, 11);
            this.btnModelMaterialAmbient.Name = "btnModelMaterialAmbient";
            this.btnModelMaterialAmbient.Size = new System.Drawing.Size(60, 23);
            this.btnModelMaterialAmbient.TabIndex = 3;
            this.btnModelMaterialAmbient.Text = "#XXXXXX";
            this.btnModelMaterialAmbient.UseVisualStyleBackColor = true;
            this.btnModelMaterialAmbient.Click += new System.EventHandler(this.btnModelMaterialAmbient_Click);
            // 
            // lblModelMaterialAlpha
            // 
            this.lblModelMaterialAlpha.AutoSize = true;
            this.lblModelMaterialAlpha.Location = new System.Drawing.Point(6, 74);
            this.lblModelMaterialAlpha.Name = "lblModelMaterialAlpha";
            this.lblModelMaterialAlpha.Size = new System.Drawing.Size(37, 13);
            this.lblModelMaterialAlpha.TabIndex = 9;
            this.lblModelMaterialAlpha.Text = "Alpha:";
            // 
            // lblModelMaterialAmbient
            // 
            this.lblModelMaterialAmbient.AutoSize = true;
            this.lblModelMaterialAmbient.Location = new System.Drawing.Point(121, 16);
            this.lblModelMaterialAmbient.Name = "lblModelMaterialAmbient";
            this.lblModelMaterialAmbient.Size = new System.Drawing.Size(48, 13);
            this.lblModelMaterialAmbient.TabIndex = 2;
            this.lblModelMaterialAmbient.Text = "Ambient:";
            // 
            // btnModelMaterialDiffuse
            // 
            this.btnModelMaterialDiffuse.Location = new System.Drawing.Point(55, 11);
            this.btnModelMaterialDiffuse.Name = "btnModelMaterialDiffuse";
            this.btnModelMaterialDiffuse.Size = new System.Drawing.Size(60, 23);
            this.btnModelMaterialDiffuse.TabIndex = 1;
            this.btnModelMaterialDiffuse.Text = "#XXXXXX";
            this.btnModelMaterialDiffuse.UseVisualStyleBackColor = true;
            this.btnModelMaterialDiffuse.Click += new System.EventHandler(this.btnModelMaterialDiffuse_Click);
            // 
            // lblModelMaterialDiffuse
            // 
            this.lblModelMaterialDiffuse.AutoSize = true;
            this.lblModelMaterialDiffuse.Location = new System.Drawing.Point(6, 16);
            this.lblModelMaterialDiffuse.Name = "lblModelMaterialDiffuse";
            this.lblModelMaterialDiffuse.Size = new System.Drawing.Size(43, 13);
            this.lblModelMaterialDiffuse.TabIndex = 0;
            this.lblModelMaterialDiffuse.Text = "Diffuse:";
            // 
            // grpModelMaterialFlags
            // 
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialShiniessTable);
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialFarClipping);
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialRenderOnePixelPolygons);
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialDepthTestDecal);
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialFog);
            this.grpModelMaterialFlags.Controls.Add(this.chkModelMaterialWireMode);
            this.grpModelMaterialFlags.Location = new System.Drawing.Point(6, 86);
            this.grpModelMaterialFlags.Name = "grpModelMaterialFlags";
            this.grpModelMaterialFlags.Size = new System.Drawing.Size(259, 158);
            this.grpModelMaterialFlags.TabIndex = 13;
            this.grpModelMaterialFlags.TabStop = false;
            this.grpModelMaterialFlags.Text = "Flags";
            // 
            // chkModelMaterialShiniessTable
            // 
            this.chkModelMaterialShiniessTable.AutoSize = true;
            this.chkModelMaterialShiniessTable.Location = new System.Drawing.Point(6, 134);
            this.chkModelMaterialShiniessTable.Name = "chkModelMaterialShiniessTable";
            this.chkModelMaterialShiniessTable.Size = new System.Drawing.Size(259, 17);
            this.chkModelMaterialShiniessTable.TabIndex = 5;
            this.chkModelMaterialShiniessTable.Text = "Use Reflection Table to Calculate Specular Elem.";
            this.chkModelMaterialShiniessTable.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialFarClipping
            // 
            this.chkModelMaterialFarClipping.AutoSize = true;
            this.chkModelMaterialFarClipping.Location = new System.Drawing.Point(6, 65);
            this.chkModelMaterialFarClipping.Name = "chkModelMaterialFarClipping";
            this.chkModelMaterialFarClipping.Size = new System.Drawing.Size(111, 17);
            this.chkModelMaterialFarClipping.TabIndex = 4;
            this.chkModelMaterialFarClipping.Text = "Far Plane Clipping";
            this.chkModelMaterialFarClipping.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialRenderOnePixelPolygons
            // 
            this.chkModelMaterialRenderOnePixelPolygons.AutoSize = true;
            this.chkModelMaterialRenderOnePixelPolygons.Location = new System.Drawing.Point(6, 111);
            this.chkModelMaterialRenderOnePixelPolygons.Name = "chkModelMaterialRenderOnePixelPolygons";
            this.chkModelMaterialRenderOnePixelPolygons.Size = new System.Drawing.Size(141, 17);
            this.chkModelMaterialRenderOnePixelPolygons.TabIndex = 3;
            this.chkModelMaterialRenderOnePixelPolygons.Text = "Render 1-Pixel Polygons";
            this.chkModelMaterialRenderOnePixelPolygons.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialDepthTestDecal
            // 
            this.chkModelMaterialDepthTestDecal.AutoSize = true;
            this.chkModelMaterialDepthTestDecal.Location = new System.Drawing.Point(6, 88);
            this.chkModelMaterialDepthTestDecal.Name = "chkModelMaterialDepthTestDecal";
            this.chkModelMaterialDepthTestDecal.Size = new System.Drawing.Size(253, 17);
            this.chkModelMaterialDepthTestDecal.TabIndex = 2;
            this.chkModelMaterialDepthTestDecal.Text = "Only Render if Depth Value Equals Depth Buffer";
            this.chkModelMaterialDepthTestDecal.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialFog
            // 
            this.chkModelMaterialFog.AutoSize = true;
            this.chkModelMaterialFog.Location = new System.Drawing.Point(6, 42);
            this.chkModelMaterialFog.Name = "chkModelMaterialFog";
            this.chkModelMaterialFog.Size = new System.Drawing.Size(80, 17);
            this.chkModelMaterialFog.TabIndex = 1;
            this.chkModelMaterialFog.Text = "Enable Fog";
            this.chkModelMaterialFog.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialWireMode
            // 
            this.chkModelMaterialWireMode.AutoSize = true;
            this.chkModelMaterialWireMode.Location = new System.Drawing.Point(6, 19);
            this.chkModelMaterialWireMode.Name = "chkModelMaterialWireMode";
            this.chkModelMaterialWireMode.Size = new System.Drawing.Size(111, 17);
            this.chkModelMaterialWireMode.TabIndex = 0;
            this.chkModelMaterialWireMode.Text = "Display Wireframe";
            this.chkModelMaterialWireMode.UseVisualStyleBackColor = true;
            this.chkModelMaterialWireMode.CheckedChanged += new System.EventHandler(this.chkModelMaterialWireMode_CheckedChanged);
            // 
            // cmbModelMaterialPolygonMode
            // 
            this.cmbModelMaterialPolygonMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModelMaterialPolygonMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialPolygonMode.FormattingEnabled = true;
            this.cmbModelMaterialPolygonMode.Items.AddRange(new object[] {
            "Modulation",
            "Decal",
            "Toon Highlight Shading",
            "Shadow"});
            this.cmbModelMaterialPolygonMode.Location = new System.Drawing.Point(144, 59);
            this.cmbModelMaterialPolygonMode.Name = "cmbModelMaterialPolygonMode";
            this.cmbModelMaterialPolygonMode.Size = new System.Drawing.Size(121, 21);
            this.cmbModelMaterialPolygonMode.TabIndex = 12;
            // 
            // lblModelMaterialPolygonMode
            // 
            this.lblModelMaterialPolygonMode.AutoSize = true;
            this.lblModelMaterialPolygonMode.Location = new System.Drawing.Point(4, 62);
            this.lblModelMaterialPolygonMode.Name = "lblModelMaterialPolygonMode";
            this.lblModelMaterialPolygonMode.Size = new System.Drawing.Size(78, 13);
            this.lblModelMaterialPolygonMode.TabIndex = 11;
            this.lblModelMaterialPolygonMode.Text = "Polygon Mode:";
            // 
            // cmbModelMaterialPolygonDrawingFace
            // 
            this.cmbModelMaterialPolygonDrawingFace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModelMaterialPolygonDrawingFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelMaterialPolygonDrawingFace.FormattingEnabled = true;
            this.cmbModelMaterialPolygonDrawingFace.Items.AddRange(new object[] {
            "Front",
            "Back",
            "Both"});
            this.cmbModelMaterialPolygonDrawingFace.Location = new System.Drawing.Point(144, 36);
            this.cmbModelMaterialPolygonDrawingFace.Name = "cmbModelMaterialPolygonDrawingFace";
            this.cmbModelMaterialPolygonDrawingFace.Size = new System.Drawing.Size(121, 21);
            this.cmbModelMaterialPolygonDrawingFace.TabIndex = 8;
            // 
            // lblModelMaterialPolygonDrawingFace
            // 
            this.lblModelMaterialPolygonDrawingFace.AutoSize = true;
            this.lblModelMaterialPolygonDrawingFace.Location = new System.Drawing.Point(4, 39);
            this.lblModelMaterialPolygonDrawingFace.Name = "lblModelMaterialPolygonDrawingFace";
            this.lblModelMaterialPolygonDrawingFace.Size = new System.Drawing.Size(117, 13);
            this.lblModelMaterialPolygonDrawingFace.TabIndex = 7;
            this.lblModelMaterialPolygonDrawingFace.Text = "Polygon Drawing Face:";
            // 
            // chkModelMaterialLight4
            // 
            this.chkModelMaterialLight4.AutoSize = true;
            this.chkModelMaterialLight4.Location = new System.Drawing.Point(233, 19);
            this.chkModelMaterialLight4.Name = "chkModelMaterialLight4";
            this.chkModelMaterialLight4.Size = new System.Drawing.Size(32, 17);
            this.chkModelMaterialLight4.TabIndex = 4;
            this.chkModelMaterialLight4.Text = "4";
            this.chkModelMaterialLight4.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialLight3
            // 
            this.chkModelMaterialLight3.AutoSize = true;
            this.chkModelMaterialLight3.Location = new System.Drawing.Point(195, 19);
            this.chkModelMaterialLight3.Name = "chkModelMaterialLight3";
            this.chkModelMaterialLight3.Size = new System.Drawing.Size(32, 17);
            this.chkModelMaterialLight3.TabIndex = 3;
            this.chkModelMaterialLight3.Text = "3";
            this.chkModelMaterialLight3.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialLight2
            // 
            this.chkModelMaterialLight2.AutoSize = true;
            this.chkModelMaterialLight2.Location = new System.Drawing.Point(157, 19);
            this.chkModelMaterialLight2.Name = "chkModelMaterialLight2";
            this.chkModelMaterialLight2.Size = new System.Drawing.Size(32, 17);
            this.chkModelMaterialLight2.TabIndex = 2;
            this.chkModelMaterialLight2.Text = "2";
            this.chkModelMaterialLight2.UseVisualStyleBackColor = true;
            // 
            // chkModelMaterialLight1
            // 
            this.chkModelMaterialLight1.AutoSize = true;
            this.chkModelMaterialLight1.Location = new System.Drawing.Point(119, 19);
            this.chkModelMaterialLight1.Name = "chkModelMaterialLight1";
            this.chkModelMaterialLight1.Size = new System.Drawing.Size(32, 17);
            this.chkModelMaterialLight1.TabIndex = 1;
            this.chkModelMaterialLight1.Text = "1";
            this.chkModelMaterialLight1.UseVisualStyleBackColor = true;
            // 
            // lblModelMaterialLights
            // 
            this.lblModelMaterialLights.AutoSize = true;
            this.lblModelMaterialLights.Location = new System.Drawing.Point(4, 20);
            this.lblModelMaterialLights.Name = "lblModelMaterialLights";
            this.lblModelMaterialLights.Size = new System.Drawing.Size(38, 13);
            this.lblModelMaterialLights.TabIndex = 0;
            this.lblModelMaterialLights.Text = "Lights:";
            // 
            // btnModelMaterialRemoveMaterial
            // 
            this.btnModelMaterialRemoveMaterial.Location = new System.Drawing.Point(32, 107);
            this.btnModelMaterialRemoveMaterial.Name = "btnModelMaterialRemoveMaterial";
            this.btnModelMaterialRemoveMaterial.Size = new System.Drawing.Size(23, 23);
            this.btnModelMaterialRemoveMaterial.TabIndex = 4;
            this.btnModelMaterialRemoveMaterial.Text = "-";
            this.btnModelMaterialRemoveMaterial.UseVisualStyleBackColor = true;
            this.btnModelMaterialRemoveMaterial.Click += new System.EventHandler(this.btnModelMaterialRemoveMaterial_Click);
            // 
            // btnModelMaterialRenameMaterial
            // 
            this.btnModelMaterialRenameMaterial.Location = new System.Drawing.Point(61, 107);
            this.btnModelMaterialRenameMaterial.Name = "btnModelMaterialRenameMaterial";
            this.btnModelMaterialRenameMaterial.Size = new System.Drawing.Size(75, 23);
            this.btnModelMaterialRenameMaterial.TabIndex = 3;
            this.btnModelMaterialRenameMaterial.Text = "Rename";
            this.btnModelMaterialRenameMaterial.UseVisualStyleBackColor = true;
            this.btnModelMaterialRenameMaterial.Click += new System.EventHandler(this.btnModelMaterialRenameMaterial_Click);
            // 
            // btnModelMaterialAddMaterial
            // 
            this.btnModelMaterialAddMaterial.Location = new System.Drawing.Point(6, 107);
            this.btnModelMaterialAddMaterial.Name = "btnModelMaterialAddMaterial";
            this.btnModelMaterialAddMaterial.Size = new System.Drawing.Size(23, 23);
            this.btnModelMaterialAddMaterial.TabIndex = 2;
            this.btnModelMaterialAddMaterial.Text = "+";
            this.btnModelMaterialAddMaterial.UseVisualStyleBackColor = true;
            this.btnModelMaterialAddMaterial.Click += new System.EventHandler(this.btnModelMaterialAddMaterial_Click);
            // 
            // lbxModelMaterials
            // 
            this.lbxModelMaterials.FormattingEnabled = true;
            this.lbxModelMaterials.Location = new System.Drawing.Point(6, 6);
            this.lbxModelMaterials.Name = "lbxModelMaterials";
            this.lbxModelMaterials.Size = new System.Drawing.Size(271, 95);
            this.lbxModelMaterials.TabIndex = 1;
            this.lbxModelMaterials.SelectedIndexChanged += new System.EventHandler(this.lbxModelMaterials_SelectedIndexChanged);
            // 
            // tpgModelTextures
            // 
            this.tpgModelTextures.Controls.Add(this.btnModelTexturesReplace);
            this.tpgModelTextures.Controls.Add(this.grpModelTexturesPreview);
            this.tpgModelTextures.Controls.Add(this.grpModelTexturesSettings);
            this.tpgModelTextures.Controls.Add(this.lbxModelTextures);
            this.tpgModelTextures.Controls.Add(this.btnModelTexturesAddTexture);
            this.tpgModelTextures.Controls.Add(this.btnModelTexturesRenameTexture);
            this.tpgModelTextures.Controls.Add(this.txtModelTexturesName);
            this.tpgModelTextures.Controls.Add(this.btnModelTexturesRemoveTexture);
            this.tpgModelTextures.Location = new System.Drawing.Point(4, 22);
            this.tpgModelTextures.Name = "tpgModelTextures";
            this.tpgModelTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tpgModelTextures.Size = new System.Drawing.Size(300, 494);
            this.tpgModelTextures.TabIndex = 3;
            this.tpgModelTextures.Text = "Textures";
            this.tpgModelTextures.UseVisualStyleBackColor = true;
            // 
            // btnModelTexturesReplace
            // 
            this.btnModelTexturesReplace.Location = new System.Drawing.Point(64, 120);
            this.btnModelTexturesReplace.Name = "btnModelTexturesReplace";
            this.btnModelTexturesReplace.Size = new System.Drawing.Size(61, 23);
            this.btnModelTexturesReplace.TabIndex = 6;
            this.btnModelTexturesReplace.Text = "Replace";
            this.btnModelTexturesReplace.UseVisualStyleBackColor = true;
            this.btnModelTexturesReplace.Click += new System.EventHandler(this.btnModelTexturesReplace_Click);
            // 
            // grpModelTexturesPreview
            // 
            this.grpModelTexturesPreview.Controls.Add(this.pnlModelTexturesPreviewPanel);
            this.grpModelTexturesPreview.Location = new System.Drawing.Point(6, 245);
            this.grpModelTexturesPreview.Name = "grpModelTexturesPreview";
            this.grpModelTexturesPreview.Size = new System.Drawing.Size(288, 247);
            this.grpModelTexturesPreview.TabIndex = 5;
            this.grpModelTexturesPreview.TabStop = false;
            this.grpModelTexturesPreview.Text = "Preview";
            // 
            // pnlModelTexturesPreviewPanel
            // 
            this.pnlModelTexturesPreviewPanel.AutoScroll = true;
            this.pnlModelTexturesPreviewPanel.Controls.Add(this.pbxModelTexturesPreview);
            this.pnlModelTexturesPreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlModelTexturesPreviewPanel.Location = new System.Drawing.Point(3, 16);
            this.pnlModelTexturesPreviewPanel.Name = "pnlModelTexturesPreviewPanel";
            this.pnlModelTexturesPreviewPanel.Size = new System.Drawing.Size(282, 228);
            this.pnlModelTexturesPreviewPanel.TabIndex = 1;
            // 
            // pbxModelTexturesPreview
            // 
            this.pbxModelTexturesPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxModelTexturesPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxModelTexturesPreview.Location = new System.Drawing.Point(0, 0);
            this.pbxModelTexturesPreview.Name = "pbxModelTexturesPreview";
            this.pbxModelTexturesPreview.Size = new System.Drawing.Size(282, 228);
            this.pbxModelTexturesPreview.TabIndex = 0;
            this.pbxModelTexturesPreview.TabStop = false;
            // 
            // grpModelTexturesSettings
            // 
            this.grpModelTexturesSettings.Controls.Add(this.cmbModelTexturesPalette);
            this.grpModelTexturesSettings.Controls.Add(this.lblModelTexturesPalette);
            this.grpModelTexturesSettings.Controls.Add(this.cmbModelTexturesHeight);
            this.grpModelTexturesSettings.Controls.Add(this.cmbModelTexturesWidth);
            this.grpModelTexturesSettings.Controls.Add(this.lblModelTexturesDimensions);
            this.grpModelTexturesSettings.Controls.Add(this.cmbModelTexturesFormat);
            this.grpModelTexturesSettings.Controls.Add(this.lblModelTexturesFormat);
            this.grpModelTexturesSettings.Location = new System.Drawing.Point(6, 149);
            this.grpModelTexturesSettings.Name = "grpModelTexturesSettings";
            this.grpModelTexturesSettings.Size = new System.Drawing.Size(288, 90);
            this.grpModelTexturesSettings.TabIndex = 0;
            this.grpModelTexturesSettings.TabStop = false;
            this.grpModelTexturesSettings.Text = "Texture Settings";
            // 
            // cmbModelTexturesPalette
            // 
            this.cmbModelTexturesPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelTexturesPalette.FormattingEnabled = true;
            this.cmbModelTexturesPalette.Items.AddRange(new object[] {
            "A3I5 Translucent",
            "4-Colour Palette",
            "16-Colour Palette",
            "256-Colour Palette",
            "4x4-Texel Compressed",
            "A5I3 Translucent",
            "Direct"});
            this.cmbModelTexturesPalette.Location = new System.Drawing.Point(137, 60);
            this.cmbModelTexturesPalette.Name = "cmbModelTexturesPalette";
            this.cmbModelTexturesPalette.Size = new System.Drawing.Size(145, 21);
            this.cmbModelTexturesPalette.TabIndex = 11;
            this.cmbModelTexturesPalette.SelectedIndexChanged += new System.EventHandler(this.cmbModelTexturesPalette_SelectedIndexChanged);
            // 
            // lblModelTexturesPalette
            // 
            this.lblModelTexturesPalette.AutoSize = true;
            this.lblModelTexturesPalette.Location = new System.Drawing.Point(6, 63);
            this.lblModelTexturesPalette.Name = "lblModelTexturesPalette";
            this.lblModelTexturesPalette.Size = new System.Drawing.Size(43, 13);
            this.lblModelTexturesPalette.TabIndex = 10;
            this.lblModelTexturesPalette.Text = "Palette:";
            // 
            // cmbModelTexturesHeight
            // 
            this.cmbModelTexturesHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelTexturesHeight.FormattingEnabled = true;
            this.cmbModelTexturesHeight.Location = new System.Drawing.Point(210, 36);
            this.cmbModelTexturesHeight.Name = "cmbModelTexturesHeight";
            this.cmbModelTexturesHeight.Size = new System.Drawing.Size(72, 21);
            this.cmbModelTexturesHeight.TabIndex = 9;
            this.cmbModelTexturesHeight.SelectedIndexChanged += new System.EventHandler(this.cmbModelTexturesHeight_SelectedIndexChanged);
            // 
            // cmbModelTexturesWidth
            // 
            this.cmbModelTexturesWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelTexturesWidth.FormattingEnabled = true;
            this.cmbModelTexturesWidth.Location = new System.Drawing.Point(137, 36);
            this.cmbModelTexturesWidth.Name = "cmbModelTexturesWidth";
            this.cmbModelTexturesWidth.Size = new System.Drawing.Size(72, 21);
            this.cmbModelTexturesWidth.TabIndex = 8;
            this.cmbModelTexturesWidth.SelectedIndexChanged += new System.EventHandler(this.cmbModelTexturesWidth_SelectedIndexChanged);
            // 
            // lblModelTexturesDimensions
            // 
            this.lblModelTexturesDimensions.AutoSize = true;
            this.lblModelTexturesDimensions.Location = new System.Drawing.Point(6, 39);
            this.lblModelTexturesDimensions.Name = "lblModelTexturesDimensions";
            this.lblModelTexturesDimensions.Size = new System.Drawing.Size(64, 13);
            this.lblModelTexturesDimensions.TabIndex = 7;
            this.lblModelTexturesDimensions.Text = "Dimensions:";
            // 
            // cmbModelTexturesFormat
            // 
            this.cmbModelTexturesFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelTexturesFormat.Enabled = false;
            this.cmbModelTexturesFormat.FormattingEnabled = true;
            this.cmbModelTexturesFormat.Items.AddRange(new object[] {
            "A3I5 Translucent",
            "4-Colour Palette",
            "16-Colour Palette",
            "256-Colour Palette",
            "4x4-Texel Compressed",
            "A5I3 Translucent",
            "Direct"});
            this.cmbModelTexturesFormat.Location = new System.Drawing.Point(137, 13);
            this.cmbModelTexturesFormat.Name = "cmbModelTexturesFormat";
            this.cmbModelTexturesFormat.Size = new System.Drawing.Size(145, 21);
            this.cmbModelTexturesFormat.TabIndex = 6;
            // 
            // lblModelTexturesFormat
            // 
            this.lblModelTexturesFormat.AutoSize = true;
            this.lblModelTexturesFormat.Location = new System.Drawing.Point(6, 16);
            this.lblModelTexturesFormat.Name = "lblModelTexturesFormat";
            this.lblModelTexturesFormat.Size = new System.Drawing.Size(42, 13);
            this.lblModelTexturesFormat.TabIndex = 5;
            this.lblModelTexturesFormat.Text = "Format:";
            // 
            // lbxModelTextures
            // 
            this.lbxModelTextures.FormattingEnabled = true;
            this.lbxModelTextures.Location = new System.Drawing.Point(6, 6);
            this.lbxModelTextures.Name = "lbxModelTextures";
            this.lbxModelTextures.Size = new System.Drawing.Size(288, 108);
            this.lbxModelTextures.TabIndex = 0;
            this.lbxModelTextures.SelectedIndexChanged += new System.EventHandler(this.lbxModelTextures_SelectedIndexChanged);
            // 
            // btnModelTexturesAddTexture
            // 
            this.btnModelTexturesAddTexture.Location = new System.Drawing.Point(6, 120);
            this.btnModelTexturesAddTexture.Name = "btnModelTexturesAddTexture";
            this.btnModelTexturesAddTexture.Size = new System.Drawing.Size(23, 23);
            this.btnModelTexturesAddTexture.TabIndex = 1;
            this.btnModelTexturesAddTexture.Text = "+";
            this.btnModelTexturesAddTexture.UseVisualStyleBackColor = true;
            this.btnModelTexturesAddTexture.Click += new System.EventHandler(this.btnModelTexturesAddTexture_Click);
            // 
            // btnModelTexturesRenameTexture
            // 
            this.btnModelTexturesRenameTexture.Location = new System.Drawing.Point(127, 120);
            this.btnModelTexturesRenameTexture.Name = "btnModelTexturesRenameTexture";
            this.btnModelTexturesRenameTexture.Size = new System.Drawing.Size(56, 23);
            this.btnModelTexturesRenameTexture.TabIndex = 2;
            this.btnModelTexturesRenameTexture.Text = "Rename";
            this.btnModelTexturesRenameTexture.UseVisualStyleBackColor = true;
            this.btnModelTexturesRenameTexture.Click += new System.EventHandler(this.btnModelTexturesRenameTexture_Click);
            // 
            // txtModelTexturesName
            // 
            this.txtModelTexturesName.Location = new System.Drawing.Point(187, 122);
            this.txtModelTexturesName.Name = "txtModelTexturesName";
            this.txtModelTexturesName.Size = new System.Drawing.Size(107, 20);
            this.txtModelTexturesName.TabIndex = 3;
            // 
            // btnModelTexturesRemoveTexture
            // 
            this.btnModelTexturesRemoveTexture.Location = new System.Drawing.Point(35, 120);
            this.btnModelTexturesRemoveTexture.Name = "btnModelTexturesRemoveTexture";
            this.btnModelTexturesRemoveTexture.Size = new System.Drawing.Size(23, 23);
            this.btnModelTexturesRemoveTexture.TabIndex = 4;
            this.btnModelTexturesRemoveTexture.Text = "-";
            this.btnModelTexturesRemoveTexture.UseVisualStyleBackColor = true;
            this.btnModelTexturesRemoveTexture.Click += new System.EventHandler(this.btnModelTexturesRemoveTexture_Click);
            // 
            // tpgPalettes
            // 
            this.tpgPalettes.Controls.Add(this.grpModelPalettesColours);
            this.tpgPalettes.Controls.Add(this.lbxModelPalettes);
            this.tpgPalettes.Controls.Add(this.btnModelPalettesRenamePalette);
            this.tpgPalettes.Controls.Add(this.txtModelPalettesName);
            this.tpgPalettes.Location = new System.Drawing.Point(4, 22);
            this.tpgPalettes.Name = "tpgPalettes";
            this.tpgPalettes.Padding = new System.Windows.Forms.Padding(3);
            this.tpgPalettes.Size = new System.Drawing.Size(300, 494);
            this.tpgPalettes.TabIndex = 4;
            this.tpgPalettes.Text = "Palettes";
            this.tpgPalettes.UseVisualStyleBackColor = true;
            // 
            // grpModelPalettesColours
            // 
            this.grpModelPalettesColours.Controls.Add(this.gridModelPalettesPaletteColours);
            this.grpModelPalettesColours.Controls.Add(this.btnModelPalettesSelectedColour);
            this.grpModelPalettesColours.Controls.Add(this.lblModelPalettesPaletteSelectedColour);
            this.grpModelPalettesColours.Location = new System.Drawing.Point(6, 159);
            this.grpModelPalettesColours.Name = "grpModelPalettesColours";
            this.grpModelPalettesColours.Size = new System.Drawing.Size(288, 316);
            this.grpModelPalettesColours.TabIndex = 11;
            this.grpModelPalettesColours.TabStop = false;
            this.grpModelPalettesColours.Text = "Colours";
            // 
            // gridModelPalettesPaletteColours
            // 
            this.gridModelPalettesPaletteColours.AllowUserToAddRows = false;
            this.gridModelPalettesPaletteColours.AllowUserToDeleteRows = false;
            this.gridModelPalettesPaletteColours.AllowUserToResizeColumns = false;
            this.gridModelPalettesPaletteColours.AllowUserToResizeRows = false;
            this.gridModelPalettesPaletteColours.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridModelPalettesPaletteColours.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridModelPalettesPaletteColours.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridModelPalettesPaletteColours.ColumnHeadersVisible = false;
            this.gridModelPalettesPaletteColours.EnableHeadersVisualStyles = false;
            this.gridModelPalettesPaletteColours.Location = new System.Drawing.Point(6, 19);
            this.gridModelPalettesPaletteColours.MultiSelect = false;
            this.gridModelPalettesPaletteColours.Name = "gridModelPalettesPaletteColours";
            this.gridModelPalettesPaletteColours.ReadOnly = true;
            this.gridModelPalettesPaletteColours.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridModelPalettesPaletteColours.RowHeadersVisible = false;
            this.gridModelPalettesPaletteColours.RowHeadersWidth = 23;
            this.gridModelPalettesPaletteColours.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridModelPalettesPaletteColours.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridModelPalettesPaletteColours.Size = new System.Drawing.Size(260, 260);
            this.gridModelPalettesPaletteColours.TabIndex = 8;
            this.gridModelPalettesPaletteColours.CurrentCellChanged += new System.EventHandler(this.gridModelPalettesPaletteColours_CurrentCellChanged);
            // 
            // btnModelPalettesSelectedColour
            // 
            this.btnModelPalettesSelectedColour.Location = new System.Drawing.Point(191, 285);
            this.btnModelPalettesSelectedColour.Name = "btnModelPalettesSelectedColour";
            this.btnModelPalettesSelectedColour.Size = new System.Drawing.Size(75, 23);
            this.btnModelPalettesSelectedColour.TabIndex = 9;
            this.btnModelPalettesSelectedColour.Text = "#XXXXXX";
            this.btnModelPalettesSelectedColour.UseVisualStyleBackColor = true;
            this.btnModelPalettesSelectedColour.Click += new System.EventHandler(this.btnModelPalettesSelectedColour_Click);
            // 
            // lblModelPalettesPaletteSelectedColour
            // 
            this.lblModelPalettesPaletteSelectedColour.AutoSize = true;
            this.lblModelPalettesPaletteSelectedColour.Location = new System.Drawing.Point(3, 290);
            this.lblModelPalettesPaletteSelectedColour.Name = "lblModelPalettesPaletteSelectedColour";
            this.lblModelPalettesPaletteSelectedColour.Size = new System.Drawing.Size(85, 13);
            this.lblModelPalettesPaletteSelectedColour.TabIndex = 10;
            this.lblModelPalettesPaletteSelectedColour.Text = "Selected Colour:";
            // 
            // lbxModelPalettes
            // 
            this.lbxModelPalettes.FormattingEnabled = true;
            this.lbxModelPalettes.Location = new System.Drawing.Point(6, 6);
            this.lbxModelPalettes.Name = "lbxModelPalettes";
            this.lbxModelPalettes.Size = new System.Drawing.Size(288, 121);
            this.lbxModelPalettes.TabIndex = 0;
            this.lbxModelPalettes.SelectedIndexChanged += new System.EventHandler(this.lbxModelPalettes_SelectedIndexChanged);
            // 
            // btnModelPalettesRenamePalette
            // 
            this.btnModelPalettesRenamePalette.Location = new System.Drawing.Point(6, 131);
            this.btnModelPalettesRenamePalette.Name = "btnModelPalettesRenamePalette";
            this.btnModelPalettesRenamePalette.Size = new System.Drawing.Size(55, 23);
            this.btnModelPalettesRenamePalette.TabIndex = 5;
            this.btnModelPalettesRenamePalette.Text = "Rename";
            this.btnModelPalettesRenamePalette.UseVisualStyleBackColor = true;
            this.btnModelPalettesRenamePalette.Click += new System.EventHandler(this.btnModelPalettesRenamePalette_Click);
            // 
            // txtModelPalettesName
            // 
            this.txtModelPalettesName.Location = new System.Drawing.Point(67, 133);
            this.txtModelPalettesName.Name = "txtModelPalettesName";
            this.txtModelPalettesName.Size = new System.Drawing.Size(227, 20);
            this.txtModelPalettesName.TabIndex = 6;
            // 
            // txtModelPreviewScale
            // 
            this.txtModelPreviewScale.Location = new System.Drawing.Point(91, 1);
            this.txtModelPreviewScale.Name = "txtModelPreviewScale";
            this.txtModelPreviewScale.Size = new System.Drawing.Size(106, 20);
            this.txtModelPreviewScale.TabIndex = 15;
            this.txtModelPreviewScale.TextChanged += new System.EventHandler(this.txtModelPreviewScale_TextChanged);
            // 
            // tsModelPreview
            // 
            this.tsModelPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblModelPreviewScale});
            this.tsModelPreview.Location = new System.Drawing.Point(0, 0);
            this.tsModelPreview.Name = "tsModelPreview";
            this.tsModelPreview.Size = new System.Drawing.Size(562, 25);
            this.tsModelPreview.TabIndex = 1;
            this.tsModelPreview.Text = "toolStrip1";
            // 
            // lblModelPreviewScale
            // 
            this.lblModelPreviewScale.Name = "lblModelPreviewScale";
            this.lblModelPreviewScale.Size = new System.Drawing.Size(81, 22);
            this.lblModelPreviewScale.Text = "Preview Scale:";
            // 
            // glModelView
            // 
            this.glModelView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glModelView.BackColor = System.Drawing.Color.Black;
            this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glModelView.Location = new System.Drawing.Point(0, 0);
            this.glModelView.Margin = new System.Windows.Forms.Padding(0);
            this.glModelView.Name = "glModelView";
            this.glModelView.Size = new System.Drawing.Size(562, 520);
            this.glModelView.TabIndex = 0;
            this.glModelView.VSync = false;
            // 
            // tpgCollisionMap
            // 
            this.tpgCollisionMap.Controls.Add(this.splCollisionMap);
            this.tpgCollisionMap.Location = new System.Drawing.Point(4, 22);
            this.tpgCollisionMap.Name = "tpgCollisionMap";
            this.tpgCollisionMap.Padding = new System.Windows.Forms.Padding(3);
            this.tpgCollisionMap.Size = new System.Drawing.Size(880, 526);
            this.tpgCollisionMap.TabIndex = 1;
            this.tpgCollisionMap.Text = "Collision Map (KCL)";
            this.tpgCollisionMap.UseVisualStyleBackColor = true;
            // 
            // splCollisionMap
            // 
            this.splCollisionMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splCollisionMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splCollisionMap.IsSplitterFixed = true;
            this.splCollisionMap.Location = new System.Drawing.Point(3, 3);
            this.splCollisionMap.Name = "splCollisionMap";
            // 
            // splCollisionMap.Panel1
            // 
            this.splCollisionMap.Panel1.Controls.Add(this.tcCollisionMapSettings);
            // 
            // splCollisionMap.Panel2
            // 
            this.splCollisionMap.Panel2.Controls.Add(this.cmbCollisionMapPreviewFillMode);
            this.splCollisionMap.Panel2.Controls.Add(this.tsCollisionMapPreview);
            this.splCollisionMap.Panel2.Controls.Add(this.glCollisionMapView);
            this.splCollisionMap.Size = new System.Drawing.Size(874, 520);
            this.splCollisionMap.SplitterDistance = 308;
            this.splCollisionMap.TabIndex = 0;
            // 
            // tcCollisionMapSettings
            // 
            this.tcCollisionMapSettings.Controls.Add(this.tpgCollisionMapGeneral);
            this.tcCollisionMapSettings.Controls.Add(this.tpgCollisionMapPlanes);
            this.tcCollisionMapSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcCollisionMapSettings.Location = new System.Drawing.Point(0, 0);
            this.tcCollisionMapSettings.Name = "tcCollisionMapSettings";
            this.tcCollisionMapSettings.SelectedIndex = 0;
            this.tcCollisionMapSettings.Size = new System.Drawing.Size(308, 520);
            this.tcCollisionMapSettings.TabIndex = 0;
            // 
            // tpgCollisionMapGeneral
            // 
            this.tpgCollisionMapGeneral.Controls.Add(this.grpCollisionMapGeneralScale);
            this.tpgCollisionMapGeneral.Controls.Add(this.grpCollisionMapGeneralTarget);
            this.tpgCollisionMapGeneral.Controls.Add(this.groupBox1);
            this.tpgCollisionMapGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpgCollisionMapGeneral.Name = "tpgCollisionMapGeneral";
            this.tpgCollisionMapGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpgCollisionMapGeneral.Size = new System.Drawing.Size(300, 494);
            this.tpgCollisionMapGeneral.TabIndex = 0;
            this.tpgCollisionMapGeneral.Text = "General";
            this.tpgCollisionMapGeneral.UseVisualStyleBackColor = true;
            // 
            // grpCollisionMapGeneralScale
            // 
            this.grpCollisionMapGeneralScale.Controls.Add(this.txtCollisionMapGeneralTargetScale);
            this.grpCollisionMapGeneralScale.Controls.Add(this.txtCollisionMapGeneralScale);
            this.grpCollisionMapGeneralScale.Controls.Add(this.lblCollisionMapGeneralScale);
            this.grpCollisionMapGeneralScale.Controls.Add(this.CollisionMapGeneralTargetScale);
            this.grpCollisionMapGeneralScale.Location = new System.Drawing.Point(3, 310);
            this.grpCollisionMapGeneralScale.Name = "grpCollisionMapGeneralScale";
            this.grpCollisionMapGeneralScale.Size = new System.Drawing.Size(294, 63);
            this.grpCollisionMapGeneralScale.TabIndex = 24;
            this.grpCollisionMapGeneralScale.TabStop = false;
            this.grpCollisionMapGeneralScale.Text = "Scale";
            // 
            // txtCollisionMapGeneralTargetScale
            // 
            this.txtCollisionMapGeneralTargetScale.Location = new System.Drawing.Point(182, 35);
            this.txtCollisionMapGeneralTargetScale.Name = "txtCollisionMapGeneralTargetScale";
            this.txtCollisionMapGeneralTargetScale.Size = new System.Drawing.Size(106, 20);
            this.txtCollisionMapGeneralTargetScale.TabIndex = 15;
            this.txtCollisionMapGeneralTargetScale.TextChanged += new System.EventHandler(this.txtCollisionMapGeneralTargetScale_TextChanged);
            // 
            // txtCollisionMapGeneralScale
            // 
            this.txtCollisionMapGeneralScale.Location = new System.Drawing.Point(182, 13);
            this.txtCollisionMapGeneralScale.Name = "txtCollisionMapGeneralScale";
            this.txtCollisionMapGeneralScale.Size = new System.Drawing.Size(106, 20);
            this.txtCollisionMapGeneralScale.TabIndex = 1;
            this.txtCollisionMapGeneralScale.TextChanged += new System.EventHandler(this.txtCollisionMapGeneralScale_TextChanged);
            // 
            // lblCollisionMapGeneralScale
            // 
            this.lblCollisionMapGeneralScale.AutoSize = true;
            this.lblCollisionMapGeneralScale.Location = new System.Drawing.Point(6, 16);
            this.lblCollisionMapGeneralScale.Name = "lblCollisionMapGeneralScale";
            this.lblCollisionMapGeneralScale.Size = new System.Drawing.Size(37, 13);
            this.lblCollisionMapGeneralScale.TabIndex = 0;
            this.lblCollisionMapGeneralScale.Text = "Scale:";
            // 
            // CollisionMapGeneralTargetScale
            // 
            this.CollisionMapGeneralTargetScale.AutoSize = true;
            this.CollisionMapGeneralTargetScale.Location = new System.Drawing.Point(7, 38);
            this.CollisionMapGeneralTargetScale.Name = "CollisionMapGeneralTargetScale";
            this.CollisionMapGeneralTargetScale.Size = new System.Drawing.Size(68, 13);
            this.CollisionMapGeneralTargetScale.TabIndex = 14;
            this.CollisionMapGeneralTargetScale.Text = "Game Scale:";
            // 
            // grpCollisionMapGeneralTarget
            // 
            this.grpCollisionMapGeneralTarget.Controls.Add(this.lblCollisionMapGeneralTarget);
            this.grpCollisionMapGeneralTarget.Controls.Add(this.txtCollisionMapGeneralTargetName);
            this.grpCollisionMapGeneralTarget.Controls.Add(this.btnCollisionMapGeneralSelectTarget);
            this.grpCollisionMapGeneralTarget.Location = new System.Drawing.Point(3, 6);
            this.grpCollisionMapGeneralTarget.Name = "grpCollisionMapGeneralTarget";
            this.grpCollisionMapGeneralTarget.Size = new System.Drawing.Size(287, 40);
            this.grpCollisionMapGeneralTarget.TabIndex = 23;
            this.grpCollisionMapGeneralTarget.TabStop = false;
            this.grpCollisionMapGeneralTarget.Text = "Target";
            // 
            // lblCollisionMapGeneralTarget
            // 
            this.lblCollisionMapGeneralTarget.AutoSize = true;
            this.lblCollisionMapGeneralTarget.Location = new System.Drawing.Point(6, 16);
            this.lblCollisionMapGeneralTarget.Name = "lblCollisionMapGeneralTarget";
            this.lblCollisionMapGeneralTarget.Size = new System.Drawing.Size(38, 13);
            this.lblCollisionMapGeneralTarget.TabIndex = 16;
            this.lblCollisionMapGeneralTarget.Text = "Name:";
            // 
            // txtCollisionMapGeneralTargetName
            // 
            this.txtCollisionMapGeneralTargetName.Location = new System.Drawing.Point(50, 13);
            this.txtCollisionMapGeneralTargetName.Name = "txtCollisionMapGeneralTargetName";
            this.txtCollisionMapGeneralTargetName.ReadOnly = true;
            this.txtCollisionMapGeneralTargetName.Size = new System.Drawing.Size(200, 20);
            this.txtCollisionMapGeneralTargetName.TabIndex = 17;
            // 
            // btnCollisionMapGeneralSelectTarget
            // 
            this.btnCollisionMapGeneralSelectTarget.Location = new System.Drawing.Point(256, 11);
            this.btnCollisionMapGeneralSelectTarget.Name = "btnCollisionMapGeneralSelectTarget";
            this.btnCollisionMapGeneralSelectTarget.Size = new System.Drawing.Size(25, 23);
            this.btnCollisionMapGeneralSelectTarget.TabIndex = 18;
            this.btnCollisionMapGeneralSelectTarget.Text = "...";
            this.btnCollisionMapGeneralSelectTarget.UseVisualStyleBackColor = true;
            this.btnCollisionMapGeneralSelectTarget.Click += new System.EventHandler(this.btnCollisionMapGeneralSelectTarget_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridCollisionMapGeneralMaterialCollisionTypes);
            this.groupBox1.Controls.Add(this.btnCollisionMapGeneralResetMaterialCollisionTypes);
            this.groupBox1.Controls.Add(this.btnCollisionMapGeneralAssignMaterialCollisionTypes);
            this.groupBox1.Controls.Add(this.lblCollisionMapGeneralMaterialCollisionTypes);
            this.groupBox1.Controls.Add(this.txtCollisionMapGeneralFaceSizeThreshold);
            this.groupBox1.Controls.Add(this.lblCollisionMapGeneralFaceSizeThreshold);
            this.groupBox1.Location = new System.Drawing.Point(3, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 252);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Importation Settings";
            // 
            // gridCollisionMapGeneralMaterialCollisionTypes
            // 
            this.gridCollisionMapGeneralMaterialCollisionTypes.AllowUserToAddRows = false;
            this.gridCollisionMapGeneralMaterialCollisionTypes.AllowUserToDeleteRows = false;
            this.gridCollisionMapGeneralMaterialCollisionTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCollisionMapGeneralMaterialCollisionTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCollisionMapGeneralMaterialCollisionTypes.Location = new System.Drawing.Point(3, 59);
            this.gridCollisionMapGeneralMaterialCollisionTypes.Name = "gridCollisionMapGeneralMaterialCollisionTypes";
            this.gridCollisionMapGeneralMaterialCollisionTypes.RowHeadersVisible = false;
            this.gridCollisionMapGeneralMaterialCollisionTypes.Size = new System.Drawing.Size(285, 158);
            this.gridCollisionMapGeneralMaterialCollisionTypes.TabIndex = 10;
            // 
            // btnCollisionMapGeneralResetMaterialCollisionTypes
            // 
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.Location = new System.Drawing.Point(87, 223);
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.Name = "btnCollisionMapGeneralResetMaterialCollisionTypes";
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.Size = new System.Drawing.Size(75, 23);
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.TabIndex = 5;
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.Text = "Reset";
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.UseVisualStyleBackColor = true;
            this.btnCollisionMapGeneralResetMaterialCollisionTypes.Click += new System.EventHandler(this.btnCollisionMapGeneralResetMaterialCollisionTypes_Click);
            // 
            // btnCollisionMapGeneralAssignMaterialCollisionTypes
            // 
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.Location = new System.Drawing.Point(6, 223);
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.Name = "btnCollisionMapGeneralAssignMaterialCollisionTypes";
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.Size = new System.Drawing.Size(75, 23);
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.TabIndex = 4;
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.Text = "Assign";
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.UseVisualStyleBackColor = true;
            this.btnCollisionMapGeneralAssignMaterialCollisionTypes.Click += new System.EventHandler(this.btnCollisionMapGeneralAssignMaterialCollisionTypes_Click);
            // 
            // lblCollisionMapGeneralMaterialCollisionTypes
            // 
            this.lblCollisionMapGeneralMaterialCollisionTypes.AutoSize = true;
            this.lblCollisionMapGeneralMaterialCollisionTypes.Location = new System.Drawing.Point(6, 43);
            this.lblCollisionMapGeneralMaterialCollisionTypes.Name = "lblCollisionMapGeneralMaterialCollisionTypes";
            this.lblCollisionMapGeneralMaterialCollisionTypes.Size = new System.Drawing.Size(120, 13);
            this.lblCollisionMapGeneralMaterialCollisionTypes.TabIndex = 2;
            this.lblCollisionMapGeneralMaterialCollisionTypes.Text = "Material Collision Types:";
            // 
            // txtCollisionMapGeneralFaceSizeThreshold
            // 
            this.txtCollisionMapGeneralFaceSizeThreshold.Location = new System.Drawing.Point(182, 17);
            this.txtCollisionMapGeneralFaceSizeThreshold.Name = "txtCollisionMapGeneralFaceSizeThreshold";
            this.txtCollisionMapGeneralFaceSizeThreshold.Size = new System.Drawing.Size(106, 20);
            this.txtCollisionMapGeneralFaceSizeThreshold.TabIndex = 1;
            this.txtCollisionMapGeneralFaceSizeThreshold.TextChanged += new System.EventHandler(this.txtCollisionMapGeneralFaceSizeThreshold_TextChanged);
            // 
            // lblCollisionMapGeneralFaceSizeThreshold
            // 
            this.lblCollisionMapGeneralFaceSizeThreshold.AutoSize = true;
            this.lblCollisionMapGeneralFaceSizeThreshold.Location = new System.Drawing.Point(6, 20);
            this.lblCollisionMapGeneralFaceSizeThreshold.Name = "lblCollisionMapGeneralFaceSizeThreshold";
            this.lblCollisionMapGeneralFaceSizeThreshold.Size = new System.Drawing.Size(101, 13);
            this.lblCollisionMapGeneralFaceSizeThreshold.TabIndex = 0;
            this.lblCollisionMapGeneralFaceSizeThreshold.Text = "Minimum Face Size:";
            // 
            // tpgCollisionMapPlanes
            // 
            this.tpgCollisionMapPlanes.Controls.Add(this.grpCollisionMapPlanesDetails);
            this.tpgCollisionMapPlanes.Controls.Add(this.lbxCollisionMapPlanes);
            this.tpgCollisionMapPlanes.Location = new System.Drawing.Point(4, 22);
            this.tpgCollisionMapPlanes.Name = "tpgCollisionMapPlanes";
            this.tpgCollisionMapPlanes.Padding = new System.Windows.Forms.Padding(3);
            this.tpgCollisionMapPlanes.Size = new System.Drawing.Size(300, 494);
            this.tpgCollisionMapPlanes.TabIndex = 1;
            this.tpgCollisionMapPlanes.Text = "Planes";
            this.tpgCollisionMapPlanes.UseVisualStyleBackColor = true;
            // 
            // grpCollisionMapPlanesDetails
            // 
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesVertex1);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesDirection3);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesVertex2);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesDirection3);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesVertex3);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesDirection2);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblColType);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesDirection1);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesCollisionType);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesNormal);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesVertex1);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesDirection2);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesVertex2);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblCollisionMapPlanesDirection1);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.txtCollisionMapPlanesVertex3);
            this.grpCollisionMapPlanesDetails.Controls.Add(this.lblNormal);
            this.grpCollisionMapPlanesDetails.Location = new System.Drawing.Point(6, 288);
            this.grpCollisionMapPlanesDetails.Name = "grpCollisionMapPlanesDetails";
            this.grpCollisionMapPlanesDetails.Size = new System.Drawing.Size(288, 199);
            this.grpCollisionMapPlanesDetails.TabIndex = 2;
            this.grpCollisionMapPlanesDetails.TabStop = false;
            this.grpCollisionMapPlanesDetails.Text = "Details";
            // 
            // lblCollisionMapPlanesVertex1
            // 
            this.lblCollisionMapPlanesVertex1.AutoSize = true;
            this.lblCollisionMapPlanesVertex1.Location = new System.Drawing.Point(6, 16);
            this.lblCollisionMapPlanesVertex1.Name = "lblCollisionMapPlanesVertex1";
            this.lblCollisionMapPlanesVertex1.Size = new System.Drawing.Size(49, 13);
            this.lblCollisionMapPlanesVertex1.TabIndex = 17;
            this.lblCollisionMapPlanesVertex1.Text = "Vertex 1:";
            // 
            // txtCollisionMapPlanesDirection3
            // 
            this.txtCollisionMapPlanesDirection3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesDirection3.Location = new System.Drawing.Point(131, 147);
            this.txtCollisionMapPlanesDirection3.Name = "txtCollisionMapPlanesDirection3";
            this.txtCollisionMapPlanesDirection3.ReadOnly = true;
            this.txtCollisionMapPlanesDirection3.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesDirection3.TabIndex = 32;
            // 
            // lblCollisionMapPlanesVertex2
            // 
            this.lblCollisionMapPlanesVertex2.AutoSize = true;
            this.lblCollisionMapPlanesVertex2.Location = new System.Drawing.Point(6, 38);
            this.lblCollisionMapPlanesVertex2.Name = "lblCollisionMapPlanesVertex2";
            this.lblCollisionMapPlanesVertex2.Size = new System.Drawing.Size(49, 13);
            this.lblCollisionMapPlanesVertex2.TabIndex = 18;
            this.lblCollisionMapPlanesVertex2.Text = "Vertex 2:";
            // 
            // lblCollisionMapPlanesDirection3
            // 
            this.lblCollisionMapPlanesDirection3.AutoSize = true;
            this.lblCollisionMapPlanesDirection3.Location = new System.Drawing.Point(6, 150);
            this.lblCollisionMapPlanesDirection3.Name = "lblCollisionMapPlanesDirection3";
            this.lblCollisionMapPlanesDirection3.Size = new System.Drawing.Size(61, 13);
            this.lblCollisionMapPlanesDirection3.TabIndex = 31;
            this.lblCollisionMapPlanesDirection3.Text = "Direction 3:";
            // 
            // lblCollisionMapPlanesVertex3
            // 
            this.lblCollisionMapPlanesVertex3.AutoSize = true;
            this.lblCollisionMapPlanesVertex3.Location = new System.Drawing.Point(6, 60);
            this.lblCollisionMapPlanesVertex3.Name = "lblCollisionMapPlanesVertex3";
            this.lblCollisionMapPlanesVertex3.Size = new System.Drawing.Size(49, 13);
            this.lblCollisionMapPlanesVertex3.TabIndex = 19;
            this.lblCollisionMapPlanesVertex3.Text = "Vertex 3:";
            // 
            // txtCollisionMapPlanesDirection2
            // 
            this.txtCollisionMapPlanesDirection2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesDirection2.Location = new System.Drawing.Point(131, 124);
            this.txtCollisionMapPlanesDirection2.Name = "txtCollisionMapPlanesDirection2";
            this.txtCollisionMapPlanesDirection2.ReadOnly = true;
            this.txtCollisionMapPlanesDirection2.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesDirection2.TabIndex = 30;
            // 
            // lblColType
            // 
            this.lblColType.AutoSize = true;
            this.lblColType.Location = new System.Drawing.Point(6, 173);
            this.lblColType.Name = "lblColType";
            this.lblColType.Size = new System.Drawing.Size(72, 13);
            this.lblColType.TabIndex = 20;
            this.lblColType.Text = "Collision Type";
            // 
            // txtCollisionMapPlanesDirection1
            // 
            this.txtCollisionMapPlanesDirection1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesDirection1.Location = new System.Drawing.Point(131, 102);
            this.txtCollisionMapPlanesDirection1.Name = "txtCollisionMapPlanesDirection1";
            this.txtCollisionMapPlanesDirection1.ReadOnly = true;
            this.txtCollisionMapPlanesDirection1.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesDirection1.TabIndex = 29;
            // 
            // txtCollisionMapPlanesCollisionType
            // 
            this.txtCollisionMapPlanesCollisionType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesCollisionType.Location = new System.Drawing.Point(131, 170);
            this.txtCollisionMapPlanesCollisionType.Name = "txtCollisionMapPlanesCollisionType";
            this.txtCollisionMapPlanesCollisionType.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesCollisionType.TabIndex = 21;
            this.txtCollisionMapPlanesCollisionType.TextChanged += new System.EventHandler(this.txtCollisionMapPlanesCollisionType_TextChanged);
            // 
            // txtCollisionMapPlanesNormal
            // 
            this.txtCollisionMapPlanesNormal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesNormal.Location = new System.Drawing.Point(131, 80);
            this.txtCollisionMapPlanesNormal.Name = "txtCollisionMapPlanesNormal";
            this.txtCollisionMapPlanesNormal.ReadOnly = true;
            this.txtCollisionMapPlanesNormal.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesNormal.TabIndex = 28;
            // 
            // txtCollisionMapPlanesVertex1
            // 
            this.txtCollisionMapPlanesVertex1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesVertex1.Location = new System.Drawing.Point(131, 13);
            this.txtCollisionMapPlanesVertex1.Name = "txtCollisionMapPlanesVertex1";
            this.txtCollisionMapPlanesVertex1.ReadOnly = true;
            this.txtCollisionMapPlanesVertex1.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesVertex1.TabIndex = 22;
            // 
            // lblCollisionMapPlanesDirection2
            // 
            this.lblCollisionMapPlanesDirection2.AutoSize = true;
            this.lblCollisionMapPlanesDirection2.Location = new System.Drawing.Point(6, 127);
            this.lblCollisionMapPlanesDirection2.Name = "lblCollisionMapPlanesDirection2";
            this.lblCollisionMapPlanesDirection2.Size = new System.Drawing.Size(61, 13);
            this.lblCollisionMapPlanesDirection2.TabIndex = 27;
            this.lblCollisionMapPlanesDirection2.Text = "Direction 2:";
            // 
            // txtCollisionMapPlanesVertex2
            // 
            this.txtCollisionMapPlanesVertex2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesVertex2.Location = new System.Drawing.Point(131, 35);
            this.txtCollisionMapPlanesVertex2.Name = "txtCollisionMapPlanesVertex2";
            this.txtCollisionMapPlanesVertex2.ReadOnly = true;
            this.txtCollisionMapPlanesVertex2.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesVertex2.TabIndex = 23;
            // 
            // lblCollisionMapPlanesDirection1
            // 
            this.lblCollisionMapPlanesDirection1.AutoSize = true;
            this.lblCollisionMapPlanesDirection1.Location = new System.Drawing.Point(6, 105);
            this.lblCollisionMapPlanesDirection1.Name = "lblCollisionMapPlanesDirection1";
            this.lblCollisionMapPlanesDirection1.Size = new System.Drawing.Size(61, 13);
            this.lblCollisionMapPlanesDirection1.TabIndex = 26;
            this.lblCollisionMapPlanesDirection1.Text = "Direction 1:";
            // 
            // txtCollisionMapPlanesVertex3
            // 
            this.txtCollisionMapPlanesVertex3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollisionMapPlanesVertex3.Location = new System.Drawing.Point(131, 57);
            this.txtCollisionMapPlanesVertex3.Name = "txtCollisionMapPlanesVertex3";
            this.txtCollisionMapPlanesVertex3.ReadOnly = true;
            this.txtCollisionMapPlanesVertex3.Size = new System.Drawing.Size(151, 20);
            this.txtCollisionMapPlanesVertex3.TabIndex = 24;
            // 
            // lblNormal
            // 
            this.lblNormal.AutoSize = true;
            this.lblNormal.Location = new System.Drawing.Point(6, 83);
            this.lblNormal.Name = "lblNormal";
            this.lblNormal.Size = new System.Drawing.Size(43, 13);
            this.lblNormal.TabIndex = 25;
            this.lblNormal.Text = "Normal:";
            // 
            // lbxCollisionMapPlanes
            // 
            this.lbxCollisionMapPlanes.FormattingEnabled = true;
            this.lbxCollisionMapPlanes.Location = new System.Drawing.Point(3, 6);
            this.lbxCollisionMapPlanes.Name = "lbxCollisionMapPlanes";
            this.lbxCollisionMapPlanes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxCollisionMapPlanes.Size = new System.Drawing.Size(291, 277);
            this.lbxCollisionMapPlanes.TabIndex = 1;
            this.lbxCollisionMapPlanes.SelectedIndexChanged += new System.EventHandler(this.lbxCollisionMapPlanes_SelectedIndexChanged);
            // 
            // cmbCollisionMapPreviewFillMode
            // 
            this.cmbCollisionMapPreviewFillMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCollisionMapPreviewFillMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCollisionMapPreviewFillMode.FormattingEnabled = true;
            this.cmbCollisionMapPreviewFillMode.Location = new System.Drawing.Point(436, 1);
            this.cmbCollisionMapPreviewFillMode.Name = "cmbCollisionMapPreviewFillMode";
            this.cmbCollisionMapPreviewFillMode.Size = new System.Drawing.Size(121, 21);
            this.cmbCollisionMapPreviewFillMode.TabIndex = 2;
            this.cmbCollisionMapPreviewFillMode.SelectedIndexChanged += new System.EventHandler(this.cmbCollisionMapPreviewFillMode_SelectedIndexChanged);
            // 
            // tsCollisionMapPreview
            // 
            this.tsCollisionMapPreview.Location = new System.Drawing.Point(0, 0);
            this.tsCollisionMapPreview.Name = "tsCollisionMapPreview";
            this.tsCollisionMapPreview.Size = new System.Drawing.Size(562, 25);
            this.tsCollisionMapPreview.TabIndex = 1;
            this.tsCollisionMapPreview.Text = "toolStrip1";
            // 
            // glCollisionMapView
            // 
            this.glCollisionMapView.BackColor = System.Drawing.Color.Black;
            this.glCollisionMapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glCollisionMapView.Location = new System.Drawing.Point(0, 0);
            this.glCollisionMapView.Name = "glCollisionMapView";
            this.glCollisionMapView.Size = new System.Drawing.Size(562, 520);
            this.glCollisionMapView.TabIndex = 0;
            this.glCollisionMapView.VSync = false;
            this.glCollisionMapView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glCollisionMapView_MouseUp);
            // 
            // mstrMain
            // 
            this.mstrMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitLoad,
            this.mnitImport,
            this.mnitExport});
            this.mstrMain.Location = new System.Drawing.Point(0, 0);
            this.mstrMain.Name = "mstrMain";
            this.mstrMain.Size = new System.Drawing.Size(888, 24);
            this.mstrMain.TabIndex = 3;
            this.mstrMain.Text = "menuStrip1";
            // 
            // mnitLoad
            // 
            this.mnitLoad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitLoadExternalModel,
            this.mnitLoadInternalModelCollisionMap,
            this.mnitLoadRevertChanges});
            this.mnitLoad.Name = "mnitLoad";
            this.mnitLoad.Size = new System.Drawing.Size(45, 20);
            this.mnitLoad.Text = "Load";
            // 
            // mnitLoadExternalModel
            // 
            this.mnitLoadExternalModel.Name = "mnitLoadExternalModel";
            this.mnitLoadExternalModel.Size = new System.Drawing.Size(229, 22);
            this.mnitLoadExternalModel.Text = "External Model";
            this.mnitLoadExternalModel.Click += new System.EventHandler(this.mnitLoadExternalModel_Click);
            // 
            // mnitLoadInternalModelCollisionMap
            // 
            this.mnitLoadInternalModelCollisionMap.Name = "mnitLoadInternalModelCollisionMap";
            this.mnitLoadInternalModelCollisionMap.Size = new System.Drawing.Size(229, 22);
            this.mnitLoadInternalModelCollisionMap.Text = "Internal Model/Collision Map";
            this.mnitLoadInternalModelCollisionMap.Click += new System.EventHandler(this.mnitLoadInternalModelCollisionMap_Click);
            // 
            // mnitLoadRevertChanges
            // 
            this.mnitLoadRevertChanges.Name = "mnitLoadRevertChanges";
            this.mnitLoadRevertChanges.Size = new System.Drawing.Size(229, 22);
            this.mnitLoadRevertChanges.Text = "Revert Changes";
            this.mnitLoadRevertChanges.Click += new System.EventHandler(this.mnitLoadRevertChanges_Click);
            // 
            // mnitImport
            // 
            this.mnitImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitImportModelAndCollisionMap,
            this.mnitImportModelOnly,
            this.mnitImportCollisionMapOnly});
            this.mnitImport.Name = "mnitImport";
            this.mnitImport.Size = new System.Drawing.Size(55, 20);
            this.mnitImport.Text = "Import";
            // 
            // mnitImportModelAndCollisionMap
            // 
            this.mnitImportModelAndCollisionMap.Name = "mnitImportModelAndCollisionMap";
            this.mnitImportModelAndCollisionMap.Size = new System.Drawing.Size(207, 22);
            this.mnitImportModelAndCollisionMap.Text = "Model and Collision Map";
            this.mnitImportModelAndCollisionMap.Click += new System.EventHandler(this.mnitImportModelAndCollisionMap_Click);
            // 
            // mnitImportModelOnly
            // 
            this.mnitImportModelOnly.Name = "mnitImportModelOnly";
            this.mnitImportModelOnly.Size = new System.Drawing.Size(207, 22);
            this.mnitImportModelOnly.Text = "Model";
            this.mnitImportModelOnly.Click += new System.EventHandler(this.mnitImportModelOnly_Click);
            // 
            // mnitImportCollisionMapOnly
            // 
            this.mnitImportCollisionMapOnly.Name = "mnitImportCollisionMapOnly";
            this.mnitImportCollisionMapOnly.Size = new System.Drawing.Size(207, 22);
            this.mnitImportCollisionMapOnly.Text = "Collision Map";
            this.mnitImportCollisionMapOnly.Click += new System.EventHandler(this.mnitImportCollisionMapOnly_Click);
            // 
            // mnitExport
            // 
            this.mnitExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitExportModel,
            this.mnitExportCollisionMap,
            this.mnitExportTextures});
            this.mnitExport.Name = "mnitExport";
            this.mnitExport.Size = new System.Drawing.Size(52, 20);
            this.mnitExport.Text = "Export";
            // 
            // mnitExportModel
            // 
            this.mnitExportModel.Name = "mnitExportModel";
            this.mnitExportModel.Size = new System.Drawing.Size(147, 22);
            this.mnitExportModel.Text = "Model";
            this.mnitExportModel.Click += new System.EventHandler(this.mnitExportModel_Click);
            // 
            // mnitExportCollisionMap
            // 
            this.mnitExportCollisionMap.Name = "mnitExportCollisionMap";
            this.mnitExportCollisionMap.Size = new System.Drawing.Size(147, 22);
            this.mnitExportCollisionMap.Text = "Collision Map";
            this.mnitExportCollisionMap.Click += new System.EventHandler(this.mnitExportCollisionMap_Click);
            // 
            // mnitExportTextures
            // 
            this.mnitExportTextures.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitExportTexturesPNG,
            this.mnitExportTexturesBMP});
            this.mnitExportTextures.Name = "mnitExportTextures";
            this.mnitExportTextures.Size = new System.Drawing.Size(147, 22);
            this.mnitExportTextures.Text = "Textures";
            // 
            // mnitExportTexturesPNG
            // 
            this.mnitExportTexturesPNG.Name = "mnitExportTexturesPNG";
            this.mnitExportTexturesPNG.Size = new System.Drawing.Size(149, 22);
            this.mnitExportTexturesPNG.Text = "Export as PNG";
            this.mnitExportTexturesPNG.Click += new System.EventHandler(this.mnitExportTexturesPNG_Click);
            // 
            // mnitExportTexturesBMP
            // 
            this.mnitExportTexturesBMP.Name = "mnitExportTexturesBMP";
            this.mnitExportTexturesBMP.Size = new System.Drawing.Size(149, 22);
            this.mnitExportTexturesBMP.Text = "Export as BMP";
            this.mnitExportTexturesBMP.Click += new System.EventHandler(this.mnitExportTexturesBMP_Click);
            // 
            // grpModelBonesSettings
            // 
            this.grpModelBonesSettings.Controls.Add(this.chkModelBonesSettingsBillboard);
            this.grpModelBonesSettings.Location = new System.Drawing.Point(6, 202);
            this.grpModelBonesSettings.Name = "grpModelBonesSettings";
            this.grpModelBonesSettings.Size = new System.Drawing.Size(271, 44);
            this.grpModelBonesSettings.TabIndex = 21;
            this.grpModelBonesSettings.TabStop = false;
            this.grpModelBonesSettings.Text = "Settings";
            // 
            // chkModelBonesSettingsBillboard
            // 
            this.chkModelBonesSettingsBillboard.AutoSize = true;
            this.chkModelBonesSettingsBillboard.Location = new System.Drawing.Point(9, 19);
            this.chkModelBonesSettingsBillboard.Name = "chkModelBonesSettingsBillboard";
            this.chkModelBonesSettingsBillboard.Size = new System.Drawing.Size(210, 17);
            this.chkModelBonesSettingsBillboard.TabIndex = 0;
            this.chkModelBonesSettingsBillboard.Text = "Always Render Facing Camera (\"Billboard\")";
            this.chkModelBonesSettingsBillboard.UseVisualStyleBackColor = true;
            this.chkModelBonesSettingsBillboard.CheckedChanged += new System.EventHandler(chkModelBonesSettingsBillboard_CheckedChanged);
            // 
            // ModelAndCollisionMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 598);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.mstrMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mstrMain;
            this.Name = "ModelAndCollisionMapEditor";
            this.Text = "ModelAndCollisionMapEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelAndCollisionMapEditor_FormClosed);
            this.Load += new System.EventHandler(this.ModelAndCollisionMapEditor_Load);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tpgModel.ResumeLayout(false);
            this.splModel.Panel1.ResumeLayout(false);
            this.splModel.Panel2.ResumeLayout(false);
            this.splModel.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splModel)).EndInit();
            this.splModel.ResumeLayout(false);
            this.tcModelSettings.ResumeLayout(false);
            this.tpgModelGeneral.ResumeLayout(false);
            this.grpModelGeneralScale.ResumeLayout(false);
            this.grpModelGeneralScale.PerformLayout();
            this.grpModelGeneralTarget.ResumeLayout(false);
            this.grpModelGeneralTarget.PerformLayout();
            this.grpModelGeneralImportationSettings.ResumeLayout(false);
            this.grpModelGeneralImportationSettings.PerformLayout();
            this.grpModelGeneralTextureQuality.ResumeLayout(false);
            this.grpModelGeneralTextureQuality.PerformLayout();
            this.tpgModelBones.ResumeLayout(false);
            this.tpgModelBones.PerformLayout();
            this.grpModelBonesGeometries.ResumeLayout(false);
            this.mnuModelBonesPasteToGeometry.ResumeLayout(false);
            this.grpModelBonesPolylists.ResumeLayout(false);
            this.grpModelBonesPolylists.PerformLayout();
            this.mnuModelBonesPasteToBone.ResumeLayout(false);
            this.mnuModelBonesAddBoneOptions.ResumeLayout(false);
            this.tpgModelMaterials.ResumeLayout(false);
            this.tpgModelMaterials.PerformLayout();
            this.grpModelMaterialSettings.ResumeLayout(false);
            this.grpModelMaterialSettings.PerformLayout();
            this.grpModelMaterialTextureSettings.ResumeLayout(false);
            this.grpModelMaterialTextureSettings.PerformLayout();
            this.grpModelMaterialColours.ResumeLayout(false);
            this.grpModelMaterialColours.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.nudModelMaterialAlpha)).EndInit();
            this.grpModelMaterialFlags.ResumeLayout(false);
            this.grpModelMaterialFlags.PerformLayout();
            this.tpgModelTextures.ResumeLayout(false);
            this.tpgModelTextures.PerformLayout();
            this.grpModelTexturesPreview.ResumeLayout(false);
            this.pnlModelTexturesPreviewPanel.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.pbxModelTexturesPreview)).EndInit();
            this.grpModelTexturesSettings.ResumeLayout(false);
            this.grpModelTexturesSettings.PerformLayout();
            this.tpgPalettes.ResumeLayout(false);
            this.tpgPalettes.PerformLayout();
            this.grpModelPalettesColours.ResumeLayout(false);
            this.grpModelPalettesColours.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridModelPalettesPaletteColours)).EndInit();
            this.tsModelPreview.ResumeLayout(false);
            this.tsModelPreview.PerformLayout();
            this.tpgCollisionMap.ResumeLayout(false);
            this.splCollisionMap.Panel1.ResumeLayout(false);
            this.splCollisionMap.Panel2.ResumeLayout(false);
            this.splCollisionMap.Panel2.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.splCollisionMap)).EndInit();
            this.splCollisionMap.ResumeLayout(false);
            this.tcCollisionMapSettings.ResumeLayout(false);
            this.tpgCollisionMapGeneral.ResumeLayout(false);
            this.grpCollisionMapGeneralScale.ResumeLayout(false);
            this.grpCollisionMapGeneralScale.PerformLayout();
            this.grpCollisionMapGeneralTarget.ResumeLayout(false);
            this.grpCollisionMapGeneralTarget.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            //((System.ComponentModel.ISupportInitialize)(this.gridCollisionMapGeneralMaterialCollisionTypes)).EndInit();
            this.tpgCollisionMapPlanes.ResumeLayout(false);
            this.grpCollisionMapPlanesDetails.ResumeLayout(false);
            this.grpCollisionMapPlanesDetails.PerformLayout();
            this.mstrMain.ResumeLayout(false);
            this.mstrMain.PerformLayout();
            this.grpModelBonesSettings.ResumeLayout(false);
            this.grpModelBonesSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpgModel;
        private System.Windows.Forms.TabPage tpgCollisionMap;
        private System.Windows.Forms.SplitContainer splModel;
        private System.Windows.Forms.TabControl tcModelSettings;
        private System.Windows.Forms.TabPage tpgModelGeneral;
        private System.Windows.Forms.TabPage tpgModelBones;
        private System.Windows.Forms.CheckBox chkModelGeneralStripify;
        private System.Windows.Forms.CheckBox chkModelGeneralKeepVertexOrderDuringStripping;
        private System.Windows.Forms.CheckBox chkModelGeneralAlwaysWriteFullVertexCmd23h;
        private System.Windows.Forms.GroupBox grpModelGeneralTextureQuality;
        private System.Windows.Forms.RadioButton rbModelGeneralTextureAlwaysCompress;
        private System.Windows.Forms.RadioButton rbModelGeneralTextureNeverCompress;
        private System.Windows.Forms.RadioButton rbModelGeneralTextureBetterQualityWhereSensible;
        private System.Windows.Forms.CheckBox chkModelGeneralVFlipAllTextures;
        private System.Windows.Forms.TreeView tvModelBonesBones;
        private System.Windows.Forms.Button btnModelBonesRemoveGeometry;
        private System.Windows.Forms.Button btnModelBonesCutGeometry;
        private System.Windows.Forms.ListBox lbxModelBonesGeometries;
        private System.Windows.Forms.Button btnModelBonesRemovePolylist;
        private System.Windows.Forms.Button btnModelBonesCutPolylist;
        private System.Windows.Forms.ListBox lbxModelBonesPolylists;
        private System.Windows.Forms.TabPage tpgModelMaterials;
        private System.Windows.Forms.Button btnModelMaterialRemoveMaterial;
        private System.Windows.Forms.Button btnModelMaterialRenameMaterial;
        private System.Windows.Forms.Button btnModelMaterialAddMaterial;
        private System.Windows.Forms.ListBox lbxModelMaterials;
        private System.Windows.Forms.GroupBox grpModelMaterialSettings;
        private System.Windows.Forms.CheckBox chkModelMaterialLight4;
        private System.Windows.Forms.CheckBox chkModelMaterialLight3;
        private System.Windows.Forms.CheckBox chkModelMaterialLight2;
        private System.Windows.Forms.CheckBox chkModelMaterialLight1;
        private System.Windows.Forms.Label lblModelMaterialLights;
        private System.Windows.Forms.Label lblModelMaterialTexture;
        private System.Windows.Forms.Label lblModelMaterialPolygonDrawingFace;
        private System.Windows.Forms.ComboBox cmbModelMaterialPolygonDrawingFace;
        private System.Windows.Forms.NumericUpDown nudModelMaterialAlpha;
        private System.Windows.Forms.Label lblModelMaterialAlpha;
        private System.Windows.Forms.Label lblModelMaterialPolygonMode;
        private System.Windows.Forms.ComboBox cmbModelMaterialPolygonMode;
        private System.Windows.Forms.GroupBox grpModelMaterialFlags;
        private System.Windows.Forms.CheckBox chkModelMaterialWireMode;
        private System.Windows.Forms.CheckBox chkModelMaterialFog;
        private System.Windows.Forms.CheckBox chkModelMaterialDepthTestDecal;
        private System.Windows.Forms.CheckBox chkModelMaterialRenderOnePixelPolygons;
        private System.Windows.Forms.CheckBox chkModelMaterialShiniessTable;
        private System.Windows.Forms.CheckBox chkModelMaterialFarClipping;
        private System.Windows.Forms.GroupBox grpModelMaterialColours;
        private System.Windows.Forms.Label lblModelMaterialDiffuse;
        private System.Windows.Forms.Button btnModelMaterialDiffuse;
        private System.Windows.Forms.Button btnModelMaterialEmission;
        private System.Windows.Forms.Label lblModelMaterialEmission;
        private System.Windows.Forms.Button btnModelMaterialSpecular;
        private System.Windows.Forms.Label lblModelMaterialSpecular;
        private System.Windows.Forms.Button btnModelMaterialAmbient;
        private System.Windows.Forms.Label lblModelMaterialAmbient;
        private System.Windows.Forms.GroupBox grpModelMaterialTextureSettings;
        private System.Windows.Forms.CheckBox chkModelMaterialTextureEnabled;
        private System.Windows.Forms.ComboBox cmbModelMaterialTextureTilingX;
        private System.Windows.Forms.Label lblModelMaterialTextureTiling;
        private System.Windows.Forms.ComboBox cmbModelMaterialTextureTilingY;
        private System.Windows.Forms.TextBox txtModelMaterialTextureScaleX;
        private System.Windows.Forms.Label lblModelMaterialTextureScale;
        private System.Windows.Forms.TextBox txtModelMaterialTextureScaleY;
        private System.Windows.Forms.TextBox txtModelMaterialTextureTranslationY;
        private System.Windows.Forms.TextBox txtModelMaterialTextureTranslationX;
        private System.Windows.Forms.Label lblModelMaterialTextureTranslation;
        private System.Windows.Forms.TextBox txtModelMaterialTextureRotation;
        private System.Windows.Forms.Label lblModelMaterialTextureRotation;
        private System.Windows.Forms.ComboBox cmbModelMaterialTexGenMode;
        private System.Windows.Forms.Label lblModelMaterialTexGenMode;
        private System.Windows.Forms.ComboBox cmbModelMaterialTextureID;
        private System.Windows.Forms.TabPage tpgModelTextures;
        private System.Windows.Forms.GroupBox grpModelTexturesSettings;
        private System.Windows.Forms.ListBox lbxModelTextures;
        private System.Windows.Forms.ComboBox cmbModelTexturesFormat;
        private System.Windows.Forms.Label lblModelTexturesFormat;
        private System.Windows.Forms.Button btnModelTexturesRemoveTexture;
        private System.Windows.Forms.TextBox txtModelTexturesName;
        private System.Windows.Forms.Button btnModelTexturesRenameTexture;
        private System.Windows.Forms.Button btnModelTexturesAddTexture;
        private System.Windows.Forms.ComboBox cmbModelTexturesWidth;
        private System.Windows.Forms.Label lblModelTexturesDimensions;
        private System.Windows.Forms.ComboBox cmbModelTexturesHeight;
        private System.Windows.Forms.ComboBox cmbModelTexturesPalette;
        private System.Windows.Forms.Label lblModelTexturesPalette;
        private System.Windows.Forms.PictureBox pbxModelTexturesPreview;
        private System.Windows.Forms.Button btnModelGeneralSelectTarget;
        private System.Windows.Forms.TextBox txtModelGeneralTargetName;
        private System.Windows.Forms.Label lblModelGeneralTargetName;
        private System.Windows.Forms.GroupBox grpModelGeneralImportationSettings;
        private FormControls.ModelGLControlWithMarioSizeReference glModelView;
        private System.Windows.Forms.SplitContainer splCollisionMap;
        private System.Windows.Forms.TabControl tcCollisionMapSettings;
        private System.Windows.Forms.TabPage tpgCollisionMapGeneral;
        private System.Windows.Forms.TabPage tpgCollisionMapPlanes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCollisionMapGeneralFaceSizeThreshold;
        private System.Windows.Forms.Label lblCollisionMapGeneralFaceSizeThreshold;
        private System.Windows.Forms.Button btnCollisionMapGeneralResetMaterialCollisionTypes;
        private System.Windows.Forms.Button btnCollisionMapGeneralAssignMaterialCollisionTypes;
        private System.Windows.Forms.Label lblCollisionMapGeneralMaterialCollisionTypes;
        private System.Windows.Forms.ListBox lbxCollisionMapPlanes;
        private System.Windows.Forms.GroupBox grpCollisionMapPlanesDetails;
        private System.Windows.Forms.Label lblCollisionMapPlanesVertex1;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesDirection3;
        private System.Windows.Forms.Label lblCollisionMapPlanesVertex2;
        private System.Windows.Forms.Label lblCollisionMapPlanesDirection3;
        private System.Windows.Forms.Label lblCollisionMapPlanesVertex3;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesDirection2;
        private System.Windows.Forms.Label lblColType;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesDirection1;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesCollisionType;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesNormal;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesVertex1;
        private System.Windows.Forms.Label lblCollisionMapPlanesDirection2;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesVertex2;
        private System.Windows.Forms.Label lblCollisionMapPlanesDirection1;
        private System.Windows.Forms.TextBox txtCollisionMapPlanesVertex3;
        private System.Windows.Forms.Label lblNormal;
        private System.Windows.Forms.MenuStrip mstrMain;
        private System.Windows.Forms.ToolStripMenuItem mnitLoad;
        private System.Windows.Forms.ToolStripMenuItem mnitLoadExternalModel;
        private System.Windows.Forms.ToolStripMenuItem mnitLoadInternalModelCollisionMap;
        private System.Windows.Forms.ToolStripMenuItem mnitLoadRevertChanges;
        private System.Windows.Forms.ToolStripMenuItem mnitImport;
        private System.Windows.Forms.ToolStripMenuItem mnitImportModelAndCollisionMap;
        private System.Windows.Forms.ToolStripMenuItem mnitImportModelOnly;
        private System.Windows.Forms.ToolStripMenuItem mnitImportCollisionMapOnly;
        private System.Windows.Forms.ToolStripMenuItem mnitExport;
        private System.Windows.Forms.ToolStripMenuItem mnitExportModel;
        private System.Windows.Forms.ToolStripMenuItem mnitExportCollisionMap;
        private System.Windows.Forms.ToolStripStatusLabel lblMainStatus;
        private System.Windows.Forms.TextBox txtModelPreviewScale;
        private System.Windows.Forms.GroupBox grpModelGeneralTarget;
        private System.Windows.Forms.GroupBox grpModelGeneralScale;
        private System.Windows.Forms.TextBox txtModelGeneralScale;
        private System.Windows.Forms.Label lblModelGeneralScale;
        private System.Windows.Forms.GroupBox grpCollisionMapGeneralTarget;
        private System.Windows.Forms.Label lblCollisionMapGeneralTarget;
        private System.Windows.Forms.TextBox txtCollisionMapGeneralTargetName;
        private System.Windows.Forms.Button btnCollisionMapGeneralSelectTarget;
        private System.Windows.Forms.GroupBox grpCollisionMapGeneralScale;
        private System.Windows.Forms.TextBox txtCollisionMapGeneralTargetScale;
        private System.Windows.Forms.TextBox txtCollisionMapGeneralScale;
        private System.Windows.Forms.Label lblCollisionMapGeneralScale;
        private System.Windows.Forms.Label CollisionMapGeneralTargetScale;
        private System.Windows.Forms.Button btnModelMaterialApplySettings;
        private System.Windows.Forms.GroupBox grpModelBonesGeometries;
        private System.Windows.Forms.GroupBox grpModelBonesPolylists;
        private System.Windows.Forms.Label lblModelBonesPolylistMaterial;
        private System.Windows.Forms.ComboBox cmbModelBonesPolylistMaterial;
        private System.Windows.Forms.TabPage tpgPalettes;
        private System.Windows.Forms.Label lblModelPalettesPaletteSelectedColour;
        private System.Windows.Forms.Button btnModelPalettesSelectedColour;
        private System.Windows.Forms.ListBox lbxModelPalettes;
        private PaletteColourGrid gridModelPalettesPaletteColours;
        private System.Windows.Forms.Button btnModelPalettesRenamePalette;
        private System.Windows.Forms.TextBox txtModelPalettesName;
        private System.Windows.Forms.GroupBox grpModelTexturesPreview;
        private System.Windows.Forms.Panel pnlModelTexturesPreviewPanel;
        private System.Windows.Forms.GroupBox grpModelPalettesColours;
        private System.Windows.Forms.ToolStripMenuItem mnitExportTextures;
        private System.Windows.Forms.Button btnModelTexturesReplace;
        private System.Windows.Forms.TextBox txtModelMaterialName;
        private FormControls.MenuButton btnModelBonesAddBone;
        private System.Windows.Forms.ContextMenuStrip mnuModelBonesAddBoneOptions;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesAddBoneSibling;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesAddBoneChild;
        private FormControls.MenuButton btnModelBonesPasteToBone;
        private System.Windows.Forms.ContextMenuStrip mnuModelBonesPasteToBone;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesPasteToBonePreserveWeights;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesPasteToBoneAssignWeightsToTarget;
        private System.Windows.Forms.ContextMenuStrip mnuModelBonesPasteToGeometry;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesPasteToGeometryPreserveWeights;
        private FormControls.MenuButton btnModelBonesPasteToGeometry;
        private System.Windows.Forms.ToolStripMenuItem mnitModelBonesPasteToGeometryAssignWeightsToTarget;
        private System.Windows.Forms.Button btnModelBonesRenameBone;
        private System.Windows.Forms.TextBox txtModelBonesName;
        private System.Windows.Forms.Button btnModelBonesCopyGeometry;
        private System.Windows.Forms.Button btnModelBonesCopyPolylist;
        private FormControls.ModelGLControlWithPicking glCollisionMapView;
        private System.Windows.Forms.DataGridView gridCollisionMapGeneralMaterialCollisionTypes;
        private System.Windows.Forms.ToolStrip tsCollisionMapPreview;
        private System.Windows.Forms.ComboBox cmbCollisionMapPreviewFillMode;
        private System.Windows.Forms.ToolStrip tsModelPreview;
        private System.Windows.Forms.ToolStripLabel lblModelPreviewScale;
        private System.Windows.Forms.ToolStripMenuItem mnitExportTexturesPNG;
        private System.Windows.Forms.ToolStripMenuItem mnitExportTexturesBMP;
        private System.Windows.Forms.GroupBox grpModelBonesSettings;
        private System.Windows.Forms.CheckBox chkModelBonesSettingsBillboard;
    }
}