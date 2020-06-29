namespace Pretend.Events
{
    public class AppTickEvent : IEvent
    {
        public bool Processed { get; set; }
    }

    public class AppUpdateEvent : IEvent
    {
        public bool Processed { get; set; }
    }

    public class AppRenderEvent : IEvent
    {
        public bool Processed { get; set; }
    }
}
