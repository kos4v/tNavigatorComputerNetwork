using tNavigatorApi;

namespace ForTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var projectDataFile = "\"Project\\Original_project\\BLACK_OIL_DEMO.DATA\"";

            var projectDir = "C:\\tNavProjects\\model";
            var launcher = new Launcher(new LauncherConfig(
                "C:\\Program Files\\RFD\\tNavigator\\23.1\\tNavigator-con.exe",
                $"{projectDir}\\result.data"));

            //string fileText = File.ReadAllText("tNavigatorProject.json");

            //var tNavProject = JsonSerializer.Deserialize<tNavigatorModels.TNavigatorProject>(fileText)!;

            //launcher.TNavigatorRun();

            var files = new []
            {
                Directory.GetFiles(projectDir, "*.UNSMRY").First(),
                Directory.GetFiles(projectDir, "*.SMSPEC").First()
            };

            foreach (string file in files)
            {
                Console.WriteLine(file);
            }


            //var calculationResult = launcher.Start(tNavProject);

            //var calculationResultText = JsonSerializer.Serialize(calculationResult);

            //File.WriteAllText(@"C:\Users\KosachevIV\Source\Repos\tNavigatorComputerNetwork\ForTest",
            //    calculationResultText);
        }
    }
}