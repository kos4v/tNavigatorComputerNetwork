using MessageBroker;
using System.Text.Json;

namespace tNavigatorModels;

public class BrokerConfig
{
    public string BrokerHostname { get; set; }
    public string BrokerUsername { get; set; }
    public string BrokerPassword { get; set; }

    public IMessageBroker GetBroker(BrokerQueue queue, string? queueName = null)
        => GetBroker(this, queue, queueName);

    public static IMessageBroker GetBroker(BrokerConfig config, BrokerQueue queue, string? queueName = null) =>
        new RabbitMQMessageBroker(config.BrokerHostname,
            config.BrokerUsername,
            config.BrokerPassword,
            queue,
            queueName);
}

public class NodeConfig : BrokerConfig
{
    public string ProjectDirPath { get; set; }
    public string? TNavPath { get; set; }
    public string PriorityVersion { get; set; }

    public static NodeConfig LoadConfig(string configPath)
    {
        using var r = new StreamReader(configPath);
        var result = JsonSerializer.Deserialize<NodeConfig>(r.ReadToEnd())!;
        result.TNavPath ??= GetTNavigatorPath(result.PriorityVersion);
        r.Close();
        File.WriteAllText(configPath, JsonSerializer.Serialize(result));
        return result;
    }

    private static string? GetTNavigatorPath(string priorityVersion)
    {
        string fileName = "tNavigator-con.exe";

        string[] dirs =
        [
            "C:\\Program Files\\RFD\\tNavigator",
            "C:\\Program Files (x86)\\RFD\\tNavigator",
            $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Programs\\RFD",
            "C:\\Users\\KosachevIV\\AppData\\Local\\Programs\\RFD",
            "C:\\Users\\"
        ];

        string? path = null;
        List<string> result = [];
        foreach (string dir in dirs)
        {
            result.AddRange(SearchForFile(dir, fileName).Result);
            path = result.FirstOrDefault(p => p.Contains(priorityVersion));
            if (path != null)
            {
                break;
            }
        }

        return (path ?? result.FirstOrDefault())
               ?? SearchForFile("C:\\", fileName).Result.FirstOrDefault();
    }

    private static async Task<string[]> SearchForFile(string directory, string fileName)
    {
        try
        {
            List<Task<string[]>> tasks = Directory.GetDirectories(directory)
                .Select(dir => SearchForFile(dir, fileName)).ToList();

            List<string> result = [.. Directory.GetFiles(directory, fileName)];

            foreach (var task in tasks)
            {
                var subResult = await task;
                result.AddRange(subResult);
            }

            return [.. result];
        }
        catch (Exception)
        {
            // ignored
        }

        return [];
    }
}