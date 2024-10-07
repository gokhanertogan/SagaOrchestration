using MongoDB.Driver;

namespace Stock.API;

public class DataInitializer(MongoDbService dbService)
{
    private readonly MongoDbService _dbService = dbService;

    public void SeedData()
    {
        var stockCollection = _dbService.GetCollection<Entities.Stock>();

        if (!stockCollection.FindSync(x => true).Any())
        {
            stockCollection.InsertOne(new Entities.Stock
            {
                ProductId = 21,
                Count = 200
            });
            stockCollection.InsertOne(new Entities.Stock
            {
                ProductId = 22,
                Count = 100
            });
            stockCollection.InsertOne(new Entities.Stock
            {
                ProductId = 23,
                Count = 50
            });
            stockCollection.InsertOne(new Entities.Stock
            {
                ProductId = 24,
                Count = 10
            });
            stockCollection.InsertOne(new Entities.Stock
            {
                ProductId = 25,
                Count = 30
            });
        }
    }
}