using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API;
using Business.Persistence;
using FluentAssertions;
using Tests.Infrastructure;
using Xunit;

namespace Tests;

public class ApiTests : IClassFixture<ChuckWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public ApiTests(ChuckWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Given_Role_When_GettingToken_Then_TokenIsReturned()
    {
        var response = await _client.GetStringAsync($"/user/{UserRoles.Admin}");
        response.Length.Should().BeGreaterThan(10);

        // TODO check if its valid JWT token
    }

    [Fact]
    public async Task Given_InvalidRole_When_GettingToken_Then_BadRequestIsReturned()
    {
        var response = await _client.GetAsync("/user/role");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Given_SomeQuotesInDatabase_When_ListingQuotes_Then_QuotesAreReturned()
    {
        var quotes = new QuoteFaker().Generate(15);
        await TestData.InsertRecords(quotes.ToArray());

        var response = await _client.GetFromJsonAsync<Quote[]>("/quote");
        response.Should().NotBeNull();
        response!.Length.Should().Be(15);
        response.Should().BeEquivalentTo(quotes);
    }

    [Fact]
    public async Task Given_NoQuotesInDatabase_When_ListingQuotes_Then_EmptyListIsReturned()
    {
        var response = await _client.GetFromJsonAsync<Quote[]>("/quote/top");
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_QuotesWithDifferentScore_When_GettingTopQuotes_Then_OnlyTopQuotesAreReturned()
    {
        var quotes = new QuoteFaker().Generate(15);
        await TestData.InsertRecords(quotes.ToArray());

        var topQuotes = quotes.OrderByDescending(x => x.Score).Take(10);

        var response = await _client.GetFromJsonAsync<Quote[]>("/quote/top");
        response.Should().NotBeNull();
        response!.Length.Should().Be(10);
        response.Should().BeEquivalentTo(topQuotes);
    }

    [Fact]
    public async Task Given_NoQuotesInDatabase_When_GettingTopQuotes_Then_EmptyListIsReturned()
    {
        var response = await _client.GetFromJsonAsync<Quote[]>("/quote/top");
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_ExistingId_When_VotingForQuote_Then_ScoreIsIncreased()
    {
        var quote = new QuoteFaker().Generate();
        await TestData.InsertRecords(quote);

        var response = await _client.PutAsync($"/quote/{quote.Id}/vote", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quotes = await _client.GetFromJsonAsync<Quote[]>("/quote");

        quote.Should().NotBeNull();
        var votedQuote = quotes!.Single(x => x.Id == quote.Id);

        votedQuote.Score.Should().Be(quote.Score + 1);
    }

    [Fact]
    public async Task Given_NotExistingId_When_VotingForQuote_Then_NotFoundIsReturned()
    {
        var response = await _client.PutAsync($"/quote/{Guid.NewGuid()}/vote", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Given_ExistingId_When_ResettingQuoteScore_Then_ScoreIsReset()
    {
        var quote = new Quote("key", "contexnt", 10);

        await TestData.InsertRecords(quote);

        var response = await _client.PutAsync($"/quote/{quote.Id}/reset", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quotes = await _client.GetFromJsonAsync<Quote[]>("/quote");

        quote.Should().NotBeNull();
        var votedQuote = quotes!.Single(x => x.Id == quote.Id);

        votedQuote.Score.Should().Be(0);
    }

    [Fact]
    public async Task Given_NotExistingId_When_ResettingQuoteScore_Then_NotFoundIsReturned()
    {
        var response = await _client.PutAsync($"/quote/{Guid.NewGuid()}/reset", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await TestData.ClearDb();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}