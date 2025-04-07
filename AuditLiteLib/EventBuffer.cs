using System.Collections.Concurrent;
using AuditLite;

namespace AuditLiteLib;

using System;
using System.Collections.Generic;

public class EventBuffer(int maxBufferSize)
{
    private readonly ConcurrentQueue<AuditEvent> _buffer = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    // ToDo Нужно убрать тут семафор.
    public async Task AddEventAsync(AuditEvent eventData)
    {
        await _semaphore.WaitAsync();
        try
        {
            _buffer.Enqueue(eventData);
        }
        finally
        {
            _semaphore.Release();
        }
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
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Ошибка при извлечении сообщений из буфера: {e.Message}");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    // ToDo Пригодится ли этот метод StopBufferAsync()?
    public async Task StopBufferAsync()
    {
         await FlushAsync();
    }
    
    public bool IsFull()
    {
        return _buffer.Count >= maxBufferSize;
    }
}