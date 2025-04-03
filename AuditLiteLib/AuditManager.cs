using System.Text.Json;
using AuditLiteLib.Configuration;

namespace AuditLiteLib;

public class AuditManager
{
    private readonly AuditConfig _config;
    private readonly EventBuffer _buffer;
    private readonly Timer _timer;
    // здесь так же будет поле для serverClient

    public AuditManager(AuditConfig config)
    {
        _config = config;
        _buffer = new EventBuffer(config.MaxBufferSize);
        // TimerCallback - Метод-обертка. Он предсатвляет собой FlushBuffer().
        _timer = new Timer(TimerCallback, null, _config.FlushIntervalMilliseconds, Timeout.Infinite);
    }
    
    public Task CreateAuditEvent(string eventType, Dictionary<string, object>? optionalFields)
    {
        Dictionary<string, string> customAuditFields = ConvertToJsonDictionary(optionalFields);
        return _buffer.AddEventAsync(new AuditEvent(eventType, customAuditFields));
    }
    
    private async Task FlushBuffer()
    {
         await _buffer.FlushAsync();
        
        // Здесь должен быть метод отвечающий за отправку полученных событий из метода _buffer.FlushAsync()
        
        _timer.Change(_config.FlushIntervalMilliseconds, Timeout.Infinite); // Перезапуск таймера
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
    
    // Метод-обертка, для обработки асинхронной операции.
    private void TimerCallback(object? state) 
    {
        FlushBuffer().GetAwaiter().GetResult();
    }
}