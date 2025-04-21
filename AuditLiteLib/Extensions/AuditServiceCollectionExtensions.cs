using AuditLiteLib.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuditLiteLib.Extensions
{
	public static class AuditServiceCollectionExtensions
	{
		public static IServiceCollection AddAuditLite(this IServiceCollection services, Action<AuditConfigBuilder> configure)
		{
			// Создаем конфигурацию через переданный делегат
			var configBuilder = new AuditConfigBuilder();
			configure(configBuilder);
			var auditConfig = configBuilder.Build();

			services.AddSingleton(auditConfig); 
			services.AddSingleton(new EventBuffer(auditConfig.MaxBufferSize));
			services.AddSingleton(new AuditClient(auditConfig.ServerUrl));
			services.AddLogging(builder => builder.AddConsole());

			services.AddSingleton<AuditManager>();

			return services;
		}
	}
}
