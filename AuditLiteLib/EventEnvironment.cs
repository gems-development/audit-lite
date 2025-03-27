using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

namespace AuditLiteLib;

[ProtoContract]
public class EventEnvironment
{
    [ProtoMember(1)]
    public string UserName { get; }
    [ProtoMember(2)]
    public string MethodName { get; }
    [ProtoMember(3)]
    public string MachineName { get; }
    [ProtoMember(4)]
    public string IpAddress { get; }

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