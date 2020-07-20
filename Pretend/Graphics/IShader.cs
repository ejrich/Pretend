namespace Pretend.Graphics
{
    public interface IShader
    {
        void Compile(string vertexFile, string fragmentFile);
        void Bind();
        void Unbind();
        void SetInt(string name, int data);
        void SetFloat(string name, float data);
    }
}
