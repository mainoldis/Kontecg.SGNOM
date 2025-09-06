using Kontecg.Authorization.Users;
using Kontecg.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Organizations
{
    public interface IUserOrganizationUnitRepository : IRepository<UserOrganizationUnit, long>
    {
        Task<List<UserIdentifier>> GetAllUsersInOrganizationUnitHierarchicalAsync(long[] organizationUnitIds);
    }
}
