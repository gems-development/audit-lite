using ProtoBuf;
using ProtoBuf.WellKnownTypes;

namespace AuditLiteLib;

[ProtoContract]
public class AuditEvent(string eventType, Dictionary<string, string>? customFields = null)
{
    [ProtoMember(1)]
    public string EventType { get; set; } = eventType;

    [ProtoMember(2)]
    public EventEnvironment EventEnvironment { get; set; } = new();

    [ProtoMember(3)]
    public Timestamp EventDate { get; set; } = new(DateTime.UtcNow);

    [ProtoMember(4)]
    public Dictionary<string, string>? CustomFields { get; set; } = customFields;
}