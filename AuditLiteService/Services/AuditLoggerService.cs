using AuditLite;
using AuditLiteService.Data;
using AuditLiteService.Extensions;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService : AuditLogger.AuditLoggerBase
{
    private readonly IAuditEventRepository _repository;

    public AuditLoggerService(IAuditEventRepository repository)
    {
        _repository = repository;
    }
    
    public override async Task<AuditResponse> LogEvent(AuditEventList request, ServerCallContext context)
    {
        try
        {
            await _repository.SaveAsync(request);
            
            return new AuditResponse 
            { 
                Success = true,
                Message = $"Audit events successfully stored in database - Event count: {request.AuditEvents.Count}"
            };
        }
        catch (Exception ex)
        {
            return new AuditResponse
            {
                Success = false,
                Message = $"Failed to store {request.AuditEvents.Count} audit events. Error details: {ex.Message}"
            };
        }
    }
}