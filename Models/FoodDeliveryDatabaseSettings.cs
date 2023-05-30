namespace food_delivery.Models;

public class FoodDeliveryDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string MerchantsCollectionName { get; set; } = null!;
}