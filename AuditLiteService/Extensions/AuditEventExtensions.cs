using AuditLite;
using AuditLiteService.Models.Elasticsearch;
using AuditLiteService.Models.Mongo;
using AuditLiteService.Models.Postgres;

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
            MongoEventEnvironmentEntity = new MongoEventEnvironmentEntity
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
    
    public static ElasticsearchAuditEventEntity ToElasticsearchEntity(this AuditEvent request)
    {
        return new ElasticsearchAuditEventEntity
        {
            EventType = request.EventType,
            EventDate = request.EventDate.ToDateTime(),
            ElasticsearchAuditEvent = new ElasticsearchEnvironmentEntity
            {
                UserName = request.EventEnvironment.UserName,
                MethodName = request.EventEnvironment.MethodName,
                MachineName = request.EventEnvironment.MachineName,
                IpAddress = request.EventEnvironment.IpAddress
            },
            CustomFields = request.CustomFields.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }
}