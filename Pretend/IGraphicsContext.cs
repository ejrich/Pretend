using System;

namespace Pretend
{
    public interface IGraphicsContext
    {
        void CreateContext(IntPtr windowPointer);
        void UpdateWindow();
        void DeleteContext();
    }
}
