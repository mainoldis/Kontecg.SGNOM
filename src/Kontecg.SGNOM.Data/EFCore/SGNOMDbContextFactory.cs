using Kontecg.Configuration;
using Kontecg.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Kontecg.EFCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class SGNOMDbContextFactory : IDesignTimeDbContextFactory<SGNOMDbContext>
    {
        public SGNOMDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SGNOMDbContext>();
            var configuration = AppConfigurations.Get(
                KontecgContentDirectoryFinder.CalculateContentRootFolder(),
                addUserSecrets: true
            );

            SGNOMDbContextConfigurer.Configure(builder,
                configuration.GetConnectionString(SGNOMConsts.ConnectionStringName));

            return new SGNOMDbContext(builder.Options);
        }
    }
}
