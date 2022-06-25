using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Consumers;
public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _repository;
    public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }
    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;
        var item = await _repository.GetAsync(message.itemId);
        if (item is not null)
        {
            return;
        }
        item = new CatalogItem
        {
            Id = message.itemId,
            Name = message.Name,
            Description = message.Description
        };
        await _repository.CreateAsync(item);
    }
}