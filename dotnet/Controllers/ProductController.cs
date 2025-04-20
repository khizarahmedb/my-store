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

public class UpdatePriceRequest
{
    public int Price { get; set; }
}


[Route("products")]
public class ProductController(DbContext dbContext) : Controller
{
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync($"https://fakestoreapi.com/products/{id}");
            response.EnsureSuccessStatusCode();
            var apiResponseBody = await response.Content.ReadAsStringAsync();
            var storeProduct = JsonSerializer.Deserialize<StoreProduct>(apiResponseBody);
            var dbProduct = await dbContext.Products.Find(p => p.ApiId == id).FirstOrDefaultAsync(); 
            var combinedProduct = new
            {
                dbProduct.id,
                dbProduct.ApiId,
                dbProduct.description,
                dbProduct.price,
                dbProduct.category,
                dbProduct.image,
                storeProduct!.title
            };

            return Ok(combinedProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetProducts()
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync("https://fakestoreapi.com/products");
            response.EnsureSuccessStatusCode();
            var apiResponseBody = await response.Content.ReadAsStringAsync();
            var storeProducts = JsonSerializer.Deserialize<List<StoreProduct>>(apiResponseBody);

            if (storeProducts == null)
            {
                return StatusCode(500, "Failed to get product data from external API.");
            }

            var storeProductsDictionary = storeProducts.ToDictionary(p => p.id);

            var dbProducts = await dbContext.Products.Find(_ => true).ToListAsync();

            var combinedProducts = new List<object>();

            foreach (var dbProduct in dbProducts)
            {
                if (!storeProductsDictionary.TryGetValue(dbProduct.ApiId, out var apiProduct)) continue;
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
                        image = storeProduct.image,
                        price = storeProduct.price
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
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductPrice(int id, [FromBody] UpdatePriceRequest request)
    {
        try
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.ApiId, id);
            var dbProduct = await dbContext.Products.Find(filter).FirstOrDefaultAsync();
            var update = Builders<ProductModel>.Update.Set(p => p.price, request.Price);
            await dbContext.Products.UpdateOneAsync(filter, update);

            dbProduct.price = request.Price; 

            return Ok(dbProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }
    
}