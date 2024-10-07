using MassTransit;
using Order.API.Contexts;
using Order.API.Entities;
using Order.API.Enums;
using Order.API.Models;
using Shared.Events;
using Shared.Messages;
using Shared.Settings;

namespace Order.API.Services;

public interface IOrderService
{
    Task CreateOrderAsync(OrderCreateRequestModel request);
}

public class OrderService(ApplicationDbContext dbContext, ISendEndpointProvider sendEndpointProvider) : IOrderService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task CreateOrderAsync(OrderCreateRequestModel request)
    {
        var order = new Entities.Order()
        {
            BuyerId = request.BuyerId,
            OrderItems = request.OrderItems.Select(oi => new OrderItem
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId
            }).ToList(),
            OrderStatus = OrderStatus.Suspend,
            TotalPrice = request.OrderItems.Sum(oi => oi.Count * oi.Price),
            CreatedDate = DateTime.Now
        };

        await _dbContext.AddAsync<Entities.Order>(order);
        await _dbContext.SaveChangesAsync();

        OrderStartedEvent orderStartedEvent = new()
        {
            BuyerId = order.BuyerId,
            OrderId = order.Id,
            TotalPrice = order.OrderItems.Sum(oi => oi.Count * oi.Price),
            OrderItems = order.OrderItems.Select(oi => new OrderItemMessage(oi.ProductId, oi.Count, oi.Price)).ToList()
        };

        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new($"queue:{RabbitMQSettings.StateMachine}"));
        await sendEndpoint.Send<OrderStartedEvent>(orderStartedEvent);
    }
}
