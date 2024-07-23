using tNavigatorModels.Project.Schedule;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void ModifyData()
        {
            var modifier = new Modifier(launcherConfig.DataPath);
            modifier.DataText[modifier.FindIndex("START") + 1] = $" {Schedule.StartDate.Day} {Schedule.MonthConvert(Schedule.StartDate.Month)} {Schedule.StartDate.Year} /";
            
            modifier.ModifySubTagInTag("TNAVCTRL", "GPU_MODE", " GPU_MODE 4 /");
            modifier.ModifySubTagInTag("TNAVCTRL", "FRACTURE_BUILD_LOGIC", " FRACTURE_BUILD_LOGIC USE_VIRTUAL_CONNECTIONS /");


            modifier.Write();
        }
    }
}