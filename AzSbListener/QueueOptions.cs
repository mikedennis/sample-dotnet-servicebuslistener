namespace AzSbListener;

public class QueueOptions
{
    public const string SectionName = nameof(QueueOptions);

    public int MaxConcurrentCalls {get; set;}
    public int MaxAutoRenewDurationInMins {get; set;}
    public string? ConnectionString {get; set;}
    public string? QueueName {get;set;}
}