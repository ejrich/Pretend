namespace Pretend.Graphics
{
    public interface IFramebuffer
    {
        void Init(int width, int height);
        void Bind();
        void Unbind();
        void Resize(int width, int height);
        ITexture2D ColorTexture { get; }
    }
}
