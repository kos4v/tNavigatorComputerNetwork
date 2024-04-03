using tNavigatorModels.Project.Schedule.Events;
using tNavigatorModels.Result;

namespace tNavigatorModels.Project.Schedule
{
    public partial class Schedule
    {
        public static int GetEventPriority(object classType)
        {
            return classType switch
            {
                PerforationEvent typeEvent => 0,
                _ => 9999
            };
        }

        public static DateOnly StartDate => new(2024, 1, 1);
        public int CurrentStep { get; set; }
        public EventSchedule Events { get; set; }
        public static string ResultRootPythonVariable => "root_result_dir";

        public static readonly Dictionary<EnumDataType, string> DebitDirName =
            new()
            {
                { EnumDataType.TotalDebit, "TotalDebitData" },
                { EnumDataType.OilDebit, "OilDebitData" },
                { EnumDataType.GasDebit, "GasDebitData" },
                { EnumDataType.WaterDebit, "WaterDebitData" },
            };

        public static string DateTNavRow(int step) => DateTNavRow(StartDate.AddDays(step));

        private static string DateTNavRow(DateOnly date, int hour = 0) =>
            $"{date.Day} {MonthConvert(date.Month)} {date.Year} {(hour == 0 ? "" : $"{hour}:{00}:{00}")}";

        public static string DateTNavString(int step, int hour = 0) =>
            $"DATES\r\n {DateTNavRow(StartDate.AddDays(step), hour)} /\r\n /\n";


        public static string ScriptsTNavString(string scriptsDir, string resultDirPath, int scriptStartStep)
        {
            var scriptPath = Directory.GetFiles(Path.Combine("Data", "PythonActions"), "*.py").First();

            var allLines = File.ReadAllLines(scriptPath).ToList();

            var fullPath = Path.GetFullPath(resultDirPath);
            allLines[allLines.FindIndex(c => c.Contains(ResultRootPythonVariable))]
                = $"{ResultRootPythonVariable} = {fullPath}";

            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptsDir);

            var fileName = Path.GetFileName(scriptPath);
            File.WriteAllLines(Path.Combine(scriptsDir, fileName), allLines);

            return $"APPLYSCRIPT\r\n './INCLUDE/PythonActions/{fileName}' 'start' 5* {DateTNavRow(scriptStartStep)} / END /\r\n /\r\n/";
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