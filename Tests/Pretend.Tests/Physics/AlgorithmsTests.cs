using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using Pretend.ECS;
using Pretend.Physics;

namespace Pretend.Tests.Physics
{
    [TestClass]
    public class AlgorithmsTests
    {
        [TestMethod]
        public void GJK_WhenNotColliding_ReturnsFalse()
        {
            var aPos = new Vector3(0, 0, 0);
            var aOrientation = new Vector3(0, 0, 0);
            var aSize = new SizeComponent { Width = 10, Height = 10 };
            var bPos = new Vector3(0, -20, 0);
            var bOrientation = new Vector3(0, 0, 0);
            var bSize = new SizeComponent { Width = 100, Height = 10 };

            var result = Algorithms.GJK(aPos, aOrientation, aSize, bPos, bOrientation, bSize);

            Assert.IsFalse(result.Collision);
        }

        [TestMethod]
        public void GJK_WhenColliding_ReturnsTrue()
        {
            var aPos = new Vector3(0, 0, 0);
            var aOrientation = new Vector3(0, 0, 0);
            var aSize = new SizeComponent { Width = 10, Height = 10 };
            var bPos = new Vector3(0, -8, 0);
            var bOrientation = new Vector3(0, 0, 0);
            var bSize = new SizeComponent { Width = 100, Height = 10 };

            var result = Algorithms.GJK(aPos, aOrientation, aSize, bPos, bOrientation, bSize);

            Assert.IsTrue(result.Collision);
        }

        [TestMethod]
        public void GJK_3D_WhenNotColliding_ReturnsFalse()
        {
            var aPos = new Vector3(0, 0, 0);
            var bPos = new Vector3(0, -20, 0);
            var aVertices = new List<Vector3>
            {
                new Vector3(-5, -5, 5), new Vector3(-5, 5, 5), new Vector3(5, 5, 5), new Vector3(5, -5, 5),
                new Vector3(-5, -5, -5), new Vector3(-5, 5, -5), new Vector3(5, 5, -5), new Vector3(5, -5, -5),
            };
            var bVertices = new List<Vector3>
            {
                new Vector3(-25, -25, 5), new Vector3(-25, -15, 5), new Vector3(-15, -15, 5), new Vector3(-15, -25, 5),
                new Vector3(-25, -25, -5), new Vector3(-25, -15, -5), new Vector3(-15, -15, -5), new Vector3(-15, -25, -5),
            };

            var result = Algorithms.GJK(aPos, aVertices, bPos, bVertices);

            Assert.IsFalse(result.Collision);
        }

        [TestMethod]
        public void GJK_3D_WhenColliding_ReturnsTrue()
        {
            var aPos = new Vector3(0, 0, 0);
            var bPos = new Vector3(0, -8, 0);
            var aVertices = new List<Vector3>
            {
                new Vector3(-5, -5, 5), new Vector3(-5, 5, 5), new Vector3(5, 5, 5), new Vector3(5, -5, 5),
                new Vector3(-5, -5, -5), new Vector3(-5, 5, -5), new Vector3(5, 5, -5), new Vector3(5, -5, -5),
            };
            var bVertices = new List<Vector3>
            {
                new Vector3(-13, -13, 5), new Vector3(-13, -3, 5), new Vector3(-3, -3, 5), new Vector3(-3, -13, 5),
                new Vector3(-13, -13, -5), new Vector3(-13, -3, -5), new Vector3(-3, -3, -5), new Vector3(-3, -13, -5),
            };

            var result = Algorithms.GJK(aPos, aVertices, bPos, bVertices);

            Assert.IsTrue(result.Collision);
        }

        [TestMethod]
        public void EPA_WhenColliding_ReturnsPenetrationVector()
        {
            var aPos = new Vector3(0, 0, 0);
            var aOrientation = new Vector3(0, 0, 0);
            var aSize = new SizeComponent { Width = 10, Height = 10 };
            var bPos = new Vector3(2, -8, 0);
            var bOrientation = new Vector3(0, 0, 0);
            var bSize = new SizeComponent { Width = 10, Height = 10 };

            var gjkResult = Algorithms.GJK(aPos, aOrientation, aSize, bPos, bOrientation, bSize);
            var (x, y, z) = Algorithms.EPA(gjkResult);

            Assert.AreEqual(0, x);
            Assert.AreEqual(-2, y);
            Assert.AreEqual(0, z);
        }

        [TestMethod]
        public void EPA_3D_WhenColliding_ReturnsPenetrationVector()
        {
            var aPos = new Vector3(0, 0, 0);
            var bPos = new Vector3(0, -8, 0);
            var aVertices = new List<Vector3>
            {
                new Vector3(-5, -5, 5), new Vector3(-5, 5, 5), new Vector3(5, 5, 5), new Vector3(5, -5, 5),
                new Vector3(-5, -5, -5), new Vector3(-5, 5, -5), new Vector3(5, 5, -5), new Vector3(5, -5, -5),
            };
            var bVertices = new List<Vector3>
            {
                new Vector3(-5, -13, 5), new Vector3(-5, -3, 5), new Vector3(5, -3, 5), new Vector3(5, -13, 5),
                new Vector3(-5, -13, -5), new Vector3(-5, -3, -5), new Vector3(5, -3, -5), new Vector3(5, -13, -5),
            };

            var gjkResult = Algorithms.GJK(aPos, aVertices, bPos, bVertices);
            var (x, y, z) = Algorithms.EPA(gjkResult);

            Assert.AreEqual(0, x);
            Assert.AreEqual(-2, y);
            Assert.AreEqual(0, z);
        }
    }
}
