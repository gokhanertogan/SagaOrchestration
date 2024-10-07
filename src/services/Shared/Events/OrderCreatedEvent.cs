using Shared.Messages;
using MassTransit;

namespace Shared.Events;

public class OrderCreatedEvent(Guid correlationId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; } = correlationId;
    public List<OrderItemMessage>? OrderItems { get; set; }
}