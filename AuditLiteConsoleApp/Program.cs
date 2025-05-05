using AuditLiteLib;

namespace AuditLiteConsoleApp;

internal static class Program
{
    public static async Task Main(string[] args)
    {
	    
		await using var auditManager = AuditManagerFactory.Create(configure =>
		{
			configure.SetServerUrl("http://localhost:5001")
					 .SetFlushIntervalMilliseconds(1000) // значение -1 отключает срабатывание по таймеру в целом
					 .SetMaxBufferSize(10000)
					 .SetMaxChunkedRetries(3); // Значение 0 будет означать о запрете отправлять данные чанками
		});
		
		Console.WriteLine("Начало теста...");

		var eventCounter = 0;
		
		var options = new ParallelOptions
		{
			MaxDegreeOfParallelism = 8
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
            
	        await Task.Delay(1, cancellationToken); // задержка для эмуляции реальных условий. Без нее таймер не будет срабатывать должным образом.
        });

        Console.WriteLine("Тестирование завершено.");
    }
}