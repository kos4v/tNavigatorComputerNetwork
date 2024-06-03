using Microsoft.VisualBasic.CompilerServices;

namespace tNavigatorModels.Project.Schedule.Events
{
    public enum EnumStimulationType
    {
        /// <summary> Обработка призабойной зоны пласта" (ОПЗП) — это "Near-Wellbore Treatment" (NWT) </summary>
        NWT = 1,

        /// <summary> "Гидравлический Разрыв Пласта" (ГРП) — это "Hydraulic Fracturing" (HF). </summary>
        HF = 2
    }

    public abstract class PerforationEvent() : IBaseEvent
    {
        public string BoreholeName { get; set; }
        public int Step { get; set; }
        public int StartMD { get; set; }
        public int EndMD { get; set; }
        public string EventTNavName => "COMPDATMD";

        // '{BoreholeName}' 1* {StartMD} {EndMD} MD {perforationStatus:OPEN||SHUT} 2* 0.114 /";
        /// <summary> </summary>
        /// <param name="perforationStatus">SHIUT || OPEN</param>
        /// <param name="stimulationValue">это численное значения</param>
        /// <returns></returns>
        protected string _tNavArgs(string perforationStatus = "*", string stimulationValue = "*") => string.Join("\t", [
            "",
            $"'{BoreholeName}'",
            "1*",
            StartMD.ToString(),
            EndMD.ToString(),
            "MD",
            $"{perforationStatus}",
            "2*",
            "0.114",
            "*",
            // stimulationValue - skin factor
            $"{stimulationValue}".Replace(",", "."),
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

    // stimulationValue - skin factor
    public class StimulationPerforationEvent() : PerforationEvent
    {
        public EnumStimulationType[] TypeOfStimulations { get; set; }

        public static double GetStimulationTypeCost(EnumStimulationType stimulationType) => stimulationType switch
        {
            EnumStimulationType.NWT => -2,
            EnumStimulationType.HF => -4,
            _ => throw new ArgumentOutOfRangeException(nameof(stimulationType), stimulationType, null)
        };

        public override string TNavString() =>
            _tNavArgs(stimulationValue: $"{TypeOfStimulations.Sum(GetStimulationTypeCost)}");
    }
}