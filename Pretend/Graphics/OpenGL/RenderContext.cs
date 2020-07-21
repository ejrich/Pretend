using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class RenderContext : IRenderContext
    {
        public void Init()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void BackgroundColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }

        public void Draw(IVertexArray vertexArray)
        {
            GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexBuffer.Count,
                DrawElementsType.UnsignedInt, 0);
        }
    }
}
