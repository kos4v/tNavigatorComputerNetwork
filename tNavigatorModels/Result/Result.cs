﻿namespace tNavigatorModels.Result;

public enum EnumDataType
{
    WaterDebit,
    OilDebit,
    GasDebit,
    TotalDebit
}

public class BoreholeData
{
    public DateTime Date { get; set; }
    public string BoreholeName { get; set; }
    public EnumDataType DataType { get; set; }
    public List<int> ZCell { get; set; } = new();
    public List<double> Value { get; set; } = new();
}

public class Result
{
    public string TeamName { get; set; }
    public List<BoreholeData> BoreholeResults { get; set; } = new();
}