using System.Numerics;

namespace Pretend.Graphics
{
    public class OrthographicCamera : ICamera
    {
        private Matrix4x4 _view;
        private Matrix4x4 _projection;
        private Vector3 _position;

        public OrthographicCamera(Settings settings)
        {
            _projection = Matrix4x4.CreateOrthographic(settings.ResolutionX, settings.ResolutionY, -1f, 1f);
            _view = Matrix4x4.Identity;
            _position = new Vector3();

            CalculateViewProjection();
        }

        public Matrix4x4 ViewProjection { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                var transform = Matrix4x4.Identity * Matrix4x4.CreateTranslation(_position);
                Matrix4x4.Invert(transform, out _view);

                CalculateViewProjection();
            }
        }

        public void Resize(int width, int height)
        {
            _projection = Matrix4x4.CreateOrthographic(width, height, -1f, 1f);
            CalculateViewProjection();
        }

        private void CalculateViewProjection()
        {
            ViewProjection = _projection * _view;
        }
    }
}
