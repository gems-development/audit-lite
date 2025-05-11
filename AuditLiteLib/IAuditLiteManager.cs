namespace AuditLiteLib;

public interface IAuditLiteManager : IAsyncDisposable, IDisposable
{
    public Task CreateAuditEventAsync(string eventType, Dictionary<string, object>? optionalFields);
}