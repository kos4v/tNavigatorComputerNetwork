using System.Text.Json;

namespace tNavigatorModels.Result;

using CalculationResult = Dictionary<string, Dictionary<DateTime, double>>;

public enum EnumDataType
{
    WaterDebit,
    OilDebit,
    GasDebit,
    TotalDebit
}

public enum EnumPointKeys
{
    /// <summary> FlowrateReservoir </summary>
    WVPR = 0,

    /// <summary> FlowrateWater </summary>
    WWPR = 1,

    /// <summary> FlowrateOil </summary>
    WOPR = 2,

    /// <summary> FlowrateGas </summary>
    WGPR = 3,

    /// <summary> FlowrateCondensate </summary>
    FlowrateCondensate = 4,

    /// <summary> PressureBottomhole </summary>
    WBHP = 5,

    /// <summary> PressureSupply </summary>
    WTHP = 6
}

public record MultiValuePoint(
    DateTime Date,
    double FlowrateReservoir,
    double FlowrateWater,
    double FlowrateOil,
    double FlowrateGas,
    double FlowrateCondensate,
    double PressureBottomhole,
    double PressureSupply);

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

    public MultiValuePoint[] GetPoints(string boreholeName)
    {
        boreholeName = boreholeName.ToUpper();
        var boreholeParamsHistory = CalculationResult
            .Where(cr => cr.Key.Contains(boreholeName))
            .ToDictionary(pair => pair.Key.Split(':').First(), pair => pair.Value);

        var result = boreholeParamsHistory[$"{EnumPointKeys.WOPR}"].Keys
            .Select(g =>
                new MultiValuePoint(
                    g,
                    boreholeParamsHistory[$"{EnumPointKeys.WVPR}"][g],
                    boreholeParamsHistory[$"{EnumPointKeys.WWPR}"][g],
                    boreholeParamsHistory[$"{EnumPointKeys.WOPR}"][g],
                    boreholeParamsHistory[$"{EnumPointKeys.WGPR}"][g],
                    0,
                    boreholeParamsHistory[$"{EnumPointKeys.WBHP}"][g],
                    boreholeParamsHistory[$"{EnumPointKeys.WTHP}"][g]
                )
            ).ToArray();
        return result;
    }
}