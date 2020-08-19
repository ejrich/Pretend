using OpenToolkit.Mathematics;
using Pretend.Graphics;

namespace Pretend.ECS
{
    public interface IComponent
    {
    }

    public class PositionComponent : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }
    }

    public class SizeComponent : IComponent
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
    }

    public class ColorComponent : IComponent
    {
        public Vector4 Color { get; set; }
    }

    public class TextureComponent : IComponent
    {
        public ITexture2D Texture { get; set; }
    }
}
