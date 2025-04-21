using AuditLiteLib.Configuration;
using AuditLiteLib.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuditLiteLib;

public static class AuditManagerFactory
{
	public static AuditManager Create(Action<AuditConfigBuilder> configure)
	{
		var services = new ServiceCollection();
		services.AddAuditLite(configure);
		return services.BuildServiceProvider().GetRequiredService<AuditManager>();
	}
}