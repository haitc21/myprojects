using Play.Catalog.Servic.Entities;

namespace Play.Catalog.Servic.Repositories;

public interface IItemsRepository
{
    Task CreateAsync(Item entity);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item> GetAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Item entity);
}
