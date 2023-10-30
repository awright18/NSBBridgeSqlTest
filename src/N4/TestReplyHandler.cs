using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace N4;

public class TestReplyHandler :
    IHandleMessages<TestingReply>
{
    private static readonly ILog Log = LogManager.GetLogger<OrderCreatedHandler>();

    public async Task Handle(TestingReply message, IMessageHandlerContext context)
    {
        Log.Info($"ReplyToAddress {context.ReplyToAddress}");
        
        Log.Info($"Subscriber has received TestingReply event with OrderId {message.TestingReplyId}.");
        
    }
}