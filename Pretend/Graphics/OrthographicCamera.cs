using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public class OrthographicCamera : ICamera
    {
        private Matrix4 _view;
        private Matrix4 _projection;
        private Vector3 _position;

        public OrthographicCamera(IWindowAttributesProvider windowAttributes)
        {
            Matrix4.CreateOrthographic(windowAttributes.Width, windowAttributes.Height, -1f, 1f, out _projection);
            _view = Matrix4.Identity;
            _position = new Vector3();

            CalculateViewProjection();
        }

        public Matrix4 ViewProjection { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                var transform = Matrix4.Identity * Matrix4.CreateTranslation(_position);
                Matrix4.Invert(ref transform, out _view);

                CalculateViewProjection();
            }
        }

        public void Resize(int width, int height)
        {
            Matrix4.CreateOrthographic(width, height, -1f, 1f, out _projection);
            CalculateViewProjection();
        }

        private void CalculateViewProjection()
        {
            ViewProjection = _projection * _view;
        }
    }
}
