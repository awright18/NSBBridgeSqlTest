using System;
using System.Threading.Tasks;
using NServiceBus;

namespace N2;

public class CreateOrderHandler : IHandleMessages<CreateOrder>
{
    public async Task Handle(CreateOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Received CreateOrder Command with Id {message.OrderId}");

        await context.Reply(new CreateOrderResponse { OrderId = message.OrderId }).ConfigureAwait(false);

        await context.Publish(new OrderCreated() { OrderId = message.OrderId }).ConfigureAwait(false);
    }
}