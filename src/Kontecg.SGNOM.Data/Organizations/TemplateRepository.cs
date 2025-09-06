using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;

namespace Kontecg.Organizations
{
    public class TemplateRepository : SGNOMRepositoryBase<Template>, ITemplateRepository
    {
        public TemplateRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
