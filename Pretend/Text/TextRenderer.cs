using System;
using System.Collections.Generic;
using System.Numerics;
using FreeTypeSharp;
using Pretend.Graphics;
using Pretend.Mathematics;

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
            if (string.IsNullOrWhiteSpace(textObject.FontPath) || string.IsNullOrWhiteSpace(textObject.Text) || textObject.Size == 0)
                return;

            var (charMap, texture) = LoadTextureAtlas(textObject.FontPath, textObject.Size);
 
            var x = textObject.Position.X;
            var yAdjust = (float)textObject.Size / 4;

            var renderObjects = new List<Renderable2DObject>();
            var line = new List<Renderable2DObject>();
            foreach (var character in textObject.Text)
            {
                if (character == '\n')
                {
                    AdjustLine(line, textObject.Alignment, textObject.Position.X, x);
                    yAdjust += textObject.Size;
                    x = textObject.Position.X;
                    continue;
                }

                if (!charMap.TryGetValue(character, out var glyph))
                    continue;

                var renderObject = new Renderable2DObject
                {
                    X = x + (float) glyph.Width / 2 + glyph.BearingX,
                    Y = textObject.Position.Y + (float) glyph.Height / 2 - glyph.Height + glyph.BearingY - yAdjust,
                    Z = textObject.Position.Z,
                    Width = glyph.Width + 1,
                    Height = glyph.Height,
                    Rotation = textObject.Orientation.ToQuaternian(),
                    Color = textObject.Color,
                    Texture = texture,
                    SubTextureOffsetX = glyph.XOffset,
                    SubTextureOffsetY = glyph.YOffset,
                    SingleChannel = true
                };
                renderObjects.Add(renderObject);
                line.Add(renderObject);
                x += glyph.Advance;
            }
            AdjustLine(line, textObject.Alignment, textObject.Position.X, x);

            foreach (var renderObject in renderObjects)
            {
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

        private static void Rotate(Renderable2DObject renderObject)
        {
            var transformedPosition = Vector3.Transform(new Vector3(renderObject.X, renderObject.Y, renderObject.Z),
                Matrix4x4.CreateFromQuaternion(renderObject.Rotation));
            renderObject.X = transformedPosition.X;
            renderObject.Y = transformedPosition.Y;
            renderObject.Z = transformedPosition.Z;
        }

        private static void AdjustLine(List<Renderable2DObject> line, TextAlignment alignment, float x0, float x1)
        {
            var xAdjustment = alignment switch
            {
                TextAlignment.Left => 0,
                TextAlignment.Center => (x1 - x0) / 2,
                TextAlignment.Right => x1 - x0,
                _ => 0
            };

            foreach (var renderObject in line)
            {
                renderObject.X -= xAdjustment;
                Rotate(renderObject);
            }
            line.Clear();
        }
    }
}
