namespace AuditLiteLib.Configuration;

public interface IConfiguration
{
    string? GetValue(string key);
    void SetValue(string key, string value);
}