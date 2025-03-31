using AuditLiteLib;
using AuditLiteLib.Configuration;

namespace AuditLiteConsoleApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var auditConfig = new AuditConfigBuilder()
            .SetPredefinedConfigName("test")
            .SetServerUrl("http://localhost:5000")
            .SetFlushIntervalMilliseconds(4000)
            .SetMaxBufferSize(10)
            .Build();
        
        var eventBuffer = new EventBuffer(
            flushIntervalMilliseconds: auditConfig.FlushIntervalMilliseconds,
            maxBufferSize: auditConfig.MaxBufferSize
        );

        Console.WriteLine("Начало теста...");
            
            // Эмуляция многопоточности.
        await Parallel.ForEachAsync(Enumerable.Range(1, 30), async (i, cancellationToken) =>
        {
            AuditEvent eventData = new AuditEvent($"Событие {i}");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"[Поток {threadId}] Добавлено: {eventData}");
            await eventBuffer.AddEventAsync(eventData);
            
            await Task.Delay(3000, cancellationToken);
        });
        
        Console.WriteLine("Останавливаем буфер...");
        await eventBuffer.StopAsync();

        Console.WriteLine("Тестирование завершено.");
    }
}