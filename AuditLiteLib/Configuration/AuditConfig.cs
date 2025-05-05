namespace AuditLiteLib.Configuration;

public class AuditConfig
{
    public required string ServerUrl { get; set; }
    public long FlushIntervalMilliseconds { get; set; }
    public int MaxBufferSize { get; set; }
    public int MaxChunkedRetries { get; set; }

    public override string ToString()
    {
        return $"ServerUrl: {ServerUrl}, FlushInterval: {FlushIntervalMilliseconds}ms, " +
               $"MaxBufferSize: {MaxBufferSize}, MaxChunkedRetries: {MaxChunkedRetries}";
    }
}