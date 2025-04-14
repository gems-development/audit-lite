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
        // TimerCallback - Метод-обертка. Он представляет собой FlushBufferAndSendEventsAsync().
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
                await FlushBufferedEventsAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task FlushBufferedEventsAsync() // ToDo что все таки с названием? Может быть уместно оставить FlushBufferAndSendEventsAsync()?
    {
		var eventsToSend = await ExtractEventsFromBufferAsync();

		RestartFlushTimer();

        await SendEventsAsync(eventsToSend);
    }
    
    private async Task<IReadOnlyCollection<AuditEvent>> ExtractEventsFromBufferAsync()
    {
        return await _buffer.FlushAsync();
    }
    
    private void RestartFlushTimer()
    {
        _timer.Change(_config.FlushIntervalMilliseconds, Timeout.Infinite);
    }
    
    private async Task SendEventsAsync(IReadOnlyCollection<AuditEvent> eventsToSend)
    {
        var auditEventList = eventsToSend.ToAuditEventList();
        AuditResponse success = await _client.SendEventAsync(auditEventList);

        if (success.Success)
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

    private async Task StopBufferAsync()
    {
        await FlushBufferedEventsAsync();
        
        // Если нужно удалять ссылку на буфер, можно сделать это здесь
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

        await FlushBufferedEventsAsync();
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
        StopBufferAsync().GetAwaiter().GetResult();
        _timer.Dispose();
        _semaphore.Dispose();
                
        // _timer = null; // Не могу выполнить, тк таймер ReadOnly.
        // Нужно ли как-либо обработать буфер?
        // Избавиться ли от StopBufferAsync() в пользу FlushBufferAndSendEventsAsync()? 
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await StopBufferAsync().ConfigureAwait(false);
        await _timer.DisposeAsync().ConfigureAwait(false);
        _semaphore.Dispose();
    }
}