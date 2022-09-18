using Bogus;
using Business.Persistence;

namespace Tests;

public sealed class QuoteFaker : Faker<Quote>
{
    public QuoteFaker()
    {
        CustomInstantiator(x => new Quote(x.Lorem.Sentence(), x.Lorem.Sentences(5), x.Random.Number(1, 1000)));
    }
}