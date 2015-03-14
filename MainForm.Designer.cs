namespace SM64DSe
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tsToolBar = new System.Windows.Forms.ToolStrip();
            this.btnOpenROM = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEditLevel = new System.Windows.Forms.ToolStripButton();
            this.btnEditTexts = new System.Windows.Forms.ToolStripButton();
            this.btnKCLEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAnimationEditor = new System.Windows.Forms.ToolStripButton();
            this.btnOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnUpdateODB = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnHalp = new System.Windows.Forms.ToolStripButton();
            this.btnMore = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnitAdditionalPatches = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitDumpAllOvls = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitDecompressOverlaysWithinGame = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitHexDumpToBinaryFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitEditSDATINFOBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbxLevels = new System.Windows.Forms.ListBox();
            this.ofdOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.sfdSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.ssStatusBar = new System.Windows.Forms.StatusStrip();
            this.slStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.spbStatusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tbcMainFormTabControl = new System.Windows.Forms.TabControl();
            this.tbpLevels = new System.Windows.Forms.TabPage();
            this.tbpFileSystem = new System.Windows.Forms.TabPage();
            this.spcFileSystemTab = new System.Windows.Forms.SplitContainer();
            this.tvFileList = new System.Windows.Forms.TreeView();
            this.pnlFileOptions = new System.Windows.Forms.Panel();
            this.btnLZForceCompression = new System.Windows.Forms.Button();
            this.btnLZCompressWithHeader = new System.Windows.Forms.Button();
            this.btnLZForceDecompression = new System.Windows.Forms.Button();
            this.btnLZDecompressWithHeader = new System.Windows.Forms.Button();
            this.btnReplaceImport = new System.Windows.Forms.Button();
            this.btnReplaceRaw = new System.Windows.Forms.Button();
            this.btnExtractExport = new System.Windows.Forms.Button();
            this.btnExtractRaw = new System.Windows.Forms.Button();
            this.tbpARM9Overlays = new System.Windows.Forms.TabPage();
            this.spcARM9Overlays = new System.Windows.Forms.SplitContainer();
            this.tvARM9Overlays = new System.Windows.Forms.TreeView();
            this.btnReplaceOverlay = new System.Windows.Forms.Button();
            this.btnExtractOverlay = new System.Windows.Forms.Button();
            this.btnDecompressOverlay = new System.Windows.Forms.Button();
            this.tsToolBar.SuspendLayout();
            this.ssStatusBar.SuspendLayout();
            this.tbcMainFormTabControl.SuspendLayout();
            this.tbpLevels.SuspendLayout();
            this.tbpFileSystem.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.spcFileSystemTab)).BeginInit();
            this.spcFileSystemTab.Panel1.SuspendLayout();
            this.spcFileSystemTab.Panel2.SuspendLayout();
            this.spcFileSystemTab.SuspendLayout();
            this.pnlFileOptions.SuspendLayout();
            this.tbpARM9Overlays.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.spcARM9Overlays)).BeginInit();
            this.spcARM9Overlays.Panel1.SuspendLayout();
            this.spcARM9Overlays.Panel2.SuspendLayout();
            this.spcARM9Overlays.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsToolBar
            // 
            this.tsToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpenROM,
            this.toolStripSeparator1,
            this.btnEditLevel,
            this.btnEditTexts,
            this.btnKCLEditor,
            this.toolStripSeparator2,
            this.btnAnimationEditor,
            this.btnOptions,
            this.btnHalp,
            this.btnMore});
            this.tsToolBar.Location = new System.Drawing.Point(0, 0);
            this.tsToolBar.Name = "tsToolBar";
            this.tsToolBar.Size = new System.Drawing.Size(508, 25);
            this.tsToolBar.TabIndex = 0;
            this.tsToolBar.Text = "toolStrip1";
            // 
            // btnOpenROM
            // 
            this.btnOpenROM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpenROM.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenROM.Image")));
            this.btnOpenROM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenROM.Name = "btnOpenROM";
            this.btnOpenROM.Size = new System.Drawing.Size(79, 22);
            this.btnOpenROM.Text = "Open ROM...";
            this.btnOpenROM.ToolTipText = "what it says";
            this.btnOpenROM.Click += new System.EventHandler(this.btnOpenROM_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnEditLevel
            // 
            this.btnEditLevel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditLevel.Enabled = false;
            this.btnEditLevel.Image = ((System.Drawing.Image)(resources.GetObject("btnEditLevel.Image")));
            this.btnEditLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditLevel.Name = "btnEditLevel";
            this.btnEditLevel.Size = new System.Drawing.Size(58, 22);
            this.btnEditLevel.Text = "Edit level";
            this.btnEditLevel.ToolTipText = "let the fun begin!";
            this.btnEditLevel.Click += new System.EventHandler(this.btnEditLevel_Click);
            // 
            // btnEditTexts
            // 
            this.btnEditTexts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditTexts.Enabled = false;
            this.btnEditTexts.Image = ((System.Drawing.Image)(resources.GetObject("btnEditTexts.Image")));
            this.btnEditTexts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditTexts.Name = "btnEditTexts";
            this.btnEditTexts.Size = new System.Drawing.Size(58, 22);
            this.btnEditTexts.Text = "Edit texts";
            this.btnEditTexts.ToolTipText = "change what signs say";
            this.btnEditTexts.Click += new System.EventHandler(this.btnEditTexts_Click);
            // 
            // btnKCLEditor
            // 
            this.btnKCLEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnKCLEditor.Enabled = false;
            this.btnKCLEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnKCLEditor.Name = "btnKCLEditor";
            this.btnKCLEditor.Size = new System.Drawing.Size(55, 22);
            this.btnKCLEditor.Text = "Edit KCL";
            this.btnKCLEditor.Click += new System.EventHandler(this.btnKCLEditor_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAnimationEditor
            // 
            this.btnAnimationEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAnimationEditor.Enabled = false;
            this.btnAnimationEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAnimationEditor.Name = "btnAnimationEditor";
            this.btnAnimationEditor.Size = new System.Drawing.Size(101, 22);
            this.btnAnimationEditor.Text = "Animation Editor";
            this.btnAnimationEditor.ToolTipText = "Animation Editor";
            this.btnAnimationEditor.Click += new System.EventHandler(this.btnAnimationEditor_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUpdateODB,
            this.btnEditorSettings});
            this.btnOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.Image")));
            this.btnOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(62, 22);
            this.btnOptions.Text = "Options";
            this.btnOptions.ToolTipText = "configure the editor";
            // 
            // btnUpdateODB
            // 
            this.btnUpdateODB.Name = "btnUpdateODB";
            this.btnUpdateODB.Size = new System.Drawing.Size(198, 22);
            this.btnUpdateODB.Text = "Update object database";
            this.btnUpdateODB.Click += new System.EventHandler(this.btnUpdateODB_Click);
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(198, 22);
            this.btnEditorSettings.Text = "Editor settings";
            this.btnEditorSettings.Click += new System.EventHandler(this.btnEditorSettings_Click);
            // 
            // btnHalp
            // 
            this.btnHalp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnHalp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHalp.Image = ((System.Drawing.Image)(resources.GetObject("btnHalp.Image")));
            this.btnHalp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHalp.Name = "btnHalp";
            this.btnHalp.Size = new System.Drawing.Size(23, 22);
            this.btnHalp.Text = "?";
            this.btnHalp.ToolTipText = "Help, about, etc...";
            this.btnHalp.Click += new System.EventHandler(this.btnHalp_Click);
            // 
            // btnMore
            // 
            this.btnMore.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnMore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMore.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitAdditionalPatches,
            this.mnitDumpAllOvls,
            this.mnitDecompressOverlaysWithinGame,
            this.mnitHexDumpToBinaryFile,
            this.mnitEditSDATINFOBlockToolStripMenuItem,
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem});
            this.btnMore.Enabled = false;
            this.btnMore.Image = ((System.Drawing.Image)(resources.GetObject("btnMore.Image")));
            this.btnMore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(48, 19);
            this.btnMore.Text = "More";
            this.btnMore.ToolTipText = "Debug crap. Don\'t touch unless you know what you\'re doing.";
            // 
            // mnitAdditionalPatches
            // 
            this.mnitAdditionalPatches.Name = "mnitAdditionalPatches";
            this.mnitAdditionalPatches.Size = new System.Drawing.Size(294, 22);
            this.mnitAdditionalPatches.Text = "Additional Patches";
            this.mnitAdditionalPatches.Click += new System.EventHandler(this.mnitAdditionalPatches_Click);
            // 
            // mnitDumpAllOvls
            // 
            this.mnitDumpAllOvls.Name = "mnitDumpAllOvls";
            this.mnitDumpAllOvls.Size = new System.Drawing.Size(294, 22);
            this.mnitDumpAllOvls.Text = "Dump All Overlays";
            this.mnitDumpAllOvls.Click += new System.EventHandler(this.mnitDumpAllOvls_Click);
            // 
            // mnitDecompressOverlaysWithinGame
            // 
            this.mnitDecompressOverlaysWithinGame.Name = "mnitDecompressOverlaysWithinGame";
            this.mnitDecompressOverlaysWithinGame.Size = new System.Drawing.Size(294, 22);
            this.mnitDecompressOverlaysWithinGame.Text = "Decompress Overlays Within Game";
            this.mnitDecompressOverlaysWithinGame.Click += new System.EventHandler(this.mnitDecompressOverlaysWithinGame_Click);
            // 
            // mnitHexDumpToBinaryFile
            // 
            this.mnitHexDumpToBinaryFile.Name = "mnitHexDumpToBinaryFile";
            this.mnitHexDumpToBinaryFile.Size = new System.Drawing.Size(294, 22);
            this.mnitHexDumpToBinaryFile.Text = "Hex Dump to Binary File";
            this.mnitHexDumpToBinaryFile.Click += new System.EventHandler(this.mnitHexDumpToBinaryFile_Click);
            // 
            // mnitEditSDATINFOBlockToolStripMenuItem
            // 
            this.mnitEditSDATINFOBlockToolStripMenuItem.Enabled = false;
            this.mnitEditSDATINFOBlockToolStripMenuItem.Name = "mnitEditSDATINFOBlockToolStripMenuItem";
            this.mnitEditSDATINFOBlockToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.mnitEditSDATINFOBlockToolStripMenuItem.Text = "Edit SDAT INFO Block";
            this.mnitEditSDATINFOBlockToolStripMenuItem.Visible = false;
            this.mnitEditSDATINFOBlockToolStripMenuItem.Click += new System.EventHandler(this.mnitEditSDATINFOBlockToolStripMenuItem_Click);
            // 
            // mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem
            // 
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Name = "mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem";
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Text = "Toggle Suitability for NSMBe ASM Patching";
            this.mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem.Click += new System.EventHandler(mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem_Click);
            // 
            // lbxLevels
            // 
            this.lbxLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxLevels.FormattingEnabled = true;
            this.lbxLevels.IntegralHeight = false;
            this.lbxLevels.Location = new System.Drawing.Point(3, 3);
            this.lbxLevels.Margin = new System.Windows.Forms.Padding(0);
            this.lbxLevels.Name = "lbxLevels";
            this.lbxLevels.Size = new System.Drawing.Size(494, 356);
            this.lbxLevels.TabIndex = 1;
            this.lbxLevels.SelectedIndexChanged += new System.EventHandler(this.lbxLevels_SelectedIndexChanged);
            this.lbxLevels.DoubleClick += new System.EventHandler(this.lbxLevels_DoubleClick);
            // 
            // ofdOpenFile
            // 
            this.ofdOpenFile.DefaultExt = "nds";
            this.ofdOpenFile.Filter = "Nintendo DS ROM|*.nds|Any file|*.*";
            this.ofdOpenFile.Title = "Open ROM...";
            // 
            // sfdSaveFile
            // 
            this.sfdSaveFile.DefaultExt = "nds";
            this.sfdSaveFile.Filter = "Nintendo DS ROM|*.nds|Any file|*.*";
            this.sfdSaveFile.RestoreDirectory = true;
            this.sfdSaveFile.Title = "Backup ROM...";
            // 
            // ssStatusBar
            // 
            this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slStatusLabel,
            this.spbStatusProgress});
            this.ssStatusBar.Location = new System.Drawing.Point(0, 413);
            this.ssStatusBar.Name = "ssStatusBar";
            this.ssStatusBar.Size = new System.Drawing.Size(508, 22);
            this.ssStatusBar.TabIndex = 2;
            this.ssStatusBar.Text = "statusStrip1";
            // 
            // slStatusLabel
            // 
            this.slStatusLabel.Name = "slStatusLabel";
            this.slStatusLabel.Size = new System.Drawing.Size(51, 17);
            this.slStatusLabel.Text = "lolstatus";
            // 
            // spbStatusProgress
            // 
            this.spbStatusProgress.Name = "spbStatusProgress";
            this.spbStatusProgress.Size = new System.Drawing.Size(150, 16);
            this.spbStatusProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.spbStatusProgress.Visible = false;
            // 
            // tbcMainFormTabControl
            // 
            this.tbcMainFormTabControl.Controls.Add(this.tbpLevels);
            this.tbcMainFormTabControl.Controls.Add(this.tbpFileSystem);
            this.tbcMainFormTabControl.Controls.Add(this.tbpARM9Overlays);
            this.tbcMainFormTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMainFormTabControl.Location = new System.Drawing.Point(0, 25);
            this.tbcMainFormTabControl.Name = "tbcMainFormTabControl";
            this.tbcMainFormTabControl.SelectedIndex = 0;
            this.tbcMainFormTabControl.Size = new System.Drawing.Size(508, 388);
            this.tbcMainFormTabControl.TabIndex = 3;
            // 
            // tbpLevels
            // 
            this.tbpLevels.Controls.Add(this.lbxLevels);
            this.tbpLevels.Location = new System.Drawing.Point(4, 22);
            this.tbpLevels.Name = "tbpLevels";
            this.tbpLevels.Padding = new System.Windows.Forms.Padding(3);
            this.tbpLevels.Size = new System.Drawing.Size(500, 362);
            this.tbpLevels.TabIndex = 0;
            this.tbpLevels.Text = "Levels";
            this.tbpLevels.UseVisualStyleBackColor = true;
            // 
            // tbpFileSystem
            // 
            this.tbpFileSystem.Controls.Add(this.spcFileSystemTab);
            this.tbpFileSystem.Location = new System.Drawing.Point(4, 22);
            this.tbpFileSystem.Name = "tbpFileSystem";
            this.tbpFileSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tbpFileSystem.Size = new System.Drawing.Size(500, 362);
            this.tbpFileSystem.TabIndex = 1;
            this.tbpFileSystem.Text = "SM64DS File System";
            this.tbpFileSystem.UseVisualStyleBackColor = true;
            // 
            // spcFileSystemTab
            // 
            this.spcFileSystemTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcFileSystemTab.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcFileSystemTab.IsSplitterFixed = true;
            this.spcFileSystemTab.Location = new System.Drawing.Point(3, 3);
            this.spcFileSystemTab.Name = "spcFileSystemTab";
            this.spcFileSystemTab.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcFileSystemTab.Panel1
            // 
            this.spcFileSystemTab.Panel1.Controls.Add(this.tvFileList);
            // 
            // spcFileSystemTab.Panel2
            // 
            this.spcFileSystemTab.Panel2.Controls.Add(this.pnlFileOptions);
            this.spcFileSystemTab.Size = new System.Drawing.Size(494, 356);
            this.spcFileSystemTab.SplitterDistance = 275;
            this.spcFileSystemTab.TabIndex = 2;
            // 
            // tvFileList
            // 
            this.tvFileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFileList.Location = new System.Drawing.Point(0, 0);
            this.tvFileList.Name = "tvFileList";
            this.tvFileList.Size = new System.Drawing.Size(494, 275);
            this.tvFileList.TabIndex = 0;
            this.tvFileList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFileList_AfterSelect);
            // 
            // pnlFileOptions
            // 
            this.pnlFileOptions.Controls.Add(this.btnLZForceCompression);
            this.pnlFileOptions.Controls.Add(this.btnLZCompressWithHeader);
            this.pnlFileOptions.Controls.Add(this.btnLZForceDecompression);
            this.pnlFileOptions.Controls.Add(this.btnLZDecompressWithHeader);
            this.pnlFileOptions.Controls.Add(this.btnReplaceImport);
            this.pnlFileOptions.Controls.Add(this.btnReplaceRaw);
            this.pnlFileOptions.Controls.Add(this.btnExtractExport);
            this.pnlFileOptions.Controls.Add(this.btnExtractRaw);
            this.pnlFileOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFileOptions.Location = new System.Drawing.Point(0, 0);
            this.pnlFileOptions.Name = "pnlFileOptions";
            this.pnlFileOptions.Size = new System.Drawing.Size(494, 77);
            this.pnlFileOptions.TabIndex = 1;
            // 
            // btnLZForceCompression
            // 
            this.btnLZForceCompression.Location = new System.Drawing.Point(357, 26);
            this.btnLZForceCompression.Name = "btnLZForceCompression";
            this.btnLZForceCompression.Size = new System.Drawing.Size(134, 23);
            this.btnLZForceCompression.TabIndex = 7;
            this.btnLZForceCompression.Text = "LZ Force Compression";
            this.btnLZForceCompression.UseVisualStyleBackColor = true;
            this.btnLZForceCompression.Click += new System.EventHandler(this.btnLZForceCompression_Click);
            // 
            // btnLZCompressWithHeader
            // 
            this.btnLZCompressWithHeader.Location = new System.Drawing.Point(357, 2);
            this.btnLZCompressWithHeader.Name = "btnLZCompressWithHeader";
            this.btnLZCompressWithHeader.Size = new System.Drawing.Size(134, 23);
            this.btnLZCompressWithHeader.TabIndex = 6;
            this.btnLZCompressWithHeader.Text = "LZ Compress with Hdr.";
            this.btnLZCompressWithHeader.UseVisualStyleBackColor = true;
            this.btnLZCompressWithHeader.Click += new System.EventHandler(this.btnLZCompressWithHeader_Click);
            // 
            // btnLZForceDecompression
            // 
            this.btnLZForceDecompression.Location = new System.Drawing.Point(217, 26);
            this.btnLZForceDecompression.Name = "btnLZForceDecompression";
            this.btnLZForceDecompression.Size = new System.Drawing.Size(137, 23);
            this.btnLZForceDecompression.TabIndex = 5;
            this.btnLZForceDecompression.Text = "LZ Force Decompression";
            this.btnLZForceDecompression.UseVisualStyleBackColor = true;
            this.btnLZForceDecompression.Click += new System.EventHandler(this.btnLZForceDecompression_Click);
            // 
            // btnLZDecompressWithHeader
            // 
            this.btnLZDecompressWithHeader.Location = new System.Drawing.Point(217, 2);
            this.btnLZDecompressWithHeader.Name = "btnLZDecompressWithHeader";
            this.btnLZDecompressWithHeader.Size = new System.Drawing.Size(137, 23);
            this.btnLZDecompressWithHeader.TabIndex = 4;
            this.btnLZDecompressWithHeader.Text = "LZ Decompress with Hdr.";
            this.btnLZDecompressWithHeader.UseVisualStyleBackColor = true;
            this.btnLZDecompressWithHeader.Click += new System.EventHandler(this.btnLZDecompressWithHeader_Click);
            // 
            // btnReplaceImport
            // 
            this.btnReplaceImport.Enabled = false;
            this.btnReplaceImport.Location = new System.Drawing.Point(110, 26);
            this.btnReplaceImport.Name = "btnReplaceImport";
            this.btnReplaceImport.Size = new System.Drawing.Size(101, 23);
            this.btnReplaceImport.TabIndex = 3;
            this.btnReplaceImport.Text = "Replace (Import)";
            this.btnReplaceImport.UseVisualStyleBackColor = true;
            this.btnReplaceImport.Visible = false;
            // 
            // btnReplaceRaw
            // 
            this.btnReplaceRaw.Location = new System.Drawing.Point(110, 2);
            this.btnReplaceRaw.Name = "btnReplaceRaw";
            this.btnReplaceRaw.Size = new System.Drawing.Size(101, 23);
            this.btnReplaceRaw.TabIndex = 2;
            this.btnReplaceRaw.Text = "Replace (Raw)";
            this.btnReplaceRaw.UseVisualStyleBackColor = true;
            this.btnReplaceRaw.Click += new System.EventHandler(this.btnReplaceRaw_Click);
            // 
            // btnExtractExport
            // 
            this.btnExtractExport.Enabled = false;
            this.btnExtractExport.Location = new System.Drawing.Point(3, 26);
            this.btnExtractExport.Name = "btnExtractExport";
            this.btnExtractExport.Size = new System.Drawing.Size(101, 23);
            this.btnExtractExport.TabIndex = 1;
            this.btnExtractExport.Text = "Extract (Export)";
            this.btnExtractExport.UseVisualStyleBackColor = true;
            this.btnExtractExport.Visible = false;
            // 
            // btnExtractRaw
            // 
            this.btnExtractRaw.Location = new System.Drawing.Point(3, 2);
            this.btnExtractRaw.Name = "btnExtractRaw";
            this.btnExtractRaw.Size = new System.Drawing.Size(101, 23);
            this.btnExtractRaw.TabIndex = 0;
            this.btnExtractRaw.Text = "Extract (Raw)";
            this.btnExtractRaw.UseVisualStyleBackColor = true;
            this.btnExtractRaw.Click += new System.EventHandler(this.btnExtractRaw_Click);
            // 
            // tbpARM9Overlays
            // 
            this.tbpARM9Overlays.Controls.Add(this.spcARM9Overlays);
            this.tbpARM9Overlays.Location = new System.Drawing.Point(4, 22);
            this.tbpARM9Overlays.Name = "tbpARM9Overlays";
            this.tbpARM9Overlays.Padding = new System.Windows.Forms.Padding(3);
            this.tbpARM9Overlays.Size = new System.Drawing.Size(500, 362);
            this.tbpARM9Overlays.TabIndex = 2;
            this.tbpARM9Overlays.Text = "ARM 9 Overlays";
            this.tbpARM9Overlays.UseVisualStyleBackColor = true;
            // 
            // spcARM9Overlays
            // 
            this.spcARM9Overlays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcARM9Overlays.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.spcARM9Overlays.IsSplitterFixed = true;
            this.spcARM9Overlays.Location = new System.Drawing.Point(3, 3);
            this.spcARM9Overlays.Name = "spcARM9Overlays";
            this.spcARM9Overlays.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcARM9Overlays.Panel1
            // 
            this.spcARM9Overlays.Panel1.Controls.Add(this.tvARM9Overlays);
            // 
            // spcARM9Overlays.Panel2
            // 
            this.spcARM9Overlays.Panel2.Controls.Add(this.btnReplaceOverlay);
            this.spcARM9Overlays.Panel2.Controls.Add(this.btnExtractOverlay);
            this.spcARM9Overlays.Panel2.Controls.Add(this.btnDecompressOverlay);
            this.spcARM9Overlays.Size = new System.Drawing.Size(494, 356);
            this.spcARM9Overlays.SplitterDistance = 275;
            this.spcARM9Overlays.TabIndex = 0;
            // 
            // tvARM9Overlays
            // 
            this.tvARM9Overlays.Location = new System.Drawing.Point(0, 0);
            this.tvARM9Overlays.Name = "tvARM9Overlays";
            this.tvARM9Overlays.Size = new System.Drawing.Size(494, 275);
            this.tvARM9Overlays.TabIndex = 0;
            this.tvARM9Overlays.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvARM9Overlays_AfterSelect);
            // 
            // btnReplaceOverlay
            // 
            this.btnReplaceOverlay.Location = new System.Drawing.Point(122, 3);
            this.btnReplaceOverlay.Name = "btnReplaceOverlay";
            this.btnReplaceOverlay.Size = new System.Drawing.Size(113, 23);
            this.btnReplaceOverlay.TabIndex = 11;
            this.btnReplaceOverlay.Text = "Replace Overlay";
            this.btnReplaceOverlay.UseVisualStyleBackColor = true;
            this.btnReplaceOverlay.Click += new System.EventHandler(this.btnReplaceOverlay_Click);
            // 
            // btnExtractOverlay
            // 
            this.btnExtractOverlay.Location = new System.Drawing.Point(3, 2);
            this.btnExtractOverlay.Name = "btnExtractOverlay";
            this.btnExtractOverlay.Size = new System.Drawing.Size(113, 23);
            this.btnExtractOverlay.TabIndex = 10;
            this.btnExtractOverlay.Text = "Extract Overlay";
            this.btnExtractOverlay.UseVisualStyleBackColor = true;
            this.btnExtractOverlay.Click += new System.EventHandler(this.btnExtractOverlay_Click);
            // 
            // btnDecompressOverlay
            // 
            this.btnDecompressOverlay.Location = new System.Drawing.Point(241, 3);
            this.btnDecompressOverlay.Name = "btnDecompressOverlay";
            this.btnDecompressOverlay.Size = new System.Drawing.Size(137, 23);
            this.btnDecompressOverlay.TabIndex = 9;
            this.btnDecompressOverlay.Text = "Decompress Overlay";
            this.btnDecompressOverlay.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 435);
            this.Controls.Add(this.tbcMainFormTabControl);
            this.Controls.Add(this.ssStatusBar);
            this.Controls.Add(this.tsToolBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "SM64DS Editor vlol";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tsToolBar.ResumeLayout(false);
            this.tsToolBar.PerformLayout();
            this.ssStatusBar.ResumeLayout(false);
            this.ssStatusBar.PerformLayout();
            this.tbcMainFormTabControl.ResumeLayout(false);
            this.tbpLevels.ResumeLayout(false);
            this.tbpFileSystem.ResumeLayout(false);
            this.spcFileSystemTab.Panel1.ResumeLayout(false);
            this.spcFileSystemTab.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.spcFileSystemTab)).EndInit();
            this.spcFileSystemTab.ResumeLayout(false);
            this.pnlFileOptions.ResumeLayout(false);
            this.tbpARM9Overlays.ResumeLayout(false);
            this.spcARM9Overlays.Panel1.ResumeLayout(false);
            this.spcARM9Overlays.Panel2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.spcARM9Overlays)).EndInit();
            this.spcARM9Overlays.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsToolBar;
        private System.Windows.Forms.ListBox lbxLevels;
        private System.Windows.Forms.ToolStripButton btnOpenROM;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnEditLevel;
        private System.Windows.Forms.ToolStripButton btnEditTexts;
        private System.Windows.Forms.OpenFileDialog ofdOpenFile;
        private System.Windows.Forms.SaveFileDialog sfdSaveFile;
        private System.Windows.Forms.ToolStripDropDownButton btnMore;
        private System.Windows.Forms.StatusStrip ssStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel slStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar spbStatusProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnOptions;
        private System.Windows.Forms.ToolStripMenuItem btnUpdateODB;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripButton btnHalp;
        private System.Windows.Forms.ToolStripMenuItem mnitDumpAllOvls;
        private System.Windows.Forms.ToolStripButton btnKCLEditor;
        private System.Windows.Forms.ToolStripButton btnAnimationEditor;
        private System.Windows.Forms.ToolStripMenuItem mnitDecompressOverlaysWithinGame;
        private System.Windows.Forms.ToolStripMenuItem mnitHexDumpToBinaryFile;
        private System.Windows.Forms.ToolStripMenuItem mnitAdditionalPatches;
        private System.Windows.Forms.TabControl tbcMainFormTabControl;
        private System.Windows.Forms.TabPage tbpLevels;
        private System.Windows.Forms.TabPage tbpFileSystem;
        private System.Windows.Forms.Panel pnlFileOptions;
        private System.Windows.Forms.TreeView tvFileList;
        private System.Windows.Forms.Button btnReplaceImport;
        private System.Windows.Forms.Button btnReplaceRaw;
        private System.Windows.Forms.Button btnExtractExport;
        private System.Windows.Forms.Button btnExtractRaw;
        private System.Windows.Forms.Button btnLZForceCompression;
        private System.Windows.Forms.Button btnLZCompressWithHeader;
        private System.Windows.Forms.Button btnLZForceDecompression;
        private System.Windows.Forms.Button btnLZDecompressWithHeader;
        private System.Windows.Forms.SplitContainer spcFileSystemTab;
        private System.Windows.Forms.ToolStripMenuItem mnitEditSDATINFOBlockToolStripMenuItem;
        private System.Windows.Forms.TabPage tbpARM9Overlays;
        private System.Windows.Forms.SplitContainer spcARM9Overlays;
        private System.Windows.Forms.Button btnReplaceOverlay;
        private System.Windows.Forms.Button btnExtractOverlay;
        private System.Windows.Forms.Button btnDecompressOverlay;
        private System.Windows.Forms.TreeView tvARM9Overlays;
        private System.Windows.Forms.ToolStripMenuItem mnitToggleSuitabilityForNSMBeASMPatchingToolStripMenuItem;
    }
}

