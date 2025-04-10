using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

public class RabbitMqClient : IAmqpClient
{
    private readonly IServiceProvider _serviceProvider;

    private IConnection _connection;
    private IModel _channel;
    private EventingBasicConsumer _consumer;

    private Func<string, Task> _messageHandler;
    private Func<string, Task> _errorHandler;

    public RabbitMqClient(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ConnectAsync(string queueName, Func<string, Task> messageHandler, Func<string, Task> errorHandler = null)
    {
        _messageHandler = messageHandler;
        _errorHandler = errorHandler ?? (message => Task.CompletedTask);

        _connection = _serviceProvider.GetRequiredService<IConnection>();

        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += ProcessMessageAsync;

        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: _consumer);

        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        _consumer.Received -= ProcessMessageAsync;
        _channel?.Dispose();

        return Task.CompletedTask;
    }

    private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
    {        
        string message = Encoding.UTF8.GetString(args.Body.ToArray());
        //_messageHandler(message).GetAwaiter().GetResult();
        _messageHandler(message);
    }
}