using System.Collections.Generic;
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
            
            // TODO Remove this block once texture atlases are working
            {
                FT.FT_Set_Pixel_Sizes(_face.Face, 0, 60);

                var glyph = LoadGlyph(_face.GetCharIndex('B'));

                var texture = new Texture2D();
                texture.SetData(glyph.Buffer, (int)glyph.Height, (int)glyph.Width);

                return (texture, glyph.Width, glyph.Height);
            }
        }

        public IDictionary<char, Glyph> LoadTextureAtlas(uint size)
        {
            FT.FT_Set_Pixel_Sizes(_face.Face, 0, size);

            var character = FT.FT_Get_First_Char(_face.Face, out var index);

            var charMap = new Dictionary<char, Glyph>();
            do
            {
                charMap.Add((char) character, LoadGlyph(index));
                character = FT.FT_Get_Next_Char(_face.Face, character, out index);
            } while (index != 0 && charMap.Count <= 128);

            return charMap;
        }

        private Glyph LoadGlyph(uint index)
        {
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
