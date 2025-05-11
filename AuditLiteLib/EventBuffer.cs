using System.Collections.Concurrent;
using AuditLite;

namespace AuditLiteLib;

public class EventBuffer(int maxBufferSize)
{
    private readonly ConcurrentQueue<AuditEvent> _buffer = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

	public void AddEvent(AuditEvent eventData)
	{
		_buffer.Enqueue(eventData);
	}

	public async Task<IReadOnlyCollection<AuditEvent>> FlushAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var eventsToFlush = new List<AuditEvent>(_buffer.Count);
            while (_buffer.TryDequeue(out var eventData))
            {
                eventsToFlush.Add(eventData);
            }
            return eventsToFlush;
        }
        finally
        {
            _semaphore.Release();
        }
    }
   
    public bool IsFull()
    {
        return _buffer.Count >= maxBufferSize;
    }

	public bool IsEmpty()
	{
        return _buffer.IsEmpty;
	}
}