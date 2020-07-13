namespace Pretend.Graphics
{
    public interface IVertexArray : IBuffer
    {
        IVertexBuffer VertexBuffer { get; set; }
        IIndexBuffer IndexBuffer { get; set; }
    }
}
