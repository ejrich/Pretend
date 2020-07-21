using System.Drawing;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class Texture2D : ITexture2D
    {
        private int _id;

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
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        public void Bind(int slot = 0)
        {
            GL.ActiveTexture(ConvertTextureUnit(slot));
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        private TextureUnit ConvertTextureUnit(int slot)
        {
            return (TextureUnit) slot + 0x84C0;
        }
    }
}
