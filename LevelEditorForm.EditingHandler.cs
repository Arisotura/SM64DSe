using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
	public partial class LevelEditorForm
	{
        private enum EditingMode
        {
            Model = 1,
            Objects,
            Warps,
            Paths,
            Views,
            Misc
        }

        class EditingHandler
        {
            public virtual bool View_OnMouseDown(MouseEventArgs e) { return false; }
            public virtual bool View_OnMouseUp(MouseEventArgs e) { return false; }
            public virtual bool View_OnMouseHover(MouseEventArgs e) { return false; }
            public virtual bool View_OnMouseDrag(MouseEventArgs e) { return false; }
        }
	}
}
