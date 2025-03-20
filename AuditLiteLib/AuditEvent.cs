namespace AuditLiteLib;

public class AuditEvent
{
    private string EventType { get; set; }
    private EventEnvironment EventEnvironment { get; set; }
    private DateTime EventDate { get; set; }
    private Dictionary<string, object>? OptionalFields { get; set; } 
    public AuditEvent(string eventType, EventEnvironment eventEnvironment, Dictionary<string, object>? optionalFields = null)
    {
        EventType = eventType;
        EventEnvironment = eventEnvironment;
        OptionalFields = optionalFields;
        EventDate = DateTime.Now;
    }
}