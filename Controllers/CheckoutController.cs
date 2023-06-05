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
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{

    private readonly ILogger<MerchantsController> _logger;

    private readonly IMerchantsService _merchantsService;

    private readonly CustomerProducer _producer;
    private readonly string _topic;

    private readonly StripeSettings _stripeSettings;

    private readonly StripeProductsService _stripeProductsService;


    public CheckoutController(
        IMerchantsService merchantsService,
        CustomerProducer producer,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<StripeSettings> stripeSettings,
        StripeProductsService stripeProductsService,
        ILogger<MerchantsController> logger
    )
    {
        _merchantsService = merchantsService;
        _producer = producer;
        _topic = kafkaSettings.Value.OrdersTopic;
        _stripeProductsService = stripeProductsService;
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
    }


    [HttpPost]
    [Route("{merchantId:length(24)}/orders")]
    public async Task<IActionResult> Post([FromRoute] string merchantId, Order newOrder)
    {
        var domain = "http://localhost:3001";
        SessionCreateOptions options = new SessionCreateOptions();
        options.LineItems = new List<SessionLineItemOptions>(); ;
        options.Mode = "payment";
        options.SuccessUrl = domain + "/payment_succeeded";
        options.CancelUrl = domain + "/payment_failed";

        string cart = JsonSerializer.Serialize<Order>(newOrder);
        options.Metadata = new Dictionary<string, string>();
        options.Metadata.Add("merchantId", merchantId);
        options.Metadata.Add("cart", cart);

        foreach (ActiveOrderEntry orderEntry in newOrder.Dishes)
        {
            SessionLineItemOptions newItem = new SessionLineItemOptions
            {
                Price = _stripeProductsService.GetProductPriceId(merchantId, orderEntry.DishId),
                Quantity = orderEntry.Quantity,
            };
            options.LineItems.Add(newItem);
        }


        var service = new SessionService();
        Session session = service.Create(options);



        // TODO: SAVE request id (or idempotency_key?) with serialized order to get it back on order fullfillment
        // Save(session.Id, message)

        // Response.Headers.Add("Location", session.Url);
        // return new StatusCodeResult(303);
        return Ok(session.Url);
    }

}
