namespace AuditLiteLib.Configuration;

public class AuditConfigBuilder
{
    private string _serverUrl = string.Empty;
    private long _flushIntervalMilliseconds = 10000;
    private int _maxBufferSize = 1000;
    private int _maxChunkedRetries = 3;
    
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
    
    public AuditConfigBuilder SetMaxChunkedRetries(int chunkedRetries)
    {
        _maxChunkedRetries = chunkedRetries;
        return this;
    }
    
    public AuditConfig Build()
    {
        var config = new AuditConfig
        {
            ServerUrl = _serverUrl,
            FlushIntervalMilliseconds = _flushIntervalMilliseconds,
            MaxBufferSize = _maxBufferSize,
            MaxChunkedRetries = _maxChunkedRetries
        };

        var validator = new AuditConfigValidator();
        validator.Validate(config);

        return config;
    }
}