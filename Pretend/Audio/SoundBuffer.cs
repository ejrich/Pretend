using System;
using OpenTK.Audio.OpenAL;

namespace Pretend.Audio
{
    public interface ISoundBuffer : IDisposable
    {
        int Id { get; }
        void SetData(string file);
    }

    public class SoundBuffer : ISoundBuffer
    {
        public SoundBuffer() => Id = AL.GenBuffer();

        public int Id { get; }

        public void SetData(string file)
        {
            var wavFile = FileLoader.LoadWav(file);
            AL.BufferData(Id, wavFile.Format, wavFile.Data, wavFile.Frequency);
        }

        public void Dispose()
        {
            AL.DeleteBuffer(Id);
        }
    }
}
