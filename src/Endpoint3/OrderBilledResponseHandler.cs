using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared.Messages;

namespace Endpoint3;

public class OrderBilledResponseHandler :
    IHandleMessages<OrderBilledResponse>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderBilledResponseHandler>();

    public async Task Handle(OrderBilledResponse message, IMessageHandlerContext context)
    {
        Log.Info($"Subscriber has received OrderBilledResponse message with OrderId {message.OrderId}.");
    }
}