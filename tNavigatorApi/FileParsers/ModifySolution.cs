using static tNavigatorModels.Result.EnumCubeProperty;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void ModifySolution()
        {
            var modifier = new Modifier(launcherConfig.SolutionPath);

            if (modifier.DataText.Contains("RPTRST"))
                return;

            // RFIP - обобщает RFIPGAS, RFIPOIL, RFIPWAT: 
            string[] cubeNames = ["RFIP", $"{SGAS:G}", $"{SOIL:G}", $"{SWAT:G}"];

            modifier.DataText.Add($"RPTRST\r\n {string.Join(" ", cubeNames)} /");
            modifier.Write();
        }
    }
}