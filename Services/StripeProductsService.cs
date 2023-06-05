
using food_delivery.Models;
using Microsoft.Extensions.Options;
using Stripe;

namespace food_delivery.Services;

public class StripeProductsService
{
    readonly StripeSettings _stripeSettings;
    public StripeProductsService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
    }

    public Product GetProduct(string merchantId, string dishId)
    {
        var service = new ProductService();
        Product product = service.Get($"product_{merchantId}_{dishId}");
        return product;
    }

    public string GetProductPriceId(string merchantId, string dishId)
    {
        Product product = GetProduct(merchantId, dishId);
        return product.DefaultPriceId;
    }

    public Event GetEvent(string jsonResponse, string signature)
    {
        var stripeEvent = EventUtility.ConstructEvent(
            jsonResponse,
            signature,
            _stripeSettings.WebhookSecret
            );
        return stripeEvent;
    }
}