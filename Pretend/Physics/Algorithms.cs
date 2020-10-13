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
            var index = 0;
            var simplex = new Vector3[3];
            var direction = aPos - bPos;

            var aVertices = GetVertices(aPos, aOrientation, aSize);
            var bVertices = GetVertices(bPos, bOrientation, bSize);

            var a = simplex[0] = Support(aVertices, bVertices, direction);

            if (Vector3.Dot(a, direction) <= 0)
                return false;

            direction = a * -1;

            while (true)
            {
                a = simplex[++index] = Support(aVertices, bVertices, direction);

                if (Vector3.Dot(a, direction) <= 0)
                    return false;

                var a0 = a * -1;

                if (index < 2)
                {
                    var b = simplex[0];
                    var ab = b - a;
                    direction = TripleProduct(ab, a0, ab);
                    continue;
                }

                // if (index < 3)
                // {
                //     var ac = simplex[2] - simplex[0];
                //     var ab = simplex[2] - simplex[0];
                //
                //     direction = Vector3.Cross(ac, ab);
                //
                //     var ao = simplex[0] * -1;
                //     if (Vector3.Dot(direction, ao) < 0) direction *= -1;
                //     continue;
                // }

                {
                    var ab = simplex[1] - a;
                    var ac = simplex[0] - a;
                    var acPerp = TripleProduct(ab, ac, ac);
                    
                    if (Vector3.Dot(acPerp, a0) >= 0)
                    {
                        direction = acPerp;
                    }
                    else
                    {
                        var abPerp = TripleProduct(ac, ab, ab);
                    
                        if (Vector3.Dot(abPerp, a0) < 0)
                            return true;
                    
                        simplex[0] = simplex[1];
                        direction = abPerp;
                    }
                    simplex[1] = simplex[2];
                    // var da = simplex[3] - simplex[0];
                    // var db = simplex[3] - simplex[1];
                    // var dc = simplex[3] - simplex[2];
                    //
                    // var d0 = simplex[3] * -1;
                    //
                    // var abdNorm = Vector3.Cross(da, db);
                    // var bcdNorm = Vector3.Cross(db, dc);
                    // var cadNorm = Vector3.Cross(dc, da);
                    //
                    // if (Vector3.Dot(abdNorm, d0) > 0)
                    // {
                    //     simplex[2] = simplex[3];
                    //     direction = abdNorm;
                    // }
                    // else if (Vector3.Dot(bcdNorm, d0) > 0)
                    // {
                    //     simplex[0] = simplex[1];
                    //     simplex[1] = simplex[2];
                    //     simplex[2] = simplex[3];
                    //     direction = bcdNorm;
                    // }
                    // else if (Vector3.Dot(cadNorm, d0) > 0)
                    // {
                    //     simplex[1] = simplex[2];
                    //     simplex[2] = simplex[3];
                    //     direction = cadNorm;
                    // }
                    // else
                    //     return true;
                }
                --index;
            }
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
            return Vector3.Cross(Vector3.Cross(a, b), c);
        }
    }
}
