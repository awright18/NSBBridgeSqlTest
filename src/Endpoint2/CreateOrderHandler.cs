using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared.Commands;
using Shared.Events;
using Shared.Messages;

namespace N2;

public class CreateOrderHandler : IHandleMessages<CreateOrder>
{
    public async Task Handle(CreateOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Received CreateOrder Command with Id {message.OrderId}");

        Console.WriteLine($"Received CreateOrder Command with ReplyToAddress {context.ReplyToAddress}");

        await context.Reply(new CreateOrderResponse() { OrderId = message.OrderId });
        
        Console.WriteLine($"Sent CreateOrderResponse to Endpoint1 with OrderId {message.OrderId}");

        await context.Publish(new OrderCreated() { OrderId = message.OrderId }).ConfigureAwait(false);
        
        Console.WriteLine($"Published OrderCreated event with ReplyToAddress {context.ReplyToAddress}");
    }
}