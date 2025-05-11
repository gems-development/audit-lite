using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using AuditLite;

namespace AuditLiteLib.Extensions;

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

        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            var declaringType = method?.DeclaringType;
            
            if (declaringType == null) continue;

            var ns = declaringType.Namespace;
            if (ns != null && ns.StartsWith("AuditLiteLib")) continue;
            
            if (ns != null && ns.StartsWith("System")) continue;

            return $"{declaringType.FullName}.{method?.Name}";
        }

        return "Unknown";
    }
}