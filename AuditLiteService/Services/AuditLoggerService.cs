using AuditLite;
using AuditLiteService.Data;
using AuditLiteService.Extensions;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService : AuditLogger.AuditLoggerBase
{
    private readonly AuditDbContext _context;

    public AuditLoggerService(AuditDbContext context)
    {
        _context = context;
    }
     
    public override async Task<AuditResponse> LogEvent(AuditEventList request, ServerCallContext context)
    {
        try
        {
            var entities = request.AuditEvents.Select(auditEvent => auditEvent.ToEntity()).ToList();
            _context.AuditEvents.AddRange(entities);
            await _context.SaveChangesAsync();
    
            return new AuditResponse { Success = true, Message = "Событие успешно записано в базу данных" };
        }
        catch (Exception ex)
        {
            return new AuditResponse
            {
                Success = false,
                Message = $"Ошибка при сохранении события: {ex.Message}"
            };
        }
        
    }
    
}