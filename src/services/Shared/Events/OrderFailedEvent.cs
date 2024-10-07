namespace Shared.Events;

public record OrderFailedEvent(int OrderId, string Message);