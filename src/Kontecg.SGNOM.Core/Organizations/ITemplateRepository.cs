using Kontecg.Domain.Repositories;
using System.Collections.Generic;

namespace Kontecg.Organizations
{
    public interface ITemplateRepository : IRepository<Template>
    {
        IReadOnlyList<OrganizationUnitAncestor> GetOrganizationUnitAncestor(long id, int level = -1);
    }
}
