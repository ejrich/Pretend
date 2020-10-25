using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace Pretend.Audio
{
    public interface IListener
    {
        float Gain { set; }
        Vector3 Position { set; }
        Vector3 Velocity { set; }
        (Vector3 at, Vector3 up) Orientation { set; }
    }

    public class Listener : IListener
    {
        public float Gain { set => AL.Listener(ALListenerf.Gain, value); }

        public Vector3 Position { set => AL.Listener(ALListener3f.Position, ref value); }

        public Vector3 Velocity { set => AL.Listener(ALListener3f.Velocity, ref value); }

        public (Vector3 at, Vector3 up) Orientation { set => AL.Listener(ALListenerfv.Orientation, ref value.at, ref value.up); }
    }
}
