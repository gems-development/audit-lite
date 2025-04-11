using AuditLiteLib;
using AuditLiteLib.Configuration;
using Microsoft.Extensions.Logging;

namespace AuditLiteConsoleApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var auditConfig = new AuditConfigBuilder()
            .SetPredefinedConfigName("test")
            .SetServerUrl("http://localhost:5001")
            .SetFlushIntervalMilliseconds(10000)
            .SetMaxBufferSize(16)
            .Build();

        var logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<AuditManager>();
        
        var eventBuffer = new EventBuffer(
            maxBufferSize: auditConfig.MaxBufferSize
        );
        
        await using var auditManager = new AuditManager(auditConfig, logger); // Добавил using, тк реализовал в auditManager паттерн IDisposable

        Console.WriteLine("Начало теста...");

            // Эмуляция многопоточности.
            await Parallel.ForEachAsync(Enumerable.Range(1, 30), async (i, cancellationToken) =>
        {
            var eventData = new Dictionary<string, object>
            {
                { "field_1", $"Type - {i}" },
                { "field_2", $"Name - {i}" }
            };
            
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"[Поток {threadId}] Добавлено: {eventData}");
            
            await auditManager.CreateAuditEventAsync("testType", eventData);
            
            await Task.Delay(3000, cancellationToken);
        });

        Console.WriteLine("Тестирование завершено.");
    }
}