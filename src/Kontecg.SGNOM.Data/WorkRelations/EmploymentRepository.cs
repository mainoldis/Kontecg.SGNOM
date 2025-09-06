using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using Kontecg.EFCore;
using Kontecg.EFCore.Repositories;
using Kontecg.Linq.Extensions;
using Kontecg.Timing;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Kontecg.WorkRelations
{
    public class EmploymentRepository : SGNOMRepositoryBase<EmploymentDocument, long>, IEmploymentRepository
    {
        public EmploymentRepository(IDbContextProvider<SGNOMDbContext> sgnomDbContextProvider)
            : base(sgnomDbContextProvider)
        {
        }

        /// <inheritdoc />
        public EmploymentDocument GetRecentAncestorForProvisional(long employmentId)
        {
            var query = from p in GetContext().GetEmploymentProvisionalAncestor(employmentId)
                        join e in GetQueryableEmploymentDocument() on p.Id equals e.Id
                        select e;
            return query.SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<EmploymentDocument> GetRecentAncestorForProvisionalAsync(long employmentId)
        {
            var query = from p in (await GetContextAsync()).GetEmploymentProvisionalAncestor(employmentId)
                        join e in await GetQueryableEmploymentDocumentAsync() on p.Id equals e.Id
                        select e;
            return await query.SingleOrDefaultAsync();
        }

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="personId">person id</param>
        /// <returns></returns>
        public IReadOnlyList<EmploymentDocument> LastByPersonId(long personId)
        {
            var query = GetQueryableEmploymentDocument()
                .Where(emp =>
                    emp.PersonId == personId &&
                    (emp.Review == ReviewStatus.Reviewed || emp.Review == ReviewStatus.Confirmed))
                .GroupBy(emp => emp.GroupId)
                .Select(g => new {EmploymentGroupId = g.Key, Last = g.OrderByDescending(e => e.Id).FirstOrDefault()})
                .ToList();

            return query.Select(l => l.Last).ToList();
        }

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="personId">person id</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<EmploymentDocument>> LastByPersonIdAsync(long personId)
        {
            var query = (await GetQueryableEmploymentDocumentAsync())
                .Where(emp =>
                    emp.PersonId == personId &&
                    (emp.Review == ReviewStatus.Reviewed || emp.Review == ReviewStatus.Confirmed))
                .GroupBy(emp => emp.GroupId)
                .Select(g => new { EmploymentGroupId = g.Key, Last = g.OrderByDescending(e => e.Id).FirstOrDefault() })
                .ToList();

            return query.Select(l => l.Last).ToList();
        }

        public IReadOnlyList<EmploymentDocument> LastByExp(int exp)
        {
            var contract = FirstOrDefault(e => e.Exp == exp);
            return contract != null ? LastByPersonId(contract.PersonId) : new List<EmploymentDocument>();
        }

        public async Task<IReadOnlyList<EmploymentDocument>> LastByExpAsync(int exp)
        {
            var contract = await FirstOrDefaultAsync(e => e.Exp == exp);
            return contract != null ? await LastByPersonIdAsync(contract.PersonId) : new List<EmploymentDocument>();
        }

        public IReadOnlyList<EmploymentDocument> RelationshipByPeriod(IFilterRequest inputFilterRequest)
        {
            var queryable = GetQueryableEmploymentDocument()
                            .WhereIf(inputFilterRequest.Exp.HasValue, e => e.Exp == inputFilterRequest.Exp)
                            .WhereIf(inputFilterRequest.WorkPlacePaymentCode != null, e => e.WorkPlacePaymentCode == inputFilterRequest.WorkPlacePaymentCode)
                            .WhereIf(inputFilterRequest.OrganizationUnitIds is { Length: > 0 }, e => inputFilterRequest.OrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .WhereIf(inputFilterRequest.ExcludedOrganizationUnitIds is { Length: > 0 }, e => !inputFilterRequest.ExcludedOrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .Where(e => e.Review == ReviewStatus.Confirmed);

            var result = queryable.OrderBy(inputFilterRequest.Sorting).PageBy(inputFilterRequest).ToList();

            if (inputFilterRequest is {Year: not null})
            {
                ITimePeriod period;
                if (inputFilterRequest.Month != null)
                    period = new Month(inputFilterRequest.Year.Value, inputFilterRequest.Month.Value);
                else
                    period = new Year(inputFilterRequest.Year.Value);
                return result.Where(e => period.IntersectsWith(e.Period)).ToList();
            }

            return result;
        }

        public async Task<IReadOnlyList<EmploymentDocument>> RelationshipByPeriodAsync(IFilterRequest inputFilterRequest)
        {
            var queryable = (await GetQueryableEmploymentDocumentAsync())
                            .WhereIf(inputFilterRequest.Exp.HasValue, e => e.Exp == inputFilterRequest.Exp)
                            .WhereIf(inputFilterRequest.WorkPlacePaymentCode != null, e => e.WorkPlacePaymentCode == inputFilterRequest.WorkPlacePaymentCode)
                            .WhereIf(inputFilterRequest.OrganizationUnitIds is { Length: > 0 }, e => inputFilterRequest.OrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .WhereIf(inputFilterRequest.ExcludedOrganizationUnitIds is { Length: > 0 }, e => !inputFilterRequest.ExcludedOrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .Where(e => e.Review == ReviewStatus.Confirmed);

            var result = await queryable.OrderBy(inputFilterRequest.Sorting).PageBy(inputFilterRequest).ToListAsync();

            if (inputFilterRequest is { Year: not null, Month: not null })
            {
                var period = new Month(inputFilterRequest.Year.Value, inputFilterRequest.Month.Value);
                return result.Where(e => period.IntersectsWith(e.Period)).ToList();
            }

            return result;
        }

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment, includes unemployed persons
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<EmploymentDocument> CurrentRelationship(IFilterRequest? inputFilterRequest)
        {
            inputFilterRequest ??= new FilterRequest();

            var queryable = GetQueryableEmploymentDocument()
                            .WhereIf(inputFilterRequest.Exp.HasValue, e => e.Exp == inputFilterRequest.Exp)
                            .WhereIf(inputFilterRequest.WorkPlacePaymentCode != null, e => e.WorkPlacePaymentCode == inputFilterRequest.WorkPlacePaymentCode)
                            .WhereIf(inputFilterRequest.OrganizationUnitIds is { Length: > 0 }, e => inputFilterRequest.OrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .WhereIf(inputFilterRequest.ExcludedOrganizationUnitIds is { Length: > 0 }, e => !inputFilterRequest.ExcludedOrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .Where(e => e.Review == ReviewStatus.Confirmed);

            var result = queryable.GroupBy(emp => emp.GroupId)
                                  .Select(g =>
                                      g.OrderByDescending(e => e.EffectiveSince).ThenByDescending(o => o.Code).First());
            return result.ToList();
        }

        public async Task<IReadOnlyList<EmploymentDocument>> CurrentRelationshipAsync(IFilterRequest? inputFilterRequest)
        {
            inputFilterRequest ??= new FilterRequest();

            var queryable = (await GetQueryableEmploymentDocumentAsync())
                            .WhereIf(inputFilterRequest.Exp.HasValue, e => e.Exp == inputFilterRequest.Exp)
                            .WhereIf(inputFilterRequest.WorkPlacePaymentCode != null, e => e.WorkPlacePaymentCode == inputFilterRequest.WorkPlacePaymentCode)
                            .WhereIf(inputFilterRequest.OrganizationUnitIds is { Length: > 0 }, e => inputFilterRequest.OrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .WhereIf(inputFilterRequest.ExcludedOrganizationUnitIds is { Length: > 0 }, e => !inputFilterRequest.ExcludedOrganizationUnitIds.Contains(e.OrganizationUnitId))
                            .Where(e => e.Review == ReviewStatus.Confirmed);

            var result = queryable.GroupBy(emp => emp.GroupId)
                                  .Select(g =>
                                      g.OrderByDescending(e => e.EffectiveSince).ThenByDescending(o => o.Code).First());

            return await result.ToListAsync();
        }

        public IQueryable<EmploymentDocument> GetQueryableEmploymentDocument()
        {
            return GetQueryable()
                .Include(e => e.WorkShift)
                .ThenInclude(w => w.Regime)
                .Include(e => e.Plus)
                .ThenInclude(p => p.PlusDefinition)
                .Include(e => e.Previous)
                .ThenInclude(e => e.Plus)
                .ThenInclude(p => p.PlusDefinition)
                .Include(e => e.Children)
                .Include(e => e.Summary)
                .ThenInclude(s => s.Parent);
        }

        public async Task<IQueryable<EmploymentDocument>> GetQueryableEmploymentDocumentAsync()
        {
            return (await GetQueryableAsync())
                .Include(e => e.WorkShift)
                .ThenInclude(w => w.Regime)
                .Include(e => e.Plus)
                .ThenInclude(p => p.PlusDefinition)
                .Include(e => e.Previous)
                .ThenInclude(e => e.Plus)
                .ThenInclude(p => p.PlusDefinition)
                .Include(e => e.Children)
                .Include(e => e.Summary)
                .ThenInclude(s => s.Parent);
        }

        public IReadOnlyList<EmploymentDocument> GetDocumentToExpireSoon()
        {
            var queryable = GetQueryableEmploymentDocument()
                            .Where(e => e.Review == ReviewStatus.Confirmed).GroupBy(emp => emp.GroupId)
                            .Select(g =>
                                g.OrderByDescending(e => e.EffectiveSince).ThenByDescending(o => o.Code).First())
                            .Where(e => e.EffectiveUntil <= new Month(Clock.Now).End && e.Type != EmploymentType.B);
            return queryable.ToList();
        }

        public async Task<IReadOnlyList<EmploymentDocument>> GetDocumentToExpireSoonAsync()
        {
            var queryable = (await GetQueryableEmploymentDocumentAsync())
                            .Where(e => e.Review == ReviewStatus.Confirmed).GroupBy(emp => emp.GroupId)
                            .Select(g =>
                                g.OrderByDescending(e => e.EffectiveSince).ThenByDescending(o => o.Code).First())
                            .Where(e => e.EffectiveUntil <= new Month(Clock.Now).End && e.Type != EmploymentType.B);
            return await queryable.ToListAsync();
        }

        /// <inheritdoc />
        public IReadOnlyList<EmploymentDocument> GetDocumentsToReview()
        {
            var queryable = GetQueryableEmploymentDocument();
            return queryable.Where(e => e.Review == ReviewStatus.ForReview).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<EmploymentDocument>> GetDocumentsToReviewAsync()
        {
            var queryable = await GetQueryableEmploymentDocumentAsync();
            return await queryable.Where(e => e.Review == ReviewStatus.ForReview).ToListAsync();
        }

        /// <inheritdoc />
        public IReadOnlyList<EmploymentDocument> GetDocumentsReviewed()
        {
            var queryable = GetQueryableEmploymentDocument();
            return queryable.Where(e => e.Review == ReviewStatus.Reviewed).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<EmploymentDocument>> GetDocumentsReviewedAsync()
        {
            var queryable = await GetQueryableEmploymentDocumentAsync();
            return await queryable.Where(e => e.Review == ReviewStatus.Reviewed).ToListAsync();
        }

        public void UpdateEmploymentsCenterCost(int centerCost, params long[] employmentIds)
        {
            if(employmentIds == null || employmentIds.Length == 0) return;

            GetAll() 
                .Where(employment => employmentIds.Contains(employment.Id))
                .Update(x => new EmploymentDocument {CenterCost = centerCost });
        }

        public async Task UpdateEmploymentsCenterCostAsync(int centerCost, params long[]? employmentIds)
        {
            if (employmentIds == null || employmentIds.Length == 0) return;

            await (await GetAllAsync())
                .Where(employment => employmentIds.Contains(employment.Id))
                .UpdateAsync(x => new EmploymentDocument { CenterCost = centerCost });
        }

        public void UpdateEmploymentsWorkPlace(long workPlaceUnitId, params long[]? employmentIds)
        {
            if (employmentIds == null || employmentIds.Length == 0) return;

            GetAll()
                .Where(employment => employmentIds.Contains(employment.Id))
                .Update(x => new EmploymentDocument { OrganizationUnitId = workPlaceUnitId });
        }

        public async Task UpdateEmploymentsWorkPlaceAsync(long workPlaceUnitId, params long[]? employmentIds)
        {
            if (employmentIds == null || employmentIds.Length == 0) return;

            await (await GetAllAsync())
                .Where(employment => employmentIds.Contains(employment.Id))
                .UpdateAsync(x => new EmploymentDocument { OrganizationUnitId = workPlaceUnitId });
        }

        public void UpdateReviewStatus(ReviewStatus status, params long[]? employmentIds)
        {
            if (employmentIds == null || employmentIds.Length == 0) return;

            GetAll()
                  .Where(employment => employmentIds.Contains(employment.Id))
                  .Update(x => new EmploymentDocument { Review = status });
        }

        public async Task UpdateReviewStatusAsync(ReviewStatus status, params long[]? employmentIds)
        {
            if (employmentIds == null || employmentIds.Length == 0) return;

            await (await GetAllAsync())
                  .Where(employment => employmentIds.Contains(employment.Id))
                  .UpdateAsync(x => new EmploymentDocument { Review = status });
        }
    }
}
