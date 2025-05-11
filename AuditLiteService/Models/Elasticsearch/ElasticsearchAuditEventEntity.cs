namespace AuditLiteService.Models.Elasticsearch;

public class ElasticsearchAuditEventEntity
{
    public string EventType { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public ElasticsearchEnvironmentEntity ElasticsearchAuditEvent { get; set; } = null!;
    public Dictionary<string, string> CustomFields { get; set; } = new();
}