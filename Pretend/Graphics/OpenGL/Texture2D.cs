using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Pretend.Graphics.OpenGL
{
    public class Texture2D : ITexture2D
    {
        private readonly int _id;

        public Texture2D()
        {
            _id = GL.GenTexture();
        }

        ~Texture2D()
        {
            GL.DeleteTexture(_id);
        }

        public void SetData(string file)
        {
            Bind();

            using (var image = new Bitmap(file))
            {
                image.RotateFlip(RotateFlipType.Rotate180FlipX);
                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, DrawingPixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        public void SetData(IntPtr buffer, int rows, int columns)
        {
            Bind();

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRed,
                columns, rows, 0, PixelFormat.Red, PixelType.UnsignedByte, buffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        public void Bind(int slot = 0)
        {
            GL.ActiveTexture(ConvertTextureUnit(slot));
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        private static TextureUnit ConvertTextureUnit(int slot)
        {
            return (TextureUnit) slot + 0x84C0;
        }
    }
}
