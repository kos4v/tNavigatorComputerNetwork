using MessageBroker;

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
    public string ResultDirPath { get; set; }
    public string ProjectDirPath { get; set; }
    public string TNavPath { get; set; }
    public string ResultAddress { get; set; }

    public static NodeConfig LoadConfig(string configPath)
    {
        using var r = new StreamReader(configPath);
        return JsonUtil.Deserialize<NodeConfig>(r.ReadToEnd())!;
    }
}