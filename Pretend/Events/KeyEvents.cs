namespace Pretend.Events
{
    public class KeyPressedEvent : IEvent
    {
        public bool Processed { get; set; }
        public KeyCode KeyCode { get; set; }
        public KeyMod KeyMod { get; set; }
    }

    public class KeyReleasedEvent : IEvent
    {
        public bool Processed { get; set; }
        public KeyCode KeyCode { get; set; }
    }
}
