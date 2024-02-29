using System.Diagnostics;
using System.Text;
using tNavigatorModels;

namespace tNavigatorApi
{
    public record LauncherConfig(
        string TNavigatorConsoleExePath,
        string ProjectFilePath
    );

    public class Launcher(LauncherConfig launcherConfig)
    {
        public string? Output { get; set; }

        /// <returns>Project directory</returns>
        public string CreateProjectFiles(TNavigatorProject project)
        {
            throw new NotImplementedException();
        }

        public CalculationResult ReadCalculationResult()
        {
            throw new NotImplementedException();
        }

        /// <returns>Calculation result directory</returns>
        public void TNavigatorRun()
        {
            try
            {
                var exeFile = launcherConfig.TNavigatorConsoleExePath;

                using Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = exeFile,
                        Arguments =
                            $"--use-gpu \"{launcherConfig.ProjectFilePath}\" --ecl-rsm --ecl-root -eiru --ignore-lock",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    }
                };

                process.Start();

                // Получение вывода скрипта
                Output = process.StandardOutput.ReadToEnd();

                // Ожидание завершения выполнения процесса
                process.WaitForExit();

                // Вывод результата выполнения скрипта
                if (!string.IsNullOrEmpty(Output))
                {
                    Console.WriteLine($"PowerShell script output: \n{Output}");
                }

                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message} {Output} ");
            }
        }


        public CalculationResult Start(TNavigatorProject project)
        {
            var tNavigatorProjectDirectory = CreateProjectFiles(project);
            TNavigatorRun();
            var calculationResult = ReadCalculationResult();
            return calculationResult;
        }
    }
}