namespace Pretend.Graphics
{
    public interface IRenderer
    {
        void Init();
        void Begin();
        void End();
        void Submit(IShader shader, IVertexArray vertexArray);
    }

    public class Renderer : IRenderer
    {
        private readonly IRenderContext _renderContext;

        public Renderer(IRenderContext renderContext)
        {
            _renderContext = renderContext;
        }

        public void Init()
        {
            _renderContext.Init();
        }

        public void Begin()
        {
            // TODO Add the camera
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
            vertexArray.Bind();

            _renderContext.Draw(vertexArray);
        }
    }
}
