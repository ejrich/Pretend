namespace Pretend.Events
{
    public class WindowResiveEvent : IEvent
    {
        public bool Processed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class WindowCloseEvent : IEvent
    {
        public bool Processed { get; set; }
    }
}
