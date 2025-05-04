using System.Text.Json;
using AuditLite;
using AuditLiteLib.Configuration;
using Grpc.Core;
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
    private int? _optimalChunkSize;

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
                RestartFlushTimer();
            }
        }
        finally
        {
            _semaphore.Release();
        }
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
        
        _timer.Dispose();
        
        if (!_buffer.IsEmpty())  
        {
            PushEventsToServiceAsync().GetAwaiter().GetResult();
            _logger.LogInformation("All remaining events were successfully pushed");
        }
        _semaphore.Dispose();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _timer.DisposeAsync().ConfigureAwait(false);
        
        if (!_buffer.IsEmpty())  
        {
            await PushEventsToServiceAsync().ConfigureAwait(false);
            _logger.LogInformation("All remaining events were successfully pushed");
        }
        _semaphore.Dispose();
    }
    
    private async Task PushEventsToServiceAsync()
    {
        try
        {
            var eventsToSend = await ExtractEventsFromBufferAsync();
            
            await SendEventsWithRetryAsync(eventsToSend, _config.MaxBufferSize, _config.MaxChunkedRetries);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed push audit events to service. Error details: {e.Message}");
            // Todo возможно тут стоит дописать логику по сохранению событий, который не смогли отправиться.
        }
    }
    
    private async Task SendEventsWithRetryAsync(
        IReadOnlyCollection<AuditEvent> events,
        int initialChunkSize,
        int maxChunkedRetries)
    {
        var currentChunkSize = _optimalChunkSize ?? initialChunkSize;
        var chunkedRetries = maxChunkedRetries;

        while (chunkedRetries >= 0)
        {
            try
            {
                foreach (var chunk in events.Chunk(currentChunkSize))
                {
                    await SendEventsAsync(chunk);
                }
                _optimalChunkSize = currentChunkSize;
                return;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.ResourceExhausted && chunkedRetries > 0)
            {
                _logger.LogWarning($"Chunk size {currentChunkSize} too large. Retrying with size {currentChunkSize / 2}...");
                currentChunkSize = Math.Max(1, currentChunkSize / 2);
                chunkedRetries--;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogError($"Service unavailable. Failed to send {events.Count} audit events. Error details: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                if (chunkedRetries <= 0)
                {
                    _logger.LogError($"Failed {maxChunkedRetries} retry attempts to push audit events. Try Error details: {e.Message}");
                    return;
                }
                throw;
            }
        }
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
        AuditResponse response = await _client.SendEventAsync(auditEventList);
        
        if (response.Success)
        {
            _logger.LogInformation(response.Message);
        }
        else
        {
            _logger.LogError(response.Message);
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
        try
        {
            if (_buffer.IsEmpty()) return;
            
            await PushEventsToServiceAsync();
        }
        finally
        {
            RestartFlushTimer();
        }
    }
}