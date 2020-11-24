using System;
using System.Collections.Generic;

namespace Pretend.Graphics
{
    public class BufferLayout
    {
        public Type Type { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }
        public bool Normalized { get; set; }
    }

    public interface IBuffer : IDisposable
    {
        void Bind();
        void Unbind();
    }

    public interface IVertexBuffer : IBuffer
    {
        void SetSize<T>(int count) where T : struct;
        void SetData(float[] vertices);
        void AddData<T>(T[] data) where T : struct;
        void AddLayout<T>(int count, bool normalized = false) where T : struct;
        IEnumerable<BufferLayout> Layouts { get; }
        int Stride { get; }
    }

    public interface IIndexBuffer : IBuffer
    {
        void AddData(uint[] indices);
        int Count { get; }
    }
}
