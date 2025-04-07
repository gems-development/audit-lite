using AuditLiteLib;
using AuditLiteLib.Configuration;

namespace AuditLiteConsoleApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var auditConfig = new AuditConfigBuilder()
            .SetPredefinedConfigName("test")
            .SetServerUrl("http://localhost:5001")
            .SetFlushIntervalMilliseconds(5000)
            .SetMaxBufferSize(8)
            .Build();
        
        var eventBuffer = new EventBuffer(
            maxBufferSize: auditConfig.MaxBufferSize
        );
        
        var auditManager = new AuditManager(auditConfig);

        Console.WriteLine("Начало теста...");
            
            // Эмуляция многопоточности.
            await Parallel.ForEachAsync(Enumerable.Range(1, 30), async (i, cancellationToken) =>
        {
            var eventData = new Dictionary<string, object>
            {
                { "Key1", $"Value{i}" },
                { "Key2", i }
            };
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"[Поток {threadId}] Добавлено: {eventData}");
            await auditManager.CreateAuditEvent("testType", eventData);
            
            await Task.Delay(3000, cancellationToken);
        });
        
        Console.WriteLine("Останавливаем буфер...");
        await eventBuffer.StopBufferAsync();

        Console.WriteLine("Тестирование завершено.");
    }
}