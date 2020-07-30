using System.Runtime.InteropServices;
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
    }
    
    public interface I2DRenderer : IRenderer
    {
        void Submit(Renderable2DObject renderObject);
    }

    public class Renderer2D : I2DRenderer
    {
        [StructLayout(LayoutKind.Explicit, Size = 36)]
        private struct Renderable2DBuffer
        {
            [FieldOffset(0)]
            public readonly Vector4 Position;
            [FieldOffset(16)]
            public readonly Vector2 TextureLocation;
            [FieldOffset(20)]
            public readonly Vector4 Color;
            // public bool HasTexture;

            public Renderable2DBuffer(Vector4 position, Vector2 textureLocation, Vector4 color, bool hasTexture)
            {
                Position = position;
                TextureLocation = textureLocation;
                Color = color;
                // HasTexture = hasTexture;
            }
        }

        private const int BufferSize = 400;

        private readonly IRenderContext _renderContext;
        private readonly IFactory _factory;

        private Vector4[] _vertices;
        private Matrix4 _viewProjection;
        private IVertexArray _vertexArray;
        private IShader _objectShader;

        public Renderer2D(IRenderContext renderContext, IFactory factory)
        {
            _renderContext = renderContext;
            _factory = factory;
        }

        public void Init()
        {
            _renderContext.Init();

            _vertices = new Vector4[]
            {
                new Vector4(0.5f, 0.5f, 0, 1), 
                new Vector4(0.5f, -0.5f, 0, 1), 
                new Vector4(-0.5f, -0.5f, 0, 1), 
                new Vector4(-0.5f, 0.5f, 0, 1), 
            };

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetSize<Renderable2DBuffer>(BufferSize);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);
            vertexBuffer.AddLayout<float>(4);

            var indices = new uint[] { 0, 1, 3, 1, 2, 3 };
            var indexBuffer = _factory.Create<IIndexBuffer>();
            indexBuffer.AddData(indices);

            _vertexArray = _factory.Create<IVertexArray>();
            _vertexArray.VertexBuffer = vertexBuffer;
            _vertexArray.IndexBuffer = indexBuffer;

            _objectShader = _factory.Create<IShader>();
            _objectShader.Compile("Pretend.Graphics.Shaders.2DObject.glsl");
            _objectShader.SetInt("texture0", 0);
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

            var buffers = new Renderable2DBuffer[4];

            buffers[0] = new Renderable2DBuffer(_vertices[0] * transform,
                new Vector2(1,1),
                renderObject.Color,
                renderObject.Texture != null);
            buffers[1] = new Renderable2DBuffer(_vertices[1] * transform,
                new Vector2(1, 0),
                renderObject.Color,
                renderObject.Texture != null);
            buffers[2] = new Renderable2DBuffer(_vertices[2] * transform,
                new Vector2(0, 0),
                renderObject.Color,
                renderObject.Texture != null);
            buffers[3] = new Renderable2DBuffer(_vertices[3] * transform,
                new Vector2(0, 1),
                renderObject.Color,
                renderObject.Texture != null);

            _vertexArray.VertexBuffer.AddData(buffers);

            renderObject.Texture?.Bind();

            Submit(_objectShader, _vertexArray);
        }
    }
}
