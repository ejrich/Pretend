using System;
using System.Collections.Generic;
using System.Linq;
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

        private const int MaxSubmissions = 400;
        private const int VerticesInSubmission = 4;

        private readonly IRenderContext _renderContext;
        private readonly IFactory _factory;
        private readonly List<Renderable2DBuffer> _submissions = new List<Renderable2DBuffer>();

        private Vector4[] _vertices;
        private Vector2[] _textureCoordinates;
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

            _vertices = new[]
            {
                new Vector4(0.5f, 0.5f, 0, 1),
                new Vector4(0.5f, -0.5f, 0, 1),
                new Vector4(-0.5f, -0.5f, 0, 1),
                new Vector4(-0.5f, 0.5f, 0, 1)
            };
            _textureCoordinates = new[]
            {
                new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)
            };

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetSize<Renderable2DBuffer>(MaxSubmissions * VerticesInSubmission);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);
            vertexBuffer.AddLayout<float>(4);

            var indices = Enumerable.Range(0, MaxSubmissions)
                .SelectMany(i =>
                {
                    var startingIndex = Convert.ToUInt32(i * VerticesInSubmission);
                    return new[]
                    {
                        0 + startingIndex, 1 + startingIndex, 3 + startingIndex,
                        1 + startingIndex, 2 + startingIndex, 3 + startingIndex
                    };
                }).ToArray();
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
            Flush();
        }

        [Obsolete("Only use this method if you want to immediately render something")]
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

            if (_submissions.Count / VerticesInSubmission == MaxSubmissions)
                Flush();

            foreach (var vertex in Enumerable.Range(0, VerticesInSubmission))
            {
                _submissions.Add(new Renderable2DBuffer(_vertices[vertex] * transform, _textureCoordinates[vertex],
                    renderObject.Color, renderObject.Texture != null));
            }
        }

        private void Flush()
        {
            _vertexArray.VertexBuffer.AddData(_submissions.ToArray());
            _submissions.Clear();

            _objectShader.Bind();
            _objectShader.SetMat4("viewProjection", _viewProjection);

            _renderContext.Draw(_vertexArray);
        }
    }
}
