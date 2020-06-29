namespace Pretend.Events
{
    public interface IEvent
    {
        bool Processed { get; set; }
    }
}
