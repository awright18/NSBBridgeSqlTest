using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared.Messages;

namespace N1;

public class CreateOrderResponseHandler : IHandleMessages<CreateOrderResponse>
{
    public Task Handle(CreateOrderResponse message, IMessageHandlerContext context)
    {
        Console.WriteLine($"OrderResponse ReplyTo Address{context.ReplyToAddress}");

        Console.WriteLine($"OrderResponse Reply received with Id {message.OrderId}");

        return Task.CompletedTask;
    }
}