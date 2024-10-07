using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Settings;

namespace Stock.API.Consumers;

public class OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider) : IConsumer<OrderCreatedEvent>
{
    private readonly MongoDbService _mongoDbService = mongoDbService;
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        IMongoCollection<Entities.Stock> collection = _mongoDbService.GetCollection<Entities.Stock>();
        bool allItemsInStock = await Task.WhenAll(context.Message.OrderItems!.Select(async orderItem =>
        {
            return await collection.Find(s => s.ProductId == orderItem.ProductId && s.Count > orderItem.Count).AnyAsync();
        })).ContinueWith(result => result.Result.All(r => r));

        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachine}"));

        if (allItemsInStock)
        {
            var updates = context.Message.OrderItems!.Select(orderItem =>
            {
                var filter = Builders<Entities.Stock>.Filter.Eq(s => s.ProductId, orderItem.ProductId);
                var update = Builders<Entities.Stock>.Update.Inc(s => s.Count, -orderItem.Count);
                return new UpdateOneModel<Entities.Stock>(filter, update);
            });

            await collection.BulkWriteAsync(updates);

            StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
            {
                OrderItems = context.Message.OrderItems
            };

            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
            {
                Message = "Stock could not be reserved"
            };

            await sendEndpoint.Send(stockNotReservedEvent);
        }
    }
}
