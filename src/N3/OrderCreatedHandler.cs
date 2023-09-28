using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class OrderCreatedHandler :
    IHandleMessages<OrderCreated>
{
    static ILog log = LogManager.GetLogger<OrderCreatedHandler>();

    public Task Handle(OrderCreated message, IMessageHandlerContext context)
    {
        log.Info($"Subscriber has received OrderCreated event with OrderId {message.OrderId}.");
        return Task.CompletedTask;
    }
}