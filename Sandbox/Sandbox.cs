using Pretend;

namespace Sandbox
{
    public enum ActiveLayer
    {
        ExampleLayer,
        Layer2D,
        PhysicsLayer,
        TextLayer
    }

    public interface ISandbox
    {
        ActiveLayer ActiveLayer { get; set; }
    }

    [Singleton]
    public class Sandbox : ISandbox
    {
        public ActiveLayer ActiveLayer { get; set; } = ActiveLayer.TextLayer;
    }
}
