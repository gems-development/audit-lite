namespace AuditLiteService;

public class AuditRepositoryFactory : IAuditRepositoryFactory
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;

    public AuditRepositoryFactory(IServiceProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public IAuditEventRepository CreateRepository()
    {
        var databaseType = _configuration["DatabaseSettings:DatabaseType"];
        return databaseType switch
        {
            "MongoDB" => _provider.GetRequiredService<MongoAuditEventRepository>(),
            "Postgres" => _provider.GetRequiredService<PostgresAuditEventRepository>(),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported")
        };
    }
}