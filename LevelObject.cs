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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public class LevelObject
    {
        public static int NUM_OBJ_TYPES = 326;
        public static string[] k_Layers = { "(all stars)", "(star 1)", "(star 2)", "(star 3)", "(star 4)", "(star 5)", "(star 6)", "(star 7)" };

        public LevelObject(INitroROMBlock data, int layer)
        {
            m_Layer = layer;
            m_Area = -1;

            //m_TestMatrix = new Matrix4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        public virtual void GenerateProperties() { }

        public virtual string GetDescription() { return "LevelObject"; }

        public virtual bool SupportsRotation() { return true; }
        public virtual bool SupportsActs() { return false; }
        public virtual bool HasPosition() { return true; }
        // return value: bit0=refresh display, bit1=refresh propertygrid, bit2=refresh object list
        public virtual int SetProperty(string field, object newval) { return 0; }
        public virtual void SaveChanges(System.IO.BinaryWriter binWriter) { }
        
        public virtual void Render(RenderMode mode)
        {
            if (!m_Renderer.GottaRender(mode))
                return;

            GL.PushMatrix();

            GL.Translate(Position);
            if (YRotation != 0f)
                GL.Rotate(YRotation, 0f, 1f, 0f);

            //GL.MultMatrix(ref m_TestMatrix);

            m_Renderer.Render(mode);

            GL.PopMatrix();
        }
        
        public virtual KCL.RaycastResult? Raycast(Vector3 start, Vector3 dir)
        {
            if (m_KCLName == null) return null;
            KCL kcl = KCLCache.GetKCL(m_KCLName);
            if (kcl == null) return null;

            return kcl.Raycast(start - Position, Vector3.Transform(dir, new Quaternion(Vector3.UnitY, YRotation * (2 * (float)Math.PI) / 360)));
        }

        //public Matrix4 m_TestMatrix;

        public virtual ObjectRenderer InitialiseRenderer() 
        {
            return (Helper.IsOpenGLAvailable() ? BuildRenderer() : null); 
        }
        public virtual ObjectRenderer BuildRenderer() { return null; }
        public virtual string InitializeKCL() { return ""; }

        public virtual void Release()
        {
            m_Renderer.Release();
        }

        public virtual LevelObject Copy()
        {
            LevelObject copy = (LevelObject)MemberwiseClone();
            copy.GenerateProperties();
            copy.m_Renderer = copy.InitialiseRenderer();
            copy.m_KCLName = copy.InitializeKCL();

            return copy;
        }

        public static object[] ComboBoxInfoFromStrings(string[] strings)
        {
            object[] list = new object[strings.Length * 2];
            int index = 0;
            for (int i = 0; i<strings.Length;i++)
            {
                list[index++] = i;
                list[index++] = strings[i];
            }
            return list;
        }

        public enum Type
        {
            STANDARD = 0,
            ENTRANCE = 1, 
            PATH_NODE = 2, 
            PATH = 3, 
            VIEW = 4, 
            SIMPLE = 5, 
            TELEPORT_SOURCE = 6, 
            TELEPORT_DESTINATION = 7, 
            FOG = 8, 
            DOOR = 9, 
            EXIT = 10, 
            MINIMAP_TILE_ID = 11, 
            MINIMAP_SCALE = 12, 
            UNKNOWN_14 = 14
        };

        public ushort ID;
        public Vector3 Position;
        public float YRotation;

        // object specific parameters
        // for standard objects: [0] = 16bit object param, [1] and [2] = what should be X and Z rotation
        // for simple objects: [0] = 7bit object param
        public ushort[] Parameters;
        
        public int m_Layer;
        public int m_Area;
        public uint m_UniqueID;
        public Type m_Type;

        public ObjectRenderer m_Renderer;
        public PropertyTable m_Properties;
        public ParameterField[] m_ParameterFields = null;

        public string m_KCLName = null; // For blocks and whomp's towers only ;)
    }


    public class PseudoParameter
    {
        public PseudoParameter(ObjectDatabase.ObjectInfo.ParamInfo pinfo, ushort val)
        { m_ParamInfo = pinfo; m_ParamValue = val; }

        public ObjectDatabase.ObjectInfo.ParamInfo m_ParamInfo;
        public ushort m_ParamValue;
    }


    public class StandardObject : LevelObject
    {
        //private Hashtable m_PParams;

        public StandardObject(INitroROMBlock data, int num, int layer, int area)
            : base(data, layer)
        {
            m_Area = area;
            m_UniqueID = (uint)(0x10000000 | num);
            m_Type = Type.STANDARD;

            ID = data.Read16(0);
            Position.X = (float)((short)data.Read16(0x2)) / 1000f;
            Position.Y = (float)((short)data.Read16(0x4)) / 1000f;
            Position.Z = (float)((short)data.Read16(0x6)) / 1000f;
            YRotation = ((float)((short)data.Read16(0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[3];
            Parameters[0] = data.Read16( 0xE);
            Parameters[1] = data.Read16( 0x8);
            Parameters[2] = data.Read16( 0xC);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = ParameterField.ParameterFieldsForObject(this);
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return ObjectRenderer.FromLevelObject(this);
        }
        public override string InitializeKCL()
        {
            if (m_Renderer == null) return null;
            string filename = m_Renderer.m_Filename;
            if (filename == null) return null;

            return filename.Substring(0, filename.Length - 4) + ".kcl";
        }

        public override string GetDescription()
        {
            if (ID >= LevelObject.NUM_OBJ_TYPES)
                return String.Format("{0} - Unknown", ID, k_Layers[m_Layer]);
            return String.Format("{0} - {1} {2}", ID, ObjectDatabase.m_ObjectInfo[ID].m_Name, k_Layers[m_Layer]);
        }

        public override System.Boolean SupportsActs() { return true; }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Star", typeof(int), "General", "Which star(s) the object appears for. (all or one)", m_Layer, "", typeof(LayerTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Area", typeof(int), "General", "Which level area the object works in.", m_Area));
            m_Properties.Properties.Add(new PropertySpec("Object ID", typeof(ushort), "General", "What the object will be.", ID, typeof(ObjectIDTypeEditor), typeof(ObjectIDTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The object's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The object's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The object's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the object is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            /*foreach (ObjectDatabase.ObjectInfo.ParamInfo oparam in ObjectDatabase.m_ObjectInfo[ID].m_ParamInfo)
            {
                uint pmask = (uint)(Math.Pow(2, oparam.m_Length) - 1);
                ushort val = (ushort)((Parameters[oparam.m_Offset >> 4] >> (oparam.m_Offset & 0xF)) & pmask);
                PseudoParameter pparam = new PseudoParameter(oparam, val);

                m_PParams.Add(oparam.m_Name, pparam);

                m_Properties.Properties.Add(new PropertySpec(oparam.m_Name, typeof(PseudoParameter), "Object-specific", oparam.m_Description, pparam, "", typeof(UIntParamTypeConverter)));
                m_Properties[oparam.m_Name] = pparam;
            }*/

            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Object-specific (raw)", "", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Object-specific (raw)", "", Parameters[1], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(ushort), "Object-specific (raw)", "", Parameters[2], "", typeof(HexNumberTypeConverter)));

            m_Properties["Star"] = m_Layer;
            m_Properties["Area"] = m_Area;
            m_Properties["Object ID"] = ID;
            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Y rotation"] = YRotation;
            m_Properties["Parameter 1"] = Parameters[0];
            m_Properties["Parameter 2"] = Parameters[1];
            m_Properties["Parameter 3"] = Parameters[2];
        }

        public override int SetProperty(string field, object newval)
        {
            /*if (m_PParams.Contains(field))
            {
                PseudoParameter pparam = (PseudoParameter)m_PParams[field];
                uint pmask = (uint)(Math.Pow(2, pparam.m_ParamInfo.m_Length) - 1);
                Parameters[pparam.m_ParamInfo.m_Offset >> 4] &= (ushort)(~(pmask << (pparam.m_ParamInfo.m_Offset & 0xF)));
                Parameters[pparam.m_ParamInfo.m_Offset >> 4] |= (ushort)((pparam.m_ParamValue & pmask) << (pparam.m_ParamInfo.m_Offset & 0xF));
               
                m_Properties["Parameter 1"] = Parameters[0];
                m_Properties["Parameter 2"] = Parameters[1];
                m_Properties["Parameter 3"] = Parameters[2];

                m_Renderer.Release();
                m_Renderer = ObjectRenderer.FromLevelObject(this);

                return 3;
            }
            else*/
            {
                switch (field)
                {
                    case "Object ID": ID = (ushort)newval; break;
                    case "X position": Position.X = (float)newval; break;
                    case "Y position": Position.Y = (float)newval; break;
                    case "Z position": Position.Z = (float)newval; break;
                    case "Y rotation": YRotation = (float)newval; break;
                    case "Parameter 1": Parameters[0] = (ushort)newval; break;
                    case "Parameter 2": Parameters[1] = (ushort)newval; break;
                    case "Parameter 3": Parameters[2] = (ushort)newval; break;
                }

                if ((field == "Object ID") || (field.IndexOf("Parameter ") != -1))
                {
                    m_Renderer.Release();
                    m_Renderer = InitialiseRenderer();
                    m_KCLName = InitializeKCL();
                    //return 3;
                }

                if (field == "Object ID")
                    return 5;
            }

            return 1;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(ID);
            binWriter.Write((ushort)((short)(Position.X * 1000f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000f)));
            binWriter.Write(Parameters[1]);
            binWriter.Write((ushort)((short)((YRotation / 22.5f) * 4096f)));
            binWriter.Write(Parameters[2]);
            binWriter.Write(Parameters[0]);
        }
    }


    public class SimpleObject : LevelObject
    {
        public SimpleObject(INitroROMBlock data, int num, int layer, int area)
            : base(data, layer)
        {
            m_Area = area;
            m_UniqueID = (uint)(0x10000000 | num);
            m_Type = Type.SIMPLE;

            ushort idparam = data.Read16(0);
            ID = (ushort)(idparam & 0x1FF);
            Position.X = (float)((short)data.Read16(0x2)) / 1000f;
            Position.Y = (float)((short)data.Read16(0x4)) / 1000f;
            Position.Z = (float)((short)data.Read16(0x6)) / 1000f;
            YRotation = 0.0f;

            Parameters = new ushort[1];
            Parameters[0] = (ushort)(idparam >> 9);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = ParameterField.ParameterFieldsForObject(this);
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return ObjectRenderer.FromLevelObject(this);
        }

        public override string GetDescription()
        {
            if (ID == 511) return "511 - Minimap change " + k_Layers[m_Layer];
            return String.Format("{0} - {1} {2}", ID, ObjectDatabase.m_ObjectInfo[ID].m_Name, k_Layers[m_Layer]);
        }

        public override System.Boolean SupportsActs() { return true; }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Star", typeof(int), "General", "Which star(s) the object appears for. (all or one)", m_Layer, "", typeof(LayerTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Area", typeof(int), "General", "Which level area the object works in.", m_Area));
            m_Properties.Properties.Add(new PropertySpec("Object ID", typeof(ushort), "General", "What the object will be.", ID, typeof(ObjectIDTypeEditor), typeof(ObjectIDTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The object's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The object's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The object's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));

            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Object-specific (raw)", "", Parameters[0], "", typeof(HexNumberTypeConverter)));

            m_Properties["Star"] = m_Layer;
            m_Properties["Area"] = m_Area;
            m_Properties["Object ID"] = ID;
            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Parameter 1"] = Parameters[0];
        }
        
        public override bool SupportsRotation() { return false; }
        
        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Object ID": ID = (ushort)newval; break;
                case "X position": Position.X = (float)newval; break;
                case "Y position": Position.Y = (float)newval; break;
                case "Z position": Position.Z = (float)newval; break;
                case "Parameter 1": Parameters[0] = (ushort)newval; break;
            }

            if ((field == "Object ID") || (field == "Parameter 1"))
            {
                m_Renderer.Release();
                m_Renderer = InitialiseRenderer();
                m_KCLName = InitializeKCL();
            }

            if (field == "Object ID")
                return 5;

            return 1;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            ushort idparam = (ushort)((ID & 0x1FF) | (Parameters[0] << 9));
            binWriter.Write(idparam);
            binWriter.Write((ushort)((short)(Position.X * 1000f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000f)));
        }
    }

    public class EntranceObject : LevelObject
    {
        public int m_EntranceID;

        public EntranceObject(INitroROMBlock data, int num, int layer, int id)
            : base(data, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_EntranceID = id;
            m_Type = Type.ENTRANCE;

            ID = 0;
            Position.X = (float)((short)data.Read16(0x2)) / 1000.0f;
            Position.Y = (float)((short)data.Read16(0x4)) / 1000.0f;
            Position.Z = (float)((short)data.Read16(0x6)) / 1000.0f;
            YRotation = ((float)((short)data.Read16(0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[5];
            Parameters[0] = data.Read16(0x0);
            Parameters[1] = data.Read16(0x8);
            Parameters[2] = data.Read16(0xC);
            Parameters[3] = data.Read16(0xE);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = new ParameterField[]
            {
                new ListField("Parameter 4",0,9,new object[]{
                    0, "Spawns on ground standing.",
                    1, "Wingcap Mario for ? Switch only, else falls and takes damage.",
                    2, "Mario has Wingcap, other characters Spin in with Star wipe.",
                    3, "Spin in with Circle wipe.",
                    4, "Fall in, Star Wipe.",
                    5, "Fall in, Star Wipe. (Duplicate?)",
                    6, "Fall in, No Wipe.",
                    7, "Fall in, No Wipe. (Duplicate?)",
                    8, "Like jumping out of a pit/pipe, but lets you save.",
                    9, "Like jumping out of a pit/pipe, but lets you save. (Duplicate?)",
                    10, "Spin in, No Wipe.",
                    11, "Acts like painting, makes sound, lets you save.",
                    12, "Acts like painting, makes sound, lets you save. (Duplicate?)",
                    13, "Jumps with fist in air, like coming out of a pipe.",
                    14, "Spawns on ground standing (Castle Door Entrance).",
                    15, "Fall in with Mario Wipe."
                }) {Name = "EntranceMode" },
                new DefaultField("Parameter 4",9,4){ Name = "ViewId", DislpayInHex = false },
                new DefaultField("Parameter 4",13,3){ Name = "Area" },
            };
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(0, 255, 0), Color.FromArgb(0, 64, 0), true);
        }

        public override string GetDescription()
        {
            // TODO describe better
            return string.Format("[{0}] Entrance", m_EntranceID);
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The entrance's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The entrance's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The entrance's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the entrance is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "Purpose is to be a zero.", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Redundant Ex Rotation.", Parameters[1], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(ushort), "Specific", "Redundant Zee Rotation.", Parameters[2], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 4", typeof(ushort), "Specific", "EEEEEEEEEVVVVCCC: E - Entrance mode, V - View ID, C - Area ID", Parameters[3], "", typeof(HexNumberTypeConverter)));

            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Y rotation"] = YRotation;
            m_Properties["Parameter 1"] = Parameters[0];
            m_Properties["Parameter 2"] = Parameters[1];
            m_Properties["Parameter 3"] = Parameters[2];
            m_Properties["Parameter 4"] = Parameters[3];
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;
                case "Y rotation": YRotation = (float)newval; return 1;

                case "Parameter 1": Parameters[0] = (ushort)newval; return 0;
                case "Parameter 2": Parameters[1] = (ushort)newval; return 0;
                case "Parameter 3": Parameters[2] = (ushort)newval; return 0;
                case "Parameter 4": Parameters[3] = (ushort)newval; return 0;
            }
            

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(Parameters[0]);
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write(Parameters[1]);
            binWriter.Write((ushort)((short)((YRotation / 22.5f) * 4096f)));
            binWriter.Write(Parameters[2]);
            binWriter.Write(Parameters[3]);
        }
    }

    public class ExitObject : LevelObject
    {
        public int LevelID, EntranceID;
        public ushort Param1, Param2;
        public byte width, height;

        public ExitObject(INitroROMBlock data, int num, int layer)
            : base(data, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = Type.EXIT;

            Position.X = (float)((short)data.Read16(0x0)) / 1000.0f;
            Position.Y = (float)((short)data.Read16(0x2)) / 1000.0f;
            Position.Z = (float)((short)data.Read16(0x4)) / 1000.0f;
            YRotation = ((float)((short)data.Read16(0x8)) / 4096f) * 22.5f;

            LevelID    = data.Read8 (0xA);
            EntranceID = data.Read8 (0xB);
            Param1     = data.Read16(0x6);
            Param2     = data.Read16(0xC);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_Properties = new PropertyTable();
            m_ParameterFields = new ParameterField[]
            {
                new ListField("Destination level",8,8,ComboBoxInfoFromStrings(Strings.LevelNames)){Name = "Destination level"},
                new DefaultField("Destination entrance",8,8){ Name = "Destination entrance", Description = "The Entrance you spawn at in the level", DislpayInHex = false },
                new FloatConvertField("Parameter 1",0,16,0x1000,22.5f){ Name = "X Rotation"},
                new DefaultField("Parameter 2",4,4){ Name = "width", Description = "The width of the exits trigger area", DislpayInHex = false },
                new DefaultField("Parameter 2",0,4){ Name = "height", Description = "The height of the exits trigger area", DislpayInHex = false },
                new DefaultField("Parameter 2",8,8){ Name = "returnEntrance", Description = "The Entrance you spawn at, if you die or collect a star/key, 255 means you spawn at the default Entrance in Peachs Castle", DislpayInHex = false }
            };
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new ExitRenderer(Param1,Param2);
        }

        public override string GetDescription()
        {
            return string.Format("Exit ({0}, entrance {1}) {2}", Strings.LevelNames[LevelID], EntranceID, k_Layers[m_Layer]);
        }

        public override System.Boolean SupportsActs() { return true; }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Star", typeof(int), "General", "Which star(s) the exit works for. (all or one)", m_Layer, "", typeof(LayerTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The exit's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The exit's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The exit's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the exit is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Destination level", typeof(int), "Specific", "The level the exit leads to.", LevelID, "", typeof(LevelIDTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Destination entrance", typeof(int), "Specific", "The ID of the entrance in the destination level, the exit is connected to.", EntranceID));
            m_Properties.Properties.Add(new PropertySpec("width", typeof(byte), "Specific", "The width of the exit trigger area", width, "", typeof(Size16TypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("height", typeof(byte), "Specific", "The height of the exit trigger area", height, "", typeof(Size16TypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific(raw)", "Purpose unknown.", Param1, "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific(raw)", "Purpose unknown.", Param2, "", typeof(HexNumberTypeConverter)));

            m_Properties["Star"] = m_Layer;
            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Y rotation"] = YRotation;
            m_Properties["Destination level"] = LevelID;
            m_Properties["Destination entrance"] = EntranceID;
            m_Properties["Parameter 1"] = Param1;
            m_Properties["Parameter 2"] = Param2;
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;
                case "Y rotation": YRotation = (float)newval; return 1;
                case "Destination level":
                    if (newval is string) LevelID = int.Parse(((string)newval).Substring(0, ((string)newval).IndexOf(" - ")));
                    else LevelID = (int)newval; 
                    return 4;
                case "Destination entrance": EntranceID = (int)newval; return 4;
                case "Parameter 1":
                    Param1 = (ushort)newval;
                    m_Renderer.Release();
                    m_Renderer = InitialiseRenderer();
                    m_KCLName = InitializeKCL();
                    return 5;
                case "Parameter 2":
                    Param2 = (ushort)newval;
                    m_Renderer.Release();
                    m_Renderer = InitialiseRenderer();
                    m_KCLName = InitializeKCL();
                    return 5;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write(Param1);
            binWriter.Write((ushort)((short)((YRotation / 22.5f) * 4096f)));
            binWriter.Write((byte)LevelID);
            binWriter.Write((byte)EntranceID);
            binWriter.Write(Param2);
        }
    }

    public class DoorObject : LevelObject
    {
        public int DoorType, OutAreaID, InAreaID, PlaneSizeX, PlaneSizeY;

        public DoorObject(INitroROMBlock data, int num, int layer)
            : base(data, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = Type.DOOR;

            ID = 0;
            Position.X = (float)((short)data.Read16(0x0)) / 1000f;
            Position.Y = (float)((short)data.Read16(0x2)) / 1000f;
            Position.Z = (float)((short)data.Read16(0x4)) / 1000f;
            YRotation = ((float)((short)data.Read16(0x6)) / 4096f) * 22.5f;

            DoorType = data.Read8(0xA);
            if (DoorType > 0x17) DoorType = 0x17;

            InAreaID = data.Read8(0x9);
            OutAreaID = InAreaID >> 4;
            InAreaID &= 0xF;

            PlaneSizeX = data.Read8(0x8);
            PlaneSizeY = PlaneSizeX >> 4;
            PlaneSizeX &= 0xF;

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = new ParameterField[]
            {
                new ListField("Door type",8,8, ComboBoxInfoFromStrings(Strings.DoorTypes)){Name = "DoorType"},
                new DefaultField("Inside area",13,3){Name = "Inside area", Description = "The area that gets loaded, if you enter from the front side"},
                new DefaultField("Outside area",13,3){Name = "Outside area", Description = "The area that gets loaded, if you enter from the back side"},
                new DefaultField("Plane width",12,4){Name = "Plane width", Description = "The width of the plane (only for Virtual doors)", DislpayInHex = false},
                new DefaultField("Plane height",12,4){Name = "Plane height", Description = "The height of the plane (only for Virtual doors)", DislpayInHex = false}
            };
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new DoorRenderer(this);
        }

        public override string GetDescription()
        {
            return string.Format("{1}, areas {2}/{3} {0}", k_Layers[m_Layer], Strings.DoorTypes[DoorType], OutAreaID, InAreaID);
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Star", typeof(int), "General", "Which star(s) the door appears for. (all or one)", m_Layer, "", typeof(LayerTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The door's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The door's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The door's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the door is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Door type", typeof(int), "Specific", "What the door will look like.", DoorType, "", typeof(DoorTypeTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Outside area", typeof(int), "Specific", "The ID of the 'outside' area.", OutAreaID));
            m_Properties.Properties.Add(new PropertySpec("Inside area", typeof(int), "Specific", "The ID of the 'inside' area.", InAreaID));
            m_Properties.Properties.Add(new PropertySpec("Plane width", typeof(int), "Specific", "For virtual doors, the width of the door plane.", PlaneSizeX, "", typeof(Size16TypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Plane height", typeof(int), "Specific", "For virtual doors, the height of the door plane.", PlaneSizeY, "", typeof(Size16TypeConverter)));

            m_Properties["Star"] = m_Layer;
            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Y rotation"] = YRotation;
            m_Properties["Door type"] = DoorType;
            m_Properties["Outside area"] = OutAreaID;
            m_Properties["Inside area"] = InAreaID;
            m_Properties["Plane width"] = PlaneSizeX;
            m_Properties["Plane height"] = PlaneSizeY;
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;
                case "Y rotation": YRotation = (float)newval; return 1;

                case "Door type":
                    if (newval is int) DoorType = (int)newval;
                    else DoorType = int.Parse(((string)newval).Substring(0, ((string)newval).IndexOf(" - ")));
                    m_Renderer.Release();
                    m_Renderer = InitialiseRenderer();
                    m_KCLName = InitializeKCL();
                    return 5;

                case "Outside area": OutAreaID = Math.Max(0, Math.Min(15, (int)newval)); return 1;
                case "Inside area": InAreaID = Math.Max(0, Math.Min(15, (int)newval)); return 1;

                case "Plane width":
                    if (newval is int) PlaneSizeX = (int)newval;
                    else PlaneSizeX = int.Parse(((string)newval).Substring(0, ((string)newval).IndexOf("/"))) - 1;
                    return 1;

                case "Plane height":
                    if (newval is int) PlaneSizeY = (int)newval;
                    else PlaneSizeY = int.Parse(((string)newval).Substring(0, ((string)newval).IndexOf("/"))) - 1;
                    return 1;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write((ushort)((short)((YRotation / 22.5f) * 4096f)));
            binWriter.Write((byte)(PlaneSizeX | (PlaneSizeY << 4)));
            binWriter.Write((byte)(InAreaID | (OutAreaID << 4)));
            binWriter.Write((ushort)DoorType);
        }
    }

    public class PathPointObject : LevelObject
    {
        public ushort ParentPath;
        public byte m_IndexInPath; 

        public PathPointObject(INitroROMBlock data, int num, int nodeID)
            : base(data, 0)
        {
            m_UniqueID = (uint)(0x30000000 | num);
            m_Type = Type.PATH_NODE;
            ParentPath = 0;
            m_IndexInPath = 0;
            m_NodeID = nodeID;

            Position.X = (float)((short)data.Read16(0x0)) / 1000f;
            Position.Y = (float)((short)data.Read16(0x2)) / 1000f;
            Position.Z = (float)((short)data.Read16(0x4)) / 1000f;

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }
        
        public int m_NodeID;

        public override ObjectRenderer BuildRenderer()
        {
            return new ColourArrowRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false);
        }

        public override string GetDescription()
        {
            return "Path Node " + m_IndexInPath;
        }

        public override bool SupportsRotation() { return false; }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The view's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The view's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The view's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            
            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;
            }
            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
        }
    }

    public class PathObject : LevelObject
    {
        public ushort m_PathID;

        public PathObject(INitroROMBlock data, int num, ushort id)
            : base(data, 0)
        {
            m_UniqueID = (uint)(0x30000000 | num);
            m_PathID = id;
            m_Type = Type.PATH;

            Parameters = new ushort[5];
            Parameters[0] =         data.Read16(0x0);
            Parameters[1] = (ushort)data.Read8 (0x2);
            Parameters[2] = (ushort)data.Read8 (0x3);
            Parameters[3] = (ushort)data.Read8 (0x4);
            Parameters[4] = (ushort)data.Read8 (0x5);

            m_ParameterFields = new ParameterField[]
            {
                //new DefaultField("Start Node",0,16){Name = "startNode", Description = "The index of the first node in this Path", DislpayInHex = false},
                //new DefaultField("Length",8,8){Name = "pathLength", Description = "How many nodes are in this Path", DislpayInHex = false},
                new DefaultField("Parameter 3",8,8){Name = "1. Parameter"},
                new DefaultField("Parameter 4",8,8){Name = "2. Parameter", Description = "1 to 3 are different speeds for wind/quicksand/water/conveyorBelt paths?"},
                new DefaultField("Parameter 5",8,8){Name = "3. Parameter", Description = "FF means path is closed, everything else is Unkown"}
            };
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Path " + m_PathID;
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            //m_Properties.Properties.Add(new PropertySpec("Start Node", typeof(float), "General", "Index of starting node.", (float)Parameters[0], "", typeof(FloatTypeConverter)));
            //m_Properties.Properties.Add(new PropertySpec("Length", typeof(float), "General", "Number of nodes in path.", (float)Parameters[1], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(float), "General", "Unknown", (float)Parameters[2], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 4", typeof(float), "General", "Unknown", (float)Parameters[3], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 5", typeof(float), "General", "Unknown", (float)Parameters[4], "", typeof(FloatTypeConverter)));

            //m_Properties["Start Node"] = (float)Parameters[0];
            //m_Properties["Length"] = (float)Parameters[1];
            m_Properties["Parameter 3"] = (float)Parameters[2];
            m_Properties["Parameter 4"] = (float)Parameters[3];
            m_Properties["Parameter 5"] = (float)Parameters[4];
        }

        public override bool SupportsRotation() { return false; }

        public override bool HasPosition() { return false; }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Start Node": Parameters[0] = (ushort)(float)newval; break;
                case "Length": Parameters[1] = (ushort)(float)newval; break;
                case "Parameter 3": Parameters[2] = (ushort)(float)newval; break;
                case "Parameter 4": Parameters[3] = (ushort)(float)newval; break;
                case "Parameter 5": Parameters[4] = (ushort)(float)newval; break;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(Parameters[0]);
            binWriter.Write((byte)Parameters[1]);
            binWriter.Write((byte)Parameters[2]);
            binWriter.Write((byte)Parameters[3]);
            binWriter.Write((byte)Parameters[4]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class ViewObject : LevelObject
    {
        public int m_ViewID;

        public ViewObject(INitroROMBlock data, int num, int id)
            : base(data, 0)
        {
            m_UniqueID = (uint)(0x40000000 | num);
            m_ViewID = id;
            m_Type = LevelObject.Type.VIEW;

            Position.X = (float)((short)data.Read16(0x2)) / 1000.0f;
            Position.Y = (float)((short)data.Read16(0x4)) / 1000.0f;
            Position.Z = (float)((short)data.Read16(0x6)) / 1000.0f;
            YRotation = ((float)((short)data.Read16(0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[3];
            Parameters[0] = data.Read16(0x0);
            Parameters[1] = data.Read16(0x8);
            Parameters[2] = data.Read16(0xC);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = new ParameterField[]
            {
                new ListField("Parameter 1",8,8,new object[]{
                    0,"Outside cylinder",
                    1,"Inside cylinder",
                    2,"Normal camera",
                    3,"Point for MultiFocus camera",
                    4,"RotationOnly camera",
                    5,"Spiraling stairs?",
                    6,"PathFollowing camera",
                    7,"PauseCamera CenterPoint"
                }) {Name = "ViewMode" },
                new DefaultField("Parameter 1",0,8) {Name = "View Parameter"}
            };
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(255, 255, 0), Color.FromArgb(64, 64, 0), true);
        }

        public override string GetDescription()
        {
            // TODO describe better
            return string.Format("[{0}] View", m_ViewID);
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The view's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The view's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The view's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the view is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "View type stuff.", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Redundant Ex Rotation.", Parameters[1], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(ushort), "Specific", "Redundant Zee Rotation.", Parameters[2], "", typeof(HexNumberTypeConverter)));

            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Y rotation"] = YRotation;
            m_Properties["Parameter 1"] = Parameters[0];
            m_Properties["Parameter 2"] = Parameters[1];
            m_Properties["Parameter 3"] = Parameters[2];
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;
                case "Y rotation": YRotation = (float)newval; return 1;

                case "Parameter 1": Parameters[0] = (ushort)newval; return 0;
                case "Parameter 2": Parameters[1] = (ushort)newval; return 0;
                case "Parameter 3": Parameters[2] = (ushort)newval; return 0;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(Parameters[0]);
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write(Parameters[1]);
            binWriter.Write((ushort)((short)((YRotation / 22.5f) * 4096f)));
            binWriter.Write(Parameters[2]);
        }
    }

    public class TpSrcObject : LevelObject
    {
        public TpSrcObject(INitroROMBlock data, int num, int layer)
            : base(data, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = Type.TELEPORT_SOURCE;

            Position.X = (float)((short)data.Read16(0x0)) / 1000.0f;
            Position.Y = (float)((short)data.Read16(0x2)) / 1000.0f;
            Position.Z = (float)((short)data.Read16(0x4)) / 1000.0f;
            YRotation = 0.0f;

            Parameters = new ushort[2];
            Parameters[0] = data.Read8(0x6);
            Parameters[1] = data.Read8(0x7);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = new ParameterField[]
            {
                new DefaultField("Parameter 2",4,8){Name = "Destination", DislpayInHex = false},
                new DefaultField("Parameter 1",0,16)
            };
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(255, 0, 255), Color.FromArgb(64, 0, 64), false);
        }

        public override string GetDescription()
        {
            return "Teleport source " + k_Layers[m_Layer];
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The teleport source's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The teleport source's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The teleport source's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "Purpose unknown.", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Teleport destination index (0 based) * 10.", Parameters[1], "", typeof(HexNumberTypeConverter)));

            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Parameter 1"] = Parameters[0];
            m_Properties["Parameter 2"] = Parameters[1];
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;

                case "Parameter 1": Parameters[0] = (ushort)newval; return 0;
                case "Parameter 2": Parameters[1] = (ushort)newval; return 0;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write((byte)Parameters[0]);
            binWriter.Write((byte)Parameters[1]);
        }
    }

    public class TpDstObject : LevelObject
    {
        public TpDstObject(INitroROMBlock data, int num, int layer)
            : base(data, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = Type.TELEPORT_DESTINATION;

            Position.X = (float)((short)data.Read16(0x0)) / 1000.0f;
            Position.Y = (float)((short)data.Read16(0x2)) / 1000.0f;
            Position.Z = (float)((short)data.Read16(0x4)) / 1000.0f;
            YRotation = 0.0f;

            Parameters = new ushort[1];
            Parameters[0] = data.Read16(0x6);

            m_Renderer = InitialiseRenderer();
            m_KCLName = InitializeKCL();
            m_ParameterFields = new ParameterField[]
            {
                new DefaultField("Parameter",0,16)
            };
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer BuildRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(255, 128, 0), Color.FromArgb(64, 32, 0), false);
        }

        public override string GetDescription()
        {
            return "Teleport destination " + k_Layers[m_Layer];
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The teleport destination's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The teleport destination's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The teleport destination's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter", typeof(ushort), "Specific", "Purpose unknown.", Parameters[0], "", typeof(HexNumberTypeConverter)));

            m_Properties["X position"] = Position.X;
            m_Properties["Y position"] = Position.Y;
            m_Properties["Z position"] = Position.Z;
            m_Properties["Parameter"] = Parameters[0];
        }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "X position": Position.X = (float)newval; return 1;
                case "Y position": Position.Y = (float)newval; return 1;
                case "Z position": Position.Z = (float)newval; return 1;

                case "Parameter": Parameters[0] = (ushort)newval; return 0;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((ushort)((short)(Position.X * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Y * 1000.0f)));
            binWriter.Write((ushort)((short)(Position.Z * 1000.0f)));
            binWriter.Write(Parameters[0]);
        }
    }

    public class MinimapScaleObject : LevelObject
    {
        public MinimapScaleObject(INitroROMBlock data, int num, int layer, int area)
            : base(data, layer)
        {
            m_Type = Type.MINIMAP_SCALE;
            m_Area = area;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[1];
            Parameters[0] = data.Read16(0);
            m_ParameterFields = new ParameterField[]
            {
                new FloatField("Scale")
            };

            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Minimap Scale";
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Scale", typeof(float), "Specific", "Scale of minimap.", (float)(Parameters[0] / 1000f), "", typeof(FloatTypeConverter)));

            m_Properties["Scale"] = (float)(Parameters[0] / 1000f);
        }

        public override bool SupportsRotation() { return false; }

        public override bool HasPosition() { return false; }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Scale": Parameters[0] = (ushort)((float)newval * 1000f); break;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(Parameters[0]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class FogObject : LevelObject
    {
        public FogObject(INitroROMBlock data, int num, int layer, int area)
            : base(data, layer)
        {
            m_Area = area;
            m_Type = Type.FOG;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[6];
            Parameters[0] = data.Read8 (0);
            Parameters[1] = data.Read8 (1);
            Parameters[2] = data.Read8 (2);
            Parameters[3] = data.Read8 (3);
            Parameters[4] = data.Read16(4);
            Parameters[5] = data.Read16(6);

            //m_Renderer = new ColorCubeRenderer(Color.FromArgb(255, 255, 0), Color.FromArgb(Parameters[1], Parameters[2], Parameters[3]), true);
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Fog for area " + m_Area;
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Density", typeof(float), "Specific", "Density of fog. 0 - No fog, 1 - Show Fog", (float)Parameters[0], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("RGB R Value", typeof(float), "Specific", "RGB Red value for fog colour.", (float)Parameters[1], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("RGB G Value", typeof(float), "Specific", "RGB Green value for fog colour.", (float)Parameters[2], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("RGB B Value", typeof(float), "Specific", "RGB Blue value for fog colour.", (float)Parameters[3], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Start Distance", typeof(float), "Specific", "Distance at which to start drawing fog.", (float)(Parameters[4] / 1000), "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("End Distance", typeof(float), "Specific", "Distance at which to stop drawing fog.", (float)(Parameters[5] / 1000), "", typeof(FloatTypeConverter)));

            m_Properties["Density"] = (float)Parameters[0];
            m_Properties["RGB R Value"] = (float)Parameters[1];
            m_Properties["RGB G Value"] = (float)Parameters[2];
            m_Properties["RGB B Value"] = (float)Parameters[3];
            m_Properties["Start Distance"] = (float)(Parameters[4] / 1000);
            m_Properties["End Distance"] = (float)(Parameters[5] / 1000);
        }

        public override bool SupportsRotation() { return false; }

        public override bool HasPosition() { return false; }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Density": Parameters[0] = (ushort)(float)newval; break;
                case "RGB R Value": Parameters[1] = (ushort)(float)newval; break;
                case "RGB G Value": Parameters[2] = (ushort)(float)newval; break;
                case "RGB B Value": Parameters[3] = (ushort)(float)newval; break;
                case "Start Distance": Parameters[4] = (ushort)((float)newval * 1000f); break;
                case "End Distance": Parameters[5] = (ushort)((float)newval * 1000f); break;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((byte)Parameters[0]);
            binWriter.Write((byte)Parameters[1]);
            binWriter.Write((byte)Parameters[2]);
            binWriter.Write((byte)Parameters[3]);
            binWriter.Write((ushort)Parameters[4]);
            binWriter.Write((ushort)Parameters[5]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class Type14Object : LevelObject
    {
        public Type14Object(INitroROMBlock data, int num, int layer, int area)
            : base(data, layer)
        {
            m_Area = area;
            m_Type = Type.UNKNOWN_14;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[4];
            Parameters[0] = data.Read8(0);
            Parameters[1] = data.Read8(1);
            Parameters[2] = data.Read8(2);
            Parameters[3] = data.Read8(3);

            object[] cameraModes = new object[]{
                0, "Zoom in when the player thrusts the star in the air.",
                1, "Rotate to face FF07 view and zoom out.",
                2, "Stand Still.",
                3, "Spin around.",
                4, "Stand Still. Used for castle and 100 coin stars.",
                5, "Zooms in, then spins around crazily.",
                6, "Freeze Game.",
                7, "Freeze Game.",
                8, "Freeze Game.",
                9, "Player collects star, but wipe freezes game.",
                10, "0, but without zooming.",
                11, "Freezes Game.",
                12, "Cuts to FF07 camera???",
                13, "Same as 10. (DUPLICATE?)",
                14, "Same as 10. (DUPLICATE?)",
                15, "Same as 10. (DUPLICATE?)"
            };

            m_ParameterFields = new ParameterField[]
            {
                new ListField("Parameter 1", 12, 4, cameraModes) {Name = "100coins" },
                new ListField("Parameter 1", 8, 4, cameraModes) {Name = "1. Star" },
                new ListField("Parameter 2", 12, 4, cameraModes) {Name = "2. Star" },
                new ListField("Parameter 2", 8, 4, cameraModes) {Name = "3. Star" },
                new ListField("Parameter 3", 12, 4, cameraModes) {Name = "4. Star" },
                new ListField("Parameter 3", 8, 4, cameraModes) {Name = "5. Star" },
                new ListField("Parameter 4", 12, 4, cameraModes) {Name = "6. Star" },
                new ListField("Parameter 4", 8, 4, cameraModes) {Name = "7. Star" }
            };
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Star Cameras";
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(float), "Specific", "It's a mystery...", (float)Parameters[0], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(float), "Specific", "It's a mystery...", (float)Parameters[1], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(float), "Specific", "It's a mystery...", (float)Parameters[2], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 4", typeof(float), "Specific", "It's a mystery...", (float)Parameters[3], "", typeof(FloatTypeConverter)));

            m_Properties["Parameter 1"] = (float)Parameters[0];
            m_Properties["Parameter 2"] = (float)Parameters[1];
            m_Properties["Parameter 3"] = (float)Parameters[2];
            m_Properties["Parameter 4"] = (float)Parameters[3];
        }

        public override bool SupportsRotation() { return false; }

        public override bool HasPosition() { return false; }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Parameter 1": Parameters[0] = (ushort)(float)newval; break;
                case "Parameter 2": Parameters[1] = (ushort)(float)newval; break;
                case "Parameter 3": Parameters[2] = (ushort)(float)newval; break;
                case "Parameter 4": Parameters[3] = (ushort)(float)newval; break;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write((byte)Parameters[0]);
            binWriter.Write((byte)Parameters[1]);
            binWriter.Write((byte)Parameters[2]);
            binWriter.Write((byte)Parameters[3]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class MinimapTileIDObject : LevelObject
    {
        public int m_MinimapTileIDNum;

        public MinimapTileIDObject(INitroROMBlock data, int num, int layer, int id)
            : base(data, layer)
        {
            m_Type = Type.MINIMAP_TILE_ID;
            m_MinimapTileIDNum = id;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[2];
            Parameters[0] = data.Read16(0);

            m_ParameterFields = new ParameterField[]
            {
                new DefaultField("Tile ID",0,16){Name = "Tile ID", DislpayInHex = false}
            };

            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Minimap Tile ID for area " + m_MinimapTileIDNum;
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Tile ID", typeof(float), "Specific", "ID of minimap tile to use in area " + m_MinimapTileIDNum, (float)Parameters[0], "", typeof(FloatTypeConverter)));

            m_Properties["Tile ID"] = (float)Parameters[0];
        }

        public override bool SupportsRotation() { return false; }

        public override bool HasPosition() { return false; }

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Tile ID": Parameters[0] = (ushort)(float)newval; break;
            }

            return 0;
        }

        public override void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            binWriter.Write(Parameters[0]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    //Stores the texture animation data per AREA.
    public class LevelTexAnim
    {
        public LevelTexAnim(int area) 
        {
            m_Area = area;
            m_Defs = new List<Def>();
        }

        public LevelTexAnim(NitroOverlay ovl, int area, int numAreas, 
            byte levelFormatVersion = Level.k_LevelFormatVersion)
        {
            // Address of the animation data
            m_UniqueID = (uint)area;
            m_Area = area;

            if(area >= numAreas || ovl.Read32((uint)(ovl.ReadPointer(0x70) + 12 * area + 4)) == 0)
            {
                m_Defs = new List<Def>();
                return;
            }

            uint offset = ovl.ReadPointer((uint)(ovl.ReadPointer(0x70) + 12 * area + 4));
            m_NumFrames = ovl.Read32(offset + 0x00);
            uint scaleOffset = ovl.ReadPointer(offset + 0x04);
            uint rotOffset = ovl.ReadPointer(offset + 0x08);
            uint transOffset = ovl.ReadPointer(offset + 0x0c);
            uint numAnims = ovl.Read32(offset + 0x10);

            m_Defs = new List<Def>((int)numAnims);
            uint animOffsetStatic = ovl.ReadPointer(offset + 0x14);

            for (uint animOffset = animOffsetStatic; animOffset < animOffsetStatic + 0x1c * numAnims; animOffset += 0x1c)
            {
                Def def = new Def();
                def.m_MaterialName = ovl.ReadString(ovl.ReadPointer(animOffset + 0x04), -1);
                def.m_DefaultScale = ovl.Read32(animOffset + 0x08);

                uint numScale = ovl.Read16(animOffset + 0x0C);
                uint numRot   = ovl.Read16(animOffset + 0x10);
                uint numTransX = ovl.Read16(animOffset + 0x14);
                uint numTransY = ovl.Read16(animOffset + 0x18);

                uint scaleIndex = ovl.Read16(animOffset + 0x0E);
                uint rotIndex   = ovl.Read16(animOffset + 0x12);
                uint transXIndex = ovl.Read16(animOffset + 0x16);
                uint transYIndex = ovl.Read16(animOffset + 0x1A);

                def.m_ScaleValues = new List<float>((int)numScale);
                def.m_RotationValues   = new List<float>((int)numRot);
                def.m_TranslationXValues = new List<float>((int)numTransX);
                def.m_TranslationYValues = new List<float>((int)numTransY);

                uint scaleStartOffset = scaleOffset + (scaleIndex * 4);
                uint rotationStartOffset = rotOffset + (rotIndex * 2);
                uint translationXStartOffset = transOffset + (transXIndex * 4);
                uint translationYStartOffset = transOffset + (transYIndex * 4);

                for (uint offs = scaleStartOffset; offs < scaleStartOffset + numScale * 4; offs += 4)
                    def.m_ScaleValues.Add((int)ovl.Read32(offs) / 4096.0f);
                for (uint offs = rotationStartOffset; offs < rotationStartOffset + numRot * 2; offs += 2)
                    def.m_RotationValues  .Add((short)ovl.Read16(offs) / 4096.0f * 360.0f);
                for (uint offs = translationXStartOffset; offs < translationXStartOffset + numTransX * 4; offs += 4)
                    def.m_TranslationXValues.Add((int)ovl.Read32(offs) / 4096.0f);
                switch (levelFormatVersion)
                {
                    case 0:
                        if ((transYIndex + numTransY) > def.m_NumTranslationXValues)
                        {
                            if (transYIndex < def.m_NumTranslationXValues)
                            {
                                for (uint offs = translationYStartOffset; offs < translationYStartOffset + (numTransX - transYIndex) * 4; offs += 4)
                                    def.m_TranslationYValues.Add((int)ovl.Read32(offs) / 4096.0f);
                                for (uint i = numTransX; i < numTransY; i++)
                                    def.m_TranslationYValues.Add(0.0f);
                            }
                            else
                            {
                                def.m_TranslationYValues.Clear();
                                def.m_TranslationYValues.Add(0.0f);
                            }
                            break;
                        }
                        goto case 1;
                    case 1:
                        {
                            for (uint offs = translationYStartOffset; offs < translationYStartOffset + numTransY * 4; offs += 4)
                                def.m_TranslationYValues.Add((int)ovl.Read32(offs) / 4096.0f);
                        }
                        break;
                }
                m_Defs.Add(def);
            }
        }

        public string GetDescription()
        {
            return string.Format("Area: {0}, Frames: {1}", m_Area, m_NumFrames);
        }

        public static List<int> CombineTransformValues(List<List<List<int>>> tripleList)
        {
            List<List<int>> vals = new List<List<int>>();
            tripleList.ForEach(x => vals.AddRange(x));

            vals.RemoveAll(x => x.Count == 0); //get rid of empty animations
            for (int i = vals.Count - 1; i >= 0; --i)
                if (vals.GetRange(0, i).Any(x => x.SequenceEqual(vals[i])))
                    vals.RemoveAt(i);

            List<int> ret = new List<int>();
            vals.ForEach(x => ret.AddRange(x));
            return ret;
            //Further compression can be done, but the algorithm is complicated and not worth it right now.
            //To get a taste of how complicated it would be, look at the palette compression algorithm for
            //a Tex4x4 texture.
        }

        public static void SaveAll(System.IO.BinaryWriter binWriter, List<LevelTexAnim> texAnimList, uint areaTableOffset, uint numAreas)
        {
            List<int> scaleValues = CombineTransformValues(
                texAnimList.Select(x => x.m_Defs.Select(y => y.m_ScaleValuesInt.ToList())
                    .ToList()).ToList());
            List<int> rotationValues   = CombineTransformValues(
                texAnimList.Select(x => x.m_Defs.Select(y => y.m_RotationValuesInt.ToList())
                    .ToList()).ToList());
            List<int> translationValues = CombineTransformValues(
                texAnimList.Select(x => x.m_Defs.Select(y => y.m_CombinedTranslationValuesInt.ToList())
                    .ToList()).ToList());

            uint scaleOffset = (uint)binWriter.BaseStream.Position + Program.m_ROM.LevelOvlOffset;
            scaleValues.ForEach(x => binWriter.Write(x));
            uint rotOffset   = (uint)binWriter.BaseStream.Position + Program.m_ROM.LevelOvlOffset;
            rotationValues  .ForEach(x => binWriter.Write((short)x));
            Helper.AlignWriter(binWriter, 4);
            uint transOffset = (uint)binWriter.BaseStream.Position + Program.m_ROM.LevelOvlOffset;
            translationValues.ForEach(x => binWriter.Write(x));

            foreach (LevelTexAnim texAnim in texAnimList)
            {
                if (texAnim.m_Defs.Count == 0)
                    continue;

                Helper.WritePosAndRestore(binWriter, (uint)(areaTableOffset + texAnim.m_Area * 12 + 4),
                    Program.m_ROM.LevelOvlOffset);

                binWriter.Write(texAnim.m_NumFrames);
                binWriter.Write(scaleOffset);
                binWriter.Write(rotOffset);
                binWriter.Write(transOffset);
                binWriter.Write(texAnim.m_Defs.Count);
                binWriter.Write((uint)binWriter.BaseStream.Position + Program.m_ROM.LevelOvlOffset + 4);

                uint defStrPtrOffset = (uint)binWriter.BaseStream.Position + 0x04;
                foreach (Def def in texAnim.m_Defs)
                {
                    List<int> currScaleValues = def.m_ScaleValuesInt;
                    List<int> currRotationValues = def.m_RotationValuesInt;
                    List<int> currTranslationXValues = def.m_TranslationXValuesInt;
                    List<int> currTranslationYValues = def.m_TranslationYValuesInt;
                    binWriter.Write(0x0000ffff);
                    binWriter.Write(0x00000000);
                    binWriter.Write(0x00000001);
                    binWriter.Write((ushort)def.m_ScaleValues.Count);
                    binWriter.Write((ushort)Helper.FindSubList(scaleValues, currScaleValues));
                    binWriter.Write((ushort)def.m_RotationValues.Count);
                    binWriter.Write((ushort)Helper.FindSubList(rotationValues, currRotationValues));
                    binWriter.Write((ushort)def.m_TranslationXValues.Count);
                    binWriter.Write((ushort)Helper.FindSubList(translationValues, currTranslationXValues));
                    binWriter.Write((ushort)def.m_TranslationYValues.Count);
                    binWriter.Write((ushort)Helper.FindSubList(translationValues, currTranslationYValues));
                }
                foreach (Def def in texAnim.m_Defs)
                {
                    Helper.WritePosAndRestore(binWriter, defStrPtrOffset, Program.m_ROM.LevelOvlOffset);
                    binWriter.Write(def.m_MaterialName.ToCharArray());
                    binWriter.Write((byte)0);
                    defStrPtrOffset += 0x1c;
                }
            }
            
        }

        public int m_Area;
        public uint m_UniqueID;
        
        public uint m_NumFrames;

        public int m_NumScaleValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumScaleValues; } return count; } }
        public int m_NumRotationValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumRotationValues; } return count; } }
        public int m_NumTranslationXValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumTranslationXValues; } return count; } }
        public int m_NumTranslationYValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumTranslationYValues; } return count; } }

        public class Def
        {
            public string m_MaterialName;
            public List<float> m_ScaleValues;
            public List<float> m_RotationValues;
            public List<float> m_TranslationXValues;
            public List<float> m_TranslationYValues;
            public float m_DefaultScale;

            public int m_NumScaleValues { get { return (m_ScaleValues != null) ? m_ScaleValues.Count : 0; } }
            public int m_NumRotationValues { get { return (m_RotationValues != null) ? m_RotationValues.Count : 0; } }
            public int m_NumTranslationXValues { get { return (m_TranslationXValues != null) ? m_TranslationXValues.Count : 0; } }
            public int m_NumTranslationYValues { get { return (m_TranslationYValues != null) ? m_TranslationYValues.Count : 0; } }

            public List<int> m_ScaleValuesInt { get { return Helper.FloatListTo20_12IntList(m_ScaleValues); } }
            public List<int> m_RotationValuesInt { get { return Helper.FloatListToRotationIntList(m_RotationValues); } }
            public List<int> m_TranslationXValuesInt { get { return Helper.FloatListTo20_12IntList(m_TranslationXValues); } }
            public List<int> m_TranslationYValuesInt { get { return Helper.FloatListTo20_12IntList(m_TranslationYValues); } }

            public List<float> m_CombinedTranslationValues
            {
                get
                {
                    List<float> translations = new List<float>();
                    translations.AddRange(m_TranslationXValues);
                    translations.AddRange(m_TranslationYValues);
                    return translations;
                }
            }
            public List<int> m_CombinedTranslationValuesInt { get { return Helper.FloatListTo20_12IntList(m_CombinedTranslationValues); } }
        }

        public List<Def> m_Defs;
        public int m_NumDefs { get { return (m_Defs != null) ? m_Defs.Count : 0; } }
    }


    public class ParameterField
    {

        public String Name;
        public String Description;
        public bool UpdateOnChange;

        public static ParameterField[] ParameterFieldsForObject(LevelObject obj)
        {
            ushort objectID;
            if ((obj is SimpleObject)||(obj is StandardObject)) {
                objectID = obj.ID;
            }
            else
            {
                return new ParameterField[] { };
            }
            ParameterField[] fields = new ParameterField[] { };
            switch(objectID) {
                case 30: //? Block
                    return new ParameterField[] {
                        new ListField("Parameter 1",8,8,new object[]{
                            0,"Coin",
                            1,"Power Star",
                            2,"1 Up Mushroom",
                            3,"Greenshell",
                            4,"Super Mushroom",
                            5,"Feather(Mario), Power Flower(Other)",
                            6,"Power Flower",
                            7,"Lit Bob-omb(Mario), PowerFlower(Other)"
                        }){ Name = "Content" },
                        new DefaultField("Parameter 1",0,8){ Name = "Parameter" }
                    };
                case 42: //Painting
                    return new ParameterField[] {
                        new ListField("Parameter 1",0,3,new object[]{
                            0,"Normal",
                            1,"Always wobbling",
                            2,"Never wobbling",
                            3,"Mirrored"
                        }){ Name = "Painting Mode" },
                        new ListField("Parameter 1",3,5,LevelObject.ComboBoxInfoFromStrings(Strings.PaintingNames)){ Name = "Picture" },
                        new DefaultField("Parameter 1",12,4){ Name = "Width" },
                        new DefaultField("Parameter 1",8,4){ Name = "Height" },
                        new FloatConvertField("Parameter 2",0,16,0x1000,22.5f){ Name = "X Rotation"}
                    };
                case 61: //Power Star
                    return new ParameterField[] {
                        new ListField("Parameter 1",12,4,new object[]{
                            0,"100 Coins",
                            1,"1. Star",
                            2,"2. Star",
                            3,"3. Star",
                            4,"4. Star",
                            5,"5. Star",
                            6,"6. Star",
                            7,"7. Star"
                        }){ Name = "Star" },
                        new ListField("Parameter 1",8,4,new object[]{
                            0,"Normal",
                            1,"Jumping around",
                            2,"100-Coin Star",
                            3,"VS-Star",
                            4,"Just spawned?",
                            5,"Incollectable when already collected?",
                            6,"Silver-Star Star",
                            7,"Minimap only"
                        }){ Name = "Type" }
                    };
                case 63: //Star Marker
                    return new ParameterField[] {
                        new ListField("Parameter 1",12,4,new object[]{
                            0,"100 Coins",
                            1,"1. Star",
                            2,"2. Star",
                            3,"3. Star",
                            4,"4. Star",
                            5,"5. Star",
                            6,"6. Star",
                            7,"7. Star"
                        }){ Name = "Star" },
                        new ListField("Parameter 1",8,4,new object[]{
                            0,"Red Coin Shadow Star",
                            1,"VS-Star Container",
                            2,"Default StarSpawner",
                            4,"StarSphere(No Function?)",
                            6,"SwitchStar",
                            10,"Same as 2?"
                        }){ Name = "Type" }
                    };
                default:
                    return new ParameterField[] { };
            }
        }

        public ParameterField(string pgFieldName, int offset, int length)
        {
            Name = "Unknown";
            Description = "No Description Provided";
            UpdateOnChange = false;

            m_offset = Math.Min(offset, 16);
            m_length = Math.Min(length, 16 - m_offset);
            m_pgFieldName = pgFieldName;
        }

        public virtual ushort getValue() { return (ushort)Math.Pow(2, m_length); }

        public virtual void setValue(object value) { }

        public virtual Control GetControl(LevelEditorForm editorForm)
        {
            if (m_Control == null)
            {
                Label label = new Label();
            }
            return m_Control;
        }

        public Label GetLabel()
        {
            if (m_label == null)
            {
                m_label = new Label()
                {
                    Text = Name,
                    TextAlign = ContentAlignment.MiddleLeft
                };
            }
            return m_label;
        }

        public int m_offset;
        public int m_length;
        public String m_description = "";
        public String m_pgFieldName;
        private Control m_Control;
        private Label m_label;
    }

    public class DefaultField : ParameterField
    {
        public bool DislpayInHex;

        private NumericUpDown m_input;
        private Label m_label;
        public DefaultField(string pgFieldName, int offset, int length)
            : base(pgFieldName,offset, length)
        {
            DislpayInHex = true;
        }

        public override ushort getValue() {
            ushort val = (ushort)m_input.Value;
            ushort bitMask = ((ushort)(Math.Pow(2, m_length) - 1));
            return (ushort)(val & bitMask);
        }

        public override void setValue(object newValue) {
            Decimal maxVal = (Decimal)Math.Pow(2d, m_length);
            m_input.Value = Math.Min(Math.Max(0,Convert.ToDecimal(newValue)),maxVal);
        }

        public override Control GetControl(LevelEditorForm editorForm)
        {
            if (m_input == null)
            {
                m_input = new NumericUpDown
                {
                    Hexadecimal = DislpayInHex,
                    Maximum = ((ushort)(Math.Pow(2, m_length) - 1))
                };
                m_input.ValueChanged += new EventHandler(editorForm.ValueChanged);

                editorForm.defaultToolTip.SetToolTip(this.m_input, Description);
            }
            return m_input;
        }
    }

    public class ListField : ParameterField
    {
        private object[] m_comboBoxInfos;
        private List<ushort> m_valueList;
        private ComboBox m_comboBox;
        private Label m_label;
        public ListField(string pgFieldName, int offset, int length, object[] comboBoxInfos)
            : base(pgFieldName, offset, length)
        {
            m_comboBoxInfos = comboBoxInfos;
            m_valueList = new List<ushort>();
        }

        public override ushort getValue()
        {
            ushort val = m_valueList[m_comboBox.SelectedIndex];
            ushort bitMask = ((ushort)(Math.Pow(2, m_length) - 1));
            return (ushort)(val & bitMask);
        }

        public override void setValue(object value)
        {
            ushort newValue = Convert.ToUInt16(value);

            int index = m_valueList.IndexOf(newValue);

            if (index==-1)
            {
                m_valueList.Add(newValue);
                m_comboBox.Items.Add(newValue + " (unknown)");
                m_comboBox.SelectedIndex = m_comboBox.Items.Count-1;
            } else
            {
                m_comboBox.SelectedIndex = index;
            }
        }

        public override Control GetControl(LevelEditorForm editorForm)
        {
            if (m_comboBox == null)
            {
                System.Boolean needsName = false;
                m_comboBox = new ComboBox();
                
                for (int i = 0; i < m_comboBoxInfos.Length; i++)
                {
                    if (m_comboBoxInfos[i] is int)
                    {
                        m_valueList.Add((ushort)(int)m_comboBoxInfos[i]);
                        needsName = true;
                    } else if(needsName)
                    {
                        if (m_comboBoxInfos[i] is string)
                        {
                            m_comboBox.Items.Add(m_comboBoxInfos[i]);
                        } else
                        {
                            m_comboBox.Items.Add("unknown");
                        }
                        needsName = false;
                    }
                }
                if (needsName)
                {
                    m_comboBox.Items.Add("unknown");
                }
                m_comboBox.DropDownWidth = 300;
                m_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                m_comboBox.SelectedIndexChanged += new EventHandler(editorForm.ValueChanged);
            }
            return m_comboBox;
        }
    }

    public class FloatField : ParameterField
    {
        NumericUpDown m_input;
        public FloatField(string pgFieldname)
            : base(pgFieldname, 0, 0)
        {

        }
        
        public float getFloatValue()
        {
            return (float)m_input.Value;
        }
        
        public void setFloatValue(float newValue)
        {
            m_input.Value = (Decimal)newValue;
        }

        public override Control GetControl(LevelEditorForm editorForm)
        {
            if (m_input == null)
            {
                m_input = new NumericUpDown
                {
                    Maximum = Decimal.MaxValue,
                    DecimalPlaces = 3
                };
                m_input.ValueChanged += new EventHandler(editorForm.ValueChanged);

                editorForm.defaultToolTip.SetToolTip(this.m_input, Description);
            }
            return m_input;
        }
    }

    public class FloatConvertField : ParameterField
    {
        NumericUpDown m_input;
        ushort m_stepInUshort;
        float m_stepInFloat;
        Decimal MaxValue;
        public FloatConvertField(string pgFieldname, int offset, int length, ushort stepInUshort, float stepInFloat)
            : base(pgFieldname, offset, length)
        {
            m_stepInUshort = stepInUshort;
            m_stepInFloat = stepInFloat;

            MaxValue = (Decimal)((Math.Pow(2, m_length) - 1)*(m_stepInFloat/m_stepInUshort)); //this should be overwritten
        }

        public override ushort getValue()
        {
            ushort val = (ushort)(m_input.Value * (Decimal)(m_stepInUshort / m_stepInFloat));
            ushort bitMask = ((ushort)(Math.Pow(2, m_length) - 1));
            return (ushort)(val & bitMask);
        }

        public override void setValue(object newValue)
        {

            Decimal covertedValue = Convert.ToDecimal(newValue) * (Decimal)(m_stepInFloat / m_stepInUshort);

            m_input.Value = Math.Min(Math.Max(0, Convert.ToDecimal(covertedValue)), MaxValue);
        }

        public override Control GetControl(LevelEditorForm editorForm)
        {
            if (m_input == null)
            {
                m_input = new NumericUpDown
                {
                    Increment = (Decimal)m_stepInFloat,
                    Maximum = MaxValue,
                    DecimalPlaces = 3
                };
                m_input.ValueChanged += new EventHandler(editorForm.ValueChanged);

                editorForm.defaultToolTip.SetToolTip(this.m_input, Description);
            }
            return m_input;
        }
    }

}
