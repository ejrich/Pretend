using System;
using System.Numerics;
using OpenTK.Audio.OpenAL;
using Pretend.Math;

namespace Pretend.Audio
{
    public interface ISource : IDisposable
    {
        float Gain { set; }
        Vector3 Position { set; }
        Vector3 Velocity { set; }
        void Play(ISoundBuffer buffer, bool loop);
        void Pause();
        void Stop();
        void Restart();
    }

    public class Source : ISource
    {
        private readonly int _id;
        private ISoundBuffer _buffer;

        public Source() => AL.GenSource(out _id);

        public float Gain { set => AL.Source(_id, ALSourcef.Gain, value); }
        public Vector3 Position
        {
            set
            {
                var position = value.ToTKVector3();
                AL.Source(_id, ALSource3f.Position, ref position);
            }
        }

        public Vector3 Velocity
        {
            set
            {
                var velocity = value.ToTKVector3();
                AL.Source(_id, ALSource3f.Velocity, ref velocity);
            }
        }

        public void Play(ISoundBuffer buffer, bool loop = false)
        {
            _buffer = buffer;
            AL.Source(_id, ALSourcei.Buffer, buffer.Id);
            AL.Source(_id, ALSourceb.Looping, loop);
            AL.SourcePlay(_id);
        }

        public void Pause() => AL.SourcePause(_id);

        public void Stop() => AL.SourceStop(_id);

        public void Restart()
        {
            Stop();
            Play(_buffer);
        }

        public void Dispose()
        {
            AL.DeleteSource(_id);
        }
    }
}
