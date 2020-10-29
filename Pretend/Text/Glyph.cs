using System;

namespace Pretend.Text
{
    public class Glyph
    {
        public IntPtr Buffer { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public int BearingX { get; set; }
        public int BearingY { get; set; }
        public uint Advance { get; set; }
    }
}
