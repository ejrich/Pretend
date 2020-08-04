using System;
using Pretend.Graphics;
using SharpDX;

namespace Pretend.Editor
{
    public interface IFramebufferCapture
    {
        IFramebuffer Framebuffer { set; }
        Action<IFramebuffer> Callback { set; }
    }

    [Singleton]
    public class FramebufferCapture : IFramebufferCapture
    {
        public IFramebuffer Framebuffer { set => Callback(value); }
        public Action<IFramebuffer> Callback { private get; set; }
    }
}
