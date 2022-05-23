using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Services
{
    public static class Extensions
    {
        public static IventoryItemDto AsDto(InventoryItem item)
        {
            return new IventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}
