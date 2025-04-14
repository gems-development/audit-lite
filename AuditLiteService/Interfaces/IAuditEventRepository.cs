using AuditLite;

namespace AuditLiteService;

public interface IAuditEventRepository
{
    Task SaveAsync(AuditEventList events);
}