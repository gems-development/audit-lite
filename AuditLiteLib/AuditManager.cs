using System.Text.Json;
using AuditLiteLib.Configuration;
using Auditlitelib.Protos;

namespace AuditLiteLib;

public class AuditManager
{
    private readonly AuditConfig _config;
    private readonly EventBuffer _buffer;
    private readonly Timer _timer;
    private readonly AuditClient _client;

    public AuditManager(AuditConfig config)
    {
        _config = config;
        _buffer = new EventBuffer(config.MaxBufferSize);
        // TimerCallback - Метод-обертка. Он предсатвляет собой FlushBuffer().
        _timer = new Timer(TimerCallback, null, _config.FlushIntervalMilliseconds, Timeout.Infinite);
        _client = new AuditClient(config.ServerUrl);
    }
    
    public async Task CreateAuditEvent(string eventType, Dictionary<string, object>? optionalFields)
    {
        Dictionary<string, string> customAuditFields = ConvertToJsonDictionary(optionalFields);
        await _buffer.AddEventAsync(new AuditEvent().FillFromDefaults(eventType, customAuditFields));
    }
    
    private async Task FlushBuffer()
    {
        List<AuditEvent> auditEvents = await _buffer.FlushAsync();
        // Здесь должен быть метод отвечающий за отправку полученных событий из метода _buffer.FlushAsync()
        foreach (AuditEvent auditEvent in auditEvents)
        {
            bool response = await _client.SendEventAsync(auditEvent);
            Console.WriteLine(response ? "Отправлено!" : "Ошибка отправки.");
        }
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
    
    // Метод-обертка, для обработки асинхронной операции.
    private void TimerCallback(object? state) 
    {
        FlushBuffer().GetAwaiter().GetResult();
    }
}