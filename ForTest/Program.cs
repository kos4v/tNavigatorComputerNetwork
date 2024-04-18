using System.Diagnostics;
using System.Text.Json;
using tNavigatorLauncher;
using tNavigatorModels;
using tNavigatorModels.Project;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Project.Schedule.Events;
using static tNavigatorModels.Project.Schedule.Events.EnumControlTypeProductionBorehole;

namespace ForTest
{
    internal class Program
    {
        public static string Url => "http://localhost:5105/test?secret=asd";

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
            const string projectDir = @"C:\Users\KosachevIV\Desktop\tNavTests\Export1";
            const string result = $@"{projectDir}\result.json";

            var navPath = NodeConfig.LoadConfig("config.json").TNavPath;
            string converterUrl = "http://195.133.145.105:8000/";

            var launcher = new Launcher(new LauncherConfig(navPath, projectDir, converterUrl), GetProject());

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
            List<ChangeBoreholeToProductionEvent> changeBoreholeToProduction = [];
            List<ChangeBoreholeToInjectionEvent> changeBoreholeToInjection = [];
            List<PropertiesRecordEvent> propertiesRecord = [];

            var borehole = GetBorehole(20, 22);
            boreholes.Add(borehole);

            AddOpenPerforation(5, borehole.Name);
            AddChangeBoreholeToProductionEvent(5, borehole.Name, WellheadPressure, 200);
            //AddClosePerforation(15, borehole.Name);

            //borehole = GetBorehole(20, 23);
            //boreholes.Add(borehole);

            //AddOpenPerforation(10, borehole.Name);
            //AddChangeBoreholeToProductionEvent(10, borehole.Name, WellheadPressure, 200);
            //AddChangeBoreholeToProductionEvent(20, borehole.Name, Debit, 20);
            //AddChangeBoreholeToInjectionEvent(30, borehole.Name, EnumControlTypeInjectionBorehole.WellheadPressure,
            //    150);
            //AddClosePerforation(40, borehole.Name);

            for (int i = 1; i < 5; i++)
            {
                AddPropertyRecord(10 * i);
            }


            var schedule = new Schedule()
            {
                Events = new EventSchedule()
                {
                    OpenPerforationEvents = [.. openPerforation],
                    ClosePerforationEvents = [.. closePerforation],
                    ChangeBoreholeToInjectionEvents = [.. changeBoreholeToInjection],
                    ChangeBoreholeToProductionEvents = [.. changeBoreholeToProduction],
                    PropertiesRecordEvents = [.. propertiesRecord],
                }
            };

            schedule.CurrentStep = schedule.Events.GetAllEvents().Max(e => e.Step) + 1;

            string converterUrl = "http://195.133.145.105:8000/";

            var project = new Project([.. boreholes], schedule, new Team("BestTeam"), Url, converterUrl);
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);

            return;


            CoordinatePoint[] GetPoints(int x, int y) =>
            [
                new(x, y, 100, 1),
                new(x, y, 500, 2),
                new(x, y, 2500, 3)
            ];

            Borehole GetBorehole(int x, int y) => new($"Well-{x}-{y}", GetPoints(x, y));

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

            void AddChangeBoreholeToProductionEvent(int step, string name,
                EnumControlTypeProductionBorehole controlType, int value) =>
                changeBoreholeToProduction.Add(new()
                {
                    Step = step,
                    BoreholeName = name,
                    ControlType = controlType,
                    DownholePressureControlValue = value,
                    DebitControlVolume = value
                });

            void AddChangeBoreholeToInjectionEvent(int step, string name, EnumControlTypeInjectionBorehole controlType,
                int value) =>
                changeBoreholeToInjection.Add(new()
                {
                    Step = step,
                    BoreholeName = name,
                    ControlType = controlType,
                    LiquidVolume = value,
                    WellheadPressure = value
                });

            void AddPropertyRecord(int step) => propertiesRecord.Add(new()
            {
                Step = step
            });
        }
    }
}