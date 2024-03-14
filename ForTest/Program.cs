using System.Text.Json;
using tNavigatorLauncher;
using tNavigatorModels.Project;
using tNavigatorModels.Project.Schedule;
using tNavigatorModels.Project.Schedule.Events;

namespace ForTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string projectFile = "tNavigatorProject.json",
                projectDir = @"C:\Users\KosachevIV\Desktop\tNavTests\modelLaunch",
                tNavExe = @"C:\Program Files\RFD\tNavigator\23.4\tNavigator-con.exe";

            InitFile(projectFile);
            var launcher = new Launcher(new LauncherConfig(tNavExe, projectDir));

            string fileText = File.ReadAllText(projectFile);
            var tNavProject = JsonSerializer.Deserialize<Project>(fileText)!;

            launcher.Start(tNavProject);

            var calculationResult = launcher.Start(tNavProject);

            var calculationResultText = JsonSerializer.Serialize(calculationResult);

            File.WriteAllText(@"C:\Users\KosachevIV\Source\Repos\tNavigatorComputerNetwork\ForTest\result.json",
                calculationResultText);
        }

        private static void InitFile(string projectPath)
        {
            var boreholes = new List<Borehole>();
            var openPerforation = new List<OpenPerforationEvent>();
            var closePerforation = new List<ClosePerforationEvent>();

            for (int i = 20; i < 21; i++)
            {
                for (int j = 20; j < 21; j++)
                {
                    var coordinates = new CoordinatePoint[]
                    {
                        new(i, j, 100, 1),
                        new(i, j, 500, 2),
                        new(i, j, 2500, 3),
                    };

                    boreholes.Add(new Borehole($"Well-{i}-{j}", coordinates, new DateTime(2023, 1, 1)));
                 
                    openPerforation.Add(new ()
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
                    AddPerforationEvents = openPerforation.ToArray(),
                    ClosePerforationEvents = closePerforation.ToArray()
                }
            };
            schedule.CurrentStep = schedule.Events.GetAllEvents().Max(e => e.Step) + 15;

            var project = new Project(boreholes.ToArray(), schedule, new Team("BestTeam"));
            var projectData = JsonSerializer.Serialize(project);
            File.WriteAllText(projectPath, projectData);
        }
    }
}