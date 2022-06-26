using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Play.Inventory.Services;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _inventoryRepository;
    private readonly IRepository<CatalogItem> _catalogRepository;
    private readonly CatalogClient _catalogClient;
    public ItemsController(IRepository<InventoryItem> inventoryRepository,
     IRepository<CatalogItem> catalogRepository,
     CatalogClient catalogClient)
    {
        _inventoryRepository = inventoryRepository;
        _catalogRepository = catalogRepository;
        _catalogClient = catalogClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IventoryItemDto>>> GetAsync([FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }
        // var catalogItems = await _catalogClient.GetCatalogItemsAsync(); // cách này bị phụ thuộc vào CâtlogService
        var inventoryItemEntities = await _inventoryRepository.GetAllAsync(x => x.UserId == userId);
        var itemId = inventoryItemEntities.Select(x => x.CatalogItemId);
        var catalogItemEntities = await _catalogRepository.GetAllAsync(x => itemId.Contains(x.Id));
        var iventoryItemDtos = inventoryItemEntities.Select(entity =>
        {
            // var catalogItem = catalogItems.SingleOrDefault(x => x.Id == entity.CatalogItemId);
            var catalogItem = catalogItemEntities.SingleOrDefault(x => x.Id == entity.CatalogItemId);
            return entity.AsDto(catalogItem.Name, catalogItem.Description);
        });
        return Ok(iventoryItemDtos);
    }
    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        InventoryItem inventoryitem = await _inventoryRepository.
        GetAsync(x => x.UserId == grantItemsDto.UserId && x.CatalogItemId == grantItemsDto.CatalogItemId);
        if (inventoryitem is null)
        {
            inventoryitem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.Now
            };
            await _inventoryRepository.CreateAsync(inventoryitem);
        }
        else
        {
            inventoryitem.Quantity += grantItemsDto.Quantity;
            await _inventoryRepository.UpdateAsync(inventoryitem);
        }
        return Ok();
    }
}
