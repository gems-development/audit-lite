namespace AuditLiteLib.Serializers;

public interface ISerializer
{
    public static abstract byte[] Serialize (AuditEvent auditEvent);
}