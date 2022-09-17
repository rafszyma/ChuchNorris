using Business.Interfaces;
using Business.Persistence;
using Business.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RestSharp;
using Newtonsoft.Json;

namespace Harvester;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IMongoDbContext _dbContext;

    private readonly IOptions<WorkerSettings> _settings;

    public Worker(ILogger<Worker> logger, IMongoDbContext dbContext, IOptions<WorkerSettings> settings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);


            var jokes = await GetJokes(stoppingToken);
            
            var quotes = jokes.Where(x => x.IsValid()).Select(joke => new Quote(joke.Id, joke.Value, 0)).Distinct().ToArray();
            quotes = await FilterExisting(quotes, stoppingToken);

            if (quotes.Any())
            {
                
                await _dbContext.GetQuoteCollection().InsertManyAsync(quotes,
                    cancellationToken: stoppingToken);
            }

            _logger.LogInformation("Added {Items}  new items at: {Time}",quotes.Length, DateTimeOffset.Now);

            await Task.Delay(_settings.Value.HarvestIntervalInSeconds * 1000, stoppingToken);
        }
    }

    private async Task<Quote[]> FilterExisting(Quote[] quotes, CancellationToken stoppingToken)
    {
        var hashes = quotes.Select(x => x.Key);
        var collection = _dbContext.GetQuoteCollection();
        var existing = await collection.Find(x => hashes.Contains(x.Key)).ToListAsync(cancellationToken: stoppingToken);
        quotes = quotes.Except(existing, new QuoteHashComparer()).ToArray();

        return quotes.ToArray();
    }

    private async Task<List<Joke>> GetJokes(CancellationToken stoppingToken)
    {
        var jokes = new List<Joke>();
        for (var i = 0; i < _settings.Value.BatchSize; i++)
        {
            try
            {
                var client =
                    new RestClient("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("accept", "application/json");
                request.AddHeader("X-RapidAPI-Key", _settings.Value.ApiKey);
                request.AddHeader("X-RapidAPI-Host", _settings.Value.ApiHost);
                var response = await client.ExecuteAsync(request, stoppingToken);
                if (!response.IsSuccessful || response.Content == null)
                {
                    continue;
                }

                var joke = JsonConvert.DeserializeObject<Joke>(response.Content);
                if (joke != null)
                {
                    jokes.Add(joke);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while harvesting joke");
            }
        }

        return jokes;
    }
}