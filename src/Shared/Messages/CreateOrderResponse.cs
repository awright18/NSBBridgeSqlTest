using System;
using NServiceBus;

namespace Shared.Messages;
public class CreateOrderResponse
{
    public Guid OrderId { get; set; }
}