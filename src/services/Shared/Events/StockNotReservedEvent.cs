using MassTransit;

namespace Shared.Events;

public class StockNotReservedEvent(Guid correlationId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; } = correlationId;
    public string? Message { get; set; }
}
