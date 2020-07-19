namespace Pretend.Graphics
{
    public interface ITexture
    {
        void Bind(int slot);
    }

    public interface ITexture2D : ITexture
    {
        void SetData(string file);
    }
}
