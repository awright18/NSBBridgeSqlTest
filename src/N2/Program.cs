using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;

static class Program
{
    static async Task Main()
    {
        Console.Title = "N2";
        var endpointConfiguration = new EndpointConfiguration("N2");
        endpointConfiguration.UsePersistence<LearningPersistence>();

        endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Name == "CreateOrder");
        endpointConfiguration.Conventions().DefiningMessagesAs(t => t.Name == "CreateOrderResponse");
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Name == "OrderCreated");

        #region alternative-learning-transport

        endpointConfiguration.UseTransport(
            new SqlServerTransport(@"Server=.\SqlExpress;Database=N2;Integrated Security=true;")
            {
                TransportTransactionMode = TransportTransactionMode.SendsAtomicWithReceive
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