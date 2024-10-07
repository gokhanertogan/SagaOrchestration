using MassTransit;
using Order.API.Contexts;
using Order.API.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class OrderFailedEventConsumer(ApplicationDbContext dbContext) : IConsumer<OrderFailedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        Entities.Order? order = await _dbContext.FindAsync<Entities.Order>(context.Message.OrderId);
        if (order is not null)
        {
            order.OrderStatus = OrderStatus.Fail;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(context.Message.Message);
        }
    }
}
