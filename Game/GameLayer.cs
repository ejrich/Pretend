using OpenToolkit.Mathematics;
using Pretend;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Game
{
    public class GameLayer : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _camera;

        private Renderable2DObject _object;
        private float _speed = 100;
        private bool _jumping;
        private float _jumpTime;

        public GameLayer(I2DRenderer renderer, ICamera camera)
        {
            _renderer = renderer;
            _camera = camera;
        }

        public void Attach()
        {
            _renderer.Init();

            _object = new Renderable2DObject
            {
                Width = 30, Height = 30,
                Color = new Vector4(1, 0, 0, 1)
            };
        }

        public void Update(float timeStep)
        {
            // Move the object
            _object.X += _speed * timeStep;
            if (_jumping)
            {
                _jumpTime += timeStep;
                var position = 300 * _jumpTime + -400 * _jumpTime * _jumpTime;
                
                _object.Y = position < 0 ? 0 : position;
                if (position <= 0)
                {
                    _jumpTime = 0;
                    _jumping = false;
                }
            }

            _renderer.Begin(_camera);

            _renderer.Submit(_object);

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
                    break;
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }
        
        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.Space:
                    Jump();
                    break;
            }
        }

        private void Jump()
        {
            if (_jumping) return;

            _jumping = true;
        }
    }
}
