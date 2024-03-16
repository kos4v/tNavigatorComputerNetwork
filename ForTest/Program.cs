using System.Net;
using System.Text.Json;
using tNavigatorLauncher;
using tNavigatorModels;
using tNavigatorModels.Project;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Project.Schedule.Events;

namespace ForTest
{
    internal class Program
    {
        public static string Url => "http://localhost:5105/test?secret=asd";

        static void Main(string[] args)
        {
            NodeLaunch();
        }

        private static void NodeLaunch()
        {
            var projectFile = "tNavigatorProject.json";
            InitFile(projectFile);

            var fileText = File.ReadAllText(projectFile);
            var tNavProject = JsonSerializer.Deserialize<Project>(fileText)!;

            var brokerConfig = new BrokerConfig()
            {
                BrokerHostname = "195.133.145.105",
                BrokerUsername = "guest",
                BrokerPassword = "J4ntpgFtKTzG84LD"
            };

            Launcher.SendTask(brokerConfig, tNavProject);
        }

        private static void LocalLaunch(string projectPath)
        {
            const string projectFile = "tNavigatorProject.json";
            const string projectDir = @"C:\Users\KosachevIV\Desktop\tNavTests\modelLaunch";
            const string tNavExe = @"C:\Program Files\RFD\tNavigator\23.4\tNavigator-con.exe";
            const string result = @"C:\Users\KosachevIV\Source\Repos\tNavigatorComputerNetwork\ForTest\result.json";

            InitFile(projectFile);

            string fileText = File.ReadAllText(projectFile);

            var tNavProject = JsonSerializer.Deserialize<Project>(fileText)!;
            var launcher = new Launcher(new LauncherConfig(tNavExe, projectDir), tNavProject);

            var launcherResult = launcher.Start();
            var calculationResultText = JsonSerializer.Serialize(launcherResult);

            File.WriteAllText(result,calculationResultText);
        }

        private static void InitFile(string projectPath)
        {
            var boreholes = new List<Borehole>();
            var openPerforation = new List<OpenPerforationEvent>();
            var closePerforation = new List<ClosePerforationEvent>();

            for (int i = 20; i < 21; i++)
            {
                for (int j = 20; j < 22; j++)
                {
                    var coordinates = new CoordinatePoint[]
                    {
                        new(i, j, 100, 1),
                        new(i, j, 500, 2),
                        new(i, j, 2500, 3),
                    };

                    boreholes.Add(new Borehole($"Well-{i}-{j}", coordinates, new DateTime(2023, 1, 1)));

                    openPerforation.Add(new()
                    {
                        Step = 5,
                        StartMD = 2400,
                        EndMD = 2450,
                        BoreholeName = boreholes.Last().Name
                    });

                    //closePerforation.Add(new()
                    //{
                    //    Step = i * j + 3,
                    //    StartMD = 2400,
                    //    EndMD = 2410,
                    //    BoreholeName = boreholes.Last().Name
                    //});

                    //openPerforation.Add(new()
                    //{
                    //    Step = i * j + 3,
                    //    StartMD = 2412,
                    //    EndMD = 2415,
                    //    BoreholeName = boreholes.Last().Name
                    //});

                    //closePerforation.Add(new()
                    //{
                    //    Step = i * j + 3 + 3,
                    //    StartMD = 2412,
                    //    EndMD = 2415,
                    //    BoreholeName = boreholes.Last().Name
                    //});
                }
            }

            var schedule = new Schedule()
            {
                Events = new EventSchedule()
                {
                    AddPerforationEvents = [.. openPerforation],
                    ClosePerforationEvents = [.. closePerforation]
                }
            };
            schedule.CurrentStep = schedule.Events.GetAllEvents().Max(e => e.Step) + 15;

            var project = new Project([..boreholes], schedule, new Team("BestTeam"), Url);
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);
        }
    }
}