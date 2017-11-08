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
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe
{
    static class ModelCache
    {
        public static BMD GetModel(string name)
        {
            if (Program.m_ROM == null)
            {
                return null;
            }

            if (m_Models.ContainsKey(name))
            {
                CachedModel found = m_Models[name];
                found.m_References++;
                return found.m_Model;
            }

            NitroFile mdfile = Program.m_ROM.GetFileFromName(name);
            if (mdfile == null)
                return null;

            BMD model = new BMD(mdfile);
            model.PrepareToRender();

            CachedModel cmdl = new CachedModel();
            cmdl.m_Model = model;
            cmdl.m_DisplayLists = null;
            cmdl.m_References = 1;
            m_Models.Add(name, cmdl);

            return model;
        }

        public static int[] GetDisplayLists(BMD model)
        {
            if (model == null || !m_Models.ContainsKey(model.m_FileName))
                return null;

            CachedModel cmdl = m_Models[model.m_FileName];
            if (cmdl.m_DisplayLists != null)
                return cmdl.m_DisplayLists;

            int[] dl = new int[3];
            bool keep = false;

            dl[0] = GL.GenLists(1);
            GL.NewList(dl[0], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Opaque, 1f);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[0], 1); dl[0] = 0; }

            dl[1] = GL.GenLists(1);
            GL.NewList(dl[1], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Translucent, 1f);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[1], 1); dl[1] = 0; }

            dl[2] = GL.GenLists(1);
            GL.NewList(dl[2], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Picking, 1f);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[2], 1); dl[2] = 0; }

            cmdl.m_DisplayLists = dl;
            return dl;
        }

        public static int[] GetDisplayLists(BMD model,BCA animation, int frame)
        {
            if (model == null || !m_Models.ContainsKey(model.m_FileName))
                return null;

            CachedModel cmdl = m_Models[model.m_FileName];
            if (cmdl.m_DisplayLists != null)
                return cmdl.m_DisplayLists;

            int[] dl = new int[3];
            bool keep = false;

            dl[0] = GL.GenLists(1);
            GL.NewList(dl[0], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Opaque, 1f, animation, frame);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[0], 1); dl[0] = 0; }

            dl[1] = GL.GenLists(1);
            GL.NewList(dl[1], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Translucent, 1f, animation, frame);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[1], 1); dl[1] = 0; }

            dl[2] = GL.GenLists(1);
            GL.NewList(dl[2], ListMode.Compile);
            keep = cmdl.m_Model.Render(RenderMode.Picking, 1f, animation, frame);
            GL.EndList();
            if (!keep) { GL.DeleteLists(dl[2], 1); dl[2] = 0; }

            cmdl.m_DisplayLists = dl;
            return dl;
        }

        public static void RemoveModel(BMD model)
        {
            if (!m_Models.ContainsKey(model.m_FileName))
                return;

            RemoveModel(model.m_FileName);
        }

        public static void RemoveModel(string modelName)
        {
            if (!m_Models.ContainsKey(modelName))
                return;

            CachedModel cmdl = m_Models[modelName];

            cmdl.m_References--;
            if (cmdl.m_References > 0)
                return;

            if (cmdl.m_DisplayLists != null)
            {
                GL.DeleteLists(cmdl.m_DisplayLists[0], 1);
                GL.DeleteLists(cmdl.m_DisplayLists[1], 1);
                GL.DeleteLists(cmdl.m_DisplayLists[2], 1);
                cmdl.m_DisplayLists = null;
            }

            cmdl.m_Model.Release();

            m_Models.Remove(modelName);
        }


        private class CachedModel
        {
            public BMD m_Model;
            public int[] m_DisplayLists;
            public int m_References;
        }

        private static Dictionary<string, CachedModel> m_Models = new Dictionary<string, CachedModel>();
    }
}
