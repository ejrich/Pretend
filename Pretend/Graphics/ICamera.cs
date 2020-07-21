using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public interface ICamera
    {
        Matrix4 ViewProjection { get; }
        void Resize(int width, int height);
    }
}
