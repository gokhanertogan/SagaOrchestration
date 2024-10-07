using MassTransit;
using Order.API.Contexts;
using Order.API.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class OrderCompletedEventConsumer(ApplicationDbContext dbContext) : IConsumer<OrderCompletedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        Entities.Order? order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = OrderStatus.Completed;
            await _dbContext.SaveChangesAsync();
        }
    }
}
