namespace tNavigatorModels.Project.Schedule.Events
{
    public interface IBaseEvent
    {
        public int Step { get; set; }
        public string EventTNavName { get; }
        public string TNavString();
    }

    public class EventSchedule
    {
        public ChangeBoreholeControlEvent[] ChangeBoreholeControlEvent { get; set; } = [];
        public OpenPerforationEvent[] OpenPerforationEvents { get; set; } = [];
        public ClosePerforationEvent[] ClosePerforationEvents { get; set; } = [];

        public IBaseEvent[] GetAllEvents() => OpenPerforationEvents.Cast<IBaseEvent>()
            .Concat(ClosePerforationEvents)
            .Concat(ChangeBoreholeControlEvent)
            .ToArray();
    }
}