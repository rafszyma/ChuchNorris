using Business.Persistence;
using MongoDB.Driver;

namespace Business.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<Quote> GetQuoteCollection();
    }
}