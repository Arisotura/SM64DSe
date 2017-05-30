using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace SM64DSe.ImportExport.Writers
{
    public abstract class AbstractModelWriter
    {
        public ModelBase m_Model;

        protected string m_ModelFileName;
        protected string m_ModelPath;

        public AbstractModelWriter(ModelBase model, string modelFileName)
        {
            m_Model = model;

            m_ModelFileName = modelFileName;
            m_ModelPath = Path.GetDirectoryName(m_ModelFileName);
        }

        public abstract void WriteModel(bool save = true);

        protected static void ExportTextureToPNG(string destDir, ModelBase.TextureDefBase texture)
        {
            try
            {
                ExportTextureToPNG(destDir, texture.m_ID, texture.GetBitmap());
            }
            catch (IOException)
            {
                Console.Write("Cannot write image for texture: " + texture.m_ID);
            }
        }

        protected static void ExportTextureToPNG(string destDir, string destName, Bitmap lol)
        {
            //Export the current texture to .PNG
            try
            {
                lol.Save(destDir + "/" + destName + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while trying to save texture " + destName + ".\n\n " +
                    e.Message + "\n" + e.Data + "\n" + e.StackTrace + "\n" + e.Source);
            }
        }
    }
}
