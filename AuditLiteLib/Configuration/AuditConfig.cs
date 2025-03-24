namespace AuditLiteLib.Configuration;

public class AuditConfig
{
    public required string PredefinedConfigName { get; set;}
    public int FlushIntervalMilliseconds { get; set; }
    public int MaxBufferSize { get; set; }
    public required string ServerUrl { get; set; }
    public bool EnableEncryption { get; set; }
    
    public bool EnableProtobufSerialization { get; set; }

    public override string ToString()
    {
        return $"Predefined Config: {PredefinedConfigName}, FlushInterval: {FlushIntervalMilliseconds}ms, MaxBufferSize: {MaxBufferSize}," +
               $" ServerUrl: {ServerUrl}, Encryption: {EnableEncryption}, ProtobufSerialization: {EnableProtobufSerialization}";
    }
}