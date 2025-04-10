using AuditLite;
using AuditLiteService.Models;

namespace AuditLiteService.Extensions;

public static class AuditEventExtensions
{
    public static AuditEventEntity ToEntity(this AuditEvent request)
    {
        return new AuditEventEntity
        {
            EventType = request.EventType,
            EventDate = request.EventDate.ToDateTime(),
            EventEnvironmentEntity = new EventEnvironmentEntity
            {
                UserName = request.EventEnvironment.UserName,
                MethodName = request.EventEnvironment.MethodName,
                MachineName = request.EventEnvironment.MachineName,
                IpAddress = request.EventEnvironment.IpAddress
            },
            CustomFields = request.CustomFields
                .Select(kvp => new CustomFieldEntity { Key = kvp.Key, Value = kvp.Value })
                .ToList()
        };
    }
}