using System.Diagnostics;
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
            List<Borehole> boreholes = [];
            List<OpenPerforationEvent> openPerforation = [];
            List<ClosePerforationEvent> closePerforation = [];
            List<ChangeBoreholeControlEvent> changeBoreholeControl = [];


            CoordinatePoint[] GetPoints(int x, int y) =>
            [
                new(x, y, 100, 1),
                new(x, y, 500, 2),
                new(x, y, 2500, 3)
            ];

            Borehole GetBorehole(int x, int y)
                => new($"Well-{x}-{y}", GetPoints(x, y), new DateTime(2024, 1, 1));

            void AddOpenPerforation(int step, string name) => openPerforation.Add(new()
            {
                Step = step,
                StartMD = 2400,
                EndMD = 2450,
                BoreholeName = name
            });

            void AddClosePerforation(int step, string name) => closePerforation.Add(new()
            {
                Step = step,
                StartMD = 2400,
                EndMD = 2450,
                BoreholeName = name
            });

            void AddChangeBoreholeControl(int step, string name) => changeBoreholeControl.Add(new()
            {
                Step = step,
                BoreholeName = name,
                ControlType = EnumControlType.Pressure,
                DebitControlVolume = 200
            });

            var borehole = GetBorehole(20, 22);
            boreholes.Add(borehole);

            AddOpenPerforation(1, borehole.Name);
            AddChangeBoreholeControl(1, borehole.Name);
            AddClosePerforation(15, borehole.Name);

            borehole = GetBorehole(20, 23);
            boreholes.Add(borehole);

            AddOpenPerforation(10, borehole.Name);
            AddChangeBoreholeControl(10, borehole.Name);
            AddClosePerforation(20, borehole.Name);



            var schedule = new Schedule()
            {
                Events = new EventSchedule()
                {
                    OpenPerforationEvents = [.. openPerforation],
                    ClosePerforationEvents = [.. closePerforation],
                    ChangeBoreholeControlEvent = [..changeBoreholeControl]
                }
            };
            schedule.CurrentStep = schedule.Events.GetAllEvents().Max(e => e.Step) + 10;
            //schedule.CurrentStep = 30;

            var project = new Project([..boreholes], schedule, new Team("BestTeam"), Url);
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);
        }
    }
}