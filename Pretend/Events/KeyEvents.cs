namespace Pretend.Events
{
    public class KeyPressedEvent : IEvent
    {
        public bool Processed { get; set; }
        public int KeyCode { get; set; }
    }

    public class KeyReleasedEvent : IEvent
    {
        public bool Processed { get; set; }
        public int KeyCode { get; set; }
    }
}
