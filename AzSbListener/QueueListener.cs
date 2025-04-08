using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AzSbListener;

public class QueueListener : IHostedService
{
    private readonly ILogger logger;
    private readonly QueueConnectionHealthCheck queueHealthCheck;
    private IQueueClient? queueClient;
    private readonly QueueOptions queueOptions;

    public QueueListener(ILogger<QueueListener> logger, IOptions<QueueOptions> queueOptions, QueueConnectionHealthCheck queueHealthCheck)
    {
        this.logger = logger;
        this.queueOptions = queueOptions.Value;
        this.queueHealthCheck = queueHealthCheck;
    }

    public Task StartAsync(CancellationToken CancellationToken)
    {
        logger.LogInformation("Queue listener started");
        try
        {
            this.queueClient = new QueueClient(this.queueOptions.ConnectionString, this.queueOptions.QueueName);
            this.RegisterOnMessageHandlerAndReceiveMessages();
            queueHealthCheck.IsQueueConnected = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception thrown when attaching to queue.");
            queueHealthCheck.IsQueueConnected = false;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.queueClient is null)
        {
            return Task.CompletedTask;
        }

        return this.queueClient.CloseAsync();
    }

    private void RegisterOnMessageHandlerAndReceiveMessages()
    {
        if (this.queueClient is null)
        {
            logger.LogError("No queue client");
            return;
        }

        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = this.queueOptions.MaxConcurrentCalls,
            MaxAutoRenewDuration = TimeSpan.FromMinutes(this.queueOptions.MaxAutoRenewDurationInMins),
        };

        this.queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
    }

    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        var queueMessage = JsonSerializer.Deserialize<QueueMessage>(Encoding.UTF8.GetString(message.Body));
        this.queueHealthCheck.IsQueueConnected = true;
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        if (exceptionReceivedEventArgs.Exception is MessagingEntityNotFoundException)
        {
            this.queueHealthCheck.IsQueueConnected = false;
        }

        return Task.CompletedTask;
    }
}