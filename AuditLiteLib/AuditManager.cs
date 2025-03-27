namespace AuditLiteLib;

public class AuditManager
{
    private List<AuditEvent> AuditEvents { get; set; } = null!;

    public void CreateAuditEvent(string eventType, Dictionary<string, object>? optionalFields)
    {
        AuditEvents = new List<AuditEvent>();
        //AuditEvents.Add(new AuditEvent(eventType, new EventEnvironment(), optionalFields));
    }
}