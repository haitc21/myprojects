using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common.Repositories;

namespace Play.Catalog.Service.Controllers;
[ApiController]
[Route("Items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemsRepository;
    private int requestCounter = 0;
    public ItemsController(IRepository<Item> itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        requestCounter++;
        Console.WriteLine($"Request {requestCounter} strating...");
        if (requestCounter <= 2)
        {
            Console.WriteLine($"Request {requestCounter} delaying...");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        if (requestCounter <= 4)
        {
            Console.WriteLine($"Request {requestCounter} 500 errpr");
            return StatusCode(500);
        }
        var items = (await _itemsRepository.GetAllAsync())
                  .Select(x => x.AsDto());
        Console.WriteLine($"Request {requestCounter} Ok");
        return Ok(items);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        string idS = id.ToString();
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
            CreatedDate = DateTimeOffset.Now
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