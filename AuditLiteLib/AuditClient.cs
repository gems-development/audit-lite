
using AuditLite;
using Grpc.Core;
using Grpc.Net.Client;

namespace AuditLiteLib;

public class AuditClient
{
    private readonly AuditLogger.AuditLoggerClient _client;

    public AuditClient(string serverAddress)
    {
        var channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });
        _client = new AuditLogger.AuditLoggerClient(channel);
    }

    public async Task<AuditResponse> SendEventAsync(AuditEventList auditEvent)
    {
        var response = await _client.LogEventAsync(auditEvent);
        return response;
    }
}