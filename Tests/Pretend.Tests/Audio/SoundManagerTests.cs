using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Audio;
using Pretend.ECS;

namespace Pretend.Tests.Audio
{
    [TestClass]
    public class SoundManagerTests
    {
        private ISoundManager _target;
        
        private Mock<IAudioContext> _mockAudioContext;
        private Mock<IFactory> _mockFactory;
        private Mock<IListener> _mockListener;
        private IEntityContainer _entityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAudioContext = new Mock<IAudioContext>(MockBehavior.Strict);
            _mockAudioContext.Setup(_ => _.Create());
            _mockFactory = new Mock<IFactory>(MockBehavior.Strict);
            _mockListener = new Mock<IListener>(MockBehavior.Strict);
            _mockFactory.Setup(_ => _.Create<IListener>()).Returns(_mockListener.Object);
            _entityContainer = new EntityContainer();

            _target = new SoundManager(_mockAudioContext.Object, _mockFactory.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockAudioContext.VerifyAll();
            _mockFactory.VerifyAll();
            _mockListener.VerifyAll();
        }

        [TestMethod]
        public void PlaySounds_WhenNoListeners_DoesNotSetListenerProperties()
        {
            _target.PlaySounds(_entityContainer);

            _mockListener.VerifySet(_ => _.Gain = It.IsAny<float>(), Times.Never());
        }

        [TestMethod]
        public void PlaySounds_WhenActiveListener_SetsListenerProperties()
        {
            _mockListener.SetupSet(_ => _.Gain = It.IsAny<float>());
            _mockListener.SetupSet(_ => _.Position = It.IsAny<Vector3>());
            _mockListener.SetupSet(_ => _.Velocity = It.IsAny<Vector3>());

            var listener = _entityContainer.CreateEntity();
            _entityContainer.AddComponent(listener, new ListenerComponent { Active = true });
            _entityContainer.AddComponent(listener, new PositionComponent { X = 3, Y = 4, Z = 5 });
            _entityContainer.AddComponent(listener, new PhysicsComponent { Velocity = new Vector3(1, 2, 3) });

            _target.PlaySounds(_entityContainer);

            _mockListener.VerifySet(_ => _.Gain = 1);
            _mockListener.VerifySet(_ => _.Position = new Vector3(3, 4, 5));
            _mockListener.VerifySet(_ => _.Velocity = new Vector3(1, 2, 3));
        }

        [TestMethod]
        public void PlaySounds_WhenSourceWithoutPlay_DoesNotPlay()
        {
            var source = new Mock<ISource>();

            var sourceEntity = _entityContainer.CreateEntity();
            _entityContainer.AddComponent(sourceEntity, new SourceComponent { Source = source.Object });

            _target.PlaySounds(_entityContainer);

            source.VerifySet(_ => _.Position = It.IsAny<Vector3>(), Times.Never);
            source.VerifySet(_ => _.Velocity = It.IsAny<Vector3>(), Times.Never);
            source.Verify(_ => _.Play(It.IsAny<ISoundBuffer>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void PlaySounds_WhenSourceWithPlay_PlaysBuffer()
        {
            var source = new Mock<ISource>();
            source.SetupSet(_ => _.Position = It.IsAny<Vector3>());
            source.SetupSet(_ => _.Velocity = It.IsAny<Vector3>());
            var buffer = new Mock<ISoundBuffer>();

            var sourceEntity = _entityContainer.CreateEntity();
            _entityContainer.AddComponent(sourceEntity, new SourceComponent { Source = source.Object, SoundBuffer = buffer.Object, Play = true });
            _entityContainer.AddComponent(sourceEntity, new PositionComponent { X = 3, Y = 4, Z = 5 });
            _entityContainer.AddComponent(sourceEntity, new PhysicsComponent { Velocity = new Vector3(1, 2, 3) });

            _target.PlaySounds(_entityContainer);

            source.VerifySet(_ => _.Position = new Vector3(3, 4, 5));
            source.VerifySet(_ => _.Velocity = new Vector3(1, 2, 3));
            source.Verify(_ => _.Play(buffer.Object, false));
            Assert.IsFalse(sourceEntity.GetComponent<SourceComponent>().Play);
        }

        [TestMethod]
        public void SoundBufferLifecycleMethods_CreateAndDeleteObjects()
        {
            var buffer = new Mock<ISoundBuffer>(MockBehavior.Strict);
            buffer.Setup(_ => _.Dispose());
            _mockFactory.Setup(_ => _.Create<ISoundBuffer>()).Returns(buffer.Object);

            _target.CreateSoundBuffer();
            var result = _target.CreateSoundBuffer();
            _target.DeleteSoundBuffer(result);

            buffer.Verify(_ => _.Dispose(), Times.Once);
        }

        [TestMethod]
        public void SourceLifecycleMethods_CreateAndDeleteObjects()
        {
            var source = new Mock<ISource>(MockBehavior.Strict);
            source.Setup(_ => _.Dispose());
            _mockFactory.Setup(_ => _.Create<ISource>()).Returns(source.Object);

            _target.CreateSource();
            var result = _target.CreateSource();
            _target.DeleteSource(result);

            source.Verify(_ => _.Dispose(), Times.Once);
        }

        [TestMethod]
        public void Dispose_CleansUpUnmanagedData()
        {
            var buffer = new Mock<ISoundBuffer>(MockBehavior.Strict);
            buffer.Setup(_ => _.Dispose());
            _mockFactory.Setup(_ => _.Create<ISoundBuffer>()).Returns(buffer.Object);
            var source = new Mock<ISource>(MockBehavior.Strict);
            source.Setup(_ => _.Dispose());
            _mockFactory.Setup(_ => _.Create<ISource>()).Returns(source.Object);
            _mockAudioContext.Setup(_ => _.Destroy());

            _target.CreateSoundBuffer();
            _target.CreateSoundBuffer();
            _target.CreateSource();
            _target.CreateSource();
            _target.CreateSource();
            _target.Dispose();

            buffer.Verify(_ => _.Dispose(), Times.Exactly(2));
            source.Verify(_ => _.Dispose(), Times.Exactly(3));
        }
    }
}
