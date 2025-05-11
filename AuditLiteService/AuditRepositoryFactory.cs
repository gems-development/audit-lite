using AuditLiteService.Interfaces;
using AuditLiteService.Repositories;

namespace AuditLiteService;

public class AuditRepositoryFactory(IServiceProvider provider, IConfiguration configuration) : IAuditRepositoryFactory
{
    public IAuditEventRepository CreateRepository()
    {
        var databaseType = configuration["DatabaseSettings:DatabaseType"];
        return databaseType switch
        {
            "MongoDB" => provider.GetRequiredService<MongoAuditEventRepository>(),
            "Postgres" => provider.GetRequiredService<PostgresAuditEventRepository>(),
            "Elasticsearch" => provider.GetRequiredService<ElasticsearchAuditEventRepository>(),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported")
        };
    }
}