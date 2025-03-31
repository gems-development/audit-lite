using System.Collections.Concurrent;

namespace AuditLiteLib;

using System;
using System.Collections.Generic;

public class EventBuffer
{
    private readonly ConcurrentQueue<AuditEvent> _buffer = new();
    private readonly int _maxBufferSize;

    public EventBuffer(int maxBufferSize = 100)
    {
        _maxBufferSize = maxBufferSize;
    }
    
    public async Task AddEventAsync(AuditEvent eventData)
    {
        _buffer.Enqueue(eventData);

        if (IsBufferFull())
        {
            await FlushAsync();
        }
    }
    
    public async Task StopAsync()
    {
        await FlushAsync();
    }
    
    public async Task<List<AuditEvent>> FlushAsync()
    {
        return await Task.Run(() =>
        {
            var eventsToFlush = new List<AuditEvent>();
            while (_buffer.TryDequeue(out var eventData))
            {
                eventsToFlush.Add(eventData);
            }
            
            // На текущий момент выводиться несколько раз. Тк эмуляция многопточности.
            // Console.WriteLine($"Отправлено {eventsToFlush.Count()} событий.");
            
            return eventsToFlush;
        });
    }
    
    private bool IsBufferFull()
    {
        return _buffer.Count >= _maxBufferSize;
    }
}