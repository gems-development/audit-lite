using ProtoBuf;

namespace AuditLiteLib.Serializers;

public class ProtobufSerializer: ISerializer
{
    public static Object Serialize (AuditEvent auditEvent)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, auditEvent);
        return ms.ToArray();
    }
    public static AuditEvent Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        return Serializer.Deserialize<AuditEvent>(ms);
    }
    
    // перенести это в AuditManager
    // Убрать зависимость от библиотеки JsonConvert использовать встроенный функционал C# XMLSerialization
    public static Dictionary<string, string> ConvertToJsonDictionary(Dictionary<string, object>? source)
    {
        var result = new Dictionary<string, string>();
        if (source == null) return result;

        foreach (var kvp in source)
        {
            // Переписать используя JsonSerializer, встроенный в C#
            //result[kvp.Key] = JsonConvert.SerializeObject(kvp.Value);
        }
        return result;
    }

    public static Dictionary<string, object> ConvertFromJsonDictionary(Dictionary<string, string>? source)
    {
        var result = new Dictionary<string, object>();
        if (source == null) return result;

        foreach (var kvp in source)
        {
            try
            {
                //result[kvp.Key] = JsonConvert.DeserializeObject<object>(kvp.Value);
            }
            catch
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        return result;
    }
}