using System.Numerics;
using Pretend.Audio;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Text;

namespace Pretend.ECS
{
    public interface IComponent
    {
    }

    public class PositionComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }

    public class SizeComponent : IComponent
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
    }

    public class ColorComponent : IComponent
    {
        public Vector4 Color { get; set; } = Vector4.One;
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
        public bool Kinematic { get; set; }
        public bool Solid { get; set; } = true;
        public float Mass { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public Vector3 Force { get; set; }
    }

    public class ListenerComponent : IComponent
    {
        public bool Active { get; set; }
        public float Gain { get; set; } = 1;
    }

    public class SourceComponent : IComponent
    {
        public ISource Source { get; set; }
        public ISoundBuffer SoundBuffer { get; set; }
        public bool Play { get; set; }
        public bool Loop { get; set; }
    }

    public class TextComponent : IComponent
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public uint Size { get; set; }
        public TextAlignment Alignment { get; set; } = TextAlignment.Center;
        public Vector3 RelativePosition { get; set; } = Vector3.Zero;
        public Vector3 Orientation { get; set; } = Vector3.Zero;
        public Vector4 Color { get; set; } = Vector4.One;
    }
}
