using AuditLite;
using AuditLiteService.Interfaces;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService(IAuditRepositoryFactory factory, ILogger<AuditLoggerService> logger) : AuditLogger.AuditLoggerBase
{
    public override async Task<AuditResponse> LogEvent(AuditEventList request, ServerCallContext context)
    {
        var repository = factory.CreateRepository();
        try
        {
            await repository.SaveAsync(request);
            
            logger.LogInformation("Successfully stored {EventCount} audit event(s) in database.", request.AuditEvents.Count);

            if (!logger.IsEnabled(LogLevel.Debug))
                return new AuditResponse
                {
                    Success = true,
                    Message = $"Audit events successfully stored in database - Event count: {request.AuditEvents.Count}"
                };
            foreach (var auditEvent in request.AuditEvents)
            {
                logger.LogDebug("Audit Event Details:\n{EventDetails}", FormatAuditEvent(auditEvent));
            }

            return new AuditResponse 
            { 
                Success = true,
                Message = $"Audit events successfully stored in database - Event count: {request.AuditEvents.Count}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to store {EventCount} audit events. Error details: {ErrorMessage}", request.AuditEvents.Count, ex.Message);
            
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
    
    private static string FormatAuditEvent(AuditEvent auditEvent)
    {
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine($"Event Type: {auditEvent.EventType}");
        
        if (auditEvent.EventEnvironment != null)
        {
            sb.AppendLine("Event Environment:");
            var excludedProperties = new HashSet<string> { "Parser", "Descriptor" };

            foreach (var property in auditEvent.EventEnvironment.GetType().GetProperties())
            {
                if (excludedProperties.Contains(property.Name))
                    continue;

                var propertyName = property.Name;
                var propertyValue = property.GetValue(auditEvent.EventEnvironment)?.ToString() ?? "null";
                sb.AppendLine($"  {propertyName}: {propertyValue}");
            }
        }
        
        if (auditEvent.EventDate != null)
        {
            sb.AppendLine($"Event Date: {auditEvent.EventDate.ToDateTime():yyyy-MM-dd HH:mm:ss}");
        }
        
        if (auditEvent.CustomFields.Count > 0)
        {
            sb.AppendLine("Custom Fields:");
            foreach (var field in auditEvent.CustomFields)
            {
                sb.AppendLine($"  {field.Key}: {field.Value}");
            }
        }

        return sb.ToString();
    }
}