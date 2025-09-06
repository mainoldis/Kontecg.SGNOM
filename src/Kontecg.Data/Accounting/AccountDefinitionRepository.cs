using System.Linq;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;

namespace Kontecg.Accounting
{
    public class AccountDefinitionRepository : KontecgRepositoryBase<AccountDefinition> , IAccountDefinitionRepository
    {
        /// <inheritdoc />
        public AccountDefinitionRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider) 
            : base(
            dbContextProvider)
        {
        }

        /// <inheritdoc />
        public virtual AccountDefinition GetByReference(string reference)
        {
            var queryable = GetQueryableAccountDefinition()
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<AccountDefinition> GetByReferenceAsync(string reference)
        {
            var queryable = (await GetQueryableAccountDefinitionAsync())
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        protected virtual IQueryable<AccountDefinition> GetQueryableAccountDefinition()
        {
            return GetQueryable();
        }

        protected virtual async Task<IQueryable<AccountDefinition>> GetQueryableAccountDefinitionAsync()
        {
            return await GetQueryableAsync();
        }
    }
}