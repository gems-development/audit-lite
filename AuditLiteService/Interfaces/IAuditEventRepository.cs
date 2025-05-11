using AuditLite;

namespace AuditLiteService.Interfaces;

public interface IAuditEventRepository
{
    Task SaveAsync(AuditEventList events);
}