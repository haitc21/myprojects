using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Play.Inventory.Services;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _inventoryRepository;
    public ItemsController(IRepository<InventoryItem> inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IventoryItemDto>>> GetAsync([FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }
        var item = (await _inventoryRepository.GetAllAsync(x => x.UserId == userId))
                    .Select(x => x.AsDto());
        return Ok(item);
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
            inventoryitem.Quantity = grantItemsDto.Quantity;
            await _inventoryRepository.UpdateAsync(inventoryitem);
        }
        return Ok();
    }
}
