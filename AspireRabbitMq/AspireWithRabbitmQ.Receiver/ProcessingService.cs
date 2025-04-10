public class ProcessingService : IProcessingService
{
    private readonly ILogger<ProcessingService> _logger;

    public ProcessingService(ILogger<ProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task ProcessMessageAsync(TestMessage message)
    {
        await Task.Delay(5000); // Simulate some processing delay
        _logger.LogInformation("Processed message: {message}", message.Text);
    }
}
