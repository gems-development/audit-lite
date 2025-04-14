using System.Text.Json;
using AuditLite;
using AuditLiteLib.Configuration;
using Microsoft.Extensions.Logging;

namespace AuditLiteLib;
public class AuditManager : IDisposable, IAsyncDisposable
{
    private readonly AuditConfig _config;
    private readonly EventBuffer _buffer;
    private readonly Timer _timer;
    private readonly AuditClient _client;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger<AuditManager> _logger;
    
    public AuditManager(AuditConfig config, ILogger<AuditManager> logger)
    {
        _config = config;
        _logger = logger;
        _buffer = new EventBuffer(config.MaxBufferSize);
        _timer = new Timer(TimerCallback, null, _config.FlushIntervalMilliseconds, Timeout.Infinite);
        _client = new AuditClient(config.ServerUrl);
    }
    
    public async Task CreateAuditEventAsync(string eventType, Dictionary<string, object>? optionalFields)
    {
        var customAuditFields = ConvertToJsonDictionary(optionalFields);
        var auditEvent = new AuditEvent().FillFromDefaults(eventType, customAuditFields);

        _buffer.AddEvent(auditEvent);

        await _semaphore.WaitAsync();
        try
        {
            if (_buffer.IsFull())
            {
                await PushEventsToSenderAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task PushEventsToSenderAsync()
	{
        var eventsToSend = await ExtractEventsFromBufferAsync();
            
        RestartFlushTimer();

        await SendEventsAsync(eventsToSend);
    }
    
    private Task<IReadOnlyCollection<AuditEvent>> ExtractEventsFromBufferAsync()
    {
        return _buffer.FlushAsync();
    }
    
    private void RestartFlushTimer()
    {
        _timer.Change(_config.FlushIntervalMilliseconds, Timeout.Infinite);
    }
    
    private async Task SendEventsAsync(IReadOnlyCollection<AuditEvent> eventsToSend)
    {
        var auditEventList = eventsToSend.ToAuditEventList();
        bool success = await _client.SendEventAsync(auditEventList);

        if (success)
        {
            _logger.LogInformation("Successfully sent {Count} events", auditEventList.AuditEvents.Count);
        }
        else
        {
            _logger.LogWarning("Error sending {Count} events.", auditEventList.AuditEvents.Count);
        }
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
     
    private async void TimerCallback(object? state) 
    {
		// ToDo Стоит ли вынести эту проверку в FlushBufferedEventsAsync(), либо продублировать ее в Dispose().
		// Тк при вызове Dispose(), в конце выполнения, может также отправиться пустой спсиок. Не то чтобы это большая проблема, но стоит обсудить.

		if (_buffer.IsEmpty())
		{
			_logger.LogInformation("Buffer is empty. Flushing skipped");
			RestartFlushTimer();
			return;
		}

		await PushEventsToSenderAsync();

    }
    
	public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
		PushEventsToSenderAsync().GetAwaiter().GetResult();
        _timer.Dispose();
        _semaphore.Dispose();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await PushEventsToSenderAsync().ConfigureAwait(false);
		await _timer.DisposeAsync().ConfigureAwait(false);
        _semaphore.Dispose();
    }
}