namespace Shared.Messages;

public record OrderItemMessage(int ProductId, int Count, decimal Price);