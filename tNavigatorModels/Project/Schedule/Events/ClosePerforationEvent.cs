namespace tNavigatorModels.Project.Schedule.Events
{
    public class ClosePerforationEvent() : IBaseEvent
    {
        public string BoreholeName { get; set; }
        public EnumEventName EventName => EnumEventName.ClosePerforation;
        public int Step { get; set; }
        public int StartMD { get; set; }
        public int EndMD { get; set; }

        public string EventTNavName => "COMPDATMD";

        // '{BoreholeName}' 1* {StartMD} {EndMD} MD OPEN 2* 0.114 /";

        public string TNavString() => string.Join("\t", [
            "",
            $"'{BoreholeName}'",
            "1*",
            StartMD.ToString(),
            EndMD.ToString(),
            "MD",
            "SHUT",
            "2*",
            "0.114",
            "/"
        ]);

    }
}