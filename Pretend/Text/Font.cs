using System;
using System.Collections.Generic;
using FreeTypeSharp;
using FreeTypeSharp.Native;
using Pretend.Graphics;

namespace Pretend.Text
{
    public interface IFont
    {
        void Load(FreeTypeLibrary lib, string fontPath);
        (IDictionary<char, Glyph> charMap, ITexture2D texture) LoadTextureAtlas(uint size);
    }

    public class Font : IFont
    {
        private readonly IFactory _factory;
        private FreeTypeFaceFacade _face;

        public Font(IFactory factory) => _factory = factory;

        ~Font() => FT.FT_Done_Face(_face.Face);

        public void Load(FreeTypeLibrary lib, string font)
        {
            FT.FT_New_Face(lib.Native, font, 0, out var facePtr);
            _face = new FreeTypeFaceFacade(lib, facePtr);
        }

        public (IDictionary<char, Glyph> charMap, ITexture2D texture) LoadTextureAtlas(uint size)
        {
            FT.FT_Set_Pixel_Sizes(_face.Face, 0, size);

            var character = FT.FT_Get_First_Char(_face.Face, out var index);
            uint width = 0, height = 0;

            var charMap = new Dictionary<char, Glyph>();
            do
            {
                var glyph = LoadGlyph(index);

                width += glyph.Width + 1;
                if (height < glyph.Height)
                    height = glyph.Height;

                charMap.Add((char) character, glyph);
                character = FT.FT_Get_Next_Char(_face.Face, character, out index);
            } while (index != 0 && charMap.Count <= 128);

            var texture = _factory.Create<ITexture2D>();
            texture.SetSize((int) height, (int) width);

            uint xOffset = 0;
            foreach (var glyph in charMap.Values)
            {
                var buffer = LoadCharacterBuffer(glyph.Index);
                texture.SetSubData(buffer, (int) xOffset, 0, (int) glyph.Height, (int) glyph.Width);
                glyph.XOffset = xOffset;
                xOffset += glyph.Width + 1;
            }

            return (charMap, texture);
        }

        private Glyph LoadGlyph(uint index)
        {
            FT.FT_Load_Glyph(_face.Face, index, FT.FT_LOAD_RENDER);
            unsafe
            {
                return new Glyph
                {
                    Index = index,
                    Width = _face.GlyphSlot->bitmap.width,
                    Height = _face.GlyphSlot->bitmap.rows,
                    BearingX = _face.GlyphSlot->bitmap_left,
                    BearingY = _face.GlyphSlot->bitmap_top,
                    Advance = (uint)_face.GlyphSlot->advance.x >> 6
                };
            }
        }
        
        private IntPtr LoadCharacterBuffer(uint index)
        {
            FT.FT_Load_Glyph(_face.Face, index, FT.FT_LOAD_RENDER);
            unsafe
            {
                return _face.GlyphSlot->bitmap.buffer;
            }
        }
    }
}
