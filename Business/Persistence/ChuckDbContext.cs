using API.Settings;
using Business.Interfaces;
using Business.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Persistence;

public class ChuckDbContext : IMongoDbContext
{
    private IMongoDatabase Db { get; set; }

    private MongoClient Client { get; set; }

    private IOptions<DbSettings> _settings;

    public ChuckDbContext(IOptions<DbSettings> settings)
    {
        _settings = settings;
        Client = new MongoClient(settings.Value.ConnectionString);
        Db = Client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Quote> GetQuoteCollection()
    {
        return Db.GetCollection<Quote>(_settings.Value.QuotesCollectionName);
    }
}