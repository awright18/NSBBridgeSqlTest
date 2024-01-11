using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;
using Shared;

namespace N3;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Endpoint3";
        var endpointConfiguration = new EndpointConfiguration("Endpoint3");

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        var connectionString = @"Server=.\SqlExpress;Database=Db3;User Id=Db3User;Password=Db3Password!";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.UseTransport(
            new SqlServerTransport(@"Server=.\SqlExpress;Database=Db3;User Id=Db3User;Password=Db3Password!")
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