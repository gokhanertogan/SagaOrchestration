namespace Shared.Settings;

public class RabbitMQSettings
{
    public const string StateMachine = "state-machine-queue";
    public const string Stock_OrderCreatedEventQueue = "stock-order-created-queue";
    public const string Payment_StartedEventQueue = "payment-started-queue";
    public const string Order_OrderCompletedEventQueue = "order-order-completed-queue";
    public const string Order_OrderFailedEventQueue = "order-order-failed-queue";
    public const string Stock_RollbackMessageQueue = "stock-rollback-queue";
}