using Auditlitelib.Protos;
using Grpc.Net.Client;

namespace AuditLiteLib;

public class AuditClient
{
    private readonly AuditLogger.AuditLoggerClient _client;

    public AuditClient(string serverAddress)
    {
        var channel = GrpcChannel.ForAddress(serverAddress);
        _client = new AuditLogger.AuditLoggerClient(channel);
    }

    public async Task<bool> SendEventAsync(AuditEvent auditEvent)
    {
        var response = await _client.LogEventAsync(auditEvent);
        return response.Success;
    }
}