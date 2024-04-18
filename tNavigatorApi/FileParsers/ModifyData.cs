using tNavigatorModels.Project.Schedule;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void ModifyData()
        {
            var modifier = new Modifier(launcherConfig.DataPath);

            modifier.DataText[modifier.FindIndex("START") + 1] = $" {01} {Schedule.MonthConvert(1)} {2024} /";
            
            modifier.ModifySubTagInTag("TNAVCTRL", "GPU_MODE", " GPU_MODE 4 /");
            
            
            modifier.Write();
        }
    }
}