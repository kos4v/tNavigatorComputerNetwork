using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using tNavigatorModels;
using tNavigatorModels.Project;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Result;
using Utils;
using Result = tNavigatorModels.Project.Result;

namespace tNavigatorLauncher
{
    public record AxisSize(int Size, int Min, int Max)
    {
        public int StepSize => (Max - Min) / Size;
        public int Convert(int v) => Min + v * StepSize;
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
        public string SchedulePath => Directory.GetFiles(IncludeDir, "*Schedule.inc").First();
        public string ScriptDirPath => Path.Combine(IncludeDir, "PythonActions");
        public string ResultDirPath => Path.Combine(ProjectDir, "ResultData");


        public ModelSize GetModelSize()
        {
            var gridLines = File.ReadAllLines(GridPath).ToList();
            var gridValues = TrimSplit(gridLines[gridLines.IndexOf("SPECGRID") + 1]);

            var result = new ModelSize(new AxisSize(Convert.ToInt16(gridValues[0]), int.MaxValue, 0),
                new AxisSize(Convert.ToInt16(gridValues[1]), int.MaxValue, 0));

            var lines = File.ReadAllLines(GrdeclPath).ToList();
            foreach (var line in lines.Skip(lines.IndexOf("COORD") + 1))
            {
                var valuesDouble = TrimSplit(line.Replace('.', ',')).Select(Convert.ToDouble).ToArray();

                result = new ModelSize(result.X with
                {
                    Min = (int)new[] { valuesDouble[0], valuesDouble[3], result.X.Min }.Min(),
                    Max = (int)new[] { valuesDouble[0], valuesDouble[3], result.X.Max }.Max()
                }, result.Y with
                {
                    Min = (int)new[] { valuesDouble[1], valuesDouble[4], result.Y.Min }.Min(),
                    Max = (int)new[] { valuesDouble[1], valuesDouble[4], result.Y.Max }.Max()
                });

                if (line.Contains('/'))
                    break;
            }

            return result;

            string[] TrimSplit(string l) => l.Split(' ').Where(v => v is not ("" or "/")).ToArray();
        }
    }

    public class Launcher(LauncherConfig launcherConfig)
    {
        public string? Output { get; set; }
        public ModelSize Size { get; set; } = launcherConfig.GetModelSize();

        public void CreateProjectFiles(Project project)
        {
            InitBoreholes(project);
            InitSchedule(project);
        }

        public void InitSchedule(Project project)
        {
            var schedule_inc = new List<string>
            {
                "RPTSCHED",
                "'WELLS=2' 'SUMMARY=2' 'fip=3' 'RESTART=1' 'WELSPECS' 'CPU=2' /",
                "",
                "INCLUDE",
                $"'INCLUDE/{Path.GetFileName(launcherConfig.WellTrackPath)}' /",
                "",
            };

            schedule_inc.Add("WELSPECS");
            schedule_inc.AddRange(project.Boreholes.Select(borehole => $"   '{borehole.Name}'   1*  2*  /"));
            schedule_inc.Add("/\n");

            schedule_inc.Add(Schedule.ScriptsTNavString(launcherConfig.ScriptDirPath, launcherConfig.ResultDirPath));
            schedule_inc.Add("/\n");

            schedule_inc.Add("COMPDATMD");
            foreach (var borehole in project.Boreholes)
            {
                var perforations =
                    project.Schedule.Events.AddPerforationEvents.Where(p => p.BoreholeName == borehole.Name);
                schedule_inc.AddRange(perforations.Select(perforation => perforation.COMPDATMDString()));
            }

            schedule_inc.Add("/\n");

            // В *.data указана дата старта её нельзя указывать в DATES
            var all_envents = project.Schedule.Events.GetAllEvents();
            for (int i = 1; i < project.Schedule.CurrentStep; i++)
            {
                var todayEvents = all_envents.Where(e => e.Step == i)
                    .ToArray();
                schedule_inc.Add(Schedule.DateTNavString(i));
                if (todayEvents.Any() is false)
                {
                    continue;
                }

                foreach (var grouping in todayEvents.GroupBy(e => e.EventTNavName))
                {
                    schedule_inc.Add(grouping.First().EventTNavName);
                    schedule_inc.AddRange(
                        grouping.Select(baseEvent => baseEvent.TNavString()).Concat(new[] { "/", "" }));
                }
            }

            schedule_inc.Add(Schedule.DateTNavString(project.Schedule.CurrentStep));

            schedule_inc.Add("END");

            File.WriteAllText(launcherConfig.SchedulePath, string.Join('\n', schedule_inc));
        }


        public void InitBoreholes(Project project)
        {
            var welltrack_inc = new List<string>();

            foreach (Borehole borehole in project.Boreholes)
            {
                welltrack_inc.Add($"WELLTRACK '{borehole.Name}'");

                welltrack_inc.AddRange(borehole.Coordinates.OrderBy(c => c.OrderNumber)
                    .Select(point => $"{Size.X.Convert(point.X)} {Size.Y.Convert(point.Y)} {point.Z} 1*"));

                welltrack_inc[^1] = welltrack_inc.Last() + " /";
                welltrack_inc.Add("");
            }

            File.WriteAllText(launcherConfig.WellTrackPath, string.Join('\n', welltrack_inc));
        }

        public tNavigatorModels.Result.Result ReadCalculationResult()
        {
            var debitDir = Path.Combine(launcherConfig.ResultDirPath, Schedule.DebitDir);
            var debitFiles = Directory.GetFiles(debitDir);
            
            var boreholeDebitList = new List<ResultBorehole>();

            foreach (string debitFile in debitFiles)
            {
                var boreholeDebit = new ResultBoreholeDebit();
                var dt = Converter.ConvertCSVtoDataTable(debitFile);

                var date = dt.Rows[0][1].ToString()!.Split('-').Select(v => Convert.ToInt16(v)).ToList();
                var data = new DateOnly(date[0], date[1], date[2]);

                var wellName = dt.Rows[0][2].ToString()!.Split(':').First();

                foreach (DataRow dataRow in dt.Rows)
                {
                    var coordinate = dataRow[2].ToString()!.Split(':').Last().Split(',').Last();
                    var zCell = Convert.ToInt16(string.Join("", coordinate.Where(char.IsDigit)));
                    var value = Convert.ToDouble(((string)dataRow[3]).Replace('.', ','));
                }

                
            }

            boreholeDebitList.Add();

            var result = new tNavigatorModels.Result.Result()
            {
                BoreholeResults = boreholeDebitList.ToArray()
            };

            return result;
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
                            $"--use-gpu \"{launcherConfig.DataPath}\" --ecl-rsm --ecl-root -eiru --ignore-lock",
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


        public Result? Start(Project project)
        {
            //CreateProjectFiles(project);
            //TNavigatorRun();
            var calculationResult = ReadCalculationResult();
            return calculationResult;
        }
    }
}