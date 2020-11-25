using SDL2;

namespace Pretend.Windows
{
    public class SDLInput : IInput
    {
        private readonly IWindow _window;

        public SDLInput(IWindow window)
        {
            _window = window;
        }

        public bool IsKeyPressed(KeyCode keyCode)
        {
            var scancode = SDL.SDL_GetScancodeFromKey((SDL.SDL_Keycode) keyCode);

            var keysPointer = SDL.SDL_GetKeyboardState(out var _);

            unsafe {
                var keys = (byte*) keysPointer.ToPointer();

                return keys[(int) scancode] == 1;
            }
        }

        public bool IsMouseButtonPressed(MouseButton button)
        {
            var buttonPressed = SDL.SDL_GetMouseState(out var _, out var _) & SDL.SDL_BUTTON((uint) button);

            return buttonPressed != 0;
        }

        public (int x, int y) GetMousePosition()
        {
            SDL.SDL_GetMouseState(out var x, out var y);
            var w = _window.Resolution[0];
            var h = _window.Resolution[1];

            return (x - w / 2, h / 2 - y);
        }

        public int GetMouseX()
        {
            var (x, _) =  GetMousePosition();

            return x;
        }

        public int GetMouseY()
        {
            var (_, y) =  GetMousePosition();

            return y;
        }
    }
}
