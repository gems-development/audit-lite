using AuditLiteService;
using AuditLiteService.Data;
using AuditLiteService.Interfaces;
using AuditLiteService.Models.Mongo;
using AuditLiteService.Repositories;
using AuditLiteService.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Nest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();


builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var mongoUri = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(mongoUri);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("AuditDb");
    return database.GetCollection<MongoAuditEventEntity>("AuditEvents");
});

builder.Services.AddSingleton<IElasticClient>(_ =>
{
    var settings = new ConnectionSettings(new Uri(builder.Configuration.GetConnectionString("Elasticsearch")!))
        .DefaultIndex("audit-events")
        .PrettyJson()
        .DisableDirectStreaming();

    return new ElasticClient(settings);
});

builder.Services.AddScoped<MongoAuditEventRepository>();
builder.Services.AddScoped<PostgresAuditEventRepository>();
builder.Services.AddScoped<ElasticsearchAuditEventRepository>();
builder.Services.AddScoped<IAuditRepositoryFactory, AuditRepositoryFactory>();

var app = builder.Build();
app.Urls.Add("http://localhost:5001");
app.MapGrpcService<AuditLoggerService>();
app.Run();
