namespace Pretend.Graphics
{
    public interface IRenderContext
    {
        void Init();
        void Clear();
        void ClearDepth();
        void BackgroundColor(float r, float g, float b, float a);
        void Draw(IVertexArray vertexArray);
        void Draw(IVertexArray vertexArray, int count);
        void SetViewport(int width, int height);
    }
}
