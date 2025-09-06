using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Collections.Extensions;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.HumanResources;
using Kontecg.UI;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Organizations
{
    public class PersonOrganizationUnitRepository : KontecgRepositoryBase<PersonOrganizationUnit, long>,
        IPersonOrganizationUnitRepository
    {
        public PersonOrganizationUnitRepository(IDbContextProvider<KontecgCoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<PersonIdentifier>> GetAllPersonsInOrganizationUnitHierarchicalAsync(long[] organizationUnitIds)
        {
            if (organizationUnitIds.IsNullOrEmpty())
                return new List<PersonIdentifier>();

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

            var personIdQueryHierarchical = await context.PersonOrganizationUnits
                .Join(
                    context.OrganizationUnits.Where(predicate),
                    uo => uo.OrganizationUnitId,
                    ou => ou.Id,
                    (uo, ou) => new { uo.PersonId, uo.CompanyId }
                )
                .ToListAsync();

            return personIdQueryHierarchical
                .DistinctBy(x => x.PersonId)
                .Select(ou => new PersonIdentifier(ou.CompanyId, ou.PersonId))
                .ToList();
        }

        public async Task UpdatePersonToChangeTheirOrganizationUnitAsync(long organizationUnitId, long[] personIds)
        {
            if(personIds.IsNullOrEmpty())
                return;

            var context = await GetContextAsync();

            var selectedOrganizationUnitCode = await context.OrganizationUnits
                .Where(ou => organizationUnitId == ou.Id)
                .FirstOrDefaultAsync();

            if (selectedOrganizationUnitCode == null)
                throw new UserFriendlyException("Can not find an organization unit");

            var predicate = PredicateBuilder.New<Person>();
        }
    }
}
