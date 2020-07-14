namespace Pretend
{
    public interface IInput
    {
        bool IsKeyPressed(KeyCode keyCode);
        bool IsMouseButtonPressed(MouseButton button);
        (int x, int y) GetMousePosition();
        int GetMouseX();
        int GetMouseY();
    }
}
