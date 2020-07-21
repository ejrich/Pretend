using OpenToolkit.Mathematics;

namespace Pretend.Graphics
{
    public interface IShader
    {
        void Compile(string vertexFile, string fragmentFile);
        void Bind();
        void Unbind();
        void SetInt(string name, int value);
        void SetFloat(string name, float value);
        void SetMat4(string name, Matrix4 value);
    }
}
