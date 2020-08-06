namespace Pretend.Graphics
{
    public interface ITexture
    {
        int Id { get; }
        void Bind(int slot = 0);
    }

    public interface ITexture2D : ITexture
    {
        void SetData(string file);
    }
}
