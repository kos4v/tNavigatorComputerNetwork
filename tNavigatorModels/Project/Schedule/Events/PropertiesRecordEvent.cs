namespace tNavigatorModels.Project.Schedule.Events
{
    public class PropertiesRecordEvent : IBaseEvent
    {
        public int Step { get; set; }
        public string EventTNavName => "RPTMAPD";
        public string TNavString() => Schedule.DateTNavRow(Step) + " /";
    }
}