namespace tNavigatorModels.Project.Schedule.Events
{
    public enum EnumEventName
    {
        OpenPerforation,
        ClosePerforation,
    }

    public interface IBaseEvent
    {
        public EnumEventName EventName { get; }
        public int Step { get; set; }
        public string EventTNavName { get; }
        public string TNavString();
    }

    public class EventSchedule
    {
        public ClosePerforationEvent[] ClosePerforationEvents { get; set; } = Array.Empty<ClosePerforationEvent>();
        public OpenPerforationEvent[] AddPerforationEvents { get; set; } = Array.Empty<OpenPerforationEvent>();

        public IBaseEvent[] GetAllEvents() => AddPerforationEvents
            .Concat(ClosePerforationEvents.Cast<IBaseEvent>())
            .ToArray();
    }
}