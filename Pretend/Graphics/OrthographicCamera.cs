using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public class OrthographicCamera : ICamera
    {
        private Matrix4 _viewProjection;
        private Matrix4 _view;
        private Matrix4 _projection;

        public OrthographicCamera(WindowAttributes windowAttributes)
        {
            Matrix4.CreateOrthographic((float) windowAttributes.Width, (float) windowAttributes.Height, -1f, 1f, out _projection);
            _view = Matrix4.Identity;
            _viewProjection = _view * _projection;
        }

        public Matrix4 ViewProjection => _viewProjection;

        public void Resize(int width, int height)
        {
            // NYI Correctly
            Matrix4.CreateOrthographic((float) width, (float) height, -1f, 1f, out _projection);
            _viewProjection = _view * _projection;
        }
    }
}
