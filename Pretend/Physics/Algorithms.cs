using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public static class Algorithms
    {
        private static readonly Vector4[] Vertices = {
            new Vector4(0.5f, 0.5f, 0, 1), new Vector4(0.5f, -0.5f, 0, 1),
            new Vector4(-0.5f, -0.5f, 0, 1), new Vector4(-0.5f, 0.5f, 0, 1)
        };

        public static bool GJK(Vector3 aPos, Vector3 aOrientation, SizeComponent aSize,
                               Vector3 bPos, Vector3 bOrientation, SizeComponent bSize)
        {
            var iterations = 0;
            var simplex = new List<Vector3>();
            var direction = bPos - aPos;

            var aVertices = GetVertices(aPos, aOrientation, aSize);
            var bVertices = GetVertices(bPos, bOrientation, bSize);

            while (iterations++ < 20)
            {
                switch (simplex.Count)
                {
                    case 1:
                        direction *= -1;
                        break;
                    case 2:
                        {
                            var ab = simplex[1] - simplex[0];
                            var a0 = simplex[0] * -1;
                            var tmp = TripleProduct(ab, a0, Vector3.Zero);
                            direction = TripleProduct(tmp, ab, direction);
                        }
                        break;
                    case 3:
                        {
                            var ac = simplex[2] - simplex[0];
                            var ab = simplex[1] - simplex[0];
                            direction = TripleProduct(ac, ab, Vector3.Zero);

                            var a0 = simplex[0] * -1;
                            if (Vector3.Dot(direction, a0) < 0) direction *= -1;
                        }
                        break;
                    case 4:
                        var da = simplex[3] - simplex[0];
                        var db = simplex[3] - simplex[1];
                        var dc = simplex[3] - simplex[2];

                        var d0 = simplex[3] * -1;

                        var abdNorm = TripleProduct(da, db, Vector3.Zero);
                        var bcdNorm = TripleProduct(db, dc, Vector3.Zero);
                        var cadNorm = TripleProduct(dc, da, Vector3.Zero);

                        if (Vector3.Dot(abdNorm, d0) > 0)
                        {
                            simplex.Remove(simplex[2]);
                            direction = abdNorm;
                        }
                        else if (Vector3.Dot(bcdNorm, d0) > 0)
                        {
                            simplex.Remove(simplex[0]);
                            direction = bcdNorm;
                        }
                        else if (Vector3.Dot(cadNorm, d0) > 0)
                        {
                            simplex.Remove(simplex[1]);
                            direction = cadNorm;
                        }
                        else
                            return true;
                        break;
                }

                if (AddSupport(simplex, aVertices, bVertices, direction))
                    return false;
            }

            return false;
        }

        private static List<Vector3> GetVertices(Vector3 pos, Vector3 orientation, SizeComponent size)
        {
            var transform = Matrix4.Identity *
                Matrix4.CreateScale(size.Width, size.Height, 1) *
                Matrix4.CreateFromQuaternion(new Quaternion(orientation * ((float) Math.PI / 180f))) *
                Matrix4.CreateTranslation(pos);

            return Vertices.Select(vertex => (vertex * transform).Xyz).ToList();
        }

        // Returns true if no intersection is found
        private static bool AddSupport(List<Vector3> simplex, List<Vector3> aVertices, List<Vector3> bVertices, Vector3 direction)
        {
            var support = Support(aVertices, bVertices, direction);
            simplex.Add(support);
            return Vector3.Dot(direction, support) < 0;
        }

        private static Vector3 Support(List<Vector3> aVertices, List<Vector3> bVertices, Vector3 direction)
        {
            var aFurthest = FurthestPoint(aVertices, direction);
            var bFurthest = FurthestPoint(bVertices, direction * -1);

            return aFurthest - bFurthest;
        }

        private static Vector3 FurthestPoint(List<Vector3> vertices, Vector3 direction)
        {
            var maxDot = float.MinValue;
            var furthestPoint = Vector3.Zero;
            foreach (var vertex in vertices)
            {
                var dot = Vector3.Dot(vertex, direction);
                if (dot > maxDot)
                {
                    furthestPoint = vertex;
                    maxDot = dot;
                }
            }
            return furthestPoint;
        }

        private static Vector3 TripleProduct(Vector3 a, Vector3 b, Vector3 c)
        {
            var ac = Vector3.Dot(a, c);
            var ab = Vector3.Dot(a, b);

            return ac * b - ab * c;
        }
    }
}
