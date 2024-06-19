namespace tNavigatorLauncher;

public record AxisSize(int Size, int Min, int Max)
{
    public int StepSize => (Max - Min) / Size;
    public int Convert(int v) => Min + v * StepSize;
}

public record ModelSize(AxisSize X, AxisSize Y);

public record LauncherConfig(string TNavigatorConsoleExePath, string ProjectDir, string converterUrl)
{
    public Uri ConverterSmspecUnsmryUrl =>
        new(new Uri(converterUrl.Contains("http") ? converterUrl : $"http://{converterUrl}"), "ResultParse");

    public string SmspecPath => Directory.GetFiles(ProjectDir, "*.SMSPEC").First();
    public string UnsmryPath => Directory.GetFiles(ProjectDir, "*.UNSMRY").First();
    public string? UnrstPath => Directory.GetFiles(ProjectDir, "*.UNRST").FirstOrDefault();
    public string DataPath => Directory.GetFiles(ProjectDir, "*.data").First();
    public string IncludeDir => Path.Combine(ProjectDir, "INCLUDE");
    public string WellTrackPath => Directory.GetFiles(IncludeDir, "*WELLTRACK.inc").First();
    public string GrdeclPath => Directory.GetFiles(IncludeDir, "*.grdecl").First();
    public string GridPath => Directory.GetFiles(IncludeDir, "*GRID.inc").First();
    public string SchedulePath => Directory.GetFiles(IncludeDir, "*Schedule.inc").First();
    public string ScriptDirPath => Path.Combine(IncludeDir, "PythonActions");
    public string ResultDirPath => Path.Combine(ProjectDir, "ResultData");
    public string TNavLaunchArgs => $"--use-gpu \"{DataPath}\" --ecl-rsm --ecl-root -eiru --ignore-lock";
    public string SolutionPath => Directory.GetFiles(IncludeDir, "*SOLUTION.inc").First();
    public string CoordinatesPath => Path.Combine(ProjectDir, "Coordinates.json");


    public ModelSize GetModelSize()
    {
        var gridLines = File.ReadAllLines(GridPath).ToList();
        var gridValues = TrimSplit(gridLines[gridLines.IndexOf("SPECGRID") + 1]);

        var result = new ModelSize(new AxisSize(Convert.ToInt16(gridValues[0]), int.MaxValue, 0),
            new AxisSize(Convert.ToInt16(gridValues[1]), int.MaxValue, 0));

        var lines = File.ReadAllLines(GrdeclPath).ToList();
        var values = new List<double>();
        foreach (var line in lines.Skip(lines.IndexOf("COORD") + 1))
        {
            var valuesDouble = TrimSplit(line.Replace('.', ','))
                .SelectMany(v => v.Contains('*')
                    ? Enumerable.Range(0, Convert.ToInt32(v.Split('*').First())).Select(_ => v.Split('*').Last())
                    : [v])
                .Select(Convert.ToDouble)
                .ToArray();

            values.AddRange(valuesDouble);


            if (line.Contains('/'))
                break;
        }

        for (int i = 0; i < values.Count; i += 6)
        {
            result = new ModelSize(result.X with
            {
                Min = (int)new[] { values[i + 0], values[i + 3], result.X.Min }.Min(),
                Max = (int)new[] { values[i + 0], values[i + 3], result.X.Max }.Max()
            }, result.Y with
            {
                Min = (int)new[] { values[i + 1], values[i + 4], result.Y.Min }.Min(),
                Max = (int)new[] { values[i + 1], values[i + 4], result.Y.Max }.Max()
            });
        }


        return result;

        string[] TrimSplit(string l) => l.Split(' ').Where(v => v is not ("" or "/")).ToArray();
    }
}