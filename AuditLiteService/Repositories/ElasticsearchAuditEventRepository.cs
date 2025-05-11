using AuditLite;
using AuditLiteService.Extensions;
using AuditLiteService.Models.Elasticsearch;
using Nest;

namespace AuditLiteService.Repositories;

public class ElasticsearchAuditEventRepository : IAuditEventRepository
{
    private readonly IElasticClient _elasticClient;
    private readonly string _indexName = "audit-events";

    public ElasticsearchAuditEventRepository(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
        
        var indexExistsResponse = _elasticClient.Indices.Exists(_indexName);
        if (!indexExistsResponse.Exists)
        {
            _elasticClient.Indices.Create(_indexName, c => c
                .Map<ElasticsearchAuditEventEntity>(m => m.AutoMap()));
        }
    }

    public async Task SaveAsync(AuditEventList events)
    {
        var bulkOperations = new List<IBulkOperation>();

        foreach (var auditEvent in events.AuditEvents)
        {
            var elasticsearchEvent = auditEvent.ToElasticsearchEntity();
            bulkOperations.Add(new BulkIndexOperation<ElasticsearchAuditEventEntity>(elasticsearchEvent));
        }

        var bulkRequest = new BulkRequest(_indexName)
        {
            Operations = bulkOperations
        };

        var bulkResponse = await _elasticClient.BulkAsync(bulkRequest);

        if (!bulkResponse.IsValid)
        {
            throw new Exception($"Ошибка при сохранении событий в Elasticsearch: {bulkResponse.ServerError?.Error}");
        }
    }
}