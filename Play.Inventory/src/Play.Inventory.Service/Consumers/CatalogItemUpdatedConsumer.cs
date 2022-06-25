using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Consumers;
public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{
    private readonly IRepository<CatalogItem> _repository;
    public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }
    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;
        var item = await _repository.GetAsync(message.itemId);
        if (item is null)
        {
            item = new CatalogItem
            {
                Id = message.itemId,
                Name = message.Name,
                Description = message.Description
            };
            await _repository.CreateAsync(item);
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;
            await _repository.UpdateAsync(item);

        }

    }
}