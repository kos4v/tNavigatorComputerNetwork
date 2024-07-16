using System.ComponentModel.DataAnnotations;

namespace tNavigatorModels.Project.Schedule.Events
{

    /// <param name="AzimuthAngle">от 0 до 360</param>
    /// <param name="ZenithAngle">от 0 до 90</param>
    /// <param name="L1">L1 – левая полудлина трещины от ствола скважины (метры)</param>
    /// <param name="L2">L2 – правая полудлина трещины от ствола скважины (метры)</param>
    /// <param name="H1">H1 – высота трещины в одном направлении от ствола скважины (метры)</param>
    /// <param name="H2">H2 – высота трещины во втором направлении от ствола скважины (метры)</param>
    /// <param name="FZ">FZ - ширина трещины (метры)</param>
    /// <param name="NFZ">NFZ - ширина зоны влияния трещины(метры)</param>
    public record FractureTreatmentParams(
        double AzimuthAngle, double ZenithAngle, 
        double L1, double L2, 
        double H1, double H2, 
        double FZ, 
        double NFZ);

    public class FractureTreatmentEvent : IBaseEvent
    {
        [Required] public int Step { get; set; }
        [Required] public string BoreholeName { get; set; }
        [Required] public double MD { get; set; }
        

        public string EventTNavName => "FRACTURE_STAGE";
        public string TemplateName => $"'f{BoreholeName}_{MD}'";
        public FractureTreatmentParams Params = new (270, 0, 80, 80, 10, 10, 0.0002, 100);

        public string TNavString() => string.Join("\t", [
            TemplateName,
            "ON",
            "'ARITHMETIC_ONE'"
            ]);

        public string InitTemplateString() =>
            string.Join("\n", [
                $"'{BoreholeName}'",
                "*",
                TemplateName,
                MD,
                Params.AzimuthAngle,
                Params.ZenithAngle,
                Params.L1,
                Params.L2,
                Params.H1,
                Params.H2,
                Params.FZ,
                Params.NFZ
                ]);

        public static string InitEventString(IEnumerable<FractureTreatmentEvent> events) =>
            string.Join("\n", [
                "FRACTURE_SPECS",
                ..events.Select(e => e.InitTemplateString()),
                "/"
                ]);

    }
}
