using API.Persistence;
using API.Settings;
using Business.Interfaces;
using Harvester;
using Harvester.Settings;
using Serilog;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IMongoDbContext, ChuckDbContext>();
        services.Configure<WorkerSettings>(context.Configuration.GetSection("Worker"));
        services.Configure<DbSettings>(context.Configuration.GetSection("MongoDb"));
    })
    .ConfigureLogging((context, builder) =>
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        builder.AddSerilog(logger);
    })
    .Build();


await host.RunAsync();