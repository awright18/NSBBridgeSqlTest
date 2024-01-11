using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Events;
using Shared.Messages;

namespace N4;

public class OrderBilledHandler :
    IHandleMessages<OrderBilled>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderBilledHandler>();

    public async Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        Log.Info($"Subscriber has received OrderBilled event with OrderId {message.OrderId}.");

        await context.Reply(new OrderBilledResponse() { OrderId = message.OrderId });
        
        Log.Info($"Replied to publisher of OrderBilled with OrderId {message.OrderId}");
        
        
    }
}