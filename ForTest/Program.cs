using System.Diagnostics;
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
        public static string TNavExe => @"C:\Program Files\RFD\tNavigator\23.4\tNavigator-con.exe";

        static void Main(string[] args)
        {
            // Использовать RESTART для продолжения с указанной даты


            var sw = Stopwatch.StartNew();
            LocalLaunch();
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();

        }

        private static void NodeLaunch()
        {
            var brokerConfig = new BrokerConfig()
            {
                BrokerHostname = "195.133.145.105",
                BrokerUsername = "guest",
                BrokerPassword = "J4ntpgFtKTzG84LD"
            };

            Launcher.SendTask(brokerConfig, GetProject());
        }

        private static void LocalLaunch()
        {
            const string projectDir = @"C:\Users\KosachevIV\Desktop\tNavTests\modelLaunch";
            const string result = @"C:\Users\KosachevIV\Source\Repos\tNavigatorComputerNetwork\ForTest\result.json";

            var launcher = new Launcher(new LauncherConfig(TNavExe, projectDir), GetProject());

            var launcherResult = launcher.Start();
            var calculationResultText = JsonSerializer.Serialize(launcherResult);

            File.WriteAllText(result, calculationResultText);
        }

        public static Project GetProject()
        {
            const string projectFile = "tNavigatorProject.json";

            InitFile(projectFile);
            string fileText = File.ReadAllText(projectFile);

            var tNavProject = JsonSerializer.Deserialize<Project>(fileText)!;
            return tNavProject;
        }

        private static void InitFile(string projectPath)
        {
            var boreholes = new List<Borehole>();
            var openPerforation = new List<OpenPerforationEvent>();
            var closePerforation = new List<ClosePerforationEvent>();


            CoordinatePoint[] GetPoints(int x, int y) =>
            [
                new(x, y, 100, 1),
                new(x, y, 500, 2),
                new(x, y, 2500, 3)
            ];

            Borehole GetBorehole(int x, int y)
                => new($"Well-{x}-{y}", GetPoints(x, y), new DateTime(2024, 1, 1));

            void AddPerforation(int step, string name, int pressure) => openPerforation.Add(new()
            {
                Step = step,
                StartMD = 2400,
                EndMD = 2450,
                BoreholeName = name,
                BoreholeDownholePressure = pressure
            });

            var borehole = GetBorehole(20, 22);
            boreholes.Add(borehole);

            AddPerforation(1, borehole.Name, 200);
            AddPerforation(50, borehole.Name, 300);


            var schedule = new Schedule()
            {
                Events = new EventSchedule()
                {
                    AddPerforationEvents = [.. openPerforation],
                    ClosePerforationEvents = [.. closePerforation]
                }
            };
            schedule.CurrentStep = schedule.Events.GetAllEvents().Max(e => e.Step) + 50;
            //schedule.CurrentStep = 30;

            var project = new Project([..boreholes], schedule, new Team("BestTeam"), Url);
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);
        }
    }
}