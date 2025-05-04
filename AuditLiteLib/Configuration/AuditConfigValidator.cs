namespace AuditLiteLib.Configuration;

public class AuditConfigValidator
{
    public void Validate(AuditConfig config)
    {
        if (string.IsNullOrEmpty(config.ServerUrl))
        {
            throw new InvalidOperationException("ServerUrl is required.");
        }
        if (!Uri.TryCreate(config.ServerUrl, UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("ServerUrl must be a valid URL.");
        }
        if (config.FlushIntervalMilliseconds < -1)
        {
            throw new InvalidOperationException($"Invalid 'FlushIntervalMilliseconds' value: {config.FlushIntervalMilliseconds}." +
                                                $" It must be greater or equal than -1.");
        }
        if (config.MaxBufferSize <= 0)
        {
            throw new InvalidOperationException($"Invalid 'MaxBufferSize' value: {config.MaxBufferSize}. It must be greater than 0.");
        }
        if (config.MaxChunkedRetries < 0)
        {
            throw new InvalidOperationException($"Invalid 'MaxChunkedRetries' value: {config.MaxChunkedRetries}. It must be 0 or more.");
        }
    }
}