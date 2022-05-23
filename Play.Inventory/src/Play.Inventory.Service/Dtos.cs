using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record IventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);
}