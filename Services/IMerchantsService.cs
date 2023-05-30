
using food_delivery.Models;

namespace food_delivery.Services;

public interface IMerchantsService
{
    Task<List<Merchant>> GetAsync();
    Task<Merchant?> GetAsync(string id);
}