using System.Security.AccessControl;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;
using tNavigatorLauncher;
using tNavigatorModels;
using tNavigatorModels.Schedule;

namespace ForTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string projectFile = "tNavigatorProject.json",
                projectDir = "C:\\Users\\KosachevIV\\Desktop\\tNavTests\\modelLaunch",
                tNavExe = "C:\\Program Files\\RFD\\tNavigator\\23.4\\tNavigator-con.exe";

            InitFile(projectFile);
            var launcher = new Launcher(new LauncherConfig(tNavExe, projectDir));

            string fileText = File.ReadAllText(projectFile);
            var tNavProject = JsonSerializer.Deserialize<TNavigatorProject>(fileText)!;

            launcher.Start(tNavProject);

            var calculationResult = launcher.Start(tNavProject);

            var calculationResultText = JsonSerializer.Serialize(calculationResult);

            File.WriteAllText(@"C:\Users\KosachevIV\Source\Repos\tNavigatorComputerNetwork\ForTest",
                calculationResultText);
        }

        private static void InitFile(string projectPath)
        {
            var boreholes = new List<Borehole>();
            var coordinates = new CoordinatePoint[]
            {
                new(1, 2, 100, 1),
                new(1, 2, 500, 2),
            };

            boreholes.Add(new Borehole("Well #1",
                coordinates, new DateTime(2023, 1, 1)));


            var project = new TNavigatorProject(boreholes.ToArray(), new Schedule(), new Team("BestTeam"));
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);
        }
    }
}