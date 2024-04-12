using System.Text.Json;

namespace tNavigatorModels.Result;
using CalculationResult = Dictionary<string, Dictionary<DateTime, double>>;

public enum EnumDataType
{
    WaterDebit,
    OilDebit,
    GasDebit,
    TotalDebit,
    Pressure,
}

public class BoreholeData
{
    public DateTime Date { get; set; }
    public string BoreholeName { get; set; }
    public EnumDataType DataType { get; set; }
    public List<double> MD { get; set; } = new();
    public List<double> Value { get; set; } = new();
}

public class ModelResult
{
    public string Report { get; set; } = "";
    public string TeamName { get; set; }
    public List<BoreholeData> BoreholeResults { get; set; } = new();
    public CalculationResult CalculationResult { get; set; } = new();

    public void ReadCalculationResult(string data)
        => CalculationResult = JsonSerializer.Deserialize<CalculationResult>(data)!;
}