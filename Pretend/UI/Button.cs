using System;
using System.Numerics;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;

namespace Pretend.UI
{
    public class ButtonSettings
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public uint FontSize { get; set; }
        public Vector4 FontColor { get; set; } = Vector4.One;
        public Vector4 Color { get; set; } = Vector4.One;
        public ITexture2D Texture { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Size { get; set; }
    }

    public class Button
    {
        // TODO: These don't have to be public
        public PositionComponent _position;
        public SizeComponent _size;
        public ColorComponent _color;
        public TextComponent _text;
        private ButtonScript _script;

        public Action<Button> OnClick { private get; set; }
        public Action<Button> OnRelease { private get; set; }
        public Action<Button> OnMouseOver { private get; set; }
        public Action<Button> OnMouseLeave { private get; set; }
        public Action<Button> Update { private get; set; }

        public void Init(IScene scene, IEntity entity, ButtonSettings settings)
        {
            _position = scene.AddComponent(entity, new PositionComponent { Position = settings.Position });
            _size = scene.AddComponent(entity, new SizeComponent { Width = (uint)settings.Size.X, Height = (uint)settings.Size.Y });
            _color = scene.AddComponent(entity, new ColorComponent { Color = settings.Color });
            _text = scene.AddComponent(entity, new TextComponent
            {
                Text = settings.Text, Font = settings.Font, Size = settings.FontSize,
                RelativePosition = new Vector3(0, -2.5f, 0.01f), // TODO, Probably have to calculate Y
                Color = settings.FontColor
            });
            _script = scene.AddComponent(entity, new ButtonScript(_position, _size, this));
        }
        
        private class ButtonScript : IScriptComponent
        {
            private readonly Button _button;
            private readonly Vector2 _min;
            private readonly Vector2 _max;
            private bool _clicked;

            public ButtonScript(PositionComponent position, SizeComponent size, Button button)
            {
                _button = button;

                _min = new Vector2(position.Position.X - size.Width / 2f, position.Position.Y - size.Height / 2f);
                _max = new Vector2(position.Position.X + size.Width / 2f, position.Position.Y + size.Height / 2f);
            }

            public void Update(float timeStep)
            {
                _button.Update?.Invoke(_button);
            }

            public void HandleEvent(IEvent evnt)
            {
                switch (evnt)
                {
                    case MouseButtonPressedEvent mousePressed:
                        HandleMousePress(mousePressed);
                        break;
                    case MouseButtonReleasedEvent mouseReleased:
                        HandleMouseRelease(mouseReleased);
                        break;
                }
            }

            private void HandleMousePress(MouseButtonPressedEvent mousePressed)
            {
                if (_min.X > mousePressed.X || _max.X < mousePressed.X ||
                    _min.Y > mousePressed.Y || _max.Y < mousePressed.Y)
                    return;

                _clicked = true;
                _button.OnClick?.Invoke(_button);
                mousePressed.Processed = true;
            }

            private void HandleMouseRelease(MouseButtonReleasedEvent mouseReleased)
            {
                if (_min.X > mouseReleased.X || _max.X < mouseReleased.X ||
                    _min.Y > mouseReleased.Y || _max.Y < mouseReleased.Y)
                {
                    _clicked = false;
                    return;
                }

                if (_clicked)
                {
                    _button.OnRelease?.Invoke(_button);
                    mouseReleased.Processed = true;
                }
                _clicked = false;
            }
        }
    }
}
