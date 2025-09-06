using Kontecg.Configuration;
using Kontecg.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Kontecg.EFCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class KontecgCoreDbContextFactory : IDesignTimeDbContextFactory<KontecgCoreDbContext>
    {
        public KontecgCoreDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<KontecgCoreDbContext>();
            var configuration = AppConfigurations.Get(
                KontecgContentDirectoryFinder.CalculateContentRootFolder(),
                addUserSecrets: true
            );

            KontecgCoreDbContextConfigurer.Configure(builder,
                configuration.GetConnectionString(KontecgCoreConsts.ConnectionStringName));

            return new KontecgCoreDbContext(builder.Options);
        }
    }
}
