using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public class Renderable2DObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public Vector4 Color { get; set; } = Vector4.One;
        public ITexture2D Texture { get; set; }
        public IShader Shader { get; set; }
    }
    
    public interface I2DRenderer : IRenderer
    {
        void Submit(Renderable2DObject renderObject);
    }

    public class Renderer2D : I2DRenderer
    {
        private readonly IRenderContext _renderContext;
        private readonly IFactory _factory;

        private Matrix4 _viewProjection;
        private IVertexArray _vertexArray;

        public Renderer2D(IRenderContext renderContext, IFactory factory)
        {
            _renderContext = renderContext;
            _factory = factory;
        }

        public void Init()
        {
            _renderContext.Init();

            var vertices = new float[]
            {
                 0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
            };

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetData(vertices);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);

            var indices = new uint[] { 0, 1, 3, 1, 2, 3 };
            var indexBuffer = _factory.Create<IIndexBuffer>();
            indexBuffer.AddData(indices);

            _vertexArray = _factory.Create<IVertexArray>();
            _vertexArray.VertexBuffer = vertexBuffer;
            _vertexArray.IndexBuffer = indexBuffer;
        }

        public void Begin(ICamera camera)
        {
            _viewProjection = camera.ViewProjection;

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

        public void Submit(Renderable2DObject renderObject)
        {
            var transform = Matrix4.Identity *
                Matrix4.CreateScale(renderObject.Width, renderObject.Height, 1) *
                Matrix4.CreateTranslation(renderObject.X, renderObject.Y, renderObject.Z);

            renderObject.Shader.Bind();
            renderObject.Shader.SetMat4("transform", transform);
            renderObject.Shader.SetVec4("color", renderObject.Color);
            renderObject.Shader.SetBool("hasTexture", renderObject.Texture != null);

            renderObject.Texture?.Bind();

            Submit(renderObject.Shader, _vertexArray);
        }
    }
}
