using OpenTK;
using OpenTK.Graphics;

namespace SM64DSe
{
    public class HUDElement
    {
    }

    public class HaxxGLControl : GLControl
    {
        public HaxxGLControl()
            : base(new GraphicsMode(32, 24, 8))
        { }
    }
}
