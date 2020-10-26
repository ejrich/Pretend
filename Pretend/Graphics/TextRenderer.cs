using FreeTypeSharp;
using FreeTypeSharp.Native;
using OpenTK.Mathematics;

namespace Pretend.Graphics
{
    public interface ITextRenderer
    {
        void RenderText(string text, int size, Vector3 position);
        void RenderText(string text, int size, Vector3 position, Vector3 orientation);
        void RenderText(string text, int size, Vector3 position, Vector3 orientation, Vector4 color);
    }

    public class TextRenderer : ITextRenderer
    {
        private readonly I2DRenderer _renderer;

        public TextRenderer(I2DRenderer renderer)
        {
            _renderer = renderer;
        }

        public void RenderText(string text, int size, Vector3 position)
        {
            RenderText(text, size, position, Vector3.Zero, Vector4.One);
        }

        public void RenderText(string text, int size, Vector3 position, Vector3 orientation)
        {
            RenderText(text, size, position, orientation, Vector4.One);
        }

        public void RenderText(string text, int size, Vector3 position, Vector3 orientation, Vector4 color)
        {
            var lib = new FreeTypeLibrary();
            FT.FT_New_Face(lib.Native, "", 0, out var face);

            foreach (var character in text)
            {
                var renderObject = new Renderable2DObject();
                _renderer.Submit(renderObject);
            }
        }
    }
}
