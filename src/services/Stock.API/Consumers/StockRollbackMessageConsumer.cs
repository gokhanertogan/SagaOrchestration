using MassTransit;
using MongoDB.Driver;
using Shared.Messages;

namespace Stock.API.Consumers;

public class StockRollbackMessageConsumer(MongoDbService dbService) : IConsumer<StockRollBackMessage>
{
    private readonly MongoDbService _dbService = dbService;

    public async Task Consume(ConsumeContext<StockRollBackMessage> context)
    {
        var collection = _dbService.GetCollection<Entities.Stock>();

        foreach (var item in context.Message.OrderItems)
        {
            Entities.Stock stock = await collection.Find(s => s.ProductId == item.ProductId).FirstOrDefaultAsync();
            
            if (stock != null)
            {
                stock.Count += item.Count;
                await collection.FindOneAndReplaceAsync(s => s.ProductId == item.ProductId, stock);
            }
        }
    }
}
