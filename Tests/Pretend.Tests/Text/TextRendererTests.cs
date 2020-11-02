using System.Collections.Generic;
using System.Linq;
using FreeTypeSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Graphics;
using Pretend.Text;

namespace Pretend.Tests.Text
{
    [TestClass]
    public class TextRendererTests
    {
        private ITextRenderer _target;

        private Mock<I2DRenderer> _mockRenderer;
        private Mock<IFactory> _mockFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRenderer = new Mock<I2DRenderer>(MockBehavior.Strict);
            _mockFactory = new Mock<IFactory>(MockBehavior.Strict);

            _target = new TextRenderer(_mockRenderer.Object, _mockFactory.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRenderer.VerifyAll();
            _mockFactory.VerifyAll();
        }

        [TestMethod]
        public void RenderText_WhenFieldsNotProvided_Returns()
        {
            _target.RenderText(new RenderableTextObject());

            _mockRenderer.Verify(_ => _.Submit(It.IsAny<Renderable2DObject>()), Times.Never);
        }

        [TestMethod]
        public void RenderText_WhenLine_RendersCharacters()
        {
            var textObject = new RenderableTextObject
            {
                Text = "Hello world!", FontPath = "Something.ttf", Size = 10
            };

            var mockFont = new Mock<IFont>(MockBehavior.Strict);
            mockFont.Setup(_ => _.Load(It.IsAny<FreeTypeLibrary>(), It.IsAny<string>()));
            mockFont.Setup(_ => _.LoadTextureAtlas(It.IsAny<uint>())).Returns(CreateCharMap(textObject.Text));
            _mockFactory.Setup(_ => _.Create<IFont>()).Returns(mockFont.Object);
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));

            _target.RenderText(textObject);

            _mockRenderer.Verify(_ => _.Submit(It.IsAny<Renderable2DObject>()), Times.Exactly(12));
        }

        [TestMethod]
        public void RenderText_WhenMultipleLines_RendersCharacters()
        {
            var textObject = new RenderableTextObject
            {
                Text = "Hello world!\nHello again.", FontPath = "Something.ttf", Size = 10
            };

            var mockFont = new Mock<IFont>(MockBehavior.Strict);
            mockFont.Setup(_ => _.Load(It.IsAny<FreeTypeLibrary>(), It.IsAny<string>()));
            mockFont.Setup(_ => _.LoadTextureAtlas(It.IsAny<uint>())).Returns(CreateCharMap(textObject.Text));
            _mockFactory.Setup(_ => _.Create<IFont>()).Returns(mockFont.Object);
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));

            _target.RenderText(textObject);

            _mockRenderer.Verify(_ => _.Submit(It.IsAny<Renderable2DObject>()), Times.Exactly(24));
        }

        private static (IDictionary<char,Glyph> charMap, ITexture2D texture) CreateCharMap(string text)
        {
            var charMap = new Dictionary<char, Glyph>();
            foreach (var character in text)
            {
                charMap.TryAdd(character, new Glyph());
            }
            var texture = new Mock<ITexture2D>();

            return (charMap, texture.Object);
        }
    }
}
