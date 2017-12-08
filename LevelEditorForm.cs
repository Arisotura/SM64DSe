/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Globalization;
using System.Xml;
using SM64DSe.ImportExport;
using SM64DSe.SM64DSFormats;
using SM64DSe.ImportExport.LevelImportExport;


namespace SM64DSe
{
    public partial class LevelEditorForm : Form
    {
        private static Color k_SelectionColor = Color.FromArgb(255, 255, 128);
        private static Color k_HoverColor = Color.FromArgb(255, 255, 192);

        private bool settingValues = false;
        private int box_general_NextHeight = 18;
        private int box_position_NextHeight = 18;
        private int box_rotation_NextHeight = 18;
        private int box_fogSettings_NextHeight = 18;
        private int box_parameters_NextHeight = 18;

        private Button btnOpenRawEditor = new Button();
        public RawEditorForm m_rawEditor;

        private bool m_wasMinimized = false;

        private int m_areaCount = 1;
        private int m_currentArea = -1;

        public ToolTip defaultToolTip;

        private void LevelEditorForm_Load(object sender, EventArgs e)
        {
            defaultToolTip = new ToolTip();

            defaultToolTip.AutoPopDelay = 5000;
            defaultToolTip.InitialDelay = 1000;
            defaultToolTip.ReshowDelay = 500;

            defaultToolTip.ShowAlways = true;


            defaultToolTip.SetToolTip(this.val_posX, "The objects X-Position");
            defaultToolTip.SetToolTip(this.val_posY, "The objects Y-Position");
            defaultToolTip.SetToolTip(this.val_posZ, "The objects Z-Position");

            defaultToolTip.SetToolTip(this.val_rotX, "The objects X-Rotation");
            defaultToolTip.SetToolTip(this.val_rotY, "The objects Y-Rotation");

            defaultToolTip.SetToolTip(this.val_r, "The Fogs red Color Value");
            defaultToolTip.SetToolTip(this.val_g, "The Fogs green Color Value");
            defaultToolTip.SetToolTip(this.val_b, "The Fogs blue Color Value");
        }

        private void ClampRotation(ref float val, float twopi)
        {
            if (val > twopi)
            {
                while (val > twopi)
                    val -= twopi;
            }
            else if (val < -twopi)
            {
                while (val < -twopi)
                    val += twopi;
            }
        }

        private bool IsSimpleObject(ushort id)
        {
            switch (id)
            {
                case 37: // COIN
                case 38: // RED_COIN
                case 39: // BLUE_COIN
                case 41: // TREE
                case 53: // SBIRD
                case 54: // FISH
                case 55: // BUTTERFLY
                case 60: // STAR_CAMERA
                case 61: // POWER STAR
                case 62: // SILVER STAR
                case 63: // STARBASE
                case 269: // BLK_OKINOKO_TAG
                case 270: // BLK_SKINOKO_TAG
                case 271: // BLK_GNSHELL_TAG
                case 272: // BLK_SLVSTAR_TAG
                case 323: // SET_SE
                case 324: // MUGEN_BGM
                case 511: // minimap change
                    return true;
                default: return false;
            }
        }

        private void LoadLevelData()
        {
            m_Level = new Level(m_LevelID);

            AlignPathNodes();

            m_LevelModel = null;
            m_LevelCollMap = new KCL(m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.KCLFileID));

            m_SkyboxModel = null;
        }

        public LevelEditorForm(NitroROM rom, int levelid)
        {
            InitializeComponent();
            
            btnOpenRawEditor.Text = "Raw Editor";
            btnOpenRawEditor.Click += btnOpenRawEditor_Click;

            this.Text = string.Format("[{0}] {1} - {2} {3}", levelid, Strings.LevelNames[levelid], Program.AppTitle, Program.AppVersion);
            
            m_MouseDown = MouseButtons.None;

            m_ROM = rom;
            m_LevelID = levelid;
            
            m_GLLoaded = false;

            btnStar1.Checked = true;
            btnStarAll.Checked = true;
            m_ShowCommonLayer = true;
            m_AuxLayerNum = 1;
            btnEditObjects.Checked = true;
            m_EditMode = EditMode.OBJECTS;

            m_Hovered = 0xFFFFFFFF;
            m_LastHovered = 0xFFFFFFFF;
            m_HoveredObject = null;
            m_Selected = 0xFFFFFFFF;
            m_LastSelected = 0xFFFFFFFF;
            m_SelectedObject = null;
            m_LastClicked = 0xFFFFFFFF;
            m_ObjectBeingPlaced = 0xFFFF;
            m_ShiftPressed = false;

            btnMakeOverlay.Visible = (Program.m_ROM.m_Version == NitroROM.Version.EUR);

            slStatusLabel.Text = "Ready";
        }


        public void UpdateSkybox(int id)
        {
            if (id == -1)
                id = m_Level.m_LevelSettings.Background;

            if (m_SkyboxModel != null)
                ModelCache.RemoveModel(m_SkyboxModel);
            if (m_SkyboxDL != 0)
                GL.DeleteLists(m_SkyboxDL, 1);

            if (id > 0)
            {
                string filename = String.Format("data/vrbox/vr{0:D2}.bmd", id);
                m_SkyboxModel = ModelCache.GetModel(filename);

                m_SkyboxDL = GL.GenLists(1);
                m_SkyboxModel.PrepareToRender();
                GL.NewList(m_SkyboxDL, ListMode.Compile);
                m_SkyboxModel.Render(0.01f);
                GL.EndList();
            }
            else
            {
                m_SkyboxModel = null;
                m_SkyboxDL = 0;
            }

            glLevelView.Refresh();
        }

        public void UpdateLevelModel()
        {
            if (m_LevelModel != null)
                m_LevelModel.Release();

            m_LevelModel = new BMD(m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.BMDFileID));
            m_LevelModel.PrepareToRender();
            RenderLevelAreas(-1);
            m_areaCount = m_LevelModel.m_ModelChunks.Length;

        }

        public void RenderLevelAreas(int area)
        {
            m_LevelModelDLs = new int[m_LevelModel.m_ModelChunks.Length, 3];

            for (int c = 0; c < m_LevelModel.m_ModelChunks.Length; c++)
            {
                if ((area > -1) && (c != area))
                    continue;
                m_LevelModelDLs[c, 0] = GL.GenLists(1);
                GL.NewList(m_LevelModelDLs[c, 0], ListMode.Compile);
                m_LevelModel.m_ModelChunks[c].Render(RenderMode.Opaque, 1.0f);
                GL.EndList();

                m_LevelModelDLs[c, 1] = GL.GenLists(1);
                GL.NewList(m_LevelModelDLs[c, 1], ListMode.Compile);
                m_LevelModel.m_ModelChunks[c].Render(RenderMode.Translucent, 1.0f);
                GL.EndList();

                m_LevelModelDLs[c, 2] = GL.GenLists(1);
                GL.NewList(m_LevelModelDLs[c, 2], ListMode.Compile);
                m_LevelModel.m_ModelChunks[c].Render(RenderMode.Picking, 1.0f);
                GL.EndList();
            }

            glLevelView.Refresh();
        }

        private void UpdateCamera()
        {
            Vector3 up;

            if (Math.Cos(m_CamRotation.Y) < 0)
            {
                m_UpsideDown = true;
                up = new Vector3(0.0f, -1.0f, 0.0f);
            }
            else
            {
                m_UpsideDown = false;
                up = new Vector3(0.0f, 1.0f, 0.0f);
            }

            m_CamPosition.X = m_CamDistance * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamPosition.Y = m_CamDistance * (float)Math.Sin(m_CamRotation.Y);
            m_CamPosition.Z = m_CamDistance * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            Vector3 skybox_target;
            skybox_target.X = -(float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            skybox_target.Y = -(float)Math.Sin(m_CamRotation.Y);
            skybox_target.Z = -(float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            Vector3.Add(ref m_CamPosition, ref m_CamTarget, out m_CamPosition);

            // Make sure top and bottom views don't produce NaN-poisoned matrices
            // Do this after adding target to position to simulate catastrophic cancellation of subtraction
            Vector3 noNaNs = Vector3.Normalize(Vector3.Cross(m_CamPosition - m_CamTarget, up));
            if (double.IsNaN(noNaNs.X) || double.IsNaN(noNaNs.Y) || double.IsNaN(noNaNs.Z))
            {
                up = new Vector3(0.0f, 0.0f, m_CamRotation.Y > 0 ? -1.0f : 1.0f);
            }

            m_CamMatrix = Matrix4.LookAt(m_CamPosition, m_CamTarget, up);
            m_SkyboxMatrix = Matrix4.LookAt(Vector3.Zero, skybox_target, up);
        }

        private void RenderObjectLists(RenderMode mode, int layer)
        {
            int t = 0;
            switch (mode)
            {
                case RenderMode.Opaque: t = 0; break;
                case RenderMode.Translucent: t = 1; break;
                case RenderMode.Picking: t = 2; break;
            }

            if (m_ObjectDLs[layer, t] == 0)
                m_ObjectDLs[layer, t] = GL.GenLists(1);

            GL.NewList(m_ObjectDLs[layer, t], ListMode.Compile);

            if (mode == RenderMode.Picking)
            {
                IEnumerable<LevelObject> objects = m_Level.m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == (int)m_EditMode && obj.m_Layer == layer);
                foreach (LevelObject obj in objects)
                {
                    GL.Color4(Color.FromArgb((int)obj.m_UniqueID));
                    obj.Render(mode);
                }
            }
            else
            {
                IEnumerable<LevelObject> objects;
                if ((m_EditMode == EditMode.MODEL)&&(m_currentArea>-1))
                {
                    
                    objects = m_Level.m_LevelObjects.Values.
                        Where(obj => (ShowsForArea(obj, layer)));
                    
                }
                else
                {
                    objects = m_Level.m_LevelObjects.Values.Where(obj => obj.m_Layer == layer);
                }
                foreach (LevelObject obj in objects)
                    obj.Render(mode);
            }

            GL.EndList();
        }

        private bool ShowsForArea(LevelObject obj, int layer)
        {
            if ((obj.m_UniqueID >> 28) == 1)
            {
                if (obj.m_Area == m_currentArea)
                {
                    if (obj.m_Layer == layer)
                    {
                        return true;
                    }
                }
            }
            else if ((obj.m_UniqueID >> 28) == 2)
            {
                if (obj.m_Type == LevelObject.Type.DOOR)
                {
                    DoorObject door = (DoorObject)obj;
                    if ((door.InAreaID == m_currentArea) || (door.OutAreaID == m_currentArea))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void RenderObjectHilite(LevelObject obj, Color color, int dlist)
        {
            GL.NewList(dlist, ListMode.Compile);
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ColorMask(false, false, false, false);
            GL.Enable(EnableCap.StencilTest);
            GL.StencilMask(0x3);
            GL.StencilFunc(StencilFunction.Always, 0x1, 0x3);
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            obj.Render(RenderMode.Picking);

            GL.ColorMask(true, true, true, true);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(-1.0f, -1.0f);
            GL.StencilFunc(StencilFunction.Equal, 0x1, 0x3);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);
            GL.Color4(Color.FromArgb(100, color));
            obj.Render(RenderMode.Picking);

            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.LineWidth(3.0f);
            GL.StencilFunc(StencilFunction.Equal, 0x0, 0x3);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.DepthFunc(DepthFunction.Always);
            GL.Color4(color);
            obj.Render(RenderMode.Picking);

            GL.PopAttrib();
            GL.EndList();
        }

        private void RenderPathHilite(List<LevelObject> objs, System.Boolean closed, Color color, int dlist)
        {
            GL.NewList(dlist, ListMode.Compile);
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            for (int i = 0; i < objs.Count(); i++)
            {
                LevelObject obj = objs.ElementAt(i);
                if (i > 0) {
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Color4(Color.FromArgb(255, color));
                    GL.Vertex3(((PathPointObject)obj).Position);
                    GL.Vertex3(((PathPointObject)objs.ElementAt(i-1)).Position);
                    GL.End();
                    if (closed && (i == objs.Count() - 1))
                    {
                        GL.Begin(PrimitiveType.LineStrip);
                        GL.Vertex3(((PathPointObject)obj).Position);
                        GL.Vertex3(((PathPointObject)objs.ElementAt(0)).Position);
                        GL.End();
                    }
                }

                GL.ColorMask(true, true, true, true);
                GL.Enable(EnableCap.PolygonOffsetFill);
                GL.PolygonOffset(-1.0f, -1.0f);
                GL.StencilFunc(StencilFunction.Equal, 0x1, 0x3);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);
                if (i == 0 || objs.Count() == 1)
                    GL.Color4(Color.FromArgb(50, 0, 255, 0));
                else if (i == objs.Count() - 1)
                    GL.Color4(Color.FromArgb(50, 255, 0, 0));
                else
                    GL.Color4(Color.FromArgb(50, color));
                obj.Render(RenderMode.Picking);

                GL.Disable(EnableCap.PolygonOffsetFill);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(3.0f);
                GL.StencilFunc(StencilFunction.Equal, 0x0, 0x3);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
                GL.DepthFunc(DepthFunction.Always);
                if (i == 0 || objs.Count() == 1)
                    GL.Color4(Color.FromArgb(50, 0, 255, 0));
                else if (i == objs.Count() - 1)
                    GL.Color4(Color.FromArgb(50, 255, 0, 0));
                else
                    GL.Color4(Color.FromArgb(50, color));
                obj.Render(RenderMode.Picking);
            }

            GL.PopAttrib();
            GL.EndList();
        }

        private void AlignPathNodes()
        {
            IEnumerable<LevelObject> pathNodes = m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE);
            IEnumerable<LevelObject> paths = m_Level.GetAllObjectsByType(LevelObject.Type.PATH);
            AlignPathNodes(paths.ToList<LevelObject>(), pathNodes.ToList<LevelObject>());
        }

        private void AlignPathNodes(List<LevelObject> paths, List<LevelObject> nodes)
        {
            // Aligns the path nodes to point to the next node in their path to make direction clear

            foreach (LevelObject path in paths)
            {
                int startNode = path.Parameters[0], endNode = startNode + path.Parameters[1] - 1;
                int numNodes = nodes.Count;

                if (numNodes == 1)
                    ((PathPointObject)nodes[0]).m_Renderer = new ColorCubeRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false);
                else
                {
                    // Render each node as an arrow that points to next node
                    for (int i = startNode; i <= endNode; i++)
                    {
                        int next = (i != endNode) ? (i + 1) : startNode;

                        if (nodes[i] == null) continue;

                        float opposite = nodes[next].Position.X - nodes[i].Position.X;
                        float adjacent = nodes[next].Position.Z - nodes[i].Position.Z;
                        float rotY = MathHelper.RadiansToDegrees((float)Math.Atan(opposite / adjacent));
                        
                        if (adjacent >= 0)
                        {
                            rotY += 180;
                        }
                        if (float.IsNaN(rotY))
                            ((PathPointObject)nodes[i]).m_Renderer = new ColorCubeRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false);
                        else
                            ((PathPointObject)nodes[i]).m_Renderer = new ColourArrowRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false, 0f, rotY, 0f);
                    }
                }
            }
        }

        public void RefreshObjects(int layer)
        {
            AlignPathNodes();

            RenderObjectLists(RenderMode.Opaque, layer);
            RenderObjectLists(RenderMode.Translucent, layer);
            RenderObjectLists(RenderMode.Picking, layer);
            if (m_SelectedObject != null)
            {
                RenderObjectHilite(m_SelectedObject, k_SelectionColor, m_SelectHiliteDL);
                RenderObjectHilite(m_SelectedObject, k_HoverColor, m_HoverHiliteDL);
            }
            glLevelView.Refresh();
        }


        private void PopulateObjectList()
        {
            tvObjectList.Nodes.Clear();
            foreach (ToolStripItem ctl in tsEditActions.Items)
                ctl.Visible = false;

            slStatusLabel.Text = "Ready";

            switch (m_EditMode)
            {
                case EditMode.MODEL:
                    {
                        btnImportModel.Visible = true;
                        btnExportLevelModel.Visible = true;
                        btnImportOtherModel.Visible = Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters;
                        btnExportOtherModel.Visible = Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters;

                        tvObjectList.Nodes.Add("model", "All Areas").Tag = -1;
                        for (int i = 0; i<m_areaCount; i++)
                        {
                            TreeNode node = tvObjectList.Nodes.Add("model", "Area "+i);
                            node.Tag = i;
                            if (i == m_currentArea)
                                tvObjectList.SelectedNode = node;
                        }
                    }
                    break;

                case EditMode.OBJECTS:
                    {
                        btnAddObject.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnRemoveAll.Visible = true;
                        btnReplaceObjModel.Visible = true;
                        btnExportObjectModel.Visible = true;
                        btnAddPathNode.Visible = true;

                        TreeNode objectNode = new TreeNode("Objects");
                        objectNode.Name = "object";

                        IEnumerable<LevelObject> objects = m_Level.m_LevelObjects.Values.Where(obj =>
                            ((m_ShowCommonLayer && obj.m_Layer == 0) || (m_AuxLayerNum != 0 && obj.m_Layer == m_AuxLayerNum)) &&
                            (obj.m_UniqueID >> 28) == 1);
                        foreach (LevelObject obj in objects)
                            objectNode.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;

                        tvObjectList.Nodes.Add(objectNode);
                    }
                    break;

                case EditMode.WARPS:
                    {
                        btnAddWarp.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnRemoveAll.Visible = true;

                        TreeNode node0 = tvObjectList.Nodes.Add("entrance", "Entrances");
                        TreeNode node1 = tvObjectList.Nodes.Add("exit", "Exits");
                        TreeNode node2 = tvObjectList.Nodes.Add("door", "Doors");
                        TreeNode node3 = tvObjectList.Nodes.Add("teleport_source", "Teleport sources");
                        TreeNode node4 = tvObjectList.Nodes.Add("teleport_destination", "Teleport destinations");

                        IEnumerable<LevelObject> objects = m_Level.m_LevelObjects.Values.Where(obj =>
                            ((m_ShowCommonLayer && obj.m_Layer == 0) || (m_AuxLayerNum != 0 && obj.m_Layer == m_AuxLayerNum)) &&
                            (obj.m_UniqueID >> 28) == 2);
                        foreach (LevelObject obj in objects)
                        {
                            switch (obj.m_Type)
                            {
                                case LevelObject.Type.ENTRANCE: node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.EXIT: node1.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.DOOR: node2.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.TELEPORT_SOURCE: node3.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.TELEPORT_DESTINATION: node4.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                            }
                        }
                    }
                    break;

                case EditMode.PATHS:
                    {
                        btnAddPath.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnRemoveAll.Visible = true;
                        
                        IEnumerable<LevelObject> paths = m_Level.GetAllObjectsByType(LevelObject.Type.PATH);
                        IEnumerable<LevelObject> pathNodes = m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE)
                            .OrderBy(obj => ((PathPointObject)obj).m_NodeID);

                        TreeNode pathsNode = new TreeNode("Paths");
                        pathsNode.Name = "paths";
                        pathsNode.Expand();
                        for (int i = 0; i < paths.Count(); i++)
                        {
                            PathObject path = (PathObject)paths.ElementAt(i);
                            path.m_PathID = (ushort)i;

                            TreeNode pathTreeNode = new TreeNode(paths.ElementAt(i).GetDescription());
                            pathTreeNode.Name = "path"+i;
                            pathTreeNode.Tag = paths.ElementAt(i).m_UniqueID;

                            int offset = path.Parameters[0];
                            for (int j = 0; j < path.Parameters[1]; j++)
                            {
                                PathPointObject node = (PathPointObject)pathNodes.ElementAt(offset+j);
                                node.ParentPath = (ushort)i;
                                node.m_IndexInPath = (byte)j;
                                pathTreeNode.Nodes.Add(node.m_UniqueID.ToString("X8"), node.GetDescription()).Tag =
                                    node.m_UniqueID;
                            }
                            pathsNode.Nodes.Add(pathTreeNode);
                        }
                        tvObjectList.Nodes.Add(pathsNode);
                    }
                    break;

                case EditMode.VIEWS:
                    {
                        btnAddView.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnRemoveAll.Visible = true;

                        if (!m_ShowCommonLayer) break;
                        TreeNode node0 = tvObjectList.Nodes.Add("view", "Views");

                        IEnumerable<LevelObject> objects = m_Level.m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 4);
                        foreach (LevelObject obj in objects)
                        {
                            node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;
                        }
                    }
                    break;

                case EditMode.MISC:
                    {
                        btnAddMisc.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnRemoveAll.Visible = true;

                        if (!m_ShowCommonLayer) break;
                        TreeNode minimapScaleNode = tvObjectList.Nodes.Add("minimap_scale", "Minimap Scales");
                        TreeNode fogNode = tvObjectList.Nodes.Add("fog", "Fog");
                        TreeNode type14Node = tvObjectList.Nodes.Add("type_14", "Type 14 Object");
                        TreeNode minimapTileIDNode = tvObjectList.Nodes.Add("minimap_tile_id", "Minimap Tile ID");

                        IEnumerable<LevelObject> objects = m_Level.m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 5);
                        foreach (LevelObject obj in objects)
                        {
                            switch (obj.m_Type)
                            {
                                case LevelObject.Type.FOG: fogNode.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.MINIMAP_TILE_ID: minimapTileIDNode.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.MINIMAP_SCALE: minimapScaleNode.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case LevelObject.Type.UNKNOWN_14: type14Node.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                            }
                        }
                    }
                    break;
            }

            if (!(m_EditMode==EditMode.PATHS))
                tvObjectList.ExpandAll();
        }

        private void PopulatePathNodes(int pathId)
        {
            IEnumerable<LevelObject> paths = m_Level.GetAllObjectsByType(LevelObject.Type.PATH);
            IEnumerable<LevelObject> pathNodes = m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE)
                            .OrderBy(obj => ((PathPointObject)obj).m_NodeID);


            PathObject path = (PathObject)paths.ElementAt(pathId);

            TreeNode pathTreeNode = tvObjectList.Nodes[0].Nodes["path" + pathId];

            int offset = path.Parameters[0];
            pathTreeNode.Nodes.Clear();
            for (int i = 0; i < path.Parameters[1]; i++)
            {
                PathPointObject node = (PathPointObject)pathNodes.ElementAt(offset + i);
                node.ParentPath = (ushort)pathId;
                node.m_IndexInPath = (byte)i;
                pathTreeNode.Nodes.Add(node.m_UniqueID.ToString("X8"), node.GetDescription()).Tag =
                    node.m_UniqueID;
                if (node == m_SelectedObject)
                {
                    tvObjectList.SelectedNode = pathTreeNode.Nodes[i];
                    m_nextObjectToSelect = node;
                }
            }
        }

        private void UpdateTransformProperties()
        {
            PropertyTable ptable = m_SelectedObject.m_Properties;
            ptable["X position"] = m_SelectedObject.Position.X;
            ptable["Y position"] = m_SelectedObject.Position.Y;
            ptable["Z position"] = m_SelectedObject.Position.Z;
            if (m_SelectedObject.SupportsRotation())
                ptable["Y rotation"] = m_SelectedObject.YRotation;
        }
        
        private NitroROM m_ROM;
        public int m_LevelID;

        public struct PointerReference
        {
            public PointerReference(uint _ref, uint _ptr) { m_ReferenceAddr = _ref; m_PointerAddr = _ptr; }
            public uint m_ReferenceAddr; // address at which the pointer is stored
            public uint m_PointerAddr; // address to which the pointer points
        }

        private LevelObject AddObject(LevelObject.Type type, ushort id, int layer, int area)
        {
            IEnumerable<LevelObject> paths;
            LevelObject obj = null;
            string parentnode = null;
            switch (type)
            {
                case LevelObject.Type.STANDARD:
                    parentnode = "object";
                    obj = m_Level.AddStandardObject(id, layer, area);
                    break;
                case LevelObject.Type.ENTRANCE:
                    parentnode = "entrance";
                    obj = m_Level.AddEntranceObject(layer);
                    break;
                case LevelObject.Type.PATH_NODE:
                    parentnode = "path"+m_CurrentPathID;
                    
                    PathPointObject point = m_Level.AddPathPointObject(m_CurrentPathID);
                    point.m_IndexInPath = 0;
                    obj = point;

                    break;
                case LevelObject.Type.PATH:
                    paths = m_Level.GetAllObjectsByType(LevelObject.Type.PATH);
                    PathObject path = m_Level.AddPathObject();
                    int pathId = path.m_PathID;
                    TreeNode pathTreeNode = new TreeNode(path.GetDescription());
                    pathTreeNode.Name = "path" + pathId;
                    pathTreeNode.Tag = path.m_UniqueID;
                    tvObjectList.Nodes[0].Nodes.Add(pathTreeNode);
                    path.GenerateProperties();
                    //add a node to the paths
                    obj = m_Level.AddPathPointObject(pathId);
                    m_SelectedObject = obj;
                    PopulatePathNodes(pathId);
                    return obj;
                case LevelObject.Type.VIEW: parentnode = "view";  obj = m_Level.AddViewObject(); break;
                case LevelObject.Type.SIMPLE:
                    parentnode = "object";
                    obj = m_Level.AddSimpleObject(id, layer, area);
                    break;
                case LevelObject.Type.TELEPORT_SOURCE: parentnode = "teleport_source"; obj = m_Level.AddTpSrcObject(layer); break;
                case LevelObject.Type.TELEPORT_DESTINATION: parentnode = "teleport_destination"; obj = m_Level.AddTpDstObject(layer); break;
                case LevelObject.Type.FOG: parentnode = "fog"; obj = m_Level.AddFogObject(layer, area); break;
                case LevelObject.Type.DOOR: parentnode = "door"; obj = m_Level.AddDoorObject(layer); break;
                case LevelObject.Type.EXIT: parentnode = "exit"; obj = m_Level.AddExitObject(layer); break;
                case LevelObject.Type.MINIMAP_TILE_ID: parentnode = "minimap_tile_id"; obj = m_Level.AddMinimapTileIDObject(layer, area); break;
                case LevelObject.Type.MINIMAP_SCALE: parentnode = "minimap_scale"; obj = m_Level.AddMinimapScaleObject(layer, area); break;
                case LevelObject.Type.UNKNOWN_14: parentnode = "type_14"; obj = m_Level.AddType14Object(layer, area); break;
            }

            if (obj != null)
            {
                tvObjectList.Nodes[parentnode].Nodes.Add(
                    obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;
            }

            return obj;
        }
        
        private void RemoveObject(LevelObject obj, bool bulk = false)
        {
            if (m_Level.RemoveObject(obj))
            {
                TreeNode[] objectNode = tvObjectList.Nodes.Find(obj.m_UniqueID.ToString("X8"), true);
                if (objectNode.Length > 0)
                    objectNode[0].Parent.Nodes.RemoveByKey(obj.m_UniqueID.ToString("X8"));

                if (!bulk)
                {
                    if (obj.m_Type == LevelObject.Type.ENTRANCE)
                    {
                        IEnumerable<LevelObject> toupdate = m_Level.GetAllObjectsByType(LevelObject.Type.ENTRANCE)
                            .Where(obj2 => ((EntranceObject)obj2).m_EntranceID >= ((EntranceObject)obj).m_EntranceID);
                        foreach (LevelObject entrance in toupdate)
                        {
                            tvObjectList.Nodes.Find(entrance.m_UniqueID.ToString("X8"), true)[0].Text = entrance.GetDescription();
                        }
                    }
                    else if (obj.m_Type == LevelObject.Type.VIEW)
                    {
                        IEnumerable<LevelObject> toupdate = m_Level.GetAllObjectsByType(LevelObject.Type.VIEW)
                            .Where(obj2 => ((ViewObject)obj2).m_ViewID >= ((ViewObject)obj).m_ViewID);
                        foreach (LevelObject view in toupdate)
                        {
                            tvObjectList.Nodes.Find(view.m_UniqueID.ToString("X8"), true)[0].Text = view.GetDescription();
                        }
                    }
                    else if (obj.m_Type == LevelObject.Type.PATH)
                    {
                        PopulateObjectList();
                    }
                    else if (obj.m_Type == LevelObject.Type.PATH_NODE)
                    {
                        LevelObject[] paths = m_Level.GetAllObjectsByType(LevelObject.Type.PATH).ToArray();
                        PathPointObject pathPoint = (PathPointObject)obj;
                        PathObject parent = (PathObject)paths[pathPoint.ParentPath];
                        if (parent.Parameters[1] < 1)
                        {
                            RemoveObject(parent);
                            return;
                        }
                        PopulatePathNodes(((PathPointObject)obj).ParentPath);
                    }
                }
            }
        }

        private void RemoveObjects(List<LevelObject> objs)
        {
            if (objs != null)
            {
                for (int i = objs.Count - 1; i >= 0; i--)
                {
                    RemoveObject(objs[i], true);
                }
                PopulateObjectList();
            }
        }

        private void CopyObject(LevelObject objectToCopy)
        {
            LevelObject.Type type = objectToCopy.m_Type;
            ushort id = objectToCopy.ID;
            if (type == LevelObject.Type.STANDARD && IsSimpleObject(id))
                type = LevelObject.Type.SIMPLE;

            LevelObject obj = AddObject(type, id, objectToCopy.m_Layer, objectToCopy.m_Area);
            obj.Position = objectToCopy.Position;
            if (objectToCopy.SupportsRotation()) obj.YRotation = objectToCopy.YRotation;
            if (obj.Parameters != null) Array.Copy(objectToCopy.Parameters, obj.Parameters, obj.Parameters.Length);
            obj.GenerateProperties();

            if (obj.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            }

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            RefreshObjects(m_SelectedObject.m_Layer);
        }

        public Level m_Level;

        // 3D view settings
        private bool m_GLLoaded;

        private const float k_zNear = 0.01f;
        private const float k_zFar = 1000f;
        private const float k_FOV = (float)(70f * Math.PI) / 180f;

        private Vector2 m_CamRotation;
        private Vector3 m_CamTarget;
        private float m_CamDistance;
        private float m_AspectRatio;
        private float m_PixelFactorX, m_PixelFactorY;
        private Vector3 m_CamPosition;
        private bool m_UpsideDown;
        private Matrix4 m_CamMatrix, m_SkyboxMatrix;

        private bool m_OrthView = false;
        private float m_OrthZoom = 20f;

        private uint[] m_PickingFrameBuffer;
        private float m_PickingDepth;
        private float m_PickingModelDepth;

        private bool m_ShowCommonLayer;
        private int m_AuxLayerNum;
        private EditMode m_EditMode;

        private MouseButtons m_MouseDown;
        private Point m_LastMouseClick, m_LastMouseMove;
        private Point m_MouseCoords;

        private uint m_Hovered, m_LastHovered;
        private uint m_Selected, m_LastSelected;
        private uint m_LastClicked;
        private LevelObject m_HoveredObject;
        private LevelObject m_SelectedObject;
        private LevelObject m_nextObjectToSelect = null;//the object to be selected when one has been removed
        private LevelObject m_CopiedObject;
        private uint m_ObjectBeingPlaced;
        private bool m_ShiftPressed;

        private BMD m_SkyboxModel;
        private BMD m_LevelModel;
        private KCL m_LevelCollMap;

        private int m_SkyboxDL;
        private int[,] m_LevelModelDLs;
        private int[,] m_ObjectDLs;
        private int m_SelectHiliteDL;
        private int m_HoverHiliteDL;

        private Vector3 m_GridSize;
        private Vector3 m_GridOffset;

        private Vector3 m_RestrPlaneNormal;
        private Vector3 m_RestrPlaneOffset;

        private Vector3 m_SelObjPrevPos;
        private Vector3 m_SelObjTotalMov;

        private Vector3 m_copiedPosition = Vector3.Zero;

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();
        private OpenFileDialog m_OpenFileDialogue = new OpenFileDialog();
        private FolderBrowserDialog m_FolderBrowserDialogue = new FolderBrowserDialog();
        private SaveFileDialog m_SaveFileDialogue = new SaveFileDialog();

        private enum EditMode
        {
            MODEL = 0,
            OBJECTS = 1, 
            WARPS = 2, 
            PATHS = 3, 
            VIEWS = 4, 
            MISC = 5
        };

        private void SnapToGrid(ref Vector3 pos)
        {
            if (m_GridSize.X != 0)
                pos.X = (float)Math.Round((pos.X - m_GridOffset.X) / m_GridSize.X) * m_GridSize.X + m_GridOffset.X;
            if (m_GridSize.Y != 0)
                pos.Y = (float)Math.Round((pos.Y - m_GridOffset.Y) / m_GridSize.Y) * m_GridSize.Y + m_GridOffset.Y;
            if (m_GridSize.Z != 0)
                pos.Z = (float)Math.Round((pos.Z - m_GridOffset.Z) / m_GridSize.Z) * m_GridSize.Z + m_GridOffset.Z;
        }

        private void glLevelView_Load(object sender, EventArgs e)
        {
            InitialiseLevel();
        }

        private void InitialiseLevel()
        {
            // initialize OpenGL
            glLevelView.Context.MakeCurrent(glLevelView.WindowInfo);

            m_PickingFrameBuffer = new uint[9];
            m_PickingDepth = 0f;
            m_PickingModelDepth = 0f;

            GL.Viewport(glLevelView.ClientRectangle);

            m_AspectRatio = (float)glLevelView.Width / (float)glLevelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar);
            GL.MultMatrix(ref projmtx);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0);
            /* http://www.opentk.com/node/2514
             * GL.ClearDepth(float) requires the ARB_ES2_compatibility extension that's not supported by your card. IIRC, 
             * this extension was introduced along with OpenGL 4.0.
             * GL.ClearDepth(double) is a GL 1.0 function that should be available everywhere.
             */ 

            // lighting!
            //GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0.0f, -0.646484375f, -0.646484375f, 0.0f));
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1.0f, 1.0f, 1.0f, 0.0f));
            GL.Light(LightName.Light0, LightParameter.Ambient, Color.SkyBlue);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Color.SkyBlue);
            GL.Light(LightName.Light0, LightParameter.Specular, Color.SkyBlue);
            /*GL.Light(LightName.Light1, LightParameter.Position, new Vector4(-0.666015625f, -0.35546875f, -0.1103515625f, 0.0f));
            GL.Light(LightName.Light1, LightParameter.Ambient, Color.Red);
            GL.Light(LightName.Light1, LightParameter.Diffuse, Color.Red);
            GL.Light(LightName.Light1, LightParameter.Specular, Color.Red);*/

            GL.Enable(EnableCap.Normalize);

            m_CamRotation = new Vector2((float)Math.PI / 2.0f, (float)Math.PI / 8.0f);
            // m_CamRotation = new Vector2(0.0f, 0.0f);
            m_CamTarget = new Vector3(0.0f, 0.0f, 0.0f);
            m_CamDistance = 1.0f;//6.5f;
            UpdateCamera();

            m_PixelFactorX = ((2f * (float)Math.Tan(k_FOV / 2f) * m_AspectRatio) / (float)(glLevelView.Width));
            m_PixelFactorY = ((2f * (float)Math.Tan(k_FOV / 2f)) / (float)(glLevelView.Height));

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0.0f);

            GL.Enable(EnableCap.Texture2D);

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            LoadLevelData();
            PopulateObjectList();

            // prerender the skybox
            UpdateSkybox(-1);

            // prerender the level model
            m_LevelModel = null; m_LevelModelDLs = null;
            UpdateLevelModel();

            // prerender the objects
            m_ObjectDLs = new int[8, 3];
            for (int l = 0; l < 8; l++)
            {
                RenderObjectLists(RenderMode.Opaque, l);
                RenderObjectLists(RenderMode.Translucent, l);
                RenderObjectLists(RenderMode.Picking, l);
            }

            m_SelectHiliteDL = GL.GenLists(1);
            m_HoverHiliteDL = GL.GenLists(1);

            m_GLLoaded = true;

            m_GridSize = Vector3.Zero;
            m_GridOffset = Vector3.Zero;
        }

        private void glLevelView_Resize(object sender, EventArgs e)
        {
            if (!m_GLLoaded) return;
            glLevelView.Context.MakeCurrent(glLevelView.WindowInfo);

            GL.Viewport(glLevelView.ClientRectangle);

            m_AspectRatio = (float)glLevelView.Width / (float)glLevelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar);
            GL.LoadMatrix(ref projmtx);

            m_PixelFactorX = ((2f * (float)Math.Tan(k_FOV / 2f) * m_AspectRatio) / (float)(glLevelView.Width));
            m_PixelFactorY = ((2f * (float)Math.Tan(k_FOV / 2f)) / (float)(glLevelView.Height));
        }

        int lol = 0;

        private void glLevelView_Paint(object sender, PaintEventArgs e)
        {
            if (!m_GLLoaded) return;
            glLevelView.Context.MakeCurrent(glLevelView.WindowInfo);
            
            // lol temporary
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projmtx = (!m_OrthView) ? Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar) : 
                Matrix4.CreateOrthographic(m_OrthZoom, m_OrthZoom / m_AspectRatio, k_zNear, k_zFar);
            GL.LoadMatrix(ref projmtx);

            // Pass 1 - picking mode rendering (render stuff with fake colors that identify objects)

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m_CamMatrix);

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Lighting);

            for (int a = 0; a < m_LevelModelDLs.GetLength(0); a++)
            {
                GL.Color4(Color.FromArgb(a));
                GL.CallList(m_LevelModelDLs[a, 2]);
            }

            GL.Flush();
            GL.ReadPixels(m_MouseCoords.X, glLevelView.Height - m_MouseCoords.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref m_PickingModelDepth);
            m_PickingModelDepth = -(k_zFar * k_zNear / (m_PickingModelDepth * (k_zFar - k_zNear) - k_zFar));

            if (m_ShowCommonLayer) GL.CallList(m_ObjectDLs[0, 2]);
            if (m_AuxLayerNum > 0) GL.CallList(m_ObjectDLs[m_AuxLayerNum, 2]);

            GL.Flush();
            GL.ReadPixels(m_MouseCoords.X - 1, glLevelView.Height - m_MouseCoords.Y + 1, 3, 3, PixelFormat.Bgra, PixelType.UnsignedByte, m_PickingFrameBuffer);

            // depth math from http://www.opengl.org/resources/faq/technical/depthbuffer.htm
            GL.ReadPixels(m_MouseCoords.X, glLevelView.Height - m_MouseCoords.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref m_PickingDepth);
            m_PickingDepth = -(k_zFar * k_zNear / (m_PickingDepth * (k_zFar - k_zNear) - k_zFar));

            // Pass 2 - real rendering

            GL.DepthMask(true);
            GL.ClearColor(0.0f, 0.0f, 0.125f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Dither);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);

            GL.LoadMatrix(ref m_SkyboxMatrix);
            GL.CallList(m_SkyboxDL);

            GL.LoadMatrix(ref m_CamMatrix);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            // opaque polygons
            for (int a = 0; a < m_LevelModelDLs.GetLength(0); a++)
                GL.CallList(m_LevelModelDLs[a, 0]);

            if (m_ShowCommonLayer) GL.CallList(m_ObjectDLs[0, 0]);
            if (m_AuxLayerNum > 0) GL.CallList(m_ObjectDLs[m_AuxLayerNum, 0]);

            // translucent polygons
            for (int a = 0; a < m_LevelModelDLs.GetLength(0); a++)
                GL.CallList(m_LevelModelDLs[a, 1]);

            if (m_ShowCommonLayer) GL.CallList(m_ObjectDLs[0, 1]);
            if (m_AuxLayerNum > 0) GL.CallList(m_ObjectDLs[m_AuxLayerNum, 1]);

            // highlight outlines
            if (m_SelectedObject != null && m_SelectedObject != m_HoveredObject) GL.CallList(m_SelectHiliteDL);
            if (m_HoveredObject != null) GL.CallList(m_HoverHiliteDL);

            //deactivated until i know how to implement it in a usefull way
            /*
            GL.DepthFunc(DepthFunction.Always);
            
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            float x = m_MouseCoords.X;
            float y = m_MouseCoords.Y;
            float s = 0.08f;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
            //GL.Color4(Color.FromArgb(255, 255, 200, 0));
            GL.Translate(
                -1 + x / (float)glLevelView.Width*2f, 
                1 - y / (float)glLevelView.Height*2f, 
                0);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, s, 0);
            for (int i = 0; i <= 5; i++)
            {
                GL.Vertex3(Math.Sin(i * 1.25664) * s, Math.Cos(i * 1.25664) * s, 0);
                GL.Vertex3(Math.Sin(i * 1.25664 + 0.62832) * s * 0.5, Math.Cos(i * 1.25664 + 0.62832) * s * 0.5, 0);
            }
            GL.End();

            GL.Color4(Color.FromArgb(255, 0, 0, 0));
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(0, s, 0);
            for (int i = 0; i <= 5; i++)
            {
                GL.Vertex3(Math.Sin(i * 1.25664) * s, Math.Cos(i * 1.25664) * s, 0);
                GL.Vertex3(Math.Sin(i * 1.25664 + 0.62832) * s * 0.5, Math.Cos(i * 1.25664 + 0.62832) * s * 0.5, 0);
            }
            GL.End();
            GL.DepthFunc(DepthFunction.Less);
            */

            glLevelView.SwapBuffers();
        }

        private KCL.RaycastResult? TotalKCLRaycast(Vector3 start, Vector3 dir)
        {
            KCL.RaycastResult? currRes = m_LevelCollMap.Raycast(start, dir);

            /*foreach(KeyValuePair<uint, LevelObject> objPair in m_LevelObjects)
            {
                KCL.RaycastResult? res = objPair.Value.Raycast(start, dir);
                if (res != null && (currRes == null ||
                    ((KCL.RaycastResult)currRes).m_T > ((KCL.RaycastResult)res).m_T))
                    currRes = res;
            }*/

            return currRes;
        }

        private bool m_RestrPlaneEnabled { get { return m_RestrPlaneNormal != Vector3.Zero; } }

        private void glLevelView_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;

            if (m_ObjectBeingPlaced != 0xFFFF)
            {
                LevelObject.Type type = (LevelObject.Type)(m_ObjectBeingPlaced >> 16);
                ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);
                if (type == LevelObject.Type.STANDARD && IsSimpleObject(id))
                    type = LevelObject.Type.SIMPLE;

                
                LevelObject obj = AddObject(type, id, 0, 0);
                
                if (m_RestrPlaneEnabled)
                {
                    Vector3 start = Get3DCoords(e.Location, k_zNear);
                    Vector3 dir = Get3DCoords(e.Location, k_zFar) - Get3DCoords(e.Location, k_zNear);
                    Vector3? hit = null;

                    //Snap to restriction plane
                    float dot = Vector3.Dot(m_RestrPlaneNormal, dir);
                    if (dot != 0)
                    {
                        float t = Vector3.Dot(m_RestrPlaneNormal, m_RestrPlaneOffset - start) / dot;
                        hit = dir * t + start;
                    }

                    obj.Position = (hit != null) ? (Vector3)hit : Get3DCoords(e.Location, 2.0f);

                }
                else
                {
                    //Try to snap the object to the ground
                    KCL.RaycastResult? hit = TotalKCLRaycast(Get3DCoords(e.Location, k_zNear),
                    Get3DCoords(e.Location, k_zFar) - Get3DCoords(e.Location, k_zNear));
                    obj.Position = (hit != null) ? ((KCL.RaycastResult)hit).m_Point : Get3DCoords(e.Location, 2.0f);
                }

                

                SnapToGrid(ref obj.Position);
                obj.GenerateProperties();

                if (obj.m_Properties.Properties.IndexOf("X position") != -1)
                {
                    btnCopyCoordinates.Visible = true;
                    btnPasteCoordinates.Visible = true;
                }
                


                m_Selected = obj.m_UniqueID;
                m_SelectedObject = obj;
                m_LastSelected = obj.m_UniqueID;
                m_Hovered = obj.m_UniqueID;
                m_HoveredObject = obj;
                m_LastHovered = obj.m_UniqueID;
                m_LastClicked = obj.m_UniqueID;

                UpdateObjectForRawEditor();
                initializePropertyInterface();

                RefreshObjects(m_SelectedObject.m_Layer);

                if (!m_ShiftPressed)
                {
                    m_ObjectBeingPlaced = 0xFFFF;
                    slStatusLabel.Text = "Object added.";
                }
            }
            else if ((m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
            {
                if (btnRemoveSel.Checked)
                {
                    uint sel = m_PickingFrameBuffer[4];
                    EditMode type = (EditMode)(sel >> 28);
                    if (type == m_EditMode)
                    {
                        LevelObject obj = m_Level.m_LevelObjects[sel];
                        RemoveObject(obj);
                        RefreshObjects(obj.m_Layer);

                        if (!m_ShiftPressed)
                        {
                            btnRemoveSel.Checked = false;
                            slStatusLabel.Text = "Object removed.";
                        }
                    }
                }
                else
                    m_LastClicked = m_PickingFrameBuffer[4];
            }

            m_MouseDown = e.Button;
            m_LastMouseClick = e.Location;
            m_LastMouseMove = e.Location;

            if (m_LastSelected != 0xFFFFFFFF && m_LastSelected == m_LastClicked) {
                try
                {
                    m_SelObjPrevPos = m_SelectedObject.Position;
                }
                catch (Exception exept)
                {
                    Console.WriteLine("error is happening");
                }
            }

            if(e.Button == MouseButtons.Left)
                m_SelObjTotalMov = Vector3.Zero;

        }

        private void glLevelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != m_MouseDown) return;

            if ((Math.Abs(e.X - m_LastMouseClick.X) < 3) && (Math.Abs(e.Y - m_LastMouseClick.Y) < 3) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
            {
                uint sel = m_PickingFrameBuffer[4];
                EditMode type = (EditMode)(sel >> 28);

                if (type == m_EditMode && type != 0 && type != EditMode.MISC)
                {
                    m_Selected = sel;

                    if (m_LastSelected != m_Selected)
                    {
                        LevelObject obj = m_Level.m_LevelObjects[sel];
                        RenderObjectHilite(obj, k_SelectionColor, m_SelectHiliteDL);
                        m_LastSelected = m_Selected;
                        m_SelectedObject = obj;

                        UpdateObjectForRawEditor();
                        initializePropertyInterface();

                        if (obj.m_Properties.Properties.IndexOf("X position") != -1)
                        {
                            btnCopyCoordinates.Visible = true;
                            btnPasteCoordinates.Visible = true;
                        }

                        tvObjectList.SelectedNode = tvObjectList.Nodes.Find(obj.m_UniqueID.ToString("X8"), true)[0];
                    }
                }
                else
                {
                    m_Selected = 0xFFFFFFFF;
                    m_LastSelected = 0xFFFFFFFF;
                    m_SelectedObject = null;

                    UpdateObjectForRawEditor();
                    clearPropertyInterface();

                    btnCopyCoordinates.Visible = false;
                    btnPasteCoordinates.Visible = false;

                    tvObjectList.SelectedNode = null;
                }
            }

            m_MouseDown = MouseButtons.None;

            if (e.Button == MouseButtons.Left)
                m_SelObjTotalMov = Vector3.Zero;
        }

        private void glLevelView_MouseMove(object sender, MouseEventArgs e)
        {
            float xdelta = (float)(e.X - m_LastMouseMove.X);
            float ydelta = (float)(e.Y - m_LastMouseMove.Y);
            float orthPixelFactor = m_OrthZoom / glLevelView.Width;

            m_MouseCoords = e.Location;
            m_LastMouseMove = e.Location;

            //if (m_SelectedObject != null)
            //    m_SelectedObject.m_TestMatrix.M11 *= 1.001f;

            if (m_MouseDown != MouseButtons.None)
            {
                if (m_LastSelected == 0xFFFFFFFF || m_LastSelected != m_LastClicked)
                {
                    if (m_MouseDown == MouseButtons.Right)
                    {
                        /*if (btnReverseRot.Checked)
                        {
                            xdelta = -xdelta;
                            ydelta = -ydelta;
                        }*/

                        if (m_UpsideDown)
                            xdelta = -xdelta;

                        m_CamRotation.X -= xdelta * 0.002f;
                        m_CamRotation.Y -= ydelta * 0.002f;
                        //m_CamRotation.X -= (float)Math.Tan((xdelta * m_PixelFactorX) / m_PickingDepth);//xdelta * m_PixelFactorX * m_PickingDepth;
                        //m_CamRotation.Y -= ydelta * m_PixelFactorY * m_PickingDepth;

                        ClampRotation(ref m_CamRotation.X, (float)Math.PI * 2.0f);
                        ClampRotation(ref m_CamRotation.Y, (float)Math.PI * 2.0f);
                    }
                    else if (m_MouseDown == MouseButtons.Left)
                    {
                        //xdelta *= 0.005f;
                        //ydelta *= 0.005f;
                        if (m_OrthView)
                        {
                            xdelta *= orthPixelFactor;
                            ydelta *= orthPixelFactor;
                        }
                        else
                        {
                            xdelta *= Math.Min(0.005f, m_PixelFactorX * m_PickingDepth);
                            ydelta *= Math.Min(0.005f, m_PixelFactorY * m_PickingDepth);
                        }

                        m_CamTarget.X -= xdelta * (float)Math.Sin(m_CamRotation.X);
                        m_CamTarget.X -= ydelta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Sin(m_CamRotation.Y);
                        m_CamTarget.Y += ydelta * (float)Math.Cos(m_CamRotation.Y);
                        m_CamTarget.Z += xdelta * (float)Math.Cos(m_CamRotation.X);
                        m_CamTarget.Z -= ydelta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Sin(m_CamRotation.Y);
                    }

                    UpdateCamera();
                }
                else
                {
                    if (m_MouseDown == MouseButtons.Right)
                    {
                        if (m_UpsideDown)
                            xdelta = -xdelta;

                        // TODO take obj/camera rotation into account?
                        if (m_SelectedObject.SupportsRotation())
                        {
                            m_SelectedObject.YRotation += xdelta * 0.5f;

                            if (m_SelectedObject.YRotation >= 180f)
                            {
                                m_SelectedObject.YRotation = (float)(-360f + m_SelectedObject.YRotation);
                            }
                            else if (m_SelectedObject.YRotation < -180f)
                            {
                                m_SelectedObject.YRotation = (float)(360f + m_SelectedObject.YRotation);
                            }
                        }
                    }
                    else if (m_MouseDown == MouseButtons.Left)
                    {
                        Vector3 between;
                        Vector3.Subtract(ref m_CamPosition, ref m_SelectedObject.Position, out between);

                        float objz = (((between.X * (float)Math.Cos(m_CamRotation.X)) + (between.Z * (float)Math.Sin(m_CamRotation.X))) * (float)Math.Cos(m_CamRotation.Y)) + (between.Y * (float)Math.Sin(m_CamRotation.Y));
                        //float objz = m_PickingDepth;
                        if (m_OrthView)
                        {
                            xdelta *= orthPixelFactor;
                            ydelta *= -orthPixelFactor;
                        }
                        else
                        {
                            xdelta *= m_PixelFactorX * objz;
                            ydelta *= -m_PixelFactorY * objz;
                        }

                        float _xdelta = (xdelta * (float)Math.Sin(m_CamRotation.X)) - (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                        float _ydelta = ydelta * (float)Math.Cos(m_CamRotation.Y);
                        float _zdelta = (xdelta * (float)Math.Cos(m_CamRotation.X)) + (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                        m_SelObjTotalMov.X += _xdelta;
                        m_SelObjTotalMov.Y += _ydelta;
                        m_SelObjTotalMov.Z -= _zdelta;

                        if (m_RestrPlaneEnabled)
                        {
                            Vector3 start = Get3DCoords(e.Location, k_zNear);
                            Vector3 dir = Get3DCoords(e.Location, k_zFar) - Get3DCoords(e.Location, k_zNear);
                            Vector3? hit = null;

                            //Snap to restriction plane
                            float dot = Vector3.Dot(m_RestrPlaneNormal, dir);
                            if (dot != 0)
                            {
                                float t = Vector3.Dot(m_RestrPlaneNormal, m_SelObjPrevPos - start) / dot;
                                hit = dir * t + start;
                            }

                            m_SelectedObject.Position = (hit != null) ? (Vector3)hit : m_SelObjPrevPos + m_SelObjTotalMov;
                        }
                        else
                        {
                            m_SelectedObject.Position = m_SelObjPrevPos + m_SelObjTotalMov;
                        }
                        SnapToGrid(ref m_SelectedObject.Position);
                    }

                    updatePropertyInterface();
                }
            }
            //else
            {
                if ((m_PickingFrameBuffer[4] == m_PickingFrameBuffer[1]) &&
                    (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[3]) &&
                    (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[5]) &&
                    (m_PickingFrameBuffer[4] == m_PickingFrameBuffer[7]))
                {
                    uint sel = m_PickingFrameBuffer[4];
                    EditMode type = (EditMode)(sel >> 28);
                    if (type == m_EditMode)
                    {
                        m_Hovered = sel;
                        if ((type == EditMode.MODEL) || (type == EditMode.MISC))
                        {
                            m_LastHovered = 0xFFFFFFFF;
                            m_HoveredObject = null;
                        }
                        else
                        {
                            if (m_LastHovered != m_Hovered)
                            {
                                try
                                {
                                    LevelObject obj = m_Level.m_LevelObjects[sel];
                                    RenderObjectHilite(obj, k_HoverColor, m_HoverHiliteDL);
                                    m_LastHovered = m_Hovered;
                                    m_HoveredObject = obj;
                                }
                                catch (Exception x)
                                {
                                    Console.WriteLine("hovered over: " + sel);
                                }
                            }
                        }
                    }
                    else
                    {
                        m_Hovered = 0xFFFFFFFF;
                        m_LastHovered = 0xFFFFFFFF;
                        m_HoveredObject = null;
                    }
                }
            }

            glLevelView.Refresh();
        }

        private void glLevelView_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((m_MouseDown == MouseButtons.Left) && ((m_Selected >> 28) != 0xF) && (m_LastClicked == m_Selected))
            {
                float delta = -(e.Delta / 120f);
                delta = ((delta < 0f) ? -1f : 1f) * (float)Math.Pow(delta, 2f) * 0.05f;
                
                m_SelObjTotalMov.Y += delta * (float)Math.Sin(m_CamRotation.Y);
                m_SelObjTotalMov.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
                m_SelObjTotalMov.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

                float xdist = delta * (m_MouseCoords.X - (glLevelView.Width / 2f)) * m_PixelFactorX;
                float ydist = delta * (m_MouseCoords.Y - (glLevelView.Height / 2f)) * m_PixelFactorY;

                m_SelObjTotalMov.X -= (xdist * (float)Math.Sin(m_CamRotation.X)) + (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                m_SelObjTotalMov.Y += ydist * (float)Math.Cos(m_CamRotation.Y);
                m_SelObjTotalMov.Z += (xdist * (float)Math.Cos(m_CamRotation.X)) - (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                m_SelectedObject.Position = m_SelObjPrevPos + m_SelObjTotalMov;
                
                updatePropertyInterface();;
            }
            else
            {
                float delta = -((e.Delta / 120.0f) * 0.1f);
                ZoomCamera(delta);
            }

            glLevelView.Refresh();
        }

        private void ZoomCamera(float delta)
        {
            m_CamTarget.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
            m_CamTarget.Y += delta * (float)Math.Sin(m_CamRotation.Y);
            m_CamTarget.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

            UpdateCamera();
        }

        private void FocusCamera(Vector3 position)
        {
            m_CamTarget = position;
            UpdateCamera();
            glLevelView.Refresh();
        }

        private Vector3 Get3DCoords(Point coords2d, float depth)
        {
            Vector3 ret = m_CamPosition;

            float orthPixelFactor = m_OrthZoom / glLevelView.Width;

            ret.X -= (depth * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y));
            ret.Y -= (depth * (float)Math.Sin(m_CamRotation.Y));
            ret.Z -= (depth * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y));

            float x = (coords2d.X - (glLevelView.Width / 2f)) * (m_OrthView ? orthPixelFactor : m_PixelFactorX * depth);
            float y = -(coords2d.Y - (glLevelView.Height / 2f)) * (m_OrthView ? orthPixelFactor : m_PixelFactorY * depth);

            ret.X += (x * (float)Math.Sin(m_CamRotation.X)) - (y * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
            ret.Y += y * (float)Math.Cos(m_CamRotation.Y);
            ret.Z -= (x * (float)Math.Cos(m_CamRotation.X)) + (y * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

            return ret;
        }

        private Bitmap DumpOpenGLRenderingToBMP()
        {
            int width = glLevelView.Width;
            int height = glLevelView.Height;

            Bitmap dump = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bData =
                dump.LockBits(new Rectangle(0, 0, dump.Width, dump.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, dump.PixelFormat);

            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, bData.Scan0);

            dump.UnlockBits(bData);
            dump.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return dump;
        }

        private void LevelEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save confirmation goes here
            ReleaseModels();
            Program.m_LevelEditors.Remove(this);
        }

        private void ReleaseModels()
        {
            foreach (LevelObject obj in m_Level.m_LevelObjects.Values)
                obj.Release();

            foreach (int dl in m_LevelModelDLs)
                GL.DeleteLists(dl, 1);

            foreach (int dl in m_ObjectDLs)
                GL.DeleteLists(dl, 1);

            if (m_SkyboxModel != null)
                ModelCache.RemoveModel(m_SkyboxModel);

            GL.DeleteLists(m_SelectHiliteDL, 1);
            GL.DeleteLists(m_HoverHiliteDL, 1);
        }


        private void btnStarX_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;

            if (btn.Checked)
            {
                btn.Checked = false;
                m_AuxLayerNum = 0;
            }
            else
            {
                btnStar1.Checked = false;
                btnStar2.Checked = false;
                btnStar3.Checked = false;
                btnStar4.Checked = false;
                btnStar5.Checked = false;
                btnStar6.Checked = false;
                btnStar7.Checked = false;
                btn.Checked = true;
                m_AuxLayerNum = int.Parse((string)btn.Tag);
            }

            if (m_SelectedObject!=null)
            {
                
                if (m_SelectedObject.m_Layer!=m_AuxLayerNum)
                {
                    m_Selected = m_LastSelected = 0xFFFFFFFF;
                    m_SelectedObject = null;

                    UpdateObjectForRawEditor();
                    clearPropertyInterface();
                }
                
            }

            PopulateObjectList();
            glLevelView.Refresh();
        }

        private void btnStarAll_Click(object sender, EventArgs e)
        {
            m_ShowCommonLayer = btnStarAll.Checked;
            if (m_SelectedObject != null)
            {

                if ((m_SelectedObject.m_Layer == 0)&&(!m_ShowCommonLayer))
                {
                    m_Selected = m_LastSelected = 0xFFFFFFFF;
                    m_SelectedObject = null;

                    UpdateObjectForRawEditor();
                    clearPropertyInterface();
                }

            }
            PopulateObjectList();
            glLevelView.Refresh();
        }

        private void btnEditXXX_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            if (btn.Checked) return;

            btnEdit3DModel.Checked = false;
            btnEditObjects.Checked = false;
            btnEditWarps.Checked = false;
            btnEditPaths.Checked = false;
            btnEditViews.Checked = false;
            btnEditMisc.Checked = false;
            btn.Checked = true;

            EditMode lastMode = m_EditMode;
            m_EditMode = (EditMode)int.Parse((string)btn.Tag);

            for (int l = 0; l < 8; l++)
            {
                if (l == m_AuxLayerNum)
                {
                    RefreshObjects(l);
                    if ((lastMode==EditMode.MODEL)!= (m_EditMode == EditMode.MODEL))
                    {
                        RefreshObjects(0);
                        if (m_EditMode != EditMode.MODEL)
                            RenderLevelAreas(-1);
                    }
                }
                else
                    RenderObjectLists(RenderMode.Picking, l);
                
            }

            PopulateObjectList();

            glLevelView.Refresh();
        }

        private int m_StatusMarqueeOffset = 0;
        private void StatusMarqueeUpdate(object sender, ElapsedEventArgs e)
        {
            slStatusLabel.Invalidate();
            ssStatusBar.Refresh();
        }
        private void slStatusLabel_Paint(object sender, PaintEventArgs e)
        {
            string text = slStatusLabel.Text;
            int txtwidth = (int)e.Graphics.MeasureString(text, slStatusLabel.Font).Width - 12;
            if (txtwidth > e.ClipRectangle.Width)
            {
                text += " -- ";
                txtwidth = (int)e.Graphics.MeasureString(text, slStatusLabel.Font).Width - 12;

                System.Timers.Timer tmr = new System.Timers.Timer(30);
                tmr.AutoReset = false;
                tmr.Elapsed += new ElapsedEventHandler(StatusMarqueeUpdate);
                tmr.Start();

                m_StatusMarqueeOffset -= 2;
                if (-m_StatusMarqueeOffset >= txtwidth)
                    m_StatusMarqueeOffset = 0;
            }
            else
                m_StatusMarqueeOffset = 0;

            e.Graphics.FillRectangle(new SolidBrush(slStatusLabel.BackColor), e.ClipRectangle);
            e.Graphics.DrawString(text, slStatusLabel.Font, new SolidBrush(slStatusLabel.ForeColor), new PointF(m_StatusMarqueeOffset, 0));
            if (m_StatusMarqueeOffset < 0)
                e.Graphics.DrawString(text, slStatusLabel.Font, new SolidBrush(slStatusLabel.ForeColor), new PointF(m_StatusMarqueeOffset + txtwidth, 0));
        }
        private void slStatusLabel_TextChanged(object sender, EventArgs e)
        {
            m_StatusMarqueeOffset = 0;
        }

        private void btnImportModel_Click(object sender, EventArgs e)
        {
            string bmdName = Program.m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.BMDFileID).m_Name;
            string kclName = Program.m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.KCLFileID).m_Name;
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor form =
                    new ModelAndCollisionMapEditor(bmdName, kclName);
                form.Show();
            }
            else
            {
                ModelImporter form = new ModelImporter(bmdName, kclName);
                form.Show(this);
            }
        }

        private void tvObjectList_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Color fgcolor, bgcolor;

            Font font = e.Node.NodeFont;
            if (font == null) font = tvObjectList.Font;

            bool red = false;
            if (e.Node.Tag is uint && m_Level.m_LevelObjects.ContainsKey((uint)e.Node.Tag))
            {
                uint uniqueid = (uint)e.Node.Tag;
                if ((uniqueid >> 28) == 1)
                    red = m_Level.m_LevelObjects[uniqueid].ID >= LevelObject.NUM_OBJ_TYPES ?
                        true : !m_Level.m_ObjAvailable[m_Level.m_LevelObjects[uniqueid].ID];
            }

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                fgcolor = red ? Color.LightPink : SystemColors.HighlightText;
                bgcolor = SystemColors.Highlight;
            }
            else
            {
                fgcolor = red ? Color.Red : SystemColors.ControlText;
                bgcolor = SystemColors.ControlLightLight;
            }

            // apparently we can't rely on e.Bounds, we have to calculate the size of the string ourselves
            Rectangle txtbounds = e.Bounds;
            SizeF txtsize = e.Graphics.MeasureString(e.Node.Text, font);
            txtbounds.Width = (int)txtsize.Width;

            e.Graphics.FillRectangle(new SolidBrush(bgcolor), txtbounds);
            e.Graphics.DrawString(e.Node.Text, font, new SolidBrush(fgcolor), (float)e.Bounds.X, (float)e.Bounds.Y + 1f);
        }

        private void btnLevelSettings_Click(object sender, EventArgs e)
        {
            new LevelSettingsForm(m_Level).ShowDialog(this);
            m_Level.DetermineAvailableObjects();
            tvObjectList.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_Level.ContainsObjectsIncompatibleWithBankSettings())
            {
                DialogResult res = MessageBox.Show("This level contains objects which aren't available with the current object bank settings, and would crash the game.\n\nSave anyway?",
                    Program.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            }

            m_Level.SaveChanges();
            
            slStatusLabel.Text = "Changes saved.";
        }

        private void tvObjectList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_EditMode==EditMode.MODEL)
            {
                m_currentArea = (int)e.Node.Tag;
                RenderLevelAreas(m_currentArea);
                RefreshObjects(m_AuxLayerNum);
                RefreshObjects(0);
                return;
            }
            if (e.Node.Tag == null)
            {
                m_SelectedObject = null;
                m_Selected = m_LastSelected = 0xFFFFFFFF;
                return;
            }

            uint objid = (uint)e.Node.Tag;
            m_Selected = m_LastSelected = objid;
            m_SelectedObject = m_Level.m_LevelObjects[objid];

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            if (m_SelectedObject.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            } else
            {
                btnCopyCoordinates.Visible = false;
                btnPasteCoordinates.Visible = false;
            }


            if (m_SelectedObject.m_Type == LevelObject.Type.PATH)
            {
                // If object selected is a path, highlight all nodes in current path
                List<LevelObject> pathNodes = m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE)
                    .OrderBy(obj => ((PathPointObject)obj).m_NodeID).ToList();
                List<LevelObject> nodes = new List<LevelObject>();
                for (int i = ((PathObject)m_SelectedObject).Parameters[0]; i < (((PathObject)m_SelectedObject).Parameters[0] + ((PathObject)m_SelectedObject).Parameters[1]); i++)
                {
                    PathPointObject node = (PathPointObject)pathNodes[i];
                    nodes.Add(node);
                }
                PropertyTable ptable = m_SelectedObject.m_Properties;
                RenderPathHilite(nodes, ((float)ptable["Parameter 5"]==255.0f),k_SelectionColor, m_SelectHiliteDL);
                btnAddPathNode.Visible = true;
            }
            else
            {
                RenderObjectHilite(m_SelectedObject, k_SelectionColor, m_SelectHiliteDL);
                if (m_SelectedObject.m_Type == LevelObject.Type.PATH_NODE)
                    btnAddPathNode.Visible = true;
            }
            glLevelView.Refresh();
        }

        private void btnAddObject_Click(object sender, EventArgs e)
        {
            btnRemoveSel.Checked = false;

            ObjectListForm objlist = new ObjectListForm(0);
            if (objlist.ShowDialog(this) != DialogResult.OK) return;
            if (objlist.ObjectID >= LevelObject.NUM_OBJ_TYPES && objlist.ObjectID != 0x1FF) return;

            m_ObjectBeingPlaced = objlist.ObjectID;
            string placementMsg = "Click anywhere in the level to place your new object ({0} - {1}). Hold Shift while clicking to place multiple objects. Hit Escape to abort.";
            slStatusLabel.Text = (objlist.ObjectID < LevelObject.NUM_OBJ_TYPES) ? 
                string.Format(placementMsg, objlist.ObjectID, ObjectDatabase.m_ObjectInfo[objlist.ObjectID].m_Name) : 
                string.Format(placementMsg, 511, "Minimap change");
        }

        private void btnAddWhatever_Click(object sender, EventArgs e)
        {
            btnRemoveSel.Checked = false;

            uint type = uint.Parse((string)((ToolStripItem)sender).Tag);
            m_ObjectBeingPlaced = type << 16;

            string obj = "OSHIT BUG";
            switch ((LevelObject.Type)type)
            {
                case LevelObject.Type.ENTRANCE: obj = "entrance"; break;
                case LevelObject.Type.VIEW: obj = "view"; break;
                case LevelObject.Type.TELEPORT_SOURCE: obj = "teleport source"; break;
                case LevelObject.Type.TELEPORT_DESTINATION: obj = "teleport destination"; break;
                case LevelObject.Type.DOOR: obj = "door"; break;
                case LevelObject.Type.EXIT: obj = "exit"; break;
            }

            slStatusLabel.Text = "Click anywhere in the level to place your new " + obj + ". Hold Shift while clicking to place multiple " + obj + "s. Hit Escape to abort.";
        }

        private void btnRemoveSel_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject == null)
            {
                if (btnRemoveSel.Checked)
                {
                    btnRemoveSel.Checked = false;
                    slStatusLabel.Text = "Ready";
                    return;
                }

                slStatusLabel.Text = "Click the object you want to remove. Hold Shift while clicking to remove multiple objects. Hit Escape to abort.";
                btnRemoveSel.Checked = true;

                return;
            }

            LevelObject obj = m_SelectedObject;
            RemoveObject(obj);
            RefreshObjects(obj.m_Layer);
            m_SelectedObject = m_nextObjectToSelect;

            UpdateObjectForRawEditor();
            clearPropertyInterface();
            slStatusLabel.Text = "Object removed.";

            m_nextObjectToSelect = null;
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = tvObjectList.SelectedNode;
            int layer = 0;
            switch (m_EditMode)
            {
                case EditMode.OBJECTS:
                    if (parentNode == null)
                    {
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.STANDARD).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.SIMPLE).ToList());
                    }
                    else
                    {
                        List<LevelObject> objs = new List<LevelObject>();
                        foreach (TreeNode node in parentNode.Nodes)
                        {
                            objs.Add(m_Level.m_LevelObjects[uint.Parse(node.Tag.ToString())]);
                        }
                        RemoveObjects(objs);
                    }
                    layer = m_AuxLayerNum;
                    slStatusLabel.Text = "Objects removed.";
                    break;
                case EditMode.WARPS:
                    if (parentNode == null)
                    {
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.ENTRANCE).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.EXIT).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.DOOR).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.TELEPORT_SOURCE).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.TELEPORT_DESTINATION).ToList());
                        slStatusLabel.Text = "Warp objects removed.";
                    }
                    else
                    {
                        switch (parentNode.Name)
                        {
                            case "entrance":
                                RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.ENTRANCE).ToList());
                                slStatusLabel.Text = "Entrance objects removed.";
                                break;
                            case "exit":
                                RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.EXIT).ToList());
                                slStatusLabel.Text = "Exit objects removed.";
                                break;
                            case "door":
                                RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.DOOR).ToList());
                                slStatusLabel.Text = "Door objects removed.";
                                break;
                            case "teleport_source":
                                RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.TELEPORT_SOURCE).ToList());
                                slStatusLabel.Text = "Teleport source objects removed.";
                                break;
                            case "teleport_destination":
                                RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.TELEPORT_DESTINATION).ToList());
                                slStatusLabel.Text = "Teleport destination objects removed.";
                                break;
                        }
                    }
                    break;
                case EditMode.PATHS:
                    if (parentNode == null)
                    {
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE).ToList());
                        RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.PATH).ToList());
                        slStatusLabel.Text = "Paths removed.";
                    }
                    else 
                    {
                        if (parentNode.Name == "paths")
                        {
                            RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.PATH).ToList());
                            slStatusLabel.Text = "Path objects removed.";
                        }
                        else if (parentNode.Name.StartsWith("path"))
                        {
                            RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.PATH_NODE).ToList());
                            slStatusLabel.Text = "Path node objects removed.";
                        }
                    }
                    break;
                case EditMode.VIEWS:
                    RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.VIEW).ToList());
                    slStatusLabel.Text = "View objects removed.";
                    break;
                case EditMode.MISC:
                    RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.FOG).ToList());
                    RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.MINIMAP_TILE_ID).ToList());
                    RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.MINIMAP_SCALE).ToList());
                    RemoveObjects(m_Level.GetAllObjectsByType(LevelObject.Type.UNKNOWN_14).ToList());
                    slStatusLabel.Text = "Miscellaneous objects removed.";
                    break;
            }

            RefreshObjects(layer);
        }

        private void glLevelView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.RShiftKey)
                m_ShiftPressed = true;
            else if (e.KeyCode == Keys.Escape)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                btnRemoveSel.Checked = false;
                slStatusLabel.Text = "Ready";
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (m_SelectedObject != null)
                    btnRemoveSel.PerformClick(); // quick cheat
            }

            if (e.KeyCode == Keys.Q)
                btnLOL.PerformClick();

            // Front, side, top, left, back, and bottom views
            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up: //Top
                        m_CamRotation.X = (float)Math.PI / 2;
                        m_CamRotation.Y = (float)Math.PI / 2;
                        UpdateCamera(); glLevelView.Refresh(); break;

                    case Keys.Down: //Bottom
                        m_CamRotation.X = (float)Math.PI / 2;
                        m_CamRotation.Y = -(float)Math.PI / 2;
                        UpdateCamera(); glLevelView.Refresh(); break;

                    case Keys.Left: //Left
                        m_CamRotation.X = (float)Math.PI;
                        m_CamRotation.Y = 0;
                        UpdateCamera(); glLevelView.Refresh(); break;

                    case Keys.Right: //Right
                        m_CamRotation.X = 0;
                        m_CamRotation.Y = 0;
                        UpdateCamera(); glLevelView.Refresh(); break;

                    case Keys.F: //Front
                        m_CamRotation.X = (float)Math.PI / 2;
                        m_CamRotation.Y = 0;
                        UpdateCamera(); glLevelView.Refresh(); break;

                    case Keys.B: //Back
                        m_CamRotation.X = -(float)Math.PI / 2;
                        m_CamRotation.Y = 0;
                        UpdateCamera(); glLevelView.Refresh(); break;

                }
            }

            // Orthognal View Zoom
            if (e.KeyCode == Keys.PageUp)
            {
                m_OrthZoom -= 1f;
                glLevelView.Refresh();
            }
            if (e.KeyCode == Keys.PageDown)
            {
                m_OrthZoom += 1f;
                glLevelView.Refresh();
            }

            // Standard View Zoom / Orthogonal View Slice
            if (e.KeyCode == Keys.Home)
            {
                ZoomCamera(-0.5f);
                glLevelView.Refresh();
            }
            if (e.KeyCode == Keys.End)
            {
                ZoomCamera(0.5f);
                glLevelView.Refresh();
            }
        }

        private void glLevelView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.RShiftKey)
                m_ShiftPressed = false;

            // Copy and Paste Objects
            if (e.Control && e.KeyCode == Keys.C)
                m_CopiedObject = m_SelectedObject.Copy();
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (m_CopiedObject != null)
                    CopyObject(m_CopiedObject);
            }
        }

        private void btnStarAll_DoubleClick(object sender, EventArgs e)
        {
            btnStarAll.Checked = !btnStarAll.Checked;
            m_ShowCommonLayer = btnStarAll.Checked;
            glLevelView.Refresh();
            return;
        }

        private void btnDumpOverlay_Click(object sender, EventArgs e)
        {
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, m_ROM.GetLevelOverlayID(m_LevelID));
            string filename = "level" + m_LevelID.ToString() + "_overlay.bin";
            System.IO.File.WriteAllBytes(filename, ovl.m_Data);
            slStatusLabel.Text = "Level overlay dumped to " + filename;
        }

        private void btnLOL_Click(object sender, EventArgs e)
        {
            lol++;
            if (lol >= KCL.OctreeNode.m_List.Count) lol = 0;
            glLevelView.Refresh();
        }

        private void btnEditMinimap_Click(object sender, EventArgs e)
        {
            new MinimapEditor(m_Level).Show(this);
        }

        private void btnReplaceObjModel_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject == null)
            {
                slStatusLabel.Text = "Click the object whose model you want to replace.";

                return;
            }

            if (null == m_SelectedObject.m_Renderer.GetFilename())
            {
                slStatusLabel.Text = "This object uses more than one model, use the 'Model and Collision Map Importer' to replace them.";
                return;
            }

            //Get the name of the selected object's BMD (model) file
            string selObjBMDName = m_SelectedObject.m_Renderer.GetFilename();
            //Get the name of the selected object's KCL (collision data) file
            string selObjKCLName = selObjBMDName.Substring(0, selObjBMDName.Length - 4) + ".kcl";

            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor form =
                    new ModelAndCollisionMapEditor(selObjBMDName, selObjKCLName, m_SelectedObject.m_Renderer.GetScale().X);
                form.ShowDialog(this);
            }
            else
            {
                ModelImporter form = new ModelImporter(selObjBMDName, selObjKCLName, m_SelectedObject.m_Renderer.GetScale().X);
                if (form != null && !form.m_EarlyClosure)
                    form.ShowDialog(this);
            }

            ModelCache.RemoveModel(m_SelectedObject.m_Renderer.GetFilename());
        }

        private void btnEditTexAnim_Click(object sender, EventArgs e)
        {
            new TextureAnimationForm(m_Level).Show(this);
        }

        private void btnCLPS_Click(object sender, EventArgs e)
        {
            new CLPS_Form(m_Level.m_CLPS).Show(this);
        }

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            uint type0 = 3;
            m_ObjectBeingPlaced = type0 << 16;

            LevelObject.Type type = (LevelObject.Type)(m_ObjectBeingPlaced >> 16);
            ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);

            LevelObject obj = AddObject(type, id, 0, 0);
            obj.GenerateProperties();

            if (obj.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            }

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            RefreshObjects(m_SelectedObject.m_Layer);

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        private int m_CurrentPathID = -1;

        private void btnAddPathNode_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject == null)
                return;
            m_ObjectBeingPlaced = (int)LevelObject.Type.PATH_NODE << 16;

            int nodeIndex = -1;
            if (m_SelectedObject.m_Type == LevelObject.Type.PATH)
            {
                m_CurrentPathID = ((PathObject)m_SelectedObject).m_PathID;
            }
            else if (m_SelectedObject.m_Type == LevelObject.Type.PATH_NODE)
            {
                PathPointObject pathPoint = (PathPointObject)m_SelectedObject;
                m_CurrentPathID = pathPoint.ParentPath;
                nodeIndex = pathPoint.m_IndexInPath+1;
            }
            else return;

            PathPointObject obj = m_Level.AddPathPointObject(m_CurrentPathID,nodeIndex);
            
            if (obj.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            }

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            RefreshObjects(m_SelectedObject.m_Layer);
            PopulatePathNodes(m_CurrentPathID);

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        void btnAddMisc_DropDownItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            int selected = int.Parse(e.ClickedItem.Tag.ToString());

            uint type0 = (uint)selected;
            m_ObjectBeingPlaced = type0 << 16;

            LevelObject.Type type = (LevelObject.Type)(m_ObjectBeingPlaced >> 16);
            ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);

            LevelObject obj = AddObject(type, id, 0, 0);
            obj.GenerateProperties();

            if (obj.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            }
            
            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            RefreshObjects(m_SelectedObject.m_Layer);

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        private bool ExportModel(BMD bmd)
        {
            m_SaveFileDialogue.FileName = bmd.m_File.m_Name;
            m_SaveFileDialogue.Filter = Strings.MODEL_EXPORT_FORMATS_FILTER;
            m_SaveFileDialogue.DefaultExt = ".dae";
            if (m_SaveFileDialogue.ShowDialog() == DialogResult.OK)
            {
                BMD_BCA_KCLExporter.ExportBMDModel(bmd, m_SaveFileDialogue.FileName);
                return true;
            }
            return false;
        }

        private void btnExportLevelModel_Click(object sender, EventArgs e)
        {
            if (ExportModel(new BMD(m_ROM.GetFileFromInternalID(m_Level.m_LevelSettings.BMDFileID))))
            {
                slStatusLabel.Text = "Finished exporting level model.";
            }
        }

        private void btnExportObjectModel_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject == null)
            {
                slStatusLabel.Text = "Click the object whose model you want to export.";
                return;
            }

            //Get the name of the selected object's BMD (model) file(s)
            string selObjBMDName = m_SelectedObject.m_Renderer.GetFilename();
            string[] names = selObjBMDName.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string name in names)
            {
                BMD objectBMD = new BMD(m_ROM.GetFileFromName(name));

                if (!ExportModel(objectBMD))
                    return;
            }

            slStatusLabel.Text = "Finished exporting model.";
        }

        private void btnImportOtherModel_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.Text = "Select a model (BMD) file to replace.";
            DialogResult result = m_ROMFileSelect.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                String modelName = m_ROMFileSelect.m_SelectedFile;
                ModelImporter mdlImp = new ModelImporter(modelName, modelName.Substring(0, modelName.Length - 4) + ".kcl");
                if (mdlImp != null && !mdlImp.m_EarlyClosure)
                    mdlImp.ShowDialog(this);
            }
        }

        private void btnExportOtherModel_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.Text = "Select a model (BMD) file to export.";
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                BMD objectBMD = new BMD(m_ROM.GetFileFromName(m_ROMFileSelect.m_SelectedFile));

                if (ExportModel(objectBMD))
                    slStatusLabel.Text = "Finished exporting model.";
            }
        }

        private void menuGridSettings_DropDownClosed(object sender, EventArgs e)
        {
            ToolStripDropDownButton dropdown = (ToolStripDropDownButton)sender;
            ToolStripItemCollection items = dropdown.DropDownItems;

            Helper.TryParseFloat(items[items.IndexOfKey("txtGridSizeX")].Text, out m_GridSize.X);
            Helper.TryParseFloat(items[items.IndexOfKey("txtGridSizeY")].Text, out m_GridSize.Y);
            Helper.TryParseFloat(items[items.IndexOfKey("txtGridSizeZ")].Text, out m_GridSize.Z);

            Helper.TryParseFloat(items[items.IndexOfKey("txtGridOffsetX")].Text, out m_GridOffset.X);
            Helper.TryParseFloat(items[items.IndexOfKey("txtGridOffsetY")].Text, out m_GridOffset.Y);
            Helper.TryParseFloat(items[items.IndexOfKey("txtGridOffsetZ")].Text, out m_GridOffset.Z);
        }

        private void menuRestrictionPlane_DropDownClosed(object sender, EventArgs e)
        {
            ToolStripDropDownButton dropdown = (ToolStripDropDownButton)sender;
            ToolStripItemCollection items = dropdown.DropDownItems;

            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneX")].Text, out m_RestrPlaneNormal.X);
            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneY")].Text, out m_RestrPlaneNormal.Y);
            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneZ")].Text, out m_RestrPlaneNormal.Z);

            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneOffX")].Text, out m_RestrPlaneOffset.X);
            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneOffY")].Text, out m_RestrPlaneOffset.Y);
            Helper.TryParseFloat(items[items.IndexOfKey("txtRstPlaneOffZ")].Text, out m_RestrPlaneOffset.Z);
        }

        private void btnMakeOverlay_Click(object sender, EventArgs e)
        {
            m_FolderBrowserDialogue.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            DialogResult result = m_FolderBrowserDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                NitroOverlay ovl = new NitroOverlay(Program.m_ROM, m_Level.m_LevelSettings.ObjectBanks[7] + 7);

                DirectoryInfo dir = new DirectoryInfo(m_FolderBrowserDialogue.SelectedPath);
                Patcher.PatchMaker pm = new Patcher.PatchMaker(dir, ovl.GetRAMAddr());
                pm.compilePatch();
                pm.makeOverlay(m_Level.m_LevelSettings.ObjectBanks[7] + 7);
            }
        }

        private void btnCopyCoordinates_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject != null)
            {
                btnPasteCoordinates.Enabled = true;
                m_copiedPosition = m_SelectedObject.Position;
            }
        }

        private void btnPasteCoordinates_Click(object sender, EventArgs e)
        {
            if (m_SelectedObject != null)
            {
                m_SelectedObject.Position = m_copiedPosition;
                updatePropertyInterface();;
            }
                
        }

        private void btnOffsetAllCoords_Click(object sender, EventArgs e)
        {
            new OffsetAllObjectCoordsForm().Show(this);
        }

        private void btnExportXML_Click(object sender, EventArgs e)
        {
            NitroOverlay ovl = new NitroOverlay(m_ROM, (uint)m_LevelID);
            m_SaveFileDialogue.FileName = "Level_" + m_LevelID;
            m_SaveFileDialogue.DefaultExt = ".xml";
            m_SaveFileDialogue.Filter = Strings.FILTER_XML;

            if (m_SaveFileDialogue.ShowDialog() == DialogResult.OK)
            {
                LevelDataXML_Exporter.ExportLevelDataToXML(m_Level, m_SaveFileDialogue.FileName);

                slStatusLabel.Text = "Level successfully exported.";
            }
        }

        private void btnImportXML_Click(object sender, EventArgs e)
        {
            m_OpenFileDialogue.FileName = "Level_" + m_LevelID;
            m_OpenFileDialogue.DefaultExt = ".xml";
            m_OpenFileDialogue.Filter = Strings.FILTER_XML;

            if (m_OpenFileDialogue.ShowDialog() == DialogResult.OK)
            {
                NitroOverlay ovl = new NitroOverlay(m_ROM, (uint)m_LevelID);

                try { LevelDataXML_Importer.ImportLevel(m_Level, m_OpenFileDialogue.FileName, true); }
                catch (InvalidDataException ex) { MessageBox.Show(ex.Message); return; }
                catch (Exception ex) { new ExceptionMessageBox("Error parsing level, changes have not been saved", ex).ShowDialog(); return; }

                ReleaseModels();

                InitialiseLevel();

                slStatusLabel.Text = "Level imported successfully.";
            }
        }
        public void ValueChanged(object sender, EventArgs e)
        {
            if (settingValues)
                return;

            String propertyName = "";

            object newValue = 0;
            
            if (sender == val_posX)
            {
                propertyName = "X position";
                newValue = (float)val_posX.Value;

            } else if (sender == val_posY)
            {
                propertyName = "Y position";
                newValue = (float)val_posY.Value;
            }
            else if (sender == val_posZ)
            {
                propertyName = "Z position";
                newValue = (float)val_posZ.Value;
            }
            else if(sender == val_rotY)
            {
                val_rotY.Value = Wrap((float)(val_rotY.Value + 180), 360) - 180;
                propertyName = "Y rotation";
                newValue = (float)val_rotY.Value;

            }
            else if(sender == val_objectId)
            {
                newValue = (ushort)val_objectId.Value;
                if (IsSimpleObject((ushort)newValue) ^ IsSimpleObject(m_SelectedObject.ID))
                {
                    ConvertLevelObject((ushort)newValue);
                    return;
                }
                propertyName = "Object ID";
            }
            else if (sender == val_act)
            {
                int newstar = val_act.SelectedIndex;
                int lastLayer = m_SelectedObject.m_Layer;
                m_SelectedObject.m_Layer = newstar;
                m_SelectedObject.m_Properties["Star"] = newstar;

                //refresh the involved layers
                RefreshObjects(lastLayer);
                RefreshObjects(newstar);
                
                if (m_AuxLayerNum!= newstar)
                {
                    switch (newstar)
                    {
                        case 0:
                            if (!btnStarAll.Checked)
                                btnStarAll.PerformClick();
                            break;
                        case 1: btnStar1.PerformClick(); break;
                        case 2: btnStar2.PerformClick(); break;
                        case 3: btnStar3.PerformClick(); break;
                        case 4: btnStar4.PerformClick(); break;
                        case 5: btnStar5.PerformClick(); break;
                        case 6: btnStar6.PerformClick(); break;
                        case 7: btnStar7.PerformClick(); break;
                    }
                }
                return;
            }
            else if (sender == val_area)
            {
                m_SelectedObject.m_Area = (int)val_area.Value;
                m_SelectedObject.m_Properties["Area"] = (int)val_area.Value;
                return;
            }
            else if (sender == check_displayFog)
            {
                newValue = (check_displayFog.Checked?1f:0f);
                propertyName = "Density";
            }
            else if (sender ==  val_r)
            {
                newValue = (float)val_r.Value;
                propertyName = "RGB R Value";
                box_color.BackColor = Color.FromArgb(
                    (int)val_r.Value,
                    (int)val_g.Value,
                    (int)val_b.Value
                );
            }
            else if (sender == val_g)
            {
                newValue = (float)val_g.Value;
                propertyName = "RGB G Value";
                box_color.BackColor = Color.FromArgb(
                    (int)val_r.Value,
                    (int)val_g.Value,
                    (int)val_b.Value
                );
            }
            else if (sender == val_b)
            {
                newValue = (float)val_b.Value;
                propertyName = "RGB B Value";
                box_color.BackColor = Color.FromArgb(
                    (int)val_r.Value,
                    (int)val_g.Value,
                    (int)val_b.Value
                );
            }
            else if (sender == val_startDistance)
            {
                newValue = (float)val_startDistance.Value;
                propertyName = "Start Distance";
            }
            else if (sender == val_endDistance)
            {
                newValue = (float)val_endDistance.Value;
                propertyName = "End Distance";
            }
            else if (m_SelectedObject.m_ParameterFields!=null)
            {
                foreach (ParameterField field in m_SelectedObject.m_ParameterFields)
                {
                    if (field.GetControl(this)==sender)
                    {
                        //access Properties
                        PropertyTable ptable = m_SelectedObject.m_Properties;

                        bool floatToFloat = ((ptable[field.m_pgFieldName] is float) && (field is FloatField));
                        float newFloat = 0f;
                        ushort newUshort = 0;
                        if (floatToFloat)
                            newFloat = ((FloatField)field).getFloatValue();
                        else
                            newUshort = InsertBits(ptable[field.m_pgFieldName], field.getValue(), field.m_offset, field.m_length);
                        
                        propertyName = field.m_pgFieldName;
                        if (ptable[field.m_pgFieldName] is float)
                        {
                            if (floatToFloat)
                                newValue = newFloat;
                            else
                                newValue = (float)newUshort;
                        }
                        else if (ptable[field.m_pgFieldName] is int)
                        {
                            newValue = (int)newUshort;
                        }
                        else if (ptable[field.m_pgFieldName] is ushort)
                        {
                            newValue = newUshort;
                        }
                        else
                        {
                            return;
                        }
                        UpdateParameterForRawEditor(propertyName,Convert.ToUInt16(newValue));
                    }
                }
            } else
            {
                return;
            }

            SetProperty(propertyName, newValue);
        }
        
        public void SetProperty(string propertyName, object newValue)
        {
            m_SelectedObject.m_Properties[propertyName] = newValue;

            int actmask = m_SelectedObject.SetProperty(propertyName, newValue);
            if ((actmask & 4) != 0)
                tvObjectList.Nodes.Find(m_SelectedObject.m_UniqueID.ToString("X8"), true)[0].Text = m_SelectedObject.GetDescription();
            if ((actmask & 1) != 0)
                RefreshObjects(m_SelectedObject.m_Layer);

            UpdateTransformProperties();
        }

        private void btnOrthView_Click(object sender, EventArgs e)
        {
            if (!btnOrthView.Checked)
            {
                btnOrthView.Checked = true;
                m_OrthView = true;
            }
            else
            {
                btnOrthView.Checked = false;
                m_OrthView = false;
            }

            glLevelView.Refresh();
        }

        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            Bitmap dump = DumpOpenGLRenderingToBMP();

            m_SaveFileDialogue.FileName = "Screenshot_Level_" + m_LevelID + '_' + Helper.CurrentTimeMillis();
            m_SaveFileDialogue.DefaultExt = ".png";
            m_SaveFileDialogue.Filter = Strings.IMAGE_EXPORT_PNG_FILTER;

            if (m_SaveFileDialogue.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dump.Save(m_SaveFileDialogue.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    slStatusLabel.Text = "Screenshot saved.";
                }
                catch (Exception ex)
                {
                    new ExceptionMessageBox("An error occurred whilst saving the screenshot", ex);
                    return;
                }
            }
        }

        private void tc_switchPropertyInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateObjectForRawEditor();
            initializePropertyInterface();
        }
        
        private void UpdateObjectForRawEditor()
        {
            if (m_rawEditor!=null)
                m_rawEditor.UpdateForObject(m_SelectedObject);
        }

        private void UpdateParameterForRawEditor(string name, ushort value)
        {
            if (m_rawEditor != null)
                m_rawEditor.UpdateParameter(name, value);
        }

        public void initializePropertyInterface()
        {
            if (m_SelectedObject == null)
            {
                clearPropertyInterface();
                return;
            }
            
            System.Boolean displayGeneral = m_SelectedObject.SupportsActs();
            System.Boolean displayPos =     m_SelectedObject.HasPosition();
            System.Boolean displayRot =     m_SelectedObject.SupportsRotation();
            System.Boolean displayFog =     (m_SelectedObject is FogObject);
            System.Boolean displayParams =  (m_SelectedObject.m_ParameterFields!=null);
            
            settingValues = true;
            
            int nextBoxSnapY = 0;

            box_general.Visible = displayGeneral;
            if (displayGeneral)
            {
                nextBoxSnapY = Helper.snapControlVertically(box_general, nextBoxSnapY);
                val_act.SelectedIndex = m_SelectedObject.m_Layer;

                val_area.Enabled = !(m_SelectedObject is ExitObject);
                val_objectId.Enabled = !(m_SelectedObject is ExitObject);
                btnOpenObjectList.Enabled = !(m_SelectedObject is ExitObject);
                if (!(m_SelectedObject is ExitObject))
                {
                    val_area.Value = m_SelectedObject.m_Area;
                    val_objectId.Value = m_SelectedObject.ID;
                }
            }
            box_position.Visible = displayPos;
            if (displayPos)
            {
                nextBoxSnapY = Helper.snapControlVertically(box_position, nextBoxSnapY);

                val_posX.Value = (Decimal)m_SelectedObject.Position.X;
                val_posY.Value = (Decimal)m_SelectedObject.Position.Y;
                val_posZ.Value = (Decimal)m_SelectedObject.Position.Z;
            }
            box_rotation.Visible = displayRot;
            if (displayRot)
            {
                nextBoxSnapY = Helper.snapControlVertically(box_rotation, nextBoxSnapY);

                val_rotY.Value = (Decimal)m_SelectedObject.YRotation;
            }
            box_fogSettings.Visible = displayFog;
            if (displayFog)
            {
                nextBoxSnapY = Helper.snapControlVertically(box_fogSettings, nextBoxSnapY);

                check_displayFog.Checked = (m_SelectedObject.Parameters[0] != 0);
                val_r.Value =              m_SelectedObject.Parameters[1];
                val_g.Value =              m_SelectedObject.Parameters[2];
                val_b.Value =              m_SelectedObject.Parameters[3];
                val_startDistance.Value =  (Decimal)(m_SelectedObject.Parameters[4]/1000f);
                val_endDistance.Value =    (Decimal)(m_SelectedObject.Parameters[5] / 1000f);

                box_color.BackColor = Color.FromArgb(
                    (int)val_r.Value,
                    (int)val_g.Value,
                    (int)val_b.Value
                );

            }
            box_parameters.Visible = displayParams;
            if (displayParams)
            {
                int nextFieldSnapY = 18;
                if (box_parameters.Height > 18)
                {
                    Control keep = box_parameters.Controls[0];
                    box_parameters.Controls.Clear();
                    box_parameters.Controls.Add(keep);
                    
                    foreach (ParameterField field in m_SelectedObject.m_ParameterFields)
                    {
                        PropertyTable ptable = m_SelectedObject.m_Properties;
                        object value = ptable[field.m_pgFieldName];
                        
                        Label label = field.GetLabel();
                        Control control = field.GetControl(this);

                        bool floatToFloat = ((ptable[field.m_pgFieldName] is float) && (field is FloatField));
                        if (floatToFloat)
                        {
                            float newValue = (float)value;
                            FloatField floatField = (FloatField)field;
                            floatField.setFloatValue(newValue);
                        }
                        else
                        {
                            ushort extractedValue = ExtractBits(value, field.m_offset, field.m_length);
                            field.setValue(extractedValue);
                        }

                        box_parameters.Controls.Add(label);
                        box_parameters.Controls.Add(control);

                        //snap Vertically
                        Helper.snapControlVertically(label, nextFieldSnapY);
                        nextFieldSnapY = Helper.snapControlVertically(control, nextFieldSnapY);

                        //snap Horizontally
                        int nextSnapX = 0;
                        nextSnapX = Helper.snapControlHorizontally(label, nextSnapX);
                        Helper.snapControlHorizontally(control, nextSnapX);


                    }

                    if ((m_SelectedObject is SimpleObject) || (m_SelectedObject is StandardObject) || (m_SelectedObject is PathObject))
                    {
                        box_parameters.Controls.Add(btnOpenRawEditor);
                        nextFieldSnapY = Helper.snapControlVertically(btnOpenRawEditor, nextFieldSnapY,2);
                    }
                }
                
                box_parameters.Height = nextFieldSnapY;
                nextBoxSnapY = Helper.snapControlVertically(box_parameters, nextBoxSnapY);
            }
            
            settingValues = false;
        }

        private void updatePropertyInterface()
        {
            settingValues = true;
            val_posX.Value = (Decimal)m_SelectedObject.Position.X;
            val_posY.Value = (Decimal)m_SelectedObject.Position.Y;
            val_posZ.Value = (Decimal)m_SelectedObject.Position.Z;

            val_rotY.Value = (Decimal)m_SelectedObject.YRotation;

            UpdateTransformProperties();
            RefreshObjects(m_SelectedObject.m_Layer);
            settingValues = false;
        }

        private void clearPropertyInterface()
        {
            box_general.Visible = false;
            box_position.Visible = false;
            box_rotation.Visible = false;
            box_fogSettings.Visible = false;
            box_parameters.Visible = false;
        }

        private void tvObjectList_DoubleClick(object sender, EventArgs e)
        {
            if (m_SelectedObject != null)
            {
                if (m_SelectedObject.HasPosition())
                {
                    FocusCamera(m_SelectedObject.Position);
                }
            }
        }

        private void btnToogleCollapsePosition_Click(object sender, EventArgs e)
        {
            int storedValue = box_position.Height;
            box_position.Height = box_position_NextHeight;
            box_position_NextHeight = storedValue;
            initializePropertyInterface();
        }

        private void btnToogleCollapseRotation_Click(object sender, EventArgs e)
        {
            int storedValue = box_rotation.Height;
            box_rotation.Height = box_rotation_NextHeight;
            box_rotation_NextHeight = storedValue;
            initializePropertyInterface();
        }

        private void btnToogleCollapseColor_Click(object sender, EventArgs e)
        {
            int storedValue = box_fogSettings.Height;
            box_fogSettings.Height = box_fogSettings_NextHeight;
            box_fogSettings_NextHeight = storedValue;
            initializePropertyInterface();
        }

        private void btnToogleCollapseParameters_Click(object sender, EventArgs e)
        {
            int storedValue = box_parameters.Height;
            box_parameters.Height = box_parameters_NextHeight;
            box_parameters_NextHeight = storedValue;
            initializePropertyInterface();
        }

        private void btnToogleCollapseGeneral_Click(object sender, EventArgs e)
        {
            int storedValue = box_general.Height;
            box_general.Height = box_general_NextHeight;
            box_general_NextHeight = storedValue;
            initializePropertyInterface();
        }

        private void btnOpenObjectList_Click(object sender, EventArgs e)
        {
            ObjectListForm dlg = new ObjectListForm((ushort)val_objectId.Value);
            DialogResult result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                val_objectId.Value = dlg.ObjectID;
            }
        }

        private void btnOpenRawEditor_Click(object sender, EventArgs e)
        {
            if (m_rawEditor==null)
                m_rawEditor = new RawEditorForm(this);
            m_rawEditor.ShowForObject(m_SelectedObject);
        }

        private void ConvertLevelObject(ushort newid)
        {
            LevelObject oldobj = m_SelectedObject;
            RemoveObject(oldobj);

            LevelObject.Type type = IsSimpleObject(newid) ? LevelObject.Type.SIMPLE : LevelObject.Type.STANDARD;
            LevelObject obj = AddObject(type, newid, oldobj.m_Layer, oldobj.m_Area);
            obj.Position = oldobj.Position;
            obj.Parameters[0] = (ushort)(oldobj.Parameters[0] & ((type == LevelObject.Type.SIMPLE) ? 0x007F : 0xFFFF));
            obj.GenerateProperties();

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            UpdateObjectForRawEditor();
            initializePropertyInterface();

            if (obj.m_Properties.Properties.IndexOf("X position") != -1)
            {
                btnCopyCoordinates.Visible = true;
                btnPasteCoordinates.Visible = true;
            }

            RefreshObjects(obj.m_Layer);
        }

        public Decimal Wrap(float a, float b)
        {
            return (Decimal)(a - b * Math.Floor(a / b));

        }

        public ushort ExtractBits(object value, int offset, int size)
        {
            String bitString = Convert.ToString(Convert.ToUInt16(value), 2).PadLeft(16,'0');
            String extractedBits = bitString.Substring(offset, size);
            return Convert.ToUInt16(extractedBits, 2);
        }

        public ushort InsertBits(object value, object insertValue, int offset, int size)
        {
            String bitString = Convert.ToString(Convert.ToUInt16(value), 2).PadLeft(16, '0');
            String newBitString = bitString.Remove(offset, size);
            String insertString = Convert.ToString(Convert.ToUInt16(insertValue) & (ushort)Math.Pow(2, size) - 1, 2).PadLeft(size, '0');
            newBitString = newBitString.Insert(offset, insertString);
            return Convert.ToUInt16(newBitString, 2);
        }
    }
}
