using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Authorization.Users;
using Kontecg.Collections.Extensions;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.UI;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Organizations
{
    public class PersonRepository : KontecgRepositoryBase<UserOrganizationUnit, long>,
        IUserOrganizationUnitRepository
    {
        public PersonRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<UserIdentifier>> GetAllUsersInOrganizationUnitHierarchicalAsync(long[] organizationUnitIds)
        {
            if (organizationUnitIds.IsNullOrEmpty())
                return new List<UserIdentifier>();

            var context = await GetContextAsync();

            var selectedOrganizationUnitCodes = await context.OrganizationUnits
                .Where(ou => organizationUnitIds.Contains(ou.Id))
                .ToListAsync();

            if (selectedOrganizationUnitCodes == null)
                throw new UserFriendlyException("Can not find an organization unit");

            var predicate = PredicateBuilder.New<OrganizationUnit>();

            foreach (var selectedOrganizationUnitCode in selectedOrganizationUnitCodes)
            {
                predicate = predicate.Or(ou => ou.Code.StartsWith(selectedOrganizationUnitCode.Code));
            }

            var userIdQueryHierarchical = await context.UserOrganizationUnits
                .Join(
                    context.OrganizationUnits.Where(predicate),
                    uo => uo.OrganizationUnitId,
                    ou => ou.Id,
                    (uo, ou) => new {uo.UserId, uo.CompanyId}
                )
                .ToListAsync();

            return userIdQueryHierarchical
                .DistinctBy(x => x.UserId)
                .Select(ou => new UserIdentifier(ou.CompanyId, ou.UserId))
                .ToList();
        }
    }
}
