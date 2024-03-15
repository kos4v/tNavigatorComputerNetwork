using tNavigatorModels.Project;

namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public void InitBoreholes()
        {
            var welltrackInc = new List<string>();

            foreach (Borehole borehole in project.Boreholes)
            {
                welltrackInc.Add($"WELLTRACK '{borehole.Name}'");

                welltrackInc.AddRange(borehole.Coordinates.OrderBy(c => c.OrderNumber)
                    .Select(point => $"{Size.X.Convert(point.X)} {Size.Y.Convert(point.Y)} {point.Z} 1*"));

                welltrackInc[^1] = welltrackInc.Last() + " /";
                welltrackInc.Add("");
            }

            File.WriteAllText(launcherConfig.WellTrackPath, string.Join('\n', welltrackInc));
        }
    }
}