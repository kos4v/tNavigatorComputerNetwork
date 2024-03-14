namespace tNavigatorModels.Project.Schedule.Events
{
    public class OpenPerforationEvent() : IBaseEvent
    {
        public int StartMD { get; set; }
        public int EndMD { get; set; }
        public string BoreholeName { get; set; }
        public EnumEventName EventName => EnumEventName.OpenPerforation;
        public int Step { get; set; }

        // public string COMPDATMDString() => $"\t'{BoreholeName}'\t1*\t{StartMD}\t{EndMD}\tMD\tOPEN\t2*\t0.114\t/";
        public string COMPDATMDString() => string.Join('\t', [
            "",
            $"'{BoreholeName}'",
            "1*",
            $"{StartMD}",
            $"{EndMD}",
            "MD",
            "OPEN",
            "2*",
            "0.114",
            "/",
        ]);

        public string EventTNavName => "WCONPROD";

        //public string TNavString() => $"\t'{BoreholeName}'\tOPEN\tBHP\t5*\t200\t/";
        public string TNavString() => string.Join('\t', [
            "",
            $"'{BoreholeName}'",
            "OPEN",
            "BHP",
            "5*",
            "200",
            "/",
        ]);
    }
}