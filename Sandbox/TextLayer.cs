using OpenTK.Mathematics;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Text;

namespace Sandbox
{
    public class TextLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly I2DRenderer _renderer;
        private readonly ITextRenderer _textRenderer;

        private RenderableTextObject _textObject;

        public TextLayer(ICamera camera, I2DRenderer renderer, ITextRenderer textRenderer)
        {
            _camera = camera;
            _renderer = renderer;
            _textRenderer = textRenderer;
        }

        public void Attach()
        {
            _renderer.Init();

            _textObject = new RenderableTextObject
            {
                Text = "Hello world! yogurt pretty good",
                FontPath = "Assets/Roboto-Medium.ttf",
                Size = 60,
                // Alignment = TextAlignment.Left
                // Color = new Vector4(1, 0, 0, 1)
            };
        }

        public void Update(float timeStep)
        {
            _renderer.Begin(_camera);

            _textRenderer.RenderText(_textObject);

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
        }
    }
}
