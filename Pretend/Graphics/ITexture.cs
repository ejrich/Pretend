using System;

namespace Pretend.Graphics
{
    public interface ITexture : IDisposable
    {
        void Bind(int slot = 0);
        int Width { get; }
        int Height { get; }
    }

    public interface ITexture2D : ITexture
    {
        void SetData(string file);
        void SetData(IntPtr buffer, int rows, int columns);
        void SetSize(int width, int height);
        void SetSubData(IntPtr buffer, int xOffset, int yOffset, int rows, int columns);
    }
}
