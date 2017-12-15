using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SM64DSe.ImportExport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SM64DSe.ImportExport.Loaders.InternalLoaders;
using SM64DSe.SM64DSFormats;
using System.Text.RegularExpressions;
using SM64DSe.ImportExport.Writers.InternalWriters;
using System.Drawing.Imaging;
using System.IO;

namespace SM64DSe
{
    public partial class ModelAndCollisionMapEditor : Form
    {
        public enum StartMode
        {
            ModelAndCollisionMap,
            CollisionMap
        };

        protected enum ModelSourceType
        {
            External,
            Internal, 
            None
        };

        protected enum CopyMode
        {
            Copy, 
            Cut, 
            None
        };

        protected enum CopySourceType
        {
            Geometry, 
            PolyList, 
            None
        };

        protected class ModelImportationSettings
        {
            public float m_Scale;
            public float m_InGamePreviewScale;
            public BMDImporter.BMDExtraImportOptions m_ExtraOptions;

            public ModelImportationSettings(float scale, float gameScale, BMDImporter.BMDExtraImportOptions extraOptions)
            {
                m_Scale = scale;
                m_InGamePreviewScale = gameScale;
                m_ExtraOptions = extraOptions;
            }

            public ModelImportationSettings(float gameScale)
                : this(1f, gameScale, BMDImporter.BMDExtraImportOptions.DEFAULT) { }

            public ModelImportationSettings()
                : this(1f) { }

            public float GetImportationScale()
            {
                return m_Scale;
            }

            public float GetPreviewScale()
            {
                return m_Scale * m_InGamePreviewScale;
            }
        }

        protected class CollisionMapImportationSettings
        {
            public float m_Scale;
            public float m_InGameModelScale;
            public float m_MinimumFaceSize;

            public CollisionMapImportationSettings(float scale, float gameScale, float minimumFaceSize)
            {
                m_Scale = scale;
                m_InGameModelScale = gameScale;
                m_MinimumFaceSize = minimumFaceSize;
            }

            public CollisionMapImportationSettings(float gameScale)
                : this(1f, gameScale, 0f) { }

            public CollisionMapImportationSettings()
                : this(1f) { }

            public float GetImportationScale()
            {
                return m_Scale * m_InGameModelScale;
            }

            public float GetPreviewScale()
            {
                return GetImportationScale();
            }
        }

        private static readonly string TITLE_MODEL_AND_COLLISION_MAP_IMPORTER = "Model and Collision Map Importer";
        private static readonly string TITLE_COLLISION_MAP_EDITOR = "Collision Map Editor";

        private StartMode m_StartMode;

        private ModelBase m_ModelBase;

        private SortedDictionary<string, ModelBase.TextureDefNitro> m_WorkingTexturesCopy;
        private SortedDictionary<string, byte[]> m_WorkingPalettesCopy;

        private ModelSourceType m_ModelSourceType;
        private string m_ModelSourceName;
        private bool m_ModelSourceLoaded;

        private string m_BMDTargetName;
        private string m_KCLTargetName;

        private BMD m_ImportedModel;
        private KCL m_ImportedCollisionMap;

        private ModelImportationSettings m_ModelImportationSettings;
        private CollisionMapImportationSettings m_CollisionMapImportationSettings;

        private float m_ModelPreviewScale;
        private float m_CollisionMapPreviewScale;

        private int[] m_BMDDisplayLists;
        private int[] m_KCLPickingDisplayLists;
        private int[] m_KCLMeshDisplayLists;

        private Dictionary<string, int> m_CollisionMapMaterialTypeMap;

        private CopyMode m_CopyMode;
        private CopySourceType m_CopySourceType;
        private ModelBase.BoneDef m_SourceBone;
        private List<ModelBase.GeometryDef> m_SourceGeometries;
        private List<ModelBase.PolyListDef> m_SourcePolylists;
        private ModelBase.GeometryDef m_TargetGeometry;
        private ModelBase.BoneDef m_TargetBone;

        private bool m_CollisionMapWireFrameView;
        private List<KCL.ColFace> m_CollisionMapPlanes;
        private KCLLoader.CollisionMapColours m_CollisionMapColours;
        private int m_CollisionMapSelectedTriangle;

        private OpenFileDialog m_OpenFileDialogue;
        private ROMFileSelect m_ROMFileSelect;
        private FolderBrowserDialog m_FolderBrowserDialogue;
        private SaveFileDialog m_SaveFileDialogue;

        private static readonly Regex ASCII_PRINTABLE_NO_SPACE = new Regex("[^!-~]+");

        private static readonly byte[] DUMMY_BMD_DATA;
        private static readonly NitroFile DUMMY_BMD_NITRO_FILE;
        private static readonly BMD DUMMY_BMD;
        private static readonly byte[] DUMMY_KCL_DATA;
        private static readonly NitroFile DUMMY_KCL_NITRO_FILE;
        private static readonly KCL DUMMY_KCL;
        static ModelAndCollisionMapEditor()
        {
            DUMMY_BMD_DATA = new byte[0x30];
            DUMMY_BMD_NITRO_FILE = new NitroFile();
            DUMMY_BMD_NITRO_FILE.m_ID = 0xFFFF;
            DUMMY_BMD_NITRO_FILE.m_Name = "DUMMY_BMD";
            DUMMY_BMD_NITRO_FILE.m_Data = DUMMY_BMD_DATA;
            DUMMY_BMD = new BMD(DUMMY_BMD_NITRO_FILE);

            DUMMY_KCL_DATA = new byte[0x30];
            DUMMY_KCL_NITRO_FILE = new NitroFile();
            DUMMY_KCL_NITRO_FILE.m_ID = 0xFFFF;
            DUMMY_KCL_NITRO_FILE.m_Name = "DUMMY_KCL";
            DUMMY_KCL_NITRO_FILE.m_Data = DUMMY_KCL_DATA;
            DUMMY_KCL = new KCL(DUMMY_KCL_NITRO_FILE);
        }

        public ModelAndCollisionMapEditor(
            string bmdModelTargetName, 
            string kclTargetName, 
            float gameScale, 
            StartMode startMode
            )
        {
            m_BMDTargetName = bmdModelTargetName;
            m_KCLTargetName = kclTargetName;

            m_BMDDisplayLists = new int[3]; // Standard, Geometry Highlighting, Skeleton
            m_KCLMeshDisplayLists = new int[3]; // Fill, WireFrame, Highlight
            m_KCLPickingDisplayLists = new int[1];

            m_ModelImportationSettings = new ModelImportationSettings(gameScale);
            m_CollisionMapImportationSettings = new CollisionMapImportationSettings(gameScale);

            m_ModelPreviewScale = m_ModelImportationSettings.GetPreviewScale();
            m_CollisionMapPreviewScale = m_CollisionMapImportationSettings.GetPreviewScale();

            m_StartMode = startMode;

            m_ModelSourceType = ModelSourceType.None;
            m_ModelSourceLoaded = false;

            InitialiseForm();
        }

        public ModelAndCollisionMapEditor(
            string bmdModelTargetName,
            string kclTargetName,
            float gameScale
            )
            : this(bmdModelTargetName, kclTargetName, gameScale, StartMode.ModelAndCollisionMap) { }

        public ModelAndCollisionMapEditor(
            string bmdModelTargetName,
            string kclTargetName
            )
            : this(bmdModelTargetName, kclTargetName, 1f) { }

        public ModelAndCollisionMapEditor(
            StartMode startMode
            )
            : this(null, null, 1f, startMode) { }

        public ModelAndCollisionMapEditor()
            : this(null, null) { }

        private void InitialiseForm()
        {
            InitializeComponent();

            switch (m_StartMode)
            {
                default:
                case StartMode.ModelAndCollisionMap:
                    Text = TITLE_MODEL_AND_COLLISION_MAP_IMPORTER;
                    break;
                case StartMode.CollisionMap:
                    Text = TITLE_COLLISION_MAP_EDITOR;
                    break;
            }

            UpdateEnabledStateMenuControls();

            if (m_StartMode == StartMode.ModelAndCollisionMap)
            {
                // Model General Settings
                txtModelGeneralTargetName.Text = m_BMDTargetName;

                chkModelGeneralStripify.Checked = m_ModelImportationSettings.m_ExtraOptions.m_ConvertToTriangleStrips;
                chkModelGeneralKeepVertexOrderDuringStripping.Checked = m_ModelImportationSettings.m_ExtraOptions.m_KeepVertexOrderDuringStripping;
                chkModelGeneralAlwaysWriteFullVertexCmd23h.Checked = m_ModelImportationSettings.m_ExtraOptions.m_AlwaysWriteFullVertexCmd23h;
                switch (m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting)
                {
                    case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.SmallestSize:
                        rbModelGeneralTextureAlwaysCompress.Checked = true;
                        break;
                    case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BetterQualityWhereSensible:
                        rbModelGeneralTextureBetterQualityWhereSensible.Checked = true;
                        break;
                    case BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BestQuality:
                        rbModelGeneralTextureNeverCompress.Checked = true;
                        break;
                }
                chkModelGeneralVFlipAllTextures.Checked = m_ModelImportationSettings.m_ExtraOptions.m_VerticallyFlipAllTextures;

                txtModelGeneralScale.Text = Helper.ToString4DP(m_ModelImportationSettings.m_Scale);
                txtModelPreviewScale.Text = Helper.ToString4DP(m_ModelImportationSettings.m_InGamePreviewScale);

                // Model Bones
                SetEnabledStateModelBoneBaseControls(false);
                SetEnabledStateModelBoneSpecificControls(false);
                SetEnabledStateModelBoneGeometryControls(false);
                SetEnabledStateModelBonePolylistControls(false);
                ResetGeometryDuplicationAndMovementState();

                // Model Materials
                SetEnabledStateModelMaterialControls(false);

                // Model Textures
                SetEnabledStateModelTextureControls(false);

                // Model Palettes
                SetEnabledStateModelPaletteControls(false);

                // Model GL
                glModelView.Initialise();
                glModelView.ProvideDisplayLists(m_BMDDisplayLists);
                glModelView.ProvideScaleRef(ref m_ModelPreviewScale);
            }
            else
            {
                tcMain.TabPages.Remove(tpgModel);
                mnitImport.DropDownItems.Remove(mnitImportModelAndCollisionMap);
                mnitImport.DropDownItems.Remove(mnitImportModelOnly);
                mnitExport.DropDownItems.Remove(mnitExportModel);
                mnitExport.DropDownItems.Remove(mnitExportTextures);
            }

            // Collision Map General Settings
            txtCollisionMapGeneralTargetName.Text = m_KCLTargetName;

            txtCollisionMapGeneralFaceSizeThreshold.Text = Helper.ToString4DP(m_CollisionMapImportationSettings.m_MinimumFaceSize);
            txtCollisionMapGeneralScale.Text = Helper.ToString4DP(m_CollisionMapImportationSettings.m_Scale);
            txtCollisionMapGeneralTargetScale.Text = Helper.ToString4DP(m_CollisionMapImportationSettings.m_InGameModelScale);

            // Collision Map Material Collision Types
            SetEnabledStateCollisionMapMaterialCollisionTypes(false);
            SetEnabledStateCollisionMapPlanes(false);

            // Collision Map GL
            PopulateCollisionMapPreviewFillModeOptions();

            glCollisionMapView.Initialise();
            glCollisionMapView.ProvideDisplayLists(m_KCLMeshDisplayLists);
            glCollisionMapView.ProvidePickingDisplayLists(m_KCLPickingDisplayLists);
            glCollisionMapView.ProvideCallListForDisplayLists(CallListForKCLDisplayLists);

            // File selection dialogues
            m_OpenFileDialogue = new OpenFileDialog();
            m_ROMFileSelect = new ROMFileSelect();
            m_FolderBrowserDialogue = new FolderBrowserDialog();
            m_FolderBrowserDialogue.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            m_SaveFileDialogue = new SaveFileDialog();
        }

        private void ModelAndCollisionMapEditor_Load(object sender, System.EventArgs e)
        {
            if (m_StartMode == StartMode.CollisionMap && IsKCLTargetSet())
            {
                m_ModelSourceName = m_KCLTargetName;
                m_ModelSourceType = ModelSourceType.Internal;

                LoadCollisionMap();
            }
        }

        private void ModelAndCollisionMapEditor_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (!m_ModelSourceLoaded) return;

            glModelView.PrepareForClose();
            glCollisionMapView.PrepareForClose();

            if (m_ImportedModel != null) m_ImportedModel.Release();
        }

        private bool IsBMDTargetSet()
        {
            return (m_BMDTargetName != null && m_BMDTargetName.Trim().Length > 0);
        }

        private bool IsKCLTargetSet()
        {
            return (m_KCLTargetName != null && m_KCLTargetName.Trim().Length > 0);
        }

        private void LoadModel()
        {
            m_ModelBase = null;
            if (!ModelSourceType.None.Equals(m_ModelSourceType))
            {
                try
                {
                    m_ModelBase = BMDImporter.LoadModel(m_ModelSourceName);
                }
                catch (Exception e)
                {
                    new ExceptionMessageBox("Error loading model", e).ShowDialog();
                    return;
                }
            }

            DeleteDisplayLists();

            // BMD
            m_ImportedModel = DUMMY_BMD;

            m_WorkingTexturesCopy = new SortedDictionary<string, ModelBase.TextureDefNitro>();
            m_WorkingPalettesCopy = new SortedDictionary<string, byte[]>();

            foreach (KeyValuePair<string, ModelBase.TextureDefBase> textureEntry in m_ModelBase.m_Textures)
            {
                NitroTexture nitroTexture = BMDWriter.ConvertTexture(0, 0, textureEntry.Value,
                    m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting, false);
                ModelBase.TextureDefNitro textureDefNitroTexture = new ModelBase.TextureDefNitro(nitroTexture);

                m_WorkingTexturesCopy.Add(textureDefNitroTexture.GetTexName(), textureDefNitroTexture);

                if (textureDefNitroTexture.HasNitroPalette())
                {
                    byte[] palette = textureDefNitroTexture.GetNitroPalette();
                    byte[] paletteCopy = new byte[palette.Length];
                    Array.Copy(palette, paletteCopy, palette.Length);
                    m_WorkingPalettesCopy.Add(textureDefNitroTexture.GetPalName(), paletteCopy);
                }

                foreach (ModelBase.MaterialDef material in m_ModelBase.m_Materials.Values)
                {
                    if (textureEntry.Key.Equals(material.m_TextureDefID))
                    {
                        material.m_TextureDefID = textureDefNitroTexture.GetTexName();
                    }
                }
            }

            m_ModelBase.m_Textures.Clear();

            txtModelGeneralScale.BackColor = Color.White;
            txtModelPreviewScale.BackColor = Color.White;

            PopulateBoneTree();
            PopulateMaterialsList();
            PopulateTextureAndPaletteLists();

            ResetGeometryDuplicationAndMovementState();

            m_ModelSourceLoaded = true;

            UpdateBMDModelAndPreview();

            // KCL
            ResetCollisionMapState();

            string extension = Path.GetExtension(m_ModelSourceName);
            if (extension != null && extension.ToLower().Equals(".kcl"))
            {
                m_ImportedCollisionMap = new KCL(Program.m_ROM.GetFileFromName(m_ModelSourceName));

                UpdateKCLMapAndPreview(false);
            }
            else
            {
                UpdateKCLMapAndPreview(1f);
            }

            // General
            UpdateEnabledStateMenuControls();

            lblMainStatus.Text = "Source: " + m_ModelSourceName;
        }

        private void LoadCollisionMap()
        {
            LoadModel();
        }

        private void ResetCollisionMapState()
        {
            m_ImportedCollisionMap = DUMMY_KCL;
            m_CollisionMapMaterialTypeMap = new Dictionary<string, int>();
            m_CollisionMapPlanes = new List<KCL.ColFace>();
            m_CollisionMapColours = new KCLLoader.CollisionMapColours();
            m_CollisionMapSelectedTriangle = -1;
            m_CollisionMapWireFrameView = false;
        }

        protected void CallListForKCLDisplayLists()
        {
            // Solid polygons
            if (!m_CollisionMapWireFrameView)
            {
                GL.CallList(m_KCLMeshDisplayLists[0]);
            }

            // WireFrame overlay
            GL.CallList(m_KCLMeshDisplayLists[1]);

            // Highlighted triangles
            GL.CallList(m_KCLMeshDisplayLists[2]);
        }

        private void DeleteDisplayLists()
        {
            foreach (int dl in m_BMDDisplayLists)
            {
                if (dl > 0) GL.DeleteLists(dl, 1);
            }
            foreach (int dl in m_KCLMeshDisplayLists)
            {
                if (dl > 0) GL.DeleteLists(dl, 1);
            }
            foreach (int dl in m_KCLPickingDisplayLists)
            {
                if (dl > 0) GL.DeleteLists(dl, 1);
            }
        }

        private void PrerenderBMDModel()
        {
            if (m_BMDDisplayLists[0] == 0)
            {
                m_BMDDisplayLists[0] = GL.GenLists(1);
            }
            GL.NewList(m_BMDDisplayLists[0], ListMode.Compile);
            GL.PushAttrib(AttribMask.AllAttribBits);

            Vector3 previewScale = new Vector3(m_ModelPreviewScale);

            if (m_ModelSourceLoaded)
            {
                GL.Disable(EnableCap.Lighting);
                GL.PushMatrix();
                GL.Scale(previewScale);
                GL.FrontFace(FrontFaceDirection.Ccw);

                m_ImportedModel.PrepareToRender();

                m_ImportedModel.Render(RenderMode.Opaque, 1f);
                m_ImportedModel.Render(RenderMode.Translucent, 1f);

                GL.PopMatrix();
            }
            GL.PopAttrib();
            GL.EndList();
        }

        private void UpdateBMDModelAndPreview()
        {
            m_ModelBase.m_Textures.Clear();
            foreach (KeyValuePair<string, ModelBase.TextureDefNitro> textureEntry in m_WorkingTexturesCopy)
            {
                m_ModelBase.m_Textures.Add(textureEntry.Key, textureEntry.Value);
            }

            m_ImportedModel = CallBMDImporter(false);

            glModelView.SetShowMarioReference(true);
            PrerenderBMDModel();
            HighlightSelectedGeometriesAndPolyLists();
            DrawSkeleton();
            glModelView.Refresh();
        }

        private BMD CallBMDImporter(bool save = false)
        {
            Dictionary<string, Dictionary<string, ModelBase.GeometryDef>> originalGeometries =
                new Dictionary<string, Dictionary<string, ModelBase.GeometryDef>>();
            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
            {
                Dictionary<string, ModelBase.GeometryDef> boneGeometries = new Dictionary<string, ModelBase.GeometryDef>();
                foreach (KeyValuePair<string, ModelBase.GeometryDef> geometryEntry in bone.m_Geometries)
                {
                    boneGeometries[geometryEntry.Key] = DuplicateGeometry(geometryEntry.Value);
                }
                originalGeometries[bone.m_ID] = boneGeometries;
            }

            m_ModelBase.ScaleModel(m_ModelImportationSettings.GetImportationScale());

            BMD result = BMDImporter.CallBMDWriter(ref m_ImportedModel.m_File,
                m_ModelBase, m_ModelImportationSettings.m_ExtraOptions, save);

            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
            {
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        polyList.m_FaceLists =
                            originalGeometries[bone.m_ID][geometry.m_ID].m_PolyLists[polyList.m_ID].m_FaceLists;
                    }
                }
            }

            PopulateMaterialsList();

            return result;
        }

        private KCL CallKCLImporter(float scale, bool preserveIndividualFaceCollisionTypes = true, bool save = false)
        {
            List<KCL.ColFace> originalPlanes = m_CollisionMapPlanes;

            KCL result = KCLImporter.CallKCLWriter(m_ImportedCollisionMap.m_File, m_ModelBase,
                scale, m_CollisionMapImportationSettings.m_MinimumFaceSize,
                m_CollisionMapMaterialTypeMap, save);

            if (preserveIndividualFaceCollisionTypes)
            {
                for (int i = 0; i < originalPlanes.Count; i++)
                {
                    SetCollisionMapPlaneCollisionType(result, i, originalPlanes[i].type);
                }
            }

            return result;
        }

        private KCL CallKCLImporter(bool preserveIndividualFaceCollisionTypes = true, bool save = false)
        {
            return CallKCLImporter(m_CollisionMapImportationSettings.GetImportationScale(), preserveIndividualFaceCollisionTypes, save);
        }

        private void RefreshBMDScale()
        {
            m_ModelPreviewScale = m_ModelImportationSettings.GetPreviewScale();
            if (m_ModelSourceLoaded)
            {
                PrerenderBMDModel();
                glModelView.Refresh();
            }
        }

        private void RefreshKCLScale()
        {
            m_CollisionMapPreviewScale = m_CollisionMapImportationSettings.GetPreviewScale();
            if (m_ModelSourceLoaded)
            {
                PrerenderCollisionMap();
                glCollisionMapView.Refresh();
            }
        }

        private void PopulateBoneTree()
        {
            tvModelBonesBones.Nodes.Clear();

            Dictionary<string, TreeNode> parentNodes = new Dictionary<string, TreeNode>();
            foreach (ModelBase.BoneDef rootBone in m_ModelBase.m_BoneTree.GetRootBones())
            {
                foreach (ModelBase.BoneDef bone in rootBone.GetBranch())
                {
                    TreeNode node = new TreeNode(bone.m_ID);
                    if (bone.m_Parent == null)
                    {
                        tvModelBonesBones.Nodes.Add(node);
                    }
                    else
                    {
                        parentNodes[bone.m_Parent.m_ID].Nodes.Add(node);
                    }
                    parentNodes[bone.m_ID] = node;
                }
            }

            tvModelBonesBones.ExpandAll();

            SetEnabledStateModelBoneBaseControls(true, m_ModelBase.m_BoneTree.Count);
            SetEnabledStateModelBoneSpecificControls(false);
            SetEnabledStateModelBoneGeometryControls(false);
            SetEnabledStateModelBonePolylistControls(false);
            txtModelBonesName.Text = null;
            txtModelBonesName.BackColor = Color.White;
            chkModelBonesSettingsBillboard.Checked = false;

            lbxModelBonesGeometries.Items.Clear();
            lbxModelBonesPolylists.Items.Clear();
        }

        private void SetEnabledStateModelBoneBaseControls(bool state, int nBones = -1)
        {
            btnModelBonesAddBone.Enabled = (nBones > -1) ? (nBones <= ModelBase.BoneDefRoot.MAX_BONE_COUNT) : state;
        }

        private void SetEnabledStateModelBoneSpecificControls(bool state)
        {
            btnModelBonesPasteToBone.Enabled = (m_SourcePolylists != null || m_SourceGeometries != null);
            btnModelBonesRenameBone.Enabled = state;
            txtModelBonesName.Enabled = state;
            chkModelBonesSettingsBillboard.Enabled = state;
        }

        private void SetEnabledStateModelBoneGeometryControls(bool state)
        {
            btnModelBonesCopyGeometry.Enabled = state;
            btnModelBonesCutGeometry.Enabled = state;
            btnModelBonesPasteToGeometry.Enabled = (m_SourcePolylists != null);
            btnModelBonesRemoveGeometry.Enabled = state;
        }

        private void SetEnabledStateModelBonePolylistControls(bool state)
        {
            btnModelBonesCopyPolylist.Enabled = state;
            btnModelBonesCutPolylist.Enabled = state;
            btnModelBonesRemovePolylist.Enabled = state;
            cmbModelBonesPolylistMaterial.Enabled = state;
        }

        private void PopulateMaterialsList()
        {
            lbxModelMaterials.Items.Clear();
            cmbModelBonesPolylistMaterial.Items.Clear();

            foreach (ModelBase.MaterialDef material in m_ModelBase.m_Materials.Values)
            {
                lbxModelMaterials.Items.Add(material.m_ID);
                cmbModelBonesPolylistMaterial.Items.Add(material.m_ID);
            }

            txtModelMaterialName.Text = null;

            chkModelMaterialLight1.Checked = false;
            chkModelMaterialLight2.Checked = false;
            chkModelMaterialLight3.Checked = false;
            chkModelMaterialLight4.Checked = false;
            cmbModelMaterialPolygonDrawingFace.SelectedIndex = -1;
            cmbModelMaterialPolygonMode.SelectedIndex = -1;

            chkModelMaterialWireMode.Checked = false;
            chkModelMaterialDepthTestDecal.Checked = false;
            chkModelMaterialFog.Checked = false;
            chkModelMaterialRenderOnePixelPolygons.Checked = false;
            chkModelMaterialFarClipping.Checked = false;
            chkModelMaterialShiniessTable.Checked = false;

            ResetColourButtonValue(btnModelMaterialDiffuse);
            ResetColourButtonValue(btnModelMaterialAmbient);
            ResetColourButtonValue(btnModelMaterialSpecular);
            ResetColourButtonValue(btnModelMaterialEmission);
            nudModelMaterialAlpha.Value = 0;

            cmbModelMaterialTextureID.SelectedIndex = -1;
            cmbModelMaterialTextureTilingX.SelectedIndex = -1; ;
            cmbModelMaterialTextureTilingY.SelectedIndex = -1;
            txtModelMaterialTextureScaleX.Text = null;
            txtModelMaterialTextureScaleY.Text = null;
            txtModelMaterialTextureRotation.Text = null;
            txtModelMaterialTextureTranslationX.Text = null;
            txtModelMaterialTextureTranslationY.Text = null;
            cmbModelMaterialTexGenMode.SelectedIndex = -1;

            btnModelMaterialAddMaterial.Enabled = true;
        }

        private void PopulateMaterialSettings(ModelBase.MaterialDef material)
        {
            SetEnabledStateModelMaterialControls(true);

            bool referencedByBone = false;
            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree.GetAsList())
            {
                if (bone.m_MaterialsInBranch.Contains(material.m_ID))
                {
                    referencedByBone = true;
                    break;
                }
            }
            btnModelMaterialRemoveMaterial.Enabled = !referencedByBone;

            txtModelMaterialName.Text = material.m_ID;

            chkModelMaterialLight1.Checked = material.m_Lights[0];
            chkModelMaterialLight2.Checked = material.m_Lights[1];
            chkModelMaterialLight3.Checked = material.m_Lights[2];
            chkModelMaterialLight4.Checked = material.m_Lights[3];
            cmbModelMaterialPolygonDrawingFace.SelectedIndex = (int)material.m_PolygonDrawingFace;
            cmbModelMaterialPolygonMode.SelectedIndex = (int)material.m_PolygonMode;

            chkModelMaterialWireMode.Checked = material.m_WireMode;
            chkModelMaterialDepthTestDecal.Checked = material.m_DepthTestDecal;
            chkModelMaterialFog.Checked = material.m_FogFlag;
            chkModelMaterialRenderOnePixelPolygons.Checked = material.m_RenderOnePixelPolygons;
            chkModelMaterialFarClipping.Checked = material.m_FarClipping;
            chkModelMaterialShiniessTable.Checked = material.m_ShininessTableEnabled;

            SetColourButtonValue(btnModelMaterialDiffuse, material.m_Diffuse);
            SetColourButtonValue(btnModelMaterialAmbient, material.m_Ambient);
            SetColourButtonValue(btnModelMaterialSpecular, material.m_Specular);
            SetColourButtonValue(btnModelMaterialEmission, material.m_Emission);
            nudModelMaterialAlpha.Value = material.m_Alpha;

            bool hasTexture = (material.m_TextureDefID != null);
            chkModelMaterialTextureEnabled.Checked = hasTexture;
            if (hasTexture)
            {
                cmbModelMaterialTextureID.SelectedIndex = cmbModelMaterialTextureID.Items.IndexOf(material.m_TextureDefID);
            }
            cmbModelMaterialTextureTilingX.SelectedIndex = (int)material.m_TexTiling[0];
            cmbModelMaterialTextureTilingY.SelectedIndex = (int)material.m_TexTiling[1];
            txtModelMaterialTextureScaleX.Text = Helper.ToString4DP(material.m_TextureScale.X);
            txtModelMaterialTextureScaleY.Text = Helper.ToString4DP(material.m_TextureScale.Y);
            txtModelMaterialTextureRotation.Text = Helper.ToString4DP(material.m_TextureRotation);
            txtModelMaterialTextureTranslationX.Text = Helper.ToString4DP(material.m_TextureTranslation.X);
            txtModelMaterialTextureTranslationY.Text = Helper.ToString4DP(material.m_TextureTranslation.Y);
            cmbModelMaterialTexGenMode.SelectedIndex = (int)material.m_TexGenMode;
            SetEnabledStateModelMaterialTextureSettings(hasTexture);
        }

        private void ResetMaterialTextureSettings()
        {
            cmbModelMaterialTextureID.SelectedIndex = -1;
            cmbModelMaterialTextureTilingX.SelectedIndex = -1;
            cmbModelMaterialTextureTilingY.SelectedIndex = -1;
            txtModelMaterialTextureScaleX.Text = null;
            txtModelMaterialTextureScaleY.Text = null;
            txtModelMaterialTextureRotation.Text = null;
            txtModelMaterialTextureTranslationX.Text = null;
            txtModelMaterialTextureTranslationY.Text = null;
            cmbModelMaterialTexGenMode.SelectedIndex = -1;
        }

        private void SetEnabledStateModelMaterialTextureSettings(bool state)
        {
            cmbModelMaterialTextureID.Enabled = state;
            cmbModelMaterialTextureTilingX.Enabled = state;
            cmbModelMaterialTextureTilingY.Enabled = state;
            txtModelMaterialTextureScaleX.Enabled = state;
            txtModelMaterialTextureScaleY.Enabled = state;
            txtModelMaterialTextureRotation.Enabled = state;
            txtModelMaterialTextureTranslationX.Enabled = state;
            txtModelMaterialTextureTranslationY.Enabled = state;
            cmbModelMaterialTexGenMode.Enabled = state;
        }

        private void SetEnabledStateModelMaterialControls(bool state)
        {
            btnModelMaterialAddMaterial.Enabled = state;
            if (btnModelMaterialRemoveMaterial.Enabled)
            {
                btnModelMaterialRemoveMaterial.Enabled = state;
            }
            btnModelMaterialRenameMaterial.Enabled = state;
            txtModelMaterialName.Enabled = state;

            chkModelMaterialLight1.Enabled = state;
            chkModelMaterialLight2.Enabled = state;
            chkModelMaterialLight3.Enabled = state;
            chkModelMaterialLight4.Enabled = state;
            cmbModelMaterialPolygonDrawingFace.Enabled = state;
            cmbModelMaterialPolygonMode.Enabled = state;

            chkModelMaterialWireMode.Enabled = state;
            chkModelMaterialDepthTestDecal.Enabled = state;
            chkModelMaterialFog.Enabled = state;
            chkModelMaterialRenderOnePixelPolygons.Enabled = state;
            chkModelMaterialFarClipping.Enabled = state;
            chkModelMaterialShiniessTable.Enabled = state;

            btnModelMaterialDiffuse.Enabled = state;
            btnModelMaterialAmbient.Enabled = state;
            btnModelMaterialSpecular.Enabled = state;
            btnModelMaterialEmission.Enabled = state;
            nudModelMaterialAlpha.Enabled = state;

            chkModelMaterialTextureEnabled.Enabled = state;
            SetEnabledStateModelMaterialTextureSettings(state);

            btnModelMaterialApplySettings.Enabled = state;
        }

        private void SetColourButtonValue(Button button, Color colour)
        {
            string hexColourString = Helper.GetHexColourString(colour);
            button.Text = hexColourString;
            button.BackColor = colour;
            float luma = 0.2126f * colour.R + 0.7152f * colour.G + 0.0722f * colour.B;
            if (luma < 50)
            {
                button.ForeColor = Color.White;
            }
            else
            {
                button.ForeColor = Color.Black;
            }
        }

        private void ResetColourButtonValue(Button button)
        {
            button.Text = null;
            button.BackColor = Color.Transparent;
            button.ForeColor = Color.Black;
        }

        private void PopulateTextureAndPaletteLists()
        {
            lbxModelTextures.Items.Clear();
            cmbModelMaterialTextureID.Items.Clear();
            lbxModelPalettes.Items.Clear();
            cmbModelTexturesPalette.Items.Clear();

            txtModelTexturesName.Text = null;
            txtModelTexturesName.BackColor = Color.White;
            txtModelPalettesName.Text = null;
            txtModelPalettesName.BackColor = Color.White;

            foreach (ModelBase.TextureDefBase texture in m_WorkingTexturesCopy.Values)
            {
                lbxModelTextures.Items.Add(texture.m_ID);
                cmbModelMaterialTextureID.Items.Add(texture.m_ID);
            }

            foreach (string paletteID in m_WorkingPalettesCopy.Keys)
            {
                lbxModelPalettes.Items.Add(paletteID);
                cmbModelTexturesPalette.Items.Add(paletteID);
            }

            pbxModelTexturesPreview.Image = null;
            pbxModelTexturesPreview.Refresh();

            gridModelPalettesPaletteColours.ClearColours();

            ResetColourButtonValue(btnModelPalettesSelectedColour);

            SetEnabledStateModelTextureControls(false);
            SetEnabledStateModelPaletteControls(false);
        }

        private void btnModelGeneralSelectTarget_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a model (BMD) file to replace", new String[] { ".bmd" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_BMDTargetName = m_ROMFileSelect.m_SelectedFile;
                txtModelGeneralTargetName.Text = m_BMDTargetName;
                UpdateEnabledStateMenuControls();
            }
        }

        private void UpdateEnabledStateMenuControls()
        {
            mnitLoad.Enabled = true;
            mnitLoadExternalModel.Enabled = true;
            mnitLoadInternalModelCollisionMap.Enabled = true;
            mnitLoadRevertChanges.Enabled = m_ModelSourceLoaded;

            mnitImport.Enabled = m_ModelSourceLoaded;
            mnitImportModelAndCollisionMap.Enabled = (IsBMDTargetSet() && IsKCLTargetSet());
            mnitImportModelOnly.Enabled = IsBMDTargetSet();
            mnitImportCollisionMapOnly.Enabled = IsKCLTargetSet();

            mnitExport.Enabled = m_ModelSourceLoaded;
            bool modelAvailable = m_ModelSourceLoaded && m_StartMode != StartMode.CollisionMap;
            mnitExportModel.Enabled = modelAvailable;
            mnitExportCollisionMap.Enabled = m_ModelSourceLoaded;
            mnitExportTextures.Enabled = modelAvailable;
        }

        private void mnitLoadExternalModel_Click(object sender, EventArgs e)
        {
            m_OpenFileDialogue.Title = "Select a model";
            m_OpenFileDialogue.Filter = Strings.MODEL_FORMATS_FILTER;
            if (m_OpenFileDialogue.ShowDialog(this) == DialogResult.OK)
            {
                m_ModelSourceName = m_OpenFileDialogue.FileName;
                m_ModelSourceType = ModelSourceType.External;

                LoadModel();
            }
        }

        private void mnitLoadInternalModelCollisionMap_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a model (BMD) or collision map (KCL) to load", new String[] { ".bmd",".kcl" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_ModelSourceName = m_ROMFileSelect.m_SelectedFile;
                m_ModelSourceType = ModelSourceType.Internal;

                LoadModel();
            }
        }

        private void mnitLoadRevertChanges_Click(object sender, EventArgs e)
        {
            if (m_ModelSourceLoaded)
            {
                LoadModel();
            }
        }

        private void tcModelSettings_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (m_ModelSourceLoaded)
            {
                UpdateBMDModelAndPreview();
            }
        }

        private bool IsModelBonesTabSelected()
        {
            if (tcModelSettings.SelectedTab == null) return false;
            return "Model.Bones".Equals(tcModelSettings.SelectedTab.Tag);
        }

        private void lbxModelMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxModelMaterials.SelectedIndex > -1)
            {
                string materialID = lbxModelMaterials.SelectedItem.ToString();
                ModelBase.MaterialDef material = m_ModelBase.m_Materials[materialID];

                PopulateMaterialSettings(material);
            }
        }

        private void btnModelMaterialDiffuse_Click(object sender, EventArgs e)
        {
            GetColourDialogueResult(btnModelMaterialDiffuse);
        }

        private void btnModelMaterialAmbient_Click(object sender, EventArgs e)
        {
            GetColourDialogueResult(btnModelMaterialAmbient);
        }

        private void btnModelMaterialSpecular_Click(object sender, EventArgs e)
        {
            GetColourDialogueResult(btnModelMaterialSpecular);
        }

        private void btnModelMaterialEmission_Click(object sender, EventArgs e)
        {
            GetColourDialogueResult(btnModelMaterialEmission);
        }

        private void chkModelMaterialWireMode_CheckedChanged(object sender, EventArgs e)
        {
            nudModelMaterialAlpha.Enabled = !chkModelMaterialWireMode.Checked;
        }

        private void chkModelMaterialTextureEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SetEnabledStateModelMaterialTextureSettings(chkModelMaterialTextureEnabled.Checked);
        }

        private void btnModelMaterialAddMaterial_Click(object sender, EventArgs e)
        {
            int materialCount = m_ModelBase.m_Materials.Count;
            if (materialCount >= int.MaxValue)
            {
                MessageBox.Show("Maximum material count reached: " + int.MaxValue);
                return;
            }

            string materialIDNewBase = "material-";
            string materialIDNewSuffix = null;
            string materialIDNew = null;

            if (materialCount < 999)
            {
                for (int i = 0; i < 1000; i++)
                {
                    materialIDNewSuffix = i.ToString("D3");
                    if (!m_ModelBase.m_Materials.ContainsKey(materialIDNewBase + materialIDNewSuffix))
                    {
                        break;
                    }
                }
                materialIDNew = materialIDNewBase + materialIDNewSuffix;
            }
            else
            {
                bool found = false;
                string candidate;
                for (int i = 0; i < 10; i++)
                {
                    candidate = System.Guid.NewGuid().ToString();
                    if (!m_ModelBase.m_Materials.ContainsKey(candidate))
                    {
                        materialIDNew = candidate;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show("Unable to generate a unique material ID. Try again.");
                    return;
                }
            }

            ModelBase.MaterialDef material = new ModelBase.MaterialDef(materialIDNew);
            m_ModelBase.m_Materials.Add(materialIDNew, material);

            PopulateMaterialsList();
            lbxModelMaterials.SelectedIndex = lbxModelMaterials.Items.IndexOf(materialIDNew);
        }

        private void btnModelMaterialRemoveMaterial_Click(object sender, EventArgs e)
        {
            string materialID = GetSelectedMaterialID();
            if (materialID == null) return;

            m_ModelBase.m_Materials.Remove(materialID);

            PopulateMaterialsList();
        }

        private void btnModelMaterialRenameMaterial_Click(object sender, EventArgs e)
        {
            string materialIDOld = GetSelectedMaterialID();
            if (materialIDOld == null) return;

            string materialIDNew = txtModelMaterialName.Text;
            if (materialIDNew == null || materialIDNew.Length < 1)
            {
                txtModelMaterialName.BackColor = Helper.LIGHT_RED;
                return;
            }
            materialIDNew = ASCII_PRINTABLE_NO_SPACE.Replace(materialIDNew, "");
            txtModelMaterialName.BackColor = Color.White;

            if (m_ModelBase.m_Materials.ContainsKey(materialIDNew))
            {
                MessageBox.Show("Material names must be unique");
                return;
            }

            ModelBase.MaterialDef material = m_ModelBase.m_Materials[materialIDOld];
            material.m_ID = materialIDNew;
            m_ModelBase.m_Materials[materialIDNew] = material;
            m_ModelBase.m_Materials.Remove(materialIDOld);

            int index;
            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree.GetAsList())
            {
                if ((index = bone.m_MaterialsInBranch.IndexOf(materialIDOld)) > -1)
                {
                    bone.m_MaterialsInBranch[index] = materialIDNew;
                }
            }

            PopulateMaterialsList();
            lbxModelMaterials.SelectedIndex = lbxModelMaterials.Items.IndexOf(materialIDNew);
        }

        private void btnModelMaterialApplySettings_Click(object sender, EventArgs e)
        {
            string materialID = GetSelectedMaterialID();
            if (materialID == null) return;

            ModelBase.MaterialDef material = m_ModelBase.m_Materials[materialID];

            material.m_Lights[0] = chkModelMaterialLight1.Checked;
            material.m_Lights[1] = chkModelMaterialLight2.Checked;
            material.m_Lights[2] = chkModelMaterialLight3.Checked;
            material.m_Lights[3] = chkModelMaterialLight4.Checked;
            material.m_PolygonDrawingFace =
                (ModelBase.MaterialDef.PolygonDrawingFace)cmbModelMaterialPolygonDrawingFace.SelectedIndex;
            material.m_PolygonMode =
                (ModelBase.MaterialDef.PolygonMode)cmbModelMaterialPolygonMode.SelectedIndex;

            material.m_WireMode = chkModelMaterialWireMode.Checked;
            material.m_DepthTestDecal = chkModelMaterialDepthTestDecal.Checked;
            material.m_FogFlag = chkModelMaterialFog.Checked;
            material.m_RenderOnePixelPolygons = chkModelMaterialRenderOnePixelPolygons.Checked;
            material.m_FarClipping = chkModelMaterialFarClipping.Checked;
            material.m_ShininessTableEnabled = chkModelMaterialShiniessTable.Checked;

            material.m_Diffuse = btnModelMaterialDiffuse.BackColor;
            material.m_Ambient = btnModelMaterialAmbient.BackColor;
            material.m_Specular = btnModelMaterialSpecular.BackColor;
            material.m_Emission = btnModelMaterialEmission.BackColor;
            material.m_Alpha = (byte)((material.m_WireMode) ? 0 : nudModelMaterialAlpha.Value);

            bool hasTexture = chkModelMaterialTextureEnabled.Checked;
            if (hasTexture)
            {
                material.m_TextureDefID = cmbModelMaterialTextureID.SelectedItem.ToString();
                material.m_TexTiling[0] = (ModelBase.MaterialDef.TexTiling)cmbModelMaterialTextureTilingX.SelectedIndex;
                material.m_TexTiling[1] = (ModelBase.MaterialDef.TexTiling)cmbModelMaterialTextureTilingY.SelectedIndex;
                Helper.TryParseFloat(txtModelMaterialTextureScaleX.Text, out material.m_TextureScale.X);
                Helper.TryParseFloat(txtModelMaterialTextureScaleY.Text, out material.m_TextureScale.Y);
                Helper.TryParseFloat(txtModelMaterialTextureRotation.Text, out material.m_TextureRotation);
                Helper.TryParseFloat(txtModelMaterialTextureTranslationX.Text, out material.m_TextureTranslation.Y);
                Helper.TryParseFloat(txtModelMaterialTextureTranslationY.Text, out material.m_TextureTranslation.Y);
                material.m_TexGenMode = (ModelBase.TexGenMode)cmbModelMaterialTexGenMode.SelectedIndex;
            }

            PopulateMaterialSettings(material);
            UpdateBMDModelAndPreview();
            lbxModelMaterials.SelectedIndex = lbxModelMaterials.Items.IndexOf(material.m_ID);
        }

        private void PopulateBoneGeometryList(ModelBase.BoneDef bone)
        {
            lbxModelBonesGeometries.Items.Clear();
            foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
            {
                lbxModelBonesGeometries.Items.Add(geometry.m_ID);
            }
            lbxModelBonesPolylists.Items.Clear();

            SetEnabledStateModelBoneGeometryControls(false);
        }

        private void tvModelBonesBones_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ModelBase.BoneDef bone = GetSelectedBone();
            if ((m_TargetBone = bone) == null) return;

            PopulateBoneGeometryList(bone);

            SetEnabledStateModelBoneSpecificControls(true);
            SetEnabledStateModelBoneGeometryControls(false);
            SetEnabledStateModelBonePolylistControls(false);

            txtModelBonesName.Text = bone.m_ID;
            chkModelBonesSettingsBillboard.Checked = bone.m_Billboard;

            DrawSkeleton();

            HighlightSelectedGeometriesAndPolyLists();
        }

        private static string GetTreeViewSelectedNodeName(TreeView treeView)
        {
            return (treeView.SelectedNode != null) ? treeView.SelectedNode.Text : null;
        }

        private static string GetListBoxSelectedItemName(ListBox listBox)
        {
            return (listBox.SelectedIndex > -1) ?
                listBox.Items[listBox.SelectedIndex].ToString() : null;
        }

        private static List<string> GetListBoxSelectedItemNames(ListBox listBox)
        {
            List<string> selectedItemNames = new List<string>();
            foreach (int index in listBox.SelectedIndices)
            {
                selectedItemNames.Add(listBox.Items[index].ToString());
            }
            return selectedItemNames;
        }

        private static bool SetListBoxSelectedItemByName(ListBox listBox, string itemName)
        {
            int index = listBox.Items.IndexOf(itemName);
            listBox.SelectedIndex = index;
            return (index > -1);
        }

        private static string GetComboBoxSelectedItemName(ComboBox comboBox)
        {
            return (comboBox.SelectedIndex > -1) ?
                comboBox.Items[comboBox.SelectedIndex].ToString() : null;
        }

        private static bool SetComboBoxSelectedItemByName(ComboBox comboBox, string itemName)
        {
            int index = comboBox.Items.IndexOf(itemName);
            comboBox.SelectedIndex = index;
            return (index > -1);
        }

        private string GetSelectedBoneID()
        {
            return GetTreeViewSelectedNodeName(tvModelBonesBones);
        }

        private string GetSelectedGeometryID()
        {
            return GetListBoxSelectedItemName(lbxModelBonesGeometries);
        }

        private List<string> GetSelectedGeometryIDs()
        {
            return GetListBoxSelectedItemNames(lbxModelBonesGeometries);
        }

        private List<string> GetSelectedPolyListIDs()
        {
            return GetListBoxSelectedItemNames(lbxModelBonesPolylists);
        }

        private string GetSelectedTextureID()
        {
            return GetListBoxSelectedItemName(lbxModelTextures);
        }

        private string GetSelectedPaletteID()
        {
            return GetListBoxSelectedItemName(lbxModelPalettes);
        }

        private string GetSelectedMaterialID()
        {
            return GetListBoxSelectedItemName(lbxModelMaterials);
        }

        private ModelBase.BoneDef GetSelectedBone()
        {
            string boneID = GetSelectedBoneID();
            if (boneID == null) return null;

            return m_ModelBase.m_BoneTree.GetBoneByID(boneID);
        }

        private ModelBase.GeometryDef GetSelectedGeometry()
        {
            string boneID = GetSelectedBoneID();
            string geometryID = GetSelectedGeometryID();

            if (boneID == null || geometryID == null) return null;

            return m_ModelBase.m_BoneTree.GetBoneByID(boneID).m_Geometries[geometryID];
        }

        private List<ModelBase.GeometryDef> GetSelectedGeometries()
        {
            ModelBase.BoneDef bone = GetSelectedBone();
            List<string> geometryIDs = GetSelectedGeometryIDs();
            List<ModelBase.GeometryDef> geometries = new List<ModelBase.GeometryDef>();

            if (bone != null && geometryIDs != null)
            {
                foreach (string geometryID in geometryIDs)
                {
                    geometries.Add(bone.m_Geometries[geometryID]);
                }
            }

            return geometries;
        }

        private List<ModelBase.PolyListDef> GetSelectedPolyLists()
        {
            List<ModelBase.PolyListDef> selectedPolyLists = new List<ModelBase.PolyListDef>();
            ModelBase.GeometryDef geometry = GetSelectedGeometry();
            if (geometry == null) return selectedPolyLists;

            List<string> polylistIDs = GetSelectedPolyListIDs();
            foreach (string polyListID in polylistIDs)
            {
                selectedPolyLists.Add(geometry.m_PolyLists[polyListID]);
            }

            return selectedPolyLists;
        }

        private void PopulateBoneGeometryPolyListList()
        {
            ModelBase.GeometryDef geometry = GetSelectedGeometry();
            if ((m_TargetGeometry = geometry) == null) return;

            lbxModelBonesPolylists.Items.Clear();
            foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
            {
                lbxModelBonesPolylists.Items.Add(polyList.m_ID);
            }

            SetEnabledStateModelBoneGeometryControls(true);
            SetEnabledStateModelBonePolylistControls(false);
        }

        private void DrawSkeleton()
        {
            m_BMDDisplayLists[2] = GL.GenLists(1);
            GL.NewList(m_BMDDisplayLists[2], ListMode.Compile);

            if (IsModelBonesTabSelected())
            {
                GL.PushAttrib(AttribMask.AllAttribBits);
                GL.PushMatrix();

                GL.Disable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Never);

                ModelBase.BoneDef selectedBone = GetSelectedBone();

                GL.Scale(new Vector3(m_ModelPreviewScale));

                Dictionary<string, Vector3> bonePositions = new Dictionary<string, Vector3>();
                foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
                {
                    Vector3 position = Vector3.Zero;
                    Vector3.Transform(ref position, ref bone.m_GlobalTransformation, out position);
                    bonePositions.Add(bone.m_ID, position);

                    DrawCube(position, 0.125f, (bone == selectedBone) ? Color.HotPink : Color.Yellow);

                    if (bone.m_Parent != null)
                    {
                        GL.Begin(PrimitiveType.Lines);

                        GL.Color3(Color.LawnGreen);
                        GL.Vertex3(bonePositions[bone.m_Parent.m_ID]);
                        GL.Vertex3(position);

                        GL.End();
                    }
                }

                GL.PopMatrix();
                GL.PopAttrib();
            }

            GL.EndList();

            glModelView.Refresh();
        }

        private void DrawCube(Vector3 position, float halfSize, Color colour)
        {
            Vector3 v1 = new Vector3(position.X + halfSize, position.Y - halfSize, position.Z - halfSize);
            Vector3 v2 = new Vector3(position.X + halfSize, position.Y - halfSize, position.Z + halfSize);
            Vector3 v3 = new Vector3(position.X - halfSize, position.Y - halfSize, position.Z + halfSize);
            Vector3 v4 = new Vector3(position.X - halfSize, position.Y - halfSize, position.Z - halfSize);
            Vector3 v5 = new Vector3(position.X + halfSize, position.Y + halfSize, position.Z - halfSize);
            Vector3 v6 = new Vector3(position.X + halfSize, position.Y + halfSize, position.Z + halfSize);
            Vector3 v7 = new Vector3(position.X - halfSize, position.Y + halfSize, position.Z + halfSize);
            Vector3 v8 = new Vector3(position.X - halfSize, position.Y + halfSize, position.Z - halfSize);

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(colour);

            GL.Vertex3(v2); GL.Vertex3(v3); GL.Vertex3(v4);
            GL.Vertex3(v8); GL.Vertex3(v7); GL.Vertex3(v6);
            GL.Vertex3(v1); GL.Vertex3(v5); GL.Vertex3(v6);
            GL.Vertex3(v2); GL.Vertex3(v6); GL.Vertex3(v7);
            GL.Vertex3(v7); GL.Vertex3(v8); GL.Vertex3(v4);
            GL.Vertex3(v1); GL.Vertex3(v4); GL.Vertex3(v8);
            GL.Vertex3(v1); GL.Vertex3(v2); GL.Vertex3(v4);
            GL.Vertex3(v5); GL.Vertex3(v8); GL.Vertex3(v6);
            GL.Vertex3(v2); GL.Vertex3(v1); GL.Vertex3(v6);
            GL.Vertex3(v3); GL.Vertex3(v2); GL.Vertex3(v7);
            GL.Vertex3(v3); GL.Vertex3(v7); GL.Vertex3(v4);
            GL.Vertex3(v5); GL.Vertex3(v1); GL.Vertex3(v8);

            GL.End();
        }

        private void lbxModelBonesGeometries_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateBoneGeometryPolyListList();

            HighlightSelectedGeometriesAndPolyLists();
        }

        private void HighlightSelectedGeometriesAndPolyLists()
        {
            List<ModelBase.GeometryDef> geometries = GetSelectedGeometries();
            List<ModelBase.PolyListDef> polyLists = GetSelectedPolyLists();

            List<ModelBase.PolyListDef> geometryOnlyPolyLists = new List<ModelBase.PolyListDef>();
            foreach (ModelBase.GeometryDef geometry in geometries)
            {
                geometryOnlyPolyLists.AddRange(geometry.m_PolyLists.Values);
            }
            foreach (ModelBase.PolyListDef polyList in polyLists)
            {
                geometryOnlyPolyLists.Remove(polyList);
            }

            m_BMDDisplayLists[1] = GL.GenLists(1);
            GL.NewList(m_BMDDisplayLists[1], ListMode.Compile);

            if (IsModelBonesTabSelected())
            {
                GL.PushMatrix();
                GL.PushAttrib(AttribMask.AllAttribBits);

                GL.Disable(EnableCap.Lighting);

                GL.Scale(new Vector3(m_ModelPreviewScale));

                GL.ColorMask(false, false, false, false);
                GL.Enable(EnableCap.StencilTest);
                GL.StencilMask(0x3);
                GL.StencilFunc(StencilFunction.Always, 0x1, 0x3);
                GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
                DrawPolyListPolygonsForHighlighting(geometryOnlyPolyLists);
                DrawPolyListPolygonsForHighlighting(polyLists);

                GL.ColorMask(true, true, true, true);
                GL.Enable(EnableCap.PolygonOffsetFill);
                GL.PolygonOffset(-1.0f, -1.0f);
                GL.StencilFunc(StencilFunction.Equal, 0x1, 0x3);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);
                GL.Color4(Color.FromArgb(100, Color.Orange));
                DrawPolyListPolygonsForHighlighting(geometryOnlyPolyLists);
                GL.Color4(Color.FromArgb(100, Color.Red));
                DrawPolyListPolygonsForHighlighting(polyLists);

                GL.Disable(EnableCap.PolygonOffsetFill);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(3.0f);
                GL.StencilFunc(StencilFunction.Equal, 0x0, 0x3);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
                GL.DepthFunc(DepthFunction.Always);
                GL.Color4(Color.Orange);
                DrawPolyListPolygonsForHighlighting(geometryOnlyPolyLists);
                GL.Color4(Color.Red);
                DrawPolyListPolygonsForHighlighting(polyLists);

                GL.PopAttrib();
                GL.PopMatrix();
            }
            GL.EndList();

            glModelView.Refresh();
        }

        private void DrawPolyListPolygonsForHighlighting(List<ModelBase.PolyListDef> polyLists)
        {
            if (polyLists == null || polyLists.Count < 1) return;

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.DepthMask(true);
            foreach (ModelBase.PolyListDef polyList in polyLists)
            {
                foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
                {
                    if (faceList.m_Type != ModelBase.PolyListType.Triangles &&
                        faceList.m_Type != ModelBase.PolyListType.TriangleStrip)
                    {
                        continue;
                    }
                    foreach (ModelBase.FaceDef face in faceList.m_Faces)
                    {
                        GL.Begin(PrimitiveType.Triangles);
                        GL.Vertex3(face.m_Vertices[0].m_Position);
                        GL.Vertex3(face.m_Vertices[1].m_Position);
                        GL.Vertex3(face.m_Vertices[2].m_Position);
                        GL.End();
                    }
                }
            }
            GL.Disable(EnableCap.CullFace);
        }

        private void PopulateBoneGeometryPolyListSettings()
        {
            List<ModelBase.PolyListDef> polyLists = GetSelectedPolyLists();
            if (polyLists.Count < 1) return;

            SetEnabledStateModelBonePolylistControls(true);

            if (polyLists.Count < 2)
            {
                SetComboBoxSelectedItemByName(cmbModelBonesPolylistMaterial, polyLists[0].m_MaterialName);
            }
            else
            {
                cmbModelBonesPolylistMaterial.SelectedIndex = -1;
            }            
        }

        private void lbxModelBonesPolylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateBoneGeometryPolyListSettings();

            HighlightSelectedGeometriesAndPolyLists();
        }

        private void DrawTexturePreview(string textureID)
        {
            ModelBase.TextureDefBase texture = m_WorkingTexturesCopy[textureID];

            Bitmap bitmap = texture.GetBitmap();

            pbxModelTexturesPreview.Image = bitmap;
            pbxModelTexturesPreview.Refresh();
        }

        private void SetEnabledStateModelTextureControls(bool state)
        {
            txtModelTexturesName.Enabled = state;
            cmbModelTexturesWidth.Enabled = state;
            cmbModelTexturesHeight.Enabled = state;
            cmbModelTexturesPalette.Enabled = state;

            btnModelTexturesAddTexture.Enabled = state;
            btnModelTexturesRemoveTexture.Enabled = state;
            btnModelTexturesRenameTexture.Enabled = state;

            cmbModelTexturesWidth.Enabled = state;
            cmbModelTexturesHeight.Enabled = state;
        }

        private void SetEnabledStateModelPaletteControls(bool state)
        {
            txtModelPalettesName.Enabled = state;
            btnModelPalettesRenamePalette.Enabled = state;
            btnModelPalettesSelectedColour.Enabled = state;
        }

        private void PopulateTextureSettings(string textureID)
        {
            ModelBase.TextureDefBase texture = m_WorkingTexturesCopy[textureID];

            txtModelTexturesName.Text = texture.GetTexName();
            cmbModelTexturesFormat.SelectedIndex = (int)texture.m_Format - 1;

            cmbModelTexturesWidth.Items.Clear();
            cmbModelTexturesHeight.Items.Clear();

            int textureSize = texture.GetWidth() * texture.GetHeight();
            int maxDimension8Shift = 0;
            for (int i = 0; i < 8; i++)
            {
                if (textureSize / (8 << i) < 8)
                {
                    break;
                }
                maxDimension8Shift = i;
            }

            for (int i = 0; i <= maxDimension8Shift; i++)
            {
                int dimension = (8 << i);
                cmbModelTexturesWidth.Items.Add(dimension);
                cmbModelTexturesHeight.Items.Add(dimension);
            }

            cmbModelTexturesWidth.SelectedIndex = cmbModelTexturesWidth.Items.IndexOf(texture.GetWidth());
            cmbModelTexturesHeight.SelectedIndex = cmbModelTexturesHeight.Items.IndexOf(texture.GetHeight());

            cmbModelTexturesPalette.SelectedIndex = cmbModelTexturesPalette.Items.IndexOf(texture.GetPalName());

            SetEnabledStateModelTextureControls(true);
        }

        private void PopulatePaletteSettings(string paletteID)
        {
            byte[] palette = m_WorkingPalettesCopy[paletteID];

            txtModelPalettesName.Text = paletteID;

            int nColours = palette.Length / 2;
            Color[] paletteColours = new Color[nColours];
            for (int i = 0; i < nColours; i++)
            {
                ushort palColour = (ushort)(palette[(i * 2)] | (palette[(i * 2) + 1] << 8));
                paletteColours[i] = Helper.BGR15ToColor(palColour);
            }

            gridModelPalettesPaletteColours.SetColours(paletteColours);

            SetEnabledStateModelPaletteControls(true);
        }

        private Color? GetColourDialogueResult(Button button)
        {
            ColorDialog colourDialogue = new ColorDialog();
            DialogResult result = colourDialogue.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                Color colour = colourDialogue.Color;
                SetColourButtonValue(button, colour);
                return colour;
            }
            return null;
        }

        private void lbxModelTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxModelTextures.SelectedIndex < 0) return;

            string textureID = GetSelectedTextureID();
            PopulateTextureSettings(textureID);
            DrawTexturePreview(textureID);
        }

        private void cmbModelTexturesPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxModelTextures.SelectedIndex < 0) return;

            string textureID = GetSelectedTextureID();
            string paletteID = GetComboBoxSelectedItemName(cmbModelTexturesPalette);
            if (paletteID == null) return;

            ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureID];

            texture = new ModelBase.TextureDefNitro(texture.GetTexName(), texture.GetNitroTexData(),
                paletteID, m_WorkingPalettesCopy[paletteID], texture.GetWidth(), texture.GetHeight(), 
                texture.GetColor0Mode(), texture.m_Format);
            m_WorkingTexturesCopy[textureID] = texture;

            DrawTexturePreview(textureID);
        }

        private void UpdateTextureDimensions(string textureID, 
            ComboBox dimensionUpdatedComboBox, ComboBox dimensionOtherComboBox, bool isUpdatedDimensionWidth)
        {
            ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureID];
            int size = texture.GetWidth() * texture.GetHeight();

            string dimensionUpdatedString = GetComboBoxSelectedItemName(dimensionUpdatedComboBox);
            int dimensionUpdated = int.Parse(dimensionUpdatedString);
            int dimensionOther = size / dimensionUpdated;

            int width = isUpdatedDimensionWidth ? dimensionUpdated : dimensionOther;
            int height = isUpdatedDimensionWidth ? dimensionOther : dimensionUpdated;

            texture = new ModelBase.TextureDefNitro(texture.GetTexName(), texture.GetNitroTexData(),
                texture.GetPalName(), texture.GetNitroPalette(), width, height, texture.GetColor0Mode(),
                texture.m_Format);
            m_WorkingTexturesCopy[textureID] = texture;

            dimensionOtherComboBox.SelectedIndex = dimensionOtherComboBox.Items.IndexOf(dimensionOther);
            
            DrawTexturePreview(textureID);
            UpdateBMDModelAndPreview();
        }

        private void cmbModelTexturesWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModelTexturesWidth.SelectedIndex < 0) return;

            UpdateTextureDimensions(GetSelectedTextureID(), cmbModelTexturesWidth, cmbModelTexturesHeight, true);
        }

        private void cmbModelTexturesHeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModelTexturesHeight.SelectedIndex < 0) return;

            UpdateTextureDimensions(GetSelectedTextureID(), cmbModelTexturesHeight, cmbModelTexturesWidth, false);
        }

        private bool GetValidNonAsciiTextBoxValue(TextBox textBox, ref string value)
        {
            value = textBox.Text;
            if (value == null ||
                (value = ASCII_PRINTABLE_NO_SPACE.Replace(value, "")).Length < 1)
            {
                textBox.BackColor = Helper.LIGHT_RED;
                textBox.Text = value;
                return false;
            }
            textBox.BackColor = Color.White;
            return true;
        }

        private void btnModelTexturesRenameTexture_Click(object sender, EventArgs e)
        {
            string textureIDOld = GetSelectedTextureID();
            if (textureIDOld == null) return;

            string textureIDNew = null;
            if (!GetValidNonAsciiTextBoxValue(txtModelTexturesName, ref textureIDNew)) return;

            if (m_WorkingTexturesCopy.ContainsKey(textureIDNew))
            {
                MessageBox.Show("Texture names must be unique");
                return;
            }

            ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureIDOld];
            texture = new ModelBase.TextureDefNitro(textureIDNew, texture.GetNitroTexData(),
                        texture.GetPalName(), texture.GetNitroPalette(), texture.GetWidth(), texture.GetHeight(),
                        texture.GetColor0Mode(), texture.m_Format);
            m_WorkingTexturesCopy[textureIDNew] = texture;
            m_WorkingTexturesCopy.Remove(textureIDOld);

            foreach (ModelBase.MaterialDef material in m_ModelBase.m_Materials.Values)
            {
                if (textureIDOld.Equals(material.m_TextureDefID))
                {
                    material.m_TextureDefID = textureIDNew;
                }
            }

            PopulateTextureAndPaletteLists();

            txtModelTexturesName.Text = textureIDNew;

            lbxModelTextures.SelectedIndex = lbxModelTextures.Items.IndexOf(textureIDNew);
        }

        private void btnModelTexturesRemoveTexture_Click(object sender, EventArgs e)
        {
            string textureID = GetSelectedTextureID();
            if (textureID == null) return;

            ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureID];
            if (texture.HasNitroPalette())
            {
                m_WorkingPalettesCopy.Remove(texture.GetPalName());
            }
            m_WorkingTexturesCopy.Remove(textureID);

            foreach (ModelBase.MaterialDef material in m_ModelBase.m_Materials.Values)
            {
                if (textureID.Equals(material.m_TextureDefID))
                {
                    material.m_TextureDefID = null;
                }
            }

            PopulateMaterialsList();
            PopulateTextureAndPaletteLists();
            UpdateBMDModelAndPreview();
        }

        private ModelBase.TextureDefNitro LoadTextureDefNitroFromExternalImage()
        {
            m_OpenFileDialogue.Title = "Select an image";
            m_OpenFileDialogue.Filter = null;
            if (m_OpenFileDialogue.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = m_OpenFileDialogue.FileName;
                string temporaryID = System.IO.Path.GetFileName(fileName);
                ModelBase.TextureDefExternalBitmap textureDefExternalBitmap =
                    new ModelBase.TextureDefExternalBitmap(temporaryID, fileName);

                textureDefExternalBitmap.m_ID = textureDefExternalBitmap.GetTexName();

                NitroTexture nitroTexture = BMDWriter.ConvertTexture(0, 0, textureDefExternalBitmap,
                    m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting, false);

                ModelBase.TextureDefNitro textureDefNitro = new ModelBase.TextureDefNitro(nitroTexture);

                return textureDefNitro;
            }
            return null;
        }

        private void btnModelTexturesAddTexture_Click(object sender, EventArgs e)
        {
            string fileName = m_OpenFileDialogue.FileName;
            try
            {
                ModelBase.TextureDefNitro textureDefNitro = LoadTextureDefNitroFromExternalImage();

                if (textureDefNitro == null) return;

                m_WorkingTexturesCopy.Add(textureDefNitro.GetTexName(), textureDefNitro);
                if (textureDefNitro.HasNitroPalette())
                {
                    m_WorkingPalettesCopy.Add(textureDefNitro.GetPalName(), textureDefNitro.GetNitroPalette());
                }

                PopulateTextureAndPaletteLists();
                UpdateBMDModelAndPreview();
                lbxModelTextures.SelectedIndex = lbxModelTextures.Items.IndexOf(textureDefNitro.GetTexName());
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("File not found: " + fileName);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Error loading image: " + ex.Message);
                return;
            }
        }

        private void btnModelTexturesReplace_Click(object sender, EventArgs e)
        {
            string textureID = GetSelectedTextureID();
            if (textureID == null) return;

            try
            {
                ModelBase.TextureDefNitro textureImported = LoadTextureDefNitroFromExternalImage();

                if (textureImported == null) return;

                ModelBase.TextureDefNitro textureCurrent = m_WorkingTexturesCopy[textureID];
                ModelBase.TextureDefNitro textureImportedCopiedNames = new ModelBase.TextureDefNitro(
                    textureCurrent.GetTexName(), textureImported.GetNitroTexData(), textureCurrent.GetPalName(),
                    textureImported.GetNitroPalette(), textureImported.GetWidth(), textureImported.GetHeight(),
                    textureImported.GetColor0Mode(), textureImported.m_Format);

                m_WorkingTexturesCopy[textureID] = textureImportedCopiedNames;
                m_WorkingPalettesCopy[textureImportedCopiedNames.GetPalName()] = textureImportedCopiedNames.GetNitroPalette();

                PopulateTextureAndPaletteLists();

                UpdateBMDModelAndPreview();

                lbxModelTextures.SelectedIndex = lbxModelTextures.Items.IndexOf(textureID);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("File not found: " + ex.FileName);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Error loading image: " + ex.Message);
                return;
            }
        }

        private void lbxModelPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxModelPalettes.SelectedIndex < 0) return;

            string paletteID = GetSelectedPaletteID();
            PopulatePaletteSettings(paletteID);
        }

        private void gridModelPalettesPaletteColours_CurrentCellChanged(object sender, System.EventArgs e)
        {
            if (!gridModelPalettesPaletteColours.IsAColourSelected())
            {
                ResetColourButtonValue(btnModelPalettesSelectedColour);
                return;
            }

            Color colour = (Color)gridModelPalettesPaletteColours.GetSelectedColour();
            SetColourButtonValue(btnModelPalettesSelectedColour, colour);
        }

        private void btnModelPalettesSelectedColour_Click(object sender, EventArgs e)
        {
            if (!gridModelPalettesPaletteColours.IsAColourSelected()) return;

            string paletteID = GetSelectedPaletteID();

            Color? selectedColour = GetColourDialogueResult(btnModelPalettesSelectedColour);
            if (selectedColour == null) return;

            ushort colourBGR15 = Helper.ColorToBGR15((Color)selectedColour);
            int selectedColourIndex = gridModelPalettesPaletteColours.GetSelectedColourIndex();

            byte[] palette = m_WorkingPalettesCopy[paletteID];
            DataHelper.Write16(palette, (uint)(selectedColourIndex * 2), colourBGR15);
            foreach (string textureID in m_WorkingTexturesCopy.Keys.ToList())
            {
                ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureID];
                if (paletteID.Equals(texture.GetPalName()))
                {
                    texture = new ModelBase.TextureDefNitro(texture.GetTexName(), texture.GetNitroTexData(),
                        paletteID, palette, texture.GetWidth(), texture.GetHeight(),
                        texture.GetColor0Mode(), texture.m_Format);
                    m_WorkingTexturesCopy[textureID] = texture;
                }
            }

            gridModelPalettesPaletteColours.SetColourAtIndex((Color)selectedColour, selectedColourIndex);

            UpdateBMDModelAndPreview();
        }

        private void btnModelPalettesRenamePalette_Click(object sender, EventArgs e)
        {
            string paletteIDOld = GetSelectedPaletteID();
            if (paletteIDOld == null) return;

            string paletteIDNew = null;
            if (!GetValidNonAsciiTextBoxValue(txtModelPalettesName, ref paletteIDNew)) return;

            if (m_WorkingPalettesCopy.ContainsKey(paletteIDNew))
            {
                MessageBox.Show("Palette names must be unique");
                return;
            }

            byte[] palette = m_WorkingPalettesCopy[paletteIDOld];

            m_WorkingPalettesCopy[paletteIDNew] = palette;
            m_WorkingPalettesCopy.Remove(paletteIDOld);

            foreach (string textureID in m_WorkingTexturesCopy.Keys.ToList())
            {
                ModelBase.TextureDefNitro texture = m_WorkingTexturesCopy[textureID];
                if (paletteIDOld.Equals(texture.GetPalName()))
                {
                    texture = new ModelBase.TextureDefNitro(texture.GetTexName(), texture.GetNitroTexData(),
                        paletteIDNew, palette, texture.GetWidth(), texture.GetHeight(),
                        texture.GetColor0Mode(), texture.m_Format);
                    m_WorkingTexturesCopy[textureID] = texture;
                }
            }

            PopulateTextureAndPaletteLists();
        }

        private void ResetGeometryDuplicationAndMovementState()
        {
            m_CopyMode = CopyMode.None;
            m_CopySourceType = CopySourceType.None;
            m_SourceBone = null;
            m_SourceGeometries = null;
            m_SourcePolylists = null;
            m_TargetGeometry = null;
            m_TargetBone = null;

            btnModelBonesPasteToGeometry.Enabled = false;
            btnModelBonesPasteToBone.Enabled = false;
        }

        private void StoreGeometryForCopying(CopyMode copyMode)
        {
            ResetGeometryDuplicationAndMovementState();
            m_SourceBone = GetSelectedBone();
            m_SourceGeometries = GetSelectedGeometries();
            m_TargetBone = m_SourceBone;
            m_CopyMode = copyMode;
            m_CopySourceType = CopySourceType.Geometry;

            btnModelBonesPasteToBone.Enabled = (!CopyMode.None.Equals(m_CopyMode));
        }

        private void StorePolyListsForCopying(CopyMode copyMode)
        {
            StoreGeometryForCopying(copyMode);
            m_SourcePolylists = GetSelectedPolyLists();
            m_CopySourceType = CopySourceType.PolyList;

            btnModelBonesPasteToGeometry.Enabled = (!CopyMode.None.Equals(m_CopyMode));
        }

        private void RemovePolyListsForCopying(List<ModelBase.PolyListDef> polyLists)
        {
            if (CopySourceType.PolyList.Equals(m_CopySourceType) && polyLists != null)
            {
                foreach (ModelBase.PolyListDef polyList in polyLists)
                {
                    m_SourcePolylists.Remove(polyList);
                }
                if (m_SourcePolylists.Count < 1)
                {
                    ResetGeometryDuplicationAndMovementState();
                }
            }
        }

        private void RemoveGeometriesForCopying(List<ModelBase.GeometryDef> geometries)
        {
            if (CopySourceType.Geometry.Equals(m_CopySourceType) && geometries != null)
            {
                foreach (ModelBase.GeometryDef geometry in geometries)
                {
                    m_SourceGeometries.Remove(geometry);
                }
                if (m_SourceGeometries.Count < 1)
                {
                    ResetGeometryDuplicationAndMovementState();
                }
            }
        }

        private void ResetGeometryDuplicationAndMovementSource()
        {
            m_CopyMode = CopyMode.None;
            m_SourceBone = null;
            m_SourceGeometries = new List<ModelBase.GeometryDef>();
            m_SourcePolylists = new List<ModelBase.PolyListDef>();
        }

        private void btnModelBonesCopyGeometry_Click(object sender, EventArgs e)
        {
            StoreGeometryForCopying(CopyMode.Copy);
        }

        private void btnModelBonesCutGeometry_Click(object sender, EventArgs e)
        {
            StoreGeometryForCopying(CopyMode.Cut);
        }

        private void btnModelBonesCopyPolylist_Click(object sender, EventArgs e)
        {
            StorePolyListsForCopying(CopyMode.Copy);
        }

        private void btnModelBonesCutPolylist_Click(object sender, EventArgs e)
        {
            StorePolyListsForCopying(CopyMode.Cut);
        }

        private ModelBase.PolyListDef DuplicatePolylist(ModelBase.PolyListDef sourcePolyList)
        {
            ModelBase.PolyListDef polyList = new ModelBase.PolyListDef(sourcePolyList.m_ID, sourcePolyList.m_MaterialName);
            foreach (ModelBase.FaceListDef sourceFaceList in sourcePolyList.m_FaceLists)
            {
                ModelBase.FaceListDef faceList = new ModelBase.FaceListDef(sourceFaceList.m_Type);
                foreach (ModelBase.FaceDef sourceFace in sourceFaceList.m_Faces)
                {
                    ModelBase.FaceDef face = new ModelBase.FaceDef(sourceFace.m_NumVertices);
                    for (int i = 0; i < sourceFace.m_NumVertices; i++)
                    {
                        face.m_Vertices[i] = sourceFace.m_Vertices[i];
                    }
                    faceList.m_Faces.Add(face);
                }
                polyList.m_FaceLists.Add(faceList);
            }

            return polyList;
        }

        private ModelBase.GeometryDef DuplicateGeometry(ModelBase.GeometryDef sourceGeometry)
        {
            ModelBase.GeometryDef geometry = new ModelBase.GeometryDef(sourceGeometry.m_ID);
            foreach (KeyValuePair<string, ModelBase.PolyListDef> polyListEntry in sourceGeometry.m_PolyLists)
            {
                geometry.m_PolyLists[polyListEntry.Key] = DuplicatePolylist(polyListEntry.Value);
            }

            return geometry;
        }

        private void AddPolyListToBone(ModelBase.BoneDef targetBone, ModelBase.GeometryDef targetGeometry,
            ModelBase.PolyListDef sourcePolyList)
        {
            string polyListIDOriginal = sourcePolyList.m_ID;
            string polyListIDTarget;
            int counter = 0;

            do
            {
                polyListIDTarget = polyListIDOriginal + '_' + counter++;
            }
            while (targetGeometry.m_PolyLists.ContainsKey(polyListIDTarget) && counter < 100);

            if (counter >= 100)
            {
                polyListIDTarget = polyListIDOriginal + '_' + System.Guid.NewGuid().ToString();
            }

            sourcePolyList.m_ID = polyListIDTarget;

            targetGeometry.m_PolyLists.Add(polyListIDTarget, sourcePolyList);

            if (!targetBone.m_MaterialsInBranch.Contains(sourcePolyList.m_MaterialName))
            {
                targetBone.m_MaterialsInBranch.Add(sourcePolyList.m_MaterialName);
            }
        }

        private void AddGeometryToBone(ModelBase.BoneDef targetBone, ModelBase.GeometryDef sourceGeometry)
        {
            string geometryIDOriginal = sourceGeometry.m_ID;
            string geometryIDTarget;
            int counter = 0;

            do
            {
                geometryIDTarget = geometryIDOriginal + '_' + counter++;
            }
            while (targetBone.m_Geometries.ContainsKey(geometryIDTarget) && counter < 100);

            if (counter >= 100)
            {
                geometryIDTarget = geometryIDOriginal + '_' + System.Guid.NewGuid().ToString();
            }

            sourceGeometry.m_ID = geometryIDTarget;

            targetBone.m_Geometries.Add(geometryIDTarget, sourceGeometry);

            foreach (ModelBase.PolyListDef polyList in sourceGeometry.m_PolyLists.Values)
            {
                if (!targetBone.m_MaterialsInBranch.Contains(polyList.m_MaterialName))
                {
                    targetBone.m_MaterialsInBranch.Add(polyList.m_MaterialName);
                }
            }
        }

        private void AssignPolyListVerticesToBone(ModelBase.PolyListDef polyList, ModelBase.BoneDef bone)
        {
            int boneTransformID = m_ModelBase.m_BoneTransformsMap.GetByFirst(bone.m_ID);
            foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
            {
                foreach (ModelBase.FaceDef face in faceList.m_Faces)
                {
                    for (int i = 0; i < face.m_NumVertices; i++)
                    {
                        face.m_Vertices[i].m_VertexBoneIndex = boneTransformID;
                    }
                }
            }
        }

        private void AssignGeometryVerticesToBone(ModelBase.GeometryDef geometry, ModelBase.BoneDef bone)
        {
            foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
            {
                AssignPolyListVerticesToBone(polyList, bone);
            }
        }

        private void RemoveMaterialFromBoneBranchIfNecessary(ModelBase.BoneDef sourceBone, string materialName)
        {
            bool materialUseFound = false;
            foreach (ModelBase.GeometryDef geometryDef in sourceBone.m_Geometries.Values)
            {
                foreach (ModelBase.PolyListDef polyListDef in geometryDef.m_PolyLists.Values)
                {
                    if (materialName.Equals(polyListDef.m_MaterialName))
                    {
                        materialUseFound = true;
                        break;
                    }
                }
            }
            if (!materialUseFound)
            {
                sourceBone.m_MaterialsInBranch.Remove(materialName);
            }
        }

        private void RemovePolyListFromBone(ModelBase.BoneDef sourceBone, ModelBase.GeometryDef sourceGeometry,
            ModelBase.PolyListDef polyList, string polyListIDOriginal)
        {
            sourceGeometry.m_PolyLists.Remove(polyListIDOriginal);
            RemoveMaterialFromBoneBranchIfNecessary(sourceBone, polyList.m_MaterialName);
        }

        private void RemoveGeometryFromBone(ModelBase.BoneDef sourceBone, ModelBase.GeometryDef geometry, 
            string geometryIDOriginal)
        {
            sourceBone.m_Geometries.Remove(geometryIDOriginal);
            foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
            {
                RemoveMaterialFromBoneBranchIfNecessary(sourceBone, polyList.m_MaterialName);
            }
        }

        private void CopyOrMoveSourcePolyListsToTargetGeometry(bool preserveWeights)
        {
            if (CopySourceType.PolyList.Equals(m_CopySourceType))
            {
                if (m_SourceBone != null && m_SourceGeometries != null)
                {
                    if (m_TargetGeometry == null)
                    {
                        m_TargetGeometry = new ModelBase.GeometryDef(System.Guid.NewGuid().ToString());
                        m_TargetBone.m_Geometries.Add(m_TargetGeometry.m_ID, m_TargetGeometry);
                    }

                    ModelBase.GeometryDef sourceGeometry = m_SourceGeometries[0];

                    foreach (ModelBase.PolyListDef sourcePolyList in m_SourcePolylists)
                    {
                        ModelBase.PolyListDef polyList = (CopyMode.Copy.Equals(m_CopyMode)) ?
                            DuplicatePolylist(sourcePolyList) : sourcePolyList;
                        string polyListIDOriginal = polyList.m_ID;

                        if (!preserveWeights)
                        {
                            AssignPolyListVerticesToBone(polyList, m_TargetBone);
                        }
                        AddPolyListToBone(m_TargetBone, m_TargetGeometry, polyList);

                        if (CopyMode.Cut.Equals(m_CopyMode))
                        {
                            RemovePolyListFromBone(m_SourceBone, sourceGeometry, polyList, polyListIDOriginal);
                        }
                    }

                    if (CopyMode.Cut.Equals(m_CopyMode))
                    {
                        ResetGeometryDuplicationAndMovementSource();
                    }

                    PopulateBoneGeometryList(m_TargetBone);
                    SetListBoxSelectedItemByName(lbxModelBonesGeometries, m_TargetGeometry.m_ID);
                    PopulateBoneGeometryPolyListList();
                }
            }
        }

        private void CopyOrMoveSourceGeometriesToTargetBone(bool preserveWeights)
        {
            if (CopySourceType.Geometry.Equals(m_CopySourceType))
            {
                if (m_SourceBone != null && m_SourceGeometries != null)
                {
                    if (m_TargetBone == null) return;

                    foreach (ModelBase.GeometryDef sourceGeometry in m_SourceGeometries)
                    {
                        ModelBase.GeometryDef geometry = (CopyMode.Copy.Equals(m_CopyMode)) ?
                            DuplicateGeometry(sourceGeometry) : sourceGeometry;
                        string geometryIDOriginal = geometry.m_ID;

                        if (!preserveWeights)
                        {
                            AssignGeometryVerticesToBone(geometry, m_TargetBone);
                        }
                        AddGeometryToBone(m_TargetBone, geometry);

                        if (CopyMode.Cut.Equals(m_CopyMode))
                        {
                            RemoveGeometryFromBone(m_SourceBone, geometry, geometryIDOriginal);
                        }
                    }

                    if (CopyMode.Cut.Equals(m_CopyMode))
                    {
                        ResetGeometryDuplicationAndMovementSource();
                    }

                    PopulateBoneGeometryList(m_TargetBone);
                }
            }
        }

        private void mnitModelBonesPasteToGeometryPreserveWeights_Click(object sender, EventArgs e)
        {
            CopyOrMoveSourcePolyListsToTargetGeometry(true);
        }

        private void mnitModelBonesPasteToGeometryAssignWeightsToTarget_Click(object sender, EventArgs e)
        {
            CopyOrMoveSourcePolyListsToTargetGeometry(false);
        }

        private void mnitModelBonesPasteToBonePreserveWeights_Click(object sender, EventArgs e)
        {
            switch (m_CopySourceType)
            {
                case CopySourceType.PolyList:
                    if (m_TargetBone != null)
                    {
                        CopyOrMoveSourcePolyListsToTargetGeometry(true);
                    }
                    break;
                case CopySourceType.Geometry:
                    if (m_TargetBone != null)
                    {
                        CopyOrMoveSourceGeometriesToTargetBone(true);
                    }
                    break;
                default:
                case CopySourceType.None:
                    return;
            }
            UpdateBMDModelAndPreview();
        }

        private void mnitModelBonesPasteToBoneAssignWeightsToTarget_Click(object sender, EventArgs e)
        {
            switch (m_CopySourceType)
            {
                case CopySourceType.PolyList:
                    if (m_TargetBone != null)
                    {
                        CopyOrMoveSourcePolyListsToTargetGeometry(false);
                    }
                    break;
                case CopySourceType.Geometry:
                    if (m_TargetBone != null)
                    {
                        CopyOrMoveSourceGeometriesToTargetBone(false);
                    }
                    break;
                default:
                case CopySourceType.None:
                    return;
            }
            UpdateBMDModelAndPreview();
        }

        private void btnModelBonesRemovePolyList_Click(object sender, EventArgs e)
        {
            List<ModelBase.PolyListDef> polyLists = GetSelectedPolyLists();
            if (polyLists.Count < 1) return;

            int selectedIndex = lbxModelBonesPolylists.SelectedIndex;

            ModelBase.BoneDef bone = GetSelectedBone();
            ModelBase.GeometryDef geometry = GetSelectedGeometry();

            foreach (ModelBase.PolyListDef polyList in polyLists)
            {
                RemovePolyListFromBone(bone, geometry, polyList, polyList.m_ID);
            }

            PopulateBoneGeometryPolyListList();

            RemovePolyListsForCopying(polyLists);

            SetListBoxSelectedItemByName(lbxModelBonesGeometries, geometry.m_ID);

            lbxModelBonesPolylists.SelectedIndex = selectedIndex - 1;

            UpdateBMDModelAndPreview();
        }

        private void cmbModelBonesPolylistMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<ModelBase.PolyListDef> polyLists = GetSelectedPolyLists();
            if (polyLists.Count < 1) return;

            ModelBase.BoneDef bone = GetSelectedBone();

            string material = GetComboBoxSelectedItemName(cmbModelBonesPolylistMaterial);
            if (material == null) return;

            List<string> originalMaterials = new List<string>();

            foreach (ModelBase.PolyListDef polyList in polyLists)
            {
                if (!originalMaterials.Contains(polyList.m_MaterialName))
                {
                    originalMaterials.Add(polyList.m_MaterialName);
                }
                polyList.m_MaterialName = material;
            }

            foreach (string originalMaterial in originalMaterials)
            {
                RemoveMaterialFromBoneBranchIfNecessary(bone, originalMaterial);
            }
        }
        
        private void btnModelBonesRemoveGeometry_Click(object sender, EventArgs e)
        {
            List<ModelBase.GeometryDef> geometries = GetSelectedGeometries();
            if (geometries.Count < 1) return;

            ModelBase.BoneDef bone = GetSelectedBone();

            foreach (ModelBase.GeometryDef geometry in geometries)
            {
                RemoveGeometryFromBone(bone, geometry, geometry.m_ID);
            }

            PopulateBoneGeometryList(bone);

            RemoveGeometriesForCopying(geometries);

            UpdateBMDModelAndPreview();
        }

        private void btnModelBonesRenameBone_Click(object sender, EventArgs e)
        {
            ModelBase.BoneDef bone = GetSelectedBone();
            if (bone == null) return;

            string boneIDNew = null;
            if (!GetValidNonAsciiTextBoxValue(txtModelBonesName, ref boneIDNew)) return;

            string boneIDOld = bone.m_ID;
            
            bone.m_ID = boneIDNew;

            int boneTransform = m_ModelBase.m_BoneTransformsMap.GetByFirst(boneIDOld);
            m_ModelBase.m_BoneTransformsMap.RemoveByFirst(boneIDOld);
            m_ModelBase.m_BoneTransformsMap.Add(boneIDNew, boneTransform);

            tvModelBonesBones.SelectedNode.Text = boneIDNew;
        }

        private void chkModelBonesSettingsBillboard_CheckedChanged(object sender, System.EventArgs e)
        {
            ModelBase.BoneDef bone = GetSelectedBone();
            if (bone == null) return;

            bone.m_Billboard = chkModelBonesSettingsBillboard.Checked;
        }

        private string GenerateUniqueBoneID()
        {
            string boneID;
            int counter = 0;

            do
            {
                boneID = "bone_" + counter;
            }
            while (m_ModelBase.m_BoneTree.GetBoneByID(boneID) != null && counter < 100);

            if (counter >= 100)
            {
                boneID = "bone_" + System.Guid.NewGuid().ToString();
            }
            return boneID;
        }

        private void UpdateVertexBoneIndicesForBoneInsertionAt(int index)
        {
            // If for example a model has three bones, A -> B -> C with indices 0, 1, 2 and 
            // a fourth bone is added such that the structure becomes A -> B -> D -> C meaning 
            // that the indices become 0, 1, 2, 3 then any vertex assigned to C will need its 
            // bone index updated from 2 to 3.
            foreach (ModelBase.BoneDef bone in m_ModelBase.m_BoneTree)
            {
                foreach (ModelBase.GeometryDef geometry in bone.m_Geometries.Values)
                {
                    foreach (ModelBase.PolyListDef polyList in geometry.m_PolyLists.Values)
                    {
                        foreach (ModelBase.FaceListDef faceList in polyList.m_FaceLists)
                        {
                            foreach (ModelBase.FaceDef face in faceList.m_Faces)
                            {
                                for (int i = 0; i < face.m_NumVertices; i++)
                                {
                                    if (face.m_Vertices[i].m_VertexBoneIndex >= index)
                                    {
                                        face.m_Vertices[i].m_VertexBoneIndex += 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void mnitModelBonesAddBoneChild_Click(object sender, EventArgs e)
        {
            ModelBase.BoneDef selectedBone = GetSelectedBone();
            if (selectedBone == null) return;

            string boneID = GenerateUniqueBoneID();

            ModelBase.BoneDef childBone = new ModelBase.BoneDef(boneID);

            selectedBone.AddChild(childBone);
            m_ModelBase.m_BoneTransformsMap.Add(childBone.m_ID, m_ModelBase.m_BoneTransformsMap.Count);

            int childBoneIndex = m_ModelBase.m_BoneTree.GetBoneIndex(childBone);
            UpdateVertexBoneIndicesForBoneInsertionAt(childBoneIndex);

            PopulateBoneTree();
            UpdateBMDModelAndPreview();
        }
        
        private void mnitModelBonesAddBoneSibling_Click(object sender, EventArgs e)
        {
            ModelBase.BoneDef selectedBone = GetSelectedBone();
            if (selectedBone == null) return;

            ModelBase.BoneDef parentBone = selectedBone.m_Parent;

            string boneID = GenerateUniqueBoneID();

            ModelBase.BoneDef siblingBone = new ModelBase.BoneDef(boneID);

            if (parentBone != null)
            {
                parentBone.AddChild(siblingBone);
            }
            else
            {
                m_ModelBase.m_BoneTree.AddRootBone(siblingBone);
            }
            m_ModelBase.m_BoneTransformsMap.Add(siblingBone.m_ID, m_ModelBase.m_BoneTransformsMap.Count);

            int siblingBoneIndex = m_ModelBase.m_BoneTree.GetBoneIndex(siblingBone);
            UpdateVertexBoneIndicesForBoneInsertionAt(siblingBoneIndex);

            PopulateBoneTree();
            UpdateBMDModelAndPreview();
        }

        private void chkModelGeneralStripify_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_ConvertToTriangleStrips = 
                chkModelGeneralStripify.Checked;
            chkModelGeneralKeepVertexOrderDuringStripping.Enabled = chkModelGeneralStripify.Checked;
        }

        private void chkModelGeneralKeepVertexOrderDuringStripping_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_KeepVertexOrderDuringStripping = 
                chkModelGeneralKeepVertexOrderDuringStripping.Checked;
        }

        private void chkModelGeneralAlwaysWriteFullVertexCmd23h_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_AlwaysWriteFullVertexCmd23h =
                chkModelGeneralAlwaysWriteFullVertexCmd23h.Checked;
        }

        private void rbModelGeneralTextureAlwaysCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting = 
                BMDImporter.BMDExtraImportOptions.TextureQualitySetting.SmallestSize;
        }

        private void rbModelGeneralTextureBetterQualityWhereSensible_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting = 
                BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BetterQualityWhereSensible;
        }

        private void rbModelGeneralTextureNeverCompress_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_TextureQualitySetting = 
                BMDImporter.BMDExtraImportOptions.TextureQualitySetting.BestQuality;
        }

        private void chkModelGeneralVFlipAllTextures_CheckedChanged(object sender, EventArgs e)
        {
            m_ModelImportationSettings.m_ExtraOptions.m_VerticallyFlipAllTextures =
                chkModelGeneralVFlipAllTextures.Checked;
        }

        private void txtModelGeneralScale_TextChanged(object sender, EventArgs e)
        {
            if (Helper.TryParseFloat(txtModelGeneralScale, out m_ModelImportationSettings.m_Scale))
            {
                RefreshBMDScale();
            }
        }

        private void txtModelPreviewScale_TextChanged(object sender, EventArgs e)
        {
            if (Helper.TryParseFloat(txtModelPreviewScale, out m_ModelImportationSettings.m_InGamePreviewScale))
            {
                RefreshBMDScale();
            }
        }

        private void btnCollisionMapGeneralSelectTarget_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a collision map (KCL) file to replace", new String[] { ".kcl" });
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                m_KCLTargetName = m_ROMFileSelect.m_SelectedFile;
                txtCollisionMapGeneralTargetName.Text = m_KCLTargetName;
                UpdateEnabledStateMenuControls();
            }
        }

        private void SetEnabledStateCollisionMapMaterialCollisionTypes(bool state)
        {
            gridCollisionMapGeneralMaterialCollisionTypes.Enabled = state;
            btnCollisionMapGeneralAssignMaterialCollisionTypes.Enabled = state;
            btnCollisionMapGeneralResetMaterialCollisionTypes.Enabled = state;
        }

        private void SetEnabledStateCollisionMapPlanes(bool state)
        {
            lbxCollisionMapPlanes.Enabled = state;
            txtCollisionMapPlanesCollisionType.Enabled = state;
        }

        private void UpdateKCLMapAndPreview(float scale, bool callKCLImporter = true, bool preserveIndividualFaceCollisionTypes = true)
        {
            PopulateCollisionMapMaterialCollisionTypes();
            if (callKCLImporter)
            {
                m_ImportedCollisionMap = CallKCLImporter(scale, preserveIndividualFaceCollisionTypes);
            }
            PopulateCollisionMapPlanes();

            RefreshCollisionMapView();
        }

        private void UpdateKCLMapAndPreview(bool callKCLImporter = true, bool preserveIndividualFaceCollisionTypes = true)
        {
            UpdateKCLMapAndPreview(m_CollisionMapImportationSettings.GetImportationScale(),
                callKCLImporter, preserveIndividualFaceCollisionTypes);
        }

        private void RefreshCollisionMapView()
        {
            PrerenderCollisionMap();
            RenderCollisionMapPlaneHighlight();

            glCollisionMapView.Refresh();
        }

        private void DrawCollisionMapPlane(KCL.ColFace plane, PrimitiveType primitiveType, Color colour)
        {
            GL.Begin(primitiveType);
            GL.Color4(colour);
            GL.Vertex3(plane.point1);
            GL.Vertex3(plane.point2);
            GL.Vertex3(plane.point3);
            GL.End();
        }

        private void PrerenderCollisionMap()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            m_KCLPickingDisplayLists[0] = GL.GenLists(1);
            GL.NewList(m_KCLPickingDisplayLists[0], ListMode.Compile);
            GL.PushMatrix();
            GL.Scale(new Vector3(m_CollisionMapPreviewScale));
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < m_CollisionMapPlanes.Count; i++)
            {
                DrawCollisionMapPlane(m_CollisionMapPlanes[i], PrimitiveType.Triangles, Color.FromArgb(i));
            }
            GL.PopMatrix();
            GL.EndList();

            m_KCLMeshDisplayLists[0] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDisplayLists[0], ListMode.Compile);
            GL.PushMatrix();
            GL.Scale(new Vector3(m_CollisionMapPreviewScale));
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1f, 1f);
            for (int i = 0; i < m_CollisionMapPlanes.Count; i++)
            {
                DrawCollisionMapPlane(m_CollisionMapPlanes[i], PrimitiveType.Triangles, m_CollisionMapColours[m_CollisionMapPlanes[i].type]);
            }
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.PopMatrix();
            GL.EndList();

            m_KCLMeshDisplayLists[1] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDisplayLists[1], ListMode.Compile);
            GL.PushMatrix();
            GL.Scale(new Vector3(m_CollisionMapPreviewScale));
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            for (int i = 0; i < m_CollisionMapPlanes.Count; i++)
            {
                DrawCollisionMapPlane(m_CollisionMapPlanes[i], PrimitiveType.LineLoop, Color.FromArgb(255, Color.Orange));
            }
            GL.PopMatrix();
            GL.EndList();

            GL.PopAttrib();
        }

        private void RenderCollisionMapPlaneHighlight()
        {
            m_KCLMeshDisplayLists[2] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDisplayLists[2], ListMode.Compile);
            foreach (int idx in lbxCollisionMapPlanes.SelectedIndices)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.RoyalBlue);
                GL.Vertex3(m_CollisionMapPlanes[idx].point1);
                GL.Vertex3(m_CollisionMapPlanes[idx].point2);
                GL.Vertex3(m_CollisionMapPlanes[idx].point3);
                GL.End();
            }
            GL.EndList();
        }

        private void PopulateCollisionMapPreviewFillModeOptions()
        {
            cmbCollisionMapPreviewFillMode.Items.Add("Fill");
            cmbCollisionMapPreviewFillMode.Items.Add("Wireframe");
            cmbCollisionMapPreviewFillMode.SelectedIndex = 0;
        }

        private void PopulateCollisionMapMaterialCollisionTypes()
        {
            if (m_ModelBase != null)
            {
                foreach (string material in m_CollisionMapMaterialTypeMap.Keys.ToList())
                {
                    if (!m_ModelBase.m_Materials.ContainsKey(material))
                    {
                        m_CollisionMapMaterialTypeMap.Remove(material);
                    }
                }
                foreach (string material in m_ModelBase.m_Materials.Keys)
                {
                    if (!m_CollisionMapMaterialTypeMap.ContainsKey(material))
                    {
                        m_CollisionMapMaterialTypeMap.Add(material, 0);
                    }
                }
            }

            gridCollisionMapGeneralMaterialCollisionTypes.ColumnCount = 2;
            gridCollisionMapGeneralMaterialCollisionTypes.Columns[0].HeaderText = "Material";
            gridCollisionMapGeneralMaterialCollisionTypes.Columns[0].ReadOnly = true;
            gridCollisionMapGeneralMaterialCollisionTypes.Columns[1].HeaderText = "Col. Type";

            int numMats = m_CollisionMapMaterialTypeMap.Count;
            gridCollisionMapGeneralMaterialCollisionTypes.RowCount = numMats;
            for (int i = 0; i < numMats; i++)
            {
                gridCollisionMapGeneralMaterialCollisionTypes.Rows[i].Cells[0].Value = 
                    m_CollisionMapMaterialTypeMap.Keys.ElementAt(i);
                gridCollisionMapGeneralMaterialCollisionTypes.Rows[i].Cells[1].Value = 
                    m_CollisionMapMaterialTypeMap.Values.ElementAt(i);
            }

            SetEnabledStateCollisionMapMaterialCollisionTypes(true);
        }

        private void PopulateCollisionMapPlanes()
        {
            m_CollisionMapPlanes = m_ImportedCollisionMap.m_Planes;

            lbxCollisionMapPlanes.Items.Clear();

            for (int i = 0; i < m_CollisionMapPlanes.Count; i++)
            {
                lbxCollisionMapPlanes.Items.Add("Plane " + i.ToString("00000"));
            }

            txtCollisionMapPlanesCollisionType.BackColor = Color.White;

            SetEnabledStateCollisionMapPlanes(true);
        }

        private void glCollisionMapView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point lastMouseClick = glCollisionMapView.GetLastMouseClick();
                if (Math.Abs(e.X - lastMouseClick.X) > 3 || Math.Abs(e.Y - lastMouseClick.Y) > 3) return;

                m_CollisionMapSelectedTriangle = (int)glCollisionMapView.GetColourUnderCursor();

                if (m_CollisionMapSelectedTriangle < 0) return;

                if (!lbxCollisionMapPlanes.SelectedIndices.Contains(m_CollisionMapSelectedTriangle))
                {
                    lbxCollisionMapPlanes.SelectedIndices.Add(m_CollisionMapSelectedTriangle);
                }
                else
                {
                    lbxCollisionMapPlanes.SelectedIndices.Remove(m_CollisionMapSelectedTriangle);
                }
            }
            else
            {
                m_CollisionMapSelectedTriangle = -1;
            }
        }

        private void cmbCollisionMapPreviewFillMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCollisionMapPreviewFillMode.SelectedIndex == 0)
                m_CollisionMapWireFrameView = false;
            else if (cmbCollisionMapPreviewFillMode.SelectedIndex == 1)
                m_CollisionMapWireFrameView = true;

            glCollisionMapView.Refresh();
        }

        private void btnCollisionMapGeneralAssignMaterialCollisionTypes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridCollisionMapGeneralMaterialCollisionTypes.RowCount; i++)
            {
                m_CollisionMapMaterialTypeMap[gridCollisionMapGeneralMaterialCollisionTypes.Rows[i].Cells[0].Value.ToString()] =
                    int.Parse(gridCollisionMapGeneralMaterialCollisionTypes.Rows[i].Cells[1].Value.ToString());
            }
            UpdateKCLMapAndPreview(true, false);
        }

        private void btnCollisionMapGeneralResetMaterialCollisionTypes_Click(object sender, EventArgs e)
        {
            m_CollisionMapMaterialTypeMap.Clear();
            PopulateCollisionMapMaterialCollisionTypes();
        }

        private void lbxCollisionMapPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxCollisionMapPlanes.SelectedIndices.Count == 1)
            {
                int selectedIndex = lbxCollisionMapPlanes.SelectedIndex;

                txtCollisionMapPlanesVertex1.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].point1);
                txtCollisionMapPlanesVertex2.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].point2);
                txtCollisionMapPlanesVertex3.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].point3);
                txtCollisionMapPlanesCollisionType.Text = m_CollisionMapPlanes[selectedIndex].type.ToString();
                txtCollisionMapPlanesNormal.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].normal);
                txtCollisionMapPlanesDirection1.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].dir1);
                txtCollisionMapPlanesDirection2.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].dir2);
                txtCollisionMapPlanesDirection3.Text = Helper.ToString(m_CollisionMapPlanes[selectedIndex].dir3);
            }
            else
            {
                txtCollisionMapPlanesVertex1.Text = null;
                txtCollisionMapPlanesVertex2.Text = null;
                txtCollisionMapPlanesVertex3.Text = null;
                txtCollisionMapPlanesCollisionType.Text = null;
                txtCollisionMapPlanesNormal.Text = null;
                txtCollisionMapPlanesDirection1.Text = null;
                txtCollisionMapPlanesDirection2.Text = null;
                txtCollisionMapPlanesDirection3.Text = null;
            }
            RefreshCollisionMapView();
        }

        private static void SetCollisionMapPlaneCollisionType(KCL kcl, int planeIndex, int collisionType)
        {
            if (planeIndex < 0 || planeIndex > kcl.m_Planes.Count) return;

            kcl.m_Planes[planeIndex].type = collisionType;

            NitroFile kclFile = kcl.m_File;

            uint planeStart = kclFile.Read32(8) + 0x10;

            // Get the address of this plane's Collision Type variable
            uint planeCollisionTypeOffset = (uint)(planeStart + (planeIndex * 0x10) + 0x0E);
            // Write the new value to file
            kclFile.Write16(planeCollisionTypeOffset, (ushort)collisionType);
        }

        private void SetCollisionMapPlaneCollisionType(int planeIndex, int collisionType)
        {
            SetCollisionMapPlaneCollisionType(m_ImportedCollisionMap, planeIndex, collisionType);
        }

        private void txtCollisionMapPlanesCollisionType_TextChanged(object sender, EventArgs e)
        {
            if (txtCollisionMapPlanesCollisionType.Text == null || txtCollisionMapPlanesCollisionType.Text.Trim().Length < 1) return;
            
            int collisionType = 0;
            if (Helper.TryParseInt(txtCollisionMapPlanesCollisionType, ref collisionType))
            {
                foreach (int idx in lbxCollisionMapPlanes.SelectedIndices)
                {
                    SetCollisionMapPlaneCollisionType(idx, collisionType);
                }

                RefreshCollisionMapView();
            }
        }

        private void txtCollisionMapGeneralFaceSizeThreshold_TextChanged(object sender, EventArgs e)
        {
            Helper.TryParseFloat(txtCollisionMapGeneralFaceSizeThreshold, out m_CollisionMapImportationSettings.m_MinimumFaceSize);
        }

        private void txtCollisionMapGeneralScale_TextChanged(object sender, EventArgs e)
        {
            if (Helper.TryParseFloat(txtCollisionMapGeneralScale, out m_CollisionMapImportationSettings.m_Scale))
            {
                RefreshKCLScale();
            }
        }

        private void txtCollisionMapGeneralTargetScale_TextChanged(object sender, EventArgs e)
        {
            if (Helper.TryParseFloat(txtCollisionMapGeneralTargetScale, out m_CollisionMapImportationSettings.m_InGameModelScale))
            {
                RefreshKCLScale();
            }
        }

        private void CopyNitroFileData(NitroFile sourceFile, NitroFile targetFile, bool save)
        {
            targetFile.Clear();
            targetFile.WriteBlock(0, sourceFile.m_Data);
            if (save)
            {
                targetFile.SaveChanges();
            }
        }

        private bool ImportModel()
        {
            if (IsBMDTargetSet())
            {                
                try
                {
                    m_ImportedModel = CallBMDImporter(false);

                    CopyNitroFileData(m_ImportedModel.m_File,
                        Program.m_ROM.GetFileFromName(m_BMDTargetName), true);

                    return true;
                }
                catch (Exception ex)
                {
                    new ExceptionMessageBox("Error importing model", ex).ShowDialog();
                    return false;
                }
            }
            return false;
        }

        private bool ImportCollisionMap()
        {
            if (IsKCLTargetSet())
            {
                try
                {
                    m_ImportedCollisionMap = CallKCLImporter(true);

                    NitroFile kclTargetFile = Program.m_ROM.GetFileFromName(m_KCLTargetName);
                    if (m_ImportedCollisionMap.m_File.m_ID != kclTargetFile.m_ID)
                    {
                        CopyNitroFileData(m_ImportedCollisionMap.m_File, kclTargetFile, true);
                    }
                    else
                    {
                        m_ImportedCollisionMap.m_File.SaveChanges();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    new ExceptionMessageBox("Error importing collision map", ex).ShowDialog();
                    return false;
                }
            }
            return false;
        }

        private void mnitImportModelOnly_Click(object sender, EventArgs e)
        {
            if (ImportModel())
            {
                lblMainStatus.Text = "Model successfully imported";
            }
        }

        private void mnitImportCollisionMapOnly_Click(object sender, EventArgs e)
        {
            if (ImportCollisionMap())
            {
                lblMainStatus.Text = "Collision map successfully imported";
            }
        }

        private void mnitImportModelAndCollisionMap_Click(object sender, EventArgs e)
        {
            if (ImportModel() && ImportCollisionMap())
            {
                lblMainStatus.Text = "Model and collision map successfully imported";
            }
        }

        private void ExportAllTextures(string targetFolder, ImageFormat imageFormat)
        {
            if (Strings.IsNotBlank(targetFolder))
            {
                foreach (ModelBase.TextureDefNitro texture in m_WorkingTexturesCopy.Values)
                {
                    string fileName = targetFolder + '/' + texture.GetTexName() + '.' + imageFormat.ToString().ToLower();

                    try
                    {
                        texture.GetBitmap().Save(fileName, imageFormat);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        DialogResult result = MessageBox.Show("Error exporting texture [" + texture.GetTexName() + 
                            "]. Would you like to continue?",
                            e.Message, MessageBoxButtons.YesNo);
                        if (result == DialogResult.No) return;
                    }
                }
            }
        }

        private void mnitExportTexturesPNG_Click(object sender, EventArgs e)
        {
            DialogResult result = m_FolderBrowserDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExportAllTextures(m_FolderBrowserDialogue.SelectedPath, ImageFormat.Png);
            }
        }

        private void mnitExportTexturesBMP_Click(object sender, EventArgs e)
        {
            DialogResult result = m_FolderBrowserDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExportAllTextures(m_FolderBrowserDialogue.SelectedPath, ImageFormat.Bmp);
            }
        }

        private void mnitExportModel_Click(object sender, EventArgs e)
        {
            if (m_ModelSourceLoaded)
            {
                m_SaveFileDialogue.FileName = Path.GetFileNameWithoutExtension(m_ModelSourceName); ;
                m_SaveFileDialogue.DefaultExt = ".dae";
                m_SaveFileDialogue.Filter = Strings.MODEL_EXPORT_FORMATS_FILTER;
                DialogResult result = m_SaveFileDialogue.ShowDialog();
                if (result == DialogResult.OK)
                {
                    BMD_BCA_KCLExporter.ExportBMDModel(m_ImportedModel, m_SaveFileDialogue.FileName);
                }
            }
        }

        private void mnitExportCollisionMap_Click(object sender, EventArgs e)
        {
            if (m_ModelSourceLoaded)
            {
                m_SaveFileDialogue.FileName = Path.GetFileNameWithoutExtension(m_ModelSourceName) + "_collision_map";
                m_SaveFileDialogue.DefaultExt = ".dae";
                m_SaveFileDialogue.Filter = Strings.MODEL_EXPORT_FORMATS_FILTER;
                DialogResult result = m_SaveFileDialogue.ShowDialog();
                if (result == DialogResult.OK)
                {
                    BMD_BCA_KCLExporter.ExportKCLModel(m_ImportedCollisionMap, m_SaveFileDialogue.FileName);
                }
            }
        }
    }
}
