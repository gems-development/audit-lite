namespace AuditLiteService.Models.Mongo;

using MongoDB.Bson.Serialization.Attributes;

public class MongoCustomFieldEntity
{
    [BsonElement("key")]
    public string Key { get; set; } = null!;

    [BsonElement("value")]
    public string Value { get; set; } = null!;
}
