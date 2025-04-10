public interface IAmqpClient
{
    Task ConnectAsync(string queueName, Func<string, Task> messageHandler, Func<string, Task> errorHandler = null);
    Task DisconnectAsync();
}