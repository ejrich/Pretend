using FreeTypeSharp;
using FreeTypeSharp.Native;

namespace Pretend.Text
{
    public class Font
    {
        public void Load(string font)
        {
            var lib = new FreeTypeLibrary();
            FT.FT_New_Face(lib.Native, font, 0, out var facePtr);
            var face = new FreeTypeFaceFacade(lib, facePtr);

            FT.FT_Done_Face(face.Face);
            lib.Dispose();
        }
    }
}
