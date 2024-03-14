using System.Diagnostics;
using System.Net;
using System.Text;
using MessageBroker;
using tNavigatorModels;
using tNavigatorModels.Project;
using Utils;
using static MessageBroker.BrokerQueue;

namespace tNavigatorDistributionNode
{
    internal class Program
    {
        public static IMessageBroker GetBroker(NodeConfig config, BrokerQueue queue) =>
            new RabbitMQMessageBroker(config.BrokerHostname,
                config.BrokerUsername,
                config.BrokerPassword,
                queue);

        static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            var hostName = Dns.GetHostName();
            NodeConfig config = NodeConfig.LoadConfig("config.json");

            var projectDirs = new HashSet<string>();

            IMessageBroker messageBroker = GetBroker(config, ModelCalculation),
                resultBroker = GetBroker(config, ModelResult);

            Console.WriteLine($"Active node count: {messageBroker.GetConsumersCount()}");

            void ReceivedEventHandler(byte[] message)
            {
                var result = JsonUtil.Deserialize<Result>(Encoding.UTF8.GetString(message));
                if (result.Message is not null)
                {
                    Console.WriteLine(result.Message);
                    projectDirs.Clear();
                }
                else
                {
                    projectDirs.Remove(result.ProjectDirPath);
                }
            }

            resultBroker.ConsumeMessageAsync(ReceivedEventHandler);

            string projectDirPath;
            while (Directory.Exists(projectDirPath = GetNextDir()))
            {
                var sharedDir = Directory.GetParent(projectDirPath);

                var sharedDirLocalPath = sharedDir.FullName;
                Dir.MakeDirPublic(sharedDirLocalPath);

                var targetDirNetworkPath = $"//{hostName}/{sharedDir.Name}/{Path.GetFileName(projectDirPath)}";

                var message = JsonUtil.Serialize(new ConsumeModel(targetDirNetworkPath));
                await messageBroker.PublishMessage(message);

                projectDirs.Add(targetDirNetworkPath);
            }

            while (projectDirs.Any())
            {
                Thread.Sleep(300);
            }

            Console.WriteLine($"Finish: {sw.Elapsed}");
            resultBroker.PurgeQueue();
        }

        public static string? GetNextDir()
        {
            var dir = Console.ReadLine();
            if(Directory.Exists(dir) is false)
            {
                Console.WriteLine($"Directory '{dir}' is not exist.");
            }
            return dir;
        }
    }

    public class NodeConfig
    {
        public string BrokerHostname { get; set; }
        public string BrokerUsername { get; set; }
        public string BrokerPassword { get; set; }
        // public string ResultDirPath { get; set; }
        // public string ProjectDirPath { get; set; }

        public static NodeConfig LoadConfig(string configPath)
        {
            using var r = new StreamReader(configPath);
            return JsonUtil.Deserialize<NodeConfig>(r.ReadToEnd())!;
        }
    }

    public class ModelConfig
    {
        public string Url { get; set; }
    }
}