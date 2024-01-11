using System;
using NServiceBus;

namespace Shared.Events;

public class OrderBilled
{
    public Guid OrderId { get; set; }
}