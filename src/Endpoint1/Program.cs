using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;
using Shared.Commands;
using Shared.Events;

namespace Endpoint1;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Endpoint1";
        var endpointConfiguration = new EndpointConfiguration("Endpoint1");

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();

        var connectionString = @"Server=.\SqlExpress;Database=Db1;User Id=Db1User;Password=Db1password!";
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () => new SqlConnection(connectionString));
        var subscriptions = persistence.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(1));

        endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Namespace == "Shared.Commands");
        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Namespace == "Shared.Messages");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Namespace == "Shared.Events");
        
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        var routing =
            endpointConfiguration.UseTransport(
                new SqlServerTransport(@"Server=.\SqlExpress;Database=Db1;User Id=Db1User;Password=Db1password!;")
                {
                    TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                });
        routing.RouteToEndpoint(typeof(CreateOrder), "Endpoint2");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        await Start(endpointInstance)
            .ConfigureAwait(false);
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0010:Add missing cases", Justification = "<Pending>")]
    static async Task Start(IEndpointInstance endpointInstance)
    {
        Console.WriteLine("Press '1' to send the CreateOrder command");
        Console.WriteLine("Press '2' to publish the OrderCreated event");
        Console.WriteLine("Press 'esc' other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            var orderId = Guid.NewGuid();
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    var createOrder = new CreateOrder { OrderId = orderId };
                    await endpointInstance.Send(createOrder)
                        .ConfigureAwait(false);
                    Console.WriteLine($"Send CreateOrder Command with Id {orderId}");
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    var orderCreated = new OrderCreated { OrderId = orderId };
                    await endpointInstance.Publish(orderCreated)
                        .ConfigureAwait(false);
                    Console.WriteLine($"Published OrderCreated Event with Id {orderId}.");
                    break;
                case ConsoleKey.Escape:
                    return;
                default:
                    break;
            }
        }
    }
}