using System.Collections.Concurrent;
using Auditlitelib.Protos;

namespace AuditLiteLib;

using System;
using System.Collections.Generic;

public class EventBuffer(int maxBufferSize)
{
    private readonly ConcurrentQueue<AuditEvent> _buffer = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task AddEventAsync(AuditEvent eventData)
    {
        try
        {
            _buffer.Enqueue(eventData);

            if (IsBufferFull())
            {
                await Task.Delay(1000);
                await FlushAsync();
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Ошибка при добавлении события в буфер: {e.Message}");
            throw;
        }
    }
    
    public async Task<List<AuditEvent>> FlushAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_buffer.IsEmpty)
            {
                return new List<AuditEvent>(); // Если буфер пуст, возвращаем пустой лист
            }

            var eventsToFlush = new List<AuditEvent>(_buffer.Count);
            while (_buffer.TryDequeue(out var eventData))
            {
                eventsToFlush.Add(eventData);
            }

            Console.WriteLine($"Отправлено {eventsToFlush.Count} событий.");

            return eventsToFlush;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Ошибка при очистке буфера: {e.Message}");
            throw;
        }
        finally
        {
            _semaphore.Release(); // Освобождаем семафор
        }
    }
    
    public async Task StopAsync()
    {
         await FlushAsync();
    }
    
    private bool IsBufferFull()
    {
        return _buffer.Count >= maxBufferSize;
    }
}