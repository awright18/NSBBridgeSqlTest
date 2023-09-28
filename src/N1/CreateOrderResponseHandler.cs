using System;
using System.Threading.Tasks;
using NServiceBus;

public class CreateOrderResponseHandler : IHandleMessages<CreateOrderResponse>
{
    public Task Handle(CreateOrderResponse message, IMessageHandlerContext context)
    {
        Console.WriteLine($"OrderResponse Reply received with Id {message.OrderId}");

        return Task.CompletedTask;
    }
}