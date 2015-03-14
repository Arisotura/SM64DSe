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


namespace SM64DSe
{
    public partial class LevelEditorForm : Form
    {
        private static Color k_SelectionColor = Color.FromArgb(255, 255, 128);
        private static Color k_HoverColor = Color.FromArgb(255, 255, 192);

        public Dictionary<ushort, bool> m_ObjAvailable;
        private void GetObjectsAvailable()
        {
            m_ObjAvailable.Clear();

            for (int i = 0; i < 326; i++)
            {
                ObjectDatabase.ObjectInfo objinfo = ObjectDatabase.m_ObjectInfo[i];

                bool available = true;
                if (objinfo.m_BankRequirement == 1)
                {
                    if (m_LevelSettings.ObjectBanks[objinfo.m_NumBank] != objinfo.m_BankSetting)
                        available = false;
                }
                else if (objinfo.m_BankRequirement == 2)
                    available = false;

                m_ObjAvailable.Add((ushort)i, available);
            }

            m_ObjAvailable.Add(511, true);
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

        private int m_EntranceID = 0;
        private int m_PathNodeID = 0;
        private int m_MinimapTileIDNum = 0;
        private void ReadObjectTable(uint offset, int area)
        {
            AddPointer(offset + 0x4);
            uint subtbl_num = m_Overlay.Read32(offset);
            uint subtbl_offset = m_Overlay.ReadPointer(offset + 0x4);
            for (uint st = 0; st < subtbl_num; st++)
            {
                uint curoffset = subtbl_offset + (st * 8);
                AddPointer(curoffset + 0x4);

                byte flags = m_Overlay.Read8(curoffset);
                byte entries_num = m_Overlay.Read8(curoffset + 0x1);
                uint entries_offset = m_Overlay.ReadPointer(curoffset + 0x4);

                byte type = (byte)(flags & 0x1F);
                byte layer = (byte)(flags >> 5);

                switch (type)
                {
                    case 0:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new StandardObject(m_Overlay, (uint)(entries_offset + (e * 16)), m_LevelObjects.Count, layer, area);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 1:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new EntranceObject(m_Overlay, (uint)(entries_offset + (e * 16)), m_LevelObjects.Count, layer, m_EntranceID++);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 2: // Path Node
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new PathPointObject(m_Overlay, (uint)(entries_offset + (e * 6)), m_LevelObjects.Count, m_PathNodeID++);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 3: // Path
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new PathObject(m_Overlay, (uint)(entries_offset + (e * 6)), m_LevelObjects.Count);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 4:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new ViewObject(m_Overlay, (uint)(entries_offset + (e * 14)), m_LevelObjects.Count);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 5:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new SimpleObject(m_Overlay, (uint)(entries_offset + (e * 8)), m_LevelObjects.Count, layer, area);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 6:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new TpSrcObject(m_Overlay, (uint)(entries_offset + (e * 8)), m_LevelObjects.Count, layer);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 7:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new TpDstObject(m_Overlay, (uint)(entries_offset + (e * 8)), m_LevelObjects.Count, layer);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 8:
                        // Fog
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new FogObject(m_Overlay, (uint)(entries_offset + (e * 8)), m_LevelObjects.Count, layer, area);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 9:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new DoorObject(m_Overlay, (uint)(entries_offset + (e * 12)), m_LevelObjects.Count, layer);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 10:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new ExitObject(m_Overlay, (uint)(entries_offset + (e * 14)), m_LevelObjects.Count, layer);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 11:
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new MinimapTileIDObject(m_Overlay, (uint)(entries_offset + (e * 2)), m_LevelObjects.Count, layer, m_MinimapTileIDNum++);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        // This is still used by Minimap Editor
                        m_MinimapFileIDs = new ushort[entries_num];
                        for (byte e = 0; e < entries_num; e++)
                            m_MinimapFileIDs[e] = m_Overlay.Read16((uint)(entries_offset + (e * 2)));

                        break;

                    case 12:
                        // per-area minimap scale factors
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new MinimapScaleObject(m_Overlay, (uint)(entries_offset + (e * 2)), m_LevelObjects.Count, layer, area);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;

                    case 14:
                        // ??? Unknown
                        for (byte e = 0; e < entries_num; e++)
                        {
                            LevelObject obj = new Type14Object(m_Overlay, (uint)(entries_offset + (e * 4)), m_LevelObjects.Count, layer, area);
                            m_LevelObjects.Add(obj.m_UniqueID, obj);
                        }
                        break;
                }
            }

            IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 2);
            IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 3);
            AlignPathNodes(paths.ToList<LevelObject>(), pathNodes.ToList<LevelObject>());
        }

        public void ReadTextureAnimations(uint offset, int area)
        {
            AddPointer(offset + 0x4);
            AddPointer(offset + 0x8);
            AddPointer(offset + 0xC);
            AddPointer(offset + 0x14);

            uint numanim = m_Overlay.Read32(offset + 0x10);
            uint animaddr = m_Overlay.ReadPointer(offset + 0x14);
            for (uint i = 0; i < numanim; i++)
            {
                AddPointer(animaddr + 0x4);

                m_TexAnims[area].Add(new LevelTexAnim(m_Overlay, offset, animaddr, m_TexAnims[area].Count, area));

                animaddr += 0x1C;
            }
        }

        private void LoadLevelData()
        {
            m_PointerList = new List<PointerReference>();
            AddPointer(0x60);
            AddPointer(0x64);
            AddPointer(0x70);

            m_LevelSettings = new LevelSettings(m_Overlay);

            // read object lists

            m_NumAreas = m_Overlay.Read8(0x74);
            uint objlistptr = m_Overlay.ReadPointer(0x70);

            m_LevelObjects = new Dictionary<uint, LevelObject>();
            m_TexAnims = new List<LevelTexAnim>[m_NumAreas];
            for (int a = 0; a < m_NumAreas; a++)
            {
                m_TexAnims[a] = new List<LevelTexAnim>();
            }

            ReadObjectTable(m_Overlay.ReadPointer(0x64), 0);
            for (byte a = 0; a < m_NumAreas; a++)
            {
                // read object tables
                uint addr = (uint)(objlistptr + (a * 12));
                if (m_Overlay.Read32(addr) != 0)
                {
                    AddPointer(addr);
                    ReadObjectTable(m_Overlay.ReadPointer(addr), a);
                }

                // read texture animation
                addr += 4;
                if (m_Overlay.Read32(addr) != 0)
                {
                    AddPointer(addr);
                    ReadTextureAnimations(m_Overlay.ReadPointer(addr), a);
                }

            }

            m_LevelModel = null;
            m_LevelCollMap = new KCL(m_ROM.GetFileFromInternalID(m_LevelSettings.KCLFileID));
            //MessageBox.Show(KCL.OctreeNode.maxkids.ToString());

            m_SkyboxModel = null;

            //m_LevelModified = false;
            m_ObjAvailable = new Dictionary<ushort, bool>();
            GetObjectsAvailable();

            // Check if the level contains addresses not aligned to 4 byte boundaries but don't attempt fix
            m_PointerList = m_PointerList.OrderBy(o => o.m_PointerAddr).ToList();
            bool checkCorrupt = CheckCorrupt(false);

            if (checkCorrupt)
            {
                DialogResult result = MessageBox.Show("This level contains addresses that are not aligned to 4 byte boundaries and therefore may not work in-game." +
                    "\n\nDo you want to attempt to fix these issues?\n\nChanges will not be saved.", "Warning", MessageBoxButtons.YesNo);
                // Check if the level contains addresses not aligned to 4 byte boundaries and do attempt fix
                if (result == DialogResult.Yes)
                    CheckCorrupt(true);
            }
        }

        public bool CheckCorrupt(bool attemptFix, int startIndex = 0)
        {
            for (int i = startIndex; i < m_PointerList.Count; i++)
            {
                int pointerModFour = (int)(m_PointerList[i].m_PointerAddr % 4);
                if (pointerModFour != 0)
                {
                    if (attemptFix)
                    {
                        AddSpace(m_PointerList[i].m_PointerAddr, (uint)(4 - pointerModFour));
                        CheckCorrupt(attemptFix, i + 1);
                    }
                    return true;
                }
            }
            return false;
        }

        public LevelEditorForm(NitroROM rom, int levelid)
        {
            InitializeComponent();

            // remove debug controls if needed
            if (!Program.AppVersion.ToLowerInvariant().Contains("private beta"))
            {
                btnDumpOverlay.Visible = false;
                btnEditPaths.Visible = false;
            }

            this.Text = string.Format("[{0}] {1} - {2} {3}", levelid, Strings.LevelNames[levelid], Program.AppTitle, Program.AppVersion);

            m_MouseDown = MouseButtons.None;

            m_ROM = rom;
            m_LevelID = levelid;

            m_Overlay = new NitroOverlay(m_ROM, m_ROM.GetLevelOverlayID(m_LevelID));

            // dump overlay
            //System.IO.File.WriteAllBytes(string.Format("level{0}_overlay.bin", m_LevelID), m_Overlay.m_Data);

            m_GLLoaded = false;

            btnStar1.Checked = true;
            btnStarAll.Checked = true;
            m_ShowCommonLayer = true;
            m_AuxLayerNum = 1;
            btnEditObjects.Checked = true;
            m_EditMode = 1;

            m_Hovered = 0xFFFFFFFF;
            m_LastHovered = 0xFFFFFFFF;
            m_HoveredObject = null;
            m_Selected = 0xFFFFFFFF;
            m_LastSelected = 0xFFFFFFFF;
            m_SelectedObject = null;
            m_LastClicked = 0xFFFFFFFF;
            m_ObjectBeingPlaced = 0xFFFF;
            m_ShiftPressed = false;

            slStatusLabel.Text = "Ready";
        }


        public void UpdateSkybox(int id)
        {
            if (id == -1)
                id = m_LevelSettings.Background;

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

            m_LevelModel = new BMD(m_ROM.GetFileFromInternalID(m_LevelSettings.BMDFileID));
            m_LevelModel.PrepareToRender();

            m_LevelModelDLs = new int[m_LevelModel.m_ModelChunks.Length, 3];

            for (int c = 0; c < m_LevelModel.m_ModelChunks.Length; c++)
            {
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
                IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == m_EditMode && obj.m_Layer == layer);
                foreach (LevelObject obj in objects)
                {
                    GL.Color4(Color.FromArgb((int)obj.m_UniqueID));
                    obj.Render(mode);
                }
            }
            else
            {
                IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj => obj.m_Layer == layer);
                foreach (LevelObject obj in objects)
                    obj.Render(mode);
            }

            GL.EndList();
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

            // would be faster, but doesn't quite work right
            // (highlights overlapping glitch)
            /*GL.Enable(EnableCap.StencilTest);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(-1f, 1f);
            GL.StencilFunc(StencilFunction.Always, 0x1, 0x1);
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            GL.Color4(Color.FromArgb(100, color));
            obj.Render(RenderMode.Picking);

            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.LineWidth(3.0f);
            GL.StencilFunc(StencilFunction.Equal, 0x0, 0x1);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            GL.DepthFunc(DepthFunction.Always);
            GL.Color4(color);
            obj.Render(RenderMode.Picking);*/

            GL.PopAttrib();
            GL.EndList();
        }

        private void RenderPathHilite(List<LevelObject> objs, Color color, int dlist)
        {
            GL.NewList(dlist, ListMode.Compile);
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.Lighting);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            for (int i = 0; i < objs.Count(); i++)
            {
                LevelObject obj = objs.ElementAt(i);

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
                        int next = 0;
                        if (i != endNode)
                            next = i + 1;
                        else
                            next = startNode;
                        try
                        {
                            float opposite = nodes[next].Position.X - nodes[i].Position.X;
                            float adjacent = nodes[next].Position.Z - nodes[i].Position.Z;
                            float rotY = MathHelper.RadiansToDegrees((float)Math.Atan(opposite / adjacent));
                            if (adjacent >= 0)
                            {
                                rotY += 180;
                            }
                            ((PathPointObject)nodes[i]).m_Renderer = new ColourArrowRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false, 0f, rotY, 0f);
                        }
                        catch
                        {
                            // Object has been deleted
                        }
                    }
                }
            }
        }

        public void RefreshObjects(int layer)
        {
            IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 2 && (obj.m_Layer) == layer);
            IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 3 && (obj.m_Layer) == layer);
            AlignPathNodes(paths.ToList<LevelObject>(), pathNodes.ToList<LevelObject>());

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
                case 0:
                    {
                        btnImportModel.Visible = true;// Properties.Settings.Default.lolhax;
                        btnExportLevelModel.Visible = true;
                        //btnAddTexAnim.Visible = true;
                        //btnRemoveSel.Visible = true;
                        btnImportOtherModel.Visible = true;
                        btnExportOtherModel.Visible = true;

                        /*TreeNode node0 = tvObjectList.Nodes.Add("Texture animations");
                        for (int a = 0; a < m_TexAnims.Length; a++)
                            foreach (LevelTexAnim anim in m_TexAnims[a])
                                node0.Nodes.Add(anim.m_UniqueID.ToString("X8"), anim.GetDescription()).Tag = anim.m_UniqueID;*/
                        tvObjectList.Nodes.Add("lol", "(nothing available for now)");
                    }
                    break;

                case 1:
                    {
                        btnAddObject.Visible = true;
                        btnRemoveSel.Visible = true;
                        btnReplaceObjModel.Visible = true;
                        btnExportObjectModel.Visible = true;
                        btnOffsetAllCoords.Visible = true;

                        TreeNode node0 = tvObjectList.Nodes.Add("parent0", "Objects");

                        IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj =>
                            ((m_ShowCommonLayer && obj.m_Layer == 0) || (m_AuxLayerNum != 0 && obj.m_Layer == m_AuxLayerNum)) &&
                            (obj.m_UniqueID >> 28) == 1);
                        foreach (LevelObject obj in objects)
                            node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;
                    }
                    break;

                case 2:
                    {
                        btnAddWarp.Visible = true;
                        btnRemoveSel.Visible = true;

                        TreeNode node0 = tvObjectList.Nodes.Add("parent0", "Entrances");
                        TreeNode node1 = tvObjectList.Nodes.Add("parent1", "Exits");
                        TreeNode node2 = tvObjectList.Nodes.Add("parent2", "Doors");
                        TreeNode node3 = tvObjectList.Nodes.Add("parent3", "Teleport sources");
                        TreeNode node4 = tvObjectList.Nodes.Add("parent4", "Teleport destinations");

                        IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj =>
                            ((m_ShowCommonLayer && obj.m_Layer == 0) || (m_AuxLayerNum != 0 && obj.m_Layer == m_AuxLayerNum)) &&
                            (obj.m_UniqueID >> 28) == 2);
                        foreach (LevelObject obj in objects)
                        {
                            switch (obj.m_Type)
                            {
                                case 1: node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 10: node1.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 9: node2.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 6: node3.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 7: node4.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        btnAddPathNodes.Visible = true;
                        btnAddPath.Visible = true;
                        btnRemoveSel.Visible = true;

                        TreeNode node0 = tvObjectList.Nodes.Add("parent0", "Paths");

                        IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 3);
                        for (int i = 0; i < paths.Count(); i++)
                        {
                            node0.Nodes.Add(paths.ElementAt(i).m_UniqueID.ToString("X8"), paths.ElementAt(i).GetDescription() + " " + i).Tag = paths.ElementAt(i).m_UniqueID;
                            tvObjectList.Nodes.Add("parent" + (i + 1), "Path " + i + " Nodes");
                        }

                        IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 2);
                        //TreeNode node1 = tvObjectList.Nodes.Add("parent1", "Path Nodes");
                        for (int i = 0; i < paths.Count(); i++)
                        {
                            int start = paths.ElementAt(i).Parameters[0];
                            int end = start + paths.ElementAt(i).Parameters[1];
                            // Need to add by Node ID
                            for (int j = start; j < end; j++)
                            {
                                PathPointObject node = (PathPointObject)pathNodes.Where(obj => ((PathPointObject)obj).m_NodeID == j).ElementAt(0);
                                tvObjectList.Nodes[i + 1].Nodes.
                                    Add(node.m_UniqueID.ToString("X8"), node.GetDescription() + " " + j).Tag = node.m_UniqueID;
                            }
                        }

                        btnAddPathNodes.DropDownItems.Clear();
                        for (int i = 0; i < paths.Count(); i++)
                            btnAddPathNodes.DropDownItems.Add("Add Node to Path " + i);
                    }
                    break;

                case 4:
                    {
                        btnAddView.Visible = true;
                        btnRemoveSel.Visible = true;

                        if (!m_ShowCommonLayer) break;
                        TreeNode node0 = tvObjectList.Nodes.Add("parent0", "Views");

                        IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 4);
                        foreach (LevelObject obj in objects)
                        {
                            node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;
                        }
                    }
                    break;

                case 5:
                    {
                        btnAddMisc.Visible = true;
                        btnRemoveSel.Visible = true;

                        if (!m_ShowCommonLayer) break;
                        TreeNode node0 = tvObjectList.Nodes.Add("parent0", "Minimap Scales");
                        TreeNode node1 = tvObjectList.Nodes.Add("parent1", "Fog");
                        TreeNode node2 = tvObjectList.Nodes.Add("parent2", "Type 14 Object");
                        TreeNode node3 = tvObjectList.Nodes.Add("parent3", "Minimap Tile ID");

                        IEnumerable<LevelObject> objects = m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 5);
                        foreach (LevelObject obj in objects)
                        {
                            switch (obj.m_Type)
                            {
                                case 8: node1.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 11: node3.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 12: node0.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                                case 14: node2.Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID; break;
                            }
                        }
                    }
                    break;
            }

            tvObjectList.ExpandAll();
        }

        private void UpdateSelection()
        {
            PropertyTable ptable = (PropertyTable)pgObjectProperties.SelectedObject;
            ptable["X position"] = m_SelectedObject.Position.X;
            ptable["Y position"] = m_SelectedObject.Position.Y;
            ptable["Z position"] = m_SelectedObject.Position.Z;
            if (m_SelectedObject.SupportsRotation())
                ptable["Y rotation"] = m_SelectedObject.YRotation;
            pgObjectProperties.Refresh();

            RefreshObjects(m_SelectedObject.m_Layer);
        }


        private NitroROM m_ROM;
        public int m_LevelID;
        public NitroOverlay m_Overlay;


        public struct PointerReference
        {
            public PointerReference(uint _ref, uint _ptr) { m_ReferenceAddr = _ref; m_PointerAddr = _ptr; }
            public uint m_ReferenceAddr; // where the pointer is stored
            public uint m_PointerAddr; // where the pointer points
        }
        public List<PointerReference> m_PointerList;

        public void AddPointer(uint _ref)
        {
            uint _ptr = m_Overlay.ReadPointer(_ref);
            m_PointerList.Add(new PointerReference(_ref, _ptr));
        }

        public void RemovePointer(uint _ref)
        {
            for (int i = 0; i < m_PointerList.Count; )
            {
                if (m_PointerList[i].m_ReferenceAddr == _ref)
                    m_PointerList.RemoveAt(i);
                else
                    i++;
            }
        }

        public void RemovePointerByPointerAddress(uint _ptr)
        {
            for (int i = 0; i < m_PointerList.Count; )
            {
                if (m_PointerList[i].m_PointerAddr == _ptr)
                    m_PointerList.RemoveAt(i);
                else
                    i++;
            }
        }

        public void UpdateObjectOffsets(uint start, uint delta)
        {
            foreach (LevelObject obj in m_LevelObjects.Values)
                if (obj.m_Offset >= start) obj.m_Offset += delta;

            for (int a = 0; a < m_TexAnims.Length; a++)
            {
                foreach (LevelTexAnim anim in m_TexAnims[a])
                {
                    if (anim.m_TexAnimHeaderOffset >= start) anim.m_TexAnimHeaderOffset = (uint)(anim.m_TexAnimHeaderOffset + delta);
                    if (anim.m_Offset >= start) anim.m_Offset = (uint)(anim.m_Offset + delta);
                    if (anim.m_ScaleTblOffset >= start) anim.m_ScaleTblOffset = (uint)(anim.m_ScaleTblOffset + delta);
                    if (anim.m_RotTblOffset >= start) anim.m_RotTblOffset = (uint)(anim.m_RotTblOffset + delta);
                    if (anim.m_TransTblOffset >= start) anim.m_TransTblOffset = (uint)(anim.m_TransTblOffset + delta);
                    if (anim.m_MatNameOffset >= start) anim.m_MatNameOffset = (uint)(anim.m_MatNameOffset + delta);
                    if (anim.m_BaseScaleTblAddr >= start) anim.m_BaseScaleTblAddr = (uint)(anim.m_BaseScaleTblAddr + delta);
                    if (anim.m_BaseRotTblAddr >= start) anim.m_BaseRotTblAddr = (uint)(anim.m_BaseRotTblAddr + delta);
                    if (anim.m_BaseTransTblAddr >= start) anim.m_BaseTransTblAddr = (uint)(anim.m_BaseTransTblAddr + delta);
                }
            }
        }

        public void AddSpace(uint offset, uint amount)
        {
            if ((m_Overlay.GetSize() + amount) > NitroROM.LEVEL_OVERLAY_SIZE)
                throw new Exception("This level has reached the level size limit. Cannot add more data.");

            // move the data
            byte[] block = m_Overlay.ReadBlock(offset, (uint)(m_Overlay.GetSize() - offset));
            m_Overlay.WriteBlock(offset + amount, block);

            // write zeroes in the newly created space
            for (int i = 0; i < amount; i++)
                m_Overlay.Write8((uint)(offset + i), 0);

            // update the pointers
            for (int i = 0; i < m_PointerList.Count; i++)
            {
                PointerReference ptrref = m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= offset)
                    ptrref.m_ReferenceAddr += amount;
                if (ptrref.m_PointerAddr >= offset)
                {
                    ptrref.m_PointerAddr += amount;
                    m_Overlay.WritePointer(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                m_PointerList[i] = ptrref;
            }

            // update the objects 'n' all
            UpdateObjectOffsets(offset, amount);
        }

        public void RemoveSpace(uint offset, uint amount)
        {
            // move the data
            byte[] block = m_Overlay.ReadBlock(offset + amount, (uint)(m_Overlay.GetSize() - offset - amount));
            m_Overlay.WriteBlock(offset, block);
            m_Overlay.SetSize(m_Overlay.GetSize() - amount);

            // update the pointers
            for (int i = 0; i < m_PointerList.Count; i++)
            {
                PointerReference ptrref = m_PointerList[i];
                if (ptrref.m_ReferenceAddr >= (offset + amount))
                    ptrref.m_ReferenceAddr -= amount;
                if (ptrref.m_PointerAddr >= (offset + amount))
                {
                    ptrref.m_PointerAddr -= amount;
                    m_Overlay.WritePointer(ptrref.m_ReferenceAddr, ptrref.m_PointerAddr);
                }
                m_PointerList[i] = ptrref;
            }

            // update the objects 'n' all
            UpdateObjectOffsets(offset + amount, (uint)-amount);
        }

        private uint AddObjectSlot(int type, int layer, int area, int off = -1)
        {
            int[] sizes = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
            int size = sizes[type];

            uint tableptr;
            if (type == 0 || type == 5)
            {
                uint areaptr = (uint)(m_Overlay.ReadPointer(0x70) + (area * 12));
                tableptr = m_Overlay.ReadPointer(areaptr);

                if (tableptr == 0xFFFFFFFF)
                {
                    tableptr = (uint)((m_Overlay.GetSize() + 3) & ~3);
                    AddSpace(m_Overlay.GetSize(), tableptr - m_Overlay.GetSize());
                    m_Overlay.WritePointer(areaptr, tableptr);
                    AddPointer(areaptr);
                    m_Overlay.Write32(tableptr, 1);
                    m_Overlay.WritePointer(tableptr + 4, tableptr + 8);
                    AddPointer(tableptr + 4);
                    m_Overlay.Write8(tableptr + 8, (byte)(type | (layer << 5)));
                    m_Overlay.Write8(tableptr + 9, 1);
                    m_Overlay.Write16(tableptr + 10, 0);
                    m_Overlay.WritePointer(tableptr + 12, tableptr + 16);
                    AddPointer(tableptr + 12);

                    for (int i = 0; i < ((size + 3) & ~3); i++)
                        m_Overlay.Write8((uint)((tableptr + 16) + i), 0x00);

                    return tableptr + 16;
                }
            }
            else
                tableptr = m_Overlay.ReadPointer(0x64);

            uint numentries = m_Overlay.Read32(tableptr);// Number of object tables in object table list
            for (uint i = 0; i < numentries; i++)
            {
                uint curptr = (uint)(m_Overlay.ReadPointer(tableptr + 4) + (i * 8));// Start offset of current object table

                byte type_layer = m_Overlay.Read8(curptr);
                if ((type_layer & 0x1F) != type) continue;
                if ((type_layer >> 5) != layer) continue;

                byte numobjs = m_Overlay.Read8(curptr + 1);
                if (numobjs == 255) continue;

                uint endptr = (uint)(m_Overlay.ReadPointer(curptr + 4) + (numobjs * size));
                uint new_obj_addr = (off == -1) ? endptr : (uint)off;

                // Need to make sure that following addresses remain 4 byte aligned
                uint endptr_AlignedNextFour = (uint)((endptr + 3) & ~3);
                uint endptrPlusSize_AlignedNextFour = (uint)(((endptr + (uint)size) + 3) & ~3);
                uint endptr_AlignedNextFour_PlusSize = endptr_AlignedNextFour + (uint)size;
                int padding = (int)endptr_AlignedNextFour_PlusSize - (int)endptrPlusSize_AlignedNextFour;

                AddSpace(new_obj_addr, (uint)size);

                if (((-1) * padding) > 0)
                    AddSpace(endptr + (uint)size, (uint)((-1) * padding));
                else if (((-1) * padding) < 0)
                    RemoveSpace(endptr + (uint)size, (uint)(padding));

                m_Overlay.Write8(curptr + 1, (byte)(numobjs + 1));

                return new_obj_addr;
            }
            // If a new table needs created for this object type
            uint tableendptr = (uint)(m_Overlay.ReadPointer(tableptr + 4) + (numentries * 8));
            AddSpace(tableendptr, 8);
            m_Overlay.Write32(tableptr, numentries + 1);

            uint objaddr = (uint)((m_Overlay.GetSize() + 3) & ~3);
            m_Overlay.Write8(tableendptr, (byte)(type | (layer << 5)));
            m_Overlay.Write8(tableendptr + 1, 1);
            m_Overlay.Write16(tableendptr + 2, 0);
            m_Overlay.WritePointer(tableendptr + 4, objaddr);
            AddPointer(tableendptr + 4);

            for (int i = 0; i < ((size + 3) & ~3); i++)
                m_Overlay.Write8((uint)(objaddr + i), 0x00);

            return objaddr;
        }

        private void RemoveObjectSlot(LevelObject obj)
        {
            int type = obj.m_Type;
            int[] sizes = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
            int size = sizes[type];

            uint tableptr;
            if (type == 0 || type == 5)
            {
                uint areaptr = (uint)(m_Overlay.ReadPointer(0x70) + (obj.m_Area * 12));
                tableptr = m_Overlay.ReadPointer(areaptr);
            }
            else
                tableptr = m_Overlay.ReadPointer(0x64);

            uint numentries = m_Overlay.Read32(tableptr);
            for (uint i = 0; i < numentries; i++)
            {
                uint curptr = (uint)(m_Overlay.ReadPointer(tableptr + 4) + (i * 8));

                int tbltype = m_Overlay.Read8(curptr) & 0x1F;
                if (tbltype != type) continue;

                int numobjs = m_Overlay.Read8(curptr + 1);
                uint tblstart = m_Overlay.ReadPointer(curptr + 4);
                uint tblend = (uint)(tblstart + (numobjs * sizes[tbltype]));

                if (obj.m_Offset < tblstart || obj.m_Offset >= tblend)
                    continue;

                RemoveSpace(obj.m_Offset, (uint)size);

                // If needed, add or remove padding at end of table to ensure following addresses are 4 byte aligned
                uint oldTblEndAlignedFour = (uint)((tblend + 3) & ~3);
                uint currentTblEnd = (uint)(((tblend - (uint)size) + 3) & ~3);
                uint currentTblEndAlignedFour = oldTblEndAlignedFour - (uint)size;
                int padding = (int)(currentTblEndAlignedFour - currentTblEnd);

                if (padding > 0)
                    RemoveSpace(currentTblEnd, (uint)padding);
                else if (padding < 0)
                    AddSpace(currentTblEndAlignedFour, (uint)((-1) * padding));

                if (numobjs > 1)
                {
                    m_Overlay.Write8(curptr + 1, (byte)(numobjs - 1));
                    return;
                }

                RemovePointer(curptr + 4);
                RemoveSpace(curptr, 8);
                if (numentries > 1 || (type != 0 && type != 5))
                {
                    m_Overlay.Write32(tableptr, (uint)(numentries - 1));
                    return;
                }

                RemovePointer(tableptr + 4);
                RemoveSpace(tableptr, 8);
                uint areaptr = (uint)(m_Overlay.ReadPointer(0x70) + (obj.m_Area * 12));
                RemovePointer(areaptr);
                m_Overlay.WritePointer(areaptr, 0xFFFFFFFF);

                return;
            }
        }

        private LevelObject AddObject(int type, ushort id, int layer, int area, int off = -1)
        {
            int[] sizes = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
            int size = sizes[type];

            uint offset = (off == -1) ? AddObjectSlot(type, layer, area) : AddObjectSlot(type, layer, area, off);

            // write the object ID before creating the object so that it is created
            // with the right renderer and settings
            if (type == 0 || type == 5)
                m_Overlay.Write16(offset, id);

            uint uniqueid = m_LevelObjects.Keys.DefaultIfEmpty((uint)m_LevelObjects.Count).First(uid => m_LevelObjects.Keys.Count(uid2 => (uid2 & 0x0FFFFFFF) == ((uid & 0x0FFFFFFF) + 1)) == 0);
            uniqueid = (uniqueid & 0x0FFFFFFF) + 1;

            LevelObject obj = null;
            string parentnode = "parent0";
            switch (type)
            {
                case 0: obj = new StandardObject(m_Overlay, offset, (int)uniqueid, layer, area); break;
                case 1:
                    {
                        int maxid = m_LevelObjects.Values.Where(obj2 => obj2.m_Type == 1).Max(obj2 => ((EntranceObject)obj2).m_EntranceID);
                        obj = new EntranceObject(m_Overlay, offset, (int)uniqueid, layer, maxid + 1);
                    }
                    break;
                case 2:
                    {
                        parentnode = "parent" + (1 + m_CurrentPathID);
                        // Calculate the Node ID using the parent path's start offset and length
                        IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 3);
                        int nodeID = ((PathObject)paths.ElementAt(m_CurrentPathID)).Parameters[0] +
                            ((PathObject)paths.ElementAt(m_CurrentPathID)).Parameters[1];

                        obj = new PathPointObject(m_Overlay, offset, (int)uniqueid, nodeID);
                    }
                    break;
                case 3: parentnode = "parent0"; obj = new PathObject(m_Overlay, offset, (int)uniqueid); break;
                case 4: obj = new ViewObject(m_Overlay, offset, (int)uniqueid); break;
                case 5: obj = new SimpleObject(m_Overlay, offset, (int)uniqueid, layer, area); break;
                case 6: parentnode = "parent3"; obj = new TpSrcObject(m_Overlay, offset, (int)uniqueid, layer); break;
                case 7: parentnode = "parent4"; obj = new TpDstObject(m_Overlay, offset, (int)uniqueid, layer); break;
                case 8: parentnode = "parent1"; obj = new FogObject(m_Overlay, offset, (int)uniqueid, layer, area); break;
                case 9: parentnode = "parent2"; obj = new DoorObject(m_Overlay, offset, (int)uniqueid, layer); break;
                case 10: parentnode = "parent1"; obj = new ExitObject(m_Overlay, offset, (int)uniqueid, layer); break;
                case 11: parentnode = "parent3"; obj = new MinimapTileIDObject(m_Overlay, offset, (int)uniqueid, layer, area); break;
                case 12: parentnode = "parent0"; obj = new MinimapScaleObject(m_Overlay, offset, (int)uniqueid, layer, area); break;
                case 14: parentnode = "parent2"; obj = new Type14Object(m_Overlay, offset, (int)uniqueid, layer, area); break;
            }

            if (obj != null)
            {
                m_LevelObjects.Add(obj.m_UniqueID, obj);
                tvObjectList.Nodes[parentnode].Nodes.Add(obj.m_UniqueID.ToString("X8"), obj.GetDescription()).Tag = obj.m_UniqueID;
            }

            return obj;
        }

        private void RemoveObject(LevelObject obj)
        {
            if (!m_LevelObjects.ContainsKey(obj.m_UniqueID)) return;

            RemoveObjectSlot(obj);
            obj.Release();
            m_LevelObjects.Remove(obj.m_UniqueID);
            tvObjectList.Nodes.Find(obj.m_UniqueID.ToString("X8"), true)[0].Parent.Nodes.RemoveByKey(obj.m_UniqueID.ToString("X8"));

            if (obj.m_Type == 1)
            {
                IEnumerable<LevelObject> toupdate = m_LevelObjects.Values.Where(obj2 => obj2.m_Type == 1 && ((EntranceObject)obj2).m_EntranceID > ((EntranceObject)obj).m_EntranceID);
                foreach (LevelObject entrance in toupdate)
                {
                    ((EntranceObject)entrance).m_EntranceID--;
                    tvObjectList.Nodes.Find(entrance.m_UniqueID.ToString("X8"), true)[0].Text = entrance.GetDescription();
                }
            }
        }

        private void RelocateObject(LevelObject obj, int layer, int area)
        {
            RemoveObjectSlot(obj);
            obj.m_Offset = AddObjectSlot(obj.m_Type, layer, area);
        }

        private void CopyObject(LevelObject objectToCopy)
        {
            int type = objectToCopy.m_Type;
            ushort id = objectToCopy.ID;
            if (type == 0 && IsSimpleObject(id))
                type = 5;

            LevelObject obj = AddObject(type, id, objectToCopy.m_Layer, objectToCopy.m_Area);
            obj.Position = objectToCopy.Position;
            if (obj.Parameters != null) Array.Copy(objectToCopy.Parameters, obj.Parameters, obj.Parameters.Length);
            obj.GenerateProperties();
            pgObjectProperties.SelectedObject = obj.m_Properties;

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            RefreshObjects(m_SelectedObject.m_Layer);
        }

        //private bool m_LevelModified;

        // level data
        public LevelSettings m_LevelSettings;
        public int m_NumAreas;
        public Dictionary<uint, LevelObject> m_LevelObjects;
        public List<LevelTexAnim>[] m_TexAnims;

        //public uint m_MinimapFileIDsOffset;
        public ushort[] m_MinimapFileIDs;

        private bool m_GLLoaded;

        // 3D view settings
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

        private bool m_ShowCommonLayer;
        private int m_AuxLayerNum;
        private int m_EditMode;

        private MouseButtons m_MouseDown;
        private Point m_LastMouseClick, m_LastMouseMove;
        private Point m_MouseCoords;

        private uint m_Hovered, m_LastHovered;
        private uint m_Selected, m_LastSelected;
        private uint m_LastClicked;
        private LevelObject m_HoveredObject;
        private LevelObject m_SelectedObject;
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

        CultureInfo usa = new CultureInfo("en-US");

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

            m_CamRotation = new Vector2(0.0f, (float)Math.PI / 8.0f);
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

            m_EntranceID = 0;
            m_PathNodeID = 0;
            m_MinimapTileIDNum = 0;


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
        }

        private void glLevelView_Resize(object sender, EventArgs e)
        {
            if (!m_GLLoaded) return;
            glLevelView.Context.MakeCurrent(glLevelView.WindowInfo);

            GL.Viewport(glLevelView.ClientRectangle);

            m_AspectRatio = (float)glLevelView.Width / (float)glLevelView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projmtx = Matrix4.CreatePerspectiveFieldOfView(k_FOV, m_AspectRatio, k_zNear, k_zFar);
            //Matrix4 projmtx = Matrix4.CreateOrthographic(2f, 2f, 0.01f, 1000f);
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

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // axes (temp)
            /*GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.LineWidth(2.0f);
            GL.Begin(BeginMode.Lines);
            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(500f, 0f, 0f);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 500f, 0f);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 500f);
            GL.End();*/

#if false
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.LineWidth(1f);
           // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            // enable this to view the level's collision map (slow)
            int n = 0;
            foreach (KCL.Plane plane in m_LevelCollMap.m_Planes)
	        {
                Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Magenta, Color.Yellow, Color.White, Color.OrangeRed };
                GL.Color3(colors[n & 7]); 
                //GL.Color3(colors[(n >> 12) & 7]);
                n++;
                //if (n != 37) continue;

                GL.Begin(BeginMode.Triangles);
		        Vector3 lol1 = Vector3.Cross(plane.m_Dir1, plane.m_Normal);
                float lol1len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f,Vector3.Dot(lol1, plane.m_Dir3))));
                Vector3 pta = Vector3.Add(plane.m_Position, Vector3.Multiply(lol1, lol1len));

		        Vector3 lol2 = Vector3.Cross(plane.m_Normal, plane.m_Dir2);
		        float lol2len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f,Vector3.Dot(lol2, plane.m_Dir3))));
                Vector3 ptb = Vector3.Add(plane.m_Position, Vector3.Multiply(lol2, lol2len));
		        //GL.Vertex3(plane.m_Position);

                //if (pta.Length > 100f || ptb.Length > 100f || plane.m_Position.Length > 100f)
                //    MessageBox.Show(string.Format("degenerated plane {0}: {1} {2} {3}\n\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}", 
                //        n, pta, plane.m_Position, ptb, plane.m_Position, plane.m_Normal, plane.m_Dir1, plane.m_Dir2, plane.m_Dir3, plane.m_Length));

               /* if (n == 32)
                    MessageBox.Show(String.Format("1: {0} {1}\n2: {2} {3}\n\n{4}", 
                        lol1, lol1len, lol2, lol2len,
                        Vector3.Dot(lol2, plane.m_Dir3)));*/

                GL.Vertex3(pta);
                GL.Vertex3(plane.m_Position);
		        GL.Vertex3(ptb);

		        GL.End();
            }

            // enable this to view the octree cubes (slow as well)
            //foreach (KCL.OctreeNode node in KCL.OctreeNode.m_List)
            /*KCL.OctreeNode node = KCL.OctreeNode.m_List[lol];
            {
                Vector3 s0 = node.m_Pos;
                Vector3 s1 = node.m_Pos + node.m_Size;

                //if (node.m_LOL)
                //    GL.Color3(Color.LimeGreen);
               // else
              //  if (node.m_NumPlanes > 8)
                {
                    if (node.m_NumPlanes > 22)
                        GL.Color3(Color.Red);
                    else
                        GL.Color3(Color.Blue);
                }
               // else
                //    continue;
                    //GL.Color3(Color.Green);

                GL.Begin(BeginMode.LineStrip);
                GL.Vertex3(s1.X, s1.Y, s1.Z);
                GL.Vertex3(s0.X, s1.Y, s1.Z);
                GL.Vertex3(s0.X, s1.Y, s0.Z);
                GL.Vertex3(s1.X, s1.Y, s0.Z);
                GL.Vertex3(s1.X, s1.Y, s1.Z);
                GL.Vertex3(s1.X, s0.Y, s1.Z);
                GL.Vertex3(s0.X, s0.Y, s1.Z);
                GL.Vertex3(s0.X, s0.Y, s0.Z);
                GL.Vertex3(s1.X, s0.Y, s0.Z);
                GL.Vertex3(s1.X, s0.Y, s1.Z);
                GL.End();

                GL.Begin(BeginMode.Lines);
                GL.Vertex3(s0.X, s1.Y, s1.Z);
                GL.Vertex3(s0.X, s0.Y, s1.Z);
                GL.Vertex3(s0.X, s1.Y, s0.Z);
                GL.Vertex3(s0.X, s0.Y, s0.Z);
                GL.Vertex3(s1.X, s1.Y, s0.Z);
                GL.Vertex3(s1.X, s0.Y, s0.Z);
                GL.End();

                foreach (int plol in node.m_PlaneList)
                {
                    KCL.Plane plane = m_LevelCollMap.m_Planes[plol];

                    GL.Color3(Color.LimeGreen);
                    GL.Begin(BeginMode.Triangles);
                    Vector3 lol1 = Vector3.Cross(plane.m_Dir1, plane.m_Normal);
                    float lol1len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f, Vector3.Dot(lol1, plane.m_Dir3))));
                    GL.Vertex3(Vector3.Add(plane.m_Position, Vector3.Multiply(lol1, lol1len)));

                    GL.Vertex3(plane.m_Position);

                    Vector3 lol2 = Vector3.Cross(plane.m_Normal, plane.m_Dir2);
                    float lol2len = plane.m_Length / (float)Math.Cos(Math.Acos(Math.Min(1f, Vector3.Dot(lol2, plane.m_Dir3))));
                    //GL.Vertex3(plane.m_Position);
                    GL.Vertex3(Vector3.Add(plane.m_Position, Vector3.Multiply(lol2, lol2len)));

                    GL.End();
                }
            }*/
#endif

#if false
            /*Bitmap test = new Bitmap(glLevelView.Width, glLevelView.Height);
            Graphics g = Graphics.FromImage(test);
            g.Clear(Color.FromArgb(0, 0, 0, 0));
            Pen plol = new Pen(Color.FromArgb(255, 255, 0, 0));
            Brush rofl = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
            g.DrawRectangle(plol, 20, 30, 50, 50);
            g.FillRectangle(rofl, 21, 31, 48, 48);*/

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glLevelView.Width, glLevelView.Height, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Disable(EnableCap.DepthTest);

            /*byte[] loldata = new byte[glLevelView.Width * glLevelView.Height * 4];
            for (int y = 0; y < glLevelView.Height; y++)
            {
                for (int x = 0; x < glLevelView.Width; x++)
                {
                    Color c = test.GetPixel(x, y);
                    loldata[(y * glLevelView.Width + x) * 4    ] = c.B;
                    loldata[(y * glLevelView.Width + x) * 4 + 1] = c.G;
                    loldata[(y * glLevelView.Width + x) * 4 + 2] = c.R;
                    loldata[(y * glLevelView.Width + x) * 4 + 3] = c.A;
                }
            }*/

           // System.Drawing.Imaging.BitmapData lolbmp = test.LockBits(glLevelView.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
          //  GL.BindTexture(TextureTarget.Texture2D, warp);
           // GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, glLevelView.Width, glLevelView.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, loldata);
           // test.UnlockBits(lolbmp);

          /*  GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);*/

           // GL.Enable(EnableCap.Texture2D);
           // GL.Color4(Color.FromArgb(255, 255, 0, 255));
            GL.Color4(Color.FromArgb(128, 0, 0, 255));

            GL.Begin(BeginMode.Quads);
           /* GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, glLevelView.Height);
            GL.TexCoord2(1, 1);
            GL.Vertex2(glLevelView.Width, glLevelView.Height);
            GL.TexCoord2(1, 0);
            GL.Vertex2(glLevelView.Width, 0);*/
            GL.Vertex2(20, 30);
            GL.Vertex2(20, 30 + 50);
            GL.Vertex2(20 + 50, 30 + 50);
            GL.Vertex2(20 + 50, 30);
            GL.End();

            GL.Enable(EnableCap.DepthTest);
#endif

            glLevelView.SwapBuffers();
        }

        private void glLevelView_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_MouseDown != MouseButtons.None) return;

            if (m_ObjectBeingPlaced != 0xFFFF)
            {
                int type = (int)(m_ObjectBeingPlaced >> 16);
                ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);
                if (type == 0 && IsSimpleObject(id))
                    type = 5;

                LevelObject obj = AddObject(type, id, 0, 0);
                obj.Position = Get3DCoords(e.Location, 2f);
                obj.GenerateProperties();
                pgObjectProperties.SelectedObject = obj.m_Properties;

                m_Selected = obj.m_UniqueID;
                m_SelectedObject = obj;
                m_LastSelected = obj.m_UniqueID;
                m_Hovered = obj.m_UniqueID;
                m_HoveredObject = obj;
                m_LastHovered = obj.m_UniqueID;
                m_LastClicked = obj.m_UniqueID;

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
                    uint type = (sel >> 28);
                    if (type == m_EditMode)
                    {
                        LevelObject obj = m_LevelObjects[sel];
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
                uint type = (sel >> 28);

                if (type == m_EditMode && type != 0 && type != 5)
                {
                    m_Selected = sel;

                    if (m_LastSelected != m_Selected)
                    {
                        LevelObject obj = m_LevelObjects[sel];
                        RenderObjectHilite(obj, k_SelectionColor, m_SelectHiliteDL);
                        m_LastSelected = m_Selected;
                        m_SelectedObject = obj;

                        pgObjectProperties.SelectedObject = obj.m_Properties;
                        tvObjectList.SelectedNode = tvObjectList.Nodes.Find(obj.m_UniqueID.ToString("X8"), true)[0];
                    }
                }
                else
                {
                    m_Selected = 0xFFFFFFFF;
                    m_LastSelected = 0xFFFFFFFF;
                    m_SelectedObject = null;

                    pgObjectProperties.SelectedObject = null;
                    tvObjectList.SelectedNode = null;
                }
            }

            m_MouseDown = MouseButtons.None;
        }

        private void glLevelView_MouseMove(object sender, MouseEventArgs e)
        {
            float xdelta = (float)(e.X - m_LastMouseMove.X);
            float ydelta = (float)(e.Y - m_LastMouseMove.Y);

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
                        xdelta *= Math.Min(0.005f, m_PixelFactorX * m_PickingDepth);
                        ydelta *= Math.Min(0.005f, m_PixelFactorY * m_PickingDepth);

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
                        xdelta *= m_PixelFactorX * objz;
                        ydelta *= -m_PixelFactorY * objz;

                        float _xdelta = (xdelta * (float)Math.Sin(m_CamRotation.X)) - (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                        float _ydelta = ydelta * (float)Math.Cos(m_CamRotation.Y);
                        float _zdelta = (xdelta * (float)Math.Cos(m_CamRotation.X)) + (ydelta * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                        m_SelectedObject.Position.X += _xdelta;
                        m_SelectedObject.Position.Y += _ydelta;
                        m_SelectedObject.Position.Z -= _zdelta;
                    }

                    UpdateSelection();
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
                    uint type = (sel >> 28);
                    if (type == m_EditMode)
                    {
                        m_Hovered = sel;
                        if ((type == 0) || (type == 5))
                        {
                            m_LastHovered = 0xFFFFFFFF;
                            m_HoveredObject = null;
                        }
                        else
                            if (m_LastHovered != m_Hovered)
                            {
                                LevelObject obj = m_LevelObjects[sel];
                                RenderObjectHilite(obj, k_HoverColor, m_HoverHiliteDL);
                                m_LastHovered = m_Hovered;
                                m_HoveredObject = obj;
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

                m_SelectedObject.Position.X += delta * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);
                m_SelectedObject.Position.Y += delta * (float)Math.Sin(m_CamRotation.Y);
                m_SelectedObject.Position.Z += delta * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y);

                float xdist = delta * (m_MouseCoords.X - (glLevelView.Width / 2f)) * m_PixelFactorX;
                float ydist = delta * (m_MouseCoords.Y - (glLevelView.Height / 2f)) * m_PixelFactorY;

                m_SelectedObject.Position.X -= (xdist * (float)Math.Sin(m_CamRotation.X)) + (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Cos(m_CamRotation.X));
                m_SelectedObject.Position.Y += ydist * (float)Math.Cos(m_CamRotation.Y);
                m_SelectedObject.Position.Z += (xdist * (float)Math.Cos(m_CamRotation.X)) - (ydist * (float)Math.Sin(m_CamRotation.Y) * (float)Math.Sin(m_CamRotation.X));

                UpdateSelection();
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

        private Vector3 Get3DCoords(Point coords2d, float depth)
        {
            Vector3 ret = m_CamPosition;

            ret.X -= (depth * (float)Math.Cos(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y));
            ret.Y -= (depth * (float)Math.Sin(m_CamRotation.Y));
            ret.Z -= (depth * (float)Math.Sin(m_CamRotation.X) * (float)Math.Cos(m_CamRotation.Y));

            float x = (coords2d.X - (glLevelView.Width / 2f)) * m_PixelFactorX * depth;
            float y = -(coords2d.Y - (glLevelView.Height / 2f)) * m_PixelFactorY * depth;

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

        private void ReleaseObjectTable(List<LevelObject> list)
        {
            foreach (LevelObject obj in list)
                obj.Release();
        }

        private void LevelEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save confirm goes here

            foreach (LevelObject obj in m_LevelObjects.Values)
                obj.Release();

            m_LevelModel.Release();
            if (m_SkyboxModel != null)
                ModelCache.RemoveModel(m_SkyboxModel);

            Program.m_LevelEditors.Remove(this);
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

            PopulateObjectList();
            glLevelView.Refresh();
        }

        private void btnStarAll_Click(object sender, EventArgs e)
        {
            m_ShowCommonLayer = btnStarAll.Checked;
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

            m_EditMode = int.Parse((string)btn.Tag);

            for (int l = 0; l < 8; l++)
                RenderObjectLists(RenderMode.Picking, l);
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
            ModelImporter form = new ModelImporter(Program.m_ROM.GetFileFromInternalID(m_LevelSettings.BMDFileID).m_Name, Program.m_ROM.GetFileFromInternalID(m_LevelSettings.KCLFileID).m_Name);
            if (form.ShowDialog(this) == DialogResult.Cancel)
                return;
        }

        private void tvObjectList_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Color fgcolor, bgcolor;

            Font font = e.Node.NodeFont;
            if (font == null) font = tvObjectList.Font;

            bool red = false;
            if (e.Node.Tag is uint && m_LevelObjects.ContainsKey((uint)e.Node.Tag))
            {
                uint uniqueid = (uint)e.Node.Tag;
                if ((uniqueid >> 28) == 1)
                    red = !m_ObjAvailable[m_LevelObjects[uniqueid].ID];
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
            new LevelSettingsForm(m_LevelSettings).ShowDialog(this);
            GetObjectsAvailable();
            tvObjectList.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool bankwarning = false;
            IEnumerable<LevelObject> objs = m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 1);
            foreach (LevelObject obj in objs)
                if (!m_ObjAvailable[obj.ID])
                {
                    bankwarning = true;
                    break;
                }

            if (bankwarning)
            {
                DialogResult res = MessageBox.Show("This level contains objects which aren't available with the current object bank settings, and would crash the game.\n\nSave anyway?",
                    Program.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            }

            m_LevelSettings.SaveChanges();

            foreach (LevelObject obj in m_LevelObjects.Values)
                obj.SaveChanges();

            m_Overlay.SaveChanges();
            slStatusLabel.Text = "Changes saved.";
        }

        private void tvObjectList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;

            uint objid = (uint)e.Node.Tag;
            m_Selected = m_LastSelected = objid;
            m_SelectedObject = m_LevelObjects[objid];
            pgObjectProperties.SelectedObject = m_SelectedObject.m_Properties;
            if (m_SelectedObject.m_Type == 3)
            {
                // If object selected is a path, highlight all nodes in current path
                IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj => (obj.m_Type) == 2);
                List<LevelObject> nodes = new List<LevelObject>();
                for (int i = ((PathObject)m_SelectedObject).Parameters[0]; i < (((PathObject)m_SelectedObject).Parameters[0] + ((PathObject)m_SelectedObject).Parameters[1]); i++)
                {
                    PathPointObject node = (PathPointObject)pathNodes.Where(obj0 => ((PathPointObject)obj0).m_NodeID == i).ElementAt(0);
                    nodes.Add(node);
                }
                RenderPathHilite(nodes, k_SelectionColor, m_SelectHiliteDL);
            }
            else
                RenderObjectHilite(m_SelectedObject, k_SelectionColor, m_SelectHiliteDL);
            glLevelView.Refresh();
        }

        private void pgObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (m_SelectedObject == null) // should never happen but we never know
            {
                MessageBox.Show("No object was selected. This shouldn't have happened. Tell Mega-Mario about it.", "Bug!");
                return;
            }

            if (e.ChangedItem.Label == "Object ID")
            {
                if (IsSimpleObject((ushort)e.ChangedItem.Value) ^ IsSimpleObject(m_SelectedObject.ID))
                {
                    LevelObject oldobj = m_SelectedObject;
                    RemoveObject(oldobj);

                    ushort newid = (ushort)e.ChangedItem.Value;
                    int type = IsSimpleObject(newid) ? 5 : 0;
                    LevelObject obj = AddObject(type, newid, oldobj.m_Layer, oldobj.m_Area);
                    obj.Position = oldobj.Position;
                    obj.Parameters[0] = (ushort)(oldobj.Parameters[0] & ((type == 5) ? 0x007F : 0xFFFF));
                    obj.GenerateProperties();
                    pgObjectProperties.SelectedObject = obj.m_Properties;

                    m_Selected = obj.m_UniqueID;
                    m_SelectedObject = obj;
                    m_LastSelected = obj.m_UniqueID;
                    m_Hovered = obj.m_UniqueID;
                    m_HoveredObject = obj;
                    m_LastHovered = obj.m_UniqueID;
                    m_LastClicked = obj.m_UniqueID;

                    RefreshObjects(obj.m_Layer);
                    return;
                }
            }

            if (e.ChangedItem.Label == "Star")
            {
                int newstar;
                if (e.ChangedItem.Value is int) newstar = (int)e.ChangedItem.Value;
                else if ((string)e.ChangedItem.Value == "All") newstar = 0;
                else newstar = int.Parse((string)e.ChangedItem.Value);

                RelocateObject(m_SelectedObject, newstar, m_SelectedObject.m_Area);
                return;
            }

            if (e.ChangedItem.Label == "Area")
            {
                RelocateObject(m_SelectedObject, m_SelectedObject.m_Layer, (int)e.ChangedItem.Value);
                return;
            }

            int actmask = m_SelectedObject.SetProperty(e.ChangedItem.Label, e.ChangedItem.Value);
            if ((actmask & 4) != 0)
                tvObjectList.Nodes.Find(m_SelectedObject.m_UniqueID.ToString("X8"), true)[0].Text = m_SelectedObject.GetDescription();
            if ((actmask & 2) != 0)
                pgObjectProperties.Refresh();
            if ((actmask & 1) != 0)
                RefreshObjects(m_SelectedObject.m_Layer);
        }

        private void btnAddObject_Click(object sender, EventArgs e)
        {
            btnRemoveSel.Checked = false;

            ObjectListForm objlist = new ObjectListForm(0);
            if (objlist.ShowDialog(this) != DialogResult.OK) return;
            if (objlist.ObjectID > 0x145 && objlist.ObjectID != 0x1FF) return;

            m_ObjectBeingPlaced = objlist.ObjectID;
            string placementMsg = "Click anywhere in the level to place your new object ({0} - {1}). Hold Shift while clicking to place multiple objects. Hit Escape to abort.";
            slStatusLabel.Text = (objlist.ObjectID < 326) ? 
                string.Format(placementMsg, objlist.ObjectID, ObjectDatabase.m_ObjectInfo[objlist.ObjectID].m_Name) : 
                string.Format(placementMsg, 511, "Minimap change");
        }

        private void btnAddWhatever_Click(object sender, EventArgs e)
        {
            btnRemoveSel.Checked = false;

            uint type = uint.Parse((string)((ToolStripItem)sender).Tag);
            m_ObjectBeingPlaced = type << 16;

            string obj = "OSHIT BUG";
            switch (type)
            {
                case 1: obj = "entrance"; break;
                case 4: obj = "view"; break;
                case 6: obj = "teleport source"; break;
                case 7: obj = "teleport destination"; break;
                case 9: obj = "door"; break;
                case 10: obj = "exit"; break;
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

            if (obj.m_Type == 2)
                UpdatePathsNodeRemoved((PathPointObject)obj);

            RemoveObject(obj);
            RefreshObjects(obj.m_Layer);
            slStatusLabel.Text = "Object removed.";
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
            if (Properties.Settings.Default.lolhax)
            {
                btnStarAll.Checked = !btnStarAll.Checked;
                m_ShowCommonLayer = btnStarAll.Checked;
                glLevelView.Refresh();
                return;
            }

            new TempHaxForm().ShowDialog();
            btnImportModel.Visible = (m_EditMode == 0) && Properties.Settings.Default.lolhax;
        }

        private void btnDumpOverlay_Click(object sender, EventArgs e)
        {
            string filename = "level" + m_LevelID.ToString() + "_overlay.bin";
            System.IO.File.WriteAllBytes(filename, m_Overlay.m_Data);
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
            new MinimapEditor().Show(this);
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
                slStatusLabel.Text = "This object uses more than one model, use 'Import Other Model' to replace them.";
                return;
            }

            //Get the name of the selected object's BMD (model) file
            string selObjBMDName = m_SelectedObject.m_Renderer.GetFilename();
            //Get the name of the selected object's KCL (collision data) file
            string selObjKCLName = selObjBMDName.Substring(0, selObjBMDName.Length - 4) + ".kcl";

            ModelImporter form = new ModelImporter(selObjBMDName, selObjKCLName, m_SelectedObject.m_Renderer.GetScale().X);
            if (form != null && !form.m_EarlyClosure)
                form.ShowDialog(this);

            ModelCache.RemoveModel(m_SelectedObject.m_Renderer.GetFilename());
        }

        private void btnEditTexAnim_Click(object sender, EventArgs e)
        {
            new TextureAnimationForm(this).Show(this);
        }

        private void btnCLPS_Click(object sender, EventArgs e)
        {
            new CLPS_Form(this).Show(this);
        }

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            uint type0 = 3;
            m_ObjectBeingPlaced = type0 << 16;

            int type = (int)(m_ObjectBeingPlaced >> 16);
            ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);

            LevelObject obj = AddObject(type, id, 0, 0);
            obj.GenerateProperties();
            pgObjectProperties.SelectedObject = obj.m_Properties;

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            RefreshObjects(m_SelectedObject.m_Layer);

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        private int m_CurrentPathID = -1;
        void btnAddPathNodes_DropDownItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            uint type0 = 2;
            m_ObjectBeingPlaced = type0 << 16;

            int type = (int)(m_ObjectBeingPlaced >> 16);
            ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);

            // Parse path to which node is to be added
            String chosenPath = e.ClickedItem.Text;
            m_CurrentPathID = int.Parse(chosenPath.Substring(17, chosenPath.Length - 17));

            IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 3);
            IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 2);
            long lastNodeInPathOff = -1;
            try
            {
                lastNodeInPathOff = pathNodes.ElementAt(paths.ElementAt(m_CurrentPathID).Parameters[0] +
                    (paths.ElementAt(m_CurrentPathID).Parameters[1] - 1)).m_Offset;
            }
            catch { }

            int nodeID = ((PathObject)paths.ElementAt(m_CurrentPathID)).Parameters[0] +
                            ((PathObject)paths.ElementAt(m_CurrentPathID)).Parameters[1];

            // Update Node ID's of following path nodes
            for (int i = pathNodes.Count() - 1; i >= nodeID; i--)
            {
                PathPointObject node = (PathPointObject)pathNodes.Where(obj0 => ((PathPointObject)obj0).m_NodeID == i).ElementAt(0);
                node.m_NodeID++;
            }

            // If possible, create object after last node in path
            LevelObject obj = AddObject(type, id, 0, 0, ((lastNodeInPathOff != -1) ? ((int)lastNodeInPathOff + 6) : -1));
            obj.GenerateProperties();
            pgObjectProperties.SelectedObject = obj.m_Properties;

            // Update start indices and lengths of paths
            for (int i = m_CurrentPathID; i < paths.Count(); i++)
            {
                if (i == m_CurrentPathID)
                {
                    // Increase length of parent path
                    paths.ElementAt(i).Parameters[1] += 1;
                }
                else if (i > m_CurrentPathID)
                {
                    // Increase start node index for all following paths
                    paths.ElementAt(i).Parameters[0] += 1;
                }
                paths.ElementAt(i).GenerateProperties();
            }

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            RefreshObjects(m_SelectedObject.m_Layer);
            PopulateObjectList();

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        public int GetPathNodeParentIDFromNodeID(int nodeID)
        {
            int pos = -1;

            IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 3);
            IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 2);
            for (int i = 0; i < paths.Count(); i++)
            {
                if (nodeID >= paths.ElementAt(i).Parameters[0] &&
                    nodeID < paths.ElementAt(i).Parameters[0] + paths.ElementAt(i).Parameters[1])
                {
                    pos = i;
                    break;
                }
            }

            return pos;
        }

        void UpdatePathsNodeRemoved(PathPointObject removedNode)
        {
            int pathNum = GetPathNodeParentIDFromNodeID(removedNode.m_NodeID);
            if (pathNum == -1) return;

            // Decrease length of current path
            IEnumerable<LevelObject> paths = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 3);
            paths.ElementAt(pathNum).Parameters[1] -= 1;
            paths.ElementAt(pathNum).GenerateProperties();
            // Decrease starting indices of following paths
            for (int i = pathNum + 1; i < paths.Count(); i++)
            {
                paths.ElementAt(i).Parameters[0] -= 1;
                paths.ElementAt(i).GenerateProperties();
            }
            IEnumerable<LevelObject> pathNodes = m_LevelObjects.Values.Where(obj0 => (obj0.m_Type) == 2);
            // Update Node ID's of following path nodes
            for (int i = removedNode.m_NodeID; i < pathNodes.Count(); i++)
            {
                PathPointObject node = (PathPointObject)pathNodes.Where(obj0 => ((PathPointObject)obj0).m_NodeID == i).ElementAt(0);
                node.m_NodeID--;
            }
        }

        void btnAddMisc_DropDownItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            int selected = int.Parse(e.ClickedItem.Tag.ToString());

            uint type0 = (uint)selected;
            m_ObjectBeingPlaced = type0 << 16;

            int type = (int)(m_ObjectBeingPlaced >> 16);
            ushort id = (ushort)(m_ObjectBeingPlaced & 0xFFFF);

            LevelObject obj = AddObject(type, id, 0, 0);
            obj.GenerateProperties();
            pgObjectProperties.SelectedObject = obj.m_Properties;

            m_Selected = obj.m_UniqueID;
            m_SelectedObject = obj;
            m_LastSelected = obj.m_UniqueID;
            m_Hovered = obj.m_UniqueID;
            m_HoveredObject = obj;
            m_LastHovered = obj.m_UniqueID;
            m_LastClicked = obj.m_UniqueID;

            RefreshObjects(m_SelectedObject.m_Layer);

            if (!m_ShiftPressed)
            {
                m_ObjectBeingPlaced = 0xFFFF;
                slStatusLabel.Text = "Object added.";
            }
        }

        private void btnExportLevelModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveModel = new SaveFileDialog();
            saveModel.FileName = "SM64DS_Model";//Default name
            saveModel.DefaultExt = ".dae";//Default file extension
            saveModel.Filter = "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";//Filter by .DAE and .OBJ
            if (saveModel.ShowDialog() == DialogResult.Cancel)
                return;

            BMD_BCA_KCLExporter.ExportBMDModel(new BMD(m_ROM.GetFileFromInternalID(m_LevelSettings.BMDFileID)), saveModel.FileName);

            slStatusLabel.Text = "Finished exporting level model.";
        }//End Method

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

                SaveFileDialog saveModel = new SaveFileDialog();
                saveModel.FileName = "SM64DS_Model_" + name.Substring(name.LastIndexOf("/"));//Default name
                saveModel.DefaultExt = ".dae";//Default file extension
                saveModel.Filter = "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";//Filter by .DAE and .OBJ
                if (saveModel.ShowDialog() == DialogResult.Cancel)
                    return;

                BMD_BCA_KCLExporter.ExportBMDModel(objectBMD, saveModel.FileName);
            }

            slStatusLabel.Text = "Finished exporting model.";
        }

        private void btnImportOtherModel_Click(object sender, EventArgs e)
        {
            using (var form = new ROMFileSelect("Please select a model (BMD) file to replace."))
            {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    String modelName = form.m_SelectedFile;
                    ModelImporter mdlImp = new ModelImporter(modelName, modelName.Substring(0, modelName.Length - 4) + ".kcl");
                    if (mdlImp != null && !mdlImp.m_EarlyClosure)
                        mdlImp.ShowDialog(this);
                }
            }
        }

        private void btnExportOtherModel_Click(object sender, EventArgs e)
        {
            using (var form = new ROMFileSelect("Please select a model (BMD) file to export."))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SaveFileDialog saveModel = new SaveFileDialog();
                    saveModel.FileName = "SM64DS_Model";//Default name
                    saveModel.DefaultExt = ".dae";//Default file extension
                    saveModel.Filter = "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";//Filter by .DAE and .OBJ
                    if (saveModel.ShowDialog() == DialogResult.Cancel)
                        return;

                    BMD objectBMD = new BMD(m_ROM.GetFileFromName(form.m_SelectedFile));

                    BMD_BCA_KCLExporter.ExportBMDModel(objectBMD, saveModel.FileName);

                    slStatusLabel.Text = "Finished exporting model.";
                }
            }
        }

        private void btnOffsetAllCoords_Click(object sender, EventArgs e)
        {
            new OffsetAllObjectCoordsForm().Show(this);
        }

        private void btnExportXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Level_" + m_LevelID;
            sfd.DefaultExt = ".xml";//Default file extension
            sfd.Filter = "XML Document (.xml)|*.xml";//Filter by .xml

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                LevelDataXML_Exporter.ExportLevelDataToXML(m_Overlay, m_LevelID, m_LevelSettings,
                    m_LevelObjects, m_TexAnims, sfd.FileName);

                slStatusLabel.Text = "Level successfully exported.";
            }
        }

        private void btnImportXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "Level_" + m_LevelID;
            ofd.DefaultExt = ".xml";//Default file extension
            ofd.Filter = "XML Document (.xml)|*.xml";//Filter by .xml

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                int success = LevelDataXML_Importer.ImportLevelDataFromXML(ofd.FileName, m_Overlay, m_LevelID, m_MinimapFileIDs, false);

                if (success != 0)
                {
                    slStatusLabel.Text = "Level importing failed, no changes have been saved. It is advised to reload the level.";
                    return;
                }

                LevelDataXML_Importer.SaveChangesToAllFiles();

                foreach (LevelObject obj in m_LevelObjects.Values)
                    obj.Release();

                InitialiseLevel();

                slStatusLabel.Text = "Level imported successfully.";
            }

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

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Screenshot_Level_" + m_LevelID;
            sfd.DefaultExt = ".png";//Default file extension
            sfd.Filter = "PNG Image (.png)|*.png";//Filter by .png

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    dump.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    slStatusLabel.Text = "Screenshot saved.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while trying to save texture: \n\n" +
                        ex.Message + "\n" + ex.Data + "\n" + ex.StackTrace + "\n" + ex.Source);
                }
            }
        }
    }
}
