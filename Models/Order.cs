namespace food_delivery.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Order
{
    // [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    // public string? Id { get; set; }

    public PaymentInfos? PaymentInfos { get; set; }

    public List<ActiveOrderEntry> Dishes { get; set; }

    public DeliveryInfos DeliveryInfos { get; set; }
}
