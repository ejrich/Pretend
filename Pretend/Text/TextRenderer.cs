using System.Collections.Generic;
using FreeTypeSharp;
using OpenTK.Mathematics;
using Pretend.Graphics;

namespace Pretend.Text
{
    public class RenderableTextObject
    {
        public string Text { get; set; }
        public string FontPath { get; set; }
        public uint Size { get; set; }
        public TextAlignment Alignment { get; set; } = TextAlignment.Center;
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Orientation { get; set; } = Vector3.Zero;
        public Vector4 Color { get; set; } = Vector4.One;
    }

    public interface ITextRenderer
    {
        void RenderText(RenderableTextObject textObject);
    }

    public class TextRenderer : ITextRenderer
    {
        private readonly I2DRenderer _renderer;
        private readonly IFactory _factory;
        private readonly FreeTypeLibrary _lib;

        private readonly IDictionary<string, IFont> _fonts = new Dictionary<string, IFont>();
        private readonly IDictionary<string, IDictionary<uint, (IDictionary<char, Glyph> charMap, ITexture2D texture)>> _characterMappings
            = new Dictionary<string, IDictionary<uint, (IDictionary<char, Glyph> charMap, ITexture2D texture)>>();

        public TextRenderer(I2DRenderer renderer, IFactory factory)
        {
            _renderer = renderer;
            _factory = factory;
            _lib = new FreeTypeLibrary();
        }

        ~TextRenderer() => _lib.Dispose();

        public void RenderText(RenderableTextObject textObject)
        {
            if (string.IsNullOrWhiteSpace(textObject.FontPath) || string.IsNullOrWhiteSpace(textObject.Text)) return;

            var (charMap, texture) = LoadTextureAtlas(textObject.FontPath, textObject.Size);
 
            var (x, y, z) = textObject.Position;
            var (pitch, roll, yaw) = textObject.Orientation;
            var yAdjust = (float)textObject.Size / 4;

            var renderObjects = new List<Renderable2DObject>();
            foreach (var character in textObject.Text)
            {
                var glyph = charMap[character];
                var renderObject = new Renderable2DObject
                {
                    X = x + (float) glyph.Width / 2 + glyph.BearingX,
                    Y = y + (float) glyph.Height / 2 - glyph.Height + glyph.BearingY - yAdjust,
                    Z = z,
                    Width = glyph.Width + 1,
                    Height = glyph.Height,
                    Rotation = new Quaternion(MathHelper.DegreesToRadians(pitch),
                        MathHelper.DegreesToRadians(roll), MathHelper.DegreesToRadians(yaw)),
                    Color = textObject.Color,
                    Texture = texture,
                    SubTextureOffsetX = glyph.XOffset,
                    SubTextureOffsetY = glyph.YOffset,
                    SingleChannel = true
                };
                renderObjects.Add(renderObject);
                x += glyph.Advance;
            }

            var dx = textObject.Alignment switch
            {
                TextAlignment.Left => 0,
                TextAlignment.Center => (x - textObject.Position.X) / 2,
                TextAlignment.Right => x - textObject.Position.X,
                _ => 0
            };

            // _renderer.Submit(new Renderable2DObject
            // {
            //     Z = -0.9f,
            //     Width = (uint)(x - textObject.Position.X),
            //     Height = textObject.Size,
            //     Color = new Vector4(1, 0, 0, 1)
            // });
            foreach (var renderObject in renderObjects)
            {
                renderObject.X -= dx;
                _renderer.Submit(renderObject);
            }
        }

        private (IDictionary<char, Glyph> charMap, ITexture2D texture) LoadTextureAtlas(string fontPath, uint size)
        {
            if (!_characterMappings.TryGetValue(fontPath, out var sizeMappings))
                _characterMappings[fontPath] = sizeMappings = new Dictionary<uint, (IDictionary<char, Glyph> charMap, ITexture2D texture)>();

            if (sizeMappings.TryGetValue(size, out var textureAtlas))
                return textureAtlas;

            if (!_fonts.TryGetValue(fontPath, out var font))
            {
                _fonts[fontPath] = font = _factory.Create<IFont>();
                font.Load(_lib, fontPath);
            }
            sizeMappings[size] = textureAtlas = font.LoadTextureAtlas(size);

            return textureAtlas;
        }
    }
}
