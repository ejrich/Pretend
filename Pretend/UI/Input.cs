using System;
using System.Numerics;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Text;

namespace Pretend.UI
{
    public class InputSettings
    {
        private string _font;
        private uint _fontSize;
        private Vector4 _fontColor = Vector4.One;
        private Vector4 _color = Vector4.One;
        private ITexture2D _texture;
        private Vector3 _position;
        private Vector2 _size;
        private bool _disabled;

        public string InitialValue { get; set; }

        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
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

    public interface IInput
    {
        public string Value { get; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public Action OnClick { set; }
        public Action<string> OnInput { set; }
        public Action<InputSettings> OnUpdate { set; }

        public void Init(IScene scene, InputSettings settings);
    }

    public class Input : IInput
    {
        private PositionComponent _position;
        private SizeComponent _size;
        private ColorComponent _color;
        private TextureComponent _texture;
        private TextComponent _text;

        public string Value { get; private set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
        public Action OnClick { private get; set; }
        public Action<string> OnInput { private get; set; }
        public Action<InputSettings> OnUpdate { private get; set; }

        public void Init(IScene scene, InputSettings settings)
        {
            var entity = scene.CreateEntity();

            Value = settings.InitialValue;
            Disabled = settings.Disabled;
            _position = scene.AddComponent(entity, new PositionComponent { Position = settings.Position });
            _size = scene.AddComponent(entity, new SizeComponent { Width = (uint)settings.Size.X, Height = (uint)settings.Size.Y });
            _color = scene.AddComponent(entity, new ColorComponent { Color = settings.Color });
            _texture = scene.AddComponent(entity, new TextureComponent { Texture = settings.Texture });
            _text = scene.AddComponent(entity, new TextComponent
            {
                Text = settings.InitialValue, Font = settings.Font, Size = settings.FontSize,
                RelativePosition = new Vector3(0, -2.5f, 0.01f), // TODO, Probably have to calculate Y
                Color = settings.FontColor
            });
            scene.AddComponent(entity, new InputScript(_position, _size, this, settings));
        }

        private void Update(InputSettings settings)
        {
            Disabled = settings.Disabled;
            _position.Position = settings.Position;
            _size.Width = (uint)settings.Size.X;
            _size.Height = (uint)settings.Size.Y;
            _color.Color = settings.Color;
            _texture.Texture = settings.Texture;
            _text.Text = settings.InitialValue;
            _text.Font = settings.Font;
            _text.Size = settings.FontSize;
            _text.Color = settings.FontColor;
            settings.Changed = false;
        }

        private void UpdateInputValue(char character)
        {
            Value += character;
            _text.Text = Value;
        }

        private class InputScript : IScriptComponent
        {
            private readonly Input _input;
            private readonly InputSettings _settings;
            private readonly Vector2 _min;
            private readonly Vector2 _max;

            public InputScript(PositionComponent position, SizeComponent size, Input input, InputSettings settings)
            {
                _input = input;
                _settings = settings;

                _min = new Vector2(position.Position.X - size.Width / 2f, position.Position.Y - size.Height / 2f);
                _max = new Vector2(position.Position.X + size.Width / 2f, position.Position.Y + size.Height / 2f);
            }

            public void Update(float timeStep)
            {
                if (_input.OnUpdate == null) return;

                _input.OnUpdate.Invoke(_settings);
                if (_settings.Changed)
                    _input.Update(_settings);
            }

            public void HandleEvent(IEvent evnt)
            {
                switch (evnt)
                {
                    case MouseButtonPressedEvent mousePressed:
                        HandleMousePress(mousePressed);
                        break;
                    case KeyPressedEvent keyPressed:
                        HandleKeyPress(keyPressed);
                        break;
                }
            }

            private void HandleMousePress(MouseButtonPressedEvent mousePressed)
            {
                if (_input.Disabled) return;

                if (MouseOutsideInput(mousePressed.X, mousePressed.Y))
                {
                    _input.Selected = false;
                    return;
                }

                _input.Selected = true;
                _input.OnClick?.Invoke();
                mousePressed.Processed = true;
            }

            private void HandleKeyPress(KeyPressedEvent keyPressed)
            {
                if (!_input.Selected || _input.Disabled) return;

                // Handle reserved chars
                if (keyPressed.KeyCode == KeyCode.Backspace || keyPressed.KeyCode == KeyCode.Space)
                    _input.UpdateInputValue((char)keyPressed.KeyCode);

                var character = keyPressed.KeyCode.GetChar(keyPressed.KeyMod);
                if (character != '\0')
                    _input.UpdateInputValue(character);

                keyPressed.Processed = true;
            }

            private bool MouseOutsideInput(float x, float y)
            {
                return _min.X > x || _max.X < x || _min.Y > y || _max.Y < y;
            }
        }
    }
}
