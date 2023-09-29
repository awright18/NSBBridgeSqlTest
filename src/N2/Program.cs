using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;

namespace N2;

static class Program
{
    static async Task Main()
    {
        Console.Title = "N2";
        var endpointConfiguration = new EndpointConfiguration("N2");
        
        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        
        var connectionString = @"Server=.\SqlExpress;Database=N2;Integrated Security=true;";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Name == "CreateOrder");
        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Name == "CreateOrderResponse");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Name == "OrderCreated");

        #region alternative-learning-transport

        endpointConfiguration.UseTransport(
            new SqlServerTransport(@"Server=.\SqlExpress;Database=N2;Integrated Security=true;")
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