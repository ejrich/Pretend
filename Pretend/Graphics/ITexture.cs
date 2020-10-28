using System;

namespace Pretend.Graphics
{
    public interface ITexture
    {
        void Bind(int slot = 0);
    }

    public interface ITexture2D : ITexture
    {
        void SetData(string file);
        public void SetData(IntPtr buffer, int rows, int columns);
    }
}
