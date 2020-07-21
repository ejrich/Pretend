using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public interface IRenderer
    {
        void Init();
        void Begin(ICamera camera);
        void End();
        void Submit(IShader shader, IVertexArray vertexArray);
    }

    public class Renderer : IRenderer
    {
        private readonly IRenderContext _renderContext;
        private Matrix4 _viewProjection;

        public Renderer(IRenderContext renderContext)
        {
            _renderContext = renderContext;
        }

        public void Init()
        {
            _renderContext.Init();
        }

        public void Begin(ICamera camera)
        {
            _viewProjection = camera.ViewProjection;

            _renderContext.BackgroundColor(1, 0, 1, 1);
            _renderContext.Clear();
        }

        public void End()
        {
            // Eventually this will batch render submitted content
        }

        public void Submit(IShader shader, IVertexArray vertexArray)
        {
            shader.Bind();
            shader.SetMat4("viewProjection", _viewProjection);

            vertexArray.Bind();

            _renderContext.Draw(vertexArray);
        }
    }
}
