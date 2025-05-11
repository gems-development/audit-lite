namespace AuditLiteService.Models.Elasticsearch;

public class ElasticsearchEnvironmentEntity
{
    public string UserName { get; set; } = null!;
    public string MethodName { get; set; } = null!;
    public string MachineName { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
}