using MassTransit;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDb;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
// Sử dụng Play.Common package
services.AddMongo()
        .AddMongoRepository<Item>("Items")
        .AddMassTransitWithRabbitMq();

services.AddControllers(option =>
{
    option.SuppressAsyncSuffixInActionNames = false;
});
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
