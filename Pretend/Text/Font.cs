using FreeTypeSharp;
using FreeTypeSharp.Native;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;

namespace Pretend.Text
{
    public class Font
    {
        ~Font()
        {
            FT.FT_Done_Face(_face.Face);
        }
        private FreeTypeFaceFacade _face;

        public (ITexture2D text, uint x, uint y) Load(FreeTypeLibrary lib, string font)
        {
            FT.FT_New_Face(lib.Native, font, 0, out var facePtr);
            _face = new FreeTypeFaceFacade(lib, facePtr);
            _face.SelectCharSize(20, 300, 0);

            var glyph = LoadGlyph('B');

            var texture = new Texture2D();
            texture.SetData(glyph.Buffer, (int) glyph.Height, (int) glyph.Width);

            return (texture, glyph.Width, glyph.Height);
        }

        public void LoadTextureAtlas(int size)
        {
            
        }

        // private char LoadChar(uint index)
        // {
        //     var c = FT.FT_Load_Char(_face.Face, index, FT.FT_LOAD_RENDER);
        //     return 'A';
        // }

        private Glyph LoadGlyph(char c)
        {
            var index = _face.GetCharIndex(c);
            FT.FT_Load_Glyph(_face.Face, index, FT.FT_LOAD_RENDER);
            unsafe
            {
                return new Glyph
                {
                    Buffer = _face.GlyphSlot->bitmap.buffer,
                    Width = _face.GlyphSlot->bitmap.width,
                    Height = _face.GlyphSlot->bitmap.rows,
                    BearingX = _face.GlyphSlot->bitmap_left,
                    BearingY = _face.GlyphSlot->bitmap_top,
                    Advance = (uint)_face.GlyphSlot->advance.x >> 6
                };
            }
        }
    }
}
