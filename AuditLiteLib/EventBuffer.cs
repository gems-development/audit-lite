using System.Collections.Concurrent;

namespace AuditLiteLib;

using System;
using System.Collections.Generic;
using System.Threading;

public class EventBuffer
{
    private readonly ConcurrentQueue<string> _buffer = new();
    private readonly int _maxBufferSize;
    private readonly int _flushInterval;
    private readonly Timer _timer;

    public EventBuffer(int flushIntervalMilliseconds = 300000, int maxBufferSize = 100)
    {
        _flushInterval = flushIntervalMilliseconds;
        _maxBufferSize = maxBufferSize;
        // Timeout.Infinite - только ручной перезапуск таймера.
        _timer = new Timer(async _ => await FlushAsync(), null, _flushInterval, Timeout.Infinite);
    }
    
    public async Task AddEventAsync(string eventData)
    {
        await Task.Run(() => _buffer.Enqueue(eventData));

        if (_buffer.Count >= _maxBufferSize)
        {
            await FlushAsync();
        }
    }
    
    public async Task StopAsync()
    {
        _timer.Dispose();
        await FlushAsync(); // Асинхронное опустошение очереди
    }
    
    private async Task FlushAsync()
    {
        var eventsToFlush = new List<string>();
        while (_buffer.TryDequeue(out var eventData))
        {
            eventsToFlush.Add(eventData);
        }

        if (eventsToFlush.Count > 0)
        {
            await SendEventsAsync(eventsToFlush); // Отправка данных
        }

        _timer.Change(_flushInterval, Timeout.Infinite); // Перезапуск таймера
    }
    
    private async Task SendEventsAsync(IEnumerable<string> events)
    {
        await Task.Delay(100);
        Console.WriteLine($"Отправлено {events.Count()} событий.");
    }
}