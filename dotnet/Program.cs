using dotnet.Database;
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
builder.Services.AddScoped<DbContext>();

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

app.Run();