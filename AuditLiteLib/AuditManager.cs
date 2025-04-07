using System.Text.Json;
using AuditLite;
using AuditLiteLib.Configuration;

namespace AuditLiteLib;
// Посмотреть про IAsyncDisposable
public class AuditManager : IDisposable
{
    private readonly AuditConfig _config;
    private readonly EventBuffer _buffer;
    private readonly Timer _timer;
    private readonly AuditClient _client;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;
    
    public AuditManager(AuditConfig config)
    {
        _config = config;
        _buffer = new EventBuffer(config.MaxBufferSize);
        // TimerCallback - Метод-обертка. Он представляет собой FlushBufferAndSendEventsAsync().
        _timer = new Timer(TimerCallback, null, _config.FlushIntervalMilliseconds, Timeout.Infinite);
        _client = new AuditClient(config.ServerUrl);
    }
    
    public async Task CreateAuditEvent(string eventType, Dictionary<string, object>? optionalFields)
    {
        var customAuditFields = ConvertToJsonDictionary(optionalFields);
        var auditEvent = new AuditEvent().FillFromDefaults(eventType, customAuditFields);

        await _buffer.AddEventAsync(auditEvent);

        await _semaphore.WaitAsync();
        try
        {
            if (_buffer.IsFull())
            {
                await FlushBufferAndSendEventsAsync();
                
                // тут вызать buffrt.FlushAsync
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // Todo решить как поступить с двумя действиями в этом методе.
    private async Task FlushBufferAndSendEventsAsync()
    {
        IReadOnlyCollection<AuditEvent> eventsToSend = await _buffer.FlushAsync();
            // Вынести foreach
        foreach (var auditEvent in eventsToSend)
        {
            bool response = await _client.SendEventAsync(auditEvent);
            Console.WriteLine(response ? "Отправлено!" : "Ошибка отправки.");
        }
        Console.WriteLine($"Отправлено {eventsToSend.Count} событий.");

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

    private Task StopBufferAsync()
    {
        return FlushBufferAndSendEventsAsync();
    }
     
    private void TimerCallback(object? state) 
    {
        FlushBufferAndSendEventsAsync().GetAwaiter().GetResult();
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        FlushBufferAndSendEventsAsync().GetAwaiter().GetResult();
        _timer.Dispose();
        _semaphore.Dispose();
        _disposed = true;
    }
}