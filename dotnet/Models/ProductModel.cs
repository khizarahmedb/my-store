namespace dotnet.Models;

public class ProductViewModel
{
    public MongoDB.Bson.ObjectId id RequestId { get; set; }

    public string description { get; set; };
    public integer price { get; set; }
    public string category { get; set; }
    public string image { get; set; }
    
}