using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;
using Shared;

namespace Endpoint2;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Endpoint2";
        var endpointConfiguration = new EndpointConfiguration("Endpoint2");

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();

        var connectionString = @"Server=localhost;Database=Db2;User Id=N2User;Password=Db2Password!;";
        // var connectionString = @"Server=.\SqlExpress;Database=Endpoint2;Integrated Security=true;";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Namespace == "Shared.Commands");
        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Namespace == "Shared.Messages");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Namespace == "Shared.Events");

        #region alternative-learning-transport

        endpointConfiguration.UseTransport(
            // new SqlServerTransport(@"Server=.\SqlExpress;Database=Endpoint2;Integrated Security=true;")
            new SqlServerTransport(@"Server=localhost;Database=Db2;User Id=Db2User;Password=Db2Password!;")
            {
                TransportTransactionMode = TransportTransactionMode.ReceiveOnly
            });

        #endregion

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

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