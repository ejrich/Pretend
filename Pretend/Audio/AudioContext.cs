using OpenTK.Audio.OpenAL;

namespace Pretend.Audio
{
    public interface IAudioContext
    {
        void Create();
        void Destroy();
    }

    public class AudioContext : IAudioContext
    {
        private ALDevice _device;
        private ALContext _context;

        public void Create()
        {
            if (_device == ALDevice.Null)
                _device = ALC.OpenDevice(null);
            if (_context == ALContext.Null)
                _context = ALC.CreateContext(_device, (int[]) null);
            ALC.MakeContextCurrent(_context);
        }

        public void Destroy()
        {
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_context);
            _context = ALContext.Null;
            ALC.CloseDevice(_device);
            _device = ALDevice.Null;
        }
    }
}
