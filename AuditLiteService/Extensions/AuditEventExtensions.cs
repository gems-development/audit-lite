using AuditLite;
using AuditLiteService.Models;
using AuditLiteService.Models.Mongo;

namespace AuditLiteService.Extensions;

public static class AuditEventExtensions
{
    public static PostgresAuditEventEntity ToEntity(this AuditEvent request)
    {
        return new PostgresAuditEventEntity
        {
            EventType = request.EventType,
            EventDate = request.EventDate.ToDateTime(),
            PostgresEventEnvironmentEntity = new PostgresEventEnvironmentEntity
            {
                UserName = request.EventEnvironment.UserName,
                MethodName = request.EventEnvironment.MethodName,
                MachineName = request.EventEnvironment.MachineName,
                IpAddress = request.EventEnvironment.IpAddress
            },
            CustomFields = request.CustomFields
                .Select(kvp => new PostgresCustomFieldEntity { Key = kvp.Key, Value = kvp.Value })
                .ToList()
        };
    }
    
    public static MongoAuditEventEntity ToMongoEntity(this AuditEvent request)
    {
        return new MongoAuditEventEntity
        {
            EventType = request.EventType,
            EventDate = request.EventDate.ToDateTime(),
            EventEnvironmentEntity = new MongoEventEnvironmentEntity
            {
                UserName = request.EventEnvironment.UserName,
                MethodName = request.EventEnvironment.MethodName,
                MachineName = request.EventEnvironment.MachineName,
                IpAddress = request.EventEnvironment.IpAddress
            },
            CustomFields = request.CustomFields
                .Select(kvp => new MongoCustomFieldEntity
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                })
                .ToList()
        };
    }
}