using System;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

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
        public Vector3 Position { set => AL.Source(_id, ALSource3f.Position, ref value); }
        public Vector3 Velocity { set => AL.Source(_id, ALSource3f.Velocity, ref value); }

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
