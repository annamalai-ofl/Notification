using Notification.Producer;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Notification.Producer.Services;

var builder = WebApplication.CreateBuilder(args);

// Build configuration
builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<EventHubProducerClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var eventHubConnectionString = config["EventHub:ConnectionString"];
    var eventHubName = config["EventHub:Name"];
    return new EventHubProducerClient(eventHubConnectionString, eventHubName);
});
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

// Add SignalR with Azure SignalR Service
builder.Services.AddSignalR().AddAzureSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Do NOT use static files or default files middleware

// Enable Swagger in all environments for API testing
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
