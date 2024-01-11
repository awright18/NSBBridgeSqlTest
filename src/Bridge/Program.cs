using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Shared;
using Shared.Events;

namespace Bridge;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Bridge";

        await Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                });
            })
            .UseNServiceBusBridge((ctx, bridgeConfiguration) =>
            {
                var bridgeTransport1 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=Db1;User Id=Db1User;Password=Db1password!;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-Endpoint1", AutoCreateQueues = true
                };

                bridgeTransport1.HasEndpoint("Endpoint1");

                var bridgetTransport2 = new BridgeTransport(
                    new SqlServerTransport(@"Server=localhost;Database=Db2;User Id=Db2User;Password=Db2Password!;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-Endpoint2", AutoCreateQueues = true
                };

                bridgetTransport2.HasEndpoint("Endpoint2");

                var bridgeTransport3 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=Db3;User Id=Db3User;Password=Db3Password!;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-Endpoint3", AutoCreateQueues = true
                };

                var endpoint3 = new BridgeEndpoint("Endpoint3");

                endpoint3.RegisterPublisher<OrderCreated>("Endpoint2");
                
                bridgeTransport3.HasEndpoint(endpoint3);

                var bridgeTransport4 = new BridgeTransport(
                    new SqlServerTransport(@"Server=localhost;Database=Db4;User Id=Db4User;Password=Db4Password!")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-Endpoint4", AutoCreateQueues = true
                };

                var endpoint4 = new BridgeEndpoint("Endpoint4");
                endpoint4.RegisterPublisher<OrderBilled>("Endpoint3");
                
                bridgeTransport4.HasEndpoint(endpoint4);
                
                bridgeConfiguration.AddTransport(bridgeTransport1);
                bridgeConfiguration.AddTransport(bridgetTransport2);
                bridgeConfiguration.AddTransport(bridgeTransport4);
                bridgeConfiguration.AddTransport(bridgeTransport3);
                
                
            })
            .Build()
            .RunAsync().ConfigureAwait(false);
    }
}