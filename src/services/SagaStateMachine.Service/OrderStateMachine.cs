using MassTransit;
using Shared.Events;
using Shared.Messages;
using Shared.Settings;

namespace SagaStateMachine.Service;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<OrderStartedEvent>? OrderStartedEvent { get; set; }
    public Event<StockReservedEvent>? StockReservedEvent { get; set; }
    public Event<PaymentCompletedEvent>? PaymentCompletedEvent { get; set; }
    public Event<PaymentFailedEvent>? PaymentFailedEvent { get; set; }
    public Event<StockNotReservedEvent>? StockNotReservedEvent { get; set; }

    public State? OrderCreated { get; set; }
    public State? StockReserved { get; set; }
    public State? PaymentCompleted { get; set; }
    public State? PaymentFailed { get; set; }
    public State? StockNotReserved { get; set; }

    public OrderStateMachine()
    {
        InstanceState(instance => instance.CurrentState);

        Event(() => OrderStartedEvent,
                orderStateInstance => orderStateInstance.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId)
                .SelectId(e => Guid.NewGuid()));

        Event(() => StockReservedEvent,
          orderStateInstance =>
          orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

        Event(() => StockNotReservedEvent,
          orderStateInstance =>
          orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

        Event(() => StockNotReservedEvent,
            orderStateInstance =>
            orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

        Event(() => PaymentCompletedEvent,
            orderStateInstance =>
            orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

        Event(() => PaymentFailedEvent,
            orderStateInstance =>
            orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

        Initially(When(OrderStartedEvent)
            .Then(context =>
            {
                context.Saga.BuyerId = context.Message.BuyerId;
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.TotalPrice = context.Message.TotalPrice;
                context.Saga.CreatedDate = DateTime.Now;
            })
            .Then(context => Console.WriteLine("step-1"))
            .Then(context => Console.WriteLine("step-2"))
            .TransitionTo(OrderCreated)
            .Then(context => Console.WriteLine("step-3"))
            .Send(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEventQueue}"), context => new OrderCreatedEvent(context.Saga.CorrelationId)
            {
                OrderItems = context.Message.OrderItems
            }));

        During(OrderCreated,
            When(StockReservedEvent)
            .TransitionTo(StockReserved)
            .Send(new Uri($"queue:{RabbitMQSettings.Payment_StartedEventQueue}"), context => new PaymentStartedEvent(context.Saga.CorrelationId)
            {
                OrderItems = context.Message.OrderItems,
                TotalPrice = context.Saga.TotalPrice
            }),
            When(StockNotReservedEvent)
            .TransitionTo(StockNotReserved)
            .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"), context =>
            new OrderFailedEvent(context.Saga.OrderId, context.Message.Message!)));

        During(StockReserved,
            When(PaymentCompletedEvent)
            .TransitionTo(PaymentCompleted)
            .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderCompletedEventQueue}"), context =>
            new OrderCompletedEvent(context.Saga.OrderId))
            .Finalize(),

            When(PaymentFailedEvent)
            .TransitionTo(PaymentFailed)
            .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"), context =>
            new OrderFailedEvent(context.Saga.OrderId, context.Message.Message!))
            .Send(new Uri($"queue:{RabbitMQSettings.Stock_RollbackMessageQueue}"), context =>
            new StockRollBackMessage(context.Message.OrderItems!)));

        SetCompletedWhenFinalized();
    }
}