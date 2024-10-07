using MassTransit;
using Shared.Messages;

namespace Shared.Events;

public class PaymentFailedEvent(Guid correlationId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; } = correlationId;
    public List<OrderItemMessage>? OrderItems { get; set; }
    public string? Message { get; set; }
}
