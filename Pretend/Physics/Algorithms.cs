﻿using System;
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
            var simplex = new List<Vector3>();
            var direction = aPos - bPos;

            var aVertices = GetVertices(aPos, aOrientation, aSize);
            var bVertices = GetVertices(bPos, bOrientation, bSize);

            var support = Support(aVertices, bVertices, direction);
            simplex.Add(support);

            direction = support * -1;

            while (true)
            {
                support = Support(aVertices, bVertices, direction);

                if (Vector3.Dot(support, direction) <= 0)
                    return false;

                simplex.Insert(0, support);

                var (collision, newDirection) = NextSimplex(simplex, direction);
                if (collision) return true;

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

        private static (bool collision, Vector3 newDirection) NextSimplex(List<Vector3> simplex, Vector3 direction)
        {
            return simplex.Count switch
            {
                2 => Line(simplex, direction),
                3 => Triangle2D(simplex, direction), // 3 => Triangle(simplex, direction),
                4 => Tetrahedron(simplex, direction),
                _ => (false, direction)
            };
        }

        private static (bool collision, Vector3 newDirection) Line(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[0];
            var b = simplex[1];

            var ab = b - a;
            var ao = -a;

            Vector3 newDirection;
            if (SameDirection(ab, ao))
                newDirection = TripleProduct(ab, ao, ab);
            else
            {
                simplex.Remove(b);
                newDirection = ao;
            }

            return (false, newDirection);
        }

        private static (bool collision, Vector3 newDirection) Triangle(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[0];
            var b = simplex[1];
            var c = simplex[2];

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

                if (SameDirection(abc, ao))
                {
                    newDirection = abc;
                }
                else
                {
                    simplex[1] = c;
                    simplex[2] = b;
                    newDirection = -abc;
                }
            }

            return (false, newDirection);
        }

        private static (bool collision, Vector3 newDirection) Triangle2D(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[0];
            var b = simplex[1];
            var c = simplex[2];

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

                return (true, direction);
            }

            return (false, newDirection);
        }

        private static (bool collision, Vector3 newDirection) Tetrahedron(List<Vector3> simplex, Vector3 direction)
        {
            var a = simplex[0];
            var b = simplex[1];
            var c = simplex[2];
            var d = simplex[3];
            
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
                simplex[2] = b;
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
    }
}
