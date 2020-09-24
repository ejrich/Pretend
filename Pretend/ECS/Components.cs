using OpenToolkit.Mathematics;
using Pretend.Events;
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

    public class CameraComponent : IComponent
    {
        public bool Active { get; set; }
        public ICamera Camera { get; set; }
    }

    public interface IScriptComponent : IComponent
    {
        void Attach() {}
        void Update(float timeStep);
        void HandleEvent(IEvent evnt) {}
        void Detach() {}
    }

    public class PhysicsComponent : IComponent
    {
        public bool Fixed { get; set; }
        public bool Solid { get; set; } = true;
        public Vector3 Velocity { get; set; } = Vector3.Zero;
    }
}
