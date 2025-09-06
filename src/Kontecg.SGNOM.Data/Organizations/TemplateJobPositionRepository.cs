using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.Extensions;
using Kontecg.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Kontecg.Organizations
{
    public class TemplateJobPositionRepository : SGNOMRepositoryBase<TemplateJobPosition>, ITemplateJobPositionRepository
    {
        public TemplateJobPositionRepository(IDbContextProvider<SGNOMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public IReadOnlyList<TemplateJobPosition> GetJobPositions(int documentId, string occupationCode, params long[] ouIds)
        {
            var query = GetAllIncluding(
                    t => t.Occupation,
                    t => t.Occupation.Group,
                    t => t.Occupation.Category,
                    t => t.Occupation.Responsibility,
                    t => t.Occupation.Requirements,
                    t => t.Template,
                    t => t.Template.Document,
                    t => t.ScholarshipLevel,
                    t => t.WorkShift,
                    t => t.Document,
                    t => t.TemporalDocument)
                .WhereIf(ouIds is { Length: > 0 }, e => ouIds.Contains(e.OrganizationUnitId))
                .WhereIf(!occupationCode.IsNullOrWhiteSpace(), e => e.Occupation.Code == occupationCode)
                .Where(t => t.Template.DocumentId == documentId);
            return query.ToList();
        }

        public async Task<IReadOnlyList<TemplateJobPosition>> GetJobPositionsAsync(int documentId, string occupationCode, params long[] ouIds)
        {
            var query = (await GetAllIncludingAsync(
                    t => t.Occupation,
                    t => t.Occupation.Group,
                    t => t.Occupation.Category,
                    t => t.Occupation.Responsibility,
                    t => t.Occupation.Requirements,
                    t => t.Template,
                    t => t.ScholarshipLevel,
                    t => t.WorkShift,
                    t => t.Document,
                    t => t.TemporalDocument))
                .WhereIf(ouIds is { Length: > 0 }, e => ouIds.Contains(e.OrganizationUnitId))
                .WhereIf(!occupationCode.IsNullOrWhiteSpace(), e => e.Occupation.Code == occupationCode)
                .Where(t => t.Template.DocumentId == documentId);
            return await query.ToListAsync();
        }

        public int TryToRemoveOrphans(int templateId, int countToRemove)
        {
            //RULE: Hay que implementar nuevamente pues existe integridad referencial asociada
            var templateJobPositionsToUpdate = GetAll()
                .Where(jp => jp.TemplateId == templateId && !jp.DocumentId.HasValue ).OrderByDescending(o => o.CreationTime);

            var ids = templateJobPositionsToUpdate.Take(countToRemove);
            return ids.Delete();
        }

        public async Task<int> TryToRemoveOrphansAsync(int templateId, int countToRemove)
        {
            //RULE: Hay que implementar nuevamente pues existe integridad referencial asociada
            var templateJobPositionsToUpdate = (await GetAllAsync())
                .Where(jp => jp.TemplateId == templateId && !jp.DocumentId.HasValue).OrderByDescending(o => o.CreationTime);

            var ids = templateJobPositionsToUpdate.Take(countToRemove);
            return await ids.DeleteAsync();
        }

        public void UpdateCodes()
        {
            var templateJobPositionsToUpdate =
                GetAllIncluding(t => t.Occupation.Group).ToArray()
                    .OrderBy(t => t.OrganizationUnitCode)
                    .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance)
                    .ToArray();

            for (int i = 0; i < templateJobPositionsToUpdate.Length; i++)
            {
                templateJobPositionsToUpdate[i].Code = (i + 1).ToString(new string('0', TemplateJobPosition.MaxCodeLength));
                Update(templateJobPositionsToUpdate[i]);
            }
        }

        public async Task UpdateCodesAsync()
        {
            var templateJobPositionsToUpdate =
                (await GetAllIncludingAsync(t => t.Occupation.Group)).ToArray()
                    .OrderBy(t => t.OrganizationUnitCode)
                    .ThenByDescending(t => t.Occupation.Group, ComplexityGroupComparer.Instance)
                    .ToArray();

            for (int i = 0; i < templateJobPositionsToUpdate.Length; i++)
            {
                templateJobPositionsToUpdate[i].Code = (i + 1).ToString(new string('0', TemplateJobPosition.MaxCodeLength));
                await UpdateAsync(templateJobPositionsToUpdate[i]);
            }
        }
    }
}
