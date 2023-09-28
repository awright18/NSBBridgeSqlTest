using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;

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
                        TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive
                    })
                {
                    
                    Name = $"SQL-N1", AutoCreateQueues = true
                };
                
                n1.HasEndpoint("N1");

                var n2 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N2;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive
                    })
                {
                    Name = $"SQL-N2", AutoCreateQueues = true
                };

                n2.HasEndpoint("N2");
                
                var n3 = new BridgeTransport(
                    new SqlServerTransport(@"Server=.\SqlExpress;Database=N3;Integrated Security=true;")
                    {
                        TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive
                    })
                {
                    Name = $"SQL-N3", AutoCreateQueues = true
                };

                var n3Endpoint = new BridgeEndpoint("N3");
                
                n3Endpoint.RegisterPublisher("OrderCreated", "N2");

                n3.HasEndpoint(n3Endpoint);
                
                bridgeConfiguration.AddTransport(n1);
                bridgeConfiguration.AddTransport(n2);
                // bridgeConfiguration.AddTransport(n3);
            })
            .Build()
            .RunAsync().ConfigureAwait(false);
    }
}