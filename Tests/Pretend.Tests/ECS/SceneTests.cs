using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.ECS;
using Pretend.Graphics;
using Pretend.Text;

namespace Pretend.Tests.ECS
{
    [TestClass]
    public class SceneTests
    {
        private IScene _target;
        
        private Mock<I2DRenderer> _mockRenderer;
        private Mock<ITextRenderer> _mockTextRenderer;
        private Mock<IEntityContainer> _mockEntityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRenderer = new Mock<I2DRenderer>(MockBehavior.Strict);
            _mockTextRenderer = new Mock<ITextRenderer>(MockBehavior.Strict);
            _mockEntityContainer = new Mock<IEntityContainer>(MockBehavior.Strict);

            _target = new Scene(_mockRenderer.Object, _mockTextRenderer.Object, _mockEntityContainer.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRenderer.VerifyAll();
            _mockEntityContainer.VerifyAll();
        }

        [TestMethod]
        public void DeleteEntity_WhenNullComponent_Returns()
        {
            _target.DeleteEntity(null);
            
            _mockEntityContainer.Verify(_ => _.DeleteEntity(It.IsAny<IEntity>()), Times.Never);
        }

        [TestMethod]
        public void DeleteEntity_WithoutScriptComponent_DoesNotRunDetach()
        {
            var entity = new Mock<IEntity>();
            entity.Setup(_ => _.GetComponent<IScriptComponent>()).Returns((IScriptComponent) null);
            _mockEntityContainer.Setup(_ => _.DeleteEntity(entity.Object));

            _target.DeleteEntity(entity.Object);

            entity.Verify(_ => _.GetComponent<IScriptComponent>(), Times.Once);
        }

        [TestMethod]
        public void DeleteEntity_WithScriptComponent_RunsDetach()
        {
            var script = new Mock<IScriptComponent>();
            script.Setup(_ => _.Detach());
            var entity = new Mock<IEntity>();
            entity.Setup(_ => _.GetComponent<IScriptComponent>()).Returns(script.Object);
            _mockEntityContainer.Setup(_ => _.DeleteEntity(entity.Object));

            _target.DeleteEntity(entity.Object);

            entity.Verify(_ => _.GetComponent<IScriptComponent>(), Times.Once);
            script.Verify(_ => _.Detach(), Times.Once);
        }

        [TestMethod]
        public void AddComponent_WhenScriptComponent_AttachesScriptToEntity()
        {
            var entity = new Mock<IEntity>();
            var script = new Mock<IScriptComponent>(MockBehavior.Strict);
            script.Setup(_ => _.Attach());
            _mockEntityContainer.Setup(_ => _.AddComponent(entity.Object, script.Object));

            _target.AddComponent(entity.Object, script.Object);

            script.Verify(_ => _.Attach(), Times.Once);
            _mockEntityContainer.Verify(_ => _.AddComponent(entity.Object, script.Object), Times.Once);
        }

        [TestMethod]
        public void AddComponent_WhenOtherComponent_AttachesScriptToEntity()
        {
            var entity = new Mock<IEntity>();
            var position = new Mock<PositionComponent>(MockBehavior.Strict);
            _mockEntityContainer.Setup(_ => _.AddComponent(entity.Object, position.Object));

            _target.AddComponent(entity.Object, position.Object);

            _mockEntityContainer.Verify(_ => _.AddComponent(entity.Object, position.Object), Times.Once);
        }

        [TestMethod]
        public void Render_WhenNoCameraComponent_SetsNullAsCamera()
        {
            _mockEntityContainer.Setup(_ => _.GetComponents<CameraComponent>()).Returns(new List<CameraComponent>());
            _mockEntityContainer.SetupGet(_ => _.Entities).Returns(new List<IEntity> { new Entity(), new Entity() });
            _mockRenderer.Setup(_ => _.Begin(It.IsAny<ICamera>()));
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));
            _mockRenderer.Setup(_ => _.End());

            _target.Render();

            _mockRenderer.Verify(_ => _.Begin(null));
        }

        [TestMethod]
        public void Render_WhenMultipleCameras_SetsActiveCamera()
        {
            var activeCamera = new CameraComponent { Camera = new Mock<ICamera>().Object, Active = true };
            _mockEntityContainer.Setup(_ => _.GetComponents<CameraComponent>()).Returns(new List<CameraComponent>
            {
                new CameraComponent(), activeCamera
            });
            _mockEntityContainer.SetupGet(_ => _.Entities).Returns(new List<IEntity> { new Entity(), new Entity() });
            _mockRenderer.Setup(_ => _.Begin(It.IsAny<ICamera>()));
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));
            _mockRenderer.Setup(_ => _.End());

            _target.Render();

            _mockRenderer.Verify(_ => _.Begin(activeCamera.Camera));
        }

        [TestMethod]
        public void Render_WhenMultipleComponents_SubmitsMultipleObjects()
        {
            _mockEntityContainer.Setup(_ => _.GetComponents<CameraComponent>()).Returns(new List<CameraComponent>());
            _mockEntityContainer.SetupGet(_ => _.Entities).Returns(new List<IEntity> { new Entity(), new Entity() });
            _mockRenderer.Setup(_ => _.Begin(It.IsAny<ICamera>()));
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));
            _mockRenderer.Setup(_ => _.End());

            _target.Render();

            _mockRenderer.Verify(_ => _.Submit(It.IsAny<Renderable2DObject>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Render_WhenTextComponent_CallsTextRenderer()
        {
            var entity = new Entity();
            entity.AddComponent(new TextComponent { Text = "Hello world", Font = "Something.ttf", Size = 10 });

            _mockEntityContainer.Setup(_ => _.GetComponents<CameraComponent>()).Returns(new List<CameraComponent>());
            _mockEntityContainer.SetupGet(_ => _.Entities).Returns(new List<IEntity> { entity });
            _mockRenderer.Setup(_ => _.Begin(It.IsAny<ICamera>()));
            _mockRenderer.Setup(_ => _.Submit(It.IsAny<Renderable2DObject>()));
            _mockRenderer.Setup(_ => _.End());
            _mockTextRenderer.Setup(_ => _.RenderText(It.IsAny<RenderableTextObject>()));

            _target.Render();

            _mockTextRenderer.Verify(_ => _.RenderText(It.IsAny<RenderableTextObject>()), Times.Once);
        }
    }
}
