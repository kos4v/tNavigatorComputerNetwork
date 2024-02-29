using System.Net;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBroker;

public enum BrokerQueue
{
    ModelCalculation,
    ModelResult,
}

public class RabbitMQMessageBroker(
    string hostname,
    string? user,
    string? password,
    BrokerQueue queue) : IMessageBroker
{
    private readonly string _queueName = queue switch
    {
        BrokerQueue.ModelCalculation => "model-calculation",
        BrokerQueue.ModelResult => "model-result",
        _ => throw new ArgumentOutOfRangeException(nameof(queue), queue, null)
    };

    private CancellationToken ConsumeCancellationToken { get; set; }
    private CancellationToken PublishCancellationToken { get; set; }

    private Queue<byte[]> MessageStack { get; set; } = new();

    private Task? PublisherTask { get; set; }

    private ConnectionFactory MakeFactory() => new()
    {
        HostName = hostname,
        CredentialsProvider = !string.IsNullOrEmpty(user) & !string.IsNullOrEmpty(password)
            ? new BasicCredentialsProvider(user, password)
            : null
    };


    public CancellationTokenSource? PublishCancelTokenSource { get; set; }
    public CancellationTokenSource ConsumeCancelTokenSource { get; set; } = new();


    public async Task PublishMessage(string message) =>
        PublishMessage(Encoding.ASCII.GetBytes(message));

    public void PurgeQueue()
    {
        using var connection = MakeFactory().CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueuePurge(_queueName);
    }

    public int GetConsumersCount()
    {
        using var connection = MakeFactory().CreateConnection();
        using var channel = connection.CreateModel();
        return (int)channel.ConsumerCount(_queueName);
    }

    private async Task PublisherAsync(int reloadPublisherInSecond = 20)
    {
        try
        {
            PublishCancelTokenSource = new();
            PublishCancellationToken = PublishCancelTokenSource.Token;

            _ = Task.Run(() =>
            {
                Thread.Sleep(reloadPublisherInSecond * 1000);
                PublishCancelTokenSource.Cancel();
            });

            using var connection = MakeFactory().CreateConnection();
            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            while (!PublishCancellationToken.IsCancellationRequested)
            {
                if (MessageStack.TryDequeue(out byte[]? stackMessage))
                {
                    channel.BasicPublish(exchange: "",
                        routingKey: _queueName,
                        basicProperties: null,
                        body: stackMessage);
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task PublishMessage(byte[] message)
    {
        MessageStack.Enqueue(message);
        PublisherTask = PublisherTask?.IsCompleted ?? true
            ? PublisherAsync()
            : PublisherTask;
    }

    public async Task ConsumeMessageAsync(Action<byte[]> job)
    {
        ConsumeCancellationToken = ConsumeCancelTokenSource.Token;
        using var connection = MakeFactory().CreateConnection();

        using var channel = connection.CreateModel();

        channel.BasicQos(0, 1, true);
        channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (_, e) =>
        {
            job(e.Body.ToArray());
            channel.BasicAck(e.DeliveryTag, false);
        };

        channel.BasicConsume(queue: _queueName,
            autoAck: false,
            consumer: consumer,
            consumerTag: Dns.GetHostName());

        while (!ConsumeCancellationToken.IsCancellationRequested) await Task.Delay(1000);
    }
}