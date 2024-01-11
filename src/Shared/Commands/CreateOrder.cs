using System;
using NServiceBus;

namespace Shared.Commands;

public class CreateOrder
{
    public Guid OrderId { get; set; }
}