namespace food_delivery.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Dish
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string? Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Summary { get; set; }
}
