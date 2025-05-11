using AuditLite;
using AuditLiteService.Data;
using AuditLiteService.Extensions;
using AuditLiteService.Interfaces;

namespace AuditLiteService.Repositories;

public class PostgresAuditEventRepository(AuditDbContext context) : IAuditEventRepository
{
    public async Task SaveAsync(AuditEventList events)
    {
        var entities = events.AuditEvents.Select(auditEvent => auditEvent.ToEntity()).ToList();
        context.AuditEvents.AddRange(entities);
        await context.SaveChangesAsync();
    }
}