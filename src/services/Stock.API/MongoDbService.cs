using MongoDB.Driver;

namespace Stock.API;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        if (configuration != null)
        {
            var mongoServer = configuration["MongoDB:Server"];
            var databaseName = configuration["MongoDB:DBName"];

            if (string.IsNullOrWhiteSpace(mongoServer))
                throw new InvalidOperationException("MongoDB server configuration is missing or invalid.");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("MongoDB database name configuration is missing or invalid.");

            var client = new MongoClient(mongoServer);
            _database = client.GetDatabase(databaseName);
        }
        else
            throw new ArgumentNullException(nameof(configuration));
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = typeof(T).Name.ToLowerInvariant();
        return _database.GetCollection<T>(collectionName);
    }
}