using AuditLite;
using AuditLiteService.Data;
using AuditLiteService.Extensions;
using AuditLiteService.Models;

namespace AuditLiteService;

public class PostgresAuditEventRepository: IAuditEventRepository
{
    private readonly AuditDbContext _context;

    public PostgresAuditEventRepository(AuditDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveAsync(AuditEventList events)
    {
        var entities = events.AuditEvents.Select(auditEvent => auditEvent.ToEntity()).ToList();
        _context.AuditEvents.AddRange(entities);
        await _context.SaveChangesAsync();
    }
}