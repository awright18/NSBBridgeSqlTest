using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

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
                var n1 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N1;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    
                    Name = $"SQL-N1", AutoCreateQueues = true
                };
                
                n1.HasEndpoint("N1");

                var n2 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N2;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-N2", AutoCreateQueues = true
                };

                n2.HasEndpoint("N2");
                
                var n3 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N3;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-N3", AutoCreateQueues = true
                };

                var n3Endpoint = new BridgeEndpoint("N3");
                
                n3Endpoint.RegisterPublisher("OrderCreated", "N2");

                n3.HasEndpoint(n3Endpoint);
                
                var n4 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N4;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.ReceiveOnly
                    })
                {
                    Name = $"SQL-N4", AutoCreateQueues = true
                };

                var n4Endpoint = new BridgeEndpoint("N4");
                
                n4Endpoint.RegisterPublisher("OrderCreated", "N2");

                n4.HasEndpoint(n4Endpoint);
                
                bridgeConfiguration.AddTransport(n1);
                bridgeConfiguration.AddTransport(n2);
                bridgeConfiguration.AddTransport(n3);
                bridgeConfiguration.AddTransport(n4);
            })
            .Build()
            .RunAsync().ConfigureAwait(false);
    }
}