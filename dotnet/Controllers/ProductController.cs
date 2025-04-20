using System.Text.Json;
using System.Text.Json.Serialization;
using dotnet.Database;
using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using dotnet.Database;

namespace dotnet.Controllers;

public class StoreProduct
{
    public int id { get; set; }
    public string title { get; set; }
    public double price { get; set; }
    public string description { get; set; }
    public string category { get; set; }
    public string image { get; set; }
    public StoreRating rating { get; set; }
}

public class StoreRating
{
    public double rate { get; set; }
    public int count { get; set; }
}


[Route("products")]
public class ProductController(DbContext dbContext) : Controller
{

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        return Ok();
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetProducts()
    {
        using var httpClient = new HttpClient();
        try
        {
            var apiResponse = await httpClient.GetAsync("https://fakestoreapi.com/products");
            apiResponse.EnsureSuccessStatusCode();
            var apiResponseBody = await apiResponse.Content.ReadAsStringAsync();
            var storeProducts = JsonSerializer.Deserialize<List<StoreProduct>>(apiResponseBody);

            if (storeProducts == null)
            {
                return StatusCode(500, "Failed to get product data from external API.");
            }

            var apiProductsDictionary = storeProducts.ToDictionary(p => p.id);

            var dbProducts = await dbContext.Products.Find(_ => true).ToListAsync();

            var combinedProducts = new List<object>();

            foreach (var dbProduct in dbProducts)
            {
                if (!apiProductsDictionary.TryGetValue(dbProduct.ApiId, out var apiProduct)) continue;
                var combinedProduct = new
                {
                    dbProduct.id,
                    dbProduct.ApiId,
                    dbProduct.description,
                    dbProduct.price,
                    dbProduct.category,
                    dbProduct.image,
                    apiProduct.title
                };
                combinedProducts.Add(combinedProduct);
            }

            return Ok(combinedProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }

    [HttpGet("initialize")]
    public  async Task<IActionResult> InitializeProducts()
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync("https://fakestoreapi.com/products");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var storeProducts = JsonSerializer.Deserialize<List<StoreProduct>>(responseBody);
            var existingProducts = await dbContext.Products.Find(_ => true).Project(p => p.ApiId).ToListAsync();
            var productsToInsert = new List<ProductModel>();
            var productsSkipped = 0;

            if (storeProducts == null) return Content(responseBody, "application/json");
            foreach (var storeProduct in storeProducts)
            {
                if (!existingProducts.Contains(storeProduct.id))
                {
                    productsToInsert.Add(new ProductModel
                    {
                        id = ObjectId.GenerateNewId(),
                        ApiId = storeProduct.id,
                        description = storeProduct.description,
                        category = storeProduct.category,
                        image = storeProduct.image
                    });
                }
                else
                {
                    productsSkipped++;
                }
            }

            if (productsToInsert.Count > 0)
            {
                await dbContext.Products.InsertManyAsync(productsToInsert);
            }

            return Ok(
                $"Successfully fetched {storeProducts.Count} products. Inserted {productsToInsert.Count} new products. Skipped {productsSkipped} existing products.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }
}