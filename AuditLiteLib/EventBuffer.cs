namespace AuditLiteLib;

using System;
using System.Collections.Generic;
using System.Threading;

public class EventBuffer
{
    private readonly Queue<string> _buffer = new();
    private readonly int _maxBufferSize;
    private readonly int _flushInterval; // Интервал для таймера отправки в миллисекундах
    private readonly Timer _timer;
    private readonly object _lock = new();

    public EventBuffer(int flushIntervalMilliseconds = 300000, int maxBufferSize = 100)
    {
        _flushInterval = flushIntervalMilliseconds;
        _maxBufferSize = maxBufferSize;
        // Запускаем таймер, с методом обратног овызова Flush. Передаем null в переменной state.
        // flushInterval - время действия таймера. Timeout.Infinite - только ручной перезапуск таймера.
        _timer = new Timer(Flush, null, _flushInterval, Timeout.Infinite);
    }

    public void AddEvent(string eventData)
    {
        lock (_lock)
        {
            _buffer.Enqueue(eventData); // Добавляем событие в буфер

            // Проверяем, не превышен ли максимальный размер буфера
            if (_buffer.Count >= _maxBufferSize)
            {
                Flush(null);
            }
        }
    }
    
    public void Stop()
    {
        // Останавливаем таймер и отправляем скопившиеся события
        _timer.Dispose();
        Flush(null);
    }
    
    private void Flush(object? state)
    {
        List<string> eventsToFlush;

        lock (_lock)
        {
            if (_buffer.Count == 0)
            {
                // Если буфер пуст, перезапускаем таймер
                _timer.Change(_flushInterval, Timeout.Infinite);
                return;
            }
            
            eventsToFlush = new List<string>(_buffer);
            _buffer.Clear();
        }

        // Отправляем события. Логика работы с сервером.
        Console.WriteLine($"Отправлено {eventsToFlush.Count} событий: {string.Join(", ", eventsToFlush)}");

        // Перезапускаем таймер
        _timer.Change(_flushInterval, Timeout.Infinite);
    }
}