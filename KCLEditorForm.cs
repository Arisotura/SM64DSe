using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;

namespace SM64DSe
{
    public partial class KCLEditorForm : Form
    {
        private bool m_WireFrameView = false;

        private int[] m_KCLMeshPickingDLists = new int[1]; // Picking
        private int[] m_KCLMeshDLists = new int[3]; // Fill, WireFrame, Highlight

        public List<KCL.ColFace> m_Planes;

        private KCLLoader.CollisionMapColours m_Colours = new KCLLoader.CollisionMapColours();

        private KCL m_KCL;

        private int m_SelectedTriangle;

        private Dictionary<string, int> m_MatColTypes = new Dictionary<string, int>();
        private static Dictionary<string, Dictionary<string, int>> m_SavedMaterialCollisionTypes = new Dictionary<string, Dictionary<string, int>>();

        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect("Please select a collision map (KCL) file to open.");

        public KCLEditorForm(NitroFile kclIn)
        {
            InitializeComponent();
            LoadKCL(kclIn);

            cmbPolygonMode.Items.Add("Fill");
            cmbPolygonMode.Items.Add("Wireframe");
            cmbPolygonMode.SelectedIndex = 0;

            glModelView.Initialise();
            glModelView.ProvidePickingDisplayLists(m_KCLMeshPickingDLists);
            glModelView.ProvideDisplayLists(m_KCLMeshDLists);
            glModelView.ProvideCallListForDisplayLists(CallListForKCLDisplayLists);
        }

        public void LoadKCL(NitroFile kcl)
        {
            m_KCL = new KCL(kcl);

            m_Planes = m_KCL.m_Planes;

            lbxPlanes.Items.Clear();

            for (int i = 0; i < m_Planes.Count; i++)
            {
                lbxPlanes.Items.Add("Plane " + i.ToString("00000"));
            }
        }

        private void WriteChanges()
        {
            NitroFile kclFile = m_KCL.m_File;

            uint planeStart = (kclFile.Read32(8));

            planeStart += (uint)(0x10);

            for (int i = 0; i < m_Planes.Count; i++)
            {
                uint posColType = (uint)(planeStart + (i * 16) + 0x0E);//Get the address of this plane's Collision Type variable

                kclFile.Write16(posColType, (ushort)m_Planes[i].type);//Write the new value to file
            }

            kclFile.SaveChanges();
        }

        private void glModelView_Load(object sender, EventArgs e)
        {
            RenderKCLMesh();
        }

        private void RenderKCLMesh()
        {
            m_KCLMeshPickingDLists[0] = GL.GenLists(1);
            GL.NewList(m_KCLMeshPickingDLists[0], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                GL.Begin(PrimitiveType.Triangles);
                GL.Color4(Color.FromArgb(i));
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.EndList();

            m_KCLMeshDLists[0] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[0], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1f, 1f);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                Color planeColour = m_Colours[m_Planes[i].type];

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(planeColour);
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.EndList();

            m_KCLMeshDLists[1] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[1], ListMode.Compile);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            for (int i = 0; i < m_Planes.Count; i++)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(Color.Orange);
                GL.Vertex3(m_Planes[i].point1);
                GL.Vertex3(m_Planes[i].point2);
                GL.Vertex3(m_Planes[i].point3);
                GL.End();
            }
            GL.EndList();
        }

        private void RenderHighlight()
        {
            m_KCLMeshDLists[2] = GL.GenLists(1);
            GL.NewList(m_KCLMeshDLists[2], ListMode.Compile);
            foreach (int idx in lbxPlanes.SelectedIndices)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.RoyalBlue);
                GL.Vertex3(m_Planes[idx].point1);
                GL.Vertex3(m_Planes[idx].point2);
                GL.Vertex3(m_Planes[idx].point3);
                GL.End();
            }
            GL.EndList();
        }

        protected void CallListForKCLDisplayLists()
        {
            // Solid polygons
            if (!m_WireFrameView)
            {
                GL.CallList(m_KCLMeshDLists[0]);
            }

            // WireFrame overlay
            GL.CallList(m_KCLMeshDLists[1]);

            // Highlighted triangles
            GL.CallList(m_KCLMeshDLists[2]);
        }

        private void glModelView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point lastMouseClick = glModelView.GetLastMouseClick();
                if (Math.Abs(e.X - lastMouseClick.X) > 3 || Math.Abs(e.Y - lastMouseClick.Y) > 3)
                {
                    return;
                }
                m_SelectedTriangle = (int)glModelView.GetColourUnderCursor();
                if (!lbxPlanes.SelectedIndices.Contains(m_SelectedTriangle))
                {
                    lbxPlanes.SelectedIndices.Add(m_SelectedTriangle);
                }
                else
                {
                    lbxPlanes.SelectedIndices.Remove(m_SelectedTriangle);
                }
            }
            else
            {
                m_SelectedTriangle = -1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbxPlanes.SelectedIndex > -1)
            {
                int lastChange;
                int.TryParse(txtColType.Text, out lastChange);
                m_Planes[lbxPlanes.SelectedIndex].type = lastChange;// Make sure to get value of current plane
            }
            WriteChanges();

            LoadKCL(m_KCL.m_File);
            glModelView.Refresh();
        }

        void txtColType_TextChanged(object sender, System.EventArgs e)
        {
            int newColType;
            int.TryParse(txtColType.Text, out newColType);
            foreach (int idx in lbxPlanes.SelectedIndices)
            {
                m_Planes[idx].type = newColType;
            }
        }

        private void lbxPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxPlanes.SelectedIndices.Count == 1)
            {
                int selPos = lbxPlanes.SelectedIndex;

                txtV1.Text = m_Planes[selPos].point1.ToString();
                txtV2.Text = m_Planes[selPos].point2.ToString();
                txtV3.Text = m_Planes[selPos].point3.ToString();
                txtColType.Text = m_Planes[selPos].type.ToString();
                txtNormal.Text = m_Planes[selPos].normal.ToString();
                txtD1.Text = m_Planes[selPos].dir1.ToString();
                txtD2.Text = m_Planes[selPos].dir2.ToString();
                txtD3.Text = m_Planes[selPos].dir3.ToString();
            }
            RenderHighlight();
            glModelView.Refresh();
        }

        private void cmbPolygonMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPolygonMode.SelectedIndex == 0)
                m_WireFrameView = false;
            else if (cmbPolygonMode.SelectedIndex == 1)
                m_WireFrameView = true;

            glModelView.Refresh();
        }

        private void btnExportKCLModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveModel = new SaveFileDialog();
            saveModel.FileName = "CollisionMap";//Default name
            saveModel.DefaultExt = ".dae";//Default file extension
            saveModel.Filter = "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";//Filter by .DAE and .OBJ
            if (saveModel.ShowDialog() == DialogResult.Cancel)
                return;

            BMD_BCA_KCLExporter.ExportKCLModel(m_KCL, saveModel.FileName);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var result = m_ROMFileSelect.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadKCL((Program.m_ROM.GetFileFromName(m_ROMFileSelect.m_SelectedFile)));
                RenderKCLMesh();
                GL.DeleteLists(m_KCLMeshDLists[2], 1); m_KCLMeshDLists[2] = 0;
                glModelView.Refresh();
            }
        }

        private void btnOpenModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Strings.MODEL_FORMATS_FILTER;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtModelName.Text = ofd.FileName;
                m_MatColTypes = KCLImporter.GetMaterialsList(ofd.FileName);
                if (Properties.Settings.Default.RememberMaterialCollisionTypeAssignments)
                {
                    string key = ofd.FileName;
                    if (m_SavedMaterialCollisionTypes.ContainsKey(key))
                    {
                        Dictionary<string, int> materialCollisionTypeAssignments = m_SavedMaterialCollisionTypes[key];
                        if (materialCollisionTypeAssignments.Keys.Count == m_MatColTypes.Keys.Count &&
                            materialCollisionTypeAssignments.Keys.Except(m_MatColTypes.Keys).Count() < 1)
                        {
                            m_MatColTypes = materialCollisionTypeAssignments;
                        }
                        else
                        {
                            m_SavedMaterialCollisionTypes.Remove(key);
                        }
                    }
                }
                PopulateColTypes();
            }
        }

        private void PopulateColTypes()
        {
            gridColTypes.ColumnCount = 2;
            gridColTypes.Columns[0].HeaderText = "Material";
            gridColTypes.Columns[0].ReadOnly = true;
            gridColTypes.Columns[1].HeaderText = "Col. Type";

            int numMats = m_MatColTypes.Count;
            gridColTypes.RowCount = numMats;
            for (int i = 0; i < numMats; i++)
            {
                gridColTypes.Rows[i].Cells[0].Value = m_MatColTypes.Keys.ElementAt(i);
                gridColTypes.Rows[i].Cells[1].Value = m_MatColTypes.Values.ElementAt(i);
            }
        }

        private void btnImportColMap_Click(object sender, EventArgs e)
        {
            float scale;
            if (!Helper.TryParseFloat(txtScale.Text, out scale))
            {
                MessageBox.Show("Please enter a valid float value for scale, eg. 1.23");
                return;
            }
            float faceSizeThreshold;
            if (!Helper.TryParseFloat(txtThreshold.Text, out faceSizeThreshold))
            {
                MessageBox.Show("Please enter a valid float value, eg. 1.23");
                return;
            }

            try
            {
                KCLImporter.ConvertModelToKCL(m_KCL.m_File, txtModelName.Text, scale, faceSizeThreshold, m_MatColTypes);
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox(ex).ShowDialog();
                return;
            }

            LoadKCL(m_KCL.m_File);
            RenderKCLMesh();
            GL.DeleteLists(m_KCLMeshDLists[2], 1); m_KCLMeshDLists[2] = 0;
            glModelView.Refresh();
        }

        private void btnAssignTypes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridColTypes.RowCount; i++)
            {
                m_MatColTypes[gridColTypes.Rows[i].Cells[0].Value.ToString()] = int.Parse(gridColTypes.Rows[i].Cells[1].Value.ToString());
            }
        }

    }
}
