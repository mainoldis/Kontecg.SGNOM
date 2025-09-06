using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.HumanResources;

namespace Kontecg.Organizations
{
    public interface IPersonOrganizationUnitRepository : IRepository<PersonOrganizationUnit, long>
    {
        Task<List<PersonIdentifier>> GetAllPersonsInOrganizationUnitHierarchicalAsync(long[] organizationUnitIds);

        Task UpdatePersonToChangeTheirOrganizationUnitAsync(long organizationUnitId, long[] personIds);
    }
}
