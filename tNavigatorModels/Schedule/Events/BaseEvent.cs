namespace tNavigatorModels.Schedule.Events
{
    public enum EnumEventName
    {
        AddPerforation
    }

    public abstract class BaseEvent
    {
        public EnumEventName EventName { get; set; }
        public DateOnly Date { get; set; }
    }
}