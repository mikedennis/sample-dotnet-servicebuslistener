namespace AzSbListener;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<QueueConnectionHealthCheck>();
        builder.Services.AddHealthChecks()
            .AddCheck<QueueConnectionHealthCheck>("QueueConnectionHealthCheck", tags: new[] {"ready"});
        builder.Services.AddHostedService<QueueListener>();
        
        var app = builder.Build();
        app.MapHealthChecks("/healthcheck");
        app.Run();
    }
}