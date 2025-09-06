using Microsoft.Extensions.DependencyInjection;

namespace Kontecg.HealthChecks
{
    public static class KontecgHealthCheck
    {
        public static IHealthChecksBuilder AddKontecgHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<KontecgDbContextHealthCheck>("Database Connection");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}