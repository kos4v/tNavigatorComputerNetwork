namespace tNavigatorModels.Result;

public class PointValue
{
    public int ZCell { get; set; }
    public double Value { get; set; }
}

public class ResultBoreholeDebit
{
    public DateOnly Date { get; set; }
    public PointValue[] Values { get; set; }
}

public class ResultBorehole
{
    public string BoreholeName { get; set; }
    public ResultBoreholeDebit[] Debit { get; set; }
}

public class Result
{
    public string TeamName { get; set; }
    public ResultBorehole[] BoreholeResults { get; set; }
}