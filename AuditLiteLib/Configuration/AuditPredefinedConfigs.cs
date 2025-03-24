namespace AuditLiteLib.Configuration;

public static class AuditPredefinedConfigs
{
    public static AuditConfig Basic()
    {
        return new AuditConfig
        {
            PredefinedConfigName = "Basic",
            FlushIntervalMilliseconds = 50000,
            MaxBufferSize = 100,
            ServerUrl = "http://localhost:5000/audit",
            EnableEncryption = false,
            EnableProtobufSerialization = false,
        };
    }
    
    public static AuditConfig Optimized()
    {
        return new AuditConfig
        {
            PredefinedConfigName = "Optimized",
            FlushIntervalMilliseconds = 600000,
            MaxBufferSize = 1000,
            ServerUrl = "http://audit-server.example.com",
            EnableEncryption = true,
            EnableProtobufSerialization = true,
        };
    }

    // Пример узконаправленной конфигурации для медицинских систем
    public static AuditConfig Medical()
    {
        return new AuditConfig
        {
            PredefinedConfigName = "Medical",
            FlushIntervalMilliseconds = 60000,
            MaxBufferSize = 500,
            ServerUrl = "http://medical-audit.example.com",
            EnableEncryption = true,
            EnableProtobufSerialization = true,
        };
    }

    // Настраиваемая, пользовательская конфигурация
    public static AuditConfig Custom(
        int flushIntervalMilliseconds = 50000,
        int maxBufferSize = 100,
        string serverUrl = "http://localhost:5000/audit",
        bool enableEncryption = false,
        bool enableProtobufSerialization = false)
    {
        return new AuditConfig
        {
            PredefinedConfigName = "Custom",
            FlushIntervalMilliseconds = flushIntervalMilliseconds,
            MaxBufferSize = maxBufferSize,
            ServerUrl = serverUrl,
            EnableEncryption = enableEncryption,
            EnableProtobufSerialization = enableProtobufSerialization
        };
    }
}