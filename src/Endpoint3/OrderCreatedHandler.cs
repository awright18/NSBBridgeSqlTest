using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Events;

namespace Endpoint3;

public class OrderCreatedHandler :
    IHandleMessages<OrderCreated>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderCreatedHandler>();

    public async Task Handle(OrderCreated message, IMessageHandlerContext context)
    {
        Log.Info($"Subscriber has received OrderCreated event with OrderId {message.OrderId}.");

        await context.Publish(new OrderBilled() { OrderId = message.OrderId });
        
        Log.Info($"OrderBilled Published with OrderId {message.OrderId}");
    }
}