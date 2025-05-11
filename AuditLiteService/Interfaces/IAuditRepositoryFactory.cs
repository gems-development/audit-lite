namespace AuditLiteService.Interfaces;

public interface IAuditRepositoryFactory
{
    IAuditEventRepository CreateRepository();
}