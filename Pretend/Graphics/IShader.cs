namespace Pretend.Graphics
{
    public interface IShader
    {
        void Compile(string vertexFile, string fragmentFile);
        void Bind();
        void Unbind();
    }
}
