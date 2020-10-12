using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public static class Algorithms
    {
        private static readonly Vector4[] _vertices = {
            new Vector4(0.5f, 0.5f, 0, 1), new Vector4(0.5f, -0.5f, 0, 1),
            new Vector4(-0.5f, -0.5f, 0, 1), new Vector4(-0.5f, 0.5f, 0, 1)
        };

        public static bool GJK(Vector3 aPos, Vector3 aOrientation, SizeComponent aSize,
                               Vector3 bPos, Vector3 bOrientation, SizeComponent bSize)
        {
            var simplex = new Vector3[3];
            var d = aPos - bPos;

            var aVertices = GetVertices(aPos, aOrientation, aSize);
            var bVertices = GetVertices(bPos, bOrientation, bSize);

            var a = simplex[0] = Support(aVertices, bVertices);

            return false;
        }

        private static List<Vector3> GetVertices(Vector3 pos, Vector3 orientation, SizeComponent size)
        {
            var transform = Matrix4.Identity *
                Matrix4.CreateScale(size.Width, size.Height, 1) *
                Matrix4.CreateFromQuaternion(new Quaternion(orientation * ((float) Math.PI / 180f))) *
                Matrix4.CreateTranslation(pos);

            return _vertices.Select(vertex => (vertex * transform).Xyz).ToList();
        }

        private static Vector3 Support(List<Vector3> aVertices, List<Vector3> bVertices)
        {
            return Vector3.Zero;
        }
    }
}
