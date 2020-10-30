﻿using OpenTK.Mathematics;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Sandbox
{
    public class TextLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly I2DRenderer _renderer;
        private readonly ITextRenderer _textRenderer;

        public TextLayer(ICamera camera, I2DRenderer renderer, ITextRenderer textRenderer)
        {
            _camera = camera;
            _renderer = renderer;
            _textRenderer = textRenderer;
        }

        public void Attach()
        {
            _renderer.Init();
        }

        public void Update(float timeStep)
        {
            _renderer.Begin(_camera);

            _textRenderer.RenderText("Hello world! yogurt pretty good", "Assets/Roboto-Medium.ttf", 60, new Vector3(-300, 0, 0));

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
        }
    }
}
