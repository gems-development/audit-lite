using AuditLite;
using AuditLiteService.Interfaces;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService(IAuditRepositoryFactory factory) : AuditLogger.AuditLoggerBase
{
    public override async Task<AuditResponse> LogEvent(AuditEventList request, ServerCallContext context)
    {
        var repository = factory.CreateRepository();
        try
        {
            await repository.SaveAsync(request);
            
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
    
    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse());
    }
}