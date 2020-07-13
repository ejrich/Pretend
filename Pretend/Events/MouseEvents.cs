namespace Pretend.Events
{
    public class MouseMovedEvent : IEvent
    {
        public bool Processed { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class MouseScrollEvent : IEvent
    {
        public bool Processed { get; set; }
        public double XOffset { get; set; }
        public double YOffset { get; set; }
    }

    public class MouseButtonPressedEvent : IEvent
    {
        public bool Processed { get; set; }
        public MouseButton Button { get; set; }
    }

    public class MouseButtonReleasedEvent : IEvent
    {
        public bool Processed { get; set; }
        public MouseButton Button { get; set; }
    }
}
