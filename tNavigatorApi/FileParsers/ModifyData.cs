using System.Linq;
using System.Text.RegularExpressions;
using tNavigatorModels.Project.Schedule;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void ModifyData()
        {
            var dataText = File.ReadAllLines(launcherConfig.DataPath).ToList();

            int FindIndex(string match)
                => dataText.FindIndex(line => line == match);

            int FindMatchIndex(string match) =>
                dataText.FindIndex(line => line.Contains(match));


            void ModifySubTagInTag(string match, string subTag, string subTagValue)
            {
                var tagIndex = FindIndex(match);
                var tagCloseIndex = dataText[tagIndex..].FindIndex(l => l == "/") + tagIndex;
                var subTagIndex = FindMatchIndex(subTag);
                
                if (subTagIndex > 0)
                    dataText[subTagIndex] = subTagValue;
                else
                    dataText.Insert(tagCloseIndex, subTagValue);
            }

            dataText[FindIndex("START") + 1] = $" {01} {Schedule.MonthConvert(1)} {2024} /";
            ModifySubTagInTag("TNAVCTRL", "GPU_MODE", " GPU_MODE 4 /");

            File.WriteAllText(launcherConfig.DataPath, string.Join('\n', dataText));
        }
    }
}