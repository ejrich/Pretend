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
        private readonly FreeTypeLibrary _lib;

        private readonly IDictionary<string, Font> _fonts = new Dictionary<string, Font>();
        private readonly IDictionary<uint, IDictionary<char, Glyph>> _characterMappings = new Dictionary<uint, IDictionary<char, Glyph>>();

        public TextRenderer(I2DRenderer renderer)
        {
            _renderer = renderer;
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
                _fonts[fontPath] = font = new Font();

            // TODO Remove me and put in block above once the texture atlas is working
            var texture = font.Load(_lib, fontPath);

            if (!_characterMappings.TryGetValue(size, out var textureAtlas))
                _characterMappings[size] = textureAtlas = font.LoadTextureAtlas(size);

            foreach (var character in text)
            {
                var glyph = textureAtlas[character];
                var renderObject = new Renderable2DObject
                {
                    Width = glyph.Width,
                    Height = glyph.Height,
                    Texture = texture.text,
                    Color = color
                };
                _renderer.Submit(renderObject);
            }
        }
    }
}
