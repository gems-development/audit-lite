namespace AuditLiteLib.Configuration;

public static class AuditPredefinedConfigs
{
    public static AuditConfig Basic()
    {
        return new AuditConfigBuilder()
            .SetPredefinedConfigName("Basic")
            .SetServerUrl("http://localhost:5001/audit")
            .SetFlushIntervalMilliseconds(50000)
            .SetMaxBufferSize(100)
            .SetEnableProtobufSerialization(false)
            .Build();
    }

    public static AuditConfig Optimized()
    {
        return new AuditConfigBuilder()
            .SetPredefinedConfigName("Optimized")
            .SetServerUrl("http://audit-server.example.com")
            .SetFlushIntervalMilliseconds(600000)
            .SetMaxBufferSize(1000)
            .SetEnableProtobufSerialization(true)
            .Build();
    }

    public static AuditConfig Medical()
    {
        return new AuditConfigBuilder()
            .SetPredefinedConfigName("Medical")
            .SetServerUrl("http://medical-audit.example.com")
            .SetFlushIntervalMilliseconds(60000)
            .SetMaxBufferSize(500)
            .SetEnableProtobufSerialization(true)
            .Build();
    }
}