using API.Persistence;
using Business.Interfaces;
using Business.Persistence;
using Harvester.Settings;
using Microsoft.Extensions.Options;
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

            var jokes = new List<Joke>();
            for (var i = 0; i < _settings.Value.BatchSize; i++)
            {
                var client = new RestClient("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("accept", "application/json");
                request.AddHeader("X-RapidAPI-Key", _settings.Value.ApiKey);
                request.AddHeader("X-RapidAPI-Host", _settings.Value.ApiHost);
                var response = await client.ExecuteAsync(request, stoppingToken);

                // todo handle it instead of surpress
                jokes.Add(JsonConvert.DeserializeObject<Joke>(response.Content!)!);

            }

            var quotes = jokes.Where(x => x.IsValid()).Select(joke => new Quote(joke.Id, joke.Value, 0));
            var collection = _dbContext.GetQuoteCollection();
            if (quotes.Any())
            {
                await collection.InsertManyAsync(quotes,
                    cancellationToken: stoppingToken);
            }


            _logger.LogInformation("Added {Items}  new items at: {Time}",quotes.Count(), DateTimeOffset.Now);

            await Task.Delay(_settings.Value.HarvestIntervalInSeconds * 1000, stoppingToken);
        }
    }
}