using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace SM64DSe.FormControls
{
    public class SelectionHighlightGLControl : OpenTK.GLControl
    {
        public SelectionHighlightGLControl() : base(new GraphicsMode(new ColorFormat(32), 24, 8)) { }
    }
}
