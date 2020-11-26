using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pretend.ECS;

namespace Pretend.Audio
{
    public interface ISoundManager : IDisposable
    {
        bool Running { get; }
        void Start(int hertz, IEntityContainer entityContainer);
        void Stop();
        void PlaySounds(IEntityContainer entityContainer);
        ISoundBuffer CreateSoundBuffer();
        void DeleteSoundBuffer(ISoundBuffer soundBuffer);
        ISource CreateSource();
        void DeleteSource(ISource source);
    }

    public class SoundManager : ISoundManager
    {
        private readonly IAudioContext _context;
        private readonly IFactory _factory;
        private readonly IListener _listener;

        private readonly List<ISoundBuffer> _buffers = new List<ISoundBuffer>();
        private readonly List<ISource> _sources = new List<ISource>();

        public SoundManager(IAudioContext context, IFactory factory)
        {
            _context = context;
            _factory = factory;

            _context.Create();
            _listener = _factory.Create<IListener>();
        }

        public bool Running { get; private set; }

        public void Start(int hertz, IEntityContainer entityContainer)
        {
            var ms = (int)(1000f / hertz);
            Running = true;
            var task = new Task(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (Running)
                {
                    PlaySounds(entityContainer);
                    stopwatch.Stop();
                    var dt = ms - (int)stopwatch.ElapsedMilliseconds;
                    if (dt > 0)
                        Thread.Sleep(dt);

                    stopwatch.Restart();
                }
            });
            task.Start();
        }

        public void Stop() => Running = false;

        public void PlaySounds(IEntityContainer entityContainer)
        {
            var listeners = entityContainer.GetEntitiesWithComponent<ListenerComponent>();
            var activeListener = listeners.FirstOrDefault(_ => _.GetComponent<ListenerComponent>().Active);
            if (activeListener != null)
            {
                var listener = activeListener.GetComponent<ListenerComponent>();
                var position = activeListener.GetComponent<PositionComponent>();
                var physics = activeListener.GetComponent<PhysicsComponent>();
                _listener.Gain = listener.Gain;
                if (position != null)
                    _listener.Position = position.Position;
                if (physics != null)
                    _listener.Velocity = physics.Velocity;
            }

            var entities = entityContainer.GetEntitiesWithComponent<SourceComponent>();

            foreach (var entity in entities)
            {
                var source = entity.GetComponent<SourceComponent>();
                var position = entity.GetComponent<PositionComponent>();
                var physics = entity.GetComponent<PhysicsComponent>();

                if (position != null)
                    source.Source.Position = position.Position;
                if (physics != null)
                    source.Source.Velocity = physics.Velocity;

                if (!source.Play) continue;

                source.Source.Play(source.SoundBuffer, source.Loop);
                source.Play = false;
            }
        }

        public ISoundBuffer CreateSoundBuffer()
        {
            var buffer = _factory.Create<ISoundBuffer>();
            _buffers.Add(buffer);

            return buffer;
        }

        public void DeleteSoundBuffer(ISoundBuffer soundBuffer)
        {
            _buffers.Remove(soundBuffer);
            soundBuffer.Dispose();
        }

        public ISource CreateSource()
        {
            var source =  _factory.Create<ISource>();
            _sources.Add(source);

            return source;
        }

        public void DeleteSource(ISource source)
        {
            _sources.Remove(source);
            source.Dispose();
        }

        public void Dispose()
        {
            foreach (var source in _sources)
                source.Dispose();
            _sources.Clear();

            foreach (var buffer in _buffers)
                buffer.Dispose();
            _buffers.Clear();

            _context.Destroy();
        }
    }
}
