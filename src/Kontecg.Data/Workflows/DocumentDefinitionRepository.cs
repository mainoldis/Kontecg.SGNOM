using System.Linq;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;

namespace Kontecg.Workflows
{
    public class DocumentDefinitionRepository : KontecgRepositoryBase<DocumentDefinition>, IDocumentDefinitionRepository
    {
        /// <inheritdoc />
        public DocumentDefinitionRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        /// <inheritdoc />
        public DocumentDefinition GetByReference(string reference)
        {
            var queryable = GetQueryableDocumentDefinition()
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<DocumentDefinition> GetByReferenceAsync(string reference)
        {
            var queryable = (await GetQueryableDocumentDefinitionAsync())
                .Where(a => a.Reference == reference && a.IsActive);

            return queryable.SingleOrDefault();
        }

        protected virtual IQueryable<DocumentDefinition> GetQueryableDocumentDefinition()
        {
            return GetAllIncluding(p => p.Views, p => p.SignOnDefinitions);
        }

        protected virtual async Task<IQueryable<DocumentDefinition>> GetQueryableDocumentDefinitionAsync()
        {
            return await GetAllIncludingAsync(p => p.Views, p => p.SignOnDefinitions);
        }
    }
}