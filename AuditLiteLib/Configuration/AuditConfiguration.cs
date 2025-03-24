namespace AuditLiteLib.Configuration;

public class AuditConfiguration : IConfiguration
{
    private readonly Dictionary<string, string> _settings = new();

    public string ServerUrl
    {
        get => GetValue("ServerUrl") ?? "http://localhost:8080/api/audit";
        set => SetValue("ServerUrl", value);
    }

    public int BufferSize
    {
        get => int.TryParse(GetValue("BufferSize"), out var result) ? result : 50;
        set => SetValue("BufferSize", value.ToString());
    }
    
    // Для случаев, когда нашу библиотеку захотят использовать отдельно от сервера
    // и понадобится другой формат данных. Оставляем?
    public string EncodingType
    {
        get => GetValue("EncodingType") ?? "protobuf";
        set => SetValue("EncodingType", value);
    }
    
    public string? GetValue(string key) => _settings.GetValueOrDefault(key);
    public void SetValue(string key, string value) => _settings[key] = value;
}