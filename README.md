# sample-dotnet-servicebuslistener

## AzSbListener ##

Based upon this implementation [https://github.com/davidfowl/Achievements]

Sample for creating service bus listener application in .net.
- Using background hosted service that listens on a queue for a message
- Implements healthcheck endpoint for containerization

### Build ###
dotnet build

### Run ###
dotnet run

### Healthcheck ###
[http://localhost:5000/healthcheck]

## AspireRabbitMq

Based upon this implementation [https://github.com/ryanninodizon/AspireWithRabbitMQ]

Sample for creating queue listener in .net using Aspire and RabbitMQ integration

### Build and Run ###
cd AspireRabbitMq
dotnet run --project ./AspireWithRabbitMQ.AppHost

### Test ###
click the link in log to open Aspire dashboard
