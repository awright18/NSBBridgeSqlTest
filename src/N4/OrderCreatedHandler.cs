using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace N4;

public class OrderCreatedHandler :
    IHandleMessages<OrderCreated>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderCreatedHandler>();

    public Task Handle(OrderCreated message, IMessageHandlerContext context)
    {
        Log.Info($"Subscriber has received OrderCreated event with OrderId {message.OrderId}.");
        return Task.CompletedTask;
    }
}