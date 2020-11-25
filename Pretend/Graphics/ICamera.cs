using System.Numerics;

namespace Pretend.Graphics
{
    public interface ICamera
    {
        Matrix4x4 ViewProjection { get; }
        Vector3 Position { get; set; }
        void Resize(int width, int height);
    }
}
