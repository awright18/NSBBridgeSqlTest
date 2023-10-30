using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace N2;

public class CreateOrderResponseHandler : IHandleMessages<CreateOrderResponse>
{
    public async Task Handle(CreateOrderResponse message, IMessageHandlerContext context)
    {
        Console.WriteLine($"ReplyToAddress {context.ReplyToAddress}");
        
        Console.WriteLine($"OrderResponse Reply received with Id {message.OrderId}");

        await context.Reply(new TestingReply() { TestingReplyId = message.OrderId });
    }
}