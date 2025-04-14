namespace AuditLiteService.Models.Mongo;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class MongoEventEnvironmentEntity
{
    [BsonElement("userName")]
    public string UserName { get; set; } = null!;

    [BsonElement("methodName")]
    public string MethodName { get; set; } = null!;

    [BsonElement("machineName")]
    public string MachineName { get; set; } = null!;

    [BsonElement("ipAddress")]
    public string IpAddress { get; set; } = null!;
}
