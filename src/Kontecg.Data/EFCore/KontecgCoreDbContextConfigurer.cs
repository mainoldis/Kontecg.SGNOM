using System.Data.Common;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public static class KontecgCoreDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<KontecgCoreDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
            builder.UseExceptionProcessor();
            if(Debugging.DebugHelper.IsDebug)
                builder.EnableSensitiveDataLogging();
        }

        public static void Configure(DbContextOptionsBuilder<KontecgCoreDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
            builder.UseExceptionProcessor();
            if (Debugging.DebugHelper.IsDebug)
                builder.EnableSensitiveDataLogging();
        }
    }
}
