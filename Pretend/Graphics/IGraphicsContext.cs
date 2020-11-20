using System;

namespace Pretend.Graphics
{
    public interface IGraphicsContext
    {
        void CreateContext(IntPtr windowPointer);
        void UpdateWindow();
        void DeleteContext();
        bool Vsync { set; }
    }
}
