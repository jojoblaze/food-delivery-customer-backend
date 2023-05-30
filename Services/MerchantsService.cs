namespace food_delivery.Services;

using food_delivery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


public class MerchantsService : IMerchantsService
{
    private readonly IMongoCollection<Merchant> _merchantsCollection;

    public MerchantsService(IOptions<FoodDeliveryDatabaseSettings> foodDeliveryDatabaseSettings)
    {
        var mongoClient = new MongoClient(foodDeliveryDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(foodDeliveryDatabaseSettings.Value.DatabaseName);
        _merchantsCollection = mongoDatabase.GetCollection<Merchant>(foodDeliveryDatabaseSettings.Value.MerchantsCollectionName);
    }

    public async Task<List<Merchant>> GetAsync() =>
            await _merchantsCollection.Find(_ => true).ToListAsync();

    public async Task<Merchant?> GetAsync(string id) =>
        await _merchantsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // public async Task CreateOrderAsync(Order newOrder) =>
    //     await _merchantsCollection.InsertOneAsync(newOrder);

    // public async Task UpdateAsync(string id, Merchant updatedBook) =>
    //     await _merchantsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    // public async Task RemoveAsync(string id) =>
    //     await _merchantsCollection.DeleteOneAsync(x => x.Id == id);

}
