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
        private LevelEditorForm m_parent;
        private bool m_displayInBinary;

        private RawUshortEdit[] m_ParameterControls = new RawUshortEdit[5];

        private Label m_NothingCtrl;

        public RawEditorForm(LevelEditorForm levelEditor)
        {
            m_parent = levelEditor;
            m_displayInBinary = false;
            InitializeComponent();

            for (int i = 0; i < m_ParameterControls.Length; i++)
            {
                m_ParameterControls[i] = new RawUshortEdit(0, valueInput, m_displayInBinary) { ValueChanged = SendValueToLevelForm };
                m_ParameterControls[i].MouseDown += DeselectOthers;
                m_ParameterControls[i].Tag = "Parameter " + i;
            }

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
            panControls.Controls.Clear();

            int nextSnapY = 7;

            int nParameters;
            int firstParameterIndex;
            switch (obj.m_Type)
            {
                case LevelObject.Type.STANDARD:
                    nParameters = 3;
                    firstParameterIndex = 0;
                    break;
                case LevelObject.Type.SIMPLE:
                    nParameters = 1;
                    firstParameterIndex = 0;
                    break;
                case LevelObject.Type.PATH:
                    nParameters = 3;
                    firstParameterIndex = 2;
                    break;
                default:
                    nParameters = -1;
                    firstParameterIndex = -1;
                    break;
            }

            if (nParameters > -1)
            {
                for (int i = 0; i < nParameters; i++)
                {
                    RawUshortEdit control = m_ParameterControls[i + firstParameterIndex];
                    panControls.Controls.Add(control);
                    nextSnapY = Helper.snapControlVertically(control, nextSnapY) + 7;
                    control.SetValue(obj.Parameters[i + firstParameterIndex]);
                }
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
            m_parent.InitializePropertyInterface();
        }

        private void btnToggleHex_Click(object sender, EventArgs e)
        {
            if (valueInput.Hexadecimal)
            {
                valueInput.Hexadecimal = false;
                btnToggleHex.Text = "Hex";
            }
            else
            {
                valueInput.Hexadecimal = true;
                btnToggleHex.Text = "Dec";
            }
        }
    }

    public partial class RawUshortEdit : UserControl
    {
        int m_FieldWidth;
        ushort m_ValueToEdit;
        NumericUpDown m_ValueInput;
        string m_StringRepresentation;
        int m_SelectionStartIndex;
        int m_SelectionEndIndex;
        bool m_InBinary;
        bool m_SettingValues;

        public delegate void OnValueChanged(RawUshortEdit sender);

        public OnValueChanged ValueChanged = null;

        public RawUshortEdit(ushort value, NumericUpDown valueInput, bool displayBinary = false)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.UserPaint | 
                ControlStyles.OptimizedDoubleBuffer, 
                true);
            m_ValueInput = valueInput;
            m_ValueInput.ValueChanged += M_valueInput_ValueChanged;
            m_InBinary = displayBinary;
            m_SelectionStartIndex = -1;
            m_SelectionEndIndex = 0;
            SetValue(value);
        }

        private void M_valueInput_ValueChanged(object sender, EventArgs e)
        {
            InsertValue((ushort)((NumericUpDown)sender).Value);
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this);
            }
        }

        public void Deselect()
        {
            m_SelectionStartIndex = -1;
            Refresh();
        }

        public bool IsSelected()
        {
            return (m_SelectionStartIndex != -1);
        }

        public bool InsertValue(ushort value)
        {
            if ((m_SelectionStartIndex == -1)|| m_SettingValues)
                return false;
            int selectionStart = m_SelectionStartIndex;
            int selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;
            string stringValue = Convert.ToString(value, m_InBinary?2:16).PadLeft(selectionLength,'0');
            if (stringValue.Length<= selectionLength)
            {
                string stringWithGap = m_StringRepresentation.Remove(selectionStart, selectionLength);
                m_StringRepresentation = stringWithGap.Insert(selectionStart,stringValue);
                Refresh();
                RefreshValue();
                return true;
            }
            return false;
        }

        public ushort GetValue()
        {
            return m_ValueToEdit;
        }

        public void SetValue(ushort value)
        {
            m_ValueToEdit = value;
            if (m_InBinary)
            {
                Width = 320;
                Height = 20;
                m_FieldWidth = 20;
                m_StringRepresentation = Convert.ToString(m_ValueToEdit, 2).PadLeft(16, '0');
                if (m_SelectionStartIndex != -1)
                {
                    int selectionStart = m_SelectionStartIndex;
                    int selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;
                    m_SettingValues = true; //prevent the valueChanged event

                    m_ValueInput.Value = Convert.ToUInt16(m_StringRepresentation.Substring(selectionStart, selectionLength), 2);

                    m_SettingValues = false;
                }
            }
            else
            {
                Width = 160;
                Height = 20;
                m_FieldWidth = 40;
                m_StringRepresentation = Convert.ToString(m_ValueToEdit, 16).PadLeft(4, '0').ToUpper();
                if (m_SelectionStartIndex != -1)
                {
                    int selectionStart = m_SelectionStartIndex;
                    int selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;
                    m_SettingValues = true; //prevent the valueChanged event
                    
                    m_ValueInput.Value = Convert.ToUInt16(m_StringRepresentation.Substring(selectionStart, selectionLength), 16);

                    m_SettingValues = false;
                }
            }
            Refresh();
        }

        public void SetBinary(bool value)
        {
            if (value == m_InBinary) return;
            if (value)
            {
                Width = 320;
                Height = 20;
                m_InBinary = true;
                m_FieldWidth = 20;
                m_StringRepresentation = Convert.ToString(m_ValueToEdit, 2).PadLeft(16, '0');
                if (m_SelectionStartIndex != -1)
                {
                    m_SelectionStartIndex *= 4;
                    m_SelectionEndIndex *= 4;
                    m_SelectionEndIndex += 3;
                }
            }
            else
            {
                Width = 160;
                Height = 20;
                m_InBinary = false;
                m_FieldWidth = 40;
                m_StringRepresentation = Convert.ToString(m_ValueToEdit, 16).PadLeft(4, '0');
                if (m_SelectionStartIndex != -1)
                {
                    m_SelectionStartIndex /= 4;
                    m_SelectionEndIndex /= 4;

                    int selectionStart = m_SelectionStartIndex;
                    int selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;
                    m_SettingValues = true; //prevent the valueChanged event

                    m_ValueInput.Maximum = (ushort)(Math.Pow(m_InBinary ? 2 : 16, selectionLength) - 1);
                    m_ValueInput.Value = Convert.ToUInt16(m_StringRepresentation.Substring(selectionStart, selectionLength), 16);

                    m_SettingValues = false;
                }
            }
            Refresh();
        }

        public void RefreshValue()
        {
            if (m_InBinary)
            {
                m_ValueToEdit = Convert.ToUInt16(m_StringRepresentation, 2);
            }
            else
            {
                m_ValueToEdit = Convert.ToUInt16(m_StringRepresentation, 16);
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
            if (m_SelectionStartIndex <= m_SelectionEndIndex)
            {
                selectionStart = m_SelectionStartIndex;
                selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;
            }
            else
            {
                selectionStart = m_SelectionEndIndex;
                selectionLength = (m_SelectionStartIndex - m_SelectionEndIndex) + 1;
            }

            e.Graphics.Clear(backColor);

            if (m_SelectionStartIndex!=-1)
            {
                Rectangle selectionRect = new Rectangle(
                    selectionStart * m_FieldWidth, 0,
                    selectionLength * m_FieldWidth, Height
                );
                e.Graphics.FillRectangle(selBackColor, selectionRect);
            }
            for (int i = 0; i < (m_InBinary?16:4); i++)
            {
                Rectangle fieldRect = new Rectangle(1 + i*m_FieldWidth, 1, m_FieldWidth - 2, Height - 2);
                if ((i>= selectionStart) && (i< selectionStart + selectionLength) && (m_SelectionStartIndex != -1))
                    e.Graphics.FillRectangle(selFieldColor, fieldRect);
                else
                    e.Graphics.FillRectangle(fieldColor, fieldRect);
                if ( m_InBinary && (Math.Floor(i/4.0)%2==0) )
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 100, 100, 50)), fieldRect);
                e.Graphics.DrawString(m_StringRepresentation[i].ToString(), textFont, textColor, fieldRect.Left+(m_InBinary?1:3),fieldRect.Top+2);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_SelectionStartIndex = (int)Math.Floor(e.X /(float)m_FieldWidth);
            m_SelectionEndIndex = m_SelectionStartIndex;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                m_SelectionEndIndex = Math.Min(Math.Max(0,(int)Math.Floor(e.X / (float)m_FieldWidth)), m_InBinary ? 15 : 3);
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                if(m_SelectionStartIndex > m_SelectionEndIndex)
                {
                    int startIndexBefore = m_SelectionStartIndex;
                    m_SelectionStartIndex = m_SelectionEndIndex;
                    m_SelectionEndIndex = startIndexBefore;
                }

                int selectionStart = m_SelectionStartIndex;
                int selectionLength = (m_SelectionEndIndex - m_SelectionStartIndex) + 1;

                m_SettingValues = true; //prevent the valueChanged event

                m_ValueInput.Maximum = (ushort)(Math.Pow(m_InBinary ? 2 : 16, selectionLength)-1);
                m_ValueInput.Value = Convert.ToUInt16(m_StringRepresentation.Substring(selectionStart, selectionLength), m_InBinary ? 2 : 16);

                m_SettingValues = false;
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            m_SelectionStartIndex = 0;
            int selectionLength = m_InBinary ? 16 : 4;
            m_SelectionEndIndex = selectionLength - 1;

            m_SettingValues = true; //prevent the valueChanged event

            m_ValueInput.Maximum = (ushort)(Math.Pow(m_InBinary ? 2 : 16, selectionLength) - 1);
            m_ValueInput.Value = Convert.ToUInt16(m_StringRepresentation.Substring(m_SelectionStartIndex, selectionLength), m_InBinary ? 2 : 16);

            m_SettingValues = false;

            Refresh();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //TODO implement a Direct Digit Writing
        }
    }
}
