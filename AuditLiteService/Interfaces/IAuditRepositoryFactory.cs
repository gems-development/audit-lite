using AuditLite;

namespace AuditLiteService;

public interface IAuditRepositoryFactory
{
    IAuditEventRepository CreateRepository();
}