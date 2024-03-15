using tNavigatorModels.Project;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController(LauncherConfig launcherConfig, Project project)
    {
        public ModelSize Size { get; set; } = launcherConfig.GetModelSize();
    }
}