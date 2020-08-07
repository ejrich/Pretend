using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class Framebuffer : IFramebuffer
    {
        private readonly IFactory _factory;
        private int _id;

        public Framebuffer(ITexture2D texture, IFactory factory)
        {
            _factory = factory;
            ColorTexture = texture;
 
            _id = GL.GenFramebuffer();
        }

        ~Framebuffer()
        {
            GL.DeleteFramebuffer(_id);
        }

        public int Width { get; private set; } = 1280;
        public int Height { get; private set; } = 720;
        public ITexture2D ColorTexture { get; private set; }

        public void Init()
        {
            Bind();

            // Color Texture
            ColorTexture.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture.Id, 0);

            // Depth Texture
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int depthTexture);
            GL.BindTexture(TextureTarget.Texture2D, depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, Width, Height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, depthTexture, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Framebuffer is incomplete");

            Unbind();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            GL.DeleteFramebuffer(_id);
            GL.DeleteTexture(ColorTexture.Id);

            _id = GL.GenFramebuffer();
            ColorTexture = _factory.Create<ITexture2D>();
            Init();
        }
    }
}
