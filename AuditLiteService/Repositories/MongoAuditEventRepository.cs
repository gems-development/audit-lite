using AuditLite;
using AuditLiteService.Extensions;
using AuditLiteService.Models.Mongo;
using MongoDB.Driver;

namespace AuditLiteService.Repositories;

public class MongoAuditEventRepository : IAuditEventRepository
{
    private readonly IMongoCollection<MongoAuditEventEntity> _collection;

    public MongoAuditEventRepository(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDB"));
        var database = client.GetDatabase("AuditDb");
        _collection = database.GetCollection<MongoAuditEventEntity>("AuditEvents");
    }

    public async Task SaveAsync(AuditEventList events)
    {
        var entities = events.AuditEvents.Select(auditEvent => 
            auditEvent.ToMongoEntity()).ToList();
        if (entities.Any())
        {
            await _collection.InsertManyAsync(entities);
        }
    }
}