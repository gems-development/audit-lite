using AuditLite;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace AuditLiteLib.Extensions;

public static class AuditEventExtensions
{
    public static AuditEvent FillFromDefaults(this AuditEvent auditEvent, string eventType, 
        Dictionary<string, string>? customFields = null)
    {
        auditEvent.EventType = eventType;
        auditEvent.EventEnvironment = new EventEnvironment().FillFromCurrentEnvironment();
        auditEvent.EventDate = Timestamp.FromDateTime(DateTime.UtcNow);
        if (customFields != null)
            auditEvent.CustomFields.Add(customFields);
        return auditEvent;
    }
}