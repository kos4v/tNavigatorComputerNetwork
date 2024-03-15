using System.Linq;
using tNavigatorModels.Project.Schedule;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void ModifyData()
        {
            var dataText = File.ReadAllLines(launcherConfig.DataPath).ToList();

            dataText[dataText.FindIndex(line => line.Contains("START")) + 1] = 
                $"\t{01} {Schedule.MonthConvert(1)} {2024} /";


            File.WriteAllText(launcherConfig.DataPath, string.Join('\n', dataText));
        }
    }
}