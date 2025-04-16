using System.Text.Json;
using AuditLite;
using AuditLiteLib.Configuration;
using Microsoft.Extensions.Logging;

namespace AuditLiteLib;
public class AuditManager : IDisposable, IAsyncDisposable
{
    private readonly AuditConfig _config;
    private readonly EventBuffer _buffer;
    private readonly AuditClient _client;
	private readonly ILogger<AuditManager> _logger;
	private readonly Timer _timer;
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	public AuditManager(AuditConfig config, EventBuffer buffer, AuditClient client, ILogger<AuditManager> logger)
    {
        _config = config;
        _buffer = buffer;
        _client = client;
		_logger = logger;
		_timer = new Timer(TimerCallback, null, _config.FlushIntervalMilliseconds, Timeout.Infinite);
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
                await PushEventsToServiceAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task PushEventsToServiceAsync()
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
     
    private async void TimerCallback(object? state) 
    {

		if (_buffer.IsEmpty())
		{
			_logger.LogInformation("Buffer is empty. Extract and send events skipped");
			RestartFlushTimer();
			return;
		}

		await PushEventsToServiceAsync();

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
		PushEventsToServiceAsync().GetAwaiter().GetResult();
        _timer.Dispose();
        _semaphore.Dispose();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await PushEventsToServiceAsync().ConfigureAwait(false);
		await _timer.DisposeAsync().ConfigureAwait(false);
        _semaphore.Dispose();
    }
}