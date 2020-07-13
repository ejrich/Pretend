namespace Pretend.OpenGL
{
    public interface IBuffer
    {
        void Bind();
        void Unbind();
    }

    public interface IVertexBuffer : IBuffer
    {
        void SetData();
    }

    public interface IIndexBuffer : IBuffer
    {
        int Count { get; }
    }
}
