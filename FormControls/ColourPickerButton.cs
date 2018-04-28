using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SM64DSe.FormControls
{
    public class ColourPickerButton : Button
    {
        public Color? Colour 
        {
            get
            {
                return m_Colour;
            }
            set
            {
                if (value != null)
                {
                    setColourButtonValue((Color)value);
                }
                else
                {
                    m_Colour = value;
                }
            }
        }
        private Color? m_Colour;

        public ColourPickerButton() : base() 
        {
            this.UseVisualStyleBackColor = true;
            ResetColourButtonValue();
        }

        public void ResetColourButtonValue()
        {
            this.Text = null;
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.Black;
        }

        public Color? SelectColour()
        {
            Color? colour = getColourDialogueResult();
            if (colour.HasValue)
            {
                Colour = colour;
            }
            return colour;
        }

        private Color? getColourDialogueResult()
        {
            ColorDialog colourDialogue = new ColorDialog();
            DialogResult result = colourDialogue.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                Color colour = colourDialogue.Color;
                setColourButtonValue(colour);
                return colour;
            }
            return null;
        }

        private void setColourButtonValue(Color colour)
        {
            string hexColourString = Helper.GetHexColourString(colour);
            this.Text = hexColourString;
            this.BackColor = colour;
            float luma = 0.2126f * colour.R + 0.7152f * colour.G + 0.0722f * colour.B;
            if (luma < 50)
            {
                this.ForeColor = Color.White;
            }
            else
            {
                this.ForeColor = Color.Black;
            }
            m_Colour = colour;
        }
    }
}
