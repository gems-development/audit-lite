using System.Text.Json;
using AuditLiteLib.Configuration;
using AuditLiteLib.Serializers;

namespace AuditLiteLib;

public class AuditManager
{
    private AuditConfig _config;
    private EventBuffer _buffer;
    // здесь так же будет поле для serverClient

    public AuditManager(AuditConfig config)
    {
        _config = config;
        _buffer = new EventBuffer(config.FlushIntervalMilliseconds, config.MaxBufferSize);
    }
    

    public void CreateAuditEvent(string eventType, Dictionary<string, object>? optionalFields)
    {
        Dictionary<string, string> customAuditFields = ConvertToJsonDictionary(optionalFields);
        _buffer.AddEventAsync(new AuditEvent(eventType, customAuditFields));
    }
    
    
    
    private static Dictionary<string, string> ConvertToJsonDictionary(Dictionary<string, object>? source)
    {
        var result = new Dictionary<string, string>();
        if (source == null) return result;

        foreach (var kvp in source)
        {
            result[kvp.Key] = JsonSerializer.Serialize(kvp.Value);
        }
        return result;
    }

    // Это должно быть на сервере
    private static Dictionary<string, object> ConvertFromJsonDictionary(Dictionary<string, string>? source)
    {
        var result = new Dictionary<string, object>();
        if (source == null) return result;

        foreach (var kvp in source)
        {
            try
            {
                result[kvp.Key] = JsonSerializer.Deserialize<object>(kvp.Value)!;
            }
            catch
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        return result;
    }
}