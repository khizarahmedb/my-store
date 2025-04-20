using dotnet.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dotnet.Database;

public class DbContext
{
    private readonly IMongoDatabase _database;
    public DbContext(IOptions<DatabaseSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
        Console.WriteLine("Successfully connected to MongoDB!");
    }
    public IMongoCollection<ProductModel> Products => _database.GetCollection<ProductModel>("Products");
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}