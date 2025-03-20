using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace AuditLiteLib;

public class EventEnvironment
{
    private string UserName { get; set; }
    private string MethodName { get; set; }
    private string MachineName { get; set; }
    private string IpAddress { get; set; }

    public EventEnvironment()
    {
        UserName = Environment.UserName;
        MachineName = Environment.MachineName;
        IpAddress = GetLocalIpAddress();
        MethodName = GetCallerMethodName();
    }
    private string GetLocalIpAddress()
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
    
    private string GetCallerMethodName()
    {
        var stackTrace = new StackTrace();
        return stackTrace.GetFrame(2)?.GetMethod()?.Name ?? "Unknown";
    }
    
}