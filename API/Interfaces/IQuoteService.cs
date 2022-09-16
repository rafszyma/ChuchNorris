using API.Persistence;
using Business.Persistence;

namespace API.Interfaces;

public interface IQuoteService
{
    public Task<List<Quote>> ListQuotes(int? skip = null, int? take = null);

    public Task VoteForQuote(Guid quoteId);

    public Task<List<Quote>> ListTop(int take = 0);

    public Task ResetVotes(Guid quoteId);
}