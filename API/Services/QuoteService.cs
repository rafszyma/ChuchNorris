
using API.Settings;
using Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Services;


public class QuoteService
{
    private readonly IMongoCollection<Quote> _collection;

    public QuoteService(IOptionsMonitor<DbSettings> monitor)
    {
        
        var mongoClient = new MongoClient(monitor.CurrentValue.ConnectionString);

        

        var db = mongoClient.GetDatabase(monitor.CurrentValue.DatabaseName);

        _collection = db.GetCollection<Quote>(monitor.CurrentValue.QuotesCollectionName);
    }

    public async Task<long> GetCount()
    {
        await _collection.InsertOneAsync(new Quote
        {
            Content = "lol",
            Score = 1
        });
        return await _collection.CountDocumentsAsync(x => x.Score > 0);
    }
}