using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;

namespace N3;

static class Program
{
    static async Task Main()
    {
        Console.Title = "N3";
        var endpointConfiguration = new EndpointConfiguration("N3");
        
        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>(); 
        var connectionString = @"Server=.\SqlExpress;Database=N3;Integrated Security=true;";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));
        
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.UseTransport(
            new SqlServerTransport(@"Server=.\SqlExpress;Database=N3;Integrated Security=true;")
            {
                TransportTransactionMode = TransportTransactionMode.ReceiveOnly
            });

        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Name == "CreateOrderResponse");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Name == "OrderCreated");

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