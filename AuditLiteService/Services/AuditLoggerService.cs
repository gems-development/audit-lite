using AuditLite;
using AuditLiteService.Data;
using AuditLiteService.Extensions;
using Grpc.Core;

namespace AuditLiteService.Services;

public class AuditLoggerService : AuditLogger.AuditLoggerBase
{
    private readonly IAuditRepositoryFactory _factory;

    public AuditLoggerService(IAuditRepositoryFactory factory)
    {
        _factory = factory;
    }
    
    public override async Task<AuditResponse> LogEvent(AuditEventList request, ServerCallContext context)
    {
        var repository = _factory.CreateRepository();
        try
        {
            await repository.SaveAsync(request);
            return new AuditResponse { Success = true, Message = $"События успешно сохранены в базу данных. " +
                                                                 $"Кол-во событий:{request.AuditEvents.Count()}" };
        }
        catch (Exception ex)
        {
            return new AuditResponse
            {
                Success = false,
                Message = $"Ошибка при сохранении событий: {ex.Message}"
            };
        }
        
    }
    
}