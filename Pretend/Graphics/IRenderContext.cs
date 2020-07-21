namespace Pretend.Graphics
{
    public interface IRenderContext
    {
        void Init();
        void Clear();
        void BackgroundColor(float r, float g, float b, float a);
        void Draw(IVertexArray vertexArray);
    }
}
