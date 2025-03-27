using ProtoBuf;
using ProtoBuf.WellKnownTypes;

namespace AuditLiteLib;

[ProtoContract]
public class AuditEvent
{
    [ProtoMember(1)]
    public string EventType { get; set; }
    [ProtoMember(2)]
    public EventEnvironment EventEnvironment { get; set; }
    [ProtoMember(3)]
    public Timestamp EventDate { get; set; }
    [ProtoMember(4)]
    public Dictionary<string, string>? CustomFields { get; set; } 
    public AuditEvent(string eventType, Dictionary<string, string>? customFields = null)
    {
        EventType = eventType;
        CustomFields = customFields;
        EventEnvironment = new EventEnvironment();
        EventDate = new Timestamp(DateTime.UtcNow);
    }
}