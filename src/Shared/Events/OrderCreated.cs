using System;
using NServiceBus;

namespace Shared.Events;
public class OrderCreated
{
    public Guid OrderId { get; set; }
}