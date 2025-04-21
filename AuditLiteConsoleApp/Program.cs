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
					 .SetFlushIntervalMilliseconds(3000)
					 .SetMaxBufferSize(50000);
		});
		
		Console.WriteLine("Начало теста...");

		var eventCounter = 0;
		
		var options = new ParallelOptions
		{
			MaxDegreeOfParallelism = 8 // Устанавливаем максимальное количество потоков
		};
	
            // Эмуляция многопоточности.
            await Parallel.ForEachAsync(Enumerable.Range(1, 100000), options, async (i, cancellationToken) =>
        {
            var eventData = new Dictionary<string, object>
            {
                { "field_1", $"Type - {i}" },
                { "field_2", $"Name - {i}" }
            };
            
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var currentEventNumber = Interlocked.Increment(ref eventCounter);
            
	        //Console.WriteLine($"[Поток {threadId}] Номер: {currentEventNumber} Событие: {eventData}");
            
            await auditManager.CreateAuditEventAsync("testType", eventData);
            
	        //await Task.Delay(100, cancellationToken); // Раньше, без задержки, таймер не срабатывал. До появления Семафора в TimerCallBack.
        });

        Console.WriteLine("Тестирование завершено.");
    }
}