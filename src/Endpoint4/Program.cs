using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;
using Shared;

namespace Endpoint4;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Endpoint4";
        var endpointConfiguration = new EndpointConfiguration("Endpoint4");

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();

        var connectionString = @"Server=localhost;Database=Db4;User Id=Db4User;Password=Db4Password!";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.UseTransport(
            new SqlServerTransport(@"Server=localhost;Database=Db4;User Id=Db4User;Password=Db4Password!")
            {
                TransportTransactionMode = TransportTransactionMode.ReceiveOnly
            });
        
        endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Namespace == "Shared.Commands");
        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Namespace == "Shared.Messages");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Namespace == "Shared.Events");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}