using System;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace Pretend.Audio
{
    public class Sound
    {
        public void Play(string file)
        {
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int[]) null);
            ALC.MakeContextCurrent(context);
            CheckError("Init");
        
            // Load file and place in buffer
            var wav = LoadWav(file);
            AL.GenBuffer(out var buffer);
            AL.BufferData(buffer, wav.Format, wav.Data, wav.Frequency);
            CheckError("Set Data");
        
            // Generate source and listener
            AL.Listener(ALListenerf.Gain, 1f);
            AL.Listener(ALListener3f.Position, 0, 0, 0);
            AL.Listener(ALListener3f.Velocity, 0, 0, 0);
            CheckError("Set Listener");
            AL.GenSource(out var source);
            AL.Source(source, ALSourcef.Gain, 1);
            AL.Source(source, ALSource3f.Position, 0, 0, 0);
            CheckError("Set Source");
        
            // Play sound
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(source);
            CheckError("Play");
        
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(context);
            ALC.CloseDevice(device);
        }
        
        private static void CheckError(string location)
        {
            var error = AL.GetError();
            Console.WriteLine($"{error} @ {location}");
        }
        
        private static WavFile LoadWav(string file)
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
        
        private class WavFile
        {
            public byte[] Data { get; set; }
            public ALFormat Format { get; set; }
            public int Frequency { get; set; }
        }
    }
}
