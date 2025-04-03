using AuditLiteService.Data;
using AuditLiteService.Models;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService : AuditLogger.AuditLoggerBase
{
    private readonly AuditDbContext _context;

    public AuditLoggerService(AuditDbContext context)
    {
        _context = context;
    }

    public override async Task<AuditResponse> LogEvent(AuditEvent request, ServerCallContext context)
    {
        try
        {
            var auditEvent = new AuditEventEntity()
            {
                EventType = request.EventType,
                EventDate = request.EventDate.ToDateTime(),
                EventEnvironmentEntity = new EventEnvironmentEntity
                {
                    UserName = request.EventEnvironment.UserName,
                    MethodName = request.EventEnvironment.MethodName,
                    MachineName = request.EventEnvironment.MachineName,
                    IpAddress = request.EventEnvironment.IpAddress
                },
                CustomFields = request.CustomFields
                    .Select(kvp => new CustomFieldEntity { Key = kvp.Key, Value = kvp.Value })
                    .ToList()
            };

            _context.AuditEvents.Add(auditEvent);
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