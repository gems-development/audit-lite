using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Auditlitelib.Protos;


namespace AuditLiteLib;

public static class EventEnvironmentExtensions
{
    public static EventEnvironment FillFromCurrentEnvironment(this EventEnvironment env)
    {
        return new EventEnvironment
        {
            UserName = Environment.UserName,
            MachineName = Environment.MachineName,
            IpAddress = GetLocalIpAddress(),
            MethodName = GetCallerMethodName()
        };
    }
    private static string GetLocalIpAddress()
    {
        try
        {
            return Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                .ToString() ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
    
    private static string GetCallerMethodName()
    {
        var stackTrace = new StackTrace();
        return stackTrace.GetFrame(2)?.GetMethod()?.Name ?? "Unknown";
    }
    
}