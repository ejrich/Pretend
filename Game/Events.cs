using Pretend.Events;

namespace Game
{
    public class GameResumeEvent : IEvent
    {
        public bool Processed { get; set; }
    }
}
