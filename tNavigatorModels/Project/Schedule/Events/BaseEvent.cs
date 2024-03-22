using System.Reflection;

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
        public ChangeBoreholeToInjectionEvent[] ChangeBoreholeToInjectionEvents { get; set; } = [];
        public ChangeBoreholeToProductionEvent[] ChangeBoreholeToProductionEvents { get; set; } = [];
        public OpenPerforationEvent[] OpenPerforationEvents { get; set; } = [];
        public ClosePerforationEvent[] ClosePerforationEvents { get; set; } = [];
        public PropertiesRecordEvent[] PropertiesRecordEvents { get; set; } = [];


        public IEnumerable<IBaseEvent> GetAllEvents() => GetType()
            .GetProperties()
            .Select(pi => pi.GetValue(this, null))
            .OfType<IEnumerable<IBaseEvent>>()
            .SelectMany(c => c);


        //public IBaseEvent[] GetAllEvents() => OpenPerforationEvents.Cast<IBaseEvent>()
        //    .Concat(ClosePerforationEvents)
        //    .Concat(ChangeBoreholeToInjectionEvents)
        //    .Concat(ChangeBoreholeToProductionEvents)
        //    .Concat(PropertiesRecordEvents)
        //    .ToArray();
    }
}