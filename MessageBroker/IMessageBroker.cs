namespace MessageBroker;

public interface IMessageBroker
{
    public CancellationTokenSource ConsumeCancelTokenSource { get; set; }
    public CancellationTokenSource PublishCancelTokenSource { get; set; }

    public Task PublishMessage(byte[] message);
    public Task PublishMessage(string message);
    public Task ConsumeMessageAsync(Action<byte[]> job);
    public void PurgeQueue();
    public int GetConsumersCount();
}