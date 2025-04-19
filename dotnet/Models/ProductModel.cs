namespace dotnet.Models;

public class ProductViewModel
{
    public MongoDB.Bson.ObjectId id { get; set; }

    public string description { get; set; }
    public int price { get; set; }
    public string category { get; set; }
    public string image { get; set; }
    
}