using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe
{
    static class KCLCache
    {
        public static KCL GetKCL(string name)
        {
            if (m_Clsns.ContainsKey(name))
            {
                CachedKCL found = m_Clsns[name];
                found.m_References++;
                return found.m_Clsn;
            }

            if (!Program.m_ROM.FileExists(name))
                return null;
            NitroFile kclfile = Program.m_ROM.GetFileFromName(name);

            KCL clsn = new KCL(kclfile);

            CachedKCL ckcl = new CachedKCL();
            ckcl.m_Clsn = clsn;
            ckcl.m_References = 1;
            m_Clsns.Add(name, ckcl);

            return clsn;
        }

        public static void RemoveKCL(KCL clsn)
        {
            if (!m_Clsns.ContainsKey(clsn.m_File.m_Name))
                return;

            RemoveKCL(clsn.m_File.m_Name);
        }

        public static void RemoveKCL(string clsnName)
        {
            if (!m_Clsns.ContainsKey(clsnName))
                return;

            CachedKCL ckcl = m_Clsns[clsnName];

            ckcl.m_References--;
            if (ckcl.m_References > 0)
                return;

            m_Clsns.Remove(clsnName);
        }


        private class CachedKCL
        {
            public KCL m_Clsn;
            public int m_References;
        }

        private static Dictionary<string, CachedKCL> m_Clsns = new Dictionary<string, CachedKCL>();
    }
}