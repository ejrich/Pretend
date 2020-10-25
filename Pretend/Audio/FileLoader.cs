using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace Pretend.Audio
{
    public static class FileLoader
    {
        public static WavFile LoadWav(string file)
        {
            using var reader = new WaveFileReader(file);

            var buffer = new byte[reader.Length];
            reader.Read(buffer, 0, buffer.Length);

            return new WavFile
            {
                Data = buffer,
                Format = WaveFormatToALFormat(reader.WaveFormat),
                Frequency = reader.WaveFormat.SampleRate
            };
        }
        
        private static ALFormat WaveFormatToALFormat(WaveFormat waveFormat)
        {
            return waveFormat.Channels switch
            {
                1 => waveFormat.BitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
                2 => waveFormat.BitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
                _ => ALFormat.Stereo16
            };
        }

        public class WavFile
        {
            public byte[] Data { get; set; }
            public ALFormat Format { get; set; }
            public int Frequency { get; set; }
        }
    }
}
