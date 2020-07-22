namespace Pretend
{
    public interface IWindow
    {
        void Init();
        float GetTimestep();
        void OnUpdate();
        void Close();
    }

    public interface IWindowAttributesProvider
    {
        string Title { get; }
        int Width { get => 1280; }
        int Height { get => 720; }
    }
}
