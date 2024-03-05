using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Utils
{
    public static class Dir
    {
        /// <summary>
        /// string sourceDir = "//W09531/Shared/CS1.6";
        /// string targetDir = @"C:\\Users\\KosachevIV\\Desktop\\TestApp\\test";
        /// CopyDirectory(sourceDir, targetDir);
        /// </summary>
        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            // Получение всех файлов в исходной директории
            string[] files = Directory.GetFiles(sourceDir);
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            Directory.CreateDirectory(targetDir);

            // Копирование файлов
            var tasks = new List<Task>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                tasks.Add(Task.Run(() => File.Copy(file, destFile, true)));
            }

            // Получение всех поддиректорий и рекурсивное копирование их содержимого
            string[] subdirectories = Directory.GetDirectories(sourceDir);
            foreach (string subdir in subdirectories)
            {
                string subDirName = Path.GetFileName(subdir);
                string destDir = Path.Combine(targetDir, subDirName);
                CopyDirectory(subdir, destDir);
            }

            Task.WhenAll(tasks.ToArray());
        }


        public static void StartProcess(string arguments)
        {
            try
            {
                var exeFile = "powershell.exe";
                var processStartInfo = new ProcessStartInfo(exeFile, arguments);
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;

                using Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();

                // Получение вывода скрипта
                string output = process.StandardOutput.ReadToEnd();

                // Ожидание завершения выполнения процесса
                process.WaitForExit();

                // Вывод результата выполнения скрипта

                if (output.Contains("2118") || string.IsNullOrEmpty(output))
                    return;

                Console.WriteLine($"PowerShell script output: \n {output}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static void MakeDirUnPublic(string directory)
        {
            var dirName = directory.Split(@"\").Last();
            var arguments = $"Remove-SmbShare -Name {dirName} -force";
            StartProcess(arguments);
        }


        /// <summary>
        /// var directory = @"C:\Test\d";
        /// Console.WriteLine(System.Net.Dns.GetHostName());
        /// MakeDirPublic(directory);
        /// </summary>
        public static void MakeDirPublic(string directory)
        {
            var rootDir = Path.GetDirectoryName(directory);
            string script1 = "";
            script1 += $"$NewAcl = Get-Acl -Path \"{rootDir}\"";
            script1 += "\n$fileSystemAccessRuleArgumentList = 'Все', 'FullControl', 'ContainerInherit,ObjectInherit', 'None', 'Allow'";
            script1 += "\n$fileSystemAccessRule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $fileSystemAccessRuleArgumentList";
            script1 += "\n$NewAcl.SetAccessRule($fileSystemAccessRule)";
            script1 += $"\nSet-Acl -Path \"{rootDir}\" -AclObject $NewAcl";
            StartProcess(script1);
            
            var dirName = directory.Split(@"\").Last();
            var arguments = $"New-SmbShare -Name {dirName} -Path {directory} –FullAccess все";
            StartProcess(arguments);
        }
    }
}