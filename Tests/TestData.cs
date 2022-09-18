using System.Threading.Tasks;
using Business.Persistence;
using MongoDB.Driver;

namespace Tests;

public static class TestData
{
    // TODO this should be all parametrized and read from config and initialized only once using DI
    public static async Task ClearDb()
    {
        var client = new MongoClient("mongodb://localhost:27017");

        var db = client.GetDatabase("chuckdb");

        var collection = db.GetCollection<Quote>("Quotes");

        await collection.DeleteManyAsync(x => true);
    }
    public static async Task InsertRecords(params Quote[] quotes)
    {
        var client = new MongoClient("mongodb://localhost:27017");

        var db = client.GetDatabase("chuckdb");

        var collection = db.GetCollection<Quote>("Quotes");

        await collection.InsertManyAsync(quotes);
    }
}