namespace Pretend.Graphics
{
    public interface IBuffer
    {
        void Bind();
        void Unbind();
    }

    public interface IVertexBuffer : IBuffer
    {
        void SetData(float[] vertices);
    }

    public interface IIndexBuffer : IBuffer
    {
        void AddData(uint[] indices);
        int Count { get; }
    }
}
