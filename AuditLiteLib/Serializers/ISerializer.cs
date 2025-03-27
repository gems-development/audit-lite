namespace AuditLiteLib.Serializers;

public interface ISerializer
{
    public static abstract Object Serialize (AuditEvent auditEvent);
}