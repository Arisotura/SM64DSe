using OpenTK.Graphics;
namespace SM64DSe
{
    partial class LevelEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditorForm));
            this.spcMainContainer = new System.Windows.Forms.SplitContainer();
            this.spcLeftPanel = new System.Windows.Forms.SplitContainer();
            this.tvObjectList = new System.Windows.Forms.TreeView();
            this.spcPropertyInterface = new System.Windows.Forms.SplitContainer();
            this.box_general = new System.Windows.Forms.GroupBox();
            this.btnOpenObjectList = new System.Windows.Forms.Button();
            this.val_act = new System.Windows.Forms.ComboBox();
            this.btnToogleCollapseGeneral = new System.Windows.Forms.PictureBox();
            this.val_objectId = new System.Windows.Forms.NumericUpDown();
            this.lbl_objectId = new System.Windows.Forms.Label();
            this.lbl_star = new System.Windows.Forms.Label();
            this.val_area = new System.Windows.Forms.NumericUpDown();
            this.lbl_area = new System.Windows.Forms.Label();
            this.box_fogSettings = new System.Windows.Forms.GroupBox();
            this.box_color = new System.Windows.Forms.Panel();
            this.lbl_r = new System.Windows.Forms.Label();
            this.val_r = new System.Windows.Forms.NumericUpDown();
            this.lbl_g = new System.Windows.Forms.Label();
            this.val_b = new System.Windows.Forms.NumericUpDown();
            this.lbl_b = new System.Windows.Forms.Label();
            this.val_g = new System.Windows.Forms.NumericUpDown();
            this.check_displayFog = new System.Windows.Forms.CheckBox();
            this.val_endDistance = new System.Windows.Forms.NumericUpDown();
            this.lbl_endDistance = new System.Windows.Forms.Label();
            this.val_startDistance = new System.Windows.Forms.NumericUpDown();
            this.lbl_startDistance = new System.Windows.Forms.Label();
            this.btnToogleCollapseColor = new System.Windows.Forms.PictureBox();
            this.box_parameters = new System.Windows.Forms.GroupBox();
            this.btnToogleCollapseParameters = new System.Windows.Forms.PictureBox();
            this.box_rotation = new System.Windows.Forms.GroupBox();
            this.btnToogleCollapseRotation = new System.Windows.Forms.PictureBox();
            this.val_rotY = new System.Windows.Forms.NumericUpDown();
            this.val_rotX = new System.Windows.Forms.NumericUpDown();
            this.lbl_rotY = new System.Windows.Forms.Label();
            this.lbl_rotX = new System.Windows.Forms.Label();
            this.box_position = new System.Windows.Forms.GroupBox();
            this.btnToogleCollapsePosition = new System.Windows.Forms.PictureBox();
            this.val_posZ = new System.Windows.Forms.NumericUpDown();
            this.val_posY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_posY = new System.Windows.Forms.Label();
            this.val_posX = new System.Windows.Forms.NumericUpDown();
            this.lbl_posX = new System.Windows.Forms.Label();
            this.btnPasteCoordinates = new System.Windows.Forms.Button();
            this.btnCopyCoordinates = new System.Windows.Forms.Button();
            this.tsEditActions = new System.Windows.Forms.ToolStrip();
            this.btnImportModel = new System.Windows.Forms.ToolStripButton();
            this.btnExportLevelModel = new System.Windows.Forms.ToolStripButton();
            this.btnAddObject = new System.Windows.Forms.ToolStripButton();
            this.btnAddWarp = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddEntrance = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddAreaWarp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddTpSrc = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddTpDst = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddView = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveSel = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveAll = new System.Windows.Forms.ToolStripButton();
            this.btnReplaceObjModel = new System.Windows.Forms.ToolStripButton();
            this.btnExportObjectModel = new System.Windows.Forms.ToolStripButton();
            this.btnAddPath = new System.Windows.Forms.ToolStripButton();
            this.btnAddMisc = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImportOtherModel = new System.Windows.Forms.ToolStripButton();
            this.btnExportOtherModel = new System.Windows.Forms.ToolStripButton();
            this.btnOffsetAllCoords = new System.Windows.Forms.ToolStripButton();
            this.btnAddPathNode = new System.Windows.Forms.ToolStripButton();
            this.glLevelView = new OpenTK.GLControl();
            this.tsViewActions = new System.Windows.Forms.ToolStrip();
            this.btnLOL = new System.Windows.Forms.ToolStripButton();
            this.btnImportXML = new System.Windows.Forms.ToolStripButton();
            this.btnExportXML = new System.Windows.Forms.ToolStripButton();
            this.btnScreenshot = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOrthView = new System.Windows.Forms.ToolStripButton();
            this.menuGridSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.txtGridSizeX = new System.Windows.Forms.ToolStripTextBox();
            this.txtGridSizeY = new System.Windows.Forms.ToolStripTextBox();
            this.txtGridSizeZ = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.txtGridOffsetX = new System.Windows.Forms.ToolStripTextBox();
            this.txtGridOffsetY = new System.Windows.Forms.ToolStripTextBox();
            this.txtGridOffsetZ = new System.Windows.Forms.ToolStripTextBox();
            this.menuRestrictionPlane = new System.Windows.Forms.ToolStripDropDownButton();
            this.txtRstPlaneX = new System.Windows.Forms.ToolStripTextBox();
            this.txtRstPlaneY = new System.Windows.Forms.ToolStripTextBox();
            this.txtRstPlaneZ = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.txtRstPlaneOffX = new System.Windows.Forms.ToolStripTextBox();
            this.txtRstPlaneOffY = new System.Windows.Forms.ToolStripTextBox();
            this.txtRstPlaneOffZ = new System.Windows.Forms.ToolStripTextBox();
            this.btnMakeOverlay = new System.Windows.Forms.ToolStripButton();
            this.tsToolBar = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLevelSettings = new System.Windows.Forms.ToolStripButton();
            this.btnEditMinimap = new System.Windows.Forms.ToolStripButton();
            this.btnEditTexAnim = new System.Windows.Forms.ToolStripButton();
            this.btnCLPS = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.btnEdit3DModel = new System.Windows.Forms.ToolStripButton();
            this.btnEditObjects = new System.Windows.Forms.ToolStripButton();
            this.btnEditWarps = new System.Windows.Forms.ToolStripButton();
            this.btnEditPaths = new System.Windows.Forms.ToolStripButton();
            this.btnEditViews = new System.Windows.Forms.ToolStripButton();
            this.btnEditMisc = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.btnStar1 = new System.Windows.Forms.ToolStripButton();
            this.btnStar2 = new System.Windows.Forms.ToolStripButton();
            this.btnStar3 = new System.Windows.Forms.ToolStripButton();
            this.btnStar4 = new System.Windows.Forms.ToolStripButton();
            this.btnStar5 = new System.Windows.Forms.ToolStripButton();
            this.btnStar6 = new System.Windows.Forms.ToolStripButton();
            this.btnStar7 = new System.Windows.Forms.ToolStripButton();
            this.btnStarAll = new System.Windows.Forms.ToolStripButton();
            this.ssStatusBar = new System.Windows.Forms.StatusStrip();
            this.slStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).BeginInit();
            this.spcMainContainer.Panel1.SuspendLayout();
            this.spcMainContainer.Panel2.SuspendLayout();
            this.spcMainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcLeftPanel)).BeginInit();
            this.spcLeftPanel.Panel1.SuspendLayout();
            this.spcLeftPanel.Panel2.SuspendLayout();
            this.spcLeftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcPropertyInterface)).BeginInit();
            this.spcPropertyInterface.Panel1.SuspendLayout();
            this.spcPropertyInterface.Panel2.SuspendLayout();
            this.spcPropertyInterface.SuspendLayout();
            this.box_general.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_objectId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_area)).BeginInit();
            this.box_fogSettings.SuspendLayout();
            this.box_color.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.val_r)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_b)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_g)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_endDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_startDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseColor)).BeginInit();
            this.box_parameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseParameters)).BeginInit();
            this.box_rotation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseRotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_rotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_rotX)).BeginInit();
            this.box_position.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapsePosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posX)).BeginInit();
            this.tsEditActions.SuspendLayout();
            this.tsViewActions.SuspendLayout();
            this.tsToolBar.SuspendLayout();
            this.ssStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcMainContainer
            // 
            this.spcMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spcMainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcMainContainer.Location = new System.Drawing.Point(0, 25);
            this.spcMainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.spcMainContainer.Name = "spcMainContainer";
            // 
            // spcMainContainer.Panel1
            // 
            this.spcMainContainer.Panel1.Controls.Add(this.spcLeftPanel);
            this.spcMainContainer.Panel1.Controls.Add(this.tsEditActions);
            // 
            // spcMainContainer.Panel2
            // 
            this.spcMainContainer.Panel2.Controls.Add(this.glLevelView);
            this.spcMainContainer.Panel2.Controls.Add(this.tsViewActions);
            this.spcMainContainer.Size = new System.Drawing.Size(957, 486);
            this.spcMainContainer.SplitterDistance = 289;
            this.spcMainContainer.TabIndex = 0;
            // 
            // spcLeftPanel
            // 
            this.spcLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcLeftPanel.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcLeftPanel.Location = new System.Drawing.Point(0, 132);
            this.spcLeftPanel.Name = "spcLeftPanel";
            this.spcLeftPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcLeftPanel.Panel1
            // 
            this.spcLeftPanel.Panel1.Controls.Add(this.tvObjectList);
            // 
            // spcLeftPanel.Panel2
            // 
            this.spcLeftPanel.Panel2.Controls.Add(this.spcPropertyInterface);
            this.spcLeftPanel.Size = new System.Drawing.Size(289, 354);
            this.spcLeftPanel.SplitterDistance = 75;
            this.spcLeftPanel.TabIndex = 1;
            // 
            // tvObjectList
            // 
            this.tvObjectList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjectList.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.tvObjectList.HideSelection = false;
            this.tvObjectList.Location = new System.Drawing.Point(0, 0);
            this.tvObjectList.Name = "tvObjectList";
            this.tvObjectList.Size = new System.Drawing.Size(289, 75);
            this.tvObjectList.TabIndex = 0;
            this.tvObjectList.TabStop = false;
            this.tvObjectList.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvObjectList_DrawNode);
            this.tvObjectList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvObjectList_AfterSelect);
            this.tvObjectList.DoubleClick += new System.EventHandler(this.tvObjectList_DoubleClick);
            // 
            // spcPropertyInterface
            // 
            this.spcPropertyInterface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcPropertyInterface.Location = new System.Drawing.Point(0, 0);
            this.spcPropertyInterface.Name = "spcPropertyInterface";
            this.spcPropertyInterface.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcPropertyInterface.Panel1
            // 
            this.spcPropertyInterface.Panel1.AutoScroll = true;
            this.spcPropertyInterface.Panel1.Controls.Add(this.box_position);
            this.spcPropertyInterface.Panel1.Controls.Add(this.box_rotation);
            this.spcPropertyInterface.Panel1.Controls.Add(this.box_parameters);
            this.spcPropertyInterface.Panel1.Controls.Add(this.box_fogSettings);
            this.spcPropertyInterface.Panel1.Controls.Add(this.box_general);
            // 
            // spcPropertyInterface.Panel2
            // 
            this.spcPropertyInterface.Panel2.Controls.Add(this.btnPasteCoordinates);
            this.spcPropertyInterface.Panel2.Controls.Add(this.btnCopyCoordinates);
            this.spcPropertyInterface.Size = new System.Drawing.Size(289, 275);
            this.spcPropertyInterface.SplitterDistance = 245;
            this.spcPropertyInterface.TabIndex = 1;
            // 
            // box_general
            // 
            this.box_general.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_general.Controls.Add(this.btnOpenObjectList);
            this.box_general.Controls.Add(this.val_act);
            this.box_general.Controls.Add(this.btnToogleCollapseGeneral);
            this.box_general.Controls.Add(this.val_objectId);
            this.box_general.Controls.Add(this.lbl_objectId);
            this.box_general.Controls.Add(this.lbl_star);
            this.box_general.Controls.Add(this.val_area);
            this.box_general.Controls.Add(this.lbl_area);
            this.box_general.Cursor = System.Windows.Forms.Cursors.Default;
            this.box_general.Location = new System.Drawing.Point(3, -18);
            this.box_general.Name = "box_general";
            this.box_general.Size = new System.Drawing.Size(266, 68);
            this.box_general.TabIndex = 10;
            this.box_general.TabStop = false;
            this.box_general.Text = "General";
            this.box_general.Visible = false;
            // 
            // btnOpenObjectList
            // 
            this.btnOpenObjectList.Location = new System.Drawing.Point(140, 45);
            this.btnOpenObjectList.Name = "btnOpenObjectList";
            this.btnOpenObjectList.Size = new System.Drawing.Size(75, 23);
            this.btnOpenObjectList.TabIndex = 11;
            this.btnOpenObjectList.Text = "ObjectList";
            this.btnOpenObjectList.UseVisualStyleBackColor = true;
            this.btnOpenObjectList.Click += new System.EventHandler(this.btnOpenObjectList_Click);
            // 
            // val_act
            // 
            this.val_act.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.val_act.FormattingEnabled = true;
            this.val_act.Items.AddRange(new object[] {
            "All",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.val_act.Location = new System.Drawing.Point(76, 20);
            this.val_act.Name = "val_act";
            this.val_act.Size = new System.Drawing.Size(58, 21);
            this.val_act.TabIndex = 10;
            this.val_act.SelectedValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // btnToogleCollapseGeneral
            // 
            this.btnToogleCollapseGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToogleCollapseGeneral.Image = global::SM64DSe.Properties.Resources.collapseButton;
            this.btnToogleCollapseGeneral.Location = new System.Drawing.Point(250, 0);
            this.btnToogleCollapseGeneral.Name = "btnToogleCollapseGeneral";
            this.btnToogleCollapseGeneral.Size = new System.Drawing.Size(16, 16);
            this.btnToogleCollapseGeneral.TabIndex = 9;
            this.btnToogleCollapseGeneral.TabStop = false;
            this.btnToogleCollapseGeneral.Click += new System.EventHandler(this.btnToogleCollapseGeneral_Click);
            // 
            // val_objectId
            // 
            this.val_objectId.Location = new System.Drawing.Point(76, 47);
            this.val_objectId.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_objectId.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.val_objectId.Name = "val_objectId";
            this.val_objectId.Size = new System.Drawing.Size(58, 20);
            this.val_objectId.TabIndex = 5;
            this.val_objectId.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_objectId
            // 
            this.lbl_objectId.AutoSize = true;
            this.lbl_objectId.Location = new System.Drawing.Point(147, 22);
            this.lbl_objectId.Name = "lbl_objectId";
            this.lbl_objectId.Size = new System.Drawing.Size(29, 13);
            this.lbl_objectId.TabIndex = 4;
            this.lbl_objectId.Text = "Area";
            // 
            // lbl_star
            // 
            this.lbl_star.AutoSize = true;
            this.lbl_star.Location = new System.Drawing.Point(7, 21);
            this.lbl_star.Name = "lbl_star";
            this.lbl_star.Size = new System.Drawing.Size(47, 13);
            this.lbl_star.TabIndex = 3;
            this.lbl_star.Text = "Star/Act";
            // 
            // val_area
            // 
            this.val_area.Location = new System.Drawing.Point(180, 20);
            this.val_area.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_area.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.val_area.Name = "val_area";
            this.val_area.Size = new System.Drawing.Size(58, 20);
            this.val_area.TabIndex = 3;
            this.val_area.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_area
            // 
            this.lbl_area.AutoSize = true;
            this.lbl_area.Location = new System.Drawing.Point(7, 49);
            this.lbl_area.Name = "lbl_area";
            this.lbl_area.Size = new System.Drawing.Size(65, 13);
            this.lbl_area.TabIndex = 2;
            this.lbl_area.Text = "Object Type";
            // 
            // box_fogSettings
            // 
            this.box_fogSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_fogSettings.Controls.Add(this.box_color);
            this.box_fogSettings.Controls.Add(this.check_displayFog);
            this.box_fogSettings.Controls.Add(this.val_endDistance);
            this.box_fogSettings.Controls.Add(this.lbl_endDistance);
            this.box_fogSettings.Controls.Add(this.val_startDistance);
            this.box_fogSettings.Controls.Add(this.lbl_startDistance);
            this.box_fogSettings.Controls.Add(this.btnToogleCollapseColor);
            this.box_fogSettings.Cursor = System.Windows.Forms.Cursors.Default;
            this.box_fogSettings.Location = new System.Drawing.Point(3, 266);
            this.box_fogSettings.Name = "box_fogSettings";
            this.box_fogSettings.Size = new System.Drawing.Size(266, 132);
            this.box_fogSettings.TabIndex = 6;
            this.box_fogSettings.TabStop = false;
            this.box_fogSettings.Text = "Fog Settings";
            this.box_fogSettings.Visible = false;
            // 
            // box_color
            // 
            this.box_color.Controls.Add(this.lbl_r);
            this.box_color.Controls.Add(this.val_r);
            this.box_color.Controls.Add(this.lbl_g);
            this.box_color.Controls.Add(this.val_b);
            this.box_color.Controls.Add(this.lbl_b);
            this.box_color.Controls.Add(this.val_g);
            this.box_color.Location = new System.Drawing.Point(0, 37);
            this.box_color.Name = "box_color";
            this.box_color.Size = new System.Drawing.Size(242, 34);
            this.box_color.TabIndex = 16;
            // 
            // lbl_r
            // 
            this.lbl_r.AutoSize = true;
            this.lbl_r.Location = new System.Drawing.Point(3, 10);
            this.lbl_r.Name = "lbl_r";
            this.lbl_r.Size = new System.Drawing.Size(13, 13);
            this.lbl_r.TabIndex = 2;
            this.lbl_r.Text = "r:";
            // 
            // val_r
            // 
            this.val_r.Location = new System.Drawing.Point(20, 8);
            this.val_r.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_r.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.val_r.Name = "val_r";
            this.val_r.Size = new System.Drawing.Size(58, 20);
            this.val_r.TabIndex = 3;
            this.val_r.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_g
            // 
            this.lbl_g.AutoSize = true;
            this.lbl_g.Location = new System.Drawing.Point(84, 10);
            this.lbl_g.Name = "lbl_g";
            this.lbl_g.Size = new System.Drawing.Size(16, 13);
            this.lbl_g.TabIndex = 3;
            this.lbl_g.Text = "g:";
            // 
            // val_b
            // 
            this.val_b.Location = new System.Drawing.Point(180, 8);
            this.val_b.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_b.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.val_b.Name = "val_b";
            this.val_b.Size = new System.Drawing.Size(58, 20);
            this.val_b.TabIndex = 5;
            this.val_b.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_b
            // 
            this.lbl_b.AutoSize = true;
            this.lbl_b.Location = new System.Drawing.Point(160, 10);
            this.lbl_b.Name = "lbl_b";
            this.lbl_b.Size = new System.Drawing.Size(16, 13);
            this.lbl_b.TabIndex = 4;
            this.lbl_b.Text = "b:";
            // 
            // val_g
            // 
            this.val_g.Location = new System.Drawing.Point(102, 8);
            this.val_g.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_g.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.val_g.Name = "val_g";
            this.val_g.Size = new System.Drawing.Size(52, 20);
            this.val_g.TabIndex = 4;
            this.val_g.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // check_displayFog
            // 
            this.check_displayFog.AutoSize = true;
            this.check_displayFog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.check_displayFog.Location = new System.Drawing.Point(16, 19);
            this.check_displayFog.Name = "check_displayFog";
            this.check_displayFog.Size = new System.Drawing.Size(78, 17);
            this.check_displayFog.TabIndex = 15;
            this.check_displayFog.Text = "Display fog";
            this.check_displayFog.UseVisualStyleBackColor = true;
            this.check_displayFog.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // val_endDistance
            // 
            this.val_endDistance.DecimalPlaces = 4;
            this.val_endDistance.Location = new System.Drawing.Point(73, 103);
            this.val_endDistance.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_endDistance.Maximum = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.val_endDistance.Name = "val_endDistance";
            this.val_endDistance.Size = new System.Drawing.Size(77, 20);
            this.val_endDistance.TabIndex = 13;
            this.val_endDistance.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_endDistance
            // 
            this.lbl_endDistance.AutoSize = true;
            this.lbl_endDistance.Location = new System.Drawing.Point(-3, 105);
            this.lbl_endDistance.Name = "lbl_endDistance";
            this.lbl_endDistance.Size = new System.Drawing.Size(69, 13);
            this.lbl_endDistance.TabIndex = 12;
            this.lbl_endDistance.Text = "End distance";
            // 
            // val_startDistance
            // 
            this.val_startDistance.DecimalPlaces = 4;
            this.val_startDistance.Location = new System.Drawing.Point(73, 77);
            this.val_startDistance.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_startDistance.Maximum = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.val_startDistance.Name = "val_startDistance";
            this.val_startDistance.Size = new System.Drawing.Size(77, 20);
            this.val_startDistance.TabIndex = 11;
            this.val_startDistance.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_startDistance
            // 
            this.lbl_startDistance.AutoSize = true;
            this.lbl_startDistance.Location = new System.Drawing.Point(-3, 79);
            this.lbl_startDistance.Name = "lbl_startDistance";
            this.lbl_startDistance.Size = new System.Drawing.Size(72, 13);
            this.lbl_startDistance.TabIndex = 10;
            this.lbl_startDistance.Text = "Start distance";
            // 
            // btnToogleCollapseColor
            // 
            this.btnToogleCollapseColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToogleCollapseColor.Image = global::SM64DSe.Properties.Resources.collapseButton;
            this.btnToogleCollapseColor.Location = new System.Drawing.Point(250, 0);
            this.btnToogleCollapseColor.Name = "btnToogleCollapseColor";
            this.btnToogleCollapseColor.Size = new System.Drawing.Size(16, 16);
            this.btnToogleCollapseColor.TabIndex = 9;
            this.btnToogleCollapseColor.TabStop = false;
            this.btnToogleCollapseColor.Click += new System.EventHandler(this.btnToogleCollapseColor_Click);
            // 
            // box_parameters
            // 
            this.box_parameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_parameters.Controls.Add(this.btnToogleCollapseParameters);
            this.box_parameters.Cursor = System.Windows.Forms.Cursors.Default;
            this.box_parameters.Location = new System.Drawing.Point(3, 145);
            this.box_parameters.Name = "box_parameters";
            this.box_parameters.Size = new System.Drawing.Size(266, 115);
            this.box_parameters.TabIndex = 9;
            this.box_parameters.TabStop = false;
            this.box_parameters.Text = "Parameters";
            this.box_parameters.Visible = false;
            // 
            // btnToogleCollapseParameters
            // 
            this.btnToogleCollapseParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToogleCollapseParameters.Image = global::SM64DSe.Properties.Resources.collapseButton;
            this.btnToogleCollapseParameters.Location = new System.Drawing.Point(250, 0);
            this.btnToogleCollapseParameters.Name = "btnToogleCollapseParameters";
            this.btnToogleCollapseParameters.Size = new System.Drawing.Size(16, 16);
            this.btnToogleCollapseParameters.TabIndex = 10;
            this.btnToogleCollapseParameters.TabStop = false;
            this.btnToogleCollapseParameters.Click += new System.EventHandler(this.btnToogleCollapseParameters_Click);
            // 
            // box_rotation
            // 
            this.box_rotation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_rotation.Controls.Add(this.btnToogleCollapseRotation);
            this.box_rotation.Controls.Add(this.val_rotY);
            this.box_rotation.Controls.Add(this.val_rotX);
            this.box_rotation.Controls.Add(this.lbl_rotY);
            this.box_rotation.Controls.Add(this.lbl_rotX);
            this.box_rotation.Cursor = System.Windows.Forms.Cursors.Default;
            this.box_rotation.Location = new System.Drawing.Point(3, 101);
            this.box_rotation.Name = "box_rotation";
            this.box_rotation.Size = new System.Drawing.Size(266, 39);
            this.box_rotation.TabIndex = 6;
            this.box_rotation.TabStop = false;
            this.box_rotation.Text = "Rotation";
            this.box_rotation.Visible = false;
            // 
            // btnToogleCollapseRotation
            // 
            this.btnToogleCollapseRotation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToogleCollapseRotation.Image = global::SM64DSe.Properties.Resources.collapseButton;
            this.btnToogleCollapseRotation.Location = new System.Drawing.Point(250, -1);
            this.btnToogleCollapseRotation.Name = "btnToogleCollapseRotation";
            this.btnToogleCollapseRotation.Size = new System.Drawing.Size(16, 16);
            this.btnToogleCollapseRotation.TabIndex = 8;
            this.btnToogleCollapseRotation.TabStop = false;
            this.btnToogleCollapseRotation.Click += new System.EventHandler(this.btnToogleCollapseRotation_Click);
            // 
            // val_rotY
            // 
            this.val_rotY.Cursor = System.Windows.Forms.Cursors.Default;
            this.val_rotY.DecimalPlaces = 4;
            this.val_rotY.Increment = new decimal(new int[] {
            225,
            0,
            0,
            65536});
            this.val_rotY.Location = new System.Drawing.Point(109, 18);
            this.val_rotY.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_rotY.Maximum = new decimal(new int[] {
            2025,
            0,
            0,
            65536});
            this.val_rotY.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.val_rotY.Name = "val_rotY";
            this.val_rotY.Size = new System.Drawing.Size(92, 20);
            this.val_rotY.TabIndex = 8;
            this.val_rotY.Tag = "";
            this.val_rotY.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // val_rotX
            // 
            this.val_rotX.Cursor = System.Windows.Forms.Cursors.Default;
            this.val_rotX.DecimalPlaces = 1;
            this.val_rotX.Enabled = false;
            this.val_rotX.Increment = new decimal(new int[] {
            225,
            0,
            0,
            65536});
            this.val_rotX.Location = new System.Drawing.Point(26, 18);
            this.val_rotX.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_rotX.Maximum = new decimal(new int[] {
            3375,
            0,
            0,
            65536});
            this.val_rotX.Name = "val_rotX";
            this.val_rotX.Size = new System.Drawing.Size(58, 20);
            this.val_rotX.TabIndex = 7;
            this.val_rotX.Tag = "";
            this.val_rotX.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_rotY
            // 
            this.lbl_rotY.AutoSize = true;
            this.lbl_rotY.Location = new System.Drawing.Point(90, 20);
            this.lbl_rotY.Name = "lbl_rotY";
            this.lbl_rotY.Size = new System.Drawing.Size(15, 13);
            this.lbl_rotY.TabIndex = 3;
            this.lbl_rotY.Text = "y:";
            // 
            // lbl_rotX
            // 
            this.lbl_rotX.AutoSize = true;
            this.lbl_rotX.Location = new System.Drawing.Point(7, 20);
            this.lbl_rotX.Name = "lbl_rotX";
            this.lbl_rotX.Size = new System.Drawing.Size(15, 13);
            this.lbl_rotX.TabIndex = 2;
            this.lbl_rotX.Text = "x:";
            // 
            // box_position
            // 
            this.box_position.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.box_position.Controls.Add(this.btnToogleCollapsePosition);
            this.box_position.Controls.Add(this.val_posZ);
            this.box_position.Controls.Add(this.val_posY);
            this.box_position.Controls.Add(this.label4);
            this.box_position.Controls.Add(this.lbl_posY);
            this.box_position.Controls.Add(this.val_posX);
            this.box_position.Controls.Add(this.lbl_posX);
            this.box_position.Cursor = System.Windows.Forms.Cursors.Default;
            this.box_position.Location = new System.Drawing.Point(3, 56);
            this.box_position.Name = "box_position";
            this.box_position.Size = new System.Drawing.Size(266, 39);
            this.box_position.TabIndex = 2;
            this.box_position.TabStop = false;
            this.box_position.Text = "Position";
            this.box_position.Visible = false;
            // 
            // btnToogleCollapsePosition
            // 
            this.btnToogleCollapsePosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToogleCollapsePosition.Image = global::SM64DSe.Properties.Resources.collapseButton;
            this.btnToogleCollapsePosition.Location = new System.Drawing.Point(250, 0);
            this.btnToogleCollapsePosition.Name = "btnToogleCollapsePosition";
            this.btnToogleCollapsePosition.Size = new System.Drawing.Size(16, 16);
            this.btnToogleCollapsePosition.TabIndex = 7;
            this.btnToogleCollapsePosition.TabStop = false;
            this.btnToogleCollapsePosition.Click += new System.EventHandler(this.btnToogleCollapsePosition_Click);
            // 
            // val_posZ
            // 
            this.val_posZ.DecimalPlaces = 3;
            this.val_posZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posZ.Location = new System.Drawing.Point(186, 18);
            this.val_posZ.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posZ.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posZ.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posZ.Name = "val_posZ";
            this.val_posZ.Size = new System.Drawing.Size(58, 20);
            this.val_posZ.TabIndex = 5;
            this.val_posZ.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // val_posY
            // 
            this.val_posY.DecimalPlaces = 3;
            this.val_posY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posY.Location = new System.Drawing.Point(109, 18);
            this.val_posY.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posY.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posY.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posY.Name = "val_posY";
            this.val_posY.Size = new System.Drawing.Size(58, 20);
            this.val_posY.TabIndex = 4;
            this.val_posY.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(173, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "z:";
            // 
            // lbl_posY
            // 
            this.lbl_posY.AutoSize = true;
            this.lbl_posY.Location = new System.Drawing.Point(90, 20);
            this.lbl_posY.Name = "lbl_posY";
            this.lbl_posY.Size = new System.Drawing.Size(15, 13);
            this.lbl_posY.TabIndex = 3;
            this.lbl_posY.Text = "y:";
            // 
            // val_posX
            // 
            this.val_posX.DecimalPlaces = 3;
            this.val_posX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.val_posX.Location = new System.Drawing.Point(26, 18);
            this.val_posX.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.val_posX.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.val_posX.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            -2147483648});
            this.val_posX.Name = "val_posX";
            this.val_posX.Size = new System.Drawing.Size(58, 20);
            this.val_posX.TabIndex = 3;
            this.val_posX.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lbl_posX
            // 
            this.lbl_posX.AutoSize = true;
            this.lbl_posX.Location = new System.Drawing.Point(7, 20);
            this.lbl_posX.Name = "lbl_posX";
            this.lbl_posX.Size = new System.Drawing.Size(15, 13);
            this.lbl_posX.TabIndex = 2;
            this.lbl_posX.Text = "x:";
            // 
            // btnPasteCoordinates
            // 
            this.btnPasteCoordinates.Enabled = false;
            this.btnPasteCoordinates.Location = new System.Drawing.Point(134, 0);
            this.btnPasteCoordinates.Name = "btnPasteCoordinates";
            this.btnPasteCoordinates.Size = new System.Drawing.Size(130, 23);
            this.btnPasteCoordinates.TabIndex = 2;
            this.btnPasteCoordinates.Text = "PasteCoordinates";
            this.btnPasteCoordinates.UseVisualStyleBackColor = true;
            this.btnPasteCoordinates.Visible = false;
            this.btnPasteCoordinates.Click += new System.EventHandler(this.btnPasteCoordinates_Click);
            // 
            // btnCopyCoordinates
            // 
            this.btnCopyCoordinates.Location = new System.Drawing.Point(0, 0);
            this.btnCopyCoordinates.Name = "btnCopyCoordinates";
            this.btnCopyCoordinates.Size = new System.Drawing.Size(128, 23);
            this.btnCopyCoordinates.TabIndex = 1;
            this.btnCopyCoordinates.Text = "CopyCoordinates";
            this.btnCopyCoordinates.UseVisualStyleBackColor = true;
            this.btnCopyCoordinates.Visible = false;
            this.btnCopyCoordinates.Click += new System.EventHandler(this.btnCopyCoordinates_Click);
            // 
            // tsEditActions
            // 
            this.tsEditActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportModel,
            this.btnExportLevelModel,
            this.btnAddObject,
            this.btnAddWarp,
            this.btnAddView,
            this.btnRemoveSel,
            this.btnRemoveAll,
            this.btnReplaceObjModel,
            this.btnExportObjectModel,
            this.btnAddPath,
            this.btnAddMisc,
            this.btnImportOtherModel,
            this.btnExportOtherModel,
            this.btnOffsetAllCoords,
            this.btnAddPathNode});
            this.tsEditActions.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.tsEditActions.Location = new System.Drawing.Point(0, 0);
            this.tsEditActions.Name = "tsEditActions";
            this.tsEditActions.Size = new System.Drawing.Size(289, 132);
            this.tsEditActions.TabIndex = 0;
            this.tsEditActions.Text = "toolStrip1";
            // 
            // btnImportModel
            // 
            this.btnImportModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportModel.Image = ((System.Drawing.Image)(resources.GetObject("btnImportModel.Image")));
            this.btnImportModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportModel.Name = "btnImportModel";
            this.btnImportModel.Size = new System.Drawing.Size(111, 19);
            this.btnImportModel.Text = "Import level model";
            this.btnImportModel.ToolTipText = "Replace the 3D model with one from a .obj file";
            this.btnImportModel.Click += new System.EventHandler(this.btnImportModel_Click);
            // 
            // btnExportLevelModel
            // 
            this.btnExportLevelModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportLevelModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportLevelModel.Name = "btnExportLevelModel";
            this.btnExportLevelModel.Size = new System.Drawing.Size(108, 19);
            this.btnExportLevelModel.Text = "Export level model";
            this.btnExportLevelModel.Click += new System.EventHandler(this.btnExportLevelModel_Click);
            // 
            // btnAddObject
            // 
            this.btnAddObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddObject.Image = ((System.Drawing.Image)(resources.GetObject("btnAddObject.Image")));
            this.btnAddObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddObject.Name = "btnAddObject";
            this.btnAddObject.Size = new System.Drawing.Size(69, 19);
            this.btnAddObject.Tag = "1,1";
            this.btnAddObject.Text = "Add object";
            this.btnAddObject.ToolTipText = "Add an enemy, item or whatever";
            this.btnAddObject.Click += new System.EventHandler(this.btnAddObject_Click);
            // 
            // btnAddWarp
            // 
            this.btnAddWarp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddWarp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddEntrance,
            this.btnAddExit,
            this.btnAddAreaWarp,
            this.btnAddTpSrc,
            this.btnAddTpDst,
            this.toolStripMenuItem5});
            this.btnAddWarp.Image = ((System.Drawing.Image)(resources.GetObject("btnAddWarp.Image")));
            this.btnAddWarp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddWarp.Name = "btnAddWarp";
            this.btnAddWarp.Size = new System.Drawing.Size(71, 19);
            this.btnAddWarp.Text = "Add warp";
            this.btnAddWarp.ToolTipText = "Add an entrance, exit, door, teleport or whatever";
            // 
            // btnAddEntrance
            // 
            this.btnAddEntrance.Name = "btnAddEntrance";
            this.btnAddEntrance.Size = new System.Drawing.Size(180, 22);
            this.btnAddEntrance.Tag = "1";
            this.btnAddEntrance.Text = "Entrance";
            this.btnAddEntrance.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // btnAddExit
            // 
            this.btnAddExit.Name = "btnAddExit";
            this.btnAddExit.Size = new System.Drawing.Size(180, 22);
            this.btnAddExit.Tag = "10";
            this.btnAddExit.Text = "Exit";
            this.btnAddExit.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // btnAddAreaWarp
            // 
            this.btnAddAreaWarp.Name = "btnAddAreaWarp";
            this.btnAddAreaWarp.Size = new System.Drawing.Size(180, 22);
            this.btnAddAreaWarp.Tag = "9";
            this.btnAddAreaWarp.Text = "Door";
            this.btnAddAreaWarp.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // btnAddTpSrc
            // 
            this.btnAddTpSrc.Name = "btnAddTpSrc";
            this.btnAddTpSrc.Size = new System.Drawing.Size(180, 22);
            this.btnAddTpSrc.Tag = "6";
            this.btnAddTpSrc.Text = "Teleport source";
            this.btnAddTpSrc.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // btnAddTpDst
            // 
            this.btnAddTpDst.Name = "btnAddTpDst";
            this.btnAddTpDst.Size = new System.Drawing.Size(180, 22);
            this.btnAddTpDst.Tag = "7";
            this.btnAddTpDst.Text = "Teleport destination";
            this.btnAddTpDst.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem5.Text = "toolStripMenuItem5";
            // 
            // btnAddView
            // 
            this.btnAddView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddView.Image = ((System.Drawing.Image)(resources.GetObject("btnAddView.Image")));
            this.btnAddView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(60, 19);
            this.btnAddView.Tag = "4";
            this.btnAddView.Text = "Add view";
            this.btnAddView.ToolTipText = "Add a view (camera anchor)";
            this.btnAddView.Click += new System.EventHandler(this.btnAddWhatever_Click);
            // 
            // btnRemoveSel
            // 
            this.btnRemoveSel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveSel.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveSel.Image")));
            this.btnRemoveSel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveSel.Name = "btnRemoveSel";
            this.btnRemoveSel.Size = new System.Drawing.Size(90, 19);
            this.btnRemoveSel.Text = "Remove object";
            this.btnRemoveSel.ToolTipText = "Remove an object.";
            this.btnRemoveSel.Click += new System.EventHandler(this.btnRemoveSel_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(69, 19);
            this.btnRemoveAll.Text = "Remove all";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnReplaceObjModel
            // 
            this.btnReplaceObjModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReplaceObjModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReplaceObjModel.Name = "btnReplaceObjModel";
            this.btnReplaceObjModel.Size = new System.Drawing.Size(89, 19);
            this.btnReplaceObjModel.Text = "Replace model";
            this.btnReplaceObjModel.Click += new System.EventHandler(this.btnReplaceObjModel_Click);
            // 
            // btnExportObjectModel
            // 
            this.btnExportObjectModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportObjectModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportObjectModel.Name = "btnExportObjectModel";
            this.btnExportObjectModel.Size = new System.Drawing.Size(81, 19);
            this.btnExportObjectModel.Text = "Export model";
            this.btnExportObjectModel.Click += new System.EventHandler(this.btnExportObjectModel_Click);
            // 
            // btnAddPath
            // 
            this.btnAddPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddPath.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddPath.Name = "btnAddPath";
            this.btnAddPath.Size = new System.Drawing.Size(60, 19);
            this.btnAddPath.Text = "Add Path";
            this.btnAddPath.Click += new System.EventHandler(this.btnAddPath_Click);
            // 
            // btnAddMisc
            // 
            this.btnAddMisc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddMisc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.btnAddMisc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddMisc.Name = "btnAddMisc";
            this.btnAddMisc.Size = new System.Drawing.Size(111, 19);
            this.btnAddMisc.Text = "Add Misc. Object";
            this.btnAddMisc.ToolTipText = "Add Miscellaneous Object";
            this.btnAddMisc.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnAddMisc_DropDownItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem1.Tag = "8";
            this.toolStripMenuItem1.Text = "Fog";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem2.Tag = "12";
            this.toolStripMenuItem2.Text = "Minimap Scale";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem3.Tag = "11";
            this.toolStripMenuItem3.Text = "Minimap Tile ID";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(161, 22);
            this.toolStripMenuItem4.Tag = "14";
            this.toolStripMenuItem4.Text = "Unkown Type 14";
            // 
            // btnImportOtherModel
            // 
            this.btnImportOtherModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportOtherModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportOtherModel.Name = "btnImportOtherModel";
            this.btnImportOtherModel.Size = new System.Drawing.Size(115, 19);
            this.btnImportOtherModel.Text = "Import other model";
            this.btnImportOtherModel.Click += new System.EventHandler(this.btnImportOtherModel_Click);
            // 
            // btnExportOtherModel
            // 
            this.btnExportOtherModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportOtherModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportOtherModel.Name = "btnExportOtherModel";
            this.btnExportOtherModel.Size = new System.Drawing.Size(112, 19);
            this.btnExportOtherModel.Text = "Export other model";
            this.btnExportOtherModel.Click += new System.EventHandler(this.btnExportOtherModel_Click);
            // 
            // btnOffsetAllCoords
            // 
            this.btnOffsetAllCoords.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOffsetAllCoords.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOffsetAllCoords.Name = "btnOffsetAllCoords";
            this.btnOffsetAllCoords.Size = new System.Drawing.Size(106, 19);
            this.btnOffsetAllCoords.Text = "Offset All Co-ords";
            // 
            // btnAddPathNode
            // 
            this.btnAddPathNode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddPathNode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddPathNode.Name = "btnAddPathNode";
            this.btnAddPathNode.Size = new System.Drawing.Size(92, 19);
            this.btnAddPathNode.Text = "Add Path Node";
            this.btnAddPathNode.Click += new System.EventHandler(this.btnAddPathNode_Click);
            // 
            // glLevelView
            // 
            this.glLevelView.BackColor = System.Drawing.Color.Black;
            this.glLevelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glLevelView.Location = new System.Drawing.Point(0, 25);
            this.glLevelView.Name = "glLevelView";
            this.glLevelView.Size = new System.Drawing.Size(664, 461);
            this.glLevelView.TabIndex = 1;
            this.glLevelView.VSync = false;
            this.glLevelView.Load += new System.EventHandler(this.glLevelView_Load);
            this.glLevelView.Paint += new System.Windows.Forms.PaintEventHandler(this.glLevelView_Paint);
            this.glLevelView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glLevelView_KeyDown);
            this.glLevelView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glLevelView_KeyUp);
            this.glLevelView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glLevelView_MouseDown);
            this.glLevelView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glLevelView_MouseMove);
            this.glLevelView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glLevelView_MouseUp);
            this.glLevelView.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glLevelView_MouseWheel);
            this.glLevelView.Resize += new System.EventHandler(this.glLevelView_Resize);
            // 
            // tsViewActions
            // 
            this.tsViewActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLOL,
            this.btnImportXML,
            this.btnExportXML,
            this.btnScreenshot,
            this.toolStripSeparator4,
            this.btnOrthView,
            this.menuGridSettings,
            this.menuRestrictionPlane,
            this.btnMakeOverlay});
            this.tsViewActions.Location = new System.Drawing.Point(0, 0);
            this.tsViewActions.Name = "tsViewActions";
            this.tsViewActions.Size = new System.Drawing.Size(664, 25);
            this.tsViewActions.TabIndex = 0;
            this.tsViewActions.Text = "toolStrip2";
            // 
            // btnLOL
            // 
            this.btnLOL.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnLOL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLOL.Image = ((System.Drawing.Image)(resources.GetObject("btnLOL.Image")));
            this.btnLOL.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLOL.Name = "btnLOL";
            this.btnLOL.Size = new System.Drawing.Size(24, 22);
            this.btnLOL.Text = "lol";
            this.btnLOL.ToolTipText = "general purpose button";
            this.btnLOL.Visible = false;
            this.btnLOL.Click += new System.EventHandler(this.btnLOL_Click);
            // 
            // btnImportXML
            // 
            this.btnImportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportXML.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportXML.Name = "btnImportXML";
            this.btnImportXML.Size = new System.Drawing.Size(74, 22);
            this.btnImportXML.Text = "Import XML";
            this.btnImportXML.Click += new System.EventHandler(this.btnImportXML_Click);
            // 
            // btnExportXML
            // 
            this.btnExportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExportXML.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportXML.Name = "btnExportXML";
            this.btnExportXML.Size = new System.Drawing.Size(71, 22);
            this.btnExportXML.Text = "Export XML";
            this.btnExportXML.Click += new System.EventHandler(this.btnExportXML_Click);
            // 
            // btnScreenshot
            // 
            this.btnScreenshot.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnScreenshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScreenshot.Name = "btnScreenshot";
            this.btnScreenshot.Size = new System.Drawing.Size(69, 22);
            this.btnScreenshot.Text = "Screenshot";
            this.btnScreenshot.Click += new System.EventHandler(this.btnScreenshot_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOrthView
            // 
            this.btnOrthView.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnOrthView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOrthView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOrthView.Name = "btnOrthView";
            this.btnOrthView.Size = new System.Drawing.Size(100, 22);
            this.btnOrthView.Text = "Orthogonal View";
            this.btnOrthView.Click += new System.EventHandler(this.btnOrthView_Click);
            // 
            // menuGridSettings
            // 
            this.menuGridSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuGridSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtGridSizeX,
            this.txtGridSizeY,
            this.txtGridSizeZ,
            this.toolStripSeparator5,
            this.txtGridOffsetX,
            this.txtGridOffsetY,
            this.txtGridOffsetZ});
            this.menuGridSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuGridSettings.Name = "menuGridSettings";
            this.menuGridSettings.Size = new System.Drawing.Size(42, 22);
            this.menuGridSettings.Text = "Grid";
            this.menuGridSettings.DropDownClosed += new System.EventHandler(this.menuGridSettings_DropDownClosed);
            // 
            // txtGridSizeX
            // 
            this.txtGridSizeX.Name = "txtGridSizeX";
            this.txtGridSizeX.Size = new System.Drawing.Size(100, 23);
            this.txtGridSizeX.Text = "0.0";
            this.txtGridSizeX.ToolTipText = "Size X";
            // 
            // txtGridSizeY
            // 
            this.txtGridSizeY.Name = "txtGridSizeY";
            this.txtGridSizeY.Size = new System.Drawing.Size(100, 23);
            this.txtGridSizeY.Text = "0.0";
            this.txtGridSizeY.ToolTipText = "Size Y";
            // 
            // txtGridSizeZ
            // 
            this.txtGridSizeZ.Name = "txtGridSizeZ";
            this.txtGridSizeZ.Size = new System.Drawing.Size(100, 23);
            this.txtGridSizeZ.Text = "0.0";
            this.txtGridSizeZ.ToolTipText = "Size Z";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(157, 6);
            // 
            // txtGridOffsetX
            // 
            this.txtGridOffsetX.Name = "txtGridOffsetX";
            this.txtGridOffsetX.Size = new System.Drawing.Size(100, 23);
            this.txtGridOffsetX.Text = "0.0";
            this.txtGridOffsetX.ToolTipText = "Offset X";
            // 
            // txtGridOffsetY
            // 
            this.txtGridOffsetY.Name = "txtGridOffsetY";
            this.txtGridOffsetY.Size = new System.Drawing.Size(100, 23);
            this.txtGridOffsetY.Text = "0.0";
            this.txtGridOffsetY.ToolTipText = "Offset Y";
            // 
            // txtGridOffsetZ
            // 
            this.txtGridOffsetZ.Name = "txtGridOffsetZ";
            this.txtGridOffsetZ.Size = new System.Drawing.Size(100, 23);
            this.txtGridOffsetZ.Text = "0.0";
            this.txtGridOffsetZ.ToolTipText = "Offset Z";
            // 
            // menuRestrictionPlane
            // 
            this.menuRestrictionPlane.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menuRestrictionPlane.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtRstPlaneX,
            this.txtRstPlaneY,
            this.txtRstPlaneZ,
            this.toolStripSeparator6,
            this.txtRstPlaneOffX,
            this.txtRstPlaneOffY,
            this.txtRstPlaneOffZ});
            this.menuRestrictionPlane.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuRestrictionPlane.Name = "menuRestrictionPlane";
            this.menuRestrictionPlane.Size = new System.Drawing.Size(108, 22);
            this.menuRestrictionPlane.Text = "Restriction Plane";
            this.menuRestrictionPlane.DropDownClosed += new System.EventHandler(this.menuRestrictionPlane_DropDownClosed);
            // 
            // txtRstPlaneX
            // 
            this.txtRstPlaneX.Name = "txtRstPlaneX";
            this.txtRstPlaneX.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneX.Text = "0.0";
            this.txtRstPlaneX.ToolTipText = "Normal X";
            // 
            // txtRstPlaneY
            // 
            this.txtRstPlaneY.Name = "txtRstPlaneY";
            this.txtRstPlaneY.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneY.Text = "0.0";
            this.txtRstPlaneY.ToolTipText = "Normal Y";
            // 
            // txtRstPlaneZ
            // 
            this.txtRstPlaneZ.Name = "txtRstPlaneZ";
            this.txtRstPlaneZ.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneZ.Text = "0.0";
            this.txtRstPlaneZ.ToolTipText = "Normal Z";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(157, 6);
            // 
            // txtRstPlaneOffX
            // 
            this.txtRstPlaneOffX.Name = "txtRstPlaneOffX";
            this.txtRstPlaneOffX.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneOffX.Text = "0.0";
            // 
            // txtRstPlaneOffY
            // 
            this.txtRstPlaneOffY.Name = "txtRstPlaneOffY";
            this.txtRstPlaneOffY.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneOffY.Text = "0.0";
            // 
            // txtRstPlaneOffZ
            // 
            this.txtRstPlaneOffZ.Name = "txtRstPlaneOffZ";
            this.txtRstPlaneOffZ.Size = new System.Drawing.Size(100, 23);
            this.txtRstPlaneOffZ.Text = "0.0";
            this.txtRstPlaneOffZ.ToolTipText = "Distance from Origin";
            // 
            // btnMakeOverlay
            // 
            this.btnMakeOverlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMakeOverlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMakeOverlay.Name = "btnMakeOverlay";
            this.btnMakeOverlay.Size = new System.Drawing.Size(113, 22);
            this.btnMakeOverlay.Text = "Make Level Overlay";
            this.btnMakeOverlay.ToolTipText = "Ready to show off your level coding skills?";
            this.btnMakeOverlay.Click += new System.EventHandler(this.btnMakeOverlay_Click);
            // 
            // tsToolBar
            // 
            this.tsToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripSeparator1,
            this.btnLevelSettings,
            this.btnEditMinimap,
            this.btnEditTexAnim,
            this.btnCLPS,
            this.toolStripSeparator2,
            this.toolStripLabel4,
            this.btnEdit3DModel,
            this.btnEditObjects,
            this.btnEditWarps,
            this.btnEditPaths,
            this.btnEditViews,
            this.btnEditMisc,
            this.toolStripSeparator3,
            this.toolStripLabel1,
            this.btnStar1,
            this.btnStar2,
            this.btnStar3,
            this.btnStar4,
            this.btnStar5,
            this.btnStar6,
            this.btnStar7,
            this.btnStarAll});
            this.tsToolBar.Location = new System.Drawing.Point(0, 0);
            this.tsToolBar.Name = "tsToolBar";
            this.tsToolBar.Size = new System.Drawing.Size(957, 25);
            this.tsToolBar.TabIndex = 1;
            this.tsToolBar.TabStop = true;
            this.tsToolBar.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 22);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Click me regularly if you don\'t\r\nwant to lose your changes.";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnLevelSettings
            // 
            this.btnLevelSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLevelSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnLevelSettings.Image")));
            this.btnLevelSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLevelSettings.Name = "btnLevelSettings";
            this.btnLevelSettings.Size = new System.Drawing.Size(82, 22);
            this.btnLevelSettings.Text = "Level settings";
            this.btnLevelSettings.ToolTipText = "Change the sky, the object banks and all that.";
            this.btnLevelSettings.Click += new System.EventHandler(this.btnLevelSettings_Click);
            // 
            // btnEditMinimap
            // 
            this.btnEditMinimap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditMinimap.Image = ((System.Drawing.Image)(resources.GetObject("btnEditMinimap.Image")));
            this.btnEditMinimap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditMinimap.Name = "btnEditMinimap";
            this.btnEditMinimap.Size = new System.Drawing.Size(59, 22);
            this.btnEditMinimap.Text = "Minimap";
            this.btnEditMinimap.ToolTipText = "Edit those graphics on the bottom screen";
            this.btnEditMinimap.Click += new System.EventHandler(this.btnEditMinimap_Click);
            // 
            // btnEditTexAnim
            // 
            this.btnEditTexAnim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditTexAnim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditTexAnim.Name = "btnEditTexAnim";
            this.btnEditTexAnim.Size = new System.Drawing.Size(106, 22);
            this.btnEditTexAnim.Text = "Texture animation";
            this.btnEditTexAnim.Click += new System.EventHandler(this.btnEditTexAnim_Click);
            // 
            // btnCLPS
            // 
            this.btnCLPS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCLPS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCLPS.Name = "btnCLPS";
            this.btnCLPS.Size = new System.Drawing.Size(38, 22);
            this.btnCLPS.Text = "CLPS";
            this.btnCLPS.Click += new System.EventHandler(this.btnCLPS_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabel4.Text = "Edit:";
            // 
            // btnEdit3DModel
            // 
            this.btnEdit3DModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEdit3DModel.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit3DModel.Image")));
            this.btnEdit3DModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit3DModel.Name = "btnEdit3DModel";
            this.btnEdit3DModel.Size = new System.Drawing.Size(62, 22);
            this.btnEdit3DModel.Tag = "0";
            this.btnEdit3DModel.Text = "3D Model";
            this.btnEdit3DModel.ToolTipText = "Let\'s add polygons!";
            this.btnEdit3DModel.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // btnEditObjects
            // 
            this.btnEditObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditObjects.Image = ((System.Drawing.Image)(resources.GetObject("btnEditObjects.Image")));
            this.btnEditObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditObjects.Name = "btnEditObjects";
            this.btnEditObjects.Size = new System.Drawing.Size(51, 22);
            this.btnEditObjects.Tag = "1";
            this.btnEditObjects.Text = "Objects";
            this.btnEditObjects.ToolTipText = "Add coins, trees, Goombas and whatnot";
            this.btnEditObjects.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // btnEditWarps
            // 
            this.btnEditWarps.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditWarps.Image = ((System.Drawing.Image)(resources.GetObject("btnEditWarps.Image")));
            this.btnEditWarps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditWarps.Name = "btnEditWarps";
            this.btnEditWarps.Size = new System.Drawing.Size(44, 22);
            this.btnEditWarps.Tag = "2";
            this.btnEditWarps.Text = "Warps";
            this.btnEditWarps.ToolTipText = "Modify the entrances and exits";
            this.btnEditWarps.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // btnEditPaths
            // 
            this.btnEditPaths.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditPaths.Image = ((System.Drawing.Image)(resources.GetObject("btnEditPaths.Image")));
            this.btnEditPaths.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditPaths.Name = "btnEditPaths";
            this.btnEditPaths.Size = new System.Drawing.Size(40, 22);
            this.btnEditPaths.Tag = "3";
            this.btnEditPaths.Text = "Paths";
            this.btnEditPaths.ToolTipText = "Make some objects follow funny paths";
            this.btnEditPaths.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // btnEditViews
            // 
            this.btnEditViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditViews.Image = ((System.Drawing.Image)(resources.GetObject("btnEditViews.Image")));
            this.btnEditViews.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditViews.Name = "btnEditViews";
            this.btnEditViews.Size = new System.Drawing.Size(41, 22);
            this.btnEditViews.Tag = "4";
            this.btnEditViews.Text = "Views";
            this.btnEditViews.ToolTipText = "Change view angles";
            this.btnEditViews.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // btnEditMisc
            // 
            this.btnEditMisc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditMisc.Image = ((System.Drawing.Image)(resources.GetObject("btnEditMisc.Image")));
            this.btnEditMisc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditMisc.Name = "btnEditMisc";
            this.btnEditMisc.Size = new System.Drawing.Size(36, 22);
            this.btnEditMisc.Tag = "5";
            this.btnEditMisc.Text = "Misc";
            this.btnEditMisc.ToolTipText = "Minimap settings and all...";
            this.btnEditMisc.Click += new System.EventHandler(this.btnEditXXX_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 22);
            this.toolStripLabel1.Text = "For star:";
            // 
            // btnStar1
            // 
            this.btnStar1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar1.Image = ((System.Drawing.Image)(resources.GetObject("btnStar1.Image")));
            this.btnStar1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar1.Name = "btnStar1";
            this.btnStar1.Size = new System.Drawing.Size(23, 22);
            this.btnStar1.Tag = "1";
            this.btnStar1.Text = "1";
            this.btnStar1.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar2
            // 
            this.btnStar2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar2.Image = ((System.Drawing.Image)(resources.GetObject("btnStar2.Image")));
            this.btnStar2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar2.Name = "btnStar2";
            this.btnStar2.Size = new System.Drawing.Size(23, 22);
            this.btnStar2.Tag = "2";
            this.btnStar2.Text = "2";
            this.btnStar2.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar3
            // 
            this.btnStar3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar3.Image = ((System.Drawing.Image)(resources.GetObject("btnStar3.Image")));
            this.btnStar3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar3.Name = "btnStar3";
            this.btnStar3.Size = new System.Drawing.Size(23, 22);
            this.btnStar3.Tag = "3";
            this.btnStar3.Text = "3";
            this.btnStar3.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar4
            // 
            this.btnStar4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar4.Image = ((System.Drawing.Image)(resources.GetObject("btnStar4.Image")));
            this.btnStar4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar4.Name = "btnStar4";
            this.btnStar4.Size = new System.Drawing.Size(23, 22);
            this.btnStar4.Tag = "4";
            this.btnStar4.Text = "4";
            this.btnStar4.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar5
            // 
            this.btnStar5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar5.Image = ((System.Drawing.Image)(resources.GetObject("btnStar5.Image")));
            this.btnStar5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar5.Name = "btnStar5";
            this.btnStar5.Size = new System.Drawing.Size(23, 22);
            this.btnStar5.Tag = "5";
            this.btnStar5.Text = "5";
            this.btnStar5.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar6
            // 
            this.btnStar6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar6.Image = ((System.Drawing.Image)(resources.GetObject("btnStar6.Image")));
            this.btnStar6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar6.Name = "btnStar6";
            this.btnStar6.Size = new System.Drawing.Size(23, 22);
            this.btnStar6.Tag = "6";
            this.btnStar6.Text = "6";
            this.btnStar6.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStar7
            // 
            this.btnStar7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStar7.Image = ((System.Drawing.Image)(resources.GetObject("btnStar7.Image")));
            this.btnStar7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStar7.Name = "btnStar7";
            this.btnStar7.Size = new System.Drawing.Size(23, 22);
            this.btnStar7.Tag = "7";
            this.btnStar7.Text = "7";
            this.btnStar7.Click += new System.EventHandler(this.btnStarX_Click);
            // 
            // btnStarAll
            // 
            this.btnStarAll.CheckOnClick = true;
            this.btnStarAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStarAll.DoubleClickEnabled = true;
            this.btnStarAll.Image = ((System.Drawing.Image)(resources.GetObject("btnStarAll.Image")));
            this.btnStarAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStarAll.Name = "btnStarAll";
            this.btnStarAll.Size = new System.Drawing.Size(52, 22);
            this.btnStarAll.Text = "All stars";
            this.btnStarAll.ToolTipText = "Common objects";
            this.btnStarAll.Click += new System.EventHandler(this.btnStarAll_Click);
            this.btnStarAll.DoubleClick += new System.EventHandler(this.btnStarAll_DoubleClick);
            // 
            // ssStatusBar
            // 
            this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slStatusLabel});
            this.ssStatusBar.Location = new System.Drawing.Point(0, 511);
            this.ssStatusBar.Name = "ssStatusBar";
            this.ssStatusBar.Size = new System.Drawing.Size(957, 22);
            this.ssStatusBar.TabIndex = 2;
            this.ssStatusBar.Text = "statusStrip1";
            // 
            // slStatusLabel
            // 
            this.slStatusLabel.Name = "slStatusLabel";
            this.slStatusLabel.Size = new System.Drawing.Size(942, 17);
            this.slStatusLabel.Spring = true;
            this.slStatusLabel.Text = "status!";
            this.slStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.slStatusLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.slStatusLabel_Paint);
            this.slStatusLabel.TextChanged += new System.EventHandler(this.slStatusLabel_TextChanged);
            // 
            // LevelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(957, 533);
            this.Controls.Add(this.ssStatusBar);
            this.Controls.Add(this.tsToolBar);
            this.Controls.Add(this.spcMainContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LevelEditorForm";
            this.Text = "LevelEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelEditorForm_FormClosing);
            this.Load += new System.EventHandler(this.LevelEditorForm_Load);
            this.spcMainContainer.Panel1.ResumeLayout(false);
            this.spcMainContainer.Panel1.PerformLayout();
            this.spcMainContainer.Panel2.ResumeLayout(false);
            this.spcMainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMainContainer)).EndInit();
            this.spcMainContainer.ResumeLayout(false);
            this.spcLeftPanel.Panel1.ResumeLayout(false);
            this.spcLeftPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcLeftPanel)).EndInit();
            this.spcLeftPanel.ResumeLayout(false);
            this.spcPropertyInterface.Panel1.ResumeLayout(false);
            this.spcPropertyInterface.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcPropertyInterface)).EndInit();
            this.spcPropertyInterface.ResumeLayout(false);
            this.box_general.ResumeLayout(false);
            this.box_general.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_objectId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_area)).EndInit();
            this.box_fogSettings.ResumeLayout(false);
            this.box_fogSettings.PerformLayout();
            this.box_color.ResumeLayout(false);
            this.box_color.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.val_r)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_b)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_g)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_endDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_startDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseColor)).EndInit();
            this.box_parameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseParameters)).EndInit();
            this.box_rotation.ResumeLayout(false);
            this.box_rotation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapseRotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_rotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_rotX)).EndInit();
            this.box_position.ResumeLayout(false);
            this.box_position.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnToogleCollapsePosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.val_posX)).EndInit();
            this.tsEditActions.ResumeLayout(false);
            this.tsEditActions.PerformLayout();
            this.tsViewActions.ResumeLayout(false);
            this.tsViewActions.PerformLayout();
            this.tsToolBar.ResumeLayout(false);
            this.tsToolBar.PerformLayout();
            this.ssStatusBar.ResumeLayout(false);
            this.ssStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcMainContainer;
        private System.Windows.Forms.ToolStrip tsToolBar;
        private System.Windows.Forms.StatusStrip ssStatusBar;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnLevelSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStrip tsViewActions;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripButton btnEditObjects;
        private System.Windows.Forms.ToolStripButton btnEdit3DModel;
        private System.Windows.Forms.ToolStripButton btnEditWarps;
        private System.Windows.Forms.ToolStripButton btnEditPaths;
        private System.Windows.Forms.ToolStripButton btnEditViews;
        private System.Windows.Forms.ToolStripButton btnEditMisc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnStar1;
        private System.Windows.Forms.ToolStripButton btnStar2;
        private System.Windows.Forms.ToolStripButton btnStar3;
        private System.Windows.Forms.ToolStripButton btnStar4;
        private System.Windows.Forms.ToolStripButton btnStar5;
        private System.Windows.Forms.ToolStripButton btnStar6;
        private System.Windows.Forms.ToolStripButton btnStar7;
        private System.Windows.Forms.ToolStripButton btnStarAll;
        private System.Windows.Forms.SplitContainer spcLeftPanel;
        private System.Windows.Forms.TreeView tvObjectList;
        private System.Windows.Forms.ToolStrip tsEditActions;
        private System.Windows.Forms.ToolStripStatusLabel slStatusLabel;
        private System.Windows.Forms.ToolStripButton btnImportModel;
        private System.Windows.Forms.ToolStripButton btnRemoveSel;
        private System.Windows.Forms.ToolStripButton btnAddObject;
        private System.Windows.Forms.ToolStripDropDownButton btnAddWarp;
        private System.Windows.Forms.ToolStripMenuItem btnAddEntrance;
        private System.Windows.Forms.ToolStripMenuItem btnAddExit;
        private System.Windows.Forms.ToolStripMenuItem btnAddAreaWarp;
        private System.Windows.Forms.ToolStripMenuItem btnAddTpSrc;
        private System.Windows.Forms.ToolStripMenuItem btnAddTpDst;
        private System.Windows.Forms.ToolStripButton btnAddView;
        private System.Windows.Forms.ToolStripButton btnLOL;
        private OpenTK.GLControl glLevelView;
        private System.Windows.Forms.ToolStripButton btnEditMinimap;
        private System.Windows.Forms.ToolStripButton btnReplaceObjModel;
        private System.Windows.Forms.ToolStripButton btnExportObjectModel;
        private System.Windows.Forms.ToolStripButton btnEditTexAnim;
        private System.Windows.Forms.ToolStripButton btnCLPS;
        private System.Windows.Forms.ToolStripButton btnAddPath;
        private System.Windows.Forms.ToolStripDropDownButton btnAddMisc;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripButton btnImportOtherModel;
        private System.Windows.Forms.ToolStripButton btnExportOtherModel;
        private System.Windows.Forms.ToolStripButton btnAddPathNode;
        private System.Windows.Forms.ToolStripButton btnExportLevelModel;
        private System.Windows.Forms.ToolStripButton btnImportXML;
        private System.Windows.Forms.ToolStripButton btnExportXML;
        private System.Windows.Forms.ToolStripButton btnOrthView;
        private System.Windows.Forms.ToolStripButton btnScreenshot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton menuGridSettings;
        private System.Windows.Forms.ToolStripTextBox txtGridSizeX;
        private System.Windows.Forms.ToolStripTextBox txtGridSizeY;
        private System.Windows.Forms.ToolStripTextBox txtGridSizeZ;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripTextBox txtGridOffsetX;
        private System.Windows.Forms.ToolStripTextBox txtGridOffsetY;
        private System.Windows.Forms.ToolStripTextBox txtGridOffsetZ;
        private System.Windows.Forms.ToolStripDropDownButton menuRestrictionPlane;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneX;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneY;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneZ;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneOffZ;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneOffX;
        private System.Windows.Forms.ToolStripTextBox txtRstPlaneOffY;
        private System.Windows.Forms.ToolStripButton btnMakeOverlay;
        private System.Windows.Forms.ToolStripButton btnRemoveAll;
        private System.Windows.Forms.Button btnCopyCoordinates;
        private System.Windows.Forms.Button btnPasteCoordinates;
        private System.Windows.Forms.GroupBox box_parameters;
        private System.Windows.Forms.GroupBox box_rotation;
        private System.Windows.Forms.NumericUpDown val_rotX;
        private System.Windows.Forms.Label lbl_rotY;
        private System.Windows.Forms.Label lbl_rotX;
        private System.Windows.Forms.GroupBox box_position;
        private System.Windows.Forms.NumericUpDown val_posZ;
        private System.Windows.Forms.NumericUpDown val_posY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_posY;
        private System.Windows.Forms.NumericUpDown val_posX;
        private System.Windows.Forms.Label lbl_posX;
        private System.Windows.Forms.GroupBox box_fogSettings;
        private System.Windows.Forms.NumericUpDown val_b;
        private System.Windows.Forms.NumericUpDown val_g;
        private System.Windows.Forms.Label lbl_b;
        private System.Windows.Forms.Label lbl_g;
        private System.Windows.Forms.NumericUpDown val_r;
        private System.Windows.Forms.Label lbl_r;
        private System.Windows.Forms.NumericUpDown val_rotY;
        private System.Windows.Forms.PictureBox btnToogleCollapsePosition;
        private System.Windows.Forms.PictureBox btnToogleCollapseColor;
        private System.Windows.Forms.PictureBox btnToogleCollapseParameters;
        private System.Windows.Forms.PictureBox btnToogleCollapseRotation;
        private System.Windows.Forms.GroupBox box_general;
        private System.Windows.Forms.PictureBox btnToogleCollapseGeneral;
        private System.Windows.Forms.NumericUpDown val_objectId;
        private System.Windows.Forms.Label lbl_objectId;
        private System.Windows.Forms.Label lbl_star;
        private System.Windows.Forms.NumericUpDown val_area;
        private System.Windows.Forms.Label lbl_area;
        private System.Windows.Forms.ComboBox val_act;
        private System.Windows.Forms.Button btnOpenObjectList;
        private System.Windows.Forms.NumericUpDown val_startDistance;
        private System.Windows.Forms.Label lbl_startDistance;
        private System.Windows.Forms.CheckBox check_displayFog;
        private System.Windows.Forms.NumericUpDown val_endDistance;
        private System.Windows.Forms.Label lbl_endDistance;
        private System.Windows.Forms.Panel box_color;
        private System.Windows.Forms.SplitContainer spcPropertyInterface;
        private System.Windows.Forms.ToolStripButton btnOffsetAllCoords;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
    }
}