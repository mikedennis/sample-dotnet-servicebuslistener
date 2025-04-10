public interface IProcessingService
{
    Task ProcessMessageAsync(TestMessage message);
}