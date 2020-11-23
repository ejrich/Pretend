using System;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Pretend.Graphics.OpenGL
{
    public class Texture2D : ITexture2D
    {
        private readonly int _id;
        private bool _disposed;

        public Texture2D() => _id = GL.GenTexture();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void SetData(string file)
        {
            Bind();

            using (var image = new Bitmap(file))
            {
                Width = image.Width;
                Height = image.Height;
                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, DrawingPixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            SetTextureParameters(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Repeat);
        }

        public void SetData(IntPtr buffer, int rows, int columns)
        {
            Bind();

            Width = columns;
            Height = rows;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
                columns, rows, 0, PixelFormat.Red, PixelType.UnsignedByte, buffer);

            SetTextureParameters(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.ClampToEdge);
        }

        public void SetSize(int height, int width)
        {
            var bytes = Enumerable.Range(0, height * width).Select(_ => (byte) 0).ToArray();
            unsafe
            {
                fixed (byte* ptr = bytes)
                    SetData((IntPtr) ptr, height, width);
            }
        }

        private static void SetTextureParameters(TextureMinFilter minFilter, TextureMagFilter magFilter, TextureWrapMode wrapMode)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        }

        public void SetSubData(IntPtr buffer, int xOffset, int yOffset, int rows, int columns)
        {
            Bind();

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, yOffset, columns, rows,
                PixelFormat.Red, PixelType.UnsignedByte, buffer);
        }

        public void Bind(int slot = 0)
        {
            GL.ActiveTexture(ConvertTextureUnit(slot));
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        private static TextureUnit ConvertTextureUnit(int slot) => (TextureUnit) slot + 0x84C0;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            GL.DeleteTexture(_id);

            _disposed = true;
        }
    }
}
