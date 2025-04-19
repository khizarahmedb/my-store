using MongoDB.Driver;
using Microsoft.Extensions.Options;
using dotnet.Models;

public class DbContext
{
    private readonly IMongoDatabase _database;
    public DbContext(IOptions<DatabaseSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
        Console.WriteLine("Successfully connected to MongoDB!");
    }
    public IMongoCollection<ProductViewModel> Products => _database.GetCollection<ProductViewModel>("Products");
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}