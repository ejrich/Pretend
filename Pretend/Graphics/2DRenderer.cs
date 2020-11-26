using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Pretend.Graphics
{
    public class Renderable2DObject
    {
        public Vector3 Position { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector4 Color { get; set; } = Vector4.One;
        public ITexture2D Texture { get; set; }
        public float SubTextureOffsetX { get; set; }
        public float SubTextureOffsetY { get; set; }
        public bool SingleChannel { get; set; }
    }

    public interface I2DRenderer : IRenderer
    {
        void Submit(Renderable2DObject renderObject);
    }

    public class Renderer2D : I2DRenderer
    {
        [StructLayout(LayoutKind.Explicit, Size = 44)]
        internal struct Renderable2DBuffer
        {
            [FieldOffset(0)] private readonly Vector3 Position;
            [FieldOffset(12)] private readonly Vector2 TextureLocation;
            [FieldOffset(20)] private readonly Vector4 Color;
            [FieldOffset(36)] private readonly int Texture;
            [FieldOffset(40)] private readonly int SingleChannel;

            public Renderable2DBuffer(Vector4 position, Vector2 textureLocation, Vector4 color,
                int texture, bool singleChannel)
            {
                Position = new Vector3(position.X, position.Y, position.Z);
                TextureLocation = textureLocation;
                Color = color;
                Texture = texture;
                SingleChannel = singleChannel ? 1 : 0;
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

        private bool _initialized;
        private Vector4[] _vertices;
        private Vector2[] _textureCoordinates;
        private Matrix4x4 _viewProjection;
        private IVertexArray _vertexArray;
        private IShader _objectShader;

        public Renderer2D(IRenderContext renderContext, IFactory factory)
        {
            _renderContext = renderContext;
            _factory = factory;
        }

        public void Init()
        {
            if (_initialized) return;

            _renderContext.Init();

            _vertices = new[]
            {
                new Vector4(0.5f, 0.5f, 0, 1), new Vector4(0.5f, -0.5f, 0, 1),
                new Vector4(-0.5f, -0.5f, 0, 1), new Vector4(-0.5f, 0.5f, 0, 1)
            };
            _textureCoordinates = new[] {new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0)};

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetSize<Renderable2DBuffer>(MaxSubmissions * VerticesInSubmission);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);
            vertexBuffer.AddLayout<float>(4);
            vertexBuffer.AddLayout<float>(1);
            vertexBuffer.AddLayout<float>(1);

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
            _objectShader.SetIntArray("textures[0]", Enumerable.Range(0, 32).ToArray());

            _initialized = true;
        }

        public void Begin(ICamera camera)
        {
            _viewProjection = camera.ViewProjection;
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
            if (renderObject.Width == 0 && renderObject.Height == 0) return;

            var transform =
                Matrix4x4.CreateScale(renderObject.Width, renderObject.Height, 1) *
                Matrix4x4.CreateFromQuaternion(renderObject.Rotation) *
                Matrix4x4.CreateTranslation(renderObject.Position);

            if (_submissions.Count / VerticesInSubmission == MaxSubmissions)
                Flush();
            else if (_textures.Count == MaxTextures && renderObject.Texture != null &&
                     !_textures.ContainsKey(renderObject.Texture))
                Flush(_submissions.Count / VerticesInSubmission);

            foreach (var vertex in Enumerable.Range(0, VerticesInSubmission))
            {
                var textureCoord = _textureCoordinates[vertex];
                if (renderObject.Texture != null && (renderObject.SubTextureOffsetX != 0 || renderObject.SubTextureOffsetY != 0))
                {
                    textureCoord.X = ((textureCoord.X * renderObject.Width) + renderObject.SubTextureOffsetX) / renderObject.Texture.Width;
                    textureCoord.Y = ((textureCoord.Y * renderObject.Height) + renderObject.SubTextureOffsetY) / renderObject.Texture.Height;
                }
                _submissions.Add(new Renderable2DBuffer(Vector4.Transform(_vertices[vertex], transform), textureCoord,
                    renderObject.Color, GetTextureIndex(renderObject.Texture), renderObject.SingleChannel));
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

            _vertexArray.Bind(true);

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
