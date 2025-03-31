namespace AuditLiteLib.Configuration;

public class AuditConfigBuilder
{
    private string _predefinedConfigName = string.Empty;
    private long _flushIntervalMilliseconds = 50000;
    private int _maxBufferSize = 100;
    private string _serverUrl = "http://localhost:5000/audit";
    private bool _enableProtobufSerialization = true;
    
    public AuditConfigBuilder SetPredefinedConfigName(string name)
    {
        _predefinedConfigName = name;
        return this;
    }
    
    public AuditConfigBuilder SetServerUrl(string url)
    {
        _serverUrl = url;
        return this;
    }

    public AuditConfigBuilder SetFlushIntervalMilliseconds(long interval)
    {
        _flushIntervalMilliseconds = interval;
        return this;
    }

    public AuditConfigBuilder SetMaxBufferSize(int size)
    {
        _maxBufferSize = size;
        return this;
    }

    public AuditConfigBuilder SetEnableProtobufSerialization(bool enable)
    {
        _enableProtobufSerialization = enable;
        return this;
    }
    
    public AuditConfig Build()
    {
        if (string.IsNullOrEmpty(_predefinedConfigName))
        {
            throw new InvalidOperationException("PredefinedConfigName is required.");
        }
        if (string.IsNullOrEmpty(_serverUrl))
        {
            throw new InvalidOperationException("ServerUrl is required.");
        }
        
        return new AuditConfig
        {
            PredefinedConfigName = _predefinedConfigName,
            ServerUrl = _serverUrl,
            FlushIntervalMilliseconds = _flushIntervalMilliseconds,
            MaxBufferSize = _maxBufferSize,
            EnableProtobufSerialization = _enableProtobufSerialization
        };
    }
}