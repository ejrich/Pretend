using System.Numerics;
using OpenTK.Audio.OpenAL;
using Pretend.Math;

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

        public Vector3 Position
        {
            set
            {
                var position = value.ToTKVector3();
                AL.Listener(ALListener3f.Position, ref position);
            }
        }

        public Vector3 Velocity
        {
            set
            {
                var velocity = value.ToTKVector3();
                AL.Listener(ALListener3f.Velocity, ref velocity);
            }
        }

        public (Vector3 at, Vector3 up) Orientation
        {
            set
            {
                var at = value.at.ToTKVector3();
                var up = value.up.ToTKVector3();
                AL.Listener(ALListenerfv.Orientation, ref at, ref up);
            }
        }
    }
}
