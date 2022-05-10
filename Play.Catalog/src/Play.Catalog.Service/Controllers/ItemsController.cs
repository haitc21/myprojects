using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Servic.Repositories;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Servic.Entities;

namespace Play.Catalog.Service.Controllers;
[ApiController]
[Route("Items")]
public class ItemsController : ControllerBase
{
    private readonly ItemsRepository _itemsRepository = new();
    private static List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(),"Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"AntiDote", "Cures poisn", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(),"Bronze sword", "Restores a small amount of damage", 20, DateTimeOffset.UtcNow)
        };

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await _itemsRepository.GetAllAsync())
                  .Select(x => x.AsDto());
        return items;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var result = await _itemsRepository.GetAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return result.AsDto();
    }
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),

            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
        await _itemsRepository.CreateAsync(item);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAAsync(Guid id, UpdateIItemDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }
        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;
        await _itemsRepository.UpdateAsync(existingItem);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }
        await _itemsRepository.RemoveAsync(id);
        return NoContent();
    }
}