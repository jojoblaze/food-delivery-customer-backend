using System.Text.Json;
using food_delivery.Models;
using food_delivery.Producers;
using food_delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace food_delivery.Controllers;

[ApiController]
[Route("api/merchants")]
public class MerchantsController : ControllerBase
{

    private readonly ILogger<MerchantsController> _logger;

    private readonly IMerchantsService _merchantsService;

    private readonly CustomerProducer _producer;
    private readonly string _topic;

    public MerchantsController(IMerchantsService merchantsService, CustomerProducer producer, IOptions<KafkaSettings> kafkaSettings, ILogger<MerchantsController> logger)
    {
        _merchantsService = merchantsService;
        _producer = producer;
        _topic = kafkaSettings.Value.OrdersTopic;
        _logger = logger;
    }

    [HttpGet]
    public async Task<List<Merchant>> Get()
    {
        return await _merchantsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Merchant>> Get(string id)
    {
        var merchant = await _merchantsService.GetAsync(id);

        if (merchant is null)
        {
            return NotFound();
        }

        return merchant;
    }

    [HttpPost]
    [Route("{merchantId:length(24)}/orders")]
    public async Task<IActionResult> Post([FromRoute] string merchantId, Order newOrder)
    {
        // await _merchantsService.CreateOrderAsync(newOrder);
        string message = JsonSerializer.Serialize<Order>(newOrder);
        await _producer.SendOrderRequest(_topic, message);

        // return CreatedAtAction(nameof(Get), new { id = newOrder.Id }, newOrder);
        return Ok();
    }

    // [HttpPut("{id:length(24)}")]
    // public async Task<IActionResult> Update(string id, Merchant updatedMerchant)
    // {
    //     var book = await _merchantsService.GetAsync(id);

    //     if (book is null)
    //     {
    //         return NotFound();
    //     }

    //     updatedMerchant.Id = book.Id;

    //     await _merchantsService.UpdateAsync(id, updatedMerchant);

    //     return NoContent();
    // }

    // [HttpDelete("{id:length(24)}")]
    // public async Task<IActionResult> Delete(string id)
    // {
    //     var book = await _merchantsService.GetAsync(id);

    //     if (book is null)
    //     {
    //         return NotFound();
    //     }

    //     await _merchantsService.RemoveAsync(id);

    //     return NoContent();
    // }
}
