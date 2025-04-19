using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<DatabaseSettings>(config =>
{
    config.ConnectionString = System.Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")!;
    config.DatabaseName = System.Environment.GetEnvironmentVariable("DATABASE_NAME")!;
});
builder.Services.AddSingleton<DbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
    try
    {
        var collection = dbContext.Products;
        if (collection == null)
        {
            throw new Exception("Failed to get the Products collection.");
        }

        var count = await collection.EstimatedDocumentCountAsync();
        Console.WriteLine($"Estimated document count in Products collection: {count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    }
}
app.Run();
