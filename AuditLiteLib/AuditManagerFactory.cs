using AuditLiteLib.Configuration;
using AuditLiteLib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace AuditLiteLib;

public static class AuditManagerFactory
{
	public static IAuditLiteManager Create(Action<AuditConfigBuilder> configure, IConfiguration? configuration = null)
	{
		var services = new ServiceCollection();
		services.AddAuditLite(configure);
		if (configuration is null)
		{
			var builder = new ConfigurationBuilder();
			configuration = builder.AddEnvironmentVariables().Build();
		}
		services.AddSingleton(configuration);
		services.AddLogging(logging => logging.AddConsole());
		services.AddFeatureManagement();
		
		services.AddAuditLite(configure);
		
		return services.BuildServiceProvider().GetRequiredService<IAuditLiteManager>();
	}
}