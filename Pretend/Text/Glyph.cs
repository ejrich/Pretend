namespace Pretend.Text
{
    public class Glyph
    {
        public uint Index { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public int BearingX { get; set; }
        public int BearingY { get; set; }
        public uint Advance { get; set; }
        public uint XOffset { get; set; }
        public uint YOffset { get; set; }
    }
}
