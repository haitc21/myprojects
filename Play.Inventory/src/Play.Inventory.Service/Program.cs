using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

Random jitterer = new Random();
services.AddMongo()
        .AddMongoRepository<InventoryItem>("InventoryItems");

// Cấu hình HttpClient Play.Catalog
// Sử dụng package Microsoft.Extensions.Http.Polly
services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7158");
})
// Cấu hình retry request. WaitAndRetryAsync
.AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    5,
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
    onRetry: (outcome, timespan, retryAttempts) =>
    {
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
        .LogWarning($"delay for {timespan.TotalSeconds} seconds, then making retry {retryAttempts}");
    }
))
// circuit breaker partern
.AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3,
    TimeSpan.FromSeconds(15),
    onBreak: (outcome, timespan) =>
    {
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
        .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds");
    },
    onReset: () =>
    {
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
        .LogWarning($"Closing the circuit");
    }
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
