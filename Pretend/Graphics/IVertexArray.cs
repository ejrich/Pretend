namespace Pretend.Graphics
{
    public interface IVertexArray : IBuffer
    {
        void Bind(bool bindBuffers);
        IVertexBuffer VertexBuffer { get; set; }
        IIndexBuffer IndexBuffer { get; set; }
    }
}
