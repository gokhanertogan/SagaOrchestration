using MassTransit;

namespace Shared.Events;

public class PaymentCompletedEvent(Guid correlationId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; } = correlationId;
}
