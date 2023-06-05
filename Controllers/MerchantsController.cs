using System.Text.Json;
using food_delivery.Models;
using food_delivery.Producers;
using food_delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace food_delivery.Controllers;

[ApiController]
[Route("api/merchants")]
public class MerchantsController : ControllerBase
{

    private readonly ILogger<MerchantsController> _logger;

    private readonly IMerchantsService _merchantsService;

    private readonly CustomerProducer _producer;
    private readonly string _topic;

    private readonly StripeProductsService _stripeProductsService;

    public MerchantsController(
        IMerchantsService merchantsService,
        CustomerProducer producer,
        IOptions<KafkaSettings> kafkaSettings,
        StripeProductsService stripeProductsService,
        ILogger<MerchantsController> logger
    )
    {
        _merchantsService = merchantsService;
        _producer = producer;
        _topic = kafkaSettings.Value.OrdersTopic;
        _stripeProductsService = stripeProductsService;
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
    public async Task<IActionResult> FullfillOrder([FromRoute] string merchantId, Order newOrder)
    {
        // await _merchantsService.CreateOrderAsync(newOrder);
        string message = JsonSerializer.Serialize<Order>(newOrder);
        await _producer.SendOrderRequest(_topic, message);

        // return CreatedAtAction(nameof(Get), new { id = newOrder.Id }, newOrder);
        return Ok();
    }


    [HttpPost]
    [Route("webhook")]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Console.WriteLine(json);

        try
        {
            string? signature = Request.Headers["Stripe-Signature"];
            if (string.IsNullOrEmpty(signature)) throw new Exception("signature not found");
            var stripeEvent = _stripeProductsService.GetEvent(json, signature);

            // Handle the checkout.session.completed event
            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var options = new SessionGetOptions();
                // options.AddExpand("line_items");
                var service = new SessionService();
                // Retrieve the session. If you require line items in the response, you may include them by expanding line_items.
                Session sessionWithLineItems = service.Get(session.Id, options);
                // StripeList<LineItem> lineItems = sessionWithLineItems.LineItems;

                // Fulfill the purchase...
                // this.FulfillOrder(lineItems);
                string merchantId = sessionWithLineItems.Metadata["merchantId"];
                string jsonCart = sessionWithLineItems.Metadata["cart"];
                if(string.IsNullOrEmpty(jsonCart)) throw new Exception();
                Order? deserializedOrder = JsonSerializer.Deserialize<Order>(jsonCart);
                if (deserializedOrder == null) throw new Exception("unable to deserialize cart");

                return await FullfillOrder(merchantId, deserializedOrder);
            }

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }

}
