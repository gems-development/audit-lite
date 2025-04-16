using AuditLiteLib;

namespace AuditLiteConsoleApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
		await using var auditManager = AuditManagerFactory.Create(configure =>
		{
			configure.SetPredefinedConfigName("test")
					 .SetServerUrl("http://localhost:5001")
					 .SetFlushIntervalMilliseconds(10000)
					 .SetMaxBufferSize(8);
		});

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