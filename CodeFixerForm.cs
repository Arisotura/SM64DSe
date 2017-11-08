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
    public partial class CodeFixerForm : Form
    {
        public CodeFixerForm()
        {
            InitializeComponent();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                txtFile.Text = openFileDialog.FileName;
        }

        private ushort FixOffsetAt(System.IO.BinaryReader fileR, System.IO.BinaryWriter fileW,
            ushort pos, ushort offset, ushort size)
        {
            fileR.BaseStream.Position = pos;
            ushort num = fileR.ReadUInt16();
            if (num >= offset)
            {
                num += size;
                fileW.BaseStream.Position -= 2;
                fileW.Write(num);
            }

            return num;
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            System.IO.FileStream stream = null;
            try
            {
                stream = new System.IO.FileStream(txtFile.Text, System.IO.FileMode.Open);
                System.IO.BinaryReader fileR = new System.IO.BinaryReader(stream);
                System.IO.BinaryWriter fileW = new System.IO.BinaryWriter(stream);

                ushort offset = ushort.Parse(txtOffset.Text, System.Globalization.NumberStyles.HexNumber);
                ushort size = ushort.Parse(txtSize.Text, System.Globalization.NumberStyles.HexNumber);

                ushort vtOffset = FixOffsetAt(fileR, fileW, 0x02, offset, size); //v-table offset
                ushort refOffset = FixOffsetAt(fileR, fileW, 0x06, offset, size); //references to offsets to fix in-game
                FixOffsetAt(fileR, fileW, 0x10, offset, size); //spawn function

                stream.Position = 0x04;
                ushort vtCount = fileR.ReadUInt16();
                stream.Position = 0x08;
                ushort refCount = fileR.ReadUInt16();

                stream.Position = vtOffset;
                for(ushort i = 0; i < vtCount; ++i)
                {
                    uint funcPtr = fileR.ReadUInt32();
                    if ((funcPtr & 0xF0000000) != 0)
                    {
                        FixOffsetAt(fileR, fileW, (ushort)(stream.Position - 4), offset, size);
                        stream.Position += 2;
                    }
                }

                for(ushort i = 0; i < refCount; ++i)
                {
                    stream.Position = refOffset + 2 * i;
                    ushort oldVarOffset = fileR.ReadUInt16();
                    ushort varOffset = FixOffsetAt(fileR, fileW, (ushort)(refOffset + 2 * i), offset, size);

                    //remove references whose referencees (notice: 2 e's in a row)
                    //have been deleted
                    if (varOffset < oldVarOffset && varOffset < offset || varOffset > stream.Length)
                    {
                        stream.Position = refOffset + 2 * i;
                        fileW.Write((ushort)0x0000);
                    }
                    else
                    {
                        stream.Position = varOffset;
                        uint varPtr = fileR.ReadUInt32();
                        if ((varPtr & 0xF0000000) == 0)
                        {
                            FixOffsetAt(fileR, fileW, varOffset, offset, size);
                        }
                    }
                }

                ushort j = 0;
                for (ushort i = 0; i < refCount; ++i, ++j)
                {
                    stream.Position = refOffset + 2 * i;
                    ushort varOffset = fileR.ReadUInt16();

                    if(varOffset == 0x0000)
                    {
                        --j;
                        continue;
                    }

                    stream.Position = refOffset + 2 * j;
                    fileW.Write(varOffset);
                }

                stream.Position = 0x08;
                fileW.Write(j); //new reference count
                stream.SetLength(refOffset + 2 * j);

                stream.Close();
                MessageBox.Show("Operation complete! Now go fix the offsets for loading constants.", "Good!", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                if (stream != null)
                    stream.Close();

                new ExceptionMessageBox("Error", ex).ShowDialog();
                return;
            }
        }
    }
}
