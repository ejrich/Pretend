namespace Pretend.Graphics
{
    public interface ITexture
    {
        int Id { get; }
        void Bind(int slot = 0);
        void Delete();
    }

    public interface ITexture2D : ITexture
    {
        void SetData(string file);
    }
}
