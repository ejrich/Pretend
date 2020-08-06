namespace Pretend.Graphics
{
    public interface IFramebuffer
    {
        void Init();
        void Bind();
        void Unbind();
        void Resize(int width, int height);
        int Id { get; }
        int Width { get; }
        int Height { get; }
        ITexture2D ColorTexture { get; }
    }
}
