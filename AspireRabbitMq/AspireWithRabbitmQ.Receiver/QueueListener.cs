using System.Text.Json;

public class QueueListener : BackgroundService
{
    private readonly ILogger<QueueListener> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope _scope;
    private IAmqpClient _amqpClient;


    public QueueListener(ILogger<QueueListener> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _scope = _serviceProvider.CreateScope();
        var scopedProvider = _scope.ServiceProvider;
        _amqpClient = scopedProvider.GetRequiredService<IAmqpClient>();

        string queueName = "testMessage";
        _amqpClient.ConnectAsync(queueName, ProcessMessageAsync, null);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _amqpClient.DisconnectAsync();
        _scope?.Dispose();
    }

    private async Task ProcessMessageAsync(string message)
    {
        _logger.LogInformation("Message retrieved from queue at {now}. Message Text: {text}", DateTime.Now, message);
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedProvider = scope.ServiceProvider;
            var service = scopedProvider.GetRequiredService<IProcessingService>();
            try
            {
                var testMessage = JsonSerializer.Deserialize<TestMessage>(message);
                if (testMessage == null)
                {
                    _logger.LogError("Failed to deserialize message: {message}", message);
                    return;
                }
                await service.ProcessMessageAsync(testMessage);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message: {message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the message: {message}", message);
            }
        }
    }
}
