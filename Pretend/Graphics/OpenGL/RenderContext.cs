using OpenTK.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class RenderContext : IRenderContext
    {
        public void Init()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void ClearDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void BackgroundColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }

        public void Draw(IVertexArray vertexArray)
        {
            Draw(vertexArray, vertexArray.IndexBuffer.Count);
        }

        public void Draw(IVertexArray vertexArray, int count)
        {
            GL.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, 0);
        }

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
