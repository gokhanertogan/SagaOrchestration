using MassTransit;
using Shared.Events;
using Shared.Settings;

namespace Payment.API.Consumers;

public class PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentStartedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
    {
        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachine}"));
        if (context.Message.TotalPrice <= 100)
            await sendEndpoint.Send(new PaymentCompletedEvent(context.Message.CorrelationId));
        else
            await sendEndpoint.Send(new PaymentFailedEvent(context.Message.CorrelationId)
            {
                Message = "Could not be success",
                OrderItems = context.Message.OrderItems
            });
    }
}
