namespace Pretend
{
    public interface IApplication
    {
        void Start() {}
        void Stop() {}
        WindowAttributes Attributes { get; }
    }
}
