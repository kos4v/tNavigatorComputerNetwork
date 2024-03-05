using System.Diagnostics;
using System.Drawing;
using System.Text;
using tNavigatorModels;

namespace tNavigatorLauncher
{
    public record AxisSize(int Size, int Min, int Max)
    {
        public double StepSize => (double)(Max - Min) / Size;
    }

    public record ModelSize(AxisSize X, AxisSize Y);
        
    public record LauncherConfig(
        string TNavigatorConsoleExePath,
        string ProjectDir
    )
    {
        public string DataPath => Directory.GetFiles(ProjectDir, "*.data").First();
        public string IncludeDir => Path.Combine(ProjectDir, "INCLUDE");
        public string WellTrackPath => Directory.GetFiles(IncludeDir, "*WELLTRACK.inc").First();
        public string GrdeclPath => Directory.GetFiles(IncludeDir, "*.grdecl").First();
        public string GridPath => Directory.GetFiles(IncludeDir, "*GRID.inc").First();

        public ModelSize GetModelSize()
        {
            var lines = File.ReadAllLines(GrdeclPath);
            var isCoord = false;
            double maxX = 0,
                maxY = 0,
                minX = int.MaxValue,
                minY = int.MaxValue;

            foreach (var line in lines)
            {
                if (line == "COORD")
                {
                    isCoord = true;
                    continue;
                }

                if (!isCoord)
                {
                    continue;
                }

                var values = line.Split(" ")
                    .Where(v => v != "");

                if (values.Last() == "/")
                {
                    isCoord = false;
                    values = values.SkipLast(1);
                }

                var valuesDouble = values
                    .Select(c =>
                        Convert.ToDouble(c.Replace('.', ',')))
                    .ToArray();

                maxX = new[] { valuesDouble[0], valuesDouble[3], maxX }.Max();
                maxY = new[] { valuesDouble[1], valuesDouble[4], maxY }.Max();
                minX = new[] { valuesDouble[0], valuesDouble[3], minX }.Min();
                minY = new[] { valuesDouble[1], valuesDouble[4], minY }.Min();

                if (isCoord is false)
                {
                    break;
                }
            }

            var gridLines = File.ReadAllLines(GridPath).ToList();
            var targetLine = "";
            for (int i = 0; i < gridLines.Count; i++)
            {
                if (gridLines[i] == "SPECGRID")
                {
                    targetLine = gridLines[i + 1];
                }
            }

            var gridValues = targetLine.Split(' ').Where(v => v != "").ToArray();

            new AxisSize(Convert.ToInt16(gridValues[0]), (int)minX, (int)maxX);
            return new ModelSize(
                new AxisSize(Convert.ToInt16(gridValues[0]), (int)minX, (int)maxX),
                new AxisSize(Convert.ToInt16(gridValues[1]), (int)minY, (int)maxY)
                );
        }
    }

    public class Launcher(LauncherConfig launcherConfig)
    {
        public string? Output { get; set; }
        public ModelSize Size { get; set; } = launcherConfig.GetModelSize();

        /// <returns>Project directory</returns>
        public void CreateProjectFiles(TNavigatorProject project)
        {
            InitBoreholes(project);
            //InitEvents(project);
        }

        public void InitEvents(TNavigatorProject project)
        {
            foreach (var scheduleEvent in project.Schedule.Events)
            {
                Console.WriteLine(scheduleEvent.EventName);
            }
        }

        public void InitBoreholes(TNavigatorProject project)
        {
            var welltrack_inc = new List<string>();

            foreach (Borehole borehole in project.Boreholes)
            {
                welltrack_inc.Add($"WELLTRACK '{borehole.Name}'");
                welltrack_inc.AddRange(borehole.Coordinates.Select(point => $"{point.X} {point.Y} {point.Z} 1*"));
                welltrack_inc[^1] = welltrack_inc.Last() + " /";
            }

            File.WriteAllText(launcherConfig.WellTrackPath, string.Join('\n', welltrack_inc));
        }


        public CalculationResult ReadCalculationResult()
        {
            throw new NotImplementedException();
        }

        /// <returns>Calculation result directory</returns>
        public void TNavigatorRun()
        {
            try
            {
                var exeFile = launcherConfig.TNavigatorConsoleExePath;

                using Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = exeFile,
                        Arguments =
                            $"--use-gpu \"{launcherConfig.ProjectDir}\" --ecl-rsm --ecl-root -eiru --ignore-lock",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    }
                };

                process.Start();

                // Получение вывода скрипта
                Output = process.StandardOutput.ReadToEnd();

                // Ожидание завершения выполнения процесса
                process.WaitForExit();

                // Вывод результата выполнения скрипта
                if (!string.IsNullOrEmpty(Output))
                {
                    Console.WriteLine(Output);
                }

                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message} {Output} ");
            }
        }


        public CalculationResult? Start(TNavigatorProject project)
        {
            CreateProjectFiles(project);
            return null;

            TNavigatorRun();
            var calculationResult = ReadCalculationResult();
            return calculationResult;
        }
    }
}