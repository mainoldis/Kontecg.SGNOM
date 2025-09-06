using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Kontecg.EFCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureReferenceGroupDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, KontecgEfCoreCurrentDbContext kontecgEfCoreCurrentDbContext)
        {
            return modelBuilder;
        }
    }
}