namespace tNavigatorModels.Project.Schedule.Events
{
    public enum EnumControlTypeInjectionBorehole
    {
        /// <summary> RATE - Контроль по объему закачки </summary>
        VolumeOfLiquidInjection,

        /// <summary> BHP - Контроль по забойному давлению </summary>
        WellheadPressure
    }

    public class ChangeBoreholeToInjectionEvent : IBaseEvent
    {
        private static string ToTNavKeyWord(EnumControlTypeInjectionBorehole controlType) => controlType switch
        {
            EnumControlTypeInjectionBorehole.VolumeOfLiquidInjection => "RATE",
            EnumControlTypeInjectionBorehole.WellheadPressure => "BHP",
            _ => throw new ArgumentOutOfRangeException(nameof(controlType), controlType, null)
        };

        public int Step { get; set; }
        public string EventTNavName => "WCONINJE";
        public string BoreholeName { get; set; }

        public EnumControlTypeInjectionBorehole ControlType { get; set; }

        /// <summary> METRIC: rm3/day </summary>
        public int LiquidVolume { get; set; } = 1000000;

        /// <summary> METRIC: barsa </summary>
        public int WellheadPressure { get; set; } = 200;

        public string TNavString() =>
            string.Join("\t", [
                "",
                $"'{BoreholeName}'",
                "WATER",
                "OPEN",
                ToTNavKeyWord(ControlType),
                LiquidVolume,
                WellheadPressure,
                "/"
            ]);
    }
}