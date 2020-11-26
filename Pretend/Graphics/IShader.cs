using System;
using System.Numerics;

namespace Pretend.Graphics
{
    public interface IShader : IDisposable
    {
        void Compile(string embeddedFile);
        void Compile(string vertexFile, string fragmentFile);
        void Bind();
        void Unbind();
        void SetBool(string name, bool value);
        void SetInt(string name, int value);
        void SetFloat(string name, float value);
        void SetIntArray(string name, int[] value);
        void SetVec4(string name, Vector4 value);
        void SetMat4(string name, Matrix4x4 value);
    }
}
