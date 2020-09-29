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
        public Quaternion Rotation { get; set; }
        public Vector4 Color { get; set; } = Vector4.One;
        public ITexture2D Texture { get; set; }
    }

    public interface I2DRenderer : IRenderer
    {
        void Submit(Renderable2DObject renderObject);
    }

    public class Renderer2D : I2DRenderer
    {
        [StructLayout(LayoutKind.Explicit, Size = 40)]
        internal struct Renderable2DBuffer
        {
            [FieldOffset(0)] private readonly Vector3 Position;
            [FieldOffset(12)] private readonly Vector2 TextureLocation;
            [FieldOffset(20)] private readonly Vector4 Color;
            [FieldOffset(36)] private readonly int Texture;

            public Renderable2DBuffer(Vector4 position, Vector2 textureLocation, Vector4 color, int texture)
            {
                Position = position.Xyz;
                TextureLocation = textureLocation;
                Color = color;
                Texture = texture;
            }
        }

        internal const int MaxSubmissions = 400;
        internal const int VerticesInSubmission = 4;
        internal const int IndicesInSubmission = 6;
        internal const int MaxTextures = 32;

        private readonly IRenderContext _renderContext;
        private readonly IFactory _factory;
        private readonly List<Renderable2DBuffer> _submissions = new List<Renderable2DBuffer>();
        private readonly IDictionary<ITexture2D, int> _textures = new Dictionary<ITexture2D, int>();

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
                new Vector4(0.5f, 0.5f, 0, 1), new Vector4(0.5f, -0.5f, 0, 1), new Vector4(-0.5f, -0.5f, 0, 1),
                new Vector4(-0.5f, 0.5f, 0, 1)
            };
            _textureCoordinates = new[] {new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)};

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetSize<Renderable2DBuffer>(MaxSubmissions * VerticesInSubmission);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);
            vertexBuffer.AddLayout<float>(4);
            vertexBuffer.AddLayout<float>(1);

            var indices = Enumerable.Range(0, MaxSubmissions)
                .SelectMany(i =>
                {
                    var startingIndex = Convert.ToUInt32(i * VerticesInSubmission);
                    return new[]
                    {
                        0 + startingIndex, 1 + startingIndex, 3 + startingIndex, 1 + startingIndex,
                        2 + startingIndex, 3 + startingIndex
                    };
                }).ToArray();
            var indexBuffer = _factory.Create<IIndexBuffer>();
            indexBuffer.AddData(indices);

            _vertexArray = _factory.Create<IVertexArray>();
            _vertexArray.VertexBuffer = vertexBuffer;
            _vertexArray.IndexBuffer = indexBuffer;

            _objectShader = _factory.Create<IShader>();
            _objectShader.Compile("Pretend.Graphics.Shaders.2DObject.glsl");
            _objectShader.SetIntArray("textures[0]", Enumerable.Range(0, 32).ToArray());
        }

        public void Begin(ICamera camera)
        {
            _viewProjection = camera.ViewProjection;

            _renderContext.Clear();
        }

        public void End()
        {
            Flush(_submissions.Count / VerticesInSubmission);
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
                            Matrix4.CreateFromQuaternion(renderObject.Rotation) *
                            // Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(renderObject.Rotation)) *
                            Matrix4.CreateTranslation(renderObject.X, renderObject.Y, renderObject.Z);

            if (_submissions.Count / VerticesInSubmission == MaxSubmissions)
                Flush();
            else if (_textures.Count == MaxTextures && renderObject.Texture != null &&
                     !_textures.ContainsKey(renderObject.Texture))
                Flush(_submissions.Count / VerticesInSubmission);

            foreach (var vertex in Enumerable.Range(0, VerticesInSubmission))
            {
                _submissions.Add(new Renderable2DBuffer(_vertices[vertex] * transform, _textureCoordinates[vertex],
                    renderObject.Color, GetTextureIndex(renderObject.Texture)));
            }
        }

        private void Flush(int submissionCount = MaxSubmissions)
        {
            _vertexArray.VertexBuffer.AddData(_submissions.ToArray());
            _submissions.Clear();

            _objectShader.Bind();
            _objectShader.SetMat4("viewProjection", _viewProjection);

            foreach (var (texture, slot) in _textures)
            {
                texture.Bind(slot);
            }

            _textures.Clear();

            _vertexArray.Bind();

            _renderContext.Draw(_vertexArray, submissionCount * IndicesInSubmission);
        }

        private int GetTextureIndex(ITexture2D texture)
        {
            if (texture == null) return -1;

            if (_textures.TryGetValue(texture, out var index))
                return index;

            var slot = _textures.Count;
            _textures[texture] = slot;
            return slot;
        }
    }
}
