using AuditLite;

namespace AuditLiteLib.Extensions;

public static class AuditEventListExtensions
{
    public static AuditEventList ToAuditEventList(this IEnumerable<AuditEvent> events)
    {
        var list = new AuditEventList();
        list.AuditEvents.AddRange(events);
        return list;
    }
}