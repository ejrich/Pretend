using FreeTypeSharp;
using FreeTypeSharp.Native;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;

namespace Pretend.Text
{
    public class Font
    {
        public ITexture2D Load(string font)
        {
            var lib = new FreeTypeLibrary();
            FT.FT_New_Face(lib.Native, font, 0, out var facePtr);
            var face = new FreeTypeFaceFacade(lib, facePtr);
            face.SelectCharSize(20, 1280, 720);
            var index = face.GetCharIndex('A');
            FT.FT_Load_Glyph(face.Face, index, FT.FT_LOAD_RENDER);
            unsafe
            {
                var buffer = face.GlyphSlot->bitmap.buffer;
                var rows = face.GlyphSlot->bitmap.rows;
                var width = face.GlyphSlot->bitmap.width;

                var texture = new Texture2D();
                texture.SetData(buffer, (int) rows, (int) width);

                return texture;
            }

            // FT.FT_Done_Face(face.Face);
            // lib.Dispose();
        }
    }
}
