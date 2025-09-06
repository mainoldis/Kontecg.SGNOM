using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Accounting
{
    public class CenterCostDefinitionRepository : KontecgRepositoryBase<CenterCostDefinition>, ICenterCostDefinitionRepository
    {
        /// <inheritdoc />
        public CenterCostDefinitionRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider) : base(
            dbContextProvider)
        {
        }

        public virtual CenterCostDefinition GetByReference(string reference)
        {
            var queryable = GetQueryableCenterCostDefinition()
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<CenterCostDefinition> GetByReferenceAsync(string reference)
        {
            var queryable = (await GetQueryableCenterCostDefinitionAsync())
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        protected virtual IQueryable<CenterCostDefinition> GetQueryableCenterCostDefinition()
        {
            return GetQueryable().Include(c => c.AccountDefinition);
        }

        protected virtual async Task<IQueryable<CenterCostDefinition>> GetQueryableCenterCostDefinitionAsync()
        {
            return (await GetQueryableAsync()).Include(c => c.AccountDefinition);
        }
    }
}