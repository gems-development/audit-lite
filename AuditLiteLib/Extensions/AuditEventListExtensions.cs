using AuditLite;

namespace AuditLiteLib;

public static class AuditEventListExtensions
{
    public static AuditEventList ToAuditEventList(this IEnumerable<AuditEvent> events)
    {
        var list = new AuditEventList();
        list.AuditEvents.AddRange(events);
        return list;
    }
}