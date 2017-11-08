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
        protected Level m_Level;

        public TextureAnimationForm(Level level)
        {
            InitializeComponent();
            
            m_Level = level;

            for (int i = 0; i < m_Level.m_NumAreas; i++)
            {
                lbxArea.Items.Add(i.ToString());
            }
            lbxArea.SelectedIndex = 0;//Make sure an area is selected

            ReloadData();
        }

        private void lbxTexAnim_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxTexAnim.SelectedIndex > -1)
                RefreshLbx();
        }

        private void RefreshLbx()
        {
            if (lbxArea.SelectedIndex < 0) return;
            LevelTexAnim texAnim = m_Level.m_TexAnims[lbxArea.SelectedIndex];

            if (lbxTexAnim.SelectedIndex < 0)
            {
                ClearFormFields();
                return;
            }
            LevelTexAnim.Def texAnimDef = texAnim.m_Defs[lbxTexAnim.SelectedIndex];

            txtMaterialName.Text = texAnimDef.m_MaterialName;
            txtNumFrames.Text = texAnim.m_NumFrames.ToString();
            lbxScaleValues.Items.Clear();
            for (int i = 0; i < texAnimDef.m_ScaleValues.Count; i++)
            {
                lbxScaleValues.Items.Add("Scale value " + i.ToString("D4"));
            }
            lbxRotationValues.Items.Clear();
            for (int i = 0; i < texAnimDef.m_RotationValues.Count; i++)
            {
                lbxRotationValues.Items.Add("Rotation value " + i.ToString("D4"));
            }
            lbxTranslationXValues.Items.Clear();
            for (int i = 0; i < texAnimDef.m_TranslationXValues.Count; i++)
            {
                lbxTranslationXValues.Items.Add("Translation X value " + i.ToString("D4"));
            }
            lbxTranslationYValues.Items.Clear();
            for (int i = 0; i < texAnimDef.m_TranslationYValues.Count; i++)
            {
                lbxTranslationYValues.Items.Add("Translation Y value " + i.ToString("D4"));
            }
            txtScaleLength.Text = texAnimDef.m_NumScaleValues.ToString();
            txtRotationLength.Text = texAnimDef.m_NumRotationValues.ToString();
            txtTranslationXLength.Text = texAnimDef.m_NumTranslationXValues.ToString();
            txtTranslationYLength.Text = texAnimDef.m_NumTranslationYValues.ToString();
        }

        private void ClearFormFields()
        {
            txtMaterialName.Text = null;
            txtNumFrames.Text = null;

            lbxScaleValues.Items.Clear();
            lbxRotationValues.Items.Clear();
            lbxTranslationXValues.Items.Clear();
            lbxTranslationYValues.Items.Clear();

            txtScaleLength.Text = null;
            txtRotationLength.Text = null;
            txtTranslationXLength.Text = null;
            txtTranslationXLength.Text = null;

            txtScaleValue.Text = null;
            txtRotationValue.Text = null;
            txtTranslationXValue.Text = null;
            txtTranslationYValue.Text = null;

            txtScaleGenerationStartValue.Text = null;
            txtScaleGenerationEndValue.Text = null;
            txtScaleGenerationAmount.Text = null;
            txtRotationGenerationStartValue.Text = null;
            txtRotationGenerationEndValue.Text = null;
            txtRotationGenerationAmount.Text = null;
            txtTranslationXGenerationStartValue.Text = null;
            txtTranslationXGenerationEndValue.Text = null;
            txtTranslationXGenerationAmount.Text = null;
            txtTranslationYGenerationStartValue.Text = null;
            txtTranslationYGenerationEndValue.Text = null;
            txtTranslationYGenerationAmount.Text = null;
        }

        private void lbxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex > -1)
            {
                lbxTexAnim.Items.Clear();
                for (int j = 0; j < m_Level.m_TexAnims[lbxArea.SelectedIndex].m_Defs.Count; j++)
                {
                    lbxTexAnim.Items.Add(j.ToString());
                }
            }
        }

        private float ReadScaleValue(int area, int texAnim, int index)
        {
            return m_Level.m_TexAnims[area].m_Defs[texAnim].m_ScaleValues[index];
        }

        private float ReadRotationValue(int area, int texAnim, int index)
        {
            return m_Level.m_TexAnims[area].m_Defs[texAnim].m_RotationValues[index];
        }

        private float ReadTranslationXValue(int area, int texAnim, int index)
        {
            return m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationXValues[index];
        }

        private float ReadTranslationYValue(int area, int texAnim, int index)
        {
            return m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationYValues[index];
        }

        private void SetScaleValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_ScaleValues[index] = value;
        }

        private void SetRotationValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_RotationValues[index] = value;
        }

        private void SetTranslationXValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationXValues[index] = value;
        }

        private void SetTranslationYValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationYValues[index] = value;
        }

        private void AddScaleValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_ScaleValues.Insert(index, value);
        }

        private void AddRotationValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_RotationValues.Insert(index, value);
        }

        private void AddTranslationXValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationXValues.Insert(index, value);
        }

        private void AddTranslationYValue(float value, int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationYValues.Insert(index, value);
        }

        private void SetScaleSize(ushort value, int area, int texAnim)
        {
            Helper.ResizeList(m_Level.m_TexAnims[area].m_Defs[texAnim].m_ScaleValues, value, 1.0f);
        }

        private void SetRotationSize(ushort value, int area, int texAnim)
        {
            Helper.ResizeList(m_Level.m_TexAnims[area].m_Defs[texAnim].m_RotationValues, value, 0.0f);
        }

        private void SetTranslationXSize(ushort value, int area, int texAnim)
        {
            Helper.ResizeList(m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationXValues, value, 0.0f);
        }

        private void SetTranslationYSize(ushort value, int area, int texAnim)
        {
            Helper.ResizeList(m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationYValues, value, 0.0f);
        }

        private void RemoveScaleValue(int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_ScaleValues.RemoveAt(index);
        }

        private void RemoveRotationValue(int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_RotationValues.RemoveAt(index);
        }

        private void RemoveTranslationXValue(int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationXValues.RemoveAt(index);
        }

        private void RemoveTranslationYValue(int area, int texAnim, int index)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_TranslationYValues.RemoveAt(index);
        }

        private void SetMaterialName(string value, int area, int texAnim)
        {
            m_Level.m_TexAnims[area].m_Defs[texAnim].m_MaterialName = value;
        }

        private void SetNumFrames(uint value, int area)
        {
            m_Level.m_TexAnims[area].m_NumFrames = value;
        }

        private void lbxScaleValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxScaleValues.SelectedIndex > -1)
                txtScaleValue.Text = Helper.ToString(ReadScaleValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxScaleValues.SelectedIndex));
        }

        private void lbxRotationValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxRotationValues.SelectedIndex > -1)
                txtRotationValue.Text = Helper.ToString(ReadRotationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxRotationValues.SelectedIndex));
        }

        private void lbxTranslationXValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxTranslationXValues.SelectedIndex > -1)
                txtTranslationXValue.Text = Helper.ToString(ReadTranslationXValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslationXValues.SelectedIndex));
        }

        private void lbxTranslationYValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxTranslationYValues.SelectedIndex > -1)
                txtTranslationYValue.Text = Helper.ToString(ReadTranslationYValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslationYValues.SelectedIndex));
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 8; ++i)
            {
                m_Level.m_TexAnims[i].m_NumFrames = 0;
                m_Level.m_TexAnims[i].m_Defs.Clear();
            }
            ReloadData();
        }

        private void ReloadData()
        {
            lbxArea.Items.Clear();
            lbxTexAnim.Items.Clear();
            for (int i = 0; i < m_Level.m_NumAreas; i++)
            {
                lbxArea.Items.Add(i.ToString());
            }
            lbxArea.SelectedIndex = 0;//Make sure an area is selected
            lbxTexAnim.SelectedIndex = -1;
            RefreshLbx();
        }

        private bool IsEmpty()
        {
            foreach (LevelTexAnim anim in m_Level.m_TexAnims)
            {
                if (anim.m_Defs.Count > 0)
                    return false;
            }
            return true;
        }

        private bool IsTextureAnimationSelected()
        {
            return (lbxArea.SelectedIndex > -1 && lbxTexAnim.SelectedIndex > -1);
        }

        private void txtScaleValue_TextChanged(object sender, EventArgs e)
        {
            if (Strings.IsNotBlank(txtScaleValue.Text) && IsTextureAnimationSelected() && lbxScaleValues.SelectedIndex > -1)
            {
                float value;
                if (float.TryParse(txtScaleValue.Text, out value))
                {
                    SetScaleValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxScaleValues.SelectedIndex);
                }
            }
        }

        private void txtRotationValue_TextChanged(object sender, EventArgs e)
        {
            if (Strings.IsNotBlank(txtRotationValue.Text) && IsTextureAnimationSelected() && lbxRotationValues.SelectedIndex > -1)
            {
                float value;
                if (float.TryParse(txtRotationValue.Text, out value))
                {
                    SetRotationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxRotationValues.SelectedIndex);
                }
            }
        }

        private void txtTranslationXValue_TextChanged(object sender, EventArgs e)
        {
            if (Strings.IsNotBlank(txtTranslationXValue.Text) && IsTextureAnimationSelected() && lbxTranslationXValues.SelectedIndex > -1)
            {
                float value;
                if (float.TryParse(txtTranslationXValue.Text, out value))
                {
                    SetTranslationXValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslationXValues.SelectedIndex);
                }
            }
        }

        private void txtTranslationYValue_TextChanged(object sender, EventArgs e)
        {
            if (Strings.IsNotBlank(txtTranslationYValue.Text) && IsTextureAnimationSelected() && lbxTranslationYValues.SelectedIndex > -1)
            {
                float value;
                if (float.TryParse(txtTranslationYValue.Text, out value))
                {
                    SetTranslationYValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, lbxTranslationYValues.SelectedIndex);
                }
            }
        }

        private void txtMaterialName_TextChanged(object sender, EventArgs e)
        {
            if (Strings.IsNotBlank(txtMaterialName.Text))
                SetMaterialName(txtMaterialName.Text, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
        }

        private void btnScaleRemove_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxScaleValues.SelectedIndex > -1 && lbxScaleValues.Items.Count > 1)
            {
                int index = lbxScaleValues.SelectedIndex;
                RemoveScaleValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                RefreshLbx();
                if (index > 0) lbxScaleValues.SelectedIndex = index - 1;
            }
        }

        private void btnScaleAdd_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected())
            {
                try
                {
                    int index = (lbxScaleValues.SelectedIndex > -1) ? lbxScaleValues.SelectedIndex + 1 : lbxScaleValues.Items.Count;
                    float value = (Strings.IsNotBlank(txtScaleValue.Text)) ? Helper.ParseFloat(txtScaleValue.Text) : 1f;

                    AddScaleValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    RefreshLbx();
                    lbxScaleValues.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Scale value entered."); }
            }
        }

        private void btnRotationRemove_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxRotationValues.SelectedIndex > -1 && lbxRotationValues.Items.Count > 1)
            {
                int index = lbxRotationValues.SelectedIndex;
                RemoveRotationValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                RefreshLbx();
                if (index > 0) lbxRotationValues.SelectedIndex = index - 1;
            }
        }

        private void btnRotationAdd_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected())
            {
                try
                {
                    int index = (lbxRotationValues.SelectedIndex > -1) ? lbxRotationValues.SelectedIndex + 1 : lbxRotationValues.Items.Count;
                    float value = (Strings.IsNotBlank(txtRotationValue.Text)) ? Helper.ParseFloat(txtRotationValue.Text) : 0f;

                    AddRotationValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    RefreshLbx();
                    lbxRotationValues.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Rotation value entered."); }
            }
        }

        private void btnTranslationXRemove_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxTranslationXValues.SelectedIndex > -1 && lbxTranslationXValues.Items.Count > 1)
            {
                int index = lbxTranslationXValues.SelectedIndex;
                RemoveTranslationXValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                RefreshLbx();
                if (index > 0) lbxTranslationXValues.SelectedIndex = index - 1;
            }
        }

        private void btnTranslationXAdd_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected())
            {
                try
                {
                    int index = (lbxTranslationXValues.SelectedIndex > -1) ? lbxTranslationXValues.SelectedIndex + 1 : lbxTranslationXValues.Items.Count;
                    float value = (Strings.IsNotBlank(txtTranslationXValue.Text)) ? Helper.ParseFloat(txtTranslationXValue.Text) : 1f;
                    
                    AddTranslationXValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    RefreshLbx();
                    lbxTranslationXValues.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Translation X value entered."); }
            }
        }

        private void btnTranslationYRemove_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && lbxTranslationYValues.SelectedIndex > -1 && lbxTranslationYValues.Items.Count > 1)
            {
                int index = lbxTranslationYValues.SelectedIndex;
                RemoveTranslationYValue(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                RefreshLbx();
                if (index > 0) lbxTranslationYValues.SelectedIndex = index - 1;
            }
        }

        private void btnTranslationYAdd_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected())
            {
                try
                {
                    int index = (lbxTranslationYValues.SelectedIndex > -1) ? lbxTranslationYValues.SelectedIndex + 1 : lbxTranslationYValues.Items.Count;
                    float value = (Strings.IsNotBlank(txtTranslationYValue.Text)) ? Helper.ParseFloat(txtTranslationYValue.Text) : 1f;

                    AddTranslationYValue(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex, index);
                    RefreshLbx();
                    lbxTranslationYValues.SelectedIndex = index;
                }
                catch { MessageBox.Show("Invalid Translation Y value entered."); }
            }
        }

        private void RemoveTextureAnimation(int area, int texAnim)
        {
            m_Level.m_TexAnims[area].m_Defs.RemoveAt(texAnim);
            ReloadData();
        }

        private void AddTextureAnimation(int area, int texAnim)
        {
            m_Level.m_TexAnims[area].m_Defs.Insert(texAnim, new LevelTexAnim.Def());
            ReloadData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && m_Level.m_TexAnims[lbxArea.SelectedIndex].m_Defs.Count != 0)
            {
                RemoveTextureAnimation(lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lbxArea.SelectedIndex > -1)
            {
                int index = (lbxTexAnim.Items.Count > 0) ? lbxTexAnim.Items.Count : 0;
                AddTextureAnimation(lbxArea.SelectedIndex, index);
            }
        }

        private void txtNumFrames_TextChanged(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && txtNumFrames.Text != "")
            {
                try
                {
                    SetNumFrames(Convert.ToUInt32(txtNumFrames.Text), lbxArea.SelectedIndex);
                }
                catch { MessageBox.Show("Invalid number of frames entered"); }
            }
        }

        private void btnScaleGenerate_Click(object sender, EventArgs e)
        {
            if (!IsTextureAnimationSelected()) return;

            try
            {
                float startValue = Helper.ParseFloat(txtScaleGenerationStartValue.Text);
                float endValue = Helper.ParseFloat(txtScaleGenerationEndValue.Text);
                int amount = int.Parse(txtScaleGenerationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    AddScaleValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxScaleValues.Items.Count);
                }

                RefreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating scale values."); }
        }

        private void btnRotationGenerate_Click(object sender, EventArgs e)
        {
            if (!IsTextureAnimationSelected()) return;

            try
            {
                float startValue = Helper.ParseFloat(txtRotationGenerationStartValue.Text);
                float endValue = Helper.ParseFloat(txtRotationGenerationEndValue.Text);
                int amount = int.Parse(txtRotationGenerationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    AddRotationValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxRotationValues.Items.Count);
                }

                RefreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating rotation values."); }
        }

        private void btnTranslationXGenerate_Click(object sender, EventArgs e)
        {
            if (!IsTextureAnimationSelected()) return;

            try
            {
                float startValue = Helper.ParseFloat(txtTranslationXGenerationStartValue.Text);
                float endValue = Helper.ParseFloat(txtTranslationXGenerationEndValue.Text);
                int amount = int.Parse(txtTranslationXGenerationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    AddTranslationXValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxTranslationXValues.Items.Count);
                }

                RefreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating translation X values."); }
        }

        private void btnTranslationYGenerate_Click(object sender, EventArgs e)
        {
            if (!IsTextureAnimationSelected()) return;

            try
            {
                float startValue = Helper.ParseFloat(txtTranslationYGenerationStartValue.Text);
                float endValue = Helper.ParseFloat(txtTranslationYGenerationEndValue.Text);
                int amount = int.Parse(txtTranslationYGenerationAmount.Text);
                float increment = (float)((endValue - startValue) / (float)amount);

                for (int i = 0; i <= amount; i++)
                {
                    AddTranslationYValue((startValue + ((float)(amount - i) * increment)), lbxArea.SelectedIndex,
                        lbxTexAnim.SelectedIndex, lbxTranslationYValues.Items.Count);
                }

                RefreshLbx();
            }
            catch { MessageBox.Show("Invalid value(s) entered for generating translation Y values."); }
        }

        private void btnScaleLengthUpdate_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && Strings.IsNotBlank(txtScaleLength.Text))
            {
                ushort value;
                if (ushort.TryParse(txtScaleLength.Text, out value))
                {
                    SetScaleSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Invalid Scale Length entered.");
                    return;
                }
            }
        }

        private void btnRotationLengthUpdate_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && Strings.IsNotBlank(txtRotationLength.Text))
            {
                ushort value;
                if (ushort.TryParse(txtRotationLength.Text, out value))
                {
                    SetRotationSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Invalid Rotation Length entered.");
                    return;
                }
            }
        }

        private void btnTranslationXLengthUpdate_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && Strings.IsNotBlank(txtTranslationXLength.Text))
            {
                ushort value;
                if (ushort.TryParse(txtTranslationXLength.Text, out value))
                {
                    SetTranslationXSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Invalid X Translation Length entered.");
                    return;
                }
            }
        }

        private void btnTranslationYLengthUpdate_Click(object sender, EventArgs e)
        {
            if (IsTextureAnimationSelected() && Strings.IsNotBlank(txtTranslationYLength.Text))
            {
                ushort value;
                if (ushort.TryParse(txtTranslationYLength.Text, out value))
                {
                    SetTranslationYSize(value, lbxArea.SelectedIndex, lbxTexAnim.SelectedIndex);
                }
                else
                {
                    MessageBox.Show("Invalid Y Translation Length entered.");
                    return;
                }
            }
        }
    }
}
