using MassTransit;
using Shared.Messages;

namespace Shared.Events;

public record StockReservedEvent : CorrelatedBy<Guid>
{
    public StockReservedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public Guid CorrelationId { get; }
    public List<OrderItemMessage>? OrderItems { get; set; }
}
