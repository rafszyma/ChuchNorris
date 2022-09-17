using Business.Persistence;
using Business.Settings;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var config = builder.Build();

var settings = new DbSettings();
config.Bind(settings);

var client = new MongoClient(settings.ConnectionString);

var db = client.GetDatabase(settings.DatabaseName);
var collection = db.GetCollection<Quote>(settings.QuotesCollectionName);


var indexKeysDefinition = Builders<Quote>.IndexKeys.Text(x => x.Key);
await collection.Indexes.CreateOneAsync(new CreateIndexModel<Quote>(indexKeysDefinition, new CreateIndexOptions
{
    Unique = true
}));

Console.WriteLine("Finished");