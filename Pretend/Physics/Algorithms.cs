using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public class GJKResult
    {
        public bool Collision { get; set; }
        public List<Vector3> Simplex { get; set; }
        public List<Vector3> AVertices { get; set; }
        public List<Vector3> BVertices { get; set; }
    }

    public static class Algorithms
    {
        private static readonly Vector4[] Vertices = {
            new Vector4(0.5f, 0.5f, 0, 1), new Vector4(0.5f, -0.5f, 0, 1),
            new Vector4(-0.5f, -0.5f, 0, 1), new Vector4(-0.5f, 0.5f, 0, 1)
        };

        public static GJKResult GJK(IEntity a, IEntity b)
        {
            var aPosition = a.GetComponent<PositionComponent>();
            var aPos = new Vector3(aPosition.X, aPosition.Y, aPosition.Z);
            var aOr = new Vector3(aPosition.Pitch, aPosition.Roll, aPosition.Yaw);
            var bPosition = b.GetComponent<PositionComponent>();
            var bPos = new Vector3(bPosition.X, bPosition.Y, bPosition.Z);
            var bOr = new Vector3(bPosition.Pitch, bPosition.Roll, bPosition.Yaw);

            return GJK(aPos, aOr, a.GetComponent<SizeComponent>(), bPos, bOr, b.GetComponent<SizeComponent>());
        }

        public static GJKResult GJK(Vector3 aPos, Vector3 aOrientation, SizeComponent aSize,
            Vector3 bPos, Vector3 bOrientation, SizeComponent bSize)
        {
            var aVertices = GetVertices(aPos, aOrientation, aSize);
            var bVertices = GetVertices(bPos, bOrientation, bSize);

            return GJK(aPos, aVertices, bPos, bVertices, true);
        }

        public static GJKResult GJK(Vector3 aPos, List<Vector3> aVertices, Vector3 bPos, List<Vector3> bVertices, bool twoD = false)
        {
            var result = new GJKResult
            {
                Simplex = new List<Vector3>(),
                AVertices = aVertices,
                BVertices = bVertices
            };
            var direction = aPos - bPos;

            var support = Support(aVertices, bVertices, direction);
            result.Simplex.Add(support);

            direction = support * -1;

            while (true)
            {
                support = Support(aVertices, bVertices, direction);

                if (Vector3.Dot(support, direction) <= 0)
                    return new GJKResult();

                result.Simplex.Add(support);

                var (collision, newDirection) = NextSimplex(result.Simplex, direction, twoD);
                if (collision)
                {
                    result.Collision = true;
                    return result;
                }

                direction = newDirection;
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

        private static (bool collision, Vector3 newDirection) NextSimplex(List<Vector3> simplex, Vector3 direction, bool twoD)
        {
            return simplex.Count switch
            {
                2 => Line(simplex, direction),
                3 => Triangle(simplex, direction, twoD),
                4 => Tetrahedron(simplex, direction),
                _ => (false, direction)
            };
        }

        private static (bool collision, Vector3 newDirection) Line(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[1];
            var b = simplex[0];

            var ab = b - a;
            var ao = -a;

            Vector3 newDirection;
            if (SameDirection(ab, ao))
            {
                newDirection = TripleProduct(ab, ao, ab);
                if (newDirection.LengthSquared == 0)
                    newDirection = Vector3.Cross(ab, Vector3.One);
            }
            else
            {
                simplex.Remove(b);
                newDirection = ao;
            }

            return (false, newDirection);
        }

        private static (bool collision, Vector3 newDirection) Triangle(List<Vector3> simplex, Vector3 direction, bool twoD = false)
        {
            var a = simplex[2];
            var b = simplex[1];
            var c = simplex[0];

            var ab = b - a;
            var ac = c - a;
            var ao = -a;

            var abc = Vector3.Cross(ab, ac);

            Vector3 newDirection;
            if (SameDirection(Vector3.Cross(abc, ac), ao))
            {
                if (SameDirection(ac, ao))
                {
                    simplex.Remove(b);
                    newDirection = TripleProduct(ac, ao, ac);
                }
                else
                {
                    simplex.Remove(c);
                    return Line(simplex, direction);
                }
            }
            else
            {
                if (SameDirection(Vector3.Cross(ab, abc), ao))
                {
                    simplex.Remove(c);
                    return Line(simplex, direction);
                }
                
                if (twoD) return (true, direction);

                if (SameDirection(abc, ao))
                {
                    newDirection = abc;
                }
                else
                {
                    simplex[1] = c;
                    simplex[0] = b;
                    newDirection = -abc;
                }
            }

            return (false, newDirection);
        }

        private static (bool collision, Vector3 newDirection) Tetrahedron(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[3];
            var b = simplex[2];
            var c = simplex[1];
            var d = simplex[0];
            
            var ab = b - a;
            var ac = c - a;
            var ad = d - a;
            var ao = -a;

            var abc = Vector3.Cross(ab, ac);
            var acd = Vector3.Cross(ac, ad);
            var adb = Vector3.Cross(ad, ab);

            if (SameDirection(abc, ao))
            {
                simplex.Remove(d);
                return Triangle(simplex, direction);
            }
            if (SameDirection(acd, ao))
            {
                simplex.Remove(b);
                return Triangle(simplex, direction);
            }
            if (SameDirection(adb, ao))
            {
                simplex.Remove(c);
                simplex[1] = d;
                simplex[0] = b;
                return Triangle(simplex, direction);
            }

            return (true, direction);
        }

        private static bool SameDirection(Vector3 direction, Vector3 ao)
        {
            return Vector3.Dot(direction, ao) > 0;
        }

        private static Vector3 TripleProduct(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(Vector3.Cross(a, b), c);
        }

        public static Vector3 EPA(GJKResult result)
        {
            if (!result.Collision) return Vector3.Zero;

            return result.Simplex.Count switch
            {
                3 => EPA2D(result),
                4 => EPA3D(result),
                _ => Vector3.Zero
            };
        }

        private static Vector3 EPA2D(GJKResult result)
        {
            var simplex = result.Simplex;

            var e0 = (simplex[1].X - simplex[0].X) * (simplex[1].Y + simplex[0].Y);
            var e1 = (simplex[2].X - simplex[1].X) * (simplex[2].Y + simplex[1].Y);
            var e2 = (simplex[0].X - simplex[2].X) * (simplex[0].Y + simplex[2].Y);

            var clockWise = e0 + e1 + e2 > 0;

            var intersection = Vector3.Zero;
            for (var i = 0; i < 20; i++)
            {
                var edge = FindClosestEdge(simplex, clockWise);
                var support = Support(result.AVertices, result.BVertices, edge.Normal);
                var distance = Vector3.Dot(support, edge.Normal);

                intersection = edge.Normal * distance;
                if (Math.Abs(distance - edge.Distance) < 1e-6)
                    return FixError(intersection);

                simplex.Insert(edge.Index, support);
            }

            return intersection;
        }

        private static Vector3 EPA3D(GJKResult result)
        {
            var simplex = result.Simplex;

            var faces = new List<Face>
            {
                new Face(simplex[0], simplex[1], simplex[2]),
                new Face(simplex[0], simplex[2], simplex[3]),
                new Face(simplex[0], simplex[3], simplex[1]),
                new Face(simplex[1], simplex[3], simplex[2]),
            };

            var intersection = Vector3.Zero;
            for (int iteration = 0; iteration < 20; iteration++)
            {
                var minDistance = float.MaxValue;
                var closestFace = faces.First();
                foreach (var face in faces)
                {
                    var dot = Vector3.Dot(face.Vertices[0], face.Normal);
                    if (dot < minDistance)
                    {
                        minDistance = dot;
                        closestFace = face;
                    }
                }

                var support = Support(result.AVertices, result.BVertices, closestFace.Normal);
                var distance = Vector3.Dot(support, closestFace.Normal);

                intersection = closestFace.Normal * distance;
                if (Math.Abs(distance - minDistance) < 1e-6)
                    return FixError(intersection);
                
                var looseEdges = new List<Vector3[]>();

                for (var i = 0; i < faces.Count; i++)
                {
                    if (Vector3.Dot(faces[i].Normal, support - faces[i].Vertices[0]) <= 0) continue;

                    for (var j = 0; j < 3; j++)
                    {
                        var currentEdge = new[] { faces[i].Vertices[j], faces[i].Vertices[(j + 1) % 3] };
                        var edgeFound = false;
                        for (var k = 0; k < looseEdges.Count; k++)
                        {
                            if (looseEdges[k][1] == currentEdge[0] && looseEdges[k][0] == currentEdge[1])
                            {
                                looseEdges[k] = looseEdges[^1];
                                looseEdges.Remove(looseEdges[^1]);
                                edgeFound = true;
                                k = looseEdges.Count;
                            }
                        }

                        if (!edgeFound)
                        {
                            if (looseEdges.Count >= 32) break;
                            looseEdges.Add(currentEdge);
                        }
                    }

                    faces[i] = faces[^1];
                    faces.Remove(faces[^1]);
                    i--;
                }

                for (int i = 0; i < looseEdges.Count; i++)
                {
                    if (faces.Count >= 64) break;
                    var face = new Face(looseEdges[i][0], looseEdges[i][1], support);
                    faces.Add(face);

                    if (Vector3.Dot(face.Vertices[0], face.Normal) + 1e-6 < 0)
                    {
                        face.Vertices[0] = looseEdges[i][1];
                        face.Vertices[1] = looseEdges[i][0];
                        face.Normal *= -1;
                    }
                }
            }

            return intersection;
        }

        private static Edge FindClosestEdge(List<Vector3> simplex, bool clockWise)
        {
            var edge = new Edge
            {
                Distance = float.MaxValue
            };
            for (var i = 0; i < simplex.Count; i++)
            {
                var j = i + 1 >= simplex.Count ? 0 : i + 1;
                var (x, y, _) = simplex[j] - simplex[i];
                var normal = (clockWise ? new Vector3(y, -x, 0) : new Vector3(-y, x, 0)).Normalized();

                var distance = Vector3.Dot(normal, simplex[i]);
                if (distance < edge.Distance)
                {
                    edge.Distance = distance;
                    edge.Normal = normal;
                    edge.Index = j;
                }
            }

            return edge;
        }

        private static Vector3 FixError(Vector3 a)
        {
            const double error = 10e-10;
            return new Vector3(Math.Abs(a.X) < error ? 0 : a.X, Math.Abs(a.Y) < error ? 0 : a.Y, Math.Abs(a.Z) < error ? 0 : a.Z);
        }

        private class Edge
        {
            public float Distance { get; set; }
            public Vector3 Normal { get; set; }
            public int Index { get; set; }
        }

        private class Face
        {
            public Face(Vector3 a, Vector3 b, Vector3 c)
            {
                Vertices = new List<Vector3> { a, b, c };
                Normal = Vector3.Cross(b - a, c - a).Normalized();
            }

            public List<Vector3> Vertices { get; set; }
            public Vector3 Normal { get; set; }
        }
    }
}
