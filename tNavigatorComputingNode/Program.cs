using MessageBroker;
using System.Text;
using System.Text.Json.Serialization;
using tNavigatorLauncher;
using tNavigatorModels;
using Utils;

namespace tNavigatorComputingNode;

internal class Program
{
    public static IMessageBroker GetBroker(NodeConfig config, BrokerQueue queue) =>
        new RabbitMQMessageBroker(config.BrokerHostname,
            config.BrokerUsername,
            config.BrokerPassword,
            queue);

    public static void ReCreateWorkDir(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);

        Directory.CreateDirectory(path);
    }


    static void Main(string[] args)
    {
        Console.Title = nameof(tNavigatorComputingNode);
        var config = NodeConfig.LoadJson("config.json");
        IMessageBroker brokerForConsumeTask = GetBroker(config, BrokerQueue.ModelCalculation),
            brokerForResult = GetBroker(config, BrokerQueue.ModelResult);

        ReCreateWorkDir(config.ProjectPath);

        Console.WriteLine("Start");

        void ReceivedEventHandler(byte[] message)
        {
            string? projectSourceUrl = "", errorMessage = null;
            try
            {
                Console.WriteLine("Calculate");
                projectSourceUrl = JsonUtil.Deserialize<TNavigatorModel>(Encoding.UTF8.GetString(message))!.Url;

                var calculateProjectDir = Path.Combine(config.ProjectPath, Path.GetFileName(projectSourceUrl));
                Dir.CopyDirectory(projectSourceUrl, calculateProjectDir);

                var projectFile = Directory.GetFiles(calculateProjectDir, "*.data").FirstOrDefault();
                if (projectFile is null)
                {
                    var dirFiles = string.Join(", ", Directory.GetFiles(projectSourceUrl));
                    var dirs= string.Join(", ", Directory.GetDirectories(projectSourceUrl));
                    throw new ArgumentException(
                        $"A file with the extension *.data was not found in the {projectSourceUrl} directory. {dirFiles}. {dirs}");
                }

                var launcher = new Launcher(new LauncherConfig(config.TNavPath, projectFile));
                launcher.TNavigatorRun();

                foreach (var template in new[] { "*.UNSMRY", "*.SMSPEC" })
                {
                    var file = Directory.GetFiles(calculateProjectDir, template).First();
                    File.Move(file, Path.Combine(projectSourceUrl, Path.GetFileName(file)), true);
                }

                Directory.Delete(calculateProjectDir, true);
                Console.WriteLine("Calculate complete");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + $"{e}");
                errorMessage = e.Message + $"{e}";
            }

            var resultMessage = JsonUtil.Serialize(new TNavigatorResult(projectSourceUrl, errorMessage));
            brokerForResult.PublishMessage(resultMessage).Wait();
        }

        brokerForConsumeTask.ConsumeMessageAsync(ReceivedEventHandler);
        Console.ReadKey();
    }

    public class NodeConfig
    {
   
        public string BrokerHostname { get; set; }
        public string BrokerUsername { get; set; }
        public string BrokerPassword { get; set; }
        public string TNavPath { get; set; }

        public string ProjectPath { get; set; }

        public static NodeConfig LoadJson(string configPath)
        {
            using var r = new StreamReader(configPath);
            return JsonUtil.Deserialize<NodeConfig>(r.ReadToEnd())!;
        }
    }

// public static (string BrokerHostname, string BrokerUsername, string BrokerPassword) ParseArgs(string[] args)
// {
//     var helpMessage =
//         "Для начала работы укажите: brokerHostname=url brokerUsername=name brokerPassword=password";
//
//     var argDict = args.Select(arg => arg.Split('='))
//         .ToDictionary(arg => arg.First(),
//             arg => arg.Last()) ?? new();
//
//     if (args.Any())
//     {
//         if (argDict.TryGetValue("brokerHostname", out var hostname)
//             & argDict.TryGetValue("brokerUsername", out var username)
//             & argDict.TryGetValue("brokerPassword", out var password))
//         {
//             return (hostname!, username!, password!);
//         }
//
//         throw new ArgumentException(helpMessage +
//                                     $"Указаны: ({string.Join(", ", argDict.Select(pair => $"{pair.Key}:{pair.Value}"))})");
//     }
//
//     var environmentVariables = Environment.GetEnvironmentVariables();
//
//     var envHostname = environmentVariables["brokerHostname"]?.ToString();
//     var envUsername = environmentVariables["brokerUsername"]?.ToString();
//     var envPassword = environmentVariables["brokerPassword"]?.ToString();
//
//     if (envHostname != null & envUsername != null & envPassword != null)
//         return (envHostname!, envPassword!, envPassword!);
//
//     throw new ArgumentException(helpMessage);
// }
}