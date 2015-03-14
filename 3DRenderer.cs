using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SM64DSe
{
    class TextureCache
    {
        public TextureCache()
        {
            m_TextureIDs = new Hashtable();
        }

        public int GetTextureID(BMD.MaterialGroup matgroup)
        {
            if (matgroup.m_Texture == null)
                return 0;

            string texkey = string.Format("{0}|{1}|{2:X8}", matgroup.m_Texture.m_TexName, matgroup.m_Texture.m_PalName, matgroup.m_TexParams);
            if (m_TextureIDs.Contains(texkey))
                return (int)m_TextureIDs[texkey];

            int retid = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, retid);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, (int)matgroup.m_Texture.m_Width, (int)matgroup.m_Texture.m_Height,
                0, PixelFormat.Bgra, PixelType.UnsignedByte, matgroup.m_Texture.m_Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

		    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 
                (int)(((matgroup.m_TexParams & 0x10000) == 0x10000) ? (((matgroup.m_TexParams & 0x40000) == 0x40000) ? 
                TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat) : TextureWrapMode.Clamp));
		    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 
                (int)(((matgroup.m_TexParams & 0x20000) == 0x20000) ? (((matgroup.m_TexParams & 0x80000) == 0x80000) ? 
                TextureWrapMode.MirroredRepeat : TextureWrapMode.Repeat) : TextureWrapMode.Clamp));
            
            m_TextureIDs.Add(texkey, retid);
            return retid;
        }


        private Hashtable m_TextureIDs;
    }

    class ModelPiece
    {
        public ModelPiece(BMD.MaterialGroup matgroup, TextureCache texcache)
        {
            BeginMode[] beginmodes = { BeginMode.Triangles, BeginMode.Quads, BeginMode.TriangleStrip, BeginMode.QuadStrip };

            m_TextureID = texcache.GetTextureID(matgroup);
            m_DispListID = GL.GenLists(1);

            GL.NewList(m_DispListID, ListMode.Compile);

            GL.BindTexture(TextureTarget.Texture2D, m_TextureID);

            foreach (BMD.VertexList vtxlist in matgroup.m_Geometry)
            {
                GL.Begin(beginmodes[vtxlist.m_PolyType]);

                foreach (BMD.Vertex vtx in vtxlist.m_VertexList)
                {
                    GL.Color4(vtx.m_Color);
                    if (m_TextureID != 0) GL.TexCoord2(vtx.m_TexCoord);
                    GL.Vertex3(vtx.m_Position);
                }

                GL.End();
            }

            GL.EndList();
        }

        public void Render()
        {
            GL.CallList(m_DispListID);
        }


        private int m_TextureID;
        private int m_DispListID;
    }
}
