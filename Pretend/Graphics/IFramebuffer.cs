namespace Pretend.Graphics
{
    public interface IFramebuffer
    {
        void Bind();
        void Unbind();
        void Resize(int width, int height);
        int Id { get; }
    }
}
