using System.Data.Common;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public static class SGNOMDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SGNOMDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
            builder.UseExceptionProcessor();
        }

        public static void Configure(DbContextOptionsBuilder<SGNOMDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
            builder.UseExceptionProcessor();
        }
    }
}
