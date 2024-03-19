namespace tNavigatorModels.Project.Schedule.Events
{
    public enum EnumControlType
    {
        Debit,
        Pressure
    }

    public class ChangeBoreholeControlEvent() : IBaseEvent
    {
        private static string ToTNavKeyWord(EnumControlType controlType) => controlType switch
        {
            EnumControlType.Debit => "BHP",
            EnumControlType.Pressure => "LRAT",
            _ => throw new ArgumentOutOfRangeException(nameof(controlType), controlType, null)
        };

        public string BoreholeName { get; set; }
        public EnumControlType ControlType { get; set; }

        public double DebitControlVolume { get; set; } = 20;
        public double DownholePressureControlValue { get; set; } = 200;

        public int Step { get; set; }
        public string EventTNavName => "WCONPROD";

        public string TNavString() => string.Join("\t", [
            "",
            BoreholeName,
            "*",
            ToTNavKeyWord(ControlType),
            "*",
            "*",
            "*",
            $"{DebitControlVolume}".Replace(',', '.'),
            "*",
            $"{DownholePressureControlValue}".Replace(',', '.'),
            "/"
        ]);
    }
}