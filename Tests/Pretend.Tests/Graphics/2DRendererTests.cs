using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenToolkit.Mathematics;
using Pretend.Graphics;

namespace Pretend.Tests.Graphics
{
    [TestClass]
    public class Renderer2DTests
    {
        private I2DRenderer _target;

        private Mock<IRenderContext> _mockRenderContext;
        private Mock<IFactory> _mockFactory;

        private Mock<IVertexBuffer> _mockVertexBuffer;
        private Mock<IIndexBuffer> _mockIndexBuffer;
        private Mock<IVertexArray> _mockVertexArray;
        private Mock<IShader> _mockShader;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRenderContext = new Mock<IRenderContext>(MockBehavior.Strict);
            _mockFactory = new Mock<IFactory>(MockBehavior.Strict);

            _mockVertexBuffer = new Mock<IVertexBuffer>(MockBehavior.Strict);
            _mockIndexBuffer = new Mock<IIndexBuffer>(MockBehavior.Strict);
            _mockVertexArray = new Mock<IVertexArray>(MockBehavior.Strict);
            _mockShader = new Mock<IShader>(MockBehavior.Strict);

            _target = new Renderer2D(_mockRenderContext.Object, _mockFactory.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRenderContext.VerifyAll();
            _mockFactory.VerifyAll();
        }

        [TestMethod]
        public void Init_CreatesEmptyVertexBufferAndShader()
        {
            SetupInitMocks();

            _target.Init();

            _mockVertexBuffer.Verify(_ => _.SetSize<Renderable2DBuffer>(Renderer2D.MaxSubmissions * Renderer2D.VerticesInSubmission), Times.Once);
            _mockVertexBuffer.Verify(_ => _.AddLayout<float>(It.IsAny<int>(), false), Times.Exactly(4));
            _mockIndexBuffer.Verify(_ => _.AddData(It.IsAny<uint[]>()), Times.Once);
            _mockShader.Verify(_ => _.Compile(It.IsAny<string>()), Times.Once);
            _mockShader.Verify(_ => _.SetIntArray(It.IsAny<string>(), It.IsAny<int[]>()), Times.Once);
        }

        [TestMethod]
        public void RenderFlow_WithFewerThanMaxSubmissions_DoesNotFlushUntilEnd()
        {
            const int submissions = Renderer2D.MaxSubmissions - 10;

            SetupInitMocks();
            SetupRenderMocks();

            _target.Init();
            _target.Begin(new Mock<ICamera>().Object);

            foreach (var i in Enumerable.Range(0, submissions))
            {
                _target.Submit(new Renderable2DObject
                {
                    X = i, Width = Convert.ToUInt32(i), Height = Convert.ToUInt32(i)
                });
            }

            _target.End();

            _mockRenderContext.Verify(_ => _.Draw(_mockVertexArray.Object, submissions * Renderer2D.IndicesInSubmission), Times.Once);
        }

        [TestMethod]
        public void RenderFlow_WithMoreThanMaxSubmissions_FlushesTwice()
        {
            const int submissions = Renderer2D.MaxSubmissions + 10;

            SetupInitMocks();
            SetupRenderMocks();

            _target.Init();
            _target.Begin(new Mock<ICamera>().Object);

            foreach (var i in Enumerable.Range(0, submissions))
            {
                _target.Submit(new Renderable2DObject
                {
                    X = i, Width = Convert.ToUInt32(i), Height = Convert.ToUInt32(i)
                });
            }

            _target.End();

            _mockRenderContext.Verify(_ => _.Draw(_mockVertexArray.Object, It.IsAny<int>()), Times.Exactly(2));
        }
        
        [TestMethod]
        public void RenderFlow_WithTextures_BindsBeforeDrawing()
        {
            const int submissions = Renderer2D.MaxSubmissions - 10;

            SetupInitMocks();
            SetupRenderMocks();

            _target.Init();
            _target.Begin(new Mock<ICamera>().Object);
            
            var texture = new Mock<ITexture2D>();

            foreach (var i in Enumerable.Range(0, submissions))
            {
                _target.Submit(new Renderable2DObject
                {
                    X = i, Width = Convert.ToUInt32(i), Height = Convert.ToUInt32(i),
                    Texture = texture.Object
                });
            }

            _target.End();

            _mockRenderContext.Verify(_ => _.Draw(_mockVertexArray.Object, It.IsAny<int>()), Times.Once);
            texture.Verify(_ => _.Bind(0), Times.Once);
        }
        
        [TestMethod]
        public void RenderFlow_WithMultipleTextures_RendersWhenTextureSlotsFull()
        {
            const int submissions = Renderer2D.MaxTextures * 5;

            SetupInitMocks();
            SetupRenderMocks();

            _target.Init();
            _target.Begin(new Mock<ICamera>().Object);
            
            foreach (var i in Enumerable.Range(0, submissions))
            {
                _target.Submit(new Renderable2DObject
                {
                    X = i, Width = Convert.ToUInt32(i), Height = Convert.ToUInt32(i),
                    Texture = new Mock<ITexture2D>().Object
                });
            }

            _target.End();

            _mockRenderContext.Verify(_ => _.Draw(_mockVertexArray.Object, It.IsAny<int>()), Times.Exactly(5));
        }

        private void SetupInitMocks()
        {
            _mockRenderContext.Setup(_ => _.Init());
            _mockFactory.Setup(_ => _.Create<IVertexBuffer>()).Returns(_mockVertexBuffer.Object);
            _mockFactory.Setup(_ => _.Create<IIndexBuffer>()).Returns(_mockIndexBuffer.Object);
            _mockFactory.Setup(_ => _.Create<IVertexArray>()).Returns(_mockVertexArray.Object);
            _mockFactory.Setup(_ => _.Create<IShader>()).Returns(_mockShader.Object);

            _mockVertexBuffer.Setup(_ => _.SetSize<Renderable2DBuffer>(It.IsAny<int>()));
            _mockVertexBuffer.Setup(_ => _.AddLayout<float>(It.IsAny<int>(), false));
            _mockIndexBuffer.Setup(_ => _.AddData(It.IsAny<uint[]>()));
            _mockVertexArray.SetupSet(_ => _.VertexBuffer = _mockVertexBuffer.Object);
            _mockVertexArray.SetupSet(_ => _.IndexBuffer = _mockIndexBuffer.Object);
            _mockShader.Setup(_ => _.Compile(It.IsAny<string>()));
            _mockShader.Setup(_ => _.SetIntArray(It.IsAny<string>(), It.IsAny<int[]>()));
        }

        private void SetupRenderMocks()
        {
            _mockRenderContext.Setup(_ => _.Clear());
            _mockRenderContext.Setup(_ => _.Draw(_mockVertexArray.Object, It.IsAny<int>()));
            _mockVertexBuffer.Setup(_ => _.AddData(It.IsAny<Renderable2DBuffer[]>()));
            _mockShader.Setup(_ => _.Bind());
            _mockShader.Setup(_ => _.SetMat4("viewProjection", It.IsAny<Matrix4>()));
            _mockVertexArray.Setup(_ => _.Bind());
            _mockVertexArray.SetupGet(_ => _.VertexBuffer).Returns(_mockVertexBuffer.Object);
        }
    }
}
