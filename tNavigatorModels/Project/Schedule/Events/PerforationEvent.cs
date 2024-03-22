namespace tNavigatorModels.Project.Schedule.Events
{
    public abstract class PerforationEvent() : IBaseEvent
    {
        public string BoreholeName { get; set; }
        public int Step { get; set; }
        public int StartMD { get; set; }
        public int EndMD { get; set; }
        public string EventTNavName => "COMPDATMD";

        // '{BoreholeName}' 1* {StartMD} {EndMD} MD {perforationStatus:OPEN||SHUT} 2* 0.114 /";
        protected string _tNavArgs (string perforationStatus) => string.Join("\t", [
            "",
            $"'{BoreholeName}'",
            "1*",
            StartMD.ToString(),
            EndMD.ToString(),
            "MD",
            $"{perforationStatus}",
            "2*",
            "0.114",
            "/"
        ]);
        public virtual string TNavString() => throw new NotImplementedException();
    }

    public class OpenPerforationEvent() : PerforationEvent
    {
        public override string TNavString() => _tNavArgs("OPEN");
    }


    public class ClosePerforationEvent() : PerforationEvent
    {
        public override string TNavString() => _tNavArgs("SHUT");
    }
}