namespace Pretend
{
    public interface IWindow
    {
        void Init(WindowAttributes attributes);
        float GetTimestep();
        void OnUpdate();
        void Close();
    }

    public class WindowAttributes
    {
        public string Title { get; set; }
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
    }
}
