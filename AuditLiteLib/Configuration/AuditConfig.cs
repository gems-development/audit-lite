namespace AuditLiteLib.Configuration;

public class AuditConfig
{
    public required string PredefinedConfigName { get; set;}
    public required string ServerUrl { get; set; }
    public int FlushIntervalMilliseconds { get; set; }
    public int MaxBufferSize { get; set; }
    public bool EnableProtobufSerialization { get; set; }

    public override string ToString()
    {
        return $"Predefined Config: {PredefinedConfigName},mServerUrl: {ServerUrl}, FlushInterval: {FlushIntervalMilliseconds}ms, " +
               $"MaxBufferSize: {MaxBufferSize}, ProtobufSerialization: {EnableProtobufSerialization}";
    }
}