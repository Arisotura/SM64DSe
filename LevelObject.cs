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

namespace SM64DSe
{
    // not quite the right place but whatever
    public class LevelSettings
    {
        public LevelSettings(NitroOverlay ovl)
        {
            m_Overlay = ovl;
            Background = (byte)(m_Overlay.Read8(0x78) >> 4);
            ObjectBanks = new uint[8];
            ObjectBanks[0] = m_Overlay.Read8(0x54);
            ObjectBanks[1] = m_Overlay.Read8(0x55);
            ObjectBanks[2] = m_Overlay.Read8(0x56);
            ObjectBanks[3] = m_Overlay.Read8(0x57);
            ObjectBanks[4] = m_Overlay.Read8(0x58);
            ObjectBanks[5] = m_Overlay.Read8(0x59);
            ObjectBanks[6] = m_Overlay.Read8(0x5A);
            ObjectBanks[7] = m_Overlay.Read32(0x5C);

            BMDFileID = m_Overlay.Read16(0x68);
            KCLFileID = m_Overlay.Read16(0x6A);
            MinimapTsetFileID = m_Overlay.Read16(0x6C);
            MinimapPalFileID = m_Overlay.Read16(0x6E);

            MusicBytes = new byte[3];
            MusicBytes[0] = m_Overlay.Read8(0x7C);
            MusicBytes[1] = m_Overlay.Read8(0x7D);
            MusicBytes[2] = m_Overlay.Read8(0x7E);

            MinimapCoordinateScale = m_Overlay.Read16(0x76);
            CameraStartZoomedOut = m_Overlay.Read8(0x75);
        }

        public void SaveChanges()
        {
            m_Overlay.Write8(0x78, (byte)(0xF | (Background << 4)));
            m_Overlay.Write8(0x54, (byte)ObjectBanks[0]);
            m_Overlay.Write8(0x55, (byte)ObjectBanks[1]);
            m_Overlay.Write8(0x56, (byte)ObjectBanks[2]);
            m_Overlay.Write8(0x57, (byte)ObjectBanks[3]);
            m_Overlay.Write8(0x58, (byte)ObjectBanks[4]);
            m_Overlay.Write8(0x59, (byte)ObjectBanks[5]);
            m_Overlay.Write8(0x5A, (byte)ObjectBanks[6]);
            m_Overlay.Write32(0x5C, (uint)ObjectBanks[7]);

            m_Overlay.Write8(0x7C, MusicBytes[0]);
            m_Overlay.Write8(0x7D, MusicBytes[1]);
            m_Overlay.Write8(0x7E, MusicBytes[2]);

            m_Overlay.Write16(0x76, MinimapCoordinateScale);
            m_Overlay.Write8(0x75, CameraStartZoomedOut);
        }

        public NitroOverlay m_Overlay;
        public byte Background;
        public uint[] ObjectBanks;
        public ushort BMDFileID, KCLFileID, MinimapTsetFileID, MinimapPalFileID;
        public byte[] MusicBytes;
        public ushort MinimapCoordinateScale;
        public byte CameraStartZoomedOut;
        public byte ActSelectorID;// NOT stored in the overlay - not possible
    }


    public class LevelObject
    {
        public static string[] k_Layers = { "(all stars)", "(star 1)", "(star 2)", "(star 3)", "(star 4)", "(star 5)", "(star 6)", "(star 7)" };

        public LevelObject(NitroOverlay ovl, uint offset, int layer)
        {
            m_Overlay = ovl;
            m_Offset = offset;
            m_Layer = layer;
            m_Area = 0;

            //m_TestMatrix = new Matrix4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        public virtual void GenerateProperties() { }

        public virtual string GetDescription() { return "LevelObject"; }

        public virtual bool SupportsRotation() { return true; }
        // return value: bit0=refresh display, bit1=refresh propertygrid, bit2=refresh object list
        public virtual int SetProperty(string field, object newval) { return 0; }
        public virtual void SaveChanges() { }

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

        //public Matrix4 m_TestMatrix;

        public virtual ObjectRenderer InitialiseRenderer() { return null; }

        public virtual void Release()
        {
            m_Renderer.Release();
        }

        public virtual LevelObject Copy()
        {
            LevelObject copy = (LevelObject)MemberwiseClone();
            copy.GenerateProperties();
            copy.m_Renderer = copy.InitialiseRenderer();

            return copy;
        }
        

        public ushort ID;
        public Vector3 Position;
        public float YRotation;

        // object specific parameters
        // for standard objects: [0] = 16bit object param, [1] and [2] = what should be X and Z rotation
        // for simple objects: [0] = 7bit object param
        public ushort[] Parameters;

        public NitroOverlay m_Overlay;
        public uint m_Offset;
        public int m_Layer;
        public int m_Area;
        public uint m_UniqueID;
        public int m_Type;

        public ObjectRenderer m_Renderer;
        public PropertyTable m_Properties;
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

        public StandardObject(NitroOverlay ovl, uint offset, int num, int layer, int area)
            : base(ovl, offset, layer)
        {
            m_Area = area;
            m_UniqueID = (uint)(0x10000000 | num);
            m_Type = 0;

            ID = m_Overlay.Read16(m_Offset);
            Position.X = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x6)) / 1000f;
            YRotation = ((float)((short)m_Overlay.Read16(m_Offset + 0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[3];
            Parameters[0] = m_Overlay.Read16(m_Offset + 0xE);
            Parameters[1] = m_Overlay.Read16(m_Offset + 0x8);
            Parameters[2] = m_Overlay.Read16(m_Offset + 0xC);

            m_Renderer = InitialiseRenderer();
            // m_PParams = new Hashtable();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
        {
            return ObjectRenderer.FromLevelObject(this);
        }

        public override string GetDescription()
        {
            return String.Format("{0} - {1} {2}", ID, ObjectDatabase.m_ObjectInfo[ID].m_Name, k_Layers[m_Layer]);
        }

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
                    //return 3;
                }

                if (field == "Object ID")
                    return 5;
            }

            return 1;
        }

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset, ID);
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.X * 1000f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Y * 1000f)));
            m_Overlay.Write16(m_Offset + 0x6, (ushort)((short)(Position.Z * 1000f)));
            m_Overlay.Write16(m_Offset + 0xA, (ushort)((short)((YRotation / 22.5f) * 4096f)));

            m_Overlay.Write16(m_Offset + 0xE, Parameters[0]);
            m_Overlay.Write16(m_Offset + 0x8, Parameters[1]);
            m_Overlay.Write16(m_Offset + 0xC, Parameters[2]);
        }
    }


    public class SimpleObject : LevelObject
    {
        public SimpleObject(NitroOverlay ovl, uint offset, int num, int layer, int area)
            : base(ovl, offset, layer)
        {
            m_Area = area;
            m_UniqueID = (uint)(0x10000000 | num);
            m_Type = 5;

            ushort idparam = m_Overlay.Read16(m_Offset);
            ID = (ushort)(idparam & 0x1FF);
            Position.X = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x6)) / 1000f;
            YRotation = 0.0f;

            Parameters = new ushort[1];
            Parameters[0] = (ushort)(idparam >> 9);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
        {
            return ObjectRenderer.FromLevelObject(this);
        }

        public override string GetDescription()
        {
            if (ID == 511) return "511 - Minimap change " + k_Layers[m_Layer];
            return String.Format("{0} - {1} {2}", ID, ObjectDatabase.m_ObjectInfo[ID].m_Name, k_Layers[m_Layer]);
        }

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
            }

            if (field == "Object ID")
                return 5;

            return 1;
        }

        public override void SaveChanges()
        {
            ushort idparam = (ushort)((ID & 0x1FF) | (Parameters[0] << 9));
            m_Overlay.Write16(m_Offset, idparam);
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.X * 1000f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Y * 1000f)));
            m_Overlay.Write16(m_Offset + 0x6, (ushort)((short)(Position.Z * 1000f)));
        }
    }

    public class EntranceObject : LevelObject
    {
        public int m_EntranceID;

        public EntranceObject(NitroOverlay ovl, uint offset, int num, int layer, int id)
            : base(ovl, offset, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_EntranceID = id;
            m_Type = 1;

            ID = 0;
            Position.X = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000.0f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000.0f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x6)) / 1000.0f;
            YRotation = ((float)((short)m_Overlay.Read16(m_Offset + 0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[5];
            Parameters[0] = m_Overlay.Read16(m_Offset + 0x0);
            Parameters[1] = m_Overlay.Read16(m_Offset + 0x8);
            Parameters[2] = m_Overlay.Read16(m_Offset + 0xC);
            Parameters[3] = m_Overlay.Read16(m_Offset + 0xE);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
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
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "Purpose unknown.", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Purpose unknown.", Parameters[1], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(ushort), "Specific", "Purpose unknown.", Parameters[2], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 4", typeof(ushort), "Specific", "AABC: A - Entrance mode, B - View ID, C - Purpose unknown", Parameters[3], "", typeof(HexNumberTypeConverter)));

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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x6, (ushort)((short)(Position.Z * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0xA, (ushort)((short)((YRotation / 22.5f) * 4096f)));

            m_Overlay.Write16(m_Offset + 0x0, Parameters[0]);
            m_Overlay.Write16(m_Offset + 0x8, Parameters[1]);
            m_Overlay.Write16(m_Offset + 0xC, Parameters[2]);
            m_Overlay.Write16(m_Offset + 0xE, Parameters[3]);
        }
    }

    public class ExitObject : LevelObject
    {
        public int LevelID, EntranceID;
        public ushort Param1, Param2;

        public ExitObject(NitroOverlay ovl, uint offset, int num, int layer)
            : base(ovl, offset, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = 10;

            Position.X = (float)((short)m_Overlay.Read16(m_Offset)) / 1000.0f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000.0f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000.0f;
            YRotation = ((float)((short)m_Overlay.Read16(m_Offset + 0x8)) / 4096f) * 22.5f;

            LevelID = m_Overlay.Read8(m_Offset + 0xA);
            EntranceID = m_Overlay.Read8(m_Offset + 0xB);
            Param1 = m_Overlay.Read16(m_Offset + 0x6);
            Param2 = m_Overlay.Read16(m_Offset + 0xC);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(255, 0, 0), Color.FromArgb(64, 0, 0), true);
        }

        public override string GetDescription()
        {
            return string.Format("Exit ({0}, entrance {1}) {2}", Strings.LevelNames[LevelID], EntranceID, k_Layers[m_Layer]);
        }

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
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "Purpose unknown.", Param1, "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Purpose unknown.", Param2, "", typeof(HexNumberTypeConverter)));

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
                case "Parameter 1": Param1 = (ushort)newval; return 0;
                case "Parameter 2": Param2 = (ushort)newval; return 0;
            }

            return 0;
        }

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x0, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Z * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x8, (ushort)((short)((YRotation / 22.5f) * 4096f)));

            m_Overlay.Write8(m_Offset + 0xA, (byte)LevelID);
            m_Overlay.Write8(m_Offset + 0xB, (byte)EntranceID);
            m_Overlay.Write16(m_Offset + 0x6, Param1);
            m_Overlay.Write16(m_Offset + 0xC, Param2);
        }
    }

    public class DoorObject : LevelObject
    {
        public int DoorType, OutAreaID, InAreaID, PlaneSizeX, PlaneSizeY;

        public DoorObject(NitroOverlay ovl, uint offset, int num, int layer)
            : base(ovl, offset, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = 9;

            ID = 0;
            Position.X = (float)((short)m_Overlay.Read16(m_Offset + 0x0)) / 1000f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000f;
            YRotation = ((float)((short)m_Overlay.Read16(m_Offset + 0x6)) / 4096f) * 22.5f;

            DoorType = m_Overlay.Read8(m_Offset + 0xA);
            if (DoorType > 0x17) DoorType = 0x17;

            InAreaID = m_Overlay.Read8(m_Offset + 0x9);
            OutAreaID = InAreaID >> 4;
            InAreaID &= 0xF;

            PlaneSizeX = m_Overlay.Read8(m_Offset + 0x8);
            PlaneSizeY = PlaneSizeX >> 4;
            PlaneSizeX &= 0xF;

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x0, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Z * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x6, (ushort)((short)((YRotation / 22.5f) * 4096f)));

            m_Overlay.Write8(m_Offset + 0x8, (byte)(PlaneSizeX | (PlaneSizeY << 4)));
            m_Overlay.Write8(m_Offset + 0x9, (byte)(InAreaID | (OutAreaID << 4)));
            m_Overlay.Write8(m_Offset + 0xA, (byte)DoorType);
        }
    }

    public class PathPointObject : LevelObject
    {
        public PathPointObject(NitroOverlay ovl, uint offset, int num, int nodeID)
            : base(ovl, offset, 0)
        {
            m_UniqueID = (uint)(0x30000000 | num);
            m_Type = 2;
            m_NodeID = nodeID;

            Position.X = (float)((short)m_Overlay.Read16(m_Offset)) / 1000f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000f;

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }
        
        public int m_NodeID;

        public override ObjectRenderer InitialiseRenderer()
        {
            return new ColourArrowRenderer(Color.FromArgb(0, 255, 255), Color.FromArgb(0, 64, 64), false);
        }

        public override string GetDescription()
        {
            return "PathPointObject";
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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x0, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Z * 1000.0f)));
        }
    }

    public class PathObject : LevelObject
    {
        public PathObject(NitroOverlay ovl, uint offset, int num)
            : base(ovl, offset, 0)
        {
            m_UniqueID = (uint)(0x30000000 | num);
            m_Type = 3;

            Parameters = new ushort[5];
            Parameters[0] = m_Overlay.Read16(m_Offset);
            Parameters[1] = (ushort)m_Overlay.Read8(m_Offset + 0x2);
            Parameters[2] = (ushort)m_Overlay.Read8(m_Offset + 0x3);
            Parameters[3] = (ushort)m_Overlay.Read8(m_Offset + 0x4);
            Parameters[4] = (ushort)m_Overlay.Read8(m_Offset + 0x5);

            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "PathObject";
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("Start Node", typeof(float), "General", "Index of starting node.", (float)Parameters[0], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Length", typeof(float), "General", "Number of nodes in path.", (float)Parameters[1], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(float), "General", "Unknown", (float)Parameters[2], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 4", typeof(float), "General", "Unknown", (float)Parameters[3], "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 5", typeof(float), "General", "Unknown", (float)Parameters[4], "", typeof(FloatTypeConverter)));

            m_Properties["Start Node"] = (float)Parameters[0];
            m_Properties["Length"] = (float)Parameters[1];
            m_Properties["Parameter 3"] = (float)Parameters[2];
            m_Properties["Parameter 4"] = (float)Parameters[3];
            m_Properties["Parameter 5"] = (float)Parameters[4];
        }

        public override bool SupportsRotation() { return false; }

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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x00, Parameters[0]);
            m_Overlay.Write8(m_Offset + 0x02, (byte)Parameters[1]);
            m_Overlay.Write8(m_Offset + 0x03, (byte)Parameters[2]);
            m_Overlay.Write8(m_Offset + 0x04, (byte)Parameters[3]);
            m_Overlay.Write8(m_Offset + 0x05, (byte)Parameters[4]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class ViewObject : LevelObject
    {
        public ViewObject(NitroOverlay ovl, uint offset, int num)
            : base(ovl, offset, 0)
        {
            m_UniqueID = (uint)(0x40000000 | num);
            m_Type = 4;

            Position.X = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000.0f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000.0f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x6)) / 1000.0f;
            YRotation = ((float)((short)m_Overlay.Read16(m_Offset + 0xA)) / 4096f) * 22.5f;

            Parameters = new ushort[3];
            Parameters[0] = m_Overlay.Read16(m_Offset + 0x0);
            Parameters[1] = m_Overlay.Read16(m_Offset + 0x8);
            Parameters[2] = m_Overlay.Read16(m_Offset + 0xC);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable(); 
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
        {
            return new ColorCubeRenderer(Color.FromArgb(255, 255, 0), Color.FromArgb(64, 64, 0), true);
        }

        public override string GetDescription()
        {
            return "View";
        }

        public override void GenerateProperties()
        {
            m_Properties.Properties.Clear();

            m_Properties.Properties.Add(new PropertySpec("X position", typeof(float), "General", "The view's position along the X axis.", Position.X, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y position", typeof(float), "General", "The view's position along the Y axis.", Position.Y, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Z position", typeof(float), "General", "The view's position along the Z axis.", Position.Z, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Y rotation", typeof(float), "General", "The angle in degrees the view is rotated around the Y axis.", YRotation, "", typeof(FloatTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 1", typeof(ushort), "Specific", "Purpose unknown.", Parameters[0], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 2", typeof(ushort), "Specific", "Purpose unknown.", Parameters[1], "", typeof(HexNumberTypeConverter)));
            m_Properties.Properties.Add(new PropertySpec("Parameter 3", typeof(ushort), "Specific", "Purpose unknown.", Parameters[2], "", typeof(HexNumberTypeConverter)));

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

        public override void SaveChanges()
        {
             m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.X * 1000.0f)));
             m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Y * 1000.0f)));
             m_Overlay.Write16(m_Offset + 0x6, (ushort)((short)(Position.Z * 1000.0f)));
             m_Overlay.Write16(m_Offset + 0xA, (ushort)((short)((YRotation / 22.5f) * 4096f)));

             m_Overlay.Write16(m_Offset + 0x0, Parameters[0]);
             m_Overlay.Write16(m_Offset + 0x8, Parameters[1]);
             m_Overlay.Write16(m_Offset + 0xC, Parameters[2]);
        }
    }

    public class TpSrcObject : LevelObject
    {
        public TpSrcObject(NitroOverlay ovl, uint offset, int num, int layer)
            : base(ovl, offset, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = 6;

            Position.X = (float)((short)m_Overlay.Read16(m_Offset)) / 1000.0f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000.0f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000.0f;
            YRotation = 0.0f;

            Parameters = new ushort[2];
            Parameters[0] = m_Overlay.Read8(m_Offset + 0x6);
            Parameters[1] = m_Overlay.Read8(m_Offset + 0x07);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x0, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Z * 1000.0f)));

            m_Overlay.Write8(m_Offset + 0x6, (byte)Parameters[0]);
            m_Overlay.Write8(m_Offset + 0x7, (byte)Parameters[1]);
        }
    }

    public class TpDstObject : LevelObject
    {
        public TpDstObject(NitroOverlay ovl, uint offset, int num, int layer)
            : base(ovl, offset, layer)
        {
            m_UniqueID = (uint)(0x20000000 | num);
            m_Type = 7;

            Position.X = (float)((short)m_Overlay.Read16(m_Offset)) / 1000.0f;
            Position.Y = (float)((short)m_Overlay.Read16(m_Offset + 0x2)) / 1000.0f;
            Position.Z = (float)((short)m_Overlay.Read16(m_Offset + 0x4)) / 1000.0f;
            YRotation = 0.0f;

            Parameters = new ushort[1];
            Parameters[0] = m_Overlay.Read16(m_Offset + 0x6);

            m_Renderer = InitialiseRenderer();
            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override ObjectRenderer InitialiseRenderer()
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

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset + 0x0, (ushort)((short)(Position.X * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x2, (ushort)((short)(Position.Y * 1000.0f)));
            m_Overlay.Write16(m_Offset + 0x4, (ushort)((short)(Position.Z * 1000.0f)));

            m_Overlay.Write16(m_Offset + 0x6, Parameters[0]);
        }
    }

    public class MinimapScaleObject : LevelObject
    {
        public MinimapScaleObject(NitroOverlay ovl, uint offset, int num, int layer, int area)
            : base(ovl, offset, layer)
        {
            m_Type = 12;
            m_Area = area;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[1];
            Parameters[0] = m_Overlay.Read16(m_Offset);

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

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Scale": Parameters[0] = (ushort)((float)newval * 1000f); break;
            }

            return 0;
        }

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset, (ushort)Parameters[0]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class FogObject : LevelObject
    {
        public FogObject(NitroOverlay ovl, uint offset, int num, int layer, int area)
            : base(ovl, offset, layer)
        {
            m_Area = area;
            m_Type = 8;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[6];
            Parameters[0] = m_Overlay.Read8(m_Offset);
            Parameters[1] = m_Overlay.Read8(m_Offset + 1);
            Parameters[2] = m_Overlay.Read8(m_Offset + 2);
            Parameters[3] = m_Overlay.Read8(m_Offset + 3);
            Parameters[4] = m_Overlay.Read16(m_Offset + 4);
            Parameters[5] = m_Overlay.Read16(m_Offset + 6);

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

        public override void SaveChanges()
        {
            m_Overlay.Write8(m_Offset, (byte)Parameters[0]);
            m_Overlay.Write8(m_Offset + 1, (byte)Parameters[1]);
            m_Overlay.Write8(m_Offset + 2, (byte)Parameters[2]);
            m_Overlay.Write8(m_Offset + 3, (byte)Parameters[3]);
            m_Overlay.Write16(m_Offset + 4, (ushort)Parameters[4]);
            m_Overlay.Write16(m_Offset + 6, (ushort)Parameters[5]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class Type14Object : LevelObject
    {
        public Type14Object(NitroOverlay ovl, uint offset, int num, int layer, int area)
            : base(ovl, offset, layer)
        {
            m_Area = area;
            m_Type = 14;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[4];
            Parameters[0] = m_Overlay.Read8(m_Offset);
            Parameters[1] = m_Overlay.Read8(m_Offset + 1);
            Parameters[2] = m_Overlay.Read8(m_Offset + 2);
            Parameters[3] = m_Overlay.Read8(m_Offset + 3);

            m_Properties = new PropertyTable();
            GenerateProperties();
        }

        public override string GetDescription()
        {
            return "Unknown Type 14 Object";
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

        public override void SaveChanges()
        {
            m_Overlay.Write8(m_Offset, (byte)Parameters[0]);
            m_Overlay.Write8(m_Offset + 1, (byte)Parameters[1]);
            m_Overlay.Write8(m_Offset + 2, (byte)Parameters[2]);
            m_Overlay.Write8(m_Offset + 3, (byte)Parameters[3]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }

    public class MinimapTileIDObject : LevelObject
    {
        public int m_MinimapTileIDNum;

        public MinimapTileIDObject(NitroOverlay ovl, uint offset, int num, int layer, int id)
            : base(ovl, offset, layer)
        {
            m_Type = 11;
            m_MinimapTileIDNum = id;
            m_UniqueID = (uint)(0x50000000 | num);

            Parameters = new ushort[2];
            Parameters[0] = m_Overlay.Read16(m_Offset);

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

        public override int SetProperty(string field, object newval)
        {
            switch (field)
            {
                case "Tile ID": Parameters[0] = (ushort)(float)newval; break;
            }

            return 0;
        }

        public override void SaveChanges()
        {
            m_Overlay.Write16(m_Offset, (ushort)Parameters[0]);
        }

        public override void Render(RenderMode mode) { }
        public override void Release() { }
    }


    public class LevelTexAnim
    {
        public LevelTexAnim(NitroOverlay ovl, uint tbloffset, uint offset, int num, int area)
        {
            m_Overlay = ovl;
            // Address of the animation data
            m_Offset = offset;
            m_UniqueID = (uint)num;
            m_Area = area;
            // Address of the texture animation data header for this texture animation
            m_TexAnimHeaderOffset = tbloffset;

            // Addresses at which the scale, rotation and translation data for this texture animation starts
            m_ScaleTblOffset = m_Overlay.ReadPointer(tbloffset + 0x4) + (uint)(m_Overlay.Read16(m_Offset + 0xE) * 4);
            m_RotTblOffset = m_Overlay.ReadPointer(tbloffset + 0x8) + (uint)(m_Overlay.Read16(m_Offset + 0x12) * 2);
            m_TransTblOffset = m_Overlay.ReadPointer(tbloffset + 0xC) + (uint)(m_Overlay.Read16(m_Offset + 0x16) * 4);
            
            // Addresses of the shared scale, rotation and translation tables
            m_BaseScaleTblAddr = m_Overlay.ReadPointer(tbloffset + 0x4);
            m_BaseRotTblAddr = m_Overlay.ReadPointer(tbloffset + 0x8);
            m_BaseTransTblAddr = m_Overlay.ReadPointer(tbloffset + 0xC);

            m_MatNameOffset = m_Overlay.ReadPointer(m_Offset + 0x4);
        }

        // Using getters for non-offset values so that these don't need updated every time a change is made eg. new scale value 
        // inserted, the value will be written to the address only and the field updated on each get()

        public uint getNumFrames()
        {
            m_NumFrames = m_Overlay.Read32(m_TexAnimHeaderOffset + 0x00);
            return m_NumFrames;
        }

        public int getNumTexAnims()
        {
            m_NumTexAnims = (int)m_Overlay.Read32(m_TexAnimHeaderOffset + 0x10);
            return m_NumTexAnims;
        }

        public uint getScaleTblSize()
        {
            m_ScaleTblSize = m_Overlay.Read16(m_Offset + 0xC);
            return m_ScaleTblSize;
        }

        public uint getScaleTblStart()
        {
            m_ScaleTblStart = m_Overlay.Read16(m_Offset + 0xE);
            return m_ScaleTblStart;
        }

        public uint getRotTblSize()
        {
            m_RotTblSize = m_Overlay.Read16(m_Offset + 0x10);
            return m_RotTblSize;
        }

        public uint getRotTblStart()
        {
            m_RotTblStart = m_Overlay.Read16(m_Offset + 0x12);
            return m_RotTblStart;
        }

        public uint getTransTblSize()
        {
            m_TransTblSize = m_Overlay.Read16(m_Offset + 0x14);
            return m_TransTblSize;
        }

        public uint getTransTblStart()
        {
            m_TransTblStart = m_Overlay.Read16(m_Offset + 0x16);
            return m_TransTblStart;
        }

        public string getMatName()
        {
            m_MatName = m_Overlay.ReadString(m_MatNameOffset, 0);
            return m_MatName;
        }

        public string GetDescription()
        {
            return string.Format("{0} (S:{1} R:{2} T:{3})", m_MatName, m_ScaleTblSize, m_RotTblSize, m_TransTblSize);
        }

        public void SaveChanges()
        {
        }

        public NitroOverlay m_Overlay;
        public uint m_Offset;
        public uint m_TexAnimHeaderOffset;

        public int m_Area;
        public uint m_UniqueID;

        private int m_NumTexAnims;
        private uint m_NumFrames;

        public uint m_BaseScaleTblAddr;
        public uint m_BaseRotTblAddr;
        public uint m_BaseTransTblAddr;

        public uint m_ScaleTblOffset;
        public uint m_RotTblOffset;
        public uint m_TransTblOffset;

        private ushort m_ScaleTblSize;
        private ushort m_RotTblSize;
        private ushort m_TransTblSize;
        private ushort m_ScaleTblStart;
        private ushort m_RotTblStart;
        private ushort m_TransTblStart;

        public uint m_MatNameOffset;
        private string m_MatName;
    }
}
