# Notification Service (.NET Core)

This solution contains two console applications:

- **Notification.Producer**: Sends messages to Azure Event Hub.
- **Notification.Consumer**: Receives messages from Azure Event Hub.

## Prerequisites
- .NET Core SDK 3.1 or later
- Azure Event Hub namespace and event hub
- Azure Storage account (for consumer checkpointing)

## Configuration
Add your Azure Event Hub and Storage connection strings to the respective appsettings or as environment variables.

## How to Run
1. Build the solution:
   ```
dotnet build
   ```
2. Run the producer:
   ```
dotnet run --project Notification.Producer
   ```
3. Run the consumer:
   ```
dotnet run --project Notification.Consumer
   ```


## Entity Framework Core Migrations (Notification.Consumer)

To set up the notifications table in your database:

1. Open a terminal in the solution root.
2. Run the following commands:
   ```
   dotnet tool install --global dotnet-ef # if not already installed
   dotnet ef migrations add InitialCreate -p Notification.Consumer -s Notification.Consumer
   dotnet ef database update -p Notification.Consumer -s Notification.Consumer
   ```
3. Update your `appsettings.json` with your Azure SQL connection string before running `database update` in production.

## Packages Used
- Azure.Messaging.EventHubs
- Azure.Identity
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design

## Notes
- Update the code with your Event Hub and Storage details before running.

## Email Notification Feature

This branch introduces an email notification feature to the Notification Service. With this functionality, the service can send email alerts to users based on specific events or triggers. The implementation uses standard .NET libraries for SMTP or can be extended to integrate with third-party email providers. Configuration options for email recipients, SMTP server, and message templates are available in the app settings.
