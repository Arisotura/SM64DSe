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
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe
{
    class LayerTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is int)
                    return ((int)value == 0) ? "All" : value.ToString();
                else
                    return value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string _val = (string)value;
            _val = _val.Trim();

            try
            {
                if (_val.ToLowerInvariant() == "all")
                    return 0;
                else
                    return int.Parse(_val);
            }
            catch
            {
                throw new ArgumentException("Invalid layer number.");
            }
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] layers = { "All", "1", "2", "3", "4", "5", "6", "7" };
            return new StandardValuesCollection(layers);
        }
    }


    class ObjectIDTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            ushort id = (ushort)value;
            if (id == 0x1FF)
                return "511 - Minimap change";
            if (destinationType == typeof(string))
            {
                if (id >= LevelObject.NUM_OBJ_TYPES)
                    return string.Format("{0} - Unknown", id);
                return string.Format("{0} - {1}", id, ObjectDatabase.m_ObjectInfo[id].m_Name);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            ushort ret = 0;
            string _val = (string)value;
            _val = _val.Trim();
            if (_val.IndexOf(" - ") > 0) _val = _val.Substring(0, _val.IndexOf(" - "));

            if (!ushort.TryParse(_val, out ret))
                throw new ArgumentException("Invalid object ID.");

            if (ret >= 512)
                throw new ArgumentException("Object ID out of range.\r\nValid object IDs are within 0 and 326, or 511.");

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    class ObjectIDTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                ObjectListForm dlg = new ObjectListForm((ushort)value);
                if (svc.ShowDialog(dlg) == DialogResult.OK)
                    value = dlg.ObjectID;
            }
            return value;
        }
    }


    class HexNumberTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((ushort)value).ToString("X4");
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            ushort ret = 0;
            string _val = (string)value;
            _val = _val.Trim();
            if (_val.Length > 2 && _val.Substring(0, 2).ToLowerInvariant() == "0x") _val = _val.Substring(2);

            if (!ushort.TryParse(_val, NumberStyles.HexNumber, null, out ret))
                throw new ArgumentException("Invalid value.\r\nIt should be an hexadecimal number.");

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    class FloatTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((float)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            float ret = 0;
            string _val = (string)value;
            _val = _val.Trim();

            if (!float.TryParse(_val, NumberStyles.Float, culture, out ret))
                if (!float.TryParse(_val, NumberStyles.Float, Helper.USA, out ret))
                    throw new ArgumentException("Invalid value.");

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }


    class LevelIDTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is int)
                    return string.Format("{0} - {1}", (int)value, Strings.LevelNames[(int)value]);
                else
                    return value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int ret = 0;
            string _val = (string)value;
            _val = _val.Trim();

            try
            {
                ret = int.Parse(_val.Substring(0, _val.IndexOf(" - ")));
            }
            catch
            {
                throw new ArgumentException("Invalid level ID.");
            }

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> levelnames = new List<string>(Strings.LevelNames.Length);
            for (int i = 0; i < Strings.LevelNames.Length; i++)
                levelnames.Add(string.Format("{0} - {1}", i, Strings.LevelNames[i]));
            return new StandardValuesCollection(levelnames);
        }
    }


    class DoorTypeTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is int)
                    return string.Format("{0} - {1}", (int)value, Strings.DoorTypes[(int)value]);
                else
                    return value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int ret = 0;
            string _val = (string)value;
            _val = _val.Trim();

            try
            {
                ret = int.Parse(_val.Substring(0, _val.IndexOf(" - ")));
            }
            catch
            {
                throw new ArgumentException("Invalid door type.");
            }

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> doorvals = new List<string>(Strings.DoorTypes.Length);
            for (int i = 0; i < Strings.DoorTypes.Length; i++)
                doorvals.Add(string.Format("{0} - {1}", i, Strings.DoorTypes[i]));
            return new StandardValuesCollection(doorvals);
        }
    }


    class Size16TypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is int)
                    return string.Format("{0}/16", ((int)value) + 1);
                else
                    return value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int ret = 0;
            string _val = (string)value;
            _val = _val.Trim();

            try
            {
                ret = int.Parse(_val.Substring(0, _val.IndexOf("/"))) - 1;
            }
            catch
            {
                throw new ArgumentException("Invalid size.");
            }

            return ret;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> sizevals = new List<string>(16);
            for (int i = 0; i < 16; i++)
                sizevals.Add(string.Format("{0}/16", i + 1));
            return new StandardValuesCollection(sizevals);
        }
    }


    class UIntParamTypeConverter : ExpandableObjectConverter
    {
        private PseudoParameter m_PParam;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            PseudoParameter pparam = (PseudoParameter)value;
            m_PParam = pparam;

            if (destinationType == typeof(string))
            {
                if (pparam.m_ParamInfo.m_Values == "hex")
                    return pparam.m_ParamValue.ToString("X");
                else
                    return pparam.m_ParamValue.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            try
            {
                if (m_PParam.m_ParamInfo.m_Values == "hex")
                    m_PParam.m_ParamValue = ushort.Parse((string)value, NumberStyles.HexNumber);
                else
                    m_PParam.m_ParamValue = ushort.Parse((string)value);
                
                ushort max = (ushort)(Math.Pow(2, m_PParam.m_ParamInfo.m_Length) - 1);
                if (m_PParam.m_ParamValue > max)
                    m_PParam.m_ParamValue = max;
            }
            catch
            {
                throw new ArgumentException("Invalid value.");
            }

            return m_PParam;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
