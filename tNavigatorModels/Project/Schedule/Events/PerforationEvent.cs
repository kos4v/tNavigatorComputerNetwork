namespace tNavigatorModels.Project.Schedule.Events
{
    public abstract class PerforationEvent() : IBaseEvent
    {
        public string BoreholeName { get; set; }
        public int Step { get; set; }
        public int StartMD { get; set; }
        public int EndMD { get; set; }
        public string EventTNavName => "COMPDATMD";
        protected string[] _tNavArgs (string perforationStatus) => [
            "",
            $"'{BoreholeName}'",
            "1*",
            StartMD.ToString(),
            EndMD.ToString(),
            "MD",
            "OPEN",
            "2*",
            "0.114",
            "/"
        ];
        public virtual string TNavString() => throw new NotImplementedException();
    }

    public class OpenPerforationEvent() : PerforationEvent
    {
        // '{BoreholeName}' 1* {StartMD} {EndMD} MD OPEN 2* 0.114 /";
        public override string TNavString() => string.Join("\t", [
            "",
            $"'{BoreholeName}'",
            "1*",
            StartMD.ToString(),
            EndMD.ToString(),
            "MD",
            "OPEN",
            "2*",
            "0.114",
            "/"
        ]);
    }


    public class ClosePerforationEvent() : PerforationEvent
    {
        // '{BoreholeName}' 1* {StartMD} {EndMD} MD OPEN 2* 0.114 /";
        public override string TNavString() => string.Join("\t", [
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