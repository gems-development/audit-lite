using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuditLiteService.Models.Mongo;

public class MongoAuditEventEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string EventType { get; set; } = null!;
    public DateTime EventDate { get; set; }

    public MongoEventEnvironmentEntity EventEnvironmentEntity { get; set; } = null!;
    public List<MongoCustomFieldEntity> CustomFields { get; set; } = new();
}