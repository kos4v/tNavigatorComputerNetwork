using System.Diagnostics;
using System.Text;
using System.Text.Json;
using MessageBroker;
using tNavigatorLauncher.FileParsers;
using tNavigatorModels;
using tNavigatorModels.Project;
using tNavigatorModels.Project.Schedule;


namespace tNavigatorLauncher
{
    public class Launcher(LauncherConfig launcherConfig, Project project)
    {
        private object LockObject = new();
        public string? Output { get; set; }

        public NavigatorFileController FileController { get; set; } = new(launcherConfig, project);

        public static async Task SendTask(BrokerConfig config, Project project)
        {
            var calculationBroker = config.GetBroker(BrokerQueue.ModelReadyCalculation);
            var data = JsonSerializer.Serialize(project);
            await calculationBroker.PublishMessage(data);
        }


        public tNavigatorModels.Result.ModelResult Start()
        {
            CreateProjectFiles();
            TNavigatorRun();
            var calculationResult = ReadCalculationResult();
            return calculationResult;
        }

        public void CreateProjectFiles()
        {
            FileController.ModifyData();
            FileController.ModifySolution();
            FileController.InitSchedule();
            FileController.InitBoreholes();
            
            Utils.Dir.ReCreateWorkDir(launcherConfig.ResultDirPath);
        }

        /// <returns>Calculation result directory</returns>
        public void TNavigatorRun()
        {
            try
            {
                lock (LockObject)
                {
                    using Process process = new()
                    {
                        StartInfo = new()
                        {
                            FileName = launcherConfig.TNavigatorConsoleExePath,
                            Arguments = launcherConfig.TNavLaunchArgs,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            StandardOutputEncoding = Encoding.UTF8,
                        }
                    };

                    process.Start();
                    Output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                }

                if (!string.IsNullOrEmpty(Output))
                    Console.WriteLine(Output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message} {Output} ");
            }
        }

        public tNavigatorModels.Result.ModelResult ReadCalculationResult()
        {
            var result = new tNavigatorModels.Result.ModelResult()
            {
                TeamName = project.Team.Name,
                BoreholeResults = Schedule.DebitDirName.Keys.SelectMany(FileController.GetDebit).ToList(),
            };
            var data = FileController.SmspecUnsmryParser(launcherConfig);
            result.ReadCalculationResult(data);
            return result;
        }
    }
}