using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SM64DSe
{
    public partial class TextureAnimationForm : Form
    {
        // Important Note: When adding or removing space, make sure scale etc. tables and headers for the following 
        // areas are multiples of 4, so:
        // When adding space, round up to multiples of 4 eg. 9 > 12
        // When removing space, round down to multiples of 4 eg. 9 > 8

        public LevelEditorForm _owner;
        CultureInfo usa = new CultureInfo("en-US");
        public uint numAreas;

        public TextureAnimationForm(LevelEditorForm _owner)
        {
            InitializeComponent();
            this._owner = _owner;

            for (int i = 0; i < _owner.m_NumAreas; i++)
            {
                lbxArea.Items.Add("" + i);
            }
            lbxArea.SelectedIndex = 0;//Make sure an area is selected

            reloadData();
        }

        private void lbxTexAnim_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxTexAnim.SelectedIndex != -1)
                refreshLbx();
        }

        private void refreshLbx()
        {
            txtMaterialName.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getMatName();
            txtNumFrames.Text = "" + _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getNumFrames();
            lbxScale.Items.Clear();
            for (int i = 0; i < _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getScaleTblSize(); i++)
            {
                lbxScale.Items.Add("Scale value " + i);
            }
            lbxRotation.Items.Clear();
            for (int i = 0; i < _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getRotTblSize(); i++)
            {
                lbxRotation.Items.Add("Rotation value " + i);
            }
            lbxTranslation.Items.Clear();
            for (int i = 0; i < _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getTransTblSize(); i++)
            {
                lbxTranslation.Items.Add("Translation value " + i);
            }
            txtScaleStart.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getScaleTblStart().ToString();
            txtScaleSize.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getScaleTblSize().ToString();
            txtRotStart.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getRotTblStart().ToString();
            txtRotSize.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getRotTblSize().ToString();
            txtTransStart.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getTransTblStart().ToString();
            txtTransSize.Text = _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getTransTblSize().ToString();

            //Take this out when done testing
            /*textBox1.Text = 
                "" + _owner.m_Overlay.Read16((uint)(_owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).m_Offset + 0x18)) +
                ",  " + _owner.m_Overlay.Read16((uint)(_owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).m_Offset + 0x1A));
             // Values at 0x08, 0x18 and 0x1A within animation data unknown, seems linked to number of frames possibly*/

            /*MessageBox.Show("Start of scale values: " + _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).m_BaseScaleTblAddr +
                ", end of material name (first null): " + (_owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).m_MatNameOffset +
                _owner.m_TexAnims[lbxArea.SelectedIndex].ElementAt(lbxTexAnim.SelectedIndex).getMatName().Length));*/
        }

        private void lbxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1)
            {
                lbxTexAnim.Items.Clear();
                for (int j = 0; j < _owner.m_TexAnims[lbxArea.SelectedIndex].Count; j++)
                {
                    lbxTexAnim.Items.Add("" + j);
                }
            }
        }

        private float readScaleValue(int area, int texAnim, int index)
        {
            // 20:12 20 bits whole, 12 bits fraction
            float scale = (float)(_owner.m_Overlay.Read32(_owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset +
                ((uint)index * 4)) / 4096f);
            return scale;
        }

        private float readRotationValue(int area, int texAnim, int index)
        {
            // 1024 = 90 degrees. Stored as radians 1024/4096 = 0.25 (2 * Pi) / 4 = 90 deg
            float rotation = (float)(_owner.m_Overlay.Read16(_owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset +
                ((uint)index * 2)) / (1024f / 90f));
            return rotation;
        }

        private float readTranslationValue(int area, int texAnim, int index)
        {
            // 20:12 20 bits whole, 12 bits fraction
            float translation = (float)(_owner.m_Overlay.Read32(_owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset +
                ((uint)index * 4)) / 4096f);
            return translation;
        }

        private void setScaleValue(float value, int area, int texAnim, int index)
        {
            _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset +
                ((uint)index * 4), (uint)(value * 4096f));
        }

        private void setRotationValue(float value, int area, int texAnim, int index)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset +
                ((uint)index * 2), (ushort)(value * (1024f / 90f)));
        }

        private void setTranslationValue(float value, int area, int texAnim, int index)
        {
            _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset +
                ((uint)index * 4), (uint)(value * 4096f));
        }

        private void addScaleValue(float value, int area, int texAnim, int index)
        {
            _owner.AddSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset + ((uint)index * 4), 4);
            _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset +
                ((uint)index * 4), (uint)(value * 4096f));
            setScaleSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getScaleTblSize() + 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                setScaleStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getScaleTblStart() + 1), area, i);
            }
        }

        private void addRotationValue(float value, int area, int texAnim, int index)
        {
            _owner.AddSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset + ((uint)index * 2), 2);
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset +
                ((uint)index * 2), (ushort)(value * (1024f / 90f)));
            setRotationSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getRotTblSize() + 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                setRotationStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getRotTblStart() + 1), area, i);
            }
            // Because each rotation value is 2 bytes, if there's an odd number, following addresses will no 
            // longer be multiples of 4
            if ((float)((float)_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr / 4f) != 0.0f)
                _owner.AddSpace((uint)(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr - 2), 2);
        }

        private void addTranslationValue(float value, int area, int texAnim, int index)
        {
            _owner.AddSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset + ((uint)index * 4), 4);
            _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset +
                ((uint)index * 4), (uint)(value * 4096f));
            setTranslationSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getTransTblSize() + 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                setTranslationStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getTransTblStart() + 1), area, i);
            }
        }

        private void setScaleSize(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x0C, value);
        }

        private void setRotationSize(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x10, value);
        }

        private void setTranslationSize(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x14, value);
        }

        private void setScaleStart(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x0E, value);
            // Also update the value in m_ScaleTblOffset
            _owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset = _owner.m_TexAnims[area].ElementAt(texAnim).m_BaseScaleTblAddr
                + (_owner.m_TexAnims[area].ElementAt(texAnim).getScaleTblStart() * 4);
        }

        private void setRotationStart(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x12, value);
            // Also update the value in m_RotTblOffset
            _owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset = _owner.m_TexAnims[area].ElementAt(texAnim).m_BaseRotTblAddr
                + (_owner.m_TexAnims[area].ElementAt(texAnim).getRotTblStart() * 2);
        }

        private void setTranslationStart(ushort value, int area, int texAnim)
        {
            _owner.m_Overlay.Write16(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset + 0x16, value);
            // Also update the value in m_TransTblOffset
            _owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset = _owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr
                + (_owner.m_TexAnims[area].ElementAt(texAnim).getTransTblStart() * 4);
        }

        private void removeScaleValue(int area, int texAnim, int index)
        {
            _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_ScaleTblOffset + (uint)(index * 4), 4);
            setScaleSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getScaleTblSize() - 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                if (_owner.m_TexAnims[area].ElementAt(i).getScaleTblStart() != 0)
                    setScaleStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getScaleTblStart() - 1), area, i);
            }
        }

        private void removeRotationValue(int area, int texAnim, int index)
        {
            _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_RotTblOffset + (uint)(index * 2), 2);
            setRotationSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getRotTblSize() - 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                if (_owner.m_TexAnims[area].ElementAt(i).getRotTblStart() != 0)
                    setRotationStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getRotTblStart() - 1), area, i);
            }
            // Because each rotation value is 2 bytes, if there's an odd number, following addresses will no 
            // longer be multiples of 4
            if ((float)((float)_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr / 4f) != 0.0f)
                _owner.AddSpace((uint)(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr - 2), 2);
        }

        private void removeTranslationValue(int area, int texAnim, int index)
        {
            _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_TransTblOffset + (uint)(index * 4), 4);
            setTranslationSize((ushort)(_owner.m_TexAnims[area].ElementAt(texAnim).getTransTblSize() - 1), area, texAnim);
            // Need to update start indices for following texture animations in same area
            for (int i = texAnim + 1; i < _owner.m_TexAnims[area].Count; i++)
            {
                if (_owner.m_TexAnims[area].ElementAt(i).getTransTblStart() != 0)
                    setTranslationStart((ushort)(_owner.m_TexAnims[area].ElementAt(i).getTransTblStart() - 1), area, i);
            }
        }

        private void setMaterialName(string value, int area, int texAnim)
        {
            string old = _owner.m_TexAnims[area].ElementAt(texAnim).getMatName();
            int delta = value.Length - old.Length;
            bool endNull = false;
            if (delta > 0)
            {
                // Need to make room
                delta = delta + (4 - (delta % 4));
                _owner.AddSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset + (uint)old.Length, (uint)delta);
            }
            else if (delta < 0)
            {
                // Need to remove room
                delta = (-1 * delta) & ~3;
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset + (uint)(old.Length + delta), (uint)(-1 * delta));
                endNull = true;
            }
            else if (delta == 0)
            {
                return;
            }
            for (int a = 0; a < value.Length; a++)
            {
                _owner.m_Overlay.Write8((uint)(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset + a),
                    (byte)value.ToCharArray()[a]);
            }
            if (endNull)
            {
                _owner.m_Overlay.Write8((uint)(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset + value.Length),
                    (byte)0);
            }
        }

        private void setNumFrames(uint value, int area)
        {
            _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(0).m_TexAnimHeaderOffset, value);
        }

        private void lbxScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxScale.SelectedIndex != -1)
                txtScale.Text = readScaleValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxScale.SelectedIndex).ToString(usa);
        }

        private void lbxRotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxRotation.SelectedIndex != -1)
                txtRotation.Text = readRotationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxRotation.SelectedIndex).ToString(usa);
        }

        private void lbxTranslation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxTranslation.SelectedIndex != -1)
                txtTranslation.Text = readTranslationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslation.SelectedIndex).ToString(usa);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            uint numAreas = _owner.m_Overlay.Read8(0x74);
            uint objlistptr = _owner.m_Overlay.ReadPointer(0x70);

            for (byte a = 0; a < _owner.m_NumAreas; a++)//For each area in current overlay
            {
                uint addr = (uint)(objlistptr + (a * 12));//Each level data header is 12 bytes - get the address of current one

                //Texture animation addresses have an offset of 4 bytes within each level data header
                addr += 4;
                if (_owner.m_Overlay.Read32(addr) != 0)//If current area's texture animation data pointer is not NULL
                {
                    _owner.m_Overlay.Write32(addr, 0);//Make it NULL
                }
            }
            reloadData();
            lbxScale.Items.Clear(); lbxRotation.Items.Clear(); lbxTranslation.Items.Clear();
            txtMaterialName.Text = ""; txtScale.Text = ""; txtRotation.Text = ""; txtTranslation.Text = ""; txtNumFrames.Text = "";
            // Above would still leave all the Pointers intact, potentially causing problems if the level isn't 
            // closed and reloaded afterwards before making any further texture animation changes
        }

        private void reloadData()
        {
            //Refresh data

            //The name of the last material goes straight into scale values for some levels, need to insert 4 NULLs to avoid corruption
            numAreas = _owner.m_Overlay.Read8(0x74);
            uint objlistptr = _owner.m_Overlay.ReadPointer(0x70);

            // Remove Pointers for existing texture animations
            // Note: for each area it's Object tables, Texture Animation tables
            for (int i = 0; i < numAreas; i++)
            {
                for (int j = 0; j < _owner.m_TexAnims[i].Count; j++)
                {
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_TexAnimHeaderOffset);
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_BaseScaleTblAddr);
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_BaseRotTblAddr);
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_BaseTransTblAddr);
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_Offset);
                    _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[i].ElementAt(j).m_MatNameOffset);
                }
            }

            _owner.m_TexAnims = new List<LevelTexAnim>[numAreas];
            for (int a = 0; a < numAreas; a++)
            {
                _owner.m_TexAnims[a] = new List<LevelTexAnim>();
            }

            for (byte a = 0; a < numAreas; a++)
            {
                uint addr = (uint)(objlistptr + (a * 12));

                // read texture animation
                addr += 4;
                if (_owner.m_Overlay.Read32(addr) != 0)
                {
                    _owner.AddPointer(addr);
                    _owner.ReadTextureAnimations(_owner.m_Overlay.ReadPointer(addr), a);
                }
            }

            //If the first byte after the last name is in the scale value table, make room for a null byte(s) and insert them, 
            // need to make sure scale table still starts on multiple of 4
            int[] lastNamePos = new int[2];
            //Find the last material name
            for (int a = 0; a < numAreas; a++)
            {
                for (int b = 0; b < _owner.m_TexAnims[a].Count; b++)
                {
                    lastNamePos[0] = a;
                    lastNamePos[1] = b;
                }
            }
            //Find the first scale values table
            int[] firstScalePos = new int[2];
            for (int a = 0; a < numAreas; a++)
            {
                if (_owner.m_TexAnims[a].Count >= 1)
                {
                    firstScalePos[0] = a;
                    firstScalePos[1] = 0;
                    break;
                }
            }
            
            if (!IsEmpty())
            {
                uint afterLastname = (uint)(_owner.m_TexAnims[lastNamePos[0]][lastNamePos[1]].m_MatNameOffset +
                    _owner.m_TexAnims[lastNamePos[0]][lastNamePos[1]].getMatName().Length);

                uint firstScaleStart = _owner.m_TexAnims[firstScalePos[0]][firstScalePos[1]].m_ScaleTblOffset;
                if (afterLastname == firstScaleStart)
                {
                    _owner.AddSpace((uint)(_owner.m_TexAnims[lastNamePos[0]][lastNamePos[1]].m_MatNameOffset +
                    _owner.m_TexAnims[lastNamePos[0]][lastNamePos[1]].getMatName().Length), 4);
                }
            }

            lbxArea.Items.Clear();
            lbxTexAnim.Items.Clear();
            for (int i = 0; i < _owner.m_NumAreas; i++)
            {
                lbxArea.Items.Add("" + i);
            }
            lbxArea.SelectedIndex = 0;//Make sure an area is selected
        }

        private bool IsEmpty()
        {
            bool empty = true;

            foreach (List<LevelTexAnim> list in _owner.m_TexAnims)
            {
                if (list.Count > 0)
                    return false;
            }

            return empty;
        }

        private void txtScale_TextChanged(object sender, EventArgs e)
        {
            if (txtScale.Text != "" && lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxScale.SelectedIndex != -1)
            {
                try
                {
                    float value = Convert.ToSingle(txtScale.Text);
                    setScaleValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxScale.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Scale value entered."); }
            }
        }

        private void txtRotation_TextChanged(object sender, EventArgs e)
        {
            if (txtRotation.Text != "" && lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxRotation.SelectedIndex != -1)
            {
                try
                {
                    float value = Convert.ToSingle(txtRotation.Text);
                    setRotationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxRotation.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Rotation value entered."); }
            }
        }

        private void txtTranslation_TextChanged(object sender, EventArgs e)
        {
            if (txtTranslation.Text != "" && lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxTranslation.SelectedIndex != -1)
            {
                try
                {
                    float value = Convert.ToSingle(txtTranslation.Text);
                    setTranslationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslation.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Translation value entered."); }
            }
        }

        private void btnSaveCurrent_Click(object sender, EventArgs e)
        {
            _owner.m_Overlay.SaveChanges();

            //reloadData();
        }

        private void txtMaterialName_TextChanged(object sender, EventArgs e)
        {
            if (txtMaterialName.Text != "")
                setMaterialName(txtMaterialName.Text, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
        }

        private void btnRemScale_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxScale.SelectedIndex != -1 && lbxScale.Items.Count > 1)
            {
                int index = lbxScale.SelectedIndex;
                removeScaleValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                refreshLbx();
                if (index > 0)
                    lbxScale.SelectedIndex = --index;
            }
        }

        private void btnAddScale_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1)
            {
                try
                {
                    int index = (lbxScale.SelectedIndex != -1) ? lbxScale.SelectedIndex + 1 : lbxScale.Items.Count;
                    float value = (txtScale.Text != "" && txtScale.Text != null) ? Convert.ToSingle(txtScale.Text) : 1f;

                    addScaleValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    refreshLbx();
                    lbxScale.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Scale value entered."); }
            }
        }

        private void btnRemRot_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxRotation.SelectedIndex != -1 && lbxRotation.Items.Count > 1)
            {
                int index = lbxRotation.SelectedIndex;
                removeRotationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                refreshLbx();
                if (index > 0)
                    lbxRotation.SelectedIndex = --index;
            }
        }

        private void btnAddRot_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1)
            {
                try
                {
                    int index = (lbxRotation.SelectedIndex != -1) ? lbxRotation.SelectedIndex + 1 : lbxRotation.Items.Count;
                    float value = (txtRotation.Text != "" && txtRotation.Text != null) ? Convert.ToSingle(txtRotation.Text) : 0f;

                    addRotationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    refreshLbx();
                    lbxRotation.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Rotation value entered."); }
            }
        }

        private void btnRemTrans_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && lbxTranslation.SelectedIndex != -1 && lbxTranslation.Items.Count > 1)
            {
                int index = lbxTranslation.SelectedIndex;
                removeTranslationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                refreshLbx();
                if (index > 0)
                    lbxTranslation.SelectedIndex = --index;
            }
        }

        private void btnAddTrans_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1)
            {
                try
                {
                    int index = (lbxTranslation.SelectedIndex != -1) ? lbxTranslation.SelectedIndex + 1 : lbxTranslation.Items.Count;
                    float value = (txtTranslation.Text != "" && txtTranslation.Text != null) ? Convert.ToSingle(txtTranslation.Text) : 1f;

                    addTranslationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    refreshLbx();
                    lbxTranslation.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Translation value entered."); }
            }
        }

        private void txtScaleStart_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtScaleStart.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtScaleStart.Text);
                    setScaleStart(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Scale Start Index entered."); }
            }
        }

        private void txtRotStart_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtRotStart.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtRotStart.Text);
                    setRotationStart(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Rotation Start Index entered."); }
            }
        }

        private void txtTransStart_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtTransStart.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtTransStart.Text);
                    setTranslationStart(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Translation Start Index entered."); }
            }
        }

        private void txtScaleSize_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtScaleSize.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtScaleSize.Text);
                    setScaleSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Scale Length entered."); }
            }
        }

        private void txtRotSize_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtRotSize.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtRotSize.Text);
                    setRotationSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Rotation Length entered."); }
            }
        }

        private void txtTransSize_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtTransSize.Text != "")
            {
                try
                {
                    ushort value = Convert.ToUInt16(txtTransSize.Text);
                    setTranslationSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid Translation Length entered."); }
            }
        }

        private void removeTextureAnimation(int area, int texAnim)
        {
            if (_owner.m_TexAnims[area].Count == 1)
            {
                // First, remove all pointers to texture animations for this area:
                // Addresss of Texture Animation header, addresses of scale, rotation and translation 
                // tables, address of the animation data, address of the material name

                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_TexAnimHeaderOffset);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseScaleTblAddr);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseRotTblAddr);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset);

                // Remove the space once occupied by the texture animation data:
                // Texture Animation header, Animation Data header, material name, scale, rotation and translation values
                // Order is important, remove in reverse order to listed above
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseTransTblAddr,
                    (uint)(_owner.m_TexAnims[area].ElementAt(texAnim).getTransTblSize() * 4));
                uint rotRemove = (uint)((_owner.m_TexAnims[area].ElementAt(texAnim).getRotTblSize() * 2) & ~3);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseRotTblAddr,
                    rotRemove);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_BaseScaleTblAddr,
                    (uint)(_owner.m_TexAnims[area].ElementAt(texAnim).getScaleTblSize() * 4));
                uint matNameRemove = (uint)((_owner.m_TexAnims[area].ElementAt(texAnim).getMatName().Length) & ~3);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset,
                    matNameRemove);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset, 28);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_TexAnimHeaderOffset, 24);
                // Set address of the Texture Animation Data within the Level Data header for this area 
                // to NULL
                uint objlistptr = _owner.m_Overlay.ReadPointer(0x70);
                uint addr = (uint)(objlistptr + (area * 12)) + 0x04;
                _owner.m_Overlay.Write32(addr, 0);
                _owner.m_TexAnims[area].RemoveAt(texAnim);
            }
            else if (_owner.m_TexAnims[area].Count > 1)
            {
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset);
                _owner.RemovePointerByPointerAddress(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset);

                uint matNameRemove = (uint)((_owner.m_TexAnims[area].ElementAt(texAnim).getMatName().Length) & ~3);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_MatNameOffset,
                    matNameRemove);
                _owner.RemoveSpace(_owner.m_TexAnims[area].ElementAt(texAnim).m_Offset, 28);

                // Reduce animation count
                _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim).m_TexAnimHeaderOffset + 0x10,
                    (uint)(_owner.m_TexAnims[area].Count - 1));
                _owner.m_TexAnims[area].RemoveAt(texAnim);
            }
            reloadData();
        }

        private void addTextureAnimation(int area, int texAnim)
        {
            if (_owner.m_TexAnims[area].Count == 0)
            {
                // No existing texture animations in area

                // New data will get written to end of overlay
                uint texAnimHeader = (uint)((_owner.m_Overlay.GetSize() + 3) & ~3);// multiple of 4
                uint animDataHeader = texAnimHeader + 24;
                uint matNameAddress = animDataHeader + 28;
                uint scaleTblAddress = matNameAddress + 16;
                uint rotTblAddress = scaleTblAddress + 4;
                uint transTblAddress = rotTblAddress + 4;// Rotation table should be a multiple of 4 bytes long

                _owner.AddSpace(_owner.m_Overlay.GetSize(), texAnimHeader - _owner.m_Overlay.GetSize());
                _owner.AddSpace(texAnimHeader, (uint)(24 + 28 + 16 + 4 + 4 + 4));

                // Point area level data to texture animation header
                uint objlistptr = _owner.m_Overlay.ReadPointer(0x70);
                uint addr = (uint)(objlistptr + (area * 12)) + 0x04;//Each level data header is 12 bytes - get the address of current one
                _owner.m_Overlay.WritePointer(addr, texAnimHeader);

                // Write Texture Animation header
                _owner.m_Overlay.Write32(texAnimHeader, (uint)100);// Number of frames
                _owner.m_Overlay.WritePointer(texAnimHeader + 0x04, scaleTblAddress);// Address of scale values table
                _owner.m_Overlay.WritePointer(texAnimHeader + 0x08, rotTblAddress);// Address of rotation values table
                _owner.m_Overlay.WritePointer(texAnimHeader + 0x0C, transTblAddress);// Address of translation value table
                _owner.m_Overlay.Write32(texAnimHeader + 0x10, (uint)1);// Number of animations
                _owner.m_Overlay.WritePointer(texAnimHeader + 0x14, animDataHeader);// Address of animation data header

                // Write Animation Data (header)
                _owner.m_Overlay.Write32(animDataHeader, (uint)0);// Unknown
                _owner.m_Overlay.WritePointer(animDataHeader + 0x04, matNameAddress);// Address of material name
                _owner.m_Overlay.Write32(animDataHeader + 0x08, (uint)1);// Default scale value? Set to 1
                _owner.m_Overlay.Write16(animDataHeader + 0x0C, (ushort)1);// Number of scale values
                _owner.m_Overlay.Write16(animDataHeader + 0x0E, (ushort)0);// Start index of scale values
                _owner.m_Overlay.Write16(animDataHeader + 0x10, (ushort)1);// Number of rotation values
                _owner.m_Overlay.Write16(animDataHeader + 0x12, (ushort)0);// Start index of rotation values
                _owner.m_Overlay.Write16(animDataHeader + 0x14, (ushort)1);// Number of translation values
                _owner.m_Overlay.Write16(animDataHeader + 0x16, (ushort)0);// Start index of translation values
                _owner.m_Overlay.Write32(animDataHeader + 0x18, (uint)0);// Unknown

                // Write material name
                string newMatName = "MaterialName";
                for (int i = 0; i < newMatName.Length; i++)
                {
                    _owner.m_Overlay.Write8((uint)(matNameAddress + i), (byte)(newMatName.ToCharArray()[i]));
                }
                // MaterialName length is 12, null byte(s) already written after material with AddSpace(.., 16)

                //Write scale, rotation and translation values
                _owner.m_Overlay.Write32(scaleTblAddress, (uint)4096);
                _owner.m_Overlay.Write16(rotTblAddress, (ushort)0);
                _owner.m_Overlay.Write32(transTblAddress, (uint)0);
            }
            else if (_owner.m_TexAnims[area].Count >= 1)
            {
                // Area already has texture animations

                // Make room for animation data header
                uint animDataHeader = _owner.m_TexAnims[area].ElementAt(texAnim - 1).m_Offset + 28;
                _owner.AddSpace(animDataHeader, 28);

                // Material name will get written after the existing ones and before the start of the scale values table
                uint matNameAddress = _owner.m_TexAnims[area].ElementAt(texAnim - 1).m_BaseScaleTblAddr;
                _owner.AddSpace(matNameAddress, 16);

                // Write material name
                string newMatName = "MaterialName";
                for (int i = 0; i < newMatName.Length; i++)
                {
                    _owner.m_Overlay.Write8((uint)(matNameAddress + i), (byte)(newMatName.ToCharArray()[i]));
                }
                // MaterialName length is 12, null byte(s) already written after material with AddSpace(.., 16)

                // Write Animation Data (header)
                _owner.m_Overlay.Write32(animDataHeader, (uint)0);// Unknown
                _owner.m_Overlay.WritePointer(animDataHeader + 0x04, matNameAddress);// Address of material name
                _owner.m_Overlay.Write32(animDataHeader + 0x08, (uint)1);// Default scale value? Set to 1
                _owner.m_Overlay.Write16(animDataHeader + 0x0C, (ushort)1);// Number of scale values
                _owner.m_Overlay.Write16(animDataHeader + 0x0E, (ushort)0);// Start index of scale values
                _owner.m_Overlay.Write16(animDataHeader + 0x10, (ushort)1);// Number of rotation values
                _owner.m_Overlay.Write16(animDataHeader + 0x12, (ushort)0);// Start index of rotation values
                _owner.m_Overlay.Write16(animDataHeader + 0x14, (ushort)1);// Number of translation values
                _owner.m_Overlay.Write16(animDataHeader + 0x16, (ushort)0);// Start index of translation values
                _owner.m_Overlay.Write32(animDataHeader + 0x18, (uint)0);// Unknown

                // Increase number of animations in Texture Animation header
                _owner.m_Overlay.Write32(_owner.m_TexAnims[area].ElementAt(texAnim - 1).m_TexAnimHeaderOffset + 0x10,
                    (uint)(texAnim + 1));
            }
            reloadData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && _owner.m_TexAnims[lbxArea.SelectedIndex].Count != 0)
            {
                removeTextureAnimation(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1)
            {
                int index = (lbxTexAnim.Items.Count > 0) ? lbxTexAnim.Items.Count : 0;
                addTextureAnimation(lbxArea.SelectedIndex, index);
            }
        }

        private void txtNumFrames_TextChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex != -1 && lbxTexAnim.SelectedIndex != -1 && txtNumFrames.Text != "")
            {
                try
                {
                    setNumFrames(Convert.ToUInt32(txtNumFrames.Text), lbxArea.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid number of frames entered"); }
            }
        }

        private void btnGenerateScale_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex == -1 || lbxTexAnim.SelectedIndex == -1)
                return;

            try
            {
                float startValue = float.Parse(txtGenerateScaleStartValue.Text, usa);
                float endValue = float.Parse(txtGenerateScaleEndValue.Text, usa);
                int amount = int.Parse(txtGenerateScaleAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    addScaleValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxScale.Items.Count);
                }

                refreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating scale values."); }
        }

        private void btnGenerateRotation_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex == -1 || lbxTexAnim.SelectedIndex == -1)
                return;

            try
            {
                float startValue = float.Parse(txtGenerateRotationStartValue.Text, usa);
                float endValue = float.Parse(txtGenerateRotationEndValue.Text, usa);
                int amount = int.Parse(txtGenerateRotationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    addRotationValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxRotation.Items.Count);
                }

                refreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating rotation values."); }
        }

        private void btnGenerateTranslation_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex == -1 || lbxTexAnim.SelectedIndex == -1)
                return;

            try
            {
                float startValue = float.Parse(txtGenerateTranslationStartValue.Text, usa);
                float endValue = float.Parse(txtGenerateTranslationEndValue.Text, usa);
                int amount = int.Parse(txtGenerateTranslationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    addTranslationValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxTranslation.Items.Count);
                }

                refreshLbx();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n\n" + ex.Source + "\n\n" + ex.StackTrace); }
        }

    }
}
