using ProtoBuf;

namespace AuditLiteLib.Serializers;

abstract class ProtobufSerializer: ISerializer
{
    public static byte[] Serialize (AuditEvent auditEvent)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, auditEvent);
        return ms.ToArray();
    }
}