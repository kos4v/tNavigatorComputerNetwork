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

public enum EnumPointKeys
{
    /// <summary> Well FlowrateReservoir </summary>
    WVPR = 0,

    /// <summary> Well FlowrateWater </summary>
    WWPR = 1,

    /// <summary> Well FlowrateOil </summary>
    WOPR = 2,

    /// <summary> Well FlowrateGas </summary>
    WGPR = 3,

    /// <summary> Well FlowrateCondensate </summary>
    FlowrateCondensate = 4,

    /// <summary> Well PressureBottomhole </summary>
    WBHP = 5,

    /// <summary> Well PressureSupply </summary>
    WTHP = 6,

    /// <summary> Full FlowrateResevoir Дебит жидкости для месторождения</summary>
    FVPR = 7,

    /// <summary> Full FlowrateOil  Дебит нефти для месторождения </summary>
    FOPR = 8,

    /// <summary> Full FlowrateWater Дебит воды для месторождения</summary>
    FWPR = 9,

    /// <summary> Full FlowrateGas Дебит газа для месторождения</summary>
    FGPR = 10,
}

public enum EnumCubeProperty
{
    /// <summary> Куб давления </summary>
    PRESSURE,

    /// <summary> Куб запасов газа в пластовых условиях </summary>
    RFIPGAS,

    /// <summary> Куб запасов нефти в пластовых условиях </summary>
    RFIPOIL,

    /// <summary> Куб запасов воды в пластовых условиях </summary>
    RFIPWAT,

    /// <summary> КУб насыщенности нефтью сеточных блоков  </summary>
    SOIL,

    /// <summary> КУб насыщенности газом сеточных блоков  </summary>
    SGAS,

    /// <summary> КУб насыщенности водой сеточных блоков  </summary>
    SWAT
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

    public MultiValuePoint[] GetPoints()
    {
        if (!CalculationResult.ContainsKey($"{EnumPointKeys.FOPR}"))
            return [];

        var result = CalculationResult[$"{EnumPointKeys.FOPR}"].Keys.Select(g =>
            new MultiValuePoint(
                g,
                CalculationResult[$"{EnumPointKeys.FVPR}"][g],
                CalculationResult[$"{EnumPointKeys.FWPR}"][g],
                CalculationResult[$"{EnumPointKeys.FOPR}"][g],
                CalculationResult[$"{EnumPointKeys.FGPR}"][g],
                0,
                0,
                0
            )).ToArray();
        return result;
    }

    public MultiValuePoint[] GetPoints(string boreholeName)
    {
        boreholeName = boreholeName.ToUpper();
        var boreholeParamsHistory = CalculationResult
            .Where(cr => cr.Key.Contains(boreholeName))
            .ToDictionary(pair => pair.Key.Split(':').First(), pair => pair.Value);

        if (!boreholeParamsHistory.ContainsKey($"{EnumPointKeys.WOPR}"))
            return [];

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