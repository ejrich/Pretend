using System.Collections.Generic;
using FreeTypeSharp;
using OpenTK.Mathematics;
using Pretend.Text;

namespace Pretend.Graphics
{
    public interface ITextRenderer
    {
        void RenderText(string text, string fontPath, uint size, Vector3 position);
        void RenderText(string text, string fontPath, uint size, Vector3 position, Vector3 orientation);
        void RenderText(string text, string fontPath, uint size, Vector3 position, Vector3 orientation, Vector4 color);
    }

    public class TextRenderer : ITextRenderer
    {
        private readonly I2DRenderer _renderer;
        private readonly IFactory _factory;
        private readonly FreeTypeLibrary _lib;

        private readonly IDictionary<string, IFont> _fonts = new Dictionary<string, IFont>();
        private readonly IDictionary<uint, (IDictionary<char, Glyph> charMap, ITexture2D texture)> _characterMappings = new Dictionary<uint, (IDictionary<char, Glyph>, ITexture2D)>();

        public TextRenderer(I2DRenderer renderer, IFactory factory)
        {
            _renderer = renderer;
            _factory = factory;
            _lib = new FreeTypeLibrary();
        }

        ~TextRenderer() => _lib.Dispose();

        public void RenderText(string text, string font, uint size, Vector3 position)
        {
            RenderText(text, font, size, position, Vector3.Zero, Vector4.One);
        }

        public void RenderText(string text, string font, uint size, Vector3 position, Vector3 orientation)
        {
            RenderText(text, font, size, position, orientation, Vector4.One);
        }

        public void RenderText(string text, string fontPath, uint size, Vector3 position, Vector3 orientation, Vector4 color)
        {
            if (!_fonts.TryGetValue(fontPath, out var font))
            {
                _fonts[fontPath] = font = _factory.Create<IFont>();
                font.Load(_lib, fontPath);
            }

            if (!_characterMappings.TryGetValue(size, out var textureAtlas))
                _characterMappings[size] = textureAtlas = font.LoadTextureAtlas(size);

            var (x, y, z) = position;
            foreach (var character in text)
            {
                var glyph = textureAtlas.charMap[character];
                var renderObject = new Renderable2DObject
                {
                    X = x + ((float) glyph.Width / 2),
                    Y = y + ((float) glyph.Height / 2) - (glyph.Height - glyph.BearingY),
                    Z = z,
                    Width = glyph.Width,
                    Height = glyph.Height,
                    SubTextureOffsetX = glyph.XOffset,
                    SubTextureOffsetY = glyph.YOffset,
                    Texture = textureAtlas.texture,
                    Color = color
                };
                _renderer.Submit(renderObject);
                x += glyph.Advance;
            }
        }
    }
}
