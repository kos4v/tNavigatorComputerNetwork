namespace tNavigatorModels.Project.Schedule.Events
{
    public enum EnumControlTypeProductionBorehole
    {
        /// <summary> LRAT - Контроль по дебиту жидкости </summary>
        Debit,

        /// <summary> BHP - контроль по забойному давлению </summary>
        WellheadPressure
    }

    public class ChangeBoreholeToProductionEvent() : IBaseEvent
    {
        private static string ToTNavKeyWord(EnumControlTypeProductionBorehole controlTypeProductionBorehole) =>
            controlTypeProductionBorehole switch
            {
                EnumControlTypeProductionBorehole.Debit => "LRAT",
                EnumControlTypeProductionBorehole.WellheadPressure => "BHP",
                _ => throw new ArgumentOutOfRangeException(nameof(controlTypeProductionBorehole),
                    controlTypeProductionBorehole, null)
            };

        public string BoreholeName { get; set; }

        public EnumControlTypeProductionBorehole ControlTypeProductionBorehole { get; set; } =
            EnumControlTypeProductionBorehole.WellheadPressure;

        /// <summary> sm3/day </summary>
        public double DebitControlVolume { get; set; } = 20;

        /// <summary> barsa </summary>
        public double DownholePressureControlValue { get; set; } = 200;

        public int Step { get; set; }
        public string EventTNavName => "WCONPROD";
        public EnumBoreholeOperationModes? BoreholeMode { get; set; } = null;

        public string TNavString() => string.Join("\t", [
            "",
            BoreholeName,
            $"{(BoreholeMode == null ? "*" : BoreholeMode)}",
            ToTNavKeyWord(ControlTypeProductionBorehole),
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