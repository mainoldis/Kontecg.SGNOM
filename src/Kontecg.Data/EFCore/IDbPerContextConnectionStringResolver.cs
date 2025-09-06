using System.Threading.Tasks;
using Kontecg.Domain.Uow;

namespace Kontecg.EFCore
{
    /// <summary>
    ///     Extends <see cref="IConnectionStringResolver" /> to
    ///     get connection string for given context and company.
    /// </summary>
    public interface IDbPerContextConnectionStringResolver : IConnectionStringResolver
    {
        /// <summary>
        ///     Gets the connection string for given args.
        /// </summary>
        string GetNameOrConnectionString(DbPerContextConnectionStringResolverArgs args);

        /// <summary>
        ///     Gets the connection string for given args.
        /// </summary>
        Task<string> GetNameOrConnectionStringAsync(DbPerContextConnectionStringResolverArgs args);
    }
}
