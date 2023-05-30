namespace food_delivery.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Merchant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Name { get; set; }

    public List<Dish> Dishes { get; set; }
}
