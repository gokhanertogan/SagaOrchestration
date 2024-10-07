namespace Shared.Messages;
public record StockRollBackMessage(List<OrderItemMessage> OrderItems);