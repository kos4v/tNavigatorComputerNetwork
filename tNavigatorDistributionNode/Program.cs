using System.Diagnostics;
using System.Net;
using System.Text;
using MessageBroker;
using tNavigatorModels;
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
            var sw = new Stopwatch();
            sw.Start();

            var hostName = Dns.GetHostName();
            NodeConfig config = NodeConfig.LoadConfig("config.json");

            var projectDirs = new HashSet<string>();

            IMessageBroker messageBroker = GetBroker(config, ModelCalculation),
                resultBroker = GetBroker(config, ModelResult);

            Console.WriteLine($"Count active node: {messageBroker.GetConsumersCount()}");

            void ReceivedEventHandler(byte[] message)
            {
                var result = JsonUtil.Deserialize<TNvagigaorResult>(Encoding.UTF8.GetString(message));
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

            var projectDirPath = Console.ReadLine();
            while (Directory.Exists(projectDirPath))
            {
                var sharedDir = Directory.GetParent(projectDirPath);

                var sharedDirLocalPath = sharedDir.FullName;
                Dir.MakeDirPublic(sharedDirLocalPath);

                var targetDirNetworkPath = $"//{hostName}/{sharedDir.Name}/{Path.GetFileName(projectDirPath)}";

                var message = JsonUtil.Serialize(new TNvagigaorModel(targetDirNetworkPath));
                await messageBroker.PublishMessage(message);

                projectDirs.Add(targetDirNetworkPath);
                projectDirPath = Console.ReadLine();
            }

            while (projectDirs.Any())
            {
                Thread.Sleep(100);
            }

            Console.WriteLine($"Finish: {sw.Elapsed}");
            resultBroker.PurgeQueue();
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