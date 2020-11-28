using System;
using System.Numerics;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;

namespace Pretend.UI
{
    public class ButtonSettings
    {
        private string _text;
        private string _font;
        private uint _fontSize;
        private Vector4 _fontColor = Vector4.One;
        private Vector4 _color = Vector4.One;
        private ITexture2D _texture;
        private Vector3 _position;
        private Vector2 _size;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Changed = true;
            }
        }

        public string Font
        {
            get => _font;
            set
            {
                _font = value;
                Changed = true;
            }
        }

        public uint FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Changed = true;
            }
        }

        public Vector4 FontColor
        {
            get => _fontColor;
            set
            {
                _fontColor = value;
                Changed = true;
            }
        }

        public Vector4 Color
        {
            get => _color;
            set
            {
                _color = value;
                Changed = true;
            }
        }

        public ITexture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                Changed = true;
            }
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                Changed = true;
            }
        }

        public Vector2 Size
        {
            get => _size;
            set
            {
                _size = value;
                Changed = true;
            }
        }

        public bool Changed { get; internal set; }
    }

    public interface IButton
    {
        public Action OnClick { set; }
        public Action OnRelease { set; }
        public Action OnMouseOver { set; }
        public Action OnMouseLeave { set; }
        public Action<ButtonSettings> OnUpdate { set; }

        public void Init(IScene scene, ButtonSettings settings);
    }

    public class Button : IButton
    {
        private PositionComponent _position;
        private SizeComponent _size;
        private ColorComponent _color;
        private TextComponent _text;
        private ButtonScript _script;

        public Action OnClick { private get; set; }
        public Action OnRelease { private get; set; }
        public Action OnMouseOver { private get; set; }
        public Action OnMouseLeave { private get; set; }
        public Action<ButtonSettings> OnUpdate { private get; set; }

        public void Init(IScene scene, ButtonSettings settings)
        {
            var entity = scene.CreateEntity();

            _position = scene.AddComponent(entity, new PositionComponent { Position = settings.Position });
            _size = scene.AddComponent(entity, new SizeComponent { Width = (uint)settings.Size.X, Height = (uint)settings.Size.Y });
            _color = scene.AddComponent(entity, new ColorComponent { Color = settings.Color });
            _text = scene.AddComponent(entity, new TextComponent
            {
                Text = settings.Text, Font = settings.Font, Size = settings.FontSize,
                RelativePosition = new Vector3(0, -2.5f, 0.01f), // TODO, Probably have to calculate Y
                Color = settings.FontColor
            });
            _script = scene.AddComponent(entity, new ButtonScript(_position, _size, this, settings));
        }

        private void Update(ButtonSettings settings)
        {
            _position.Position = settings.Position;
            _size.Width = (uint)settings.Size.X;
            _size.Height = (uint)settings.Size.Y;
            _color.Color = settings.Color;
            _text.Text = settings.Text;
            _text.Font = settings.Font;
            _text.Size = settings.FontSize;
            _text.Color = settings.FontColor;
            settings.Changed = false;
        }
        
        private class ButtonScript : IScriptComponent
        {
            private readonly Button _button;
            private readonly ButtonSettings _settings;
            private readonly Vector2 _min;
            private readonly Vector2 _max;
            private bool _clicked;
            private bool _onButton;

            public ButtonScript(PositionComponent position, SizeComponent size, Button button, ButtonSettings settings)
            {
                _button = button;
                _settings = settings;

                _min = new Vector2(position.Position.X - size.Width / 2f, position.Position.Y - size.Height / 2f);
                _max = new Vector2(position.Position.X + size.Width / 2f, position.Position.Y + size.Height / 2f);
            }

            public void Update(float timeStep)
            {
                if (_button.OnUpdate == null) return;

                _button.OnUpdate.Invoke(_settings);
                if (_settings.Changed)
                    _button.Update(_settings);
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
                    case MouseMovedEvent mouseMoved:
                        HandleMouseMove(mouseMoved);
                        break;
                }
            }

            private void HandleMousePress(MouseButtonPressedEvent mousePressed)
            {
                if (MouseOutsideButton(mousePressed.X, mousePressed.Y))
                {
                    _clicked = false;
                    return;
                }

                _clicked = true;
                _button.OnClick?.Invoke();
                mousePressed.Processed = true;
            }

            private void HandleMouseRelease(MouseButtonReleasedEvent mouseReleased)
            {
                if (MouseOutsideButton(mouseReleased.X, mouseReleased.Y))
                {
                    _clicked = false;
                    return;
                }

                if (_clicked)
                {
                    _button.OnRelease?.Invoke();
                    mouseReleased.Processed = true;
                }
                _clicked = false;
            }

            private void HandleMouseMove(MouseMovedEvent mouseMoved)
            {
                if (MouseOutsideButton(mouseMoved.X, mouseMoved.Y))
                {
                    if (_onButton)
                    {
                        _button.OnMouseLeave?.Invoke();
                        _onButton = false;
                    }
                    return;
                }

                if (!_onButton)
                    _button.OnMouseOver?.Invoke();

                _onButton = true;
            }

            private bool MouseOutsideButton(float x, float y)
            {
                return _min.X > x || _max.X < x || _min.Y > y || _max.Y < y;
            }
        }
    }
}
