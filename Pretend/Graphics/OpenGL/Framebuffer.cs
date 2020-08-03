using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class Framebuffer : IFramebuffer
    {
        public Framebuffer()
        {
            Id = GL.GenFramebuffer();
        }

        ~Framebuffer()
        {
            GL.DeleteFramebuffer(Id);
        }

        public int Id { get; }

        public void Bind()
        {
            throw new System.NotImplementedException();
        }

        public void Unbind()
        {
            throw new System.NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            throw new System.NotImplementedException();
        }
    }
}
