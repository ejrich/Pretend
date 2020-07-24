using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public class Renderable2DObject
    {
        public Vector2 Location { get; set; }
        // public Vector3 Location { set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public ITexture2D Texture { get; set; }
        public IShader Shader { get; set; }
    }
    
    public interface I2DRenderer : IRenderer
    {
        void Draw(Renderable2DObject renderObject);
    }

    public class Renderer2D : I2DRenderer
    {
        private readonly IRenderContext _renderContext;
        private Matrix4 _viewProjection;

        public Renderer2D(IRenderContext renderContext)
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

        public void Draw(Renderable2DObject renderObject)
        {
        }
    }
}
