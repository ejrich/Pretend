using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class Framebuffer : IFramebuffer
    {
        private readonly IFactory _factory;
        private int _id;
        private int _width;
        private int _height;

        public Framebuffer(ITexture2D colorTexture, IFactory factory)
        {
            _factory = factory;
            ColorTexture = colorTexture;
 
            _id = GL.GenFramebuffer();
        }

        ~Framebuffer()
        {
            GL.DeleteFramebuffer(_id);
        }

        public ITexture2D ColorTexture { get; private set; }
        private int _depthTexture; // Unused for now

        public void Init(int width, int height)
        {
            Bind();
            _width = width;
            _height = height;

            // Color Texture
            ColorTexture.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture.Id, 0);

            // Depth Texture
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _depthTexture);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, _width, _height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, _depthTexture, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Framebuffer is incomplete");

            Unbind();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
            GL.Viewport(0, 0, _width, _height);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height)
        {
            GL.DeleteFramebuffer(_id);
            ColorTexture.Delete();
            GL.DeleteTexture(_depthTexture);

            _id = GL.GenFramebuffer();
            ColorTexture = _factory.Create<ITexture2D>();
            Init(width, height);
        }
    }
}
