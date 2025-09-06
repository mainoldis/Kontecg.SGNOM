using System.Threading;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kontecg.HealthChecks
{
    public class KontecgDbContextHealthCheck : IHealthCheck
    {
        private readonly KontecgCoreDbDatabaseCheckHelper _checkHelper;

        public KontecgDbContextHealthCheck(KontecgCoreDbDatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        /// <inheritdoc />
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.CanConnect("db")) 
                return Task.FromResult(HealthCheckResult.Healthy("KontecgCoreDbContext connected to database."));

            return Task.FromResult(HealthCheckResult.Unhealthy("KontecgCoreDbContext could not connect to database"));
        }
    }
}