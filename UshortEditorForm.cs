using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class RawEditorForm : Form
    {

        LevelEditorForm m_parent;
        bool m_displayInBinary;

        RawUshortEdit m_parameter1Ctrl;
        RawUshortEdit m_parameter2Ctrl;
        RawUshortEdit m_parameter3Ctrl;
        RawUshortEdit m_parameter4Ctrl;
        RawUshortEdit m_parameter5Ctrl;
        Label m_NothingCtrl;

        public RawEditorForm(LevelEditorForm levelEditor)
        {
            m_parent = levelEditor;
            m_displayInBinary = false;
            InitializeComponent();

            //parameter 1
            m_parameter1Ctrl = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
            m_parameter1Ctrl.MouseDown += DeselectOthers;
            m_parameter1Ctrl.Tag = "Parameter 1";

            //parameter 2
            m_parameter2Ctrl = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
            m_parameter2Ctrl.MouseDown += DeselectOthers;
            m_parameter2Ctrl.Tag = "Parameter 2";

            //parameter 3
            m_parameter3Ctrl = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
            m_parameter3Ctrl.MouseDown += DeselectOthers;
            m_parameter3Ctrl.Tag = "Parameter 3";

            //parameter 4
            m_parameter4Ctrl = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
            m_parameter4Ctrl.MouseDown += DeselectOthers;
            m_parameter4Ctrl.Tag = "Parameter 2";

            //parameter 5
            m_parameter5Ctrl = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
            m_parameter5Ctrl.MouseDown += DeselectOthers;
            m_parameter5Ctrl.Tag = "Parameter 3";

            //Noting
            m_NothingCtrl = new Label() { Text = "No Raw Editing available for this Object", Width = 300 };
        }

        public void ShowForObject(LevelObject obj)
        {
            if (!Visible)
                Show(m_parent);
            else
                Activate();
            UpdateForObject(obj);
        }

        public void UpdateParameter(string name, ushort value)
        {
            foreach (Control ctrl in panControls.Controls)
            {
                if ((ctrl is RawUshortEdit) && ((string)ctrl.Tag == name))
                    ((RawUshortEdit)ctrl).SetValue(value);
            }
        }
        public void UpdateForObject(LevelObject obj)
        {
            Console.WriteLine("Refresh GUI");
            panControls.Controls.Clear();

            int nextSnapY = 7;
            if (obj is StandardObject)
            {
                //Parameter 1
                panControls.Controls.Add(m_parameter1Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter1Ctrl, nextSnapY) + 7;
                m_parameter1Ctrl.SetValue(obj.Parameters[0]);

                //Parameter 2
                panControls.Controls.Add(m_parameter2Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter2Ctrl, nextSnapY) + 7;
                m_parameter2Ctrl.SetValue(obj.Parameters[1]);

                //Parameter 3
                panControls.Controls.Add(m_parameter3Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter3Ctrl, nextSnapY) + 7;
                m_parameter3Ctrl.SetValue(obj.Parameters[2]);
            }
            else if (obj is SimpleObject)
            {
                //Parameter 1
                panControls.Controls.Add(m_parameter1Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter1Ctrl, nextSnapY) + 7;
                m_parameter1Ctrl.SetValue(obj.Parameters[0]);
            }
            else if (obj is PathObject)
            {
                //Parameter 1
                panControls.Controls.Add(m_parameter3Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter3Ctrl, nextSnapY) + 7;
                m_parameter3Ctrl.SetValue(obj.Parameters[2]);

                //Parameter 2
                panControls.Controls.Add(m_parameter4Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter4Ctrl, nextSnapY) + 7;
                m_parameter4Ctrl.SetValue(obj.Parameters[3]);

                //Parameter 3
                panControls.Controls.Add(m_parameter5Ctrl);
                nextSnapY = Helper.snapControlVertically(m_parameter5Ctrl, nextSnapY) + 7;
                m_parameter5Ctrl.SetValue(obj.Parameters[4]);
            }
            else
            {
                
                panControls.Controls.Add(m_NothingCtrl);
                Helper.snapControlVertically(m_NothingCtrl, nextSnapY);
            }
        }

        private void DeselectOthers(object sender, MouseEventArgs e)
        {
            foreach (Control ctrl in panControls.Controls)
            {
                if ((ctrl is RawUshortEdit) && (ctrl != sender))
                    ((RawUshortEdit)ctrl).Deselect();
            }
        }

        private void RawEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_parent.m_rawEditor = null;
        }
        
        private void btnToogleBinary_Click(object sender, EventArgs e)
        {
            m_displayInBinary = !m_displayInBinary;
            foreach (Control ctrl in panControls.Controls)
            {
                if (ctrl is RawUshortEdit)
                    ((RawUshortEdit)ctrl).SetBinary(m_displayInBinary);
            }
            ((Button)sender).Text = "Display in " + (m_displayInBinary ? "Hex" : "Binary");
        }

        private void SendValueToLevelForm(RawUshortEdit sender)
        {
            m_parent.SetProperty((string)sender.Tag, sender.GetValue());
            m_parent.initializePropertyInterface();
        }

        private void btnToogleHex_Click(object sender, EventArgs e)
        {
            if (valueInput.Hexadecimal)
            {
                valueInput.Hexadecimal = false;
                btnToogleHex.Text = "Dec";
            }
            else
            {
                valueInput.Hexadecimal = true;
                btnToogleHex.Text = "Hex";
            }
        }
    }

    public partial class RawUshortEdit : UserControl
    {
        int m_fieldWidth;
        ushort m_valueToEdit;
        NumericUpDown m_valueInput;
        String m_stringRepresentation;
        int m_selectionStartIndex;
        int m_selectionEndIndex;
        bool m_inBinary;
        bool m_settingValues;

        public delegate void OnValueChanged(RawUshortEdit sender);

        public OnValueChanged ValueChanged = null;

        public RawUshortEdit(ushort value, NumericUpDown valueInput, bool displayBinary = false)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.UserPaint | 
                ControlStyles.OptimizedDoubleBuffer, 
                true);
            Console.WriteLine("Created");
            m_valueInput = valueInput;
            m_valueInput.ValueChanged += M_valueInput_ValueChanged;
            m_inBinary = displayBinary;
            m_selectionStartIndex = -1;
            m_selectionEndIndex = 0;
            SetValue(value);
        }

        private void M_valueInput_ValueChanged(object sender, EventArgs e)
        {
            InsertValue((ushort)((NumericUpDown)sender).Value);
            ValueChanged?.Invoke(this);
        }

        public void Deselect()
        {
            m_selectionStartIndex = -1;
            Refresh();
        }

        public bool isSelected()
        {
            return (m_selectionStartIndex != -1);
        }

        public bool InsertValue(ushort value)
        {
            if ((m_selectionStartIndex == -1)|| m_settingValues)
                return false;
            int selectionStart = m_selectionStartIndex;
            int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
            string stringValue = Convert.ToString(value, m_inBinary?2:16).PadLeft(selectionLength,'0');
            if (stringValue.Length<= selectionLength)
            {
                string stringWithGap = m_stringRepresentation.Remove(selectionStart, selectionLength);
                m_stringRepresentation = stringWithGap.Insert(selectionStart,stringValue);
                Refresh();
                RefreshValue();
                return true;
            }
            return false;
        }

        public ushort GetValue()
        {
            return m_valueToEdit;
        }

        public void SetValue(ushort value)
        {
            m_valueToEdit = value;
            if (m_inBinary)
            {
                Width = 320;
                Height = 20;
                m_fieldWidth = 20;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 2).PadLeft(16, '0');
                if (m_selectionStartIndex != -1)
                {
                    int selectionStart = m_selectionStartIndex;
                    int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
                    m_settingValues = true; //prevent the valueChanged event

                    m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), 2);

                    m_settingValues = false;
                }
            }
            else
            {
                Width = 160;
                Height = 20;
                m_fieldWidth = 40;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 16).PadLeft(4, '0');
                if (m_selectionStartIndex != -1)
                {
                    int selectionStart = m_selectionStartIndex;
                    int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
                    m_settingValues = true; //prevent the valueChanged event
                    
                    m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), 16);

                    m_settingValues = false;
                }
            }
            Refresh();
        }

        public void SetBinary(bool value)
        {
            Console.WriteLine("Set Binary");
            if (value == m_inBinary)
                return;
            if (value)
            {
                Width = 320;
                Height = 20;
                m_inBinary = true;
                m_fieldWidth = 20;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 2).PadLeft(16, '0');
                if (m_selectionStartIndex != -1)
                {
                    m_selectionStartIndex *= 4;
                    m_selectionEndIndex *= 4;
                    m_selectionEndIndex += 3;
                }
            }
            else
            {
                Width = 160;
                Height = 20;
                m_inBinary = false;
                m_fieldWidth = 40;
                m_stringRepresentation = Convert.ToString(m_valueToEdit, 16).PadLeft(4, '0');
                if (m_selectionStartIndex != -1)
                {
                    m_selectionStartIndex /= 4;
                    m_selectionEndIndex /= 4;

                    int selectionStart = m_selectionStartIndex;
                    int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
                    m_settingValues = true; //prevent the valueChanged event

                    m_valueInput.Maximum = (ushort)(Math.Pow(m_inBinary ? 2 : 16, selectionLength) - 1);
                    m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), 16);

                    m_settingValues = false;
                }
            }
            Refresh();
        }

        public void RefreshValue()
        {
            if (m_inBinary)
            {
                m_valueToEdit = Convert.ToUInt16(m_stringRepresentation, 2);
            }
            else
            {
                m_valueToEdit = Convert.ToUInt16(m_stringRepresentation, 16);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color backColor = SystemColors.ButtonShadow;
            Brush selBackColor = new SolidBrush(SystemColors.Highlight);
            Brush fieldColor = new SolidBrush(SystemColors.ButtonFace);
            Brush selFieldColor = new SolidBrush(SystemColors.GradientActiveCaption);
            Brush textColor = new SolidBrush(SystemColors.ControlText);
            Font textFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

            int selectionStart;
            int selectionLength;
            if (m_selectionStartIndex <= m_selectionEndIndex)
            {
                selectionStart = m_selectionStartIndex;
                selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;
            }
            else
            {
                selectionStart = m_selectionEndIndex;
                selectionLength = (m_selectionStartIndex - m_selectionEndIndex) + 1;
            }

            e.Graphics.Clear(backColor);

            if (m_selectionStartIndex!=-1)
            {
                Rectangle selectionRect = new Rectangle(
                    selectionStart * m_fieldWidth, 0,
                    selectionLength * m_fieldWidth, Height
                );
                e.Graphics.FillRectangle(selBackColor, selectionRect);
            }
            for (int i = 0; i < (m_inBinary?16:4); i++)
            {
                Rectangle fieldRect = new Rectangle(1 + i*m_fieldWidth, 1, m_fieldWidth - 2, Height - 2);
                if ((i>= selectionStart) && (i< selectionStart + selectionLength) && (m_selectionStartIndex != -1))
                    e.Graphics.FillRectangle(selFieldColor, fieldRect);
                else
                    e.Graphics.FillRectangle(fieldColor, fieldRect);
                if ( m_inBinary && (Math.Floor(i/4.0)%2==0) )
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 100, 100, 50)), fieldRect);
                e.Graphics.DrawString(m_stringRepresentation[i].ToString(), textFont, textColor, fieldRect.Left+(m_inBinary?1:3),fieldRect.Top+2);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_selectionStartIndex = (int)Math.Floor(e.X /(float)m_fieldWidth);
            m_selectionEndIndex = m_selectionStartIndex;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                m_selectionEndIndex = Math.Min(Math.Max(0,(int)Math.Floor(e.X / (float)m_fieldWidth)), m_inBinary ? 15 : 3);
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                if(m_selectionStartIndex > m_selectionEndIndex)
                {
                    int startIndexBefore = m_selectionStartIndex;
                    m_selectionStartIndex = m_selectionEndIndex;
                    m_selectionEndIndex = startIndexBefore;
                }

                int selectionStart = m_selectionStartIndex;
                int selectionLength = (m_selectionEndIndex - m_selectionStartIndex) + 1;

                m_settingValues = true; //prevent the valueChanged event

                m_valueInput.Maximum = (ushort)(Math.Pow(m_inBinary ? 2 : 16, selectionLength)-1);
                m_valueInput.Value = Convert.ToUInt16(m_stringRepresentation.Substring(selectionStart, selectionLength), m_inBinary ? 2 : 16);

                m_settingValues = false;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //TODO implement a Direct Digit Writing
        }
    }
}
