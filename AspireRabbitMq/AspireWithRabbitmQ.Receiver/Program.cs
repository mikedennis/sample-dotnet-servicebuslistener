using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRabbitMQClient("messaging");

builder.Services.AddScoped<IAmqpClient, RabbitMqClient>();
builder.Services.AddScoped<IProcessingService, ProcessingService>();

builder.Services.AddHostedService<QueueListener>();

var app = builder.Build();

app.Run();

