using tNavigatorModels.Project.Schedule.Events;
using tNavigatorModels.Result;

namespace tNavigatorModels.Project.Schedule
{
    public partial class Schedule
    {
        public static DateOnly StartDate => new(2024, 1, 1);
        public int CurrentStep { get; set; }
        public EventSchedule Events { get; set; }

        public static string ResultRootPythonVariable => "root_result_dir";

        public static Dictionary<EnumDataType, string> DebitDirName =
            new()
            {
                { EnumDataType.TotalDebit, "TotalDebitData" },
                { EnumDataType.OilDebit, "OilDebitData" },
                { EnumDataType.GasDebit, "GasDebitData" },
                { EnumDataType.WaterDebit, "WaterDebitData" },
            };

        public static string DateTNavString(int step)
        {
            var date = StartDate.AddDays(step);
            return $"DATES\r\n {date.Day} {MonthConvert(date.Month)} {date.Year} /\r\n /\n";
        }

        public static string ScriptsTNavString(string scriptsPath, string resultDirPath)
        {
            var scriptPath = Directory.GetFiles(Path.Combine("Data", "PythonActions"), "*.py").First();

            var allLines = File.ReadAllLines(scriptPath).ToList();

            allLines[allLines.FindIndex(c => c.Contains(ResultRootPythonVariable))]
                = $"{ResultRootPythonVariable} = '{resultDirPath}'".Replace('\\', '/');

            var fileName = Path.GetFileName(scriptPath);
            File.WriteAllLines(Path.Combine(scriptsPath, fileName), allLines);

            return $"APPLYSCRIPT\r\n './INCLUDE/PythonActions/{fileName}' 'hello' 6* /\r\n /\r\n/";
        }
    }


    public partial class Schedule
    {
        public static string MonthConvert(int month) => month switch
        {
            1 => "JAN",
            2 => "FEB",
            3 => "MAR",
            4 => "APR",
            5 => "MAY",
            6 => "JUN",
            7 => "JUL",
            8 => "AUG",
            9 => "SEP",
            10 => "OCT",
            11 => "NOV",
            12 => "DEC",
            _ => throw new ArgumentException($"month should be in range 1-12. {month}")
        };
    }
}