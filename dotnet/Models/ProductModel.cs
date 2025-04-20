namespace dotnet.Models;

public class ProductModel
{
    public MongoDB.Bson.ObjectId id { get; set; }
    public int ApiId { get; set; }
    public string description { get; set; }
    public int price { get; set; }
    public string category { get; set; }
    public string image { get; set; }
    
}