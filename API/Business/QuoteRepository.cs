using API.Interfaces;
using Business.Interfaces;
using Business.Persistence;
using MongoDB.Driver;

namespace API.Business;


public class QuoteRepository : IQuoteRepository
{
    private readonly IMongoCollection<Quote> _collection;

    public QuoteRepository(IMongoDbContext dbContext)
    {
        _collection = dbContext.GetQuoteCollection();
    }

    public async Task<List<Quote>> ListQuotes(int? skip = null, int? take = null)
    {
        var query = _collection.Find(_ => true);
        if (skip != null)
        {
            query = query.Skip(skip.Value);
        }

        if (take != null)
        {
            query = query.Limit(take.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Quote?> VoteForQuote(Guid quoteId)
    {
        var update = Builders<Quote>.Update.Inc(d => d.Score, 1);
        return await _collection.FindOneAndUpdateAsync(x => x.Id == quoteId, update);
    }

    public async Task<List<Quote>> ListTop(int take = 0)
    {
        return await _collection.Find(_ => true).SortByDescending(x => x.Score).Limit(take).ToListAsync();
    }

    public async Task<Quote?> ResetVotes(Guid quoteId)
    {
        var update = Builders<Quote>.Update.Set(d => d.Score, 0);
        return await _collection.FindOneAndUpdateAsync(x => x.Id == quoteId, update);
    }
}