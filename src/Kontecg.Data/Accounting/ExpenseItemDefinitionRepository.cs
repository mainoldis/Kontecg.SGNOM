using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Kontecg.Accounting
{
    public class ExpenseItemDefinitionRepository : KontecgRepositoryBase<ExpenseItemDefinition>, IExpenseItemDefinitionRepository
    {
        /// <inheritdoc />
        public ExpenseItemDefinitionRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider) : base(
            dbContextProvider)
        {
        }

        public virtual ExpenseItemDefinition GetByReference(string reference)
        {
            var queryable = GetQueryableExpenseItemDefinition()
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<ExpenseItemDefinition> GetByReferenceAsync(string reference)
        {
            var queryable = (await GetQueryableExpenseItemDefinitionAsync())
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        protected virtual IQueryable<ExpenseItemDefinition> GetQueryableExpenseItemDefinition()
        {
            return GetQueryable()
                .Include(c => c.CenterCostDefinition)
                .ThenInclude(c => c.AccountDefinition);
        }

        protected virtual async Task<IQueryable<ExpenseItemDefinition>> GetQueryableExpenseItemDefinitionAsync()
        {
            return (await GetQueryableAsync())
                .Include(c => c.CenterCostDefinition)
                .ThenInclude(c => c.AccountDefinition);
        }
    }
}