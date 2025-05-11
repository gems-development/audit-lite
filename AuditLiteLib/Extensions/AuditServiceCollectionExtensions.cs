using AuditLiteLib.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace AuditLiteLib.Extensions
{
	public static class AuditServiceCollectionExtensions
	{
		public static IServiceCollection AddAuditLite(this IServiceCollection services, 
			Action<AuditConfigBuilder> configure)
		{
			var configBuilder = new AuditConfigBuilder();
			configure(configBuilder);
			var auditConfig = configBuilder.Build();

			services.AddSingleton(auditConfig); 
			services.AddSingleton(new EventBuffer(auditConfig.MaxBufferSize));
			services.AddSingleton(new AuditClient(auditConfig.ServerUrl));
			services.AddLogging(builder => builder.AddConsole());
			services.AddSingleton<IAuditLiteManager, AuditManager>();

			return services;
		}
	}
}
